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

using Apache.Http.Client.Methods;
using Apache.Http.Client.Protocol;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Execchain;
using Sharpen;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>This interface represents an element in the HTTP request execution chain.
	/// 	</summary>
	/// <remarks>
	/// This interface represents an element in the HTTP request execution chain. Each element can
	/// either be a decorator around another element that implements a cross cutting aspect or
	/// a self-contained executor capable of producing a response for the given request.
	/// <p/>
	/// Important: please note it is required for decorators that implement post execution aspects
	/// or response post-processing of any sort to release resources associated with the response
	/// by calling
	/// <see cref="System.IDisposable.Close()">System.IDisposable.Close()</see>
	/// methods in case of an I/O, protocol or
	/// runtime exception, or in case the response is not propagated to the caller.
	/// </remarks>
	/// <since>4.3</since>
	public interface ClientExecChain
	{
		/// <summary>
		/// Executes th request either by transmitting it to the target server or
		/// by passing it onto the next executor in the request execution chain.
		/// </summary>
		/// <remarks>
		/// Executes th request either by transmitting it to the target server or
		/// by passing it onto the next executor in the request execution chain.
		/// </remarks>
		/// <param name="route">connection route.</param>
		/// <param name="request">current request.</param>
		/// <param name="clientContext">current HTTP context.</param>
		/// <param name="execAware">receiver of notifications of blocking I/O operations.</param>
		/// <returns>
		/// HTTP response either received from the opposite endpoint
		/// or generated locally.
		/// </returns>
		/// <exception cref="System.IO.IOException">
		/// in case of a I/O error.
		/// (this type of exceptions are potentially recoverable).
		/// </exception>
		/// <exception cref="Apache.Http.HttpException">
		/// in case of an HTTP protocol error
		/// (usually this type of exceptions are non-recoverable).
		/// </exception>
		CloseableHttpResponse Execute(HttpRoute route, HttpRequestWrapper request, HttpClientContext
			 clientContext, HttpExecutionAware execAware);
	}
}
