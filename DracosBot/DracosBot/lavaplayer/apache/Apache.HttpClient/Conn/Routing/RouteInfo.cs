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

using System.Net;
using Apache.Http;
using Apache.Http.Conn.Routing;
using Sharpen;

namespace Apache.Http.Conn.Routing
{
	/// <summary>Read-only interface for route information.</summary>
	/// <remarks>Read-only interface for route information.</remarks>
	/// <since>4.0</since>
	public abstract class RouteInfo
	{
		/// <summary>The tunnelling type of a route.</summary>
		/// <remarks>
		/// The tunnelling type of a route.
		/// Plain routes are established by   connecting to the target or
		/// the first proxy.
		/// Tunnelled routes are established by connecting to the first proxy
		/// and tunnelling through all proxies to the target.
		/// Routes without a proxy cannot be tunnelled.
		/// </remarks>
		public enum TunnelType
		{
			Plain,
			Tunnelled
		}

		/// <summary>The layering type of a route.</summary>
		/// <remarks>
		/// The layering type of a route.
		/// Plain routes are established by connecting or tunnelling.
		/// Layered routes are established by layering a protocol such as TLS/SSL
		/// over an existing connection.
		/// Protocols can only be layered over a tunnel to the target, or
		/// or over a direct connection without proxies.
		/// <br/>
		/// Layering a protocol
		/// over a direct connection makes little sense, since the connection
		/// could be established with the new protocol in the first place.
		/// But we don't want to exclude that use case.
		/// </remarks>
		public enum LayerType
		{
			Plain,
			Layered
		}

		/// <summary>Obtains the target host.</summary>
		/// <remarks>Obtains the target host.</remarks>
		/// <returns>the target host</returns>
		public abstract HttpHost GetTargetHost();

		/// <summary>Obtains the local address to connect from.</summary>
		/// <remarks>Obtains the local address to connect from.</remarks>
		/// <returns>
		/// the local address,
		/// or <code>null</code>
		/// </returns>
		public abstract IPAddress GetLocalAddress();

		/// <summary>Obtains the number of hops in this route.</summary>
		/// <remarks>
		/// Obtains the number of hops in this route.
		/// A direct route has one hop. A route through a proxy has two hops.
		/// A route through a chain of <i>n</i> proxies has <i>n+1</i> hops.
		/// </remarks>
		/// <returns>the number of hops in this route</returns>
		public abstract int GetHopCount();

		/// <summary>Obtains the target of a hop in this route.</summary>
		/// <remarks>
		/// Obtains the target of a hop in this route.
		/// The target of the last hop is the
		/// <see cref="GetTargetHost()">target host</see>
		/// ,
		/// the target of previous hops is the respective proxy in the chain.
		/// For a route through exactly one proxy, target of hop 0 is the proxy
		/// and target of hop 1 is the target host.
		/// </remarks>
		/// <param name="hop">
		/// index of the hop for which to get the target,
		/// 0 for first
		/// </param>
		/// <returns>the target of the given hop</returns>
		/// <exception cref="System.ArgumentException">
		/// if the argument is negative or not less than
		/// <see cref="GetHopCount()">getHopCount()</see>
		/// </exception>
		public abstract HttpHost GetHopTarget(int hop);

		/// <summary>Obtains the first proxy host.</summary>
		/// <remarks>Obtains the first proxy host.</remarks>
		/// <returns>
		/// the first proxy in the proxy chain, or
		/// <code>null</code> if this route is direct
		/// </returns>
		public abstract HttpHost GetProxyHost();

		/// <summary>Obtains the tunnel type of this route.</summary>
		/// <remarks>
		/// Obtains the tunnel type of this route.
		/// If there is a proxy chain, only end-to-end tunnels are considered.
		/// </remarks>
		/// <returns>the tunnelling type</returns>
		public abstract RouteInfo.TunnelType GetTunnelType();

		/// <summary>Checks whether this route is tunnelled through a proxy.</summary>
		/// <remarks>
		/// Checks whether this route is tunnelled through a proxy.
		/// If there is a proxy chain, only end-to-end tunnels are considered.
		/// </remarks>
		/// <returns>
		/// <code>true</code> if tunnelled end-to-end through at least
		/// one proxy,
		/// <code>false</code> otherwise
		/// </returns>
		public abstract bool IsTunnelled();

		/// <summary>Obtains the layering type of this route.</summary>
		/// <remarks>
		/// Obtains the layering type of this route.
		/// In the presence of proxies, only layering over an end-to-end tunnel
		/// is considered.
		/// </remarks>
		/// <returns>the layering type</returns>
		public abstract RouteInfo.LayerType GetLayerType();

		/// <summary>Checks whether this route includes a layered protocol.</summary>
		/// <remarks>
		/// Checks whether this route includes a layered protocol.
		/// In the presence of proxies, only layering over an end-to-end tunnel
		/// is considered.
		/// </remarks>
		/// <returns>
		/// <code>true</code> if layered,
		/// <code>false</code> otherwise
		/// </returns>
		public abstract bool IsLayered();

		/// <summary>Checks whether this route is secure.</summary>
		/// <remarks>Checks whether this route is secure.</remarks>
		/// <returns>
		/// <code>true</code> if secure,
		/// <code>false</code> otherwise
		/// </returns>
		public abstract bool IsSecure();
	}
}
