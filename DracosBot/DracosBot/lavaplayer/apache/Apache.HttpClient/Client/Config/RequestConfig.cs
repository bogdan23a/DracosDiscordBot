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
using System.Net;
using System.Text;
using Apache.Http;
using Apache.Http.Client.Config;
using Sharpen;

namespace Apache.Http.Client.Config
{
	public class RequestConfig : ICloneable
	{
		public static readonly Apache.Http.Client.Config.RequestConfig Default = new RequestConfig.Builder
			().Build();

		private readonly bool expectContinueEnabled;

		private readonly HttpHost proxy;

		private readonly IPAddress localAddress;

		private readonly bool staleConnectionCheckEnabled;

		private readonly string cookieSpec;

		private readonly bool redirectsEnabled;

		private readonly bool relativeRedirectsAllowed;

		private readonly bool circularRedirectsAllowed;

		private readonly int maxRedirects;

		private readonly bool authenticationEnabled;

		private readonly ICollection<string> targetPreferredAuthSchemes;

		private readonly ICollection<string> proxyPreferredAuthSchemes;

		private readonly int connectionRequestTimeout;

		private readonly int connectTimeout;

		private readonly int socketTimeout;

		internal RequestConfig(bool expectContinueEnabled, HttpHost proxy, IPAddress localAddress
			, bool staleConnectionCheckEnabled, string cookieSpec, bool redirectsEnabled, bool
			 relativeRedirectsAllowed, bool circularRedirectsAllowed, int maxRedirects, bool
			 authenticationEnabled, ICollection<string> targetPreferredAuthSchemes, ICollection
			<string> proxyPreferredAuthSchemes, int connectionRequestTimeout, int connectTimeout
			, int socketTimeout) : base()
		{
			this.expectContinueEnabled = expectContinueEnabled;
			this.proxy = proxy;
			this.localAddress = localAddress;
			this.staleConnectionCheckEnabled = staleConnectionCheckEnabled;
			this.cookieSpec = cookieSpec;
			this.redirectsEnabled = redirectsEnabled;
			this.relativeRedirectsAllowed = relativeRedirectsAllowed;
			this.circularRedirectsAllowed = circularRedirectsAllowed;
			this.maxRedirects = maxRedirects;
			this.authenticationEnabled = authenticationEnabled;
			this.targetPreferredAuthSchemes = targetPreferredAuthSchemes;
			this.proxyPreferredAuthSchemes = proxyPreferredAuthSchemes;
			this.connectionRequestTimeout = connectionRequestTimeout;
			this.connectTimeout = connectTimeout;
			this.socketTimeout = socketTimeout;
		}

		/// <summary>
		/// Determines whether the 'Expect: 100-Continue' handshake is enabled
		/// for entity enclosing methods.
		/// </summary>
		/// <remarks>
		/// Determines whether the 'Expect: 100-Continue' handshake is enabled
		/// for entity enclosing methods. The purpose of the 'Expect: 100-Continue'
		/// handshake is to allow a client that is sending a request message with
		/// a request body to determine if the origin server is willing to
		/// accept the request (based on the request headers) before the client
		/// sends the request body.
		/// <p/>
		/// The use of the 'Expect: 100-continue' handshake can result in
		/// a noticeable performance improvement for entity enclosing requests
		/// (such as POST and PUT) that require the target server's
		/// authentication.
		/// <p/>
		/// 'Expect: 100-continue' handshake should be used with caution, as it
		/// may cause problems with HTTP servers and proxies that do not support
		/// HTTP/1.1 protocol.
		/// <p/>
		/// Default: <code>false</code>
		/// </remarks>
		public virtual bool IsExpectContinueEnabled()
		{
			return expectContinueEnabled;
		}

		/// <summary>Returns HTTP proxy to be used for request execution.</summary>
		/// <remarks>
		/// Returns HTTP proxy to be used for request execution.
		/// <p/>
		/// Default: <code>null</code>
		/// </remarks>
		public virtual HttpHost GetProxy()
		{
			return proxy;
		}

		/// <summary>Returns local address to be used for request execution.</summary>
		/// <remarks>
		/// Returns local address to be used for request execution.
		/// <p/>
		/// On machines with multiple network interfaces, this parameter
		/// can be used to select the network interface from which the
		/// connection originates.
		/// <p/>
		/// Default: <code>null</code>
		/// </remarks>
		public virtual IPAddress GetLocalAddress()
		{
			return localAddress;
		}

