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
using Apache.Http.Client.Config;
using Apache.Http.Client.Protocol;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Client.Protocol
{
	/// <summary>
	/// RequestExpectContinue is responsible for enabling the 'expect-continue'
	/// handshake by adding <code>Expect</code> header.
	/// </summary>
	/// <remarks>
	/// RequestExpectContinue is responsible for enabling the 'expect-continue'
	/// handshake by adding <code>Expect</code> header.
	/// <p/>
	/// This interceptor takes into account
	/// <see cref="Apache.Http.Client.Config.RequestConfig.IsExpectContinueEnabled()">Apache.Http.Client.Config.RequestConfig.IsExpectContinueEnabled()
	/// 	</see>
	/// setting.
	/// </remarks>
	/// <since>4.3</since>
	public class RequestExpectContinue : IHttpRequestInterceptor
	{
		public RequestExpectContinue() : base()
		{
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Process(IHttpRequest request, HttpContext context)
		{
			Args.NotNull(request, "HTTP request");
			if (!request.ContainsHeader(HTTP.ExpectDirective))
			{
				if (request is HttpEntityEnclosingRequest)
				{
					ProtocolVersion ver = request.GetRequestLine().GetProtocolVersion();
					HttpEntity entity = ((HttpEntityEnclosingRequest)request).GetEntity();
					// Do not send the expect header if request body is known to be empty
					if (entity != null && entity.GetContentLength() != 0 && !ver.LessEquals(HttpVersion
						.Http10))
					{
						HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
							));
						RequestConfig config = clientContext.GetRequestConfig();
						if (config.IsExpectContinueEnabled())
						{
							request.AddHeader(HTTP.ExpectDirective, HTTP.ExpectContinue);
						}
					}
				}
			}
		}
	}
}
