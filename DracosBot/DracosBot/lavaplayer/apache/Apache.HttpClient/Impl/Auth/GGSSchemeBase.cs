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
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Client.Protocol;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Auth;
using Apache.Http.Message;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Codec.Binary;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <since>4.2</since>
	public abstract class GGSSchemeBase : AuthSchemeBase
	{
		internal enum State
		{
			Uninitiated,
			ChallengeReceived,
			TokenGenerated,
			Failed
		}

		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly Base64 base64codec;

		private readonly bool stripPort;

		/// <summary>Authentication process state</summary>
		private GGSSchemeBase.State state;

		/// <summary>base64 decoded challenge</summary>
		private byte[] token;

		internal GGSSchemeBase(bool stripPort) : base()
		{
			this.base64codec = new Base64(0);
			this.stripPort = stripPort;
			this.state = GGSSchemeBase.State.Uninitiated;
		}

		internal GGSSchemeBase() : this(false)
		{
		}

		protected internal virtual GSSManager GetManager()
		{
			return GSSManager.GetInstance();
		}

		/// <exception cref="Sharpen.GSSException"></exception>
		protected internal virtual byte[] GenerateGSSToken(byte[] input, Oid oid, string 
			authServer)
		{
			byte[] token = input;
			if (token == null)
			{
				token = new byte[0];
			}
			GSSManager manager = GetManager();
			GSSName serverName = manager.CreateName("HTTP@" + authServer, GSSName.NtHostbasedService
				);
			Sharpen.GSSContext gssContext = manager.CreateContext(serverName.Canonicalize(oid
				), oid, null, Sharpen.GSSContext.DefaultLifetime);
			gssContext.RequestMutualAuth(true);
			gssContext.RequestCredDeleg(true);
			return gssContext.InitSecContext(token, 0, token.Length);
		}

		/// <exception cref="Sharpen.GSSException"></exception>
		protected internal abstract byte[] GenerateToken(byte[] input, string authServer);

		public override bool IsComplete()
		{
			return this.state == GGSSchemeBase.State.TokenGenerated || this.state == GGSSchemeBase.State
				.Failed;
		}

		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2) Use Apache.Http.Auth.ContextAwareAuthScheme.Authenticate(Apache.Http.Auth.Credentials, Org.Apache.Http.IHttpRequest, Apache.Http.Protocol.HttpContext)"
			)]
		public override Header Authenticate(Credentials credentials, IHttpRequest request
			)
		{
			return Authenticate(credentials, request, null);
		}

		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		public override Header Authenticate(Credentials credentials, IHttpRequest request
			, HttpContext context)
		{
			Args.NotNull(request, "HTTP request");
			switch (state)
			{
				case GGSSchemeBase.State.Uninitiated:
				{
					throw new AuthenticationException(GetSchemeName() + " authentication has not been initiated"
						);
				}

				case GGSSchemeBase.State.Failed:
				{
					throw new AuthenticationException(GetSchemeName() + " authentication has failed");
				}

				case GGSSchemeBase.State.ChallengeReceived:
				{
					try
					{
						HttpRoute route = (HttpRoute)context.GetAttribute(HttpClientContext.HttpRoute);
						if (route == null)
						{
							throw new AuthenticationException("Connection route is not available");
						}
						HttpHost host;
						if (IsProxy())
						{
							host = route.GetProxyHost();
							if (host == null)
							{
								host = route.GetTargetHost();
							}
						}
						else
						{
							host = route.GetTargetHost();
						}
						string authServer;
						if (!this.stripPort && host.GetPort() > 0)
						{
							authServer = host.ToHostString();
						}
						else
						{
							authServer = host.GetHostName();
						}
						if (log.IsDebugEnabled())
						{
							log.Debug("init " + authServer);
						}
						token = GenerateToken(token, authServer);
						state = GGSSchemeBase.State.TokenGenerated;
					}
					catch (GSSException gsse)
					{
						state = GGSSchemeBase.State.Failed;
						if (gsse.GetMajor() == GSSException.DefectiveCredential || gsse.GetMajor() == GSSException
							.CredentialsExpired)
						{
							throw new InvalidCredentialsException(gsse.Message, gsse);
						}
						if (gsse.GetMajor() == GSSException.NoCred)
						{
							throw new InvalidCredentialsException(gsse.Message, gsse);
						}
						if (gsse.GetMajor() == GSSException.DefectiveToken || gsse.GetMajor() == GSSException
							.DuplicateToken || gsse.GetMajor() == GSSException.OldToken)
						{
							throw new AuthenticationException(gsse.Message, gsse);
						}
						// other error
						throw new AuthenticationException(gsse.Message);
					}
					goto case GGSSchemeBase.State.TokenGenerated;
				}

				case GGSSchemeBase.State.TokenGenerated:
				{
					string tokenstr = Sharpen.Runtime.GetStringForBytes(base64codec.Encode(token));
					if (log.IsDebugEnabled())
					{
						log.Debug("Sending response '" + tokenstr + "' back to the auth server");
					}
					CharArrayBuffer buffer = new CharArrayBuffer(32);
					if (IsProxy())
					{
						buffer.Append(AUTH.ProxyAuthResp);
					}
					else
					{
						buffer.Append(AUTH.WwwAuthResp);
					}
					buffer.Append(": Negotiate ");
					buffer.Append(tokenstr);
					return new BufferedHeader(buffer);
				}

				default:
				{
					throw new InvalidOperationException("Illegal state: " + state);
				}
			}
		}

		/// <exception cref="Apache.Http.Auth.MalformedChallengeException"></exception>
		protected internal override void ParseChallenge(CharArrayBuffer buffer, int beginIndex
			, int endIndex)
		{
			string challenge = buffer.SubstringTrimmed(beginIndex, endIndex);
			if (log.IsDebugEnabled())
			{
				log.Debug("Received challenge '" + challenge + "' from the auth server");
			}
			if (state == GGSSchemeBase.State.Uninitiated)
			{
				token = Base64.DecodeBase64(Sharpen.Runtime.GetBytesForString(challenge));
				state = GGSSchemeBase.State.ChallengeReceived;
			}
			else
			{
				log.Debug("Authentication already attempted");
				state = GGSSchemeBase.State.Failed;
			}
		}
	}
}
