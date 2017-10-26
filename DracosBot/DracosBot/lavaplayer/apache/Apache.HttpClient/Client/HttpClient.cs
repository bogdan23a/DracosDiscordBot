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
using Apache.Http;
using Apache.Http.Client;
using Apache.Http.Conn;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// This interface represents only the most basic contract for HTTP request
	/// execution.
	/// </summary>
	/// <remarks>
	/// This interface represents only the most basic contract for HTTP request
	/// execution. It imposes no restrictions or particular details on the request
	/// execution process and leaves the specifics of state management,
	/// authentication and redirect handling up to individual implementations.
	/// </remarks>
	/// <since>4.0</since>
	public interface HttpClient
	{
		/// <summary>Obtains the parameters for this client.</summary>
		/// <remarks>
		/// Obtains the parameters for this client.
		/// These parameters will become defaults for all requests being
		/// executed with this client, and for the parameters of
		/// dependent objects in this client.
		/// </remarks>
		/// <returns>the default parameters</returns>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) useApache.Http.Client.Config.RequestConfig .")]
		HttpParams GetParams();

		/// <summary>Obtains the connection manager used by this client.</summary>
		/// <remarks>Obtains the connection manager used by this client.</remarks>
		/// <returns>the connection manager</returns>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) useApache.Http.Impl.Client.HttpClientBuilder ."
			)]
		ClientConnectionManager GetConnectionManager();

		/// <summary>Executes HTTP request using the default context.</summary>
		/// <remarks>Executes HTTP request using the default context.</remarks>
		/// <param name="request">the request to execute</param>
		/// <returns>
		/// the response to the request. This is always a final response,
		/// never an intermediate response with an 1xx status code.
		/// Whether redirects or authentication challenges will be returned
		/// or handled automatically depends on the implementation and
		/// configuration of this client.
		/// </returns>
		/// <exception cref="System.IO.IOException">in case of a problem or the connection was aborted
		/// 	</exception>
		/// <exception cref="ClientProtocolException">in case of an http protocol error</exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		HttpResponse Execute(IHttpUriRequest request);

		/// <summary>Executes HTTP request using the given context.</summary>
		/// <remarks>Executes HTTP request using the given context.</remarks>
		/// <param name="request">the request to execute</param>
		/// <param name="context">
		/// the context to use for the execution, or
		/// <code>null</code> to use the default context
		/// </param>
		/// <returns>
		/// the response to the request. This is always a final response,
		/// never an intermediate response with an 1xx status code.
		/// Whether redirects or authentication challenges will be returned
		/// or handled automatically depends on the implementation and
		/// configuration of this client.
		/// </returns>
		/// <exception cref="System.IO.IOException">in case of a problem or the connection was aborted
		/// 	</exception>
		/// <exception cref="ClientProtocolException">in case of an http protocol error</exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		HttpResponse Execute(IHttpUriRequest request, HttpContext context);

		/// <summary>Executes HTTP request using the default context.</summary>
		/// <remarks>Executes HTTP request using the default context.</remarks>
		/// <param name="target">
		/// the target host for the request.
		/// Implementations may accept <code>null</code>
		/// if they can still determine a route, for example
		/// to a default target or by inspecting the request.
		/// </param>
		/// <param name="request">the request to execute</param>
		/// <returns>
		/// the response to the request. This is always a final response,
		/// never an intermediate response with an 1xx status code.
		/// Whether redirects or authentication challenges will be returned
		/// or handled automatically depends on the implementation and
		/// configuration of this client.
		/// </returns>
		/// <exception cref="System.IO.IOException">in case of a problem or the connection was aborted
		/// 	</exception>
		/// <exception cref="ClientProtocolException">in case of an http protocol error</exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		HttpResponse Execute(HttpHost target, IHttpRequest request);

		/// <summary>Executes HTTP request using the given context.</summary>
		/// <remarks>Executes HTTP request using the given context.</remarks>
		/// <param name="target">
		/// the target host for the request.
		/// Implementations may accept <code>null</code>
		/// if they can still determine a route, for example
		/// to a default target or by inspecting the request.
		/// </param>
		/// <param name="request">the request to execute</param>
		/// <param name="context">
		/// the context to use for the execution, or
		/// <code>null</code> to use the default context
		/// </param>
		/// <returns>
		/// the response to the request. This is always a final response,
		/// never an intermediate response with an 1xx status code.
		/// Whether redirects or authentication challenges will be returned
		/// or handled automatically depends on the implementation and
		/// configuration of this client.
		/// </returns>
		/// <exception cref="System.IO.IOException">in case of a problem or the connection was aborted
		/// 	</exception>
		/// <exception cref="ClientProtocolException">in case of an http protocol error</exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		HttpResponse Execute(HttpHost target, IHttpRequest request, HttpContext context);

		/// <summary>
		/// Executes HTTP request using the default context and processes the
		/// response using the given response handler.
		/// </summary>
		/// <remarks>
		/// Executes HTTP request using the default context and processes the
		/// response using the given response handler.
		/// <p/>
		/// Implementing classes are required to ensure that the content entity
		/// associated with the response is fully consumed and the underlying
		/// connection is released back to the connection manager automatically
		/// in all cases relieving individual
		/// <see cref="ResponseHandler{T}">ResponseHandler&lt;T&gt;</see>
		/// s from
		/// having to manage resource deallocation internally.
		/// </remarks>
		/// <param name="request">the request to execute</param>
		/// <param name="responseHandler">the response handler</param>
		/// <returns>the response object as generated by the response handler.</returns>
		/// <exception cref="System.IO.IOException">in case of a problem or the connection was aborted
		/// 	</exception>
		/// <exception cref="ClientProtocolException">in case of an http protocol error</exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		T Execute<T, _T1>(IHttpUriRequest request, ResponseHandler<_T1> responseHandler) where 
			_T1:T;

		/// <summary>
		/// Executes HTTP request using the given context and processes the
		/// response using the given response handler.
		/// </summary>
		/// <remarks>
		/// Executes HTTP request using the given context and processes the
		/// response using the given response handler.
		/// <p/>
		/// Implementing classes are required to ensure that the content entity
		/// associated with the response is fully consumed and the underlying
		/// connection is released back to the connection manager automatically
		/// in all cases relieving individual
		/// <see cref="ResponseHandler{T}">ResponseHandler&lt;T&gt;</see>
		/// s from
		/// having to manage resource deallocation internally.
		/// </remarks>
		/// <param name="request">the request to execute</param>
		/// <param name="responseHandler">the response handler</param>
		/// <param name="context">
		/// the context to use for the execution, or
		/// <code>null</code> to use the default context
		/// </param>
		/// <returns>the response object as generated by the response handler.</returns>
		/// <exception cref="System.IO.IOException">in case of a problem or the connection was aborted
		/// 	</exception>
		/// <exception cref="ClientProtocolException">in case of an http protocol error</exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		T Execute<T, _T1>(IHttpUriRequest request, ResponseHandler<_T1> responseHandler, 
			HttpContext context) where _T1:T;

		/// <summary>
		/// Executes HTTP request to the target using the default context and
		/// processes the response using the given response handler.
		/// </summary>
		/// <remarks>
		/// Executes HTTP request to the target using the default context and
		/// processes the response using the given response handler.
		/// <p/>
		/// Implementing classes are required to ensure that the content entity
		/// associated with the response is fully consumed and the underlying
		/// connection is released back to the connection manager automatically
		/// in all cases relieving individual
		/// <see cref="ResponseHandler{T}">ResponseHandler&lt;T&gt;</see>
		/// s from
		/// having to manage resource deallocation internally.
		/// </remarks>
		/// <param name="target">
		/// the target host for the request.
		/// Implementations may accept <code>null</code>
		/// if they can still determine a route, for example
		/// to a default target or by inspecting the request.
		/// </param>
		/// <param name="request">the request to execute</param>
		/// <param name="responseHandler">the response handler</param>
		/// <returns>the response object as generated by the response handler.</returns>
		/// <exception cref="System.IO.IOException">in case of a problem or the connection was aborted
		/// 	</exception>
		/// <exception cref="ClientProtocolException">in case of an http protocol error</exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		T Execute<T, _T1>(HttpHost target, IHttpRequest request, ResponseHandler<_T1> responseHandler
			) where _T1:T;

		/// <summary>
		/// Executes HTTP request to the target using the given context and
		/// processes the response using the given response handler.
		/// </summary>
		/// <remarks>
		/// Executes HTTP request to the target using the given context and
		/// processes the response using the given response handler.
		/// <p/>
		/// Implementing classes are required to ensure that the content entity
		/// associated with the response is fully consumed and the underlying
		/// connection is released back to the connection manager automatically
		/// in all cases relieving individual
		/// <see cref="ResponseHandler{T}">ResponseHandler&lt;T&gt;</see>
		/// s from
		/// having to manage resource deallocation internally.
		/// </remarks>
		/// <param name="target">
		/// the target host for the request.
		/// Implementations may accept <code>null</code>
		/// if they can still determine a route, for example
		/// to a default target or by inspecting the request.
		/// </param>
		/// <param name="request">the request to execute</param>
		/// <param name="responseHandler">the response handler</param>
		/// <param name="context">
		/// the context to use for the execution, or
		/// <code>null</code> to use the default context
		/// </param>
		/// <returns>the response object as generated by the response handler.</returns>
		/// <exception cref="System.IO.IOException">in case of a problem or the connection was aborted
		/// 	</exception>
		/// <exception cref="ClientProtocolException">in case of an http protocol error</exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		T Execute<T, _T1>(HttpHost target, IHttpRequest request, ResponseHandler<_T1> responseHandler
			, HttpContext context) where _T1:T;
	}
}
