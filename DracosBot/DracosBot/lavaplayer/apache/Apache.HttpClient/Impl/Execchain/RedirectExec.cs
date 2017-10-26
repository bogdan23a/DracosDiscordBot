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
using System.IO;
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Protocol;
using Apache.Http.Client.Utils;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Execchain;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>
	/// Request executor in the request execution chain that is responsible
	/// for handling of request redirects.
	/// </summary>
	/// <remarks>
	/// Request executor in the request execution chain that is responsible
	/// for handling of request redirects.
	/// <p/>
	/// Further responsibilities such as communication with the opposite
	/// endpoint is delegated to the next executor in the request execution
	/// chain.
	/// </remarks>
	/// <since>4.3</since>
	public class RedirectExec : ClientExecChain
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly ClientExecChain requestExecutor;

		private readonly RedirectStrategy redirectStrategy;

		private readonly HttpRoutePlanner routePlanner;

		public RedirectExec(ClientExecChain requestExecutor, HttpRoutePlanner routePlanner
			, RedirectStrategy redirectStrategy) : base()
		{
			Args.NotNull(requestExecutor, "HTTP client request executor");
			Args.NotNull(routePlanner, "HTTP route planner");
			Args.NotNull(redirectStrategy, "HTTP redirect strategy");
			this.requestExecutor = requestExecutor;
			this.routePlanner = routePlanner;
			this.redirectStrategy = redirectStrategy;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.HttpException"></exception>
		public virtual CloseableHttpResponse Execute(HttpRoute route, HttpRequestWrapper 
			request, HttpClientContext context, HttpExecutionAware execAware)
		{
			Args.NotNull(route, "HTTP route");
			Args.NotNull(request, "HTTP request");
			Args.NotNull(context, "HTTP context");
			IList<URI> redirectLocations = context.GetRedirectLocations();
			if (redirectLocations != null)
			{
				redirectLocations.Clear();
			}
			RequestConfig config = context.GetRequestConfig();
			int maxRedirects = config.GetMaxRedirects() > 0 ? config.GetMaxRedirects() : 50;
			HttpRoute currentRoute = route;
			HttpRequestWrapper currentRequest = request;
			for (int redirectCount = 0; ; )
			{
				CloseableHttpResponse response = requestExecutor.Execute(currentRoute, currentRequest
					, context, execAware);
				try
				{
					if (config.IsRedirectsEnabled() && this.redirectStrategy.IsRedirected(currentRequest
						, response, context))
					{
						if (redirectCount >= maxRedirects)
						{
							throw new RedirectException("Maximum redirects (" + maxRedirects + ") exceeded");
						}
						redirectCount++;
						IHttpRequest redirect = this.redirectStrategy.GetRedirect(currentRequest, response
							, context);
						if (!redirect.HeaderIterator().HasNext())
						{
							IHttpRequest original = request.GetOriginal();
							redirect.SetHeaders(original.GetAllHeaders());
						}
						currentRequest = HttpRequestWrapper.Wrap(redirect);
						if (currentRequest is HttpEntityEnclosingRequest)
						{
							Proxies.EnhanceEntity((HttpEntityEnclosingRequest)currentRequest);
						}
						URI uri = currentRequest.GetURI();
						HttpHost newTarget = URIUtils.ExtractHost(uri);
						if (newTarget == null)
						{
							throw new ProtocolException("Redirect URI does not specify a valid host name: " +
								 uri);
						}
						// Reset virtual host and auth states if redirecting to another host
						if (!currentRoute.GetTargetHost().Equals(newTarget))
						{
							AuthState targetAuthState = context.GetTargetAuthState();
							if (targetAuthState != null)
							{
								this.log.Debug("Resetting target auth state");
								targetAuthState.Reset();
							}
							AuthState proxyAuthState = context.GetProxyAuthState();
							if (proxyAuthState != null)
							{
								AuthScheme authScheme = proxyAuthState.GetAuthScheme();
								if (authScheme != null && authScheme.IsConnectionBased())
								{
									this.log.Debug("Resetting proxy auth state");
									proxyAuthState.Reset();
								}
							}
						}
						currentRoute = this.routePlanner.DetermineRoute(newTarget, currentRequest, context
							);
						if (this.log.IsDebugEnabled())
						{
							this.log.Debug("Redirecting to '" + uri + "' via " + currentRoute);
						}
						EntityUtils.Consume(response.GetEntity());
						response.Close();
					}
					else
					{
						return response;
					}
				}
				catch (RuntimeException ex)
				{
					response.Close();
					throw;
				}
				catch (IOException ex)
				{
					response.Close();
					throw;
				}
				catch (HttpException ex)
				{
					// Protocol exception related to a direct.
					// The underlying connection may still be salvaged.
					try
					{
						EntityUtils.Consume(response.GetEntity());
					}
					catch (IOException ioex)
					{
						this.log.Debug("I/O error while releasing connection", ioex);
					}
					finally
					{
						response.Close();
					}
					throw;
				}
			}
		}
	}
}
