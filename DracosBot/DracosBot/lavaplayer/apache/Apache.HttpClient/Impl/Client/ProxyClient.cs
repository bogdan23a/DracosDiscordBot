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
using System.Net.Sockets;
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Client.Config;
using Apache.Http.Client.Params;
using Apache.Http.Client.Protocol;
using Apache.Http.Config;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Entity;
using Apache.Http.Impl;
using Apache.Http.Impl.Auth;
using Apache.Http.Impl.Client;
using Apache.Http.Impl.Conn;
using Apache.Http.Impl.Execchain;
using Apache.Http.Message;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>ProxyClient can be used to establish a tunnel via an HTTP proxy.</summary>
	/// <remarks>ProxyClient can be used to establish a tunnel via an HTTP proxy.</remarks>
	public class ProxyClient
	{
		private readonly HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> connFactory;

		private readonly ConnectionConfig connectionConfig;

		private readonly RequestConfig requestConfig;

		private readonly HttpProcessor httpProcessor;

		private readonly HttpRequestExecutor requestExec;

		private readonly ProxyAuthenticationStrategy proxyAuthStrategy;

		private readonly HttpAuthenticator authenticator;

		private readonly AuthState proxyAuthState;

		private readonly AuthSchemeRegistry authSchemeRegistry;

		private readonly ConnectionReuseStrategy reuseStrategy;

		/// <since>4.3</since>
		public ProxyClient(HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> 
			connFactory, ConnectionConfig connectionConfig, RequestConfig requestConfig) : base
			()
		{
			this.connFactory = connFactory != null ? connFactory : ManagedHttpClientConnectionFactory
				.Instance;
			this.connectionConfig = connectionConfig != null ? connectionConfig : ConnectionConfig
				.Default;
			this.requestConfig = requestConfig != null ? requestConfig : RequestConfig.Default;
			this.httpProcessor = new ImmutableHttpProcessor(new RequestTargetHost(), new RequestClientConnControl
				(), new RequestUserAgent());
			this.requestExec = new HttpRequestExecutor();
			this.proxyAuthStrategy = new ProxyAuthenticationStrategy();
			this.authenticator = new HttpAuthenticator();
			this.proxyAuthState = new AuthState();
			this.authSchemeRegistry = new AuthSchemeRegistry();
			this.authSchemeRegistry.Register(AuthSchemes.Basic, new BasicSchemeFactory());
			this.authSchemeRegistry.Register(AuthSchemes.Digest, new DigestSchemeFactory());
			this.authSchemeRegistry.Register(AuthSchemes.Ntlm, new NTLMSchemeFactory());
			this.authSchemeRegistry.Register(AuthSchemes.Spnego, new SPNegoSchemeFactory());
			this.authSchemeRegistry.Register(AuthSchemes.Kerberos, new KerberosSchemeFactory(
				));
			this.reuseStrategy = new DefaultConnectionReuseStrategy();
		}

		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) use ProxyClient(Apache.Http.Conn.HttpConnectionFactory{T, C}, Apache.Http.Config.ConnectionConfig, Apache.Http.Client.Config.RequestConfig)"
			)]
		public ProxyClient(HttpParams @params) : this(null, HttpParamConfig.GetConnectionConfig
			(@params), HttpClientParamConfig.GetRequestConfig(@params))
		{
		}

		/// <since>4.3</since>
		public ProxyClient(RequestConfig requestConfig) : this(null, null, requestConfig)
		{
		}

		public ProxyClient() : this(null, null, null)
		{
		}

		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) do not use.")]
		public virtual HttpParams GetParams()
		{
			return new BasicHttpParams();
		}

		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) do not use.")]
		public virtual AuthSchemeRegistry GetAuthSchemeRegistry()
		{
			return this.authSchemeRegistry;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.HttpException"></exception>
		public virtual Socket Tunnel(HttpHost proxy, HttpHost target, Credentials credentials
			)
		{
			Args.NotNull(proxy, "Proxy host");
			Args.NotNull(target, "Target host");
			Args.NotNull(credentials, "Credentials");
			HttpHost host = target;
			if (host.GetPort() <= 0)
			{
				host = new HttpHost(host.GetHostName(), 80, host.GetSchemeName());
			}
			HttpRoute route = new HttpRoute(host, this.requestConfig.GetLocalAddress(), proxy
				, false, RouteInfo.TunnelType.Tunnelled, RouteInfo.LayerType.Plain);
			ManagedHttpClientConnection conn = this.connFactory.Create(route, this.connectionConfig
				);
			HttpContext context = new BasicHttpContext();
			HttpResponse response;
			IHttpRequest connect = new BasicHttpRequest("CONNECT", host.ToHostString(), HttpVersion
				.Http11);
			BasicCredentialsProvider credsProvider = new BasicCredentialsProvider();
			credsProvider.SetCredentials(new AuthScope(proxy), credentials);
			// Populate the execution context
			context.SetAttribute(HttpCoreContext.HttpTargetHost, target);
			context.SetAttribute(HttpCoreContext.HttpConnection, conn);
			context.SetAttribute(HttpCoreContext.HttpRequest, connect);
			context.SetAttribute(HttpClientContext.HttpRoute, route);
			context.SetAttribute(HttpClientContext.ProxyAuthState, this.proxyAuthState);
			context.SetAttribute(HttpClientContext.CredsProvider, credsProvider);
			context.SetAttribute(HttpClientContext.AuthschemeRegistry, this.authSchemeRegistry
				);
			context.SetAttribute(HttpClientContext.RequestConfig, this.requestConfig);
			this.requestExec.PreProcess(connect, this.httpProcessor, context);
			for (; ; )
			{
				if (!conn.IsOpen())
				{
					Socket socket = Sharpen.Extensions.CreateSocket(proxy.GetHostName(), proxy.GetPort
						());
					conn.Bind(socket);
				}
				this.authenticator.GenerateAuthResponse(connect, this.proxyAuthState, context);
				response = this.requestExec.Execute(connect, conn, context);
				int status = response.GetStatusLine().GetStatusCode();
				if (status < 200)
				{
					throw new HttpException("Unexpected response to CONNECT request: " + response.GetStatusLine
						());
				}
				if (this.authenticator.IsAuthenticationRequested(proxy, response, this.proxyAuthStrategy
					, this.proxyAuthState, context))
				{
					if (this.authenticator.HandleAuthChallenge(proxy, response, this.proxyAuthStrategy
						, this.proxyAuthState, context))
					{
						// Retry request
						if (this.reuseStrategy.KeepAlive(response, context))
						{
							// Consume response content
							HttpEntity entity = response.GetEntity();
							EntityUtils.Consume(entity);
						}
						else
						{
							conn.Close();
						}
						// discard previous auth header
						connect.RemoveHeaders(AUTH.ProxyAuthResp);
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
			int status_1 = response.GetStatusLine().GetStatusCode();
			if (status_1 > 299)
			{
				// Buffer response content
				HttpEntity entity = response.GetEntity();
				if (entity != null)
				{
					response.SetEntity(new BufferedHttpEntity(entity));
				}
				conn.Close();
				throw new TunnelRefusedException("CONNECT refused by proxy: " + response.GetStatusLine
					(), response);
			}
			return conn.GetSocket();
		}
	}
}
