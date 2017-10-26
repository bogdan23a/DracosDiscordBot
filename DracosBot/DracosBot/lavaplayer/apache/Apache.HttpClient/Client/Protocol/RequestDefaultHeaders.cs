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

using System.Collections.Generic;
using Apache.Http;
using Apache.Http.Client.Params;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Client.Protocol
{
	/// <summary>Request interceptor that adds default request headers.</summary>
	/// <remarks>Request interceptor that adds default request headers.</remarks>
	/// <since>4.0</since>
	public class RequestDefaultHeaders : IHttpRequestInterceptor
	{
		private readonly ICollection<Header> defaultHeaders;

		/// <since>4.3</since>
		public RequestDefaultHeaders(ICollection<Header> defaultHeaders) : base()
		{
			this.defaultHeaders = defaultHeaders;
		}

		public RequestDefaultHeaders() : this(null)
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
				return;
			}
			// Add default headers
			ICollection<Header> defHeaders = (ICollection<Header>)request.GetParams().GetParameter
				(ClientPNames.DefaultHeaders);
			if (defHeaders == null)
			{
				defHeaders = this.defaultHeaders;
			}
			if (defHeaders != null)
			{
				foreach (Header defHeader in defHeaders)
				{
					request.AddHeader(defHeader);
				}
			}
		}
	}
}