		/// <summary>Determines whether stale connection check is to be used.</summary>
		/// <remarks>
		/// Determines whether stale connection check is to be used. The stale
		/// connection check can cause up to 30 millisecond overhead per request and
		/// should be used only when appropriate. For performance critical
		/// operations this check should be disabled.
		/// <p/>
		/// Default: <code>true</code>
		/// </remarks>
		public virtual bool IsStaleConnectionCheckEnabled()
		{
			return staleConnectionCheckEnabled;
		}

		/// <summary>
		/// Determines the name of the cookie specification to be used for HTTP state
		/// management.
		/// </summary>
		/// <remarks>
		/// Determines the name of the cookie specification to be used for HTTP state
		/// management.
		/// <p/>
		/// Default: <code>null</code>
		/// </remarks>
		public virtual string GetCookieSpec()
		{
			return cookieSpec;
		}

		/// <summary>Determines whether redirects should be handled automatically.</summary>
		/// <remarks>
		/// Determines whether redirects should be handled automatically.
		/// <p/>
		/// Default: <code>true</code>
		/// </remarks>
		public virtual bool IsRedirectsEnabled()
		{
			return redirectsEnabled;
		}

		/// <summary>Determines whether relative redirects should be rejected.</summary>
		/// <remarks>
		/// Determines whether relative redirects should be rejected. HTTP specification
		/// requires the location value be an absolute URI.
		/// <p/>
		/// Default: <code>true</code>
		/// </remarks>
		public virtual bool IsRelativeRedirectsAllowed()
		{
			return relativeRedirectsAllowed;
		}

		/// <summary>
		/// Determines whether circular redirects (redirects to the same location) should
		/// be allowed.
		/// </summary>
		/// <remarks>
		/// Determines whether circular redirects (redirects to the same location) should
		/// be allowed. The HTTP spec is not sufficiently clear whether circular redirects
		/// are permitted, therefore optionally they can be enabled
		/// <p/>
		/// Default: <code>false</code>
		/// </remarks>
		public virtual bool IsCircularRedirectsAllowed()
		{
			return circularRedirectsAllowed;
		}

		/// <summary>Returns the maximum number of redirects to be followed.</summary>
		/// <remarks>
		/// Returns the maximum number of redirects to be followed. The limit on number
		/// of redirects is intended to prevent infinite loops.
		/// <p/>
		/// Default: <code>50</code>
		/// </remarks>
		public virtual int GetMaxRedirects()
		{
			return maxRedirects;
		}

		/// <summary>Determines whether authentication should be handled automatically.</summary>
		/// <remarks>
		/// Determines whether authentication should be handled automatically.
		/// <p/>
		/// Default: <code>true</code>
		/// </remarks>
		public virtual bool IsAuthenticationEnabled()
		{
			return authenticationEnabled;
		}

		/// <summary>
		/// Determines the order of preference for supported authentication schemes
		/// when authenticating with the target host.
		/// </summary>
		/// <remarks>
		/// Determines the order of preference for supported authentication schemes
		/// when authenticating with the target host.
		/// <p/>
		/// Default: <code>null</code>
		/// </remarks>
		public virtual ICollection<string> GetTargetPreferredAuthSchemes()
		{
			return targetPreferredAuthSchemes;
		}

		/// <summary>
		/// Determines the order of preference for supported authentication schemes
		/// when authenticating with the proxy host.
		/// </summary>
		/// <remarks>
		/// Determines the order of preference for supported authentication schemes
		/// when authenticating with the proxy host.
		/// <p/>
		/// Default: <code>null</code>
		/// </remarks>
		public virtual ICollection<string> GetProxyPreferredAuthSchemes()
		{
			return proxyPreferredAuthSchemes;
		}

		/// <summary>
		/// Returns the timeout in milliseconds used when requesting a connection
		/// from the connection manager.
		/// </summary>
		/// <remarks>
		/// Returns the timeout in milliseconds used when requesting a connection
		/// from the connection manager. A timeout value of zero is interpreted
		/// as an infinite timeout.
		/// <p/>
		/// A timeout value of zero is interpreted as an infinite timeout.
		/// A negative value is interpreted as undefined (system default).
		/// <p/>
		/// Default: <code>-1</code>
		/// </remarks>
		public virtual int GetConnectionRequestTimeout()
		{
			return connectionRequestTimeout;
		}

