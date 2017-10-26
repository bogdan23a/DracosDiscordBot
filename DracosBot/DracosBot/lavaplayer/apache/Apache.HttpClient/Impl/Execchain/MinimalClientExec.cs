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
using System.IO;
using System.Threading;
using Apache.Http;
using Apache.Http.Client.Config;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Protocol;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Conn;
using Apache.Http.Impl.Execchain;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>
	/// Request executor that implements the most fundamental aspects of
	/// the HTTP specification and the most straight-forward request / response
	/// exchange with the target server.
	/// </summary>
	/// <remarks>
	/// Request executor that implements the most fundamental aspects of
	/// the HTTP specification and the most straight-forward request / response
	/// exchange with the target server. This executor does not support
	/// execution via proxy and will make no attempts to retry the request
	/// in case of a redirect, authentication challenge or I/O error.
	/// </remarks>
	/// <since>4.3</since>
	public class MinimalClientExec : ClientExecChain
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly HttpRequestExecutor requestExecutor;

		private readonly HttpClientConnectionManager connManager;

		private readonly ConnectionReuseStrategy reuseStrategy;

		private readonly ConnectionKeepAliveStrategy keepAliveStrategy;

		private readonly HttpProcessor httpProcessor;

		public MinimalClientExec(HttpRequestExecutor requestExecutor, HttpClientConnectionManager
			 connManager, ConnectionReuseStrategy reuseStrategy, ConnectionKeepAliveStrategy
			 keepAliveStrategy)
		{
			Args.NotNull(requestExecutor, "HTTP request executor");
			Args.NotNull(connManager, "Client connection manager");
			Args.NotNull(reuseStrategy, "Connection reuse strategy");
			Args.NotNull(keepAliveStrategy, "Connection keep alive strategy");
			this.httpProcessor = new ImmutableHttpProcessor(new RequestContent(), new RequestTargetHost
				(), new RequestClientConnControl(), new RequestUserAgent(VersionInfo.GetUserAgent
				("Apache-HttpClient", "org.apache.http.client", GetType())));
			this.requestExecutor = requestExecutor;
			this.connManager = connManager;
			this.reuseStrategy = reuseStrategy;
			this.keepAliveStrategy = keepAliveStrategy;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.HttpException"></exception>
		public virtual CloseableHttpResponse Execute(HttpRoute route, HttpRequestWrapper 
			request, HttpClientContext context, HttpExecutionAware execAware)
		{
			Args.NotNull(route, "HTTP route");
			Args.NotNull(request, "HTTP request");
			Args.NotNull(context, "HTTP context");
			ConnectionRequest connRequest = connManager.RequestConnection(route, null);
			if (execAware != null)
			{
				if (execAware.IsAborted())
				{
					connRequest.Cancel();
					throw new RequestAbortedException("Request aborted");
				}
				else
				{
					execAware.SetCancellable(connRequest);
				}
			}
			RequestConfig config = context.GetRequestConfig();
			HttpClientConnection managedConn;
			try
			{
				int timeout = config.GetConnectionRequestTimeout();
				managedConn = connRequest.Get(timeout > 0 ? timeout : 0, TimeUnit.Milliseconds);
			}
			catch (Exception interrupted)
			{
				Sharpen.Thread.CurrentThread().Interrupt();
				throw new RequestAbortedException("Request aborted", interrupted);
			}
			catch (ExecutionException ex)
			{
				Exception cause = ex.InnerException;
				if (cause == null)
				{
					cause = ex;
				}
				throw new RequestAbortedException("Request execution failed", cause);
			}
			ConnectionHolder releaseTrigger = new ConnectionHolder(log, connManager, managedConn
				);
			try
			{
				if (execAware != null)
				{
					if (execAware.IsAborted())
					{
						releaseTrigger.Close();
						throw new RequestAbortedException("Request aborted");
					}
					else
					{
						execAware.SetCancellable(releaseTrigger);
					}
				}
				if (!managedConn.IsOpen())
				{
					int timeout = config.GetConnectTimeout();
					this.connManager.Connect(managedConn, route, timeout > 0 ? timeout : 0, context);
					this.connManager.RouteComplete(managedConn, route, context);
				}
				int timeout_1 = config.GetSocketTimeout();
				if (timeout_1 >= 0)
				{
					managedConn.SetSocketTimeout(timeout_1);
				}
				HttpHost target = null;
				IHttpRequest original = request.GetOriginal();
				if (original is IHttpUriRequest)
				{
					URI uri = ((IHttpUriRequest)original).GetURI();
					if (uri.IsAbsolute())
					{
						target = new HttpHost(uri.GetHost(), uri.GetPort(), uri.GetScheme());
					}
				}
				if (target == null)
				{
					target = route.GetTargetHost();
				}
				context.SetAttribute(HttpClientContext.HttpTargetHost, target);
				context.SetAttribute(HttpClientContext.HttpRequest, request);
				context.SetAttribute(HttpClientContext.HttpConnection, managedConn);
				context.SetAttribute(HttpClientContext.HttpRoute, route);
				httpProcessor.Process(request, context);
				HttpResponse response = requestExecutor.Execute(request, managedConn, context);
				httpProcessor.Process(response, context);
				// The connection is in or can be brought to a re-usable state.
				if (reuseStrategy.KeepAlive(response, context))
				{
					// Set the idle duration of this connection
					long duration = keepAliveStrategy.GetKeepAliveDuration(response, context);
					releaseTrigger.SetValidFor(duration, TimeUnit.Milliseconds);
					releaseTrigger.MarkReusable();
				}
				else
				{
					releaseTrigger.MarkNonReusable();
				}
				// check for entity, release connection if possible
				HttpEntity entity = response.GetEntity();
				if (entity == null || !entity.IsStreaming())
				{
					// connection not needed and (assumed to be) in re-usable state
					releaseTrigger.ReleaseConnection();
					return Proxies.EnhanceResponse(response, null);
				}
				else
				{
					return Proxies.EnhanceResponse(response, releaseTrigger);
				}
			}
			catch (ConnectionShutdownException ex)
			{
				ThreadInterruptedException ioex = new ThreadInterruptedException("Connection has been shut down"
					);
				Sharpen.Extensions.InitCause(ioex, ex);
				throw ioex;
			}
			catch (HttpException ex)
			{
				releaseTrigger.AbortConnection();
				throw;
			}
			catch (IOException ex)
			{
				releaseTrigger.AbortConnection();
				throw;
			}
			catch (RuntimeException ex)
			{
				releaseTrigger.AbortConnection();
				throw;
			}
		}
	}
}
