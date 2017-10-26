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
using System.Globalization;
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <summary>
	/// Abstract authentication scheme class that serves as a basis
	/// for all authentication schemes supported by HttpClient.
	/// </summary>
	/// <remarks>
	/// Abstract authentication scheme class that serves as a basis
	/// for all authentication schemes supported by HttpClient. This class
	/// defines the generic way of parsing an authentication challenge. It
	/// does not make any assumptions regarding the format of the challenge
	/// nor does it impose any specific way of responding to that challenge.
	/// </remarks>
	/// <since>4.0</since>
	public abstract class AuthSchemeBase : ContextAwareAuthScheme
	{
		private ChallengeState challengeState;

		/// <summary>
		/// Creates an instance of <tt>AuthSchemeBase</tt> with the given challenge
		/// state.
		/// </summary>
		/// <remarks>
		/// Creates an instance of <tt>AuthSchemeBase</tt> with the given challenge
		/// state.
		/// </remarks>
		/// <since>4.2</since>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) do not use.")]
		public AuthSchemeBase(ChallengeState challengeState) : base()
		{
			this.challengeState = challengeState;
		}

		public AuthSchemeBase() : base()
		{
		}

		/// <summary>Processes the given challenge token.</summary>
		/// <remarks>
		/// Processes the given challenge token. Some authentication schemes
		/// may involve multiple challenge-response exchanges. Such schemes must be able
		/// to maintain the state information when dealing with sequential challenges
		/// </remarks>
		/// <param name="header">the challenge header</param>
		/// <exception cref="Apache.Http.Auth.MalformedChallengeException">
		/// is thrown if the authentication challenge
		/// is malformed
		/// </exception>
		public virtual void ProcessChallenge(Header header)
		{
			Args.NotNull(header, "Header");
			string authheader = header.GetName();
			if (Sharpen.Runtime.EqualsIgnoreCase(authheader, AUTH.WwwAuth))
			{
				this.challengeState = ChallengeState.Target;
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(authheader, AUTH.ProxyAuth))
				{
					this.challengeState = ChallengeState.Proxy;
				}
				else
				{
					throw new MalformedChallengeException("Unexpected header name: " + authheader);
				}
			}
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
			if (!Sharpen.Runtime.EqualsIgnoreCase(s_1, GetSchemeName()))
			{
				throw new MalformedChallengeException("Invalid scheme identifier: " + s_1);
			}
			ParseChallenge(buffer, pos, buffer.Length());
		}

		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		public virtual Header Authenticate(Credentials credentials, IHttpRequest request, 
			HttpContext context)
		{
			return Authenticate(credentials, request);
		}

		/// <exception cref="Apache.Http.Auth.MalformedChallengeException"></exception>
		protected internal abstract void ParseChallenge(CharArrayBuffer buffer, int beginIndex
			, int endIndex);

		/// <summary>
		/// Returns <code>true</code> if authenticating against a proxy, <code>false</code>
		/// otherwise.
		/// </summary>
		/// <remarks>
		/// Returns <code>true</code> if authenticating against a proxy, <code>false</code>
		/// otherwise.
		/// </remarks>
		public virtual bool IsProxy()
		{
			return this.challengeState != null && this.challengeState == ChallengeState.Proxy;
		}

		/// <summary>
		/// Returns
		/// <see cref="Apache.Http.Auth.ChallengeState">Apache.Http.Auth.ChallengeState</see>
		/// value or <code>null</code> if unchallenged.
		/// </summary>
		/// <since>4.2</since>
		public virtual ChallengeState GetChallengeState()
		{
			return this.challengeState;
		}

		public override string ToString()
		{
			string name = GetSchemeName();
			if (name != null)
			{
				return name.ToUpper(CultureInfo.InvariantCulture);
			}
			else
			{
				return base.ToString();
			}
		}

		public abstract Header Authenticate(Credentials arg1, IHttpRequest arg2);

		public abstract string GetParameter(string arg1);

		public abstract string GetRealm();

		public abstract string GetSchemeName();

		public abstract bool IsComplete();

		public abstract bool IsConnectionBased();
	}
}
