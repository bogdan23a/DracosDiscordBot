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
using Apache.Http.Impl.Auth;
using Apache.Http.Message;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <summary>
	/// NTLM is a proprietary authentication scheme developed by Microsoft
	/// and optimized for Windows platforms.
	/// </summary>
	/// <remarks>
	/// NTLM is a proprietary authentication scheme developed by Microsoft
	/// and optimized for Windows platforms.
	/// </remarks>
	/// <since>4.0</since>
	public class NTLMScheme : AuthSchemeBase
	{
		internal enum State
		{
			Uninitiated,
			ChallengeReceived,
			MsgType1Generated,
			MsgType2Recevied,
			MsgType3Generated,
			Failed
		}

		private readonly NTLMEngine engine;

		private NTLMScheme.State state;

		private string challenge;

		public NTLMScheme(NTLMEngine engine) : base()
		{
			Args.NotNull(engine, "NTLM engine");
			this.engine = engine;
			this.state = NTLMScheme.State.Uninitiated;
			this.challenge = null;
		}

		/// <since>4.3</since>
		public NTLMScheme() : this(new NTLMEngineImpl())
		{
		}

		public override string GetSchemeName()
		{
			return "ntlm";
		}

		public override string GetParameter(string name)
		{
			// String parameters not supported
			return null;
		}

		public override string GetRealm()
		{
			// NTLM does not support the concept of an authentication realm
			return null;
		}

		public override bool IsConnectionBased()
		{
			return true;
		}

		/// <exception cref="Apache.Http.Auth.MalformedChallengeException"></exception>
		protected internal override void ParseChallenge(CharArrayBuffer buffer, int beginIndex
			, int endIndex)
		{
			this.challenge = buffer.SubstringTrimmed(beginIndex, endIndex);
			if (this.challenge.Length == 0)
			{
				if (this.state == NTLMScheme.State.Uninitiated)
				{
					this.state = NTLMScheme.State.ChallengeReceived;
				}
				else
				{
					this.state = NTLMScheme.State.Failed;
				}
			}
			else
			{
				if (this.state.CompareTo(NTLMScheme.State.MsgType1Generated) < 0)
				{
					this.state = NTLMScheme.State.Failed;
					throw new MalformedChallengeException("Out of sequence NTLM response message");
				}
				else
				{
					if (this.state == NTLMScheme.State.MsgType1Generated)
					{
						this.state = NTLMScheme.State.MsgType2Recevied;
					}
				}
			}
		}

		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		public override Header Authenticate(Credentials credentials, IHttpRequest request
			)
		{
			NTCredentials ntcredentials = null;
			try
			{
				ntcredentials = (NTCredentials)credentials;
			}
			catch (InvalidCastException)
			{
				throw new InvalidCredentialsException("Credentials cannot be used for NTLM authentication: "
					 + credentials.GetType().FullName);
			}
			string response = null;
			if (this.state == NTLMScheme.State.Failed)
			{
				throw new AuthenticationException("NTLM authentication failed");
			}
			else
			{
				if (this.state == NTLMScheme.State.ChallengeReceived)
				{
					response = this.engine.GenerateType1Msg(ntcredentials.GetDomain(), ntcredentials.
						GetWorkstation());
					this.state = NTLMScheme.State.MsgType1Generated;
				}
				else
				{
					if (this.state == NTLMScheme.State.MsgType2Recevied)
					{
						response = this.engine.GenerateType3Msg(ntcredentials.GetUserName(), ntcredentials
							.GetPassword(), ntcredentials.GetDomain(), ntcredentials.GetWorkstation(), this.
							challenge);
						this.state = NTLMScheme.State.MsgType3Generated;
					}
					else
					{
						throw new AuthenticationException("Unexpected state: " + this.state);
					}
				}
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
			buffer.Append(": NTLM ");
			buffer.Append(response);
			return new BufferedHeader(buffer);
		}

		public override bool IsComplete()
		{
			return this.state == NTLMScheme.State.MsgType3Generated || this.state == NTLMScheme.State
				.Failed;
		}
	}
}
