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

using Apache.Http.Conn.Params;
using Sharpen;

namespace Apache.Http.Conn.Params
{
	/// <summary>Parameter names for connection managers in HttpConn.</summary>
	/// <remarks>Parameter names for connection managers in HttpConn.</remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.1) use configuration methods of the specific connection manager implementation."
		)]
	public abstract class ConnManagerPNames
	{
		/// <summary>
		/// Defines the timeout in milliseconds used when retrieving an instance of
		/// <see cref="Apache.Http.Conn.ManagedClientConnection">Apache.Http.Conn.ManagedClientConnection
		/// 	</see>
		/// from the
		/// <see cref="Apache.Http.Conn.ClientConnectionManager">Apache.Http.Conn.ClientConnectionManager
		/// 	</see>
		/// .
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="long">long</see>
		/// .
		/// </summary>
		public const string Timeout = "http.conn-manager.timeout";

		/// <summary>Defines the maximum number of connections per route.</summary>
		/// <remarks>
		/// Defines the maximum number of connections per route.
		/// This limit is interpreted by client connection managers
		/// and applies to individual manager instances.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="ConnPerRoute">ConnPerRoute</see>
		/// .
		/// <p>
		/// </remarks>
		public const string MaxConnectionsPerRoute = "http.conn-manager.max-per-route";

		/// <summary>Defines the maximum number of connections in total.</summary>
		/// <remarks>
		/// Defines the maximum number of connections in total.
		/// This limit is interpreted by client connection managers
		/// and applies to individual manager instances.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="int">int</see>
		/// .
		/// </remarks>
		public const string MaxTotalConnections = "http.conn-manager.max-total";
	}
}
