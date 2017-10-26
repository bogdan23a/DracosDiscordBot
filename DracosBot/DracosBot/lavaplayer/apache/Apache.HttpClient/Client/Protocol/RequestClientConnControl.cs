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

using Apache.Http.Client.Protocol;
using Apache.Http.Conn.Routing;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Client.Protocol
{
	/// <summary>
	/// This protocol interceptor is responsible for adding <code>Connection</code>
	/// or <code>Proxy-Connection</code> headers to the outgoing requests, which
	/// is essential for managing persistence of <code>HTTP/1.0</code> connections.
	/// </summary>
	/// <remarks>
	/// This protocol interceptor is responsible for adding <code>Connection</code>
	/// or <code>Proxy-Connection</code> headers to the outgoing requests, which
	/// is essential for managing persistence of <code>HTTP/1.0</code> connections.
	/// </remarks>
	/// <since>4.0</since>
	public class RequestClientConnControl : IHttpRequestInterceptor
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private const string ProxyConnDirective = "Proxy-Connection";

		public RequestClientConnControl() : base()
		{
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Process(IHttpRequest request, HttpContext context)
		{
			Args.NotNull(request, "HTTP request");
			string method = request.GetRequestLine().GetMethod();
			if (Sharpen.Runtime.EqualsIgnoreCase(method, "CONNECT"))
			{
				request.SetHeader(ProxyConnDirective, HTTP.ConnKeepAlive);
				return;
			}
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			// Obtain the client connection (required)
			RouteInfo route = clientContext.GetHttpRoute();
			if (route == null)
			{
				this.log.Debug("Connection route not set in the context");
				return;
			}
			if (route.GetHopCount() == 1 || route.IsTunnelled())
			{
				if (!request.ContainsHeader(HTTP.ConnDirective))
				{
					request.AddHeader(HTTP.ConnDirective, HTTP.ConnKeepAlive);
				}
			}
			if (route.GetHopCount() == 2 && !route.IsTunnelled())
			{
				if (!request.ContainsHeader(ProxyConnDirective))
				{
					request.AddHeader(ProxyConnDirective, HTTP.ConnKeepAlive);
				}
			}
		}
	}
}
