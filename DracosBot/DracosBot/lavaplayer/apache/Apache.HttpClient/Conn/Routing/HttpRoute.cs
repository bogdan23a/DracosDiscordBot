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
using System.Net;
using System.Text;
using Apache.Http;
using Apache.Http.Conn.Routing;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Conn.Routing
{
	/// <summary>The route for a request.</summary>
	/// <remarks>The route for a request.</remarks>
	/// <since>4.0</since>
	public sealed class HttpRoute : RouteInfo, ICloneable
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

		/// <summary>The proxy servers, if any.</summary>
		/// <remarks>The proxy servers, if any. Never null.</remarks>
		private readonly IList<HttpHost> proxyChain;

		/// <summary>Whether the the route is tunnelled through the proxy.</summary>
		/// <remarks>Whether the the route is tunnelled through the proxy.</remarks>
		private readonly RouteInfo.TunnelType tunnelled;

		/// <summary>Whether the route is layered.</summary>
		/// <remarks>Whether the route is layered.</remarks>
		private readonly RouteInfo.LayerType layered;

		/// <summary>Whether the route is (supposed to be) secure.</summary>
		/// <remarks>Whether the route is (supposed to be) secure.</remarks>
		private readonly bool secure;

		private HttpRoute(HttpHost target, IPAddress local, IList<HttpHost> proxies, bool
			 secure, RouteInfo.TunnelType tunnelled, RouteInfo.LayerType layered)
		{
			Args.NotNull(target, "Target host");
			this.targetHost = target;
			this.localAddress = local;
			if (proxies != null && !proxies.IsEmpty())
			{
				this.proxyChain = new AList<HttpHost>(proxies);
			}
			else
			{
				this.proxyChain = null;
			}
			if (tunnelled == RouteInfo.TunnelType.Tunnelled)
			{
				Args.Check(this.proxyChain != null, "Proxy required if tunnelled");
			}
			this.secure = secure;
			this.tunnelled = tunnelled != null ? tunnelled : RouteInfo.TunnelType.Plain;
			this.layered = layered != null ? layered : RouteInfo.LayerType.Plain;
		}

		/// <summary>Creates a new route with all attributes specified explicitly.</summary>
		/// <remarks>Creates a new route with all attributes specified explicitly.</remarks>
		/// <param name="target">the host to which to route</param>
		/// <param name="local">
		/// the local address to route from, or
		/// <code>null</code> for the default
		/// </param>
		/// <param name="proxies">
		/// the proxy chain to use, or
		/// <code>null</code> for a direct route
		/// </param>
		/// <param name="secure">
		/// <code>true</code> if the route is (to be) secure,
		/// <code>false</code> otherwise
		/// </param>
		/// <param name="tunnelled">the tunnel type of this route</param>
		/// <param name="layered">the layering type of this route</param>
		public HttpRoute(HttpHost target, IPAddress local, HttpHost[] proxies, bool secure
			, RouteInfo.TunnelType tunnelled, RouteInfo.LayerType layered) : this(target, local
			, proxies != null ? Arrays.AsList(proxies) : null, secure, tunnelled, layered)
		{
		}

		/// <summary>Creates a new route with at most one proxy.</summary>
		/// <remarks>Creates a new route with at most one proxy.</remarks>
		/// <param name="target">the host to which to route</param>
		/// <param name="local">
		/// the local address to route from, or
		/// <code>null</code> for the default
		/// </param>
		/// <param name="proxy">
		/// the proxy to use, or
		/// <code>null</code> for a direct route
		/// </param>
		/// <param name="secure">
		/// <code>true</code> if the route is (to be) secure,
		/// <code>false</code> otherwise
		/// </param>
		/// <param name="tunnelled">
		/// <code>true</code> if the route is (to be) tunnelled
		/// via the proxy,
		/// <code>false</code> otherwise
		/// </param>
		/// <param name="layered">
		/// <code>true</code> if the route includes a
		/// layered protocol,
		/// <code>false</code> otherwise
		/// </param>
		public HttpRoute(HttpHost target, IPAddress local, HttpHost proxy, bool secure, RouteInfo.TunnelType
			 tunnelled, RouteInfo.LayerType layered) : this(target, local, proxy != null ? Sharpen.Collections
			.SingletonList(proxy) : null, secure, tunnelled, layered)
		{
		}

		/// <summary>Creates a new direct route.</summary>
		/// <remarks>
		/// Creates a new direct route.
		/// That is a route without a proxy.
		/// </remarks>
		/// <param name="target">the host to which to route</param>
		/// <param name="local">
		/// the local address to route from, or
		/// <code>null</code> for the default
		/// </param>
		/// <param name="secure">
		/// <code>true</code> if the route is (to be) secure,
		/// <code>false</code> otherwise
		/// </param>
		public HttpRoute(HttpHost target, IPAddress local, bool secure) : this(target, local
			, Sharpen.Collections.EmptyList<HttpHost>(), secure, RouteInfo.TunnelType.Plain, 
			RouteInfo.LayerType.Plain)
		{
		}

		/// <summary>Creates a new direct insecure route.</summary>
		/// <remarks>Creates a new direct insecure route.</remarks>
		/// <param name="target">the host to which to route</param>
		public HttpRoute(HttpHost target) : this(target, null, Sharpen.Collections.EmptyList
			<HttpHost>(), false, RouteInfo.TunnelType.Plain, RouteInfo.LayerType.Plain)
		{
		}

		/// <summary>Creates a new route through a proxy.</summary>
		/// <remarks>
		/// Creates a new route through a proxy.
		/// When using this constructor, the <code>proxy</code> MUST be given.
		/// For convenience, it is assumed that a secure connection will be
		/// layered over a tunnel through the proxy.
		/// </remarks>
		/// <param name="target">the host to which to route</param>
		/// <param name="local">
		/// the local address to route from, or
		/// <code>null</code> for the default
		/// </param>
		/// <param name="proxy">the proxy to use</param>
		/// <param name="secure">
		/// <code>true</code> if the route is (to be) secure,
		/// <code>false</code> otherwise
		/// </param>
		public HttpRoute(HttpHost target, IPAddress local, HttpHost proxy, bool secure) : 
			this(target, local, Sharpen.Collections.SingletonList(Args.NotNull(proxy, "Proxy host"
			)), secure, secure ? RouteInfo.TunnelType.Tunnelled : RouteInfo.TunnelType.Plain
			, secure ? RouteInfo.LayerType.Layered : RouteInfo.LayerType.Plain)
		{
		}

		/// <summary>Creates a new plain route through a proxy.</summary>
		/// <remarks>Creates a new plain route through a proxy.</remarks>
		/// <param name="target">the host to which to route</param>
		/// <param name="proxy">the proxy to use</param>
		/// <since>4.3</since>
		public HttpRoute(HttpHost target, HttpHost proxy) : this(target, null, proxy, false
			)
		{
		}

		public HttpHost GetTargetHost()
		{
			return this.targetHost;
		}

		public IPAddress GetLocalAddress()
		{
			return this.localAddress;
		}

		public IPEndPoint GetLocalSocketAddress()
		{
			return this.localAddress != null ? new IPEndPoint(this.localAddress, 0) : null;
		}

		public int GetHopCount()
		{
			return proxyChain != null ? proxyChain.Count + 1 : 1;
		}

		public HttpHost GetHopTarget(int hop)
		{
			Args.NotNegative(hop, "Hop index");
			int hopcount = GetHopCount();
			Args.Check(hop < hopcount, "Hop index exceeds tracked route length");
			if (hop < hopcount - 1)
			{
				return this.proxyChain[hop];
			}
			else
			{
				return this.targetHost;
			}
		}

		public HttpHost GetProxyHost()
		{
			return proxyChain != null && !this.proxyChain.IsEmpty() ? this.proxyChain[0] : null;
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

		/// <summary>Compares this route to another.</summary>
		/// <remarks>Compares this route to another.</remarks>
		/// <param name="obj">the object to compare with</param>
		/// <returns>
		/// <code>true</code> if the argument is the same route,
		/// <code>false</code>
		/// </returns>
		public sealed override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is Apache.Http.Conn.Routing.HttpRoute)
			{
				Apache.Http.Conn.Routing.HttpRoute that = (Apache.Http.Conn.Routing.HttpRoute)obj;
				return (this.secure == that.secure) && (this.tunnelled == that.tunnelled) && (this
					.layered == that.layered) && LangUtils.Equals(this.targetHost, that.targetHost) 
					&& LangUtils.Equals(this.localAddress, that.localAddress) && LangUtils.Equals(this
					.proxyChain, that.proxyChain);
			}
			else
			{
				// Do the cheapest tests first
				return false;
			}
		}

		/// <summary>Generates a hash code for this route.</summary>
		/// <remarks>Generates a hash code for this route.</remarks>
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
			hash = LangUtils.HashCode(hash, this.secure);
			hash = LangUtils.HashCode(hash, this.tunnelled);
			hash = LangUtils.HashCode(hash, this.layered);
			return hash;
		}

		/// <summary>Obtains a description of this route.</summary>
		/// <remarks>Obtains a description of this route.</remarks>
		/// <returns>a human-readable representation of this route</returns>
		public sealed override string ToString()
		{
			StringBuilder cab = new StringBuilder(50 + GetHopCount() * 30);
			if (this.localAddress != null)
			{
				cab.Append(this.localAddress);
				cab.Append("->");
			}
			cab.Append('{');
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
				foreach (HttpHost aProxyChain in this.proxyChain)
				{
					cab.Append(aProxyChain);
					cab.Append("->");
				}
			}
			cab.Append(this.targetHost);
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
