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

using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Conn.Scheme;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>
	/// Management interface for
	/// <see cref="ManagedClientConnection">client connections</see>
	/// .
	/// The purpose of an HTTP connection manager is to serve as a factory for new
	/// HTTP connections, manage persistent connections and synchronize access to
	/// persistent connections making sure that only one thread of execution can
	/// have access to a connection at a time.
	/// <p>
	/// Implementations of this interface must be thread-safe. Access to shared
	/// data must be synchronized as methods of this interface may be executed
	/// from multiple threads.
	/// </summary>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) replaced by HttpClientConnectionManager .")]
	public interface ClientConnectionManager
	{
		/// <summary>Obtains the scheme registry used by this manager.</summary>
		/// <remarks>Obtains the scheme registry used by this manager.</remarks>
		/// <returns>the scheme registry, never <code>null</code></returns>
		SchemeRegistry GetSchemeRegistry();

		/// <summary>
		/// Returns a new
		/// <see cref="ClientConnectionRequest">ClientConnectionRequest</see>
		/// , from which a
		/// <see cref="ManagedClientConnection">ManagedClientConnection</see>
		/// can be obtained or the request can be
		/// aborted.
		/// </summary>
		ClientConnectionRequest RequestConnection(HttpRoute route, object state);

		/// <summary>Releases a connection for use by others.</summary>
		/// <remarks>
		/// Releases a connection for use by others.
		/// You may optionally specify how long the connection is valid
		/// to be reused.  Values &lt;= 0 are considered to be valid forever.
		/// If the connection is not marked as reusable, the connection will
		/// not be reused regardless of the valid duration.
		/// If the connection has been released before,
		/// the call will be ignored.
		/// </remarks>
		/// <param name="conn">the connection to release</param>
		/// <param name="validDuration">the duration of time this connection is valid for reuse
		/// 	</param>
		/// <param name="timeUnit">the unit of time validDuration is measured in</param>
		/// <seealso cref="CloseExpiredConnections()">CloseExpiredConnections()</seealso>
		void ReleaseConnection(ManagedClientConnection conn, long validDuration, TimeUnit
			 timeUnit);

		/// <summary>Closes idle connections in the pool.</summary>
		/// <remarks>
		/// Closes idle connections in the pool.
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
