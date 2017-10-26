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
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Client.Protocol;
using Apache.Http.Config;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Conn.Socket;
using Apache.Http.Conn.Ssl;
using Apache.Http.Cookie;
using Apache.Http.Impl;
using Apache.Http.Impl.Auth;
using Apache.Http.Impl.Client;
using Apache.Http.Impl.Conn;
using Apache.Http.Impl.Cookie;
using Apache.Http.Impl.Execchain;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Builder for
	/// <see cref="CloseableHttpClient">CloseableHttpClient</see>
	/// instances.
	/// <p/>
	/// When a particular component is not explicitly this class will
	/// use its default implementation. System properties will be taken
	/// into account when configuring the default implementations when
	/// <see cref="UseSystemProperties()">UseSystemProperties()</see>
	/// method is called prior to calling
	/// <see cref="Build()">Build()</see>
	/// .
	/// <ul>
	/// <li>ssl.TrustManagerFactory.algorithm</li>
	/// <li>javax.net.ssl.trustStoreType</li>
	/// <li>javax.net.ssl.trustStore</li>
	/// <li>javax.net.ssl.trustStoreProvider</li>
	/// <li>javax.net.ssl.trustStorePassword</li>
	/// <li>ssl.KeyManagerFactory.algorithm</li>
	/// <li>javax.net.ssl.keyStoreType</li>
	/// <li>javax.net.ssl.keyStore</li>
	/// <li>javax.net.ssl.keyStoreProvider</li>
	/// <li>javax.net.ssl.keyStorePassword</li>
	/// <li>https.protocols</li>
	/// <li>https.cipherSuites</li>
	/// <li>http.proxyHost</li>
	/// <li>http.proxyPort</li>
	/// <li>http.nonProxyHosts</li>
	/// <li>http.keepAlive</li>
	/// <li>http.maxConnections</li>
	/// <li>http.agent</li>
	/// </ul>
	/// <p/>
	/// Please note that some settings used by this class can be mutually
	/// exclusive and may not apply when building
	/// <see cref="CloseableHttpClient">CloseableHttpClient</see>
	/// instances.
	/// </summary>
	/// <since>4.3</since>
	public class HttpClientBuilder
	{
		private HttpRequestExecutor requestExec;

		private X509HostnameVerifier hostnameVerifier;

		private LayeredConnectionSocketFactory sslSocketFactory;

		private SSLContext sslcontext;

		private HttpClientConnectionManager connManager;

		private SchemePortResolver schemePortResolver;

		private ConnectionReuseStrategy reuseStrategy;

		private ConnectionKeepAliveStrategy keepAliveStrategy;

		private AuthenticationStrategy targetAuthStrategy;

		private AuthenticationStrategy proxyAuthStrategy;

		private UserTokenHandler userTokenHandler;

		private HttpProcessor httpprocessor;

		private List<IHttpRequestInterceptor> requestFirst;

		private List<IHttpRequestInterceptor> requestLast;

		private List<HttpResponseInterceptor> responseFirst;

		private List<HttpResponseInterceptor> responseLast;

		private HttpRequestRetryHandler retryHandler;

		private HttpRoutePlanner routePlanner;

		private RedirectStrategy redirectStrategy;

		private ConnectionBackoffStrategy connectionBackoffStrategy;

		private BackoffManager backoffManager;

		private ServiceUnavailableRetryStrategy serviceUnavailStrategy;

		private Lookup<AuthSchemeProvider> authSchemeRegistry;

		private Lookup<CookieSpecProvider> cookieSpecRegistry;

		private CookieStore cookieStore;

		private CredentialsProvider credentialsProvider;

		private string userAgent;

		private HttpHost proxy;

		private ICollection<Header> defaultHeaders;

		private SocketConfig defaultSocketConfig;

		private ConnectionConfig defaultConnectionConfig;

		private RequestConfig defaultRequestConfig;

		private bool systemProperties;

		private bool redirectHandlingDisabled;

		private bool automaticRetriesDisabled;

		private bool contentCompressionDisabled;

		private bool cookieManagementDisabled;

		private bool authCachingDisabled;

		private bool connectionStateDisabled;

		private int maxConnTotal = 0;

		private int maxConnPerRoute = 0;

		private IList<IDisposable> closeables;

		internal static readonly string DefaultUserAgent;

		static HttpClientBuilder()
		{
			VersionInfo vi = VersionInfo.LoadVersionInfo("org.apache.http.client", typeof(Apache.Http.Impl.Client.HttpClientBuilder
				).GetClassLoader());
			string release = (vi != null) ? vi.GetRelease() : VersionInfo.Unavailable;
			DefaultUserAgent = "Apache-HttpClient/" + release + " (java 1.5)";
		}

		public static Apache.Http.Impl.Client.HttpClientBuilder Create()
		{
			return new Apache.Http.Impl.Client.HttpClientBuilder();
		}

		protected internal HttpClientBuilder() : base()
		{
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Protocol.HttpRequestExecutor">Apache.Http.Protocol.HttpRequestExecutor
		/// 	</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetRequestExecutor(HttpRequestExecutor
			 requestExec)
		{
			this.requestExec = requestExec;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Conn.Ssl.X509HostnameVerifier">Apache.Http.Conn.Ssl.X509HostnameVerifier
		/// 	</see>
		/// instance.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)">SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)
		/// 	</see>
		/// and the
		/// <see cref="SetSSLSocketFactory(Apache.Http.Conn.Socket.LayeredConnectionSocketFactory)
		/// 	">SetSSLSocketFactory(Apache.Http.Conn.Socket.LayeredConnectionSocketFactory)</see>
		/// methods.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetHostnameVerifier(X509HostnameVerifier
			 hostnameVerifier)
		{
			this.hostnameVerifier = hostnameVerifier;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Sharpen.SSLContext">Sharpen.SSLContext</see>
		/// instance.
		/// <p/>
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)">SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)
		/// 	</see>
		/// and the
		/// <see cref="SetSSLSocketFactory(Apache.Http.Conn.Socket.LayeredConnectionSocketFactory)
		/// 	">SetSSLSocketFactory(Apache.Http.Conn.Socket.LayeredConnectionSocketFactory)</see>
		/// methods.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetSslcontext(SSLContext sslcontext
			)
		{
			this.sslcontext = sslcontext;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Conn.Socket.LayeredConnectionSocketFactory">Apache.Http.Conn.Socket.LayeredConnectionSocketFactory
		/// 	</see>
		/// instance.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)">SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)
		/// 	</see>
		/// method.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetSSLSocketFactory(LayeredConnectionSocketFactory
			 sslSocketFactory)
		{
			this.sslSocketFactory = sslSocketFactory;
			return this;
		}

		/// <summary>Assigns maximum total connection value.</summary>
		/// <remarks>
		/// Assigns maximum total connection value.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)">SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder SetMaxConnTotal(int maxConnTotal
			)
		{
			this.maxConnTotal = maxConnTotal;
			return this;
		}

		/// <summary>Assigns maximum connection per route value.</summary>
		/// <remarks>
		/// Assigns maximum connection per route value.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)">SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder SetMaxConnPerRoute(int maxConnPerRoute
			)
		{
			this.maxConnPerRoute = maxConnPerRoute;
			return this;
		}

		/// <summary>
		/// Assigns default
		/// <see cref="Apache.Http.Config.SocketConfig">Apache.Http.Config.SocketConfig</see>
		/// .
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)">SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)
		/// 	</see>
		/// method.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetDefaultSocketConfig(SocketConfig
			 config)
		{
			this.defaultSocketConfig = config;
			return this;
		}

		/// <summary>
		/// Assigns default
		/// <see cref="Apache.Http.Config.ConnectionConfig">Apache.Http.Config.ConnectionConfig
		/// 	</see>
		/// .
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)">SetConnectionManager(Apache.Http.Conn.HttpClientConnectionManager)
		/// 	</see>
		/// method.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetDefaultConnectionConfig(ConnectionConfig
			 config)
		{
			this.defaultConnectionConfig = config;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Conn.HttpClientConnectionManager">Apache.Http.Conn.HttpClientConnectionManager
		/// 	</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetConnectionManager(HttpClientConnectionManager
			 connManager)
		{
			this.connManager = connManager;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.ConnectionReuseStrategy">Apache.Http.ConnectionReuseStrategy
		/// 	</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetConnectionReuseStrategy(ConnectionReuseStrategy
			 reuseStrategy)
		{
			this.reuseStrategy = reuseStrategy;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Conn.ConnectionKeepAliveStrategy">Apache.Http.Conn.ConnectionKeepAliveStrategy
		/// 	</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetKeepAliveStrategy(ConnectionKeepAliveStrategy
			 keepAliveStrategy)
		{
			this.keepAliveStrategy = keepAliveStrategy;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Client.AuthenticationStrategy">Apache.Http.Client.AuthenticationStrategy
		/// 	</see>
		/// instance for proxy
		/// authentication.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetTargetAuthenticationStrategy(
			AuthenticationStrategy targetAuthStrategy)
		{
			this.targetAuthStrategy = targetAuthStrategy;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Client.AuthenticationStrategy">Apache.Http.Client.AuthenticationStrategy
		/// 	</see>
		/// instance for target
		/// host authentication.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetProxyAuthenticationStrategy(AuthenticationStrategy
			 proxyAuthStrategy)
		{
			this.proxyAuthStrategy = proxyAuthStrategy;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Client.UserTokenHandler">Apache.Http.Client.UserTokenHandler
		/// 	</see>
		/// instance.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="DisableConnectionState()">DisableConnectionState()</see>
		/// method.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetUserTokenHandler(UserTokenHandler
			 userTokenHandler)
		{
			this.userTokenHandler = userTokenHandler;
			return this;
		}

		/// <summary>Disables connection state tracking.</summary>
		/// <remarks>Disables connection state tracking.</remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder DisableConnectionState()
		{
			connectionStateDisabled = true;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Conn.SchemePortResolver">Apache.Http.Conn.SchemePortResolver
		/// 	</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetSchemePortResolver(SchemePortResolver
			 schemePortResolver)
		{
			this.schemePortResolver = schemePortResolver;
			return this;
		}

		/// <summary>Assigns <tt>User-Agent</tt> value.</summary>
		/// <remarks>
		/// Assigns <tt>User-Agent</tt> value.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)">SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder SetUserAgent(string userAgent)
		{
			this.userAgent = userAgent;
			return this;
		}

		/// <summary>Assigns default request header values.</summary>
		/// <remarks>
		/// Assigns default request header values.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)">SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder SetDefaultHeaders<_T0>(ICollection
			<_T0> defaultHeaders) where _T0:Header
		{
			this.defaultHeaders = defaultHeaders;
			return this;
		}

		/// <summary>Adds this protocol interceptor to the head of the protocol processing list.
		/// 	</summary>
		/// <remarks>
		/// Adds this protocol interceptor to the head of the protocol processing list.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)">SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder AddInterceptorFirst(HttpResponseInterceptor
			 itcp)
		{
			if (itcp == null)
			{
				return this;
			}
			if (responseFirst == null)
			{
				responseFirst = new List<HttpResponseInterceptor>();
			}
			responseFirst.AddFirst(itcp);
			return this;
		}

		/// <summary>Adds this protocol interceptor to the tail of the protocol processing list.
		/// 	</summary>
		/// <remarks>
		/// Adds this protocol interceptor to the tail of the protocol processing list.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)">SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder AddInterceptorLast(HttpResponseInterceptor
			 itcp)
		{
			if (itcp == null)
			{
				return this;
			}
			if (responseLast == null)
			{
				responseLast = new List<HttpResponseInterceptor>();
			}
			responseLast.AddLast(itcp);
			return this;
		}

		/// <summary>Adds this protocol interceptor to the head of the protocol processing list.
		/// 	</summary>
		/// <remarks>
		/// Adds this protocol interceptor to the head of the protocol processing list.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)">SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder AddInterceptorFirst(IHttpRequestInterceptor
			 itcp)
		{
			if (itcp == null)
			{
				return this;
			}
			if (requestFirst == null)
			{
				requestFirst = new List<IHttpRequestInterceptor>();
			}
			requestFirst.AddFirst(itcp);
			return this;
		}

		/// <summary>Adds this protocol interceptor to the tail of the protocol processing list.
		/// 	</summary>
		/// <remarks>
		/// Adds this protocol interceptor to the tail of the protocol processing list.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)">SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder AddInterceptorLast(IHttpRequestInterceptor
			 itcp)
		{
			if (itcp == null)
			{
				return this;
			}
			if (requestLast == null)
			{
				requestLast = new List<IHttpRequestInterceptor>();
			}
			requestLast.AddLast(itcp);
			return this;
		}

		/// <summary>Disables state (cookie) management.</summary>
		/// <remarks>
		/// Disables state (cookie) management.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)">SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder DisableCookieManagement()
		{
			this.cookieManagementDisabled = true;
			return this;
		}

		/// <summary>Disables automatic content decompression.</summary>
		/// <remarks>
		/// Disables automatic content decompression.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)">SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder DisableContentCompression()
		{
			contentCompressionDisabled = true;
			return this;
		}

		/// <summary>Disables authentication scheme caching.</summary>
		/// <remarks>
		/// Disables authentication scheme caching.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)">SetHttpProcessor(Apache.Http.Protocol.HttpProcessor)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder DisableAuthCaching()
		{
			this.authCachingDisabled = true;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Protocol.HttpProcessor">Apache.Http.Protocol.HttpProcessor
		/// 	</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetHttpProcessor(HttpProcessor httpprocessor
			)
		{
			this.httpprocessor = httpprocessor;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Client.HttpRequestRetryHandler">Apache.Http.Client.HttpRequestRetryHandler
		/// 	</see>
		/// instance.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="DisableAutomaticRetries()">DisableAutomaticRetries()</see>
		/// method.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetRetryHandler(HttpRequestRetryHandler
			 retryHandler)
		{
			this.retryHandler = retryHandler;
			return this;
		}

		/// <summary>Disables automatic request recovery and re-execution.</summary>
		/// <remarks>Disables automatic request recovery and re-execution.</remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder DisableAutomaticRetries()
		{
			automaticRetriesDisabled = true;
			return this;
		}

		/// <summary>Assigns default proxy value.</summary>
		/// <remarks>
		/// Assigns default proxy value.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="SetRoutePlanner(Apache.Http.Conn.Routing.HttpRoutePlanner)">SetRoutePlanner(Apache.Http.Conn.Routing.HttpRoutePlanner)
		/// 	</see>
		/// method.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder SetProxy(HttpHost proxy)
		{
			this.proxy = proxy;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Conn.Routing.HttpRoutePlanner">Apache.Http.Conn.Routing.HttpRoutePlanner
		/// 	</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetRoutePlanner(HttpRoutePlanner
			 routePlanner)
		{
			this.routePlanner = routePlanner;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Client.RedirectStrategy">Apache.Http.Client.RedirectStrategy
		/// 	</see>
		/// instance.
		/// <p/>
		/// Please note this value can be overridden by the
		/// <see cref="DisableRedirectHandling()">DisableRedirectHandling()</see>
		/// method.
		/// `
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetRedirectStrategy(RedirectStrategy
			 redirectStrategy)
		{
			this.redirectStrategy = redirectStrategy;
			return this;
		}

		/// <summary>Disables automatic redirect handling.</summary>
		/// <remarks>Disables automatic redirect handling.</remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder DisableRedirectHandling()
		{
			redirectHandlingDisabled = true;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Client.ConnectionBackoffStrategy">Apache.Http.Client.ConnectionBackoffStrategy
		/// 	</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetConnectionBackoffStrategy(ConnectionBackoffStrategy
			 connectionBackoffStrategy)
		{
			this.connectionBackoffStrategy = connectionBackoffStrategy;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Client.BackoffManager">Apache.Http.Client.BackoffManager</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetBackoffManager(BackoffManager
			 backoffManager)
		{
			this.backoffManager = backoffManager;
			return this;
		}

		/// <summary>
		/// Assigns
		/// <see cref="Apache.Http.Client.ServiceUnavailableRetryStrategy">Apache.Http.Client.ServiceUnavailableRetryStrategy
		/// 	</see>
		/// instance.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetServiceUnavailableRetryStrategy
			(ServiceUnavailableRetryStrategy serviceUnavailStrategy)
		{
			this.serviceUnavailStrategy = serviceUnavailStrategy;
			return this;
		}

		/// <summary>
		/// Assigns default
		/// <see cref="Apache.Http.Client.CookieStore">Apache.Http.Client.CookieStore</see>
		/// instance which will be used for
		/// request execution if not explicitly set in the client execution context.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetDefaultCookieStore(CookieStore
			 cookieStore)
		{
			this.cookieStore = cookieStore;
			return this;
		}

		/// <summary>
		/// Assigns default
		/// <see cref="Apache.Http.Client.CredentialsProvider">Apache.Http.Client.CredentialsProvider
		/// 	</see>
		/// instance which will be used
		/// for request execution if not explicitly set in the client execution
		/// context.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetDefaultCredentialsProvider(CredentialsProvider
			 credentialsProvider)
		{
			this.credentialsProvider = credentialsProvider;
			return this;
		}

		/// <summary>
		/// Assigns default
		/// <see cref="Apache.Http.Auth.AuthScheme">Apache.Http.Auth.AuthScheme</see>
		/// registry which will
		/// be used for request execution if not explicitly set in the client execution
		/// context.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetDefaultAuthSchemeRegistry(Lookup
			<AuthSchemeProvider> authSchemeRegistry)
		{
			this.authSchemeRegistry = authSchemeRegistry;
			return this;
		}

		/// <summary>
		/// Assigns default
		/// <see cref="Apache.Http.Cookie.CookieSpec">Apache.Http.Cookie.CookieSpec</see>
		/// registry which will
		/// be used for request execution if not explicitly set in the client execution
		/// context.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetDefaultCookieSpecRegistry(Lookup
			<CookieSpecProvider> cookieSpecRegistry)
		{
			this.cookieSpecRegistry = cookieSpecRegistry;
			return this;
		}

		/// <summary>
		/// Assigns default
		/// <see cref="Apache.Http.Client.Config.RequestConfig">Apache.Http.Client.Config.RequestConfig
		/// 	</see>
		/// instance which will be used
		/// for request execution if not explicitly set in the client execution
		/// context.
		/// </summary>
		public Apache.Http.Impl.Client.HttpClientBuilder SetDefaultRequestConfig(RequestConfig
			 config)
		{
			this.defaultRequestConfig = config;
			return this;
		}

		/// <summary>
		/// Use system properties when creating and configuring default
		/// implementations.
		/// </summary>
		/// <remarks>
		/// Use system properties when creating and configuring default
		/// implementations.
		/// </remarks>
		public Apache.Http.Impl.Client.HttpClientBuilder UseSystemProperties()
		{
			systemProperties = true;
			return this;
		}

		/// <summary>For internal use.</summary>
		/// <remarks>For internal use.</remarks>
		protected internal virtual ClientExecChain DecorateMainExec(ClientExecChain mainExec
			)
		{
			return mainExec;
		}

		/// <summary>For internal use.</summary>
		/// <remarks>For internal use.</remarks>
		protected internal virtual ClientExecChain DecorateProtocolExec(ClientExecChain protocolExec
			)
		{
			return protocolExec;
		}

		/// <summary>For internal use.</summary>
		/// <remarks>For internal use.</remarks>
		protected internal virtual void AddCloseable(IDisposable closeable)
		{
			if (closeable == null)
			{
				return;
			}
			if (closeables == null)
			{
				closeables = new AList<IDisposable>();
			}
			closeables.AddItem(closeable);
		}

		private static string[] Split(string s)
		{
			if (TextUtils.IsBlank(s))
			{
				return null;
			}
			return s.Split(" *, *");
		}

		public virtual CloseableHttpClient Build()
		{
			// Create main request executor
			HttpRequestExecutor requestExec = this.requestExec;
			if (requestExec == null)
			{
				requestExec = new HttpRequestExecutor();
			}
			HttpClientConnectionManager connManager = this.connManager;
			if (connManager == null)
			{
				LayeredConnectionSocketFactory sslSocketFactory = this.sslSocketFactory;
				if (sslSocketFactory == null)
				{
					string[] supportedProtocols = systemProperties ? Split(Runtime.GetProperty("https.protocols"
						)) : null;
					string[] supportedCipherSuites = systemProperties ? Split(Runtime.GetProperty("https.cipherSuites"
						)) : null;
					X509HostnameVerifier hostnameVerifier = this.hostnameVerifier;
					if (hostnameVerifier == null)
					{
						hostnameVerifier = SSLConnectionSocketFactory.BrowserCompatibleHostnameVerifier;
					}
					if (sslcontext != null)
					{
						sslSocketFactory = new SSLConnectionSocketFactory(sslcontext, supportedProtocols, 
							supportedCipherSuites, hostnameVerifier);
					}
					else
					{
						if (systemProperties)
						{
							sslSocketFactory = new SSLConnectionSocketFactory((SSLSocketFactory)SSLSocketFactory
								.GetDefault(), supportedProtocols, supportedCipherSuites, hostnameVerifier);
						}
						else
						{
							sslSocketFactory = new SSLConnectionSocketFactory(SSLContexts.CreateDefault(), hostnameVerifier
								);
						}
					}
				}
				PoolingHttpClientConnectionManager poolingmgr = new PoolingHttpClientConnectionManager
					(RegistryBuilder.Create<ConnectionSocketFactory>().Register("http", PlainConnectionSocketFactory
					.GetSocketFactory()).Register("https", sslSocketFactory).Build());
				if (defaultSocketConfig != null)
				{
					poolingmgr.SetDefaultSocketConfig(defaultSocketConfig);
				}
				if (defaultConnectionConfig != null)
				{
					poolingmgr.SetDefaultConnectionConfig(defaultConnectionConfig);
				}
				if (systemProperties)
				{
					string s = Runtime.GetProperty("http.keepAlive", "true");
					if (Sharpen.Runtime.EqualsIgnoreCase("true", s))
					{
						s = Runtime.GetProperty("http.maxConnections", "5");
						int max = System.Convert.ToInt32(s);
						poolingmgr.SetDefaultMaxPerRoute(max);
						poolingmgr.SetMaxTotal(2 * max);
					}
				}
				if (maxConnTotal > 0)
				{
					poolingmgr.SetMaxTotal(maxConnTotal);
				}
				if (maxConnPerRoute > 0)
				{
					poolingmgr.SetDefaultMaxPerRoute(maxConnPerRoute);
				}
				connManager = poolingmgr;
			}
			ConnectionReuseStrategy reuseStrategy = this.reuseStrategy;
			if (reuseStrategy == null)
			{
				if (systemProperties)
				{
					string s = Runtime.GetProperty("http.keepAlive", "true");
					if (Sharpen.Runtime.EqualsIgnoreCase("true", s))
					{
						reuseStrategy = DefaultConnectionReuseStrategy.Instance;
					}
					else
					{
						reuseStrategy = NoConnectionReuseStrategy.Instance;
					}
				}
				else
				{
					reuseStrategy = DefaultConnectionReuseStrategy.Instance;
				}
			}
			ConnectionKeepAliveStrategy keepAliveStrategy = this.keepAliveStrategy;
			if (keepAliveStrategy == null)
			{
				keepAliveStrategy = DefaultConnectionKeepAliveStrategy.Instance;
			}
			AuthenticationStrategy targetAuthStrategy = this.targetAuthStrategy;
			if (targetAuthStrategy == null)
			{
				targetAuthStrategy = TargetAuthenticationStrategy.Instance;
			}
			AuthenticationStrategy proxyAuthStrategy = this.proxyAuthStrategy;
			if (proxyAuthStrategy == null)
			{
				proxyAuthStrategy = ProxyAuthenticationStrategy.Instance;
			}
			UserTokenHandler userTokenHandler = this.userTokenHandler;
			if (userTokenHandler == null)
			{
				if (!connectionStateDisabled)
				{
					userTokenHandler = DefaultUserTokenHandler.Instance;
				}
				else
				{
					userTokenHandler = NoopUserTokenHandler.Instance;
				}
			}
			ClientExecChain execChain = new MainClientExec(requestExec, connManager, reuseStrategy
				, keepAliveStrategy, targetAuthStrategy, proxyAuthStrategy, userTokenHandler);
			execChain = DecorateMainExec(execChain);
			HttpProcessor httpprocessor = this.httpprocessor;
			if (httpprocessor == null)
			{
				string userAgent = this.userAgent;
				if (userAgent == null)
				{
					if (systemProperties)
					{
						userAgent = Runtime.GetProperty("http.agent");
					}
					if (userAgent == null)
					{
						userAgent = DefaultUserAgent;
					}
				}
				HttpProcessorBuilder b = HttpProcessorBuilder.Create();
				if (requestFirst != null)
				{
					foreach (IHttpRequestInterceptor i in requestFirst)
					{
						b.AddFirst(i);
					}
				}
				if (responseFirst != null)
				{
					foreach (HttpResponseInterceptor i in responseFirst)
					{
						b.AddFirst(i);
					}
				}
				b.AddAll(new RequestDefaultHeaders(defaultHeaders), new RequestContent(), new RequestTargetHost
					(), new RequestClientConnControl(), new RequestUserAgent(userAgent), new RequestExpectContinue
					());
				if (!cookieManagementDisabled)
				{
					b.Add(new RequestAddCookies());
				}
				if (!contentCompressionDisabled)
				{
					b.Add(new RequestAcceptEncoding());
				}
				if (!authCachingDisabled)
				{
					b.Add(new RequestAuthCache());
				}
				if (!cookieManagementDisabled)
				{
					b.Add(new ResponseProcessCookies());
				}
				if (!contentCompressionDisabled)
				{
					b.Add(new ResponseContentEncoding());
				}
				if (requestLast != null)
				{
					foreach (IHttpRequestInterceptor i in requestLast)
					{
						b.AddLast(i);
					}
				}
				if (responseLast != null)
				{
					foreach (HttpResponseInterceptor i in responseLast)
					{
						b.AddLast(i);
					}
				}
				httpprocessor = b.Build();
			}
			execChain = new ProtocolExec(execChain, httpprocessor);
			execChain = DecorateProtocolExec(execChain);
			// Add request retry executor, if not disabled
			if (!automaticRetriesDisabled)
			{
				HttpRequestRetryHandler retryHandler = this.retryHandler;
				if (retryHandler == null)
				{
					retryHandler = DefaultHttpRequestRetryHandler.Instance;
				}
				execChain = new RetryExec(execChain, retryHandler);
			}
			HttpRoutePlanner routePlanner = this.routePlanner;
			if (routePlanner == null)
			{
				SchemePortResolver schemePortResolver = this.schemePortResolver;
				if (schemePortResolver == null)
				{
					schemePortResolver = DefaultSchemePortResolver.Instance;
				}
				if (proxy != null)
				{
					routePlanner = new DefaultProxyRoutePlanner(proxy, schemePortResolver);
				}
				else
				{
					if (systemProperties)
					{
						routePlanner = new SystemDefaultRoutePlanner(schemePortResolver, ProxySelector.GetDefault
							());
					}
					else
					{
						routePlanner = new DefaultRoutePlanner(schemePortResolver);
					}
				}
			}
			// Add redirect executor, if not disabled
			if (!redirectHandlingDisabled)
			{
				RedirectStrategy redirectStrategy = this.redirectStrategy;
				if (redirectStrategy == null)
				{
					redirectStrategy = DefaultRedirectStrategy.Instance;
				}
				execChain = new RedirectExec(execChain, routePlanner, redirectStrategy);
			}
			// Optionally, add service unavailable retry executor
			ServiceUnavailableRetryStrategy serviceUnavailStrategy = this.serviceUnavailStrategy;
			if (serviceUnavailStrategy != null)
			{
				execChain = new ServiceUnavailableRetryExec(execChain, serviceUnavailStrategy);
			}
			// Optionally, add connection back-off executor
			BackoffManager backoffManager = this.backoffManager;
			ConnectionBackoffStrategy connectionBackoffStrategy = this.connectionBackoffStrategy;
			if (backoffManager != null && connectionBackoffStrategy != null)
			{
				execChain = new BackoffStrategyExec(execChain, connectionBackoffStrategy, backoffManager
					);
			}
			Lookup<AuthSchemeProvider> authSchemeRegistry = this.authSchemeRegistry;
			if (authSchemeRegistry == null)
			{
				authSchemeRegistry = RegistryBuilder.Create<AuthSchemeProvider>().Register(AuthSchemes
					.Basic, new BasicSchemeFactory()).Register(AuthSchemes.Digest, new DigestSchemeFactory
					()).Register(AuthSchemes.Ntlm, new NTLMSchemeFactory()).Register(AuthSchemes.Spnego
					, new SPNegoSchemeFactory()).Register(AuthSchemes.Kerberos, new KerberosSchemeFactory
					()).Build();
			}
			Lookup<CookieSpecProvider> cookieSpecRegistry = this.cookieSpecRegistry;
			if (cookieSpecRegistry == null)
			{
				cookieSpecRegistry = RegistryBuilder.Create<CookieSpecProvider>().Register(CookieSpecs
					.BestMatch, new BestMatchSpecFactory()).Register(CookieSpecs.Standard, new RFC2965SpecFactory
					()).Register(CookieSpecs.BrowserCompatibility, new BrowserCompatSpecFactory()).Register
					(CookieSpecs.Netscape, new NetscapeDraftSpecFactory()).Register(CookieSpecs.IgnoreCookies
					, new IgnoreSpecFactory()).Register("rfc2109", new RFC2109SpecFactory()).Register
					("rfc2965", new RFC2965SpecFactory()).Build();
			}
			CookieStore defaultCookieStore = this.cookieStore;
			if (defaultCookieStore == null)
			{
				defaultCookieStore = new BasicCookieStore();
			}
			CredentialsProvider defaultCredentialsProvider = this.credentialsProvider;
			if (defaultCredentialsProvider == null)
			{
				if (systemProperties)
				{
					defaultCredentialsProvider = new SystemDefaultCredentialsProvider();
				}
				else
				{
					defaultCredentialsProvider = new BasicCredentialsProvider();
				}
			}
			return new InternalHttpClient(execChain, connManager, routePlanner, cookieSpecRegistry
				, authSchemeRegistry, defaultCookieStore, defaultCredentialsProvider, defaultRequestConfig
				 != null ? defaultRequestConfig : RequestConfig.Default, closeables != null ? new 
				AList<IDisposable>(closeables) : null);
		}
	}
}
