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
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Conn;
using Apache.Http.Impl.Conn;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Default implementation of
	/// <see cref="Apache.Http.Client.AuthCache">Apache.Http.Client.AuthCache</see>
	/// .
	/// </summary>
	/// <since>4.0</since>
	public class BasicAuthCache : AuthCache
	{
		private readonly Dictionary<HttpHost, AuthScheme> map;

		private readonly SchemePortResolver schemePortResolver;

		/// <summary>Default constructor.</summary>
		/// <remarks>Default constructor.</remarks>
		/// <since>4.3</since>
		public BasicAuthCache(SchemePortResolver schemePortResolver) : base()
		{
			this.map = new Dictionary<HttpHost, AuthScheme>();
			this.schemePortResolver = schemePortResolver != null ? schemePortResolver : DefaultSchemePortResolver
				.Instance;
		}

		public BasicAuthCache() : this(null)
		{
		}

		protected internal virtual HttpHost GetKey(HttpHost host)
		{
			if (host.GetPort() <= 0)
			{
				int port;
				try
				{
					port = schemePortResolver.Resolve(host);
				}
				catch (UnsupportedSchemeException)
				{
					return host;
				}
				return new HttpHost(host.GetHostName(), port, host.GetSchemeName());
			}
			else
			{
				return host;
			}
		}

		public virtual void Put(HttpHost host, AuthScheme authScheme)
		{
			Args.NotNull(host, "HTTP host");
			this.map.Put(GetKey(host), authScheme);
		}

		public virtual AuthScheme Get(HttpHost host)
		{
			Args.NotNull(host, "HTTP host");
			return this.map.Get(GetKey(host));
		}

		public virtual void Remove(HttpHost host)
		{
			Args.NotNull(host, "HTTP host");
			Sharpen.Collections.Remove(this.map, GetKey(host));
		}

		public virtual void Clear()
		{
			this.map.Clear();
		}

		public override string ToString()
		{
			return this.map.ToString();
		}
	}
}
