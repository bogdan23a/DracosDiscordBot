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
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Params;
using Apache.Http.Client.Protocol;
using Apache.Http.Client.Utils;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Client;
using Apache.Http.Impl.Execchain;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>
	/// Request executor in the request execution chain that is responsible
	/// for implementation of HTTP specification requirements.
	/// </summary>
	/// <remarks>
	/// Request executor in the request execution chain that is responsible
	/// for implementation of HTTP specification requirements.
	/// Internally this executor relies on a
	/// <see cref="Apache.Http.Protocol.HttpProcessor">Apache.Http.Protocol.HttpProcessor
	/// 	</see>
	/// to populate
	/// requisite HTTP request headers, process HTTP response headers and update
	/// session state in
	/// <see cref="Apache.Http.Client.Protocol.HttpClientContext">Apache.Http.Client.Protocol.HttpClientContext
	/// 	</see>
	/// .
	/// <p/>
	/// Further responsibilities such as communication with the opposite
	/// endpoint is delegated to the next executor in the request execution
	/// chain.
	/// </remarks>
	/// <since>4.3</since>
	public class ProtocolExec : ClientExecChain
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly ClientExecChain requestExecutor;

		private readonly HttpProcessor httpProcessor;

		public ProtocolExec(ClientExecChain requestExecutor, HttpProcessor httpProcessor)
		{
			Args.NotNull(requestExecutor, "HTTP client request executor");
			Args.NotNull(httpProcessor, "HTTP protocol processor");
			this.requestExecutor = requestExecutor;
			this.httpProcessor = httpProcessor;
		}

		/// <exception cref="Apache.Http.ProtocolException"></exception>
		internal virtual void RewriteRequestURI(HttpRequestWrapper request, HttpRoute route
			)
		{
			try
			{
				URI uri = request.GetURI();
				if (uri != null)
				{
					if (route.GetProxyHost() != null && !route.IsTunnelled())
					{
						// Make sure the request URI is absolute
						if (!uri.IsAbsolute())
						{
							HttpHost target = route.GetTargetHost();
							uri = URIUtils.RewriteURI(uri, target, true);
						}
						else
						{
							uri = URIUtils.RewriteURI(uri);
						}
					}
					else
					{
						// Make sure the request URI is relative
						if (uri.IsAbsolute())
						{
							uri = URIUtils.RewriteURI(uri, null, true);
						}
						else
						{
							uri = URIUtils.RewriteURI(uri);
						}
					}
					request.SetURI(uri);
				}
			}
			catch (URISyntaxException ex)
			{
				throw new ProtocolException("Invalid URI: " + request.GetRequestLine().GetUri(), 
					ex);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.HttpException"></exception>
		public virtual CloseableHttpResponse Execute(HttpRoute route, HttpRequestWrapper 
			request, HttpClientContext context, HttpExecutionAware execAware)
		{
			Args.NotNull(route, "HTTP route");
			Args.NotNull(request, "HTTP request");
			Args.NotNull(context, "HTTP context");
			IHttpRequest original = request.GetOriginal();
			URI uri = null;
			if (original is IHttpUriRequest)
			{
				uri = ((IHttpUriRequest)original).GetURI();
			}
			else
			{
				string uriString = original.GetRequestLine().GetUri();
				try
				{
					uri = URI.Create(uriString);
				}
				catch (ArgumentException ex)
				{
					if (this.log.IsDebugEnabled())
					{
						this.log.Debug("Unable to parse '" + uriString + "' as a valid URI; " + "request URI and Host header may be inconsistent"
							, ex);
					}
				}
			}
			request.SetURI(uri);
			// Re-write request URI if needed
			RewriteRequestURI(request, route);
			HttpParams @params = request.GetParams();
			HttpHost virtualHost = (HttpHost)@params.GetParameter(ClientPNames.VirtualHost);
			// HTTPCLIENT-1092 - add the port if necessary
			if (virtualHost != null && virtualHost.GetPort() == -1)
			{
				int port = route.GetTargetHost().GetPort();
				if (port != -1)
				{
					virtualHost = new HttpHost(virtualHost.GetHostName(), port, virtualHost.GetSchemeName
						());
				}
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Using virtual host" + virtualHost);
				}
			}
			HttpHost target = null;
			if (virtualHost != null)
			{
				target = virtualHost;
			}
			else
			{
				if (uri != null && uri.IsAbsolute() && uri.GetHost() != null)
				{
					target = new HttpHost(uri.GetHost(), uri.GetPort(), uri.GetScheme());
				}
			}
			if (target == null)
			{
				target = route.GetTargetHost();
			}
			// Get user info from the URI
			if (uri != null)
			{
				string userinfo = uri.GetUserInfo();
				if (userinfo != null)
				{
					CredentialsProvider credsProvider = context.GetCredentialsProvider();
					if (credsProvider == null)
					{
						credsProvider = new BasicCredentialsProvider();
						context.SetCredentialsProvider(credsProvider);
					}
					credsProvider.SetCredentials(new AuthScope(target), new UsernamePasswordCredentials
						(userinfo));
				}
			}
			// Run request protocol interceptors
			context.SetAttribute(HttpClientContext.HttpTargetHost, target);
			context.SetAttribute(HttpClientContext.HttpRoute, route);
			context.SetAttribute(HttpClientContext.HttpRequest, request);
			this.httpProcessor.Process(request, context);
			CloseableHttpResponse response = this.requestExecutor.Execute(route, request, context
				, execAware);
			try
			{
				// Run response protocol interceptors
				context.SetAttribute(HttpClientContext.HttpResponse, response);
				this.httpProcessor.Process(response, context);
				return response;
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
				response.Close();
				throw;
			}
		}
	}
}