		/// <summary>Determines the timeout in milliseconds until a connection is established.
		/// 	</summary>
		/// <remarks>
		/// Determines the timeout in milliseconds until a connection is established.
		/// A timeout value of zero is interpreted as an infinite timeout.
		/// <p/>
		/// A timeout value of zero is interpreted as an infinite timeout.
		/// A negative value is interpreted as undefined (system default).
		/// <p/>
		/// Default: <code>-1</code>
		/// </remarks>
		public virtual int GetConnectTimeout()
		{
			return connectTimeout;
		}

		/// <summary>
		/// Defines the socket timeout (<code>SO_TIMEOUT</code>) in milliseconds,
		/// which is the timeout for waiting for data  or, put differently,
		/// a maximum period inactivity between two consecutive data packets).
		/// </summary>
		/// <remarks>
		/// Defines the socket timeout (<code>SO_TIMEOUT</code>) in milliseconds,
		/// which is the timeout for waiting for data  or, put differently,
		/// a maximum period inactivity between two consecutive data packets).
		/// <p/>
		/// A timeout value of zero is interpreted as an infinite timeout.
		/// A negative value is interpreted as undefined (system default).
		/// <p/>
		/// Default: <code>-1</code>
		/// </remarks>
		public virtual int GetSocketTimeout()
		{
			return socketTimeout;
		}

		/// <exception cref="Sharpen.CloneNotSupportedException"></exception>
		protected internal virtual Apache.Http.Client.Config.RequestConfig Clone()
		{
			return (Apache.Http.Client.Config.RequestConfig)base.Clone();
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(", expectContinueEnabled=").Append(expectContinueEnabled);
			builder.Append(", proxy=").Append(proxy);
			builder.Append(", localAddress=").Append(localAddress);
			builder.Append(", staleConnectionCheckEnabled=").Append(staleConnectionCheckEnabled
				);
			builder.Append(", cookieSpec=").Append(cookieSpec);
			builder.Append(", redirectsEnabled=").Append(redirectsEnabled);
			builder.Append(", relativeRedirectsAllowed=").Append(relativeRedirectsAllowed);
			builder.Append(", maxRedirects=").Append(maxRedirects);
			builder.Append(", circularRedirectsAllowed=").Append(circularRedirectsAllowed);
			builder.Append(", authenticationEnabled=").Append(authenticationEnabled);
			builder.Append(", targetPreferredAuthSchemes=").Append(targetPreferredAuthSchemes
				);
			builder.Append(", proxyPreferredAuthSchemes=").Append(proxyPreferredAuthSchemes);
			builder.Append(", connectionRequestTimeout=").Append(connectionRequestTimeout);
			builder.Append(", connectTimeout=").Append(connectTimeout);
			builder.Append(", socketTimeout=").Append(socketTimeout);
			builder.Append("]");
			return builder.ToString();
		}

		public static RequestConfig.Builder Custom()
		{
			return new RequestConfig.Builder();
		}

		public static RequestConfig.Builder Copy(Apache.Http.Client.Config.RequestConfig 
			config)
		{
			return new RequestConfig.Builder().SetExpectContinueEnabled(config.IsExpectContinueEnabled
				()).SetProxy(config.GetProxy()).SetLocalAddress(config.GetLocalAddress()).SetStaleConnectionCheckEnabled
				(config.IsStaleConnectionCheckEnabled()).SetCookieSpec(config.GetCookieSpec()).SetRedirectsEnabled
				(config.IsRedirectsEnabled()).SetRelativeRedirectsAllowed(config.IsRelativeRedirectsAllowed
				()).SetCircularRedirectsAllowed(config.IsCircularRedirectsAllowed()).SetMaxRedirects
				(config.GetMaxRedirects()).SetAuthenticationEnabled(config.IsAuthenticationEnabled
				()).SetTargetPreferredAuthSchemes(config.GetTargetPreferredAuthSchemes()).SetProxyPreferredAuthSchemes
				(config.GetProxyPreferredAuthSchemes()).SetConnectionRequestTimeout(config.GetConnectionRequestTimeout
				()).SetConnectTimeout(config.GetConnectTimeout()).SetSocketTimeout(config.GetSocketTimeout
				());
		}

