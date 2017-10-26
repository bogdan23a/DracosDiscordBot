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

using System.Collections;
using System.Collections.Generic;
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Config;
using Apache.Http.Conn.Routing;
using Apache.Http.Cookie;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Client.Protocol
{
	/// <summary>
	/// Adaptor class that provides convenience type safe setters and getters
	/// for common
	/// <see cref="Apache.Http.Protocol.HttpContext">Apache.Http.Protocol.HttpContext</see>
	/// attributes used in the course
	/// of HTTP request execution.
	/// </summary>
	/// <since>4.3</since>
	public class HttpClientContext : HttpCoreContext
	{
		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Conn.Routing.RouteInfo">Apache.Http.Conn.Routing.RouteInfo
		/// 	</see>
		/// object that represents the actual connection route.
		/// </summary>
		public const string HttpRoute = "http.route";

		/// <summary>
		/// Attribute name of a
		/// <see cref="System.Collections.IList{E}">System.Collections.IList&lt;E&gt;</see>
		/// object that represents a collection of all
		/// redirect locations received in the process of request execution.
		/// </summary>
		public const string RedirectLocations = "http.protocol.redirect-locations";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Config.Lookup{I}">Apache.Http.Config.Lookup&lt;I&gt;</see>
		/// object that represents
		/// the actual
		/// <see cref="Apache.Http.Cookie.CookieSpecProvider">Apache.Http.Cookie.CookieSpecProvider
		/// 	</see>
		/// registry.
		/// </summary>
		public const string CookiespecRegistry = "http.cookiespec-registry";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Cookie.CookieSpec">Apache.Http.Cookie.CookieSpec</see>
		/// object that represents the actual cookie specification.
		/// </summary>
		public const string CookieSpec = "http.cookie-spec";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Cookie.CookieOrigin">Apache.Http.Cookie.CookieOrigin</see>
		/// object that represents the actual details of the origin server.
		/// </summary>
		public const string CookieOrigin = "http.cookie-origin";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Client.CookieStore">Apache.Http.Client.CookieStore</see>
		/// object that represents the actual cookie store.
		/// </summary>
		public const string CookieStore = "http.cookie-store";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Client.CredentialsProvider">Apache.Http.Client.CredentialsProvider
		/// 	</see>
		/// object that represents the actual credentials provider.
		/// </summary>
		public const string CredsProvider = "http.auth.credentials-provider";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Client.AuthCache">Apache.Http.Client.AuthCache</see>
		/// object
		/// that represents the auth scheme cache.
		/// </summary>
		public const string AuthCache = "http.auth.auth-cache";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Auth.AuthState">Apache.Http.Auth.AuthState</see>
		/// object that represents the actual target authentication state.
		/// </summary>
		public const string TargetAuthState = "http.auth.target-scope";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Auth.AuthState">Apache.Http.Auth.AuthState</see>
		/// object that represents the actual proxy authentication state.
		/// </summary>
		public const string ProxyAuthState = "http.auth.proxy-scope";

		/// <summary>
		/// Attribute name of a
		/// <see cref="object">object</see>
		/// object that represents
		/// the actual user identity such as user
		/// <see cref="Sharpen.Principal">Sharpen.Principal</see>
		/// .
		/// </summary>
		public const string UserToken = "http.user-token";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Config.Lookup{I}">Apache.Http.Config.Lookup&lt;I&gt;</see>
		/// object that represents
		/// the actual
		/// <see cref="Apache.Http.Auth.AuthSchemeProvider">Apache.Http.Auth.AuthSchemeProvider
		/// 	</see>
		/// registry.
		/// </summary>
		public const string AuthschemeRegistry = "http.authscheme-registry";

		/// <summary>
		/// Attribute name of a
		/// <see cref="Apache.Http.Client.Config.RequestConfig">Apache.Http.Client.Config.RequestConfig
		/// 	</see>
		/// object that
		/// represents the actual request configuration.
		/// </summary>
		public const string RequestConfig = "http.request-config";

		public static HttpCoreContext Adapt(HttpContext context)
		{
			if (context is Apache.Http.Client.Protocol.HttpClientContext)
			{
				return (Apache.Http.Client.Protocol.HttpClientContext)context;
			}
			else
			{
				return new Apache.Http.Client.Protocol.HttpClientContext(context);
			}
		}

		public static HttpCoreContext Create()
		{
			return new Apache.Http.Client.Protocol.HttpClientContext(new BasicHttpContext());
		}

		public HttpClientContext(HttpContext context) : base(context)
		{
		}

		public HttpClientContext() : base()
		{
		}

		public virtual RouteInfo GetHttpRoute()
		{
			return GetAttribute<HttpRoute>(HttpRoute);
		}

		public virtual IList<URI> GetRedirectLocations()
		{
			return GetAttribute<IList>(RedirectLocations);
		}

		public virtual CookieStore GetCookieStore()
		{
			return GetAttribute<CookieStore>(CookieStore);
		}

		public virtual void SetCookieStore(CookieStore cookieStore)
		{
			SetAttribute(CookieStore, cookieStore);
		}

		public virtual CookieSpec GetCookieSpec()
		{
			return GetAttribute<CookieSpec>(CookieSpec);
		}

		public virtual CookieOrigin GetCookieOrigin()
		{
			return GetAttribute<CookieOrigin>(CookieOrigin);
		}

		private Lookup<T> GetLookup<T>(string name)
		{
			System.Type clazz = typeof(T);
			return GetAttribute<Lookup>(name);
		}

		public virtual Lookup<CookieSpecProvider> GetCookieSpecRegistry()
		{
			return GetLookup<CookieSpecProvider>(CookiespecRegistry);
		}

		public virtual void SetCookieSpecRegistry(Lookup<CookieSpecProvider> lookup)
		{
			SetAttribute(CookiespecRegistry, lookup);
		}

		public virtual Lookup<AuthSchemeProvider> GetAuthSchemeRegistry()
		{
			return GetLookup<AuthSchemeProvider>(AuthschemeRegistry);
		}

		public virtual void SetAuthSchemeRegistry(Lookup<AuthSchemeProvider> lookup)
		{
			SetAttribute(AuthschemeRegistry, lookup);
		}

		public virtual CredentialsProvider GetCredentialsProvider()
		{
			return GetAttribute<CredentialsProvider>(CredsProvider);
		}

		public virtual void SetCredentialsProvider(CredentialsProvider credentialsProvider
			)
		{
			SetAttribute(CredsProvider, credentialsProvider);
		}

		public virtual AuthCache GetAuthCache()
		{
			return GetAttribute<AuthCache>(AuthCache);
		}

		public virtual void SetAuthCache(AuthCache authCache)
		{
			SetAttribute(AuthCache, authCache);
		}

		public virtual AuthState GetTargetAuthState()
		{
			return GetAttribute<AuthState>(TargetAuthState);
		}

		public virtual AuthState GetProxyAuthState()
		{
			return GetAttribute<AuthState>(ProxyAuthState);
		}

		public virtual T GetUserToken<T>()
		{
			System.Type clazz = typeof(T);
			return GetAttribute(UserToken, clazz);
		}

		public virtual object GetUserToken()
		{
			return GetAttribute(UserToken);
		}

		public virtual void SetUserToken(object obj)
		{
			SetAttribute(UserToken, obj);
		}

		public virtual RequestConfig GetRequestConfig()
		{
			RequestConfig config = GetAttribute<RequestConfig>(RequestConfig);
			return config != null ? config : RequestConfig.Default;
		}

		public virtual void SetRequestConfig(RequestConfig config)
		{
			SetAttribute(RequestConfig, config);
		}
	}
}
