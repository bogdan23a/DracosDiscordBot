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
using System.Globalization;
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Client.Protocol;
using Apache.Http.Config;
using Apache.Http.Impl.Client;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	internal abstract class AuthenticationStrategyImpl : AuthenticationStrategy
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private static readonly IList<string> DefaultSchemePriority = Sharpen.Collections
			.UnmodifiableList(Arrays.AsList(AuthSchemes.Spnego, AuthSchemes.Kerberos, AuthSchemes
			.Ntlm, AuthSchemes.Digest, AuthSchemes.Basic));

		private readonly int challengeCode;

		private readonly string headerName;

		internal AuthenticationStrategyImpl(int challengeCode, string headerName) : base(
			)
		{
			this.challengeCode = challengeCode;
			this.headerName = headerName;
		}

		public virtual bool IsAuthenticationRequested(HttpHost authhost, HttpResponse response
			, HttpContext context)
		{
			Args.NotNull(response, "HTTP response");
			int status = response.GetStatusLine().GetStatusCode();
			return status == this.challengeCode;
		}

		/// <exception cref="Apache.Http.Auth.MalformedChallengeException"></exception>
		public virtual IDictionary<string, Header> GetChallenges(HttpHost authhost, HttpResponse
			 response, HttpContext context)
		{
			Args.NotNull(response, "HTTP response");
			Header[] headers = response.GetHeaders(this.headerName);
			IDictionary<string, Header> map = new Dictionary<string, Header>(headers.Length);
			foreach (Header header in headers)
			{
				CharArrayBuffer buffer;
				int pos;
				if (header is FormattedHeader)
				{
					buffer = ((FormattedHeader)header).GetBuffer();
					pos = ((FormattedHeader)header).GetValuePos();
				}
				else
				{
					string s = header.GetValue();
					if (s == null)
					{
						throw new MalformedChallengeException("Header value is null");
					}
					buffer = new CharArrayBuffer(s.Length);
					buffer.Append(s);
					pos = 0;
				}
				while (pos < buffer.Length() && HTTP.IsWhitespace(buffer.CharAt(pos)))
				{
					pos++;
				}
				int beginIndex = pos;
				while (pos < buffer.Length() && !HTTP.IsWhitespace(buffer.CharAt(pos)))
				{
					pos++;
				}
				int endIndex = pos;
				string s_1 = buffer.Substring(beginIndex, endIndex);
				map.Put(s_1.ToLower(CultureInfo.InvariantCulture), header);
			}
			return map;
		}

		internal abstract ICollection<string> GetPreferredAuthSchemes(RequestConfig config
			);

		/// <exception cref="Apache.Http.Auth.MalformedChallengeException"></exception>
		public virtual Queue<AuthOption> Select(IDictionary<string, Header> challenges, HttpHost
			 authhost, HttpResponse response, HttpContext context)
		{
			Args.NotNull(challenges, "Map of auth challenges");
			Args.NotNull(authhost, "Host");
			Args.NotNull(response, "HTTP response");
			Args.NotNull(context, "HTTP context");
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			Queue<AuthOption> options = new List<AuthOption>();
			Lookup<AuthSchemeProvider> registry = clientContext.GetAuthSchemeRegistry();
			if (registry == null)
			{
				this.log.Debug("Auth scheme registry not set in the context");
				return options;
			}
			CredentialsProvider credsProvider = clientContext.GetCredentialsProvider();
			if (credsProvider == null)
			{
				this.log.Debug("Credentials provider not set in the context");
				return options;
			}
			RequestConfig config = clientContext.GetRequestConfig();
			ICollection<string> authPrefs = GetPreferredAuthSchemes(config);
			if (authPrefs == null)
			{
				authPrefs = DefaultSchemePriority;
			}
			if (this.log.IsDebugEnabled())
			{
				this.log.Debug("Authentication schemes in the order of preference: " + authPrefs);
			}
			foreach (string id in authPrefs)
			{
				Header challenge = challenges.Get(id.ToLower(CultureInfo.InvariantCulture));
				if (challenge != null)
				{
					AuthSchemeProvider authSchemeProvider = registry.Lookup(id);
					if (authSchemeProvider == null)
					{
						if (this.log.IsWarnEnabled())
						{
							this.log.Warn("Authentication scheme " + id + " not supported");
						}
						// Try again
						continue;
					}
					AuthScheme authScheme = authSchemeProvider.Create(context);
					authScheme.ProcessChallenge(challenge);
					AuthScope authScope = new AuthScope(authhost.GetHostName(), authhost.GetPort(), authScheme
						.GetRealm(), authScheme.GetSchemeName());
					Credentials credentials = credsProvider.GetCredentials(authScope);
					if (credentials != null)
					{
						options.AddItem(new AuthOption(authScheme, credentials));
					}
				}
				else
				{
					if (this.log.IsDebugEnabled())
					{
						this.log.Debug("Challenge for " + id + " authentication scheme not available");
					}
				}
			}
			// Try again
			return options;
		}

		public virtual void AuthSucceeded(HttpHost authhost, AuthScheme authScheme, HttpContext
			 context)
		{
			Args.NotNull(authhost, "Host");
			Args.NotNull(authScheme, "Auth scheme");
			Args.NotNull(context, "HTTP context");
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			if (IsCachable(authScheme))
			{
				AuthCache authCache = clientContext.GetAuthCache();
				if (authCache == null)
				{
					authCache = new BasicAuthCache();
					clientContext.SetAuthCache(authCache);
				}
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Caching '" + authScheme.GetSchemeName() + "' auth scheme for " + 
						authhost);
				}
				authCache.Put(authhost, authScheme);
			}
		}

		protected internal virtual bool IsCachable(AuthScheme authScheme)
		{
			if (authScheme == null || !authScheme.IsComplete())
			{
				return false;
			}
			string schemeName = authScheme.GetSchemeName();
			return Sharpen.Runtime.EqualsIgnoreCase(schemeName, AuthSchemes.Basic) || Sharpen.Runtime.EqualsIgnoreCase
				(schemeName, AuthSchemes.Digest);
		}

		public virtual void AuthFailed(HttpHost authhost, AuthScheme authScheme, HttpContext
			 context)
		{
			Args.NotNull(authhost, "Host");
			Args.NotNull(context, "HTTP context");
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			AuthCache authCache = clientContext.GetAuthCache();
			if (authCache != null)
			{
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Clearing cached auth scheme for " + authhost);
				}
				authCache.Remove(authhost);
			}
		}
	}
}
