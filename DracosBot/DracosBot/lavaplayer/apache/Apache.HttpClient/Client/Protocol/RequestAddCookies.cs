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
using System.Collections.Generic;
using Apache.Http;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Client.Protocol;
using Apache.Http.Config;
using Apache.Http.Conn.Routing;
using Apache.Http.Cookie;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Client.Protocol
{
	/// <summary>
	/// Request interceptor that matches cookies available in the current
	/// <see cref="Apache.Http.Client.CookieStore">Apache.Http.Client.CookieStore</see>
	/// to the request being executed and generates
	/// corresponding <code>Cookie</code> request headers.
	/// </summary>
	/// <since>4.0</since>
	public class RequestAddCookies : IHttpRequestInterceptor
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		public RequestAddCookies() : base()
		{
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Process(IHttpRequest request, HttpContext context)
		{
			Args.NotNull(request, "HTTP request");
			Args.NotNull(context, "HTTP context");
			string method = request.GetRequestLine().GetMethod();
			if (Sharpen.Runtime.EqualsIgnoreCase(method, "CONNECT"))
			{
				return;
			}
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			// Obtain cookie store
			CookieStore cookieStore = clientContext.GetCookieStore();
			if (cookieStore == null)
			{
				this.log.Debug("Cookie store not specified in HTTP context");
				return;
			}
			// Obtain the registry of cookie specs
			Lookup<CookieSpecProvider> registry = clientContext.GetCookieSpecRegistry();
			if (registry == null)
			{
				this.log.Debug("CookieSpec registry not specified in HTTP context");
				return;
			}
			// Obtain the target host, possibly virtual (required)
			HttpHost targetHost = clientContext.GetTargetHost();
			if (targetHost == null)
			{
				this.log.Debug("Target host not set in the context");
				return;
			}
			// Obtain the route (required)
			RouteInfo route = clientContext.GetHttpRoute();
			if (route == null)
			{
				this.log.Debug("Connection route not set in the context");
				return;
			}
			RequestConfig config = clientContext.GetRequestConfig();
			string policy = config.GetCookieSpec();
			if (policy == null)
			{
				policy = CookieSpecs.BestMatch;
			}
			if (this.log.IsDebugEnabled())
			{
				this.log.Debug("CookieSpec selected: " + policy);
			}
			URI requestURI = null;
			if (request is IHttpUriRequest)
			{
				requestURI = ((IHttpUriRequest)request).GetURI();
			}
			else
			{
				try
				{
					requestURI = new URI(request.GetRequestLine().GetUri());
				}
				catch (URISyntaxException)
				{
				}
			}
			string path = requestURI != null ? requestURI.GetPath() : null;
			string hostName = targetHost.GetHostName();
			int port = targetHost.GetPort();
			if (port < 0)
			{
				port = route.GetTargetHost().GetPort();
			}
			CookieOrigin cookieOrigin = new CookieOrigin(hostName, port >= 0 ? port : 0, !TextUtils
				.IsEmpty(path) ? path : "/", route.IsSecure());
			// Get an instance of the selected cookie policy
			CookieSpecProvider provider = registry.Lookup(policy);
			if (provider == null)
			{
				throw new HttpException("Unsupported cookie policy: " + policy);
			}
			CookieSpec cookieSpec = provider.Create(clientContext);
			// Get all cookies available in the HTTP state
			IList<Apache.Http.Cookie.Cookie> cookies = new AList<Apache.Http.Cookie.Cookie>(cookieStore
				.GetCookies());
			// Find cookies matching the given origin
			IList<Apache.Http.Cookie.Cookie> matchedCookies = new AList<Apache.Http.Cookie.Cookie
				>();
			DateTime now = new DateTime();
			foreach (Apache.Http.Cookie.Cookie cookie in cookies)
			{
				if (!cookie.IsExpired(now))
				{
					if (cookieSpec.Match(cookie, cookieOrigin))
					{
						if (this.log.IsDebugEnabled())
						{
							this.log.Debug("Cookie " + cookie + " match " + cookieOrigin);
						}
						matchedCookies.AddItem(cookie);
					}
				}
				else
				{
					if (this.log.IsDebugEnabled())
					{
						this.log.Debug("Cookie " + cookie + " expired");
					}
				}
			}
			// Generate Cookie request headers
			if (!matchedCookies.IsEmpty())
			{
				IList<Header> headers = cookieSpec.FormatCookies(matchedCookies);
				foreach (Header header in headers)
				{
					request.AddHeader(header);
				}
			}
			int ver = cookieSpec.GetVersion();
			if (ver > 0)
			{
				bool needVersionHeader = false;
				foreach (Apache.Http.Cookie.Cookie cookie_1 in matchedCookies)
				{
					if (ver != cookie_1.GetVersion() || !(cookie_1 is SetCookie2))
					{
						needVersionHeader = true;
					}
				}
				if (needVersionHeader)
				{
					Header header = cookieSpec.GetVersionHeader();
					if (header != null)
					{
						// Advertise cookie version support
						request.AddHeader(header);
					}
				}
			}
			// Stick the CookieSpec and CookieOrigin instances to the HTTP context
			// so they could be obtained by the response interceptor
			context.SetAttribute(HttpClientContext.CookieSpec, cookieSpec);
			context.SetAttribute(HttpClientContext.CookieOrigin, cookieOrigin);
		}
	}
}
