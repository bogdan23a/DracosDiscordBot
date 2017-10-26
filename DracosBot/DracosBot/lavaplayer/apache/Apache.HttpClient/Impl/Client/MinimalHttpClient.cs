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
using Apache.Http;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Protocol;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Conn.Scheme;
using Apache.Http.Impl;
using Apache.Http.Impl.Client;
using Apache.Http.Impl.Execchain;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>Internal class.</summary>
	/// <remarks>Internal class.</remarks>
	/// <since>4.3</since>
	internal class MinimalHttpClient : CloseableHttpClient
	{
		private readonly HttpClientConnectionManager connManager;

		private readonly MinimalClientExec requestExecutor;

		private readonly HttpParams @params;

		public MinimalHttpClient(HttpClientConnectionManager connManager) : base()
		{
			this.connManager = Args.NotNull(connManager, "HTTP connection manager");
			this.requestExecutor = new MinimalClientExec(new HttpRequestExecutor(), connManager
				, DefaultConnectionReuseStrategy.Instance, DefaultConnectionKeepAliveStrategy.Instance
				);
			this.@params = new BasicHttpParams();
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		protected internal override CloseableHttpResponse DoExecute(HttpHost target, IHttpRequest
			 request, HttpContext context)
		{
			Args.NotNull(target, "Target host");
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
				HttpRoute route = new HttpRoute(target);
				RequestConfig config = null;
				if (request is Configurable)
				{
					config = ((Configurable)request).GetConfig();
				}
				if (config != null)
				{
					localcontext.SetRequestConfig(config);
				}
				return this.requestExecutor.Execute(route, wrapper, localcontext, execAware);
			}
			catch (HttpException httpException)
			{
				throw new ClientProtocolException(httpException);
			}
		}

		public override HttpParams GetParams()
		{
			return this.@params;
		}

		public override void Close()
		{
			this.connManager.Shutdown();
		}

		public override ClientConnectionManager GetConnectionManager()
		{
			return new _ClientConnectionManager_123(this);
		}

		private sealed class _ClientConnectionManager_123 : ClientConnectionManager
		{
			public _ClientConnectionManager_123(MinimalHttpClient _enclosing)
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

			private readonly MinimalHttpClient _enclosing;
		}
	}
}
