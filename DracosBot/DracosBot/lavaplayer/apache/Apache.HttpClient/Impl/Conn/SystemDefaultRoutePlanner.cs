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
using System.Net;
using Apache.Http;
using Apache.Http.Conn;
using Apache.Http.Impl.Conn;
using Apache.Http.Protocol;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>
	/// <see cref="Apache.Http.Conn.Routing.HttpRoutePlanner">Apache.Http.Conn.Routing.HttpRoutePlanner
	/// 	</see>
	/// implementation
	/// based on
	/// <see cref="Sharpen.ProxySelector">Sharpen.ProxySelector</see>
	/// . By default, this class will pick up
	/// the proxy settings of the JVM, either from system properties
	/// or from the browser running the application.
	/// </summary>
	/// <since>4.3</since>
	public class SystemDefaultRoutePlanner : DefaultRoutePlanner
	{
		private readonly ProxySelector proxySelector;

		public SystemDefaultRoutePlanner(SchemePortResolver schemePortResolver, ProxySelector
			 proxySelector) : base(schemePortResolver)
		{
			this.proxySelector = proxySelector != null ? proxySelector : ProxySelector.GetDefault
				();
		}

		public SystemDefaultRoutePlanner(ProxySelector proxySelector) : this(null, proxySelector
			)
		{
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		protected internal override HttpHost DetermineProxy(HttpHost target, IHttpRequest
			 request, HttpContext context)
		{
			URI targetURI;
			try
			{
				targetURI = new URI(target.ToURI());
			}
			catch (URISyntaxException ex)
			{
				throw new HttpException("Cannot convert host to URI: " + target, ex);
			}
			IList<Proxy> proxies = this.proxySelector.Select(targetURI);
			Proxy p = ChooseProxy(proxies);
			HttpHost result = null;
			if (p.Type() == Proxy.Type.Http)
			{
				// convert the socket address to an HttpHost
				if (!(p.Address() is IPEndPoint))
				{
					throw new HttpException("Unable to handle non-Inet proxy address: " + p.Address()
						);
				}
				IPEndPoint isa = (IPEndPoint)p.Address();
				// assume default scheme (http)
				result = new HttpHost(GetHost(isa), isa.Port);
			}
			return result;
		}

		private string GetHost(IPEndPoint isa)
		{
			//@@@ Will this work with literal IPv6 addresses, or do we
			//@@@ need to wrap these in [] for the string representation?
			//@@@ Having it in this method at least allows for easy workarounds.
			return isa.IsUnresolved() ? isa.GetHostName() : isa.Address.GetHostAddress();
		}

		private Proxy ChooseProxy(IList<Proxy> proxies)
		{
			Proxy result = null;
			// check the list for one we can use
			for (int i = 0; (result == null) && (i < proxies.Count); i++)
			{
				Proxy p = proxies[i];
				switch (p.Type())
				{
					case Proxy.Type.Direct:
					case Proxy.Type.Http:
					{
						result = p;
						break;
					}

					case Proxy.Type.Socks:
					{
						// SOCKS hosts are not handled on the route level.
						// The socket may make use of the SOCKS host though.
						break;
					}
				}
			}
			if (result == null)
			{
				//@@@ log as warning or info that only a socks proxy is available?
				// result can only be null if all proxies are socks proxies
				// socks proxies are not handled on the route planning level
				result = Proxy.NoProxy;
			}
			return result;
		}
	}
}
