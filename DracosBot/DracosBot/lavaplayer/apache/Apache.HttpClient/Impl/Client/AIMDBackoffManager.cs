/**
 * Couchbase Lite for .NET
 *
 * Original iOS version by Jens Alfke
 * Android Port by Marty Schoch, Traun Leyden
 * C# Port by Zack Gramana
 *
 * Copyright (c) 2012, 2013 Couchbase, Inc. All rights reserved.
 * Portions (c) 2013 Xamarin, Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the
 * License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
 * either express or implied. See the License for the specific language governing permissions
 * and limitations under the License.
 */

using System;
using System.Collections.Generic;
using Apache.Http.Client;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Client;
using Apache.Http.Pool;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// <p>The <code>AIMDBackoffManager</code> applies an additive increase,
	/// multiplicative decrease (AIMD) to managing a dynamic limit to
	/// the number of connections allowed to a given host.
	/// </summary>
	/// <remarks>
	/// <p>The <code>AIMDBackoffManager</code> applies an additive increase,
	/// multiplicative decrease (AIMD) to managing a dynamic limit to
	/// the number of connections allowed to a given host. You may want
	/// to experiment with the settings for the cooldown periods and the
	/// backoff factor to get the adaptive behavior you want.</p>
	/// <p>Generally speaking, shorter cooldowns will lead to more steady-state
	/// variability but faster reaction times, while longer cooldowns
	/// will lead to more stable equilibrium behavior but slower reaction
	/// times.</p>
	/// <p>Similarly, higher backoff factors promote greater
	/// utilization of available capacity at the expense of fairness
	/// among clients. Lower backoff factors allow equal distribution of
	/// capacity among clients (fairness) to happen faster, at the
	/// expense of having more server capacity unused in the short term.</p>
	/// </remarks>
	/// <since>4.2</since>
	public class AIMDBackoffManager : BackoffManager
	{
		private readonly ConnPoolControl<HttpRoute> connPerRoute;

		private readonly Clock clock;

		private readonly IDictionary<HttpRoute, long> lastRouteProbes;

		private readonly IDictionary<HttpRoute, long> lastRouteBackoffs;

		private long coolDown = 5 * 1000L;

		private double backoffFactor = 0.5;

		private int cap = 2;

		/// <summary>
		/// Creates an <code>AIMDBackoffManager</code> to manage
		/// per-host connection pool sizes represented by the
		/// given
		/// <see cref="Apache.Http.Pool.ConnPoolControl{T}">Apache.Http.Pool.ConnPoolControl&lt;T&gt;
		/// 	</see>
		/// .
		/// </summary>
		/// <param name="connPerRoute">
		/// per-host routing maximums to
		/// be managed
		/// </param>
		public AIMDBackoffManager(ConnPoolControl<HttpRoute> connPerRoute) : this(connPerRoute
			, new SystemClock())
		{
		}

		internal AIMDBackoffManager(ConnPoolControl<HttpRoute> connPerRoute, Clock clock)
		{
			// Per RFC 2616 sec 8.1.4
			this.clock = clock;
			this.connPerRoute = connPerRoute;
			this.lastRouteProbes = new Dictionary<HttpRoute, long>();
			this.lastRouteBackoffs = new Dictionary<HttpRoute, long>();
		}

		public virtual void BackOff(HttpRoute route)
		{
			lock (connPerRoute)
			{
				int curr = connPerRoute.GetMaxPerRoute(route);
				long lastUpdate = GetLastUpdate(lastRouteBackoffs, route);
				long now = clock.GetCurrentTime();
				if (now - lastUpdate < coolDown)
				{
					return;
				}
				connPerRoute.SetMaxPerRoute(route, GetBackedOffPoolSize(curr));
				lastRouteBackoffs.Put(route, Sharpen.Extensions.ValueOf(now));
			}
		}

		private int GetBackedOffPoolSize(int curr)
		{
			if (curr <= 1)
			{
				return 1;
			}
			return (int)(Math.Floor(backoffFactor * curr));
		}

		public virtual void Probe(HttpRoute route)
		{
			lock (connPerRoute)
			{
				int curr = connPerRoute.GetMaxPerRoute(route);
				int max = (curr >= cap) ? cap : curr + 1;
				long lastProbe = GetLastUpdate(lastRouteProbes, route);
				long lastBackoff = GetLastUpdate(lastRouteBackoffs, route);
				long now = clock.GetCurrentTime();
				if (now - lastProbe < coolDown || now - lastBackoff < coolDown)
				{
					return;
				}
				connPerRoute.SetMaxPerRoute(route, max);
				lastRouteProbes.Put(route, Sharpen.Extensions.ValueOf(now));
			}
		}

		private long GetLastUpdate(IDictionary<HttpRoute, long> updates, HttpRoute route)
		{
			long lastUpdate = updates.Get(route);
			if (lastUpdate == null)
			{
				lastUpdate = Sharpen.Extensions.ValueOf(0L);
			}
			return lastUpdate;
		}

		/// <summary>
		/// Sets the factor to use when backing off; the new
		/// per-host limit will be roughly the current max times
		/// this factor.
		/// </summary>
		/// <remarks>
		/// Sets the factor to use when backing off; the new
		/// per-host limit will be roughly the current max times
		/// this factor. <code>Math.floor</code> is applied in the
		/// case of non-integer outcomes to ensure we actually
		/// decrease the pool size. Pool sizes are never decreased
		/// below 1, however. Defaults to 0.5.
		/// </remarks>
		/// <param name="d">must be between 0.0 and 1.0, exclusive.</param>
		public virtual void SetBackoffFactor(double d)
		{
			Args.Check(d > 0.0 && d < 1.0, "Backoff factor must be 0.0 < f < 1.0");
			backoffFactor = d;
		}

		/// <summary>
		/// Sets the amount of time, in milliseconds, to wait between
		/// adjustments in pool sizes for a given host, to allow
		/// enough time for the adjustments to take effect.
		/// </summary>
		/// <remarks>
		/// Sets the amount of time, in milliseconds, to wait between
		/// adjustments in pool sizes for a given host, to allow
		/// enough time for the adjustments to take effect. Defaults
		/// to 5000L (5 seconds).
		/// </remarks>
		/// <param name="l">must be positive</param>
		public virtual void SetCooldownMillis(long l)
		{
			Args.Positive(coolDown, "Cool down");
			coolDown = l;
		}

		/// <summary>
		/// Sets the absolute maximum per-host connection pool size to
		/// probe up to; defaults to 2 (the default per-host max).
		/// </summary>
		/// <remarks>
		/// Sets the absolute maximum per-host connection pool size to
		/// probe up to; defaults to 2 (the default per-host max).
		/// </remarks>
		/// <param name="cap">must be &gt;= 1</param>
		public virtual void SetPerHostConnectionCap(int cap)
		{
			Args.Positive(cap, "Per host connection cap");
			this.cap = cap;
		}
	}
}
