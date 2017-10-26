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
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <since>4.3</since>
	public class HttpAuthenticator
	{
		private readonly Log log;

		public HttpAuthenticator(Log log) : base()
		{
			this.log = log != null ? log : LogFactory.GetLog(GetType());
		}

		public HttpAuthenticator() : this(null)
		{
		}

		public virtual bool IsAuthenticationRequested(HttpHost host, HttpResponse response
			, AuthenticationStrategy authStrategy, AuthState authState, HttpContext context)
		{
			if (authStrategy.IsAuthenticationRequested(host, response, context))
			{
				this.log.Debug("Authentication required");
				if (authState.GetState() == AuthProtocolState.Success)
				{
					authStrategy.AuthFailed(host, authState.GetAuthScheme(), context);
				}
				return true;
			}
			else
			{
				switch (authState.GetState())
				{
					case AuthProtocolState.Challenged:
					case AuthProtocolState.Handshake:
					{
						this.log.Debug("Authentication succeeded");
						authState.SetState(AuthProtocolState.Success);
						authStrategy.AuthSucceeded(host, authState.GetAuthScheme(), context);
						break;
					}

					case AuthProtocolState.Success:
					{
						break;
					}

					default:
					{
						authState.SetState(AuthProtocolState.Unchallenged);
						break;
					}
				}
				return false;
			}
		}

		public virtual bool HandleAuthChallenge(HttpHost host, HttpResponse response, AuthenticationStrategy
			 authStrategy, AuthState authState, HttpContext context)
		{
			try
			{
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug(host.ToHostString() + " requested authentication");
				}
				IDictionary<string, Header> challenges = authStrategy.GetChallenges(host, response
					, context);
				if (challenges.IsEmpty())
				{
					this.log.Debug("Response contains no authentication challenges");
					return false;
				}
				AuthScheme authScheme = authState.GetAuthScheme();
				switch (authState.GetState())
				{
					case AuthProtocolState.Failure:
					{
						return false;
					}

					case AuthProtocolState.Success:
					{
						authState.Reset();
						break;
					}

					case AuthProtocolState.Challenged:
					case AuthProtocolState.Handshake:
					{
						if (authScheme == null)
						{
							this.log.Debug("Auth scheme is null");
							authStrategy.AuthFailed(host, null, context);
							authState.Reset();
							authState.SetState(AuthProtocolState.Failure);
							return false;
						}
						goto case AuthProtocolState.Unchallenged;
					}

					case AuthProtocolState.Unchallenged:
					{
						if (authScheme != null)
						{
							string id = authScheme.GetSchemeName();
							Header challenge = challenges.Get(id.ToLower(CultureInfo.InvariantCulture));
							if (challenge != null)
							{
								this.log.Debug("Authorization challenge processed");
								authScheme.ProcessChallenge(challenge);
								if (authScheme.IsComplete())
								{
									this.log.Debug("Authentication failed");
									authStrategy.AuthFailed(host, authState.GetAuthScheme(), context);
									authState.Reset();
									authState.SetState(AuthProtocolState.Failure);
									return false;
								}
								else
								{
									authState.SetState(AuthProtocolState.Handshake);
									return true;
								}
							}
							else
							{
								authState.Reset();
							}
						}
					}
				}
				// Retry authentication with a different scheme
				Queue<AuthOption> authOptions = authStrategy.Select(challenges, host, response, context
					);
				if (authOptions != null && !authOptions.IsEmpty())
				{
					if (this.log.IsDebugEnabled())
					{
						this.log.Debug("Selected authentication options: " + authOptions);
					}
					authState.SetState(AuthProtocolState.Challenged);
					authState.Update(authOptions);
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (MalformedChallengeException ex)
			{
				if (this.log.IsWarnEnabled())
				{
					this.log.Warn("Malformed challenge: " + ex.Message);
				}
				authState.Reset();
				return false;
			}
		}

		/// <exception cref="Apache.Http.HttpException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void GenerateAuthResponse(IHttpRequest request, AuthState authState
			, HttpContext context)
		{
			AuthScheme authScheme = authState.GetAuthScheme();
			Credentials creds = authState.GetCredentials();
			switch (authState.GetState())
			{
				case AuthProtocolState.Failure:
				{
					return;
				}

				case AuthProtocolState.Success:
				{
					EnsureAuthScheme(authScheme);
					if (authScheme.IsConnectionBased())
					{
						return;
					}
					break;
				}

				case AuthProtocolState.Challenged:
				{
					Queue<AuthOption> authOptions = authState.GetAuthOptions();
					if (authOptions != null)
					{
						while (!authOptions.IsEmpty())
						{
							AuthOption authOption = authOptions.Remove();
							authScheme = authOption.GetAuthScheme();
							creds = authOption.GetCredentials();
							authState.Update(authScheme, creds);
							if (this.log.IsDebugEnabled())
							{
								this.log.Debug("Generating response to an authentication challenge using " + authScheme
									.GetSchemeName() + " scheme");
							}
							try
							{
								Header header = DoAuth(authScheme, creds, request, context);
								request.AddHeader(header);
								break;
							}
							catch (AuthenticationException ex)
							{
								if (this.log.IsWarnEnabled())
								{
									this.log.Warn(authScheme + " authentication error: " + ex.Message);
								}
							}
						}
						return;
					}
					else
					{
						EnsureAuthScheme(authScheme);
					}
				}
			}
			if (authScheme != null)
			{
				try
				{
					Header header = DoAuth(authScheme, creds, request, context);
					request.AddHeader(header);
				}
				catch (AuthenticationException ex)
				{
					if (this.log.IsErrorEnabled())
					{
						this.log.Error(authScheme + " authentication error: " + ex.Message);
					}
				}
			}
		}

		private void EnsureAuthScheme(AuthScheme authScheme)
		{
			Asserts.NotNull(authScheme, "Auth scheme");
		}

		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		private Header DoAuth(AuthScheme authScheme, Credentials creds, IHttpRequest request
			, HttpContext context)
		{
			if (authScheme is ContextAwareAuthScheme)
			{
				return ((ContextAwareAuthScheme)authScheme).Authenticate(creds, request, context);
			}
			else
			{
				return authScheme.Authenticate(creds, request);
			}
		}
	}
}
