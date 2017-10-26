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

using Apache.Http;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>Represents a manager of persistent client connections.</summary>
	/// <remarks>
	/// Represents a manager of persistent client connections.
	/// <p/>
	/// The purpose of an HTTP connection manager is to serve as a factory for new
	/// HTTP connections, manage persistent connections and synchronize access to
	/// persistent connections making sure that only one thread of execution can
	/// have access to a connection at a time.
	/// <p/>
	/// Implementations of this interface must be thread-safe. Access to shared
	/// data must be synchronized as methods of this interface may be executed
	/// from multiple threads.
	/// </remarks>
	/// <since>4.3</since>
	public interface HttpClientConnectionManager
	{
		/// <summary>
		/// Returns a new
		/// <see cref="ConnectionRequest">ConnectionRequest</see>
		/// , from which a
		/// <see cref="Apache.Http.HttpClientConnection">Apache.Http.HttpClientConnection</see>
		/// can be obtained or the request can be
		/// aborted.
		/// <p/>
		/// Please note that newly allocated connections can be returned
		/// in the closed state. The consumer of that connection is responsible
		/// for fully establishing the route the to the connection target
		/// by calling
		/// <see cref="Connect(Apache.Http.HttpClientConnection, Apache.Http.Conn.Routing.HttpRoute, int, Apache.Http.Protocol.HttpContext)
		/// 	">connect</see>
		/// in order to connect
		/// directly to the target or to the first proxy hop, optionally calling
		/// <see cref="Upgrade(Apache.Http.HttpClientConnection, Apache.Http.Conn.Routing.HttpRoute, Apache.Http.Protocol.HttpContext)
		/// 	">upgrade</see>
		/// method to upgrade
		/// the connection after having executed <code>CONNECT</code> method to
		/// all intermediate proxy hops and and finally calling
		/// <see cref="RouteComplete(Apache.Http.HttpClientConnection, Apache.Http.Conn.Routing.HttpRoute, Apache.Http.Protocol.HttpContext)
		/// 	">routeComplete</see>
		/// to mark the route
		/// as fully completed.
		/// </summary>
		/// <param name="route">HTTP route of the requested connection.</param>
		/// <param name="state">
		/// expected state of the connection or <code>null</code>
		/// if the connection is not expected to carry any state.
		/// </param>
		ConnectionRequest RequestConnection(HttpRoute route, object state);

		/// <summary>
		/// Releases the connection back to the manager making it potentially
		/// re-usable by other consumers.
		/// </summary>
		/// <remarks>
		/// Releases the connection back to the manager making it potentially
		/// re-usable by other consumers. Optionally, the maximum period
		/// of how long the manager should keep the connection alive can be
		/// defined using <code>validDuration</code> and <code>timeUnit</code>
		/// parameters.
		/// </remarks>
		/// <param name="conn">the managed connection to release.</param>
		/// <param name="validDuration">the duration of time this connection is valid for reuse.
		/// 	</param>
		/// <param name="timeUnit">the time unit.</param>
		/// <seealso cref="CloseExpiredConnections()">CloseExpiredConnections()</seealso>
		void ReleaseConnection(HttpClientConnection conn, object newState, long validDuration
			, TimeUnit timeUnit);

		/// <summary>
		/// Connects the underlying connection socket to the connection target in case
		/// of a direct route or to the first proxy hop in case of a route via a proxy
		/// (or multiple proxies).
		/// </summary>
		/// <remarks>
		/// Connects the underlying connection socket to the connection target in case
		/// of a direct route or to the first proxy hop in case of a route via a proxy
		/// (or multiple proxies).
		/// </remarks>
		/// <param name="conn">the managed connection.</param>
		/// <param name="route">the route of the connection.</param>
		/// <param name="connectTimeout">connect timeout in milliseconds.</param>
		/// <param name="context">the actual HTTP context.</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void Connect(HttpClientConnection conn, HttpRoute route, int connectTimeout, HttpContext
			 context);

		/// <summary>
		/// Upgrades the underlying connection socket to TLS/SSL (or another layering
		/// protocol) after having executed <code>CONNECT</code> method to all
		/// intermediate proxy hops
		/// </summary>
		/// <param name="conn">the managed connection.</param>
		/// <param name="route">the route of the connection.</param>
		/// <param name="context">the actual HTTP context.</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void Upgrade(HttpClientConnection conn, HttpRoute route, HttpContext context);

		/// <summary>
		/// Marks the connection as fully established with all its intermediate
		/// hops completed.
		/// </summary>
		/// <remarks>
		/// Marks the connection as fully established with all its intermediate
		/// hops completed.
		/// </remarks>
		/// <param name="conn">the managed connection.</param>
		/// <param name="route">the route of the connection.</param>
		/// <param name="context">the actual HTTP context.</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void RouteComplete(HttpClientConnection conn, HttpRoute route, HttpContext context
			);

		/// <summary>Closes idle connections in the pool.</summary>
		/// <remarks>
		/// Closes idle connections in the pool.
		/// <p/>
		/// Open connections in the pool that have not been used for the
		/// timespan given by the argument will be closed.
		/// Currently allocated connections are not subject to this method.
		/// Times will be checked with milliseconds precision
		/// All expired connections will also be closed.
		/// </remarks>
		/// <param name="idletime">the idle time of connections to be closed</param>
		/// <param name="tunit">the unit for the <code>idletime</code></param>
		/// <seealso cref="CloseExpiredConnections()">CloseExpiredConnections()</seealso>
		void CloseIdleConnections(long idletime, TimeUnit tunit);

		/// <summary>Closes all expired connections in the pool.</summary>
		/// <remarks>
		/// Closes all expired connections in the pool.
		/// <p/>
		/// Open connections in the pool that have not been used for
		/// the timespan defined when the connection was released will be closed.
		/// Currently allocated connections are not subject to this method.
		/// Times will be checked with milliseconds precision.
		/// </remarks>
		void CloseExpiredConnections();

		/// <summary>Shuts down this connection manager and releases allocated resources.</summary>
		/// <remarks>
		/// Shuts down this connection manager and releases allocated resources.
		/// This includes closing all connections, whether they are currently
		/// used or not.
		/// </remarks>
		void Shutdown();
	}
}