		public class Builder
		{
			private bool expectContinueEnabled;

			private HttpHost proxy;

			private IPAddress localAddress;

			private bool staleConnectionCheckEnabled;

			private string cookieSpec;

			private bool redirectsEnabled;

			private bool relativeRedirectsAllowed;

			private bool circularRedirectsAllowed;

			private int maxRedirects;

			private bool authenticationEnabled;

			private ICollection<string> targetPreferredAuthSchemes;

			private ICollection<string> proxyPreferredAuthSchemes;

			private int connectionRequestTimeout;

			private int connectTimeout;

			private int socketTimeout;

			internal Builder() : base()
			{
				this.staleConnectionCheckEnabled = true;
				this.redirectsEnabled = true;
				this.maxRedirects = 50;
				this.relativeRedirectsAllowed = true;
				this.authenticationEnabled = true;
				this.connectionRequestTimeout = -1;
				this.connectTimeout = -1;
				this.socketTimeout = -1;
			}

			public virtual RequestConfig.Builder SetExpectContinueEnabled(bool expectContinueEnabled
				)
			{
				this.expectContinueEnabled = expectContinueEnabled;
				return this;
			}

			public virtual RequestConfig.Builder SetProxy(HttpHost proxy)
			{
				this.proxy = proxy;
				return this;
			}

			public virtual RequestConfig.Builder SetLocalAddress(IPAddress localAddress)
			{
				this.localAddress = localAddress;
				return this;
			}

			public virtual RequestConfig.Builder SetStaleConnectionCheckEnabled(bool staleConnectionCheckEnabled
				)
			{
				this.staleConnectionCheckEnabled = staleConnectionCheckEnabled;
				return this;
			}

			public virtual RequestConfig.Builder SetCookieSpec(string cookieSpec)
			{
				this.cookieSpec = cookieSpec;
				return this;
			}

			public virtual RequestConfig.Builder SetRedirectsEnabled(bool redirectsEnabled)
			{
				this.redirectsEnabled = redirectsEnabled;
				return this;
			}

			public virtual RequestConfig.Builder SetRelativeRedirectsAllowed(bool relativeRedirectsAllowed
				)
			{
				this.relativeRedirectsAllowed = relativeRedirectsAllowed;
				return this;
			}

			public virtual RequestConfig.Builder SetCircularRedirectsAllowed(bool circularRedirectsAllowed
				)
			{
				this.circularRedirectsAllowed = circularRedirectsAllowed;
				return this;
			}

			public virtual RequestConfig.Builder SetMaxRedirects(int maxRedirects)
			{
				this.maxRedirects = maxRedirects;
				return this;
			}

			public virtual RequestConfig.Builder SetAuthenticationEnabled(bool authenticationEnabled
				)
			{
				this.authenticationEnabled = authenticationEnabled;
				return this;
			}

			public virtual RequestConfig.Builder SetTargetPreferredAuthSchemes(ICollection<string
				> targetPreferredAuthSchemes)
			{
				this.targetPreferredAuthSchemes = targetPreferredAuthSchemes;
				return this;
			}

			public virtual RequestConfig.Builder SetProxyPreferredAuthSchemes(ICollection<string
				> proxyPreferredAuthSchemes)
			{
				this.proxyPreferredAuthSchemes = proxyPreferredAuthSchemes;
				return this;
			}

			public virtual RequestConfig.Builder SetConnectionRequestTimeout(int connectionRequestTimeout
				)
			{
				this.connectionRequestTimeout = connectionRequestTimeout;
				return this;
			}

			public virtual RequestConfig.Builder SetConnectTimeout(int connectTimeout)
			{
				this.connectTimeout = connectTimeout;
				return this;
			}

			public virtual RequestConfig.Builder SetSocketTimeout(int socketTimeout)
			{
				this.socketTimeout = socketTimeout;
				return this;
			}

			public virtual RequestConfig Build()
			{
				return new RequestConfig(expectContinueEnabled, proxy, localAddress, staleConnectionCheckEnabled
					, cookieSpec, redirectsEnabled, relativeRedirectsAllowed, circularRedirectsAllowed
					, maxRedirects, authenticationEnabled, targetPreferredAuthSchemes, proxyPreferredAuthSchemes
					, connectionRequestTimeout, connectTimeout, socketTimeout);
			}
		}
	}
}
