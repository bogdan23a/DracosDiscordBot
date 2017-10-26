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
using Apache.Http.Conn;
using Apache.Http.Impl.Conn;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>
	/// Implementation of an
	/// <see cref="Apache.Http.Conn.Routing.HttpRoutePlanner">Apache.Http.Conn.Routing.HttpRoutePlanner
	/// 	</see>
	/// that routes requests through a default proxy.
	/// </summary>
	/// <since>4.3</since>
	public class DefaultProxyRoutePlanner : DefaultRoutePlanner
	{
		private readonly HttpHost proxy;

		public DefaultProxyRoutePlanner(HttpHost proxy, SchemePortResolver schemePortResolver
			) : base(schemePortResolver)
		{
			this.proxy = Args.NotNull(proxy, "Proxy host");
		}

		public DefaultProxyRoutePlanner(HttpHost proxy) : this(proxy, null)
		{
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		protected internal override HttpHost DetermineProxy(HttpHost target, IHttpRequest
			 request, HttpContext context)
		{
			return proxy;
		}
	}
}
