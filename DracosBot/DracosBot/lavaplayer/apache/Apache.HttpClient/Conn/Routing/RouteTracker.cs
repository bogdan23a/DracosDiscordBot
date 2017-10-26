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
using System.Net;
using System.Text;
using Apache.Http;
using Apache.Http.Conn.Routing;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Conn.Routing
{
	/// <summary>Helps tracking the steps in establishing a route.</summary>
	/// <remarks>Helps tracking the steps in establishing a route.</remarks>
	/// <since>4.0</since>
	public sealed class RouteTracker : RouteInfo, ICloneable
	{
		/// <summary>The target host to connect to.</summary>
		/// <remarks>The target host to connect to.</remarks>
		private readonly HttpHost targetHost;

		/// <summary>The local address to connect from.</summary>
		/// <remarks>
		/// The local address to connect from.
		/// <code>null</code> indicates that the default should be used.
		/// </remarks>
		private readonly IPAddress localAddress;

		/// <summary>Whether the first hop of the route is established.</summary>
		/// <remarks>Whether the first hop of the route is established.</remarks>
		private bool connected;

		/// <summary>The proxy chain, if any.</summary>
		/// <remarks>The proxy chain, if any.</remarks>
		private HttpHost[] proxyChain;

		/// <summary>Whether the the route is tunnelled end-to-end through proxies.</summary>
		/// <remarks>Whether the the route is tunnelled end-to-end through proxies.</remarks>
		private RouteInfo.TunnelType tunnelled;

		/// <summary>Whether the route is layered over a tunnel.</summary>
		/// <remarks>Whether the route is layered over a tunnel.</remarks>
		private RouteInfo.LayerType layered;

		/// <summary>Whether the route is secure.</summary>
		/// <remarks>Whether the route is secure.</remarks>
		private bool secure;

		/// <summary>Creates a new route tracker.</summary>
		/// <remarks>
		/// Creates a new route tracker.
		/// The target and origin need to be specified at creation time.
		/// </remarks>
		/// <param name="target">the host to which to route</param>
		/// <param name="local">
		/// the local address to route from, or
		/// <code>null</code> for the default
		/// </param>
		public RouteTracker(HttpHost target, IPAddress local)
		{
			// the attributes above are fixed at construction time
			// now follow attributes that indicate the established route
			Args.NotNull(target, "Target host");
			this.targetHost = target;
			this.localAddress = local;
			this.tunnelled = RouteInfo.TunnelType.Plain;
			this.layered = RouteInfo.LayerType.Plain;
		}

		/// <since>4.2</since>
		public void Reset()
		{
			this.connected = false;
			this.proxyChain = null;
			this.tunnelled = RouteInfo.TunnelType.Plain;
			this.layered = RouteInfo.LayerType.Plain;
			this.secure = false;
		}

		/// <summary>Creates a new tracker for the given route.</summary>
		/// <remarks>
		/// Creates a new tracker for the given route.
		/// Only target and origin are taken from the route,
		/// everything else remains to be tracked.
		/// </remarks>
		/// <param name="route">the route to track</param>
		public RouteTracker(HttpRoute route) : this(route.GetTargetHost(), route.GetLocalAddress
			())
		{
		}

		/// <summary>Tracks connecting to the target.</summary>
		/// <remarks>Tracks connecting to the target.</remarks>
		/// <param name="secure">
		/// <code>true</code> if the route is secure,
		/// <code>false</code> otherwise
		/// </param>
		public void ConnectTarget(bool secure)
		{
			Asserts.Check(!this.connected, "Already connected");
			this.connected = true;
			this.secure = secure;
		}

		/// <summary>Tracks connecting to the first proxy.</summary>
		/// <remarks>Tracks connecting to the first proxy.</remarks>
		/// <param name="proxy">the proxy connected to</param>
		/// <param name="secure">
		/// <code>true</code> if the route is secure,
		/// <code>false</code> otherwise
		/// </param>
		public void ConnectProxy(HttpHost proxy, bool secure)
		{
			Args.NotNull(proxy, "Proxy host");
			Asserts.Check(!this.connected, "Already connected");
			this.connected = true;
			this.proxyChain = new HttpHost[] { proxy };
			this.secure = secure;
		}

		/// <summary>Tracks tunnelling to the target.</summary>
		/// <remarks>Tracks tunnelling to the target.</remarks>
		/// <param name="secure">
		/// <code>true</code> if the route is secure,
		/// <code>false</code> otherwise
		/// </param>
		public void TunnelTarget(bool secure)
		{
			Asserts.Check(this.connected, "No tunnel unless connected");
			Asserts.NotNull(this.proxyChain, "No tunnel without proxy");
			this.tunnelled = RouteInfo.TunnelType.Tunnelled;
			this.secure = secure;
		}

		/// <summary>Tracks tunnelling to a proxy in a proxy chain.</summary>
		/// <remarks>
		/// Tracks tunnelling to a proxy in a proxy chain.
		/// This will extend the tracked proxy chain, but it does not mark
		/// the route as tunnelled. Only end-to-end tunnels are considered there.
		/// </remarks>
		/// <param name="proxy">the proxy tunnelled to</param>
		/// <param name="secure">
		/// <code>true</code> if the route is secure,
		/// <code>false</code> otherwise
		/// </param>
		public void TunnelProxy(HttpHost proxy, bool secure)
		{
			Args.NotNull(proxy, "Proxy host");
			Asserts.Check(this.connected, "No tunnel unless connected");
			Asserts.NotNull(this.proxyChain, "No tunnel without proxy");
			// prepare an extended proxy chain
			HttpHost[] proxies = new HttpHost[this.proxyChain.Length + 1];
			System.Array.Copy(this.proxyChain, 0, proxies, 0, this.proxyChain.Length);
			proxies[proxies.Length - 1] = proxy;
			this.proxyChain = proxies;
			this.secure = secure;
		}

		/// <summary>Tracks layering a protocol.</summary>
		/// <remarks>Tracks layering a protocol.</remarks>
		/// <param name="secure">
		/// <code>true</code> if the route is secure,
		/// <code>false</code> otherwise
		/// </param>
		public void LayerProtocol(bool secure)
		{
			// it is possible to layer a protocol over a direct connection,
			// although this case is probably not considered elsewhere
			Asserts.Check(this.connected, "No layered protocol unless connected");
			this.layered = RouteInfo.LayerType.Layered;
			this.secure = secure;
		}

		public HttpHost GetTargetHost()
		{
			return this.targetHost;
		}

		public IPAddress GetLocalAddress()
		{
			return this.localAddress;
		}

		public int GetHopCount()
		{
			int hops = 0;
			if (this.connected)
			{
				if (proxyChain == null)
				{
					hops = 1;
				}
				else
				{
					hops = proxyChain.Length + 1;
				}
			}
			return hops;
		}

		public HttpHost GetHopTarget(int hop)
		{
			Args.NotNegative(hop, "Hop index");
			int hopcount = GetHopCount();
			Args.Check(hop < hopcount, "Hop index exceeds tracked route length");
			HttpHost result = null;
			if (hop < hopcount - 1)
			{
				result = this.proxyChain[hop];
			}
			else
			{
				result = this.targetHost;
			}
			return result;
		}

		public HttpHost GetProxyHost()
		{
			return (this.proxyChain == null) ? null : this.proxyChain[0];
		}

		public bool IsConnected()
		{
			return this.connected;
		}

		public RouteInfo.TunnelType GetTunnelType()
		{
			return this.tunnelled;
		}

		public bool IsTunnelled()
		{
			return (this.tunnelled == RouteInfo.TunnelType.Tunnelled);
		}

		public RouteInfo.LayerType GetLayerType()
		{
			return this.layered;
		}

		public bool IsLayered()
		{
			return (this.layered == RouteInfo.LayerType.Layered);
		}

		public bool IsSecure()
		{
			return this.secure;
		}

		/// <summary>Obtains the tracked route.</summary>
		/// <remarks>
		/// Obtains the tracked route.
		/// If a route has been tracked, it is
		/// <see cref="IsConnected()">connected</see>
		/// .
		/// If not connected, nothing has been tracked so far.
		/// </remarks>
		/// <returns>
		/// the tracked route, or
		/// <code>null</code> if nothing has been tracked so far
		/// </returns>
		public HttpRoute ToRoute()
		{
			return !this.connected ? null : new HttpRoute(this.targetHost, this.localAddress, 
				this.proxyChain, this.secure, this.tunnelled, this.layered);
		}

		/// <summary>Compares this tracked route to another.</summary>
		/// <remarks>Compares this tracked route to another.</remarks>
		/// <param name="o">the object to compare with</param>
		/// <returns>
		/// <code>true</code> if the argument is the same tracked route,
		/// <code>false</code>
		/// </returns>
		public sealed override bool Equals(object o)
		{
			if (o == this)
			{
				return true;
			}
			if (!(o is Apache.Http.Conn.Routing.RouteTracker))
			{
				return false;
			}
			Apache.Http.Conn.Routing.RouteTracker that = (Apache.Http.Conn.Routing.RouteTracker
				)o;
			return (this.connected == that.connected) && (this.secure == that.secure) && (this
				.tunnelled == that.tunnelled) && (this.layered == that.layered) && LangUtils.Equals
				(this.targetHost, that.targetHost) && LangUtils.Equals(this.localAddress, that.localAddress
				) && LangUtils.Equals(this.proxyChain, that.proxyChain);
		}

		// Do the cheapest checks first
		/// <summary>Generates a hash code for this tracked route.</summary>
		/// <remarks>
		/// Generates a hash code for this tracked route.
		/// Route trackers are modifiable and should therefore not be used
		/// as lookup keys. Use
		/// <see cref="ToRoute()">toRoute</see>
		/// to obtain an
		/// unmodifiable representation of the tracked route.
		/// </remarks>
		/// <returns>the hash code</returns>
		public sealed override int GetHashCode()
		{
			int hash = LangUtils.HashSeed;
			hash = LangUtils.HashCode(hash, this.targetHost);
			hash = LangUtils.HashCode(hash, this.localAddress);
			if (this.proxyChain != null)
			{
				foreach (HttpHost element in this.proxyChain)
				{
					hash = LangUtils.HashCode(hash, element);
				}
			}
			hash = LangUtils.HashCode(hash, this.connected);
			hash = LangUtils.HashCode(hash, this.secure);
			hash = LangUtils.HashCode(hash, this.tunnelled);
			hash = LangUtils.HashCode(hash, this.layered);
			return hash;
		}

		/// <summary>Obtains a description of the tracked route.</summary>
		/// <remarks>Obtains a description of the tracked route.</remarks>
		/// <returns>a human-readable representation of the tracked route</returns>
		public sealed override string ToString()
		{
			StringBuilder cab = new StringBuilder(50 + GetHopCount() * 30);
			cab.Append("RouteTracker[");
			if (this.localAddress != null)
			{
				cab.Append(this.localAddress);
				cab.Append("->");
			}
			cab.Append('{');
			if (this.connected)
			{
				cab.Append('c');
			}
			if (this.tunnelled == RouteInfo.TunnelType.Tunnelled)
			{
				cab.Append('t');
			}
			if (this.layered == RouteInfo.LayerType.Layered)
			{
				cab.Append('l');
			}
			if (this.secure)
			{
				cab.Append('s');
			}
			cab.Append("}->");
			if (this.proxyChain != null)
			{
				foreach (HttpHost element in this.proxyChain)
				{
					cab.Append(element);
					cab.Append("->");
				}
			}
			cab.Append(this.targetHost);
			cab.Append(']');
			return cab.ToString();
		}

		// default implementation of clone() is sufficient
		/// <exception cref="Sharpen.CloneNotSupportedException"></exception>
		public object Clone()
		{
			return base.Clone();
		}
	}
}
