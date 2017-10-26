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

using System.Net;
using Apache.Http;
using Apache.Http.Client.Config;
using Apache.Http.Client.Protocol;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Conn;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>
	/// Default implementation of an
	/// <see cref="Apache.Http.Conn.Routing.HttpRoutePlanner">Apache.Http.Conn.Routing.HttpRoutePlanner
	/// 	</see>
	/// . It will not make use of
	/// any Java system properties, nor of system or browser proxy settings.
	/// </summary>
	/// <since>4.3</since>
	public class DefaultRoutePlanner : HttpRoutePlanner
	{
		private readonly SchemePortResolver schemePortResolver;

		public DefaultRoutePlanner(SchemePortResolver schemePortResolver) : base()
		{
			this.schemePortResolver = schemePortResolver != null ? schemePortResolver : DefaultSchemePortResolver
				.Instance;
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		public virtual HttpRoute DetermineRoute(HttpHost host, IHttpRequest request, HttpContext
			 context)
		{
			Args.NotNull(host, "Target host");
			Args.NotNull(request, "Request");
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			RequestConfig config = clientContext.GetRequestConfig();
			IPAddress local = config.GetLocalAddress();
			HttpHost proxy = config.GetProxy();
			if (proxy == null)
			{
				proxy = DetermineProxy(host, request, context);
			}
			HttpHost target;
			if (host.GetPort() <= 0)
			{
				try
				{
					target = new HttpHost(host.GetHostName(), this.schemePortResolver.Resolve(host), 
						host.GetSchemeName());
				}
				catch (UnsupportedSchemeException ex)
				{
					throw new HttpException(ex.Message);
				}
			}
			else
			{
				target = host;
			}
			bool secure = Sharpen.Runtime.EqualsIgnoreCase(target.GetSchemeName(), "https");
			if (proxy == null)
			{
				return new HttpRoute(target, local, secure);
			}
			else
			{
				return new HttpRoute(target, local, proxy, secure);
			}
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		protected internal virtual HttpHost DetermineProxy(HttpHost target, IHttpRequest 
			request, HttpContext context)
		{
			return null;
		}
	}
}
