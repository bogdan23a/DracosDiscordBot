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
using Apache.Http.Client;
using Apache.Http.Protocol;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// A strategy for determining if an HTTP request should be redirected to
	/// a new location in response to an HTTP response received from the target
	/// server.
	/// </summary>
	/// <remarks>
	/// A strategy for determining if an HTTP request should be redirected to
	/// a new location in response to an HTTP response received from the target
	/// server.
	/// <p>
	/// Implementations of this interface must be thread-safe. Access to shared
	/// data must be synchronized as methods of this interface may be executed
	/// from multiple threads.
	/// </remarks>
	/// <since>4.1</since>
	public interface RedirectStrategy
	{
		/// <summary>
		/// Determines if a request should be redirected to a new location
		/// given the response from the target server.
		/// </summary>
		/// <remarks>
		/// Determines if a request should be redirected to a new location
		/// given the response from the target server.
		/// </remarks>
		/// <param name="request">the executed request</param>
		/// <param name="response">the response received from the target server</param>
		/// <param name="context">the context for the request execution</param>
		/// <returns>
		/// <code>true</code> if the request should be redirected, <code>false</code>
		/// otherwise
		/// </returns>
		/// <exception cref="Apache.Http.ProtocolException"></exception>
		bool IsRedirected(IHttpRequest request, HttpResponse response, HttpContext context
			);

		/// <summary>
		/// Determines the redirect location given the response from the target
		/// server and the current request execution context and generates a new
		/// request to be sent to the location.
		/// </summary>
		/// <remarks>
		/// Determines the redirect location given the response from the target
		/// server and the current request execution context and generates a new
		/// request to be sent to the location.
		/// </remarks>
		/// <param name="request">the executed request</param>
		/// <param name="response">the response received from the target server</param>
		/// <param name="context">the context for the request execution</param>
		/// <returns>redirected request</returns>
		/// <exception cref="Apache.Http.ProtocolException"></exception>
		IHttpUriRequest GetRedirect(IHttpRequest request, HttpResponse response, HttpContext
			 context);
	}
}
