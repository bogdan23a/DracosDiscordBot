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
using System.IO;
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Params;
using Apache.Http.Client.Protocol;
using Apache.Http.Config;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Conn.Scheme;
using Apache.Http.Cookie;
using Apache.Http.Impl.Client;
using Apache.Http.Impl.Execchain;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>Internal class.</summary>
	/// <remarks>Internal class.</remarks>
	/// <since>4.3</since>
	internal class InternalHttpClient : CloseableHttpClient
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly ClientExecChain execChain;

		private readonly HttpClientConnectionManager connManager;

		private readonly HttpRoutePlanner routePlanner;

		private readonly Lookup<CookieSpecProvider> cookieSpecRegistry;

		private readonly Lookup<AuthSchemeProvider> authSchemeRegistry;

		private readonly CookieStore cookieStore;

		private readonly CredentialsProvider credentialsProvider;

		private readonly RequestConfig defaultConfig;

		private readonly IList<IDisposable> closeables;

		public InternalHttpClient(ClientExecChain execChain, HttpClientConnectionManager 
			connManager, HttpRoutePlanner routePlanner, Lookup<CookieSpecProvider> cookieSpecRegistry
			, Lookup<AuthSchemeProvider> authSchemeRegistry, CookieStore cookieStore, CredentialsProvider
			 credentialsProvider, RequestConfig defaultConfig, IList<IDisposable> closeables
			) : base()
		{
			Args.NotNull(execChain, "HTTP client exec chain");
			Args.NotNull(connManager, "HTTP connection manager");
			Args.NotNull(routePlanner, "HTTP route planner");
			this.execChain = execChain;
			this.connManager = connManager;
			this.routePlanner = routePlanner;
			this.cookieSpecRegistry = cookieSpecRegistry;
			this.authSchemeRegistry = authSchemeRegistry;
			this.cookieStore = cookieStore;
			this.credentialsProvider = credentialsProvider;
			this.defaultConfig = defaultConfig;
			this.closeables = closeables;
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		private HttpRoute DetermineRoute(HttpHost target, IHttpRequest request, HttpContext
			 context)
		{
			HttpHost host = target;
			if (host == null)
			{
				host = (HttpHost)request.GetParams().GetParameter(ClientPNames.DefaultHost);
			}
			Asserts.NotNull(host, "Target host");
			return this.routePlanner.DetermineRoute(host, request, context);
		}

		private void SetupContext(HttpClientContext context)
		{
			if (context.GetAttribute(HttpClientContext.TargetAuthState) == null)
			{
				context.SetAttribute(HttpClientContext.TargetAuthState, new AuthState());
			}
			if (context.GetAttribute(HttpClientContext.ProxyAuthState) == null)
			{
				context.SetAttribute(HttpClientContext.ProxyAuthState, new AuthState());
			}
			if (context.GetAttribute(HttpClientContext.AuthschemeRegistry) == null)
			{
				context.SetAttribute(HttpClientContext.AuthschemeRegistry, this.authSchemeRegistry
					);
			}
			if (context.GetAttribute(HttpClientContext.CookiespecRegistry) == null)
			{
				context.SetAttribute(HttpClientContext.CookiespecRegistry, this.cookieSpecRegistry
					);
			}
			if (context.GetAttribute(HttpClientContext.CookieStore) == null)
			{
				context.SetAttribute(HttpClientContext.CookieStore, this.cookieStore);
			}
			if (context.GetAttribute(HttpClientContext.CredsProvider) == null)
			{
				context.SetAttribute(HttpClientContext.CredsProvider, this.credentialsProvider);
			}
			if (context.GetAttribute(HttpClientContext.RequestConfig) == null)
			{
				context.SetAttribute(HttpClientContext.RequestConfig, this.defaultConfig);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		protected internal override CloseableHttpResponse DoExecute(HttpHost target, IHttpRequest
			 request, HttpContext context)
		{
			Args.NotNull(request, "HTTP request");
			HttpExecutionAware execAware = null;
			if (request is HttpExecutionAware)
			{
				execAware = (HttpExecutionAware)request;
			}
			try
			{
				HttpRequestWrapper wrapper = HttpRequestWrapper.Wrap(request);
				HttpClientContext localcontext = ((HttpClientContext)HttpClientContext.Adapt(context
					 != null ? context : new BasicHttpContext()));
				RequestConfig config = null;
				if (request is Configurable)
				{
					config = ((Configurable)request).GetConfig();
				}
				if (config == null)
				{
					HttpParams @params = request.GetParams();
					if (@params is HttpParamsNames)
					{
						if (!((HttpParamsNames)@params).GetNames().IsEmpty())
						{
							config = HttpClientParamConfig.GetRequestConfig(@params);
						}
					}
					else
					{
						config = HttpClientParamConfig.GetRequestConfig(@params);
					}
				}
				if (config != null)
				{
					localcontext.SetRequestConfig(config);
				}
				SetupContext(localcontext);
				HttpRoute route = DetermineRoute(target, wrapper, localcontext);
				return this.execChain.Execute(route, wrapper, localcontext, execAware);
			}
			catch (HttpException httpException)
			{
				throw new ClientProtocolException(httpException);
			}
		}

		public override void Close()
		{
			this.connManager.Shutdown();
			if (this.closeables != null)
			{
				foreach (IDisposable closeable in this.closeables)
				{
					try
					{
						closeable.Close();
					}
					catch (IOException ex)
					{
						this.log.Error(ex.Message, ex);
					}
				}
			}
		}

		public override HttpParams GetParams()
		{
			throw new NotSupportedException();
		}

		public override ClientConnectionManager GetConnectionManager()
		{
			return new _ClientConnectionManager_211(this);
		}

		private sealed class _ClientConnectionManager_211 : ClientConnectionManager
		{
			public _ClientConnectionManager_211(InternalHttpClient _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Shutdown()
			{
				this._enclosing.connManager.Shutdown();
			}

			public ClientConnectionRequest RequestConnection(HttpRoute route, object state)
			{
				throw new NotSupportedException();
			}

			public void ReleaseConnection(ManagedClientConnection conn, long validDuration, TimeUnit
				 timeUnit)
			{
				throw new NotSupportedException();
			}

			public SchemeRegistry GetSchemeRegistry()
			{
				throw new NotSupportedException();
			}

			public void CloseIdleConnections(long idletime, TimeUnit tunit)
			{
				this._enclosing.connManager.CloseIdleConnections(idletime, tunit);
			}

			public void CloseExpiredConnections()
			{
				this._enclosing.connManager.CloseExpiredConnections();
			}

			private readonly InternalHttpClient _enclosing;
		}
	}
}
