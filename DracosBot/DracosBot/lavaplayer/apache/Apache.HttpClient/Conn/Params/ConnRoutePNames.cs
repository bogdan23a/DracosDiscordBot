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
	/// <summary>Parameter names for connection routing.</summary>
	/// <remarks>Parameter names for connection routing.</remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) use Apache.Http.Client.Config.RequestConfig .")]
	public abstract class ConnRoutePNames
	{
		/// <summary>Parameter for the default proxy.</summary>
		/// <remarks>
		/// Parameter for the default proxy.
		/// The default value will be used by some
		/// <see cref="Apache.Http.Conn.Routing.HttpRoutePlanner">HttpRoutePlanner</see>
		/// implementations, in particular the default implementation.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="Apache.Http.HttpHost">Apache.Http.HttpHost</see>
		/// .
		/// </p>
		/// </remarks>
		public const string DefaultProxy = "http.route.default-proxy";

		/// <summary>Parameter for the local address.</summary>
		/// <remarks>
		/// Parameter for the local address.
		/// On machines with multiple network interfaces, this parameter
		/// can be used to select the network interface from which the
		/// connection originates.
		/// It will be interpreted by the standard
		/// <see cref="Apache.Http.Conn.Routing.HttpRoutePlanner">HttpRoutePlanner</see>
		/// implementations, in particular the default implementation.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="System.Net.IPAddress">System.Net.IPAddress</see>
		/// .
		/// </p>
		/// </remarks>
		public const string LocalAddress = "http.route.local-address";

		/// <summary>Parameter for an forced route.</summary>
		/// <remarks>
		/// Parameter for an forced route.
		/// The forced route will be interpreted by the standard
		/// <see cref="Apache.Http.Conn.Routing.HttpRoutePlanner">HttpRoutePlanner</see>
		/// implementations.
		/// Instead of computing a route, the given forced route will be
		/// returned, even if it points to the wrong target host.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="Apache.Http.Conn.Routing.HttpRoute">HttpRoute</see>
		/// .
		/// </p>
		/// </remarks>
		public const string ForcedRoute = "http.route.forced-route";
	}
}
