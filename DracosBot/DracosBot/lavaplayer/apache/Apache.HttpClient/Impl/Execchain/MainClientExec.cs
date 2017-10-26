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
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Protocol;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Entity;
using Apache.Http.Impl.Auth;
using Apache.Http.Impl.Conn;
using Apache.Http.Impl.Execchain;
using Apache.Http.Message;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>
	/// The last request executor in the HTTP request execution chain
	/// that is responsible for execution of request / response
	/// exchanges with the opposite endpoint.
	/// </summary>
	/// <remarks>
	/// The last request executor in the HTTP request execution chain
	/// that is responsible for execution of request / response
	/// exchanges with the opposite endpoint.
	/// This executor will automatically retry the request in case
	/// of an authentication challenge by an intermediate proxy or
	/// by the target server.
	/// </remarks>
	/// <since>4.3</since>
	public class MainClientExec : ClientExecChain
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly HttpRequestExecutor requestExecutor;

		private readonly HttpClientConnectionManager connManager;

		private readonly ConnectionReuseStrategy reuseStrategy;

		private readonly ConnectionKeepAliveStrategy keepAliveStrategy;

		private readonly HttpProcessor proxyHttpProcessor;

		private readonly AuthenticationStrategy targetAuthStrategy;

		private readonly AuthenticationStrategy proxyAuthStrategy;

		private readonly HttpAuthenticator authenticator;

		private readonly UserTokenHandler userTokenHandler;

		private readonly HttpRouteDirector routeDirector;

		public MainClientExec(HttpRequestExecutor requestExecutor, HttpClientConnectionManager
			 connManager, ConnectionReuseStrategy reuseStrategy, ConnectionKeepAliveStrategy
			 keepAliveStrategy, AuthenticationStrategy targetAuthStrategy, AuthenticationStrategy
			 proxyAuthStrategy, UserTokenHandler userTokenHandler)
		{
			Args.NotNull(requestExecutor, "HTTP request executor");
			Args.NotNull(connManager, "Client connection manager");
			Args.NotNull(reuseStrategy, "Connection reuse strategy");
			Args.NotNull(keepAliveStrategy, "Connection keep alive strategy");
			Args.NotNull(targetAuthStrategy, "Target authentication strategy");
			Args.NotNull(proxyAuthStrategy, "Proxy authentication strategy");
			Args.NotNull(userTokenHandler, "User token handler");
			this.authenticator = new HttpAuthenticator();
			this.proxyHttpProcessor = new ImmutableHttpProcessor(new RequestTargetHost(), new 
				RequestClientConnControl());
			this.routeDirector = new BasicRouteDirector();
			this.requestExecutor = requestExecutor;
			this.connManager = connManager;
			this.reuseStrategy = reuseStrategy;
			this.keepAliveStrategy = keepAliveStrategy;
			this.targetAuthStrategy = targetAuthStrategy;
			this.proxyAuthStrategy = proxyAuthStrategy;
			this.userTokenHandler = userTokenHandler;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.HttpException"></exception>
		public virtual CloseableHttpResponse Execute(HttpRoute route, HttpRequestWrapper 
			request, HttpClientContext context, HttpExecutionAware execAware)
		{
			Args.NotNull(route, "HTTP route");
			Args.NotNull(request, "HTTP request");
			Args.NotNull(context, "HTTP context");
			AuthState targetAuthState = context.GetTargetAuthState();
			if (targetAuthState == null)
			{
				targetAuthState = new AuthState();
				context.SetAttribute(HttpClientContext.TargetAuthState, targetAuthState);
			}
			AuthState proxyAuthState = context.GetProxyAuthState();
			if (proxyAuthState == null)
			{
				proxyAuthState = new AuthState();
				context.SetAttribute(HttpClientContext.ProxyAuthState, proxyAuthState);
			}
			if (request is HttpEntityEnclosingRequest)
			{
				Proxies.EnhanceEntity((HttpEntityEnclosingRequest)request);
			}
			object userToken = context.GetUserToken();
			ConnectionRequest connRequest = connManager.RequestConnection(route, userToken);
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
			context.SetAttribute(HttpClientContext.HttpConnection, managedConn);
			if (config.IsStaleConnectionCheckEnabled())
			{
				// validate connection
				if (managedConn.IsOpen())
				{
					this.log.Debug("Stale connection check");
					if (managedConn.IsStale())
					{
						this.log.Debug("Stale connection detected");
						managedConn.Close();
					}
				}
			}
			ConnectionHolder connHolder = new ConnectionHolder(this.log, this.connManager, managedConn
				);
			try
			{
				if (execAware != null)
				{
					execAware.SetCancellable(connHolder);
				}
				HttpResponse response;
				for (int execCount = 1; ; execCount++)
				{
					if (execCount > 1 && !Proxies.IsRepeatable(request))
					{
						throw new NonRepeatableRequestException("Cannot retry request " + "with a non-repeatable request entity."
							);
					}
					if (execAware != null && execAware.IsAborted())
					{
						throw new RequestAbortedException("Request aborted");
					}
					if (!managedConn.IsOpen())
					{
						this.log.Debug("Opening connection " + route);
						try
						{
							EstablishRoute(proxyAuthState, managedConn, route, request, context);
						}
						catch (TunnelRefusedException ex)
						{
							if (this.log.IsDebugEnabled())
							{
								this.log.Debug(ex.Message);
							}
							response = ex.GetResponse();
							break;
						}
					}
					int timeout = config.GetSocketTimeout();
					if (timeout >= 0)
					{
						managedConn.SetSocketTimeout(timeout);
					}
					if (execAware != null && execAware.IsAborted())
					{
						throw new RequestAbortedException("Request aborted");
					}
					if (this.log.IsDebugEnabled())
					{
						this.log.Debug("Executing request " + request.GetRequestLine());
					}
					if (!request.ContainsHeader(AUTH.WwwAuthResp))
					{
						if (this.log.IsDebugEnabled())
						{
							this.log.Debug("Target auth state: " + targetAuthState.GetState());
						}
						this.authenticator.GenerateAuthResponse(request, targetAuthState, context);
					}
					if (!request.ContainsHeader(AUTH.ProxyAuthResp) && !route.IsTunnelled())
					{
						if (this.log.IsDebugEnabled())
						{
							this.log.Debug("Proxy auth state: " + proxyAuthState.GetState());
						}
						this.authenticator.GenerateAuthResponse(request, proxyAuthState, context);
					}
					response = requestExecutor.Execute(request, managedConn, context);
					// The connection is in or can be brought to a re-usable state.
					if (reuseStrategy.KeepAlive(response, context))
					{
						// Set the idle duration of this connection
						long duration = keepAliveStrategy.GetKeepAliveDuration(response, context);
						if (this.log.IsDebugEnabled())
						{
							string s;
							if (duration > 0)
							{
								s = "for " + duration + " " + TimeUnit.Milliseconds;
							}
							else
							{
								s = "indefinitely";
							}
							this.log.Debug("Connection can be kept alive " + s);
						}
						connHolder.SetValidFor(duration, TimeUnit.Milliseconds);
						connHolder.MarkReusable();
					}
					else
					{
						connHolder.MarkNonReusable();
					}
					if (NeedAuthentication(targetAuthState, proxyAuthState, route, response, context))
					{
						// Make sure the response body is fully consumed, if present
						HttpEntity entity = response.GetEntity();
						if (connHolder.IsReusable())
						{
							EntityUtils.Consume(entity);
						}
						else
						{
							managedConn.Close();
							if (proxyAuthState.GetState() == AuthProtocolState.Success && proxyAuthState.GetAuthScheme
								() != null && proxyAuthState.GetAuthScheme().IsConnectionBased())
							{
								this.log.Debug("Resetting proxy auth state");
								proxyAuthState.Reset();
							}
							if (targetAuthState.GetState() == AuthProtocolState.Success && targetAuthState.GetAuthScheme
								() != null && targetAuthState.GetAuthScheme().IsConnectionBased())
							{
								this.log.Debug("Resetting target auth state");
								targetAuthState.Reset();
							}
						}
						// discard previous auth headers
						IHttpRequest original = request.GetOriginal();
						if (!original.ContainsHeader(AUTH.WwwAuthResp))
						{
							request.RemoveHeaders(AUTH.WwwAuthResp);
						}
						if (!original.ContainsHeader(AUTH.ProxyAuthResp))
						{
							request.RemoveHeaders(AUTH.ProxyAuthResp);
						}
					}
					else
					{
						break;
					}
				}
				if (userToken == null)
				{
					userToken = userTokenHandler.GetUserToken(context);
					context.SetAttribute(HttpClientContext.UserToken, userToken);
				}
				if (userToken != null)
				{
					connHolder.SetState(userToken);
				}
				// check for entity, release connection if possible
				HttpEntity entity_1 = response.GetEntity();
				if (entity_1 == null || !entity_1.IsStreaming())
				{
					// connection not needed and (assumed to be) in re-usable state
					connHolder.ReleaseConnection();
					return Proxies.EnhanceResponse(response, null);
				}
				else
				{
					return Proxies.EnhanceResponse(response, connHolder);
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
				connHolder.AbortConnection();
				throw;
			}
			catch (IOException ex)
			{
				connHolder.AbortConnection();
				throw;
			}
			catch (RuntimeException ex)
			{
				connHolder.AbortConnection();
				throw;
			}
		}

		/// <summary>Establishes the target route.</summary>
		/// <remarks>Establishes the target route.</remarks>
		/// <exception cref="Apache.Http.HttpException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		internal virtual void EstablishRoute(AuthState proxyAuthState, HttpClientConnection
			 managedConn, HttpRoute route, IHttpRequest request, HttpClientContext context)
		{
			RequestConfig config = context.GetRequestConfig();
			int timeout = config.GetConnectTimeout();
			RouteTracker tracker = new RouteTracker(route);
			int step;
			do
			{
				HttpRoute fact = tracker.ToRoute();
				step = this.routeDirector.NextStep(route, fact);
				switch (step)
				{
					case HttpRouteDirector.ConnectTarget:
					{
						this.connManager.Connect(managedConn, route, timeout > 0 ? timeout : 0, context);
						tracker.ConnectTarget(route.IsSecure());
						break;
					}

					case HttpRouteDirector.ConnectProxy:
					{
						this.connManager.Connect(managedConn, route, timeout > 0 ? timeout : 0, context);
						HttpHost proxy = route.GetProxyHost();
						tracker.ConnectProxy(proxy, false);
						break;
					}

					case HttpRouteDirector.TunnelTarget:
					{
						bool secure = CreateTunnelToTarget(proxyAuthState, managedConn, route, request, context
							);
						this.log.Debug("Tunnel to target created.");
						tracker.TunnelTarget(secure);
						break;
					}

					case HttpRouteDirector.TunnelProxy:
					{
						// The most simple example for this case is a proxy chain
						// of two proxies, where P1 must be tunnelled to P2.
						// route: Source -> P1 -> P2 -> Target (3 hops)
						// fact:  Source -> P1 -> Target       (2 hops)
						int hop = fact.GetHopCount() - 1;
						// the hop to establish
						bool secure = CreateTunnelToProxy(route, hop, context);
						this.log.Debug("Tunnel to proxy created.");
						tracker.TunnelProxy(route.GetHopTarget(hop), secure);
						break;
					}

					case HttpRouteDirector.LayerProtocol:
					{
						this.connManager.Upgrade(managedConn, route, context);
						tracker.LayerProtocol(route.IsSecure());
						break;
					}

					case HttpRouteDirector.Unreachable:
					{
						throw new HttpException("Unable to establish route: " + "planned = " + route + "; current = "
							 + fact);
					}

					case HttpRouteDirector.Complete:
					{
						this.connManager.RouteComplete(managedConn, route, context);
						break;
					}

					default:
					{
						throw new InvalidOperationException("Unknown step indicator " + step + " from RouteDirector."
							);
					}
				}
			}
			while (step > HttpRouteDirector.Complete);
		}

		/// <summary>Creates a tunnel to the target server.</summary>
		/// <remarks>
		/// Creates a tunnel to the target server.
		/// The connection must be established to the (last) proxy.
		/// A CONNECT request for tunnelling through the proxy will
		/// be created and sent, the response received and checked.
		/// This method does <i>not</i> update the connection with
		/// information about the tunnel, that is left to the caller.
		/// </remarks>
		/// <exception cref="Apache.Http.HttpException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		private bool CreateTunnelToTarget(AuthState proxyAuthState, HttpClientConnection 
			managedConn, HttpRoute route, IHttpRequest request, HttpClientContext context)
		{
			RequestConfig config = context.GetRequestConfig();
			int timeout = config.GetConnectTimeout();
			HttpHost target = route.GetTargetHost();
			HttpHost proxy = route.GetProxyHost();
			HttpResponse response;
			string authority = target.ToHostString();
			IHttpRequest connect = new BasicHttpRequest("CONNECT", authority, request.GetProtocolVersion
				());
			this.requestExecutor.PreProcess(connect, this.proxyHttpProcessor, context);
			for (; ; )
			{
				if (!managedConn.IsOpen())
				{
					this.connManager.Connect(managedConn, route, timeout > 0 ? timeout : 0, context);
				}
				connect.RemoveHeaders(AUTH.ProxyAuthResp);
				this.authenticator.GenerateAuthResponse(connect, proxyAuthState, context);
				response = this.requestExecutor.Execute(connect, managedConn, context);
				int status = response.GetStatusLine().GetStatusCode();
				if (status < 200)
				{
					throw new HttpException("Unexpected response to CONNECT request: " + response.GetStatusLine
						());
				}
				if (config.IsAuthenticationEnabled())
				{
					if (this.authenticator.IsAuthenticationRequested(proxy, response, this.proxyAuthStrategy
						, proxyAuthState, context))
					{
						if (this.authenticator.HandleAuthChallenge(proxy, response, this.proxyAuthStrategy
							, proxyAuthState, context))
						{
							// Retry request
							if (this.reuseStrategy.KeepAlive(response, context))
							{
								this.log.Debug("Connection kept alive");
								// Consume response content
								HttpEntity entity = response.GetEntity();
								EntityUtils.Consume(entity);
							}
							else
							{
								managedConn.Close();
							}
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
				managedConn.Close();
				throw new TunnelRefusedException("CONNECT refused by proxy: " + response.GetStatusLine
					(), response);
			}
			// How to decide on security of the tunnelled connection?
			// The socket factory knows only about the segment to the proxy.
			// Even if that is secure, the hop to the target may be insecure.
			// Leave it to derived classes, consider insecure by default here.
			return false;
		}

		/// <summary>Creates a tunnel to an intermediate proxy.</summary>
		/// <remarks>
		/// Creates a tunnel to an intermediate proxy.
		/// This method is <i>not</i> implemented in this class.
		/// It just throws an exception here.
		/// </remarks>
		/// <exception cref="Apache.Http.HttpException"></exception>
		private bool CreateTunnelToProxy(HttpRoute route, int hop, HttpClientContext context
			)
		{
			// Have a look at createTunnelToTarget and replicate the parts
			// you need in a custom derived class. If your proxies don't require
			// authentication, it is not too hard. But for the stock version of
			// HttpClient, we cannot make such simplifying assumptions and would
			// have to include proxy authentication code. The HttpComponents team
			// is currently not in a position to support rarely used code of this
			// complexity. Feel free to submit patches that refactor the code in
			// createTunnelToTarget to facilitate re-use for proxy tunnelling.
			throw new HttpException("Proxy chains are not supported.");
		}

		private bool NeedAuthentication(AuthState targetAuthState, AuthState proxyAuthState
			, HttpRoute route, HttpResponse response, HttpClientContext context)
		{
			RequestConfig config = context.GetRequestConfig();
			if (config.IsAuthenticationEnabled())
			{
				HttpHost target = context.GetTargetHost();
				if (target == null)
				{
					target = route.GetTargetHost();
				}
				if (target.GetPort() < 0)
				{
					target = new HttpHost(target.GetHostName(), route.GetTargetHost().GetPort(), target
						.GetSchemeName());
				}
				if (this.authenticator.IsAuthenticationRequested(target, response, this.targetAuthStrategy
					, targetAuthState, context))
				{
					return this.authenticator.HandleAuthChallenge(target, response, this.targetAuthStrategy
						, targetAuthState, context);
				}
				HttpHost proxy = route.GetProxyHost();
				if (this.authenticator.IsAuthenticationRequested(proxy, response, this.proxyAuthStrategy
					, proxyAuthState, context))
				{
					// if proxy is not set use target host instead
					if (proxy == null)
					{
						proxy = route.GetTargetHost();
					}
					return this.authenticator.HandleAuthChallenge(proxy, response, this.proxyAuthStrategy
						, proxyAuthState, context);
				}
			}
			return false;
		}
	}
}
