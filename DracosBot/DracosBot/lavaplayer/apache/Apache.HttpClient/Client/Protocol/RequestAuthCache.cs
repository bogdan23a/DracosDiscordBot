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

using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Client.Protocol;
using Apache.Http.Conn.Routing;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Client.Protocol
{
	/// <summary>
	/// Request interceptor that can preemptively authenticate against known hosts,
	/// if there is a cached
	/// <see cref="Apache.Http.Auth.AuthScheme">Apache.Http.Auth.AuthScheme</see>
	/// instance in the local
	/// <see cref="Apache.Http.Client.AuthCache">Apache.Http.Client.AuthCache</see>
	/// associated with the given target or proxy host.
	/// </summary>
	/// <since>4.1</since>
	public class RequestAuthCache : IHttpRequestInterceptor
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		public RequestAuthCache() : base()
		{
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Process(IHttpRequest request, HttpContext context)
		{
			Args.NotNull(request, "HTTP request");
			Args.NotNull(context, "HTTP context");
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			AuthCache authCache = clientContext.GetAuthCache();
			if (authCache == null)
			{
				this.log.Debug("Auth cache not set in the context");
				return;
			}
			CredentialsProvider credsProvider = clientContext.GetCredentialsProvider();
			if (credsProvider == null)
			{
				this.log.Debug("Credentials provider not set in the context");
				return;
			}
			RouteInfo route = clientContext.GetHttpRoute();
			HttpHost target = clientContext.GetTargetHost();
			if (target.GetPort() < 0)
			{
				target = new HttpHost(target.GetHostName(), route.GetTargetHost().GetPort(), target
					.GetSchemeName());
			}
			AuthState targetState = clientContext.GetTargetAuthState();
			if (targetState != null && targetState.GetState() == AuthProtocolState.Unchallenged)
			{
				AuthScheme authScheme = authCache.Get(target);
				if (authScheme != null)
				{
					DoPreemptiveAuth(target, authScheme, targetState, credsProvider);
				}
			}
			HttpHost proxy = route.GetProxyHost();
			AuthState proxyState = clientContext.GetProxyAuthState();
			if (proxy != null && proxyState != null && proxyState.GetState() == AuthProtocolState
				.Unchallenged)
			{
				AuthScheme authScheme = authCache.Get(proxy);
				if (authScheme != null)
				{
					DoPreemptiveAuth(proxy, authScheme, proxyState, credsProvider);
				}
			}
		}

		private void DoPreemptiveAuth(HttpHost host, AuthScheme authScheme, AuthState authState
			, CredentialsProvider credsProvider)
		{
			string schemeName = authScheme.GetSchemeName();
			if (this.log.IsDebugEnabled())
			{
				this.log.Debug("Re-using cached '" + schemeName + "' auth scheme for " + host);
			}
			AuthScope authScope = new AuthScope(host, AuthScope.AnyRealm, schemeName);
			Credentials creds = credsProvider.GetCredentials(authScope);
			if (creds != null)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase("BASIC", authScheme.GetSchemeName()))
				{
					authState.SetState(AuthProtocolState.Challenged);
				}
				else
				{
					authState.SetState(AuthProtocolState.Success);
				}
				authState.Update(authScheme, creds);
			}
			else
			{
				this.log.Debug("No credentials for preemptive authentication");
			}
		}
	}
}
