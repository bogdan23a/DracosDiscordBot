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
using Apache.Http.Client.Protocol;
using Apache.Http.Config;
using Apache.Http.Conn;
using Apache.Http.Conn.Socket;
using Apache.Http.Impl.Conn;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	internal class HttpClientConnectionOperator
	{
		internal const string SocketFactoryRegistry = "http.socket-factory-registry";

		private readonly Log log = LogFactory.GetLog(typeof(HttpClientConnectionManager));

		private readonly Lookup<ConnectionSocketFactory> socketFactoryRegistry;

		private readonly SchemePortResolver schemePortResolver;

		private readonly DnsResolver dnsResolver;

		internal HttpClientConnectionOperator(Lookup<ConnectionSocketFactory> socketFactoryRegistry
			, SchemePortResolver schemePortResolver, DnsResolver dnsResolver) : base()
		{
			Args.NotNull(socketFactoryRegistry, "Socket factory registry");
			this.socketFactoryRegistry = socketFactoryRegistry;
			this.schemePortResolver = schemePortResolver != null ? schemePortResolver : DefaultSchemePortResolver
				.Instance;
			this.dnsResolver = dnsResolver != null ? dnsResolver : SystemDefaultDnsResolver.Instance;
		}

		private Lookup<ConnectionSocketFactory> GetSocketFactoryRegistry(HttpContext context
			)
		{
			Lookup<ConnectionSocketFactory> reg = (Lookup<ConnectionSocketFactory>)context.GetAttribute
				(SocketFactoryRegistry);
			if (reg == null)
			{
				reg = this.socketFactoryRegistry;
			}
			return reg;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Connect(ManagedHttpClientConnection conn, HttpHost host, IPEndPoint
			 localAddress, int connectTimeout, SocketConfig socketConfig, HttpContext context
			)
		{
			Lookup<ConnectionSocketFactory> registry = GetSocketFactoryRegistry(context);
			ConnectionSocketFactory sf = registry.Lookup(host.GetSchemeName());
			if (sf == null)
			{
				throw new UnsupportedSchemeException(host.GetSchemeName() + " protocol is not supported"
					);
			}
			IPAddress[] addresses = this.dnsResolver.Resolve(host.GetHostName());
			int port = this.schemePortResolver.Resolve(host);
			for (int i = 0; i < addresses.Length; i++)
			{
				IPAddress address = addresses[i];
				bool last = i == addresses.Length - 1;
				System.Net.Sockets.Socket sock = sf.CreateSocket(context);
				sock.SetReuseAddress(socketConfig.IsSoReuseAddress());
				conn.Bind(sock);
				IPEndPoint remoteAddress = new IPEndPoint(address, port);
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Connecting to " + remoteAddress);
				}
				try
				{
					sock.ReceiveTimeout = socketConfig.GetSoTimeout();
					sock = sf.ConnectSocket(connectTimeout, sock, host, remoteAddress, localAddress, 
						context);
					sock.NoDelay = socketConfig.IsTcpNoDelay();
					sock.SetKeepAlive(socketConfig.IsSoKeepAlive());
					int linger = socketConfig.GetSoLinger();
					if (linger >= 0)
					{
						sock.SetSoLinger(linger > 0, linger);
					}
					conn.Bind(sock);
					return;
				}
				catch (SocketTimeoutException ex)
				{
					if (last)
					{
						throw new ConnectTimeoutException(ex, host, addresses);
					}
				}
				catch (ConnectException ex)
				{
					if (last)
					{
						string msg = ex.Message;
						if ("Connection timed out".Equals(msg))
						{
							throw new ConnectTimeoutException(ex, host, addresses);
						}
						else
						{
							throw new HttpHostConnectException(ex, host, addresses);
						}
					}
				}
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Connect to " + remoteAddress + " timed out. " + "Connection will be retried using another IP address"
						);
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Upgrade(ManagedHttpClientConnection conn, HttpHost host, HttpContext
			 context)
		{
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			Lookup<ConnectionSocketFactory> registry = GetSocketFactoryRegistry(clientContext
				);
			ConnectionSocketFactory sf = registry.Lookup(host.GetSchemeName());
			if (sf == null)
			{
				throw new UnsupportedSchemeException(host.GetSchemeName() + " protocol is not supported"
					);
			}
			if (!(sf is LayeredConnectionSocketFactory))
			{
				throw new UnsupportedSchemeException(host.GetSchemeName() + " protocol does not support connection upgrade"
					);
			}
			LayeredConnectionSocketFactory lsf = (LayeredConnectionSocketFactory)sf;
			System.Net.Sockets.Socket sock = conn.GetSocket();
			int port = this.schemePortResolver.Resolve(host);
			sock = lsf.CreateLayeredSocket(sock, host.GetHostName(), port, context);
			conn.Bind(sock);
		}
	}
}
