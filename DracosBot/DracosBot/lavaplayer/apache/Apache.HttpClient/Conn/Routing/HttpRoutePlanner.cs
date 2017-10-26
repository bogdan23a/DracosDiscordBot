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
using Apache.Http.Conn.Routing;
using Apache.Http.Protocol;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Conn.Routing
{
	/// <summary>
	/// Encapsulates logic to compute a
	/// <see cref="HttpRoute">HttpRoute</see>
	/// to a target host.
	/// Implementations may for example be based on parameters, or on the
	/// standard Java system properties.
	/// <p/>
	/// Implementations of this interface must be thread-safe. Access to shared
	/// data must be synchronized as methods of this interface may be executed
	/// from multiple threads.
	/// </summary>
	/// <since>4.0</since>
	public interface HttpRoutePlanner
	{
		/// <summary>Determines the route for a request.</summary>
		/// <remarks>Determines the route for a request.</remarks>
		/// <param name="target">
		/// the target host for the request.
		/// Implementations may accept <code>null</code>
		/// if they can still determine a route, for example
		/// to a default target or by inspecting the request.
		/// </param>
		/// <param name="request">the request to execute</param>
		/// <param name="context">
		/// the context to use for the subsequent execution.
		/// Implementations may accept <code>null</code>.
		/// </param>
		/// <returns>the route that the request should take</returns>
		/// <exception cref="Apache.Http.HttpException">in case of a problem</exception>
		HttpRoute DetermineRoute(HttpHost target, IHttpRequest request, HttpContext context
			);
	}
}
