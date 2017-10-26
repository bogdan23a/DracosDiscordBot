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
using System.Text;
using Apache.Http;
using Apache.Http.Client;
using Apache.Http.Client.Protocol;
using Apache.Http.Cookie;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Client.Protocol
{
	/// <summary>
	/// Response interceptor that populates the current
	/// <see cref="Apache.Http.Client.CookieStore">Apache.Http.Client.CookieStore</see>
	/// with data
	/// contained in response cookies received in the given the HTTP response.
	/// </summary>
	/// <since>4.0</since>
	public class ResponseProcessCookies : HttpResponseInterceptor
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		public ResponseProcessCookies() : base()
		{
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Process(HttpResponse response, HttpContext context)
		{
			Args.NotNull(response, "HTTP request");
			Args.NotNull(context, "HTTP context");
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			// Obtain actual CookieSpec instance
			CookieSpec cookieSpec = clientContext.GetCookieSpec();
			if (cookieSpec == null)
			{
				this.log.Debug("Cookie spec not specified in HTTP context");
				return;
			}
			// Obtain cookie store
			CookieStore cookieStore = clientContext.GetCookieStore();
			if (cookieStore == null)
			{
				this.log.Debug("Cookie store not specified in HTTP context");
				return;
			}
			// Obtain actual CookieOrigin instance
			CookieOrigin cookieOrigin = clientContext.GetCookieOrigin();
			if (cookieOrigin == null)
			{
				this.log.Debug("Cookie origin not specified in HTTP context");
				return;
			}
			HeaderIterator it = response.HeaderIterator(SM.SetCookie);
			ProcessCookies(it, cookieSpec, cookieOrigin, cookieStore);
			// see if the cookie spec supports cookie versioning.
			if (cookieSpec.GetVersion() > 0)
			{
				// process set-cookie2 headers.
				// Cookie2 will replace equivalent Cookie instances
				it = response.HeaderIterator(SM.SetCookie2);
				ProcessCookies(it, cookieSpec, cookieOrigin, cookieStore);
			}
		}

		private void ProcessCookies(HeaderIterator iterator, CookieSpec cookieSpec, CookieOrigin
			 cookieOrigin, CookieStore cookieStore)
		{
			while (iterator.HasNext())
			{
				Header header = iterator.NextHeader();
				try
				{
					IList<Apache.Http.Cookie.Cookie> cookies = cookieSpec.Parse(header, cookieOrigin);
					foreach (Apache.Http.Cookie.Cookie cookie in cookies)
					{
						try
						{
							cookieSpec.Validate(cookie, cookieOrigin);
							cookieStore.AddCookie(cookie);
							if (this.log.IsDebugEnabled())
							{
								this.log.Debug("Cookie accepted [" + FormatCooke(cookie) + "]");
							}
						}
						catch (MalformedCookieException ex)
						{
							if (this.log.IsWarnEnabled())
							{
								this.log.Warn("Cookie rejected [" + FormatCooke(cookie) + "] " + ex.Message);
							}
						}
					}
				}
				catch (MalformedCookieException ex)
				{
					if (this.log.IsWarnEnabled())
					{
						this.log.Warn("Invalid cookie header: \"" + header + "\". " + ex.Message);
					}
				}
			}
		}

		private static string FormatCooke(Apache.Http.Cookie.Cookie cookie)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(cookie.GetName());
			buf.Append("=\"");
			string v = cookie.GetValue();
			if (v.Length > 100)
			{
				v = Sharpen.Runtime.Substring(v, 0, 100) + "...";
			}
			buf.Append(v);
			buf.Append("\"");
			buf.Append(", version:");
			buf.Append(Sharpen.Extensions.ToString(cookie.GetVersion()));
			buf.Append(", domain:");
			buf.Append(cookie.GetDomain());
			buf.Append(", path:");
			buf.Append(cookie.GetPath());
			buf.Append(", expiry:");
			buf.Append(cookie.GetExpiryDate());
			return buf.ToString();
		}
	}
}
