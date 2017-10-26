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
using System.Text;
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Impl.Auth;
using Apache.Http.Message;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Codec.Binary;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <summary>Basic authentication scheme as defined in RFC 2617.</summary>
	/// <remarks>Basic authentication scheme as defined in RFC 2617.</remarks>
	/// <since>4.0</since>
	public class BasicScheme : RFC2617Scheme
	{
		private readonly Base64 base64codec;

		/// <summary>Whether the basic authentication process is complete</summary>
		private bool complete;

		/// <since>4.3</since>
		public BasicScheme(Encoding credentialsCharset) : base(credentialsCharset)
		{
			this.base64codec = new Base64(0);
			this.complete = false;
		}

		/// <summary>
		/// Creates an instance of <tt>BasicScheme</tt> with the given challenge
		/// state.
		/// </summary>
		/// <remarks>
		/// Creates an instance of <tt>BasicScheme</tt> with the given challenge
		/// state.
		/// </remarks>
		/// <since>4.2</since>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) do not use.")]
		public BasicScheme(ChallengeState challengeState) : base(challengeState)
		{
			this.base64codec = new Base64(0);
		}

		public BasicScheme() : this(Consts.Ascii)
		{
		}

		/// <summary>Returns textual designation of the basic authentication scheme.</summary>
		/// <remarks>Returns textual designation of the basic authentication scheme.</remarks>
		/// <returns><code>basic</code></returns>
		public override string GetSchemeName()
		{
			return "basic";
		}

		/// <summary>Processes the Basic challenge.</summary>
		/// <remarks>Processes the Basic challenge.</remarks>
		/// <param name="header">the challenge header</param>
		/// <exception cref="Apache.Http.Auth.MalformedChallengeException">
		/// is thrown if the authentication challenge
		/// is malformed
		/// </exception>
		public override void ProcessChallenge(Header header)
		{
			base.ProcessChallenge(header);
			this.complete = true;
		}

		/// <summary>Tests if the Basic authentication process has been completed.</summary>
		/// <remarks>Tests if the Basic authentication process has been completed.</remarks>
		/// <returns>
		/// <tt>true</tt> if Basic authorization has been processed,
		/// <tt>false</tt> otherwise.
		/// </returns>
		public override bool IsComplete()
		{
			return this.complete;
		}

		/// <summary>Returns <tt>false</tt>.</summary>
		/// <remarks>Returns <tt>false</tt>. Basic authentication scheme is request based.</remarks>
		/// <returns><tt>false</tt>.</returns>
		public override bool IsConnectionBased()
		{
			return false;
		}

		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2) Use Apache.Http.Auth.ContextAwareAuthScheme.Authenticate(Apache.Http.Auth.Credentials, Org.Apache.Http.IHttpRequest, Apache.Http.Protocol.HttpContext)"
			)]
		public override Header Authenticate(Credentials credentials, IHttpRequest request
			)
		{
			return Authenticate(credentials, request, new BasicHttpContext());
		}

		/// <summary>
		/// Produces basic authorization header for the given set of
		/// <see cref="Apache.Http.Auth.Credentials">Apache.Http.Auth.Credentials</see>
		/// .
		/// </summary>
		/// <param name="credentials">The set of credentials to be used for authentication</param>
		/// <param name="request">The request being authenticated</param>
		/// <exception cref="Apache.Http.Auth.InvalidCredentialsException">
		/// if authentication
		/// credentials are not valid or not applicable for this authentication scheme
		/// </exception>
		/// <exception cref="Apache.Http.Auth.AuthenticationException">
		/// if authorization string cannot
		/// be generated due to an authentication failure
		/// </exception>
		/// <returns>a basic authorization string</returns>
		public override Header Authenticate(Credentials credentials, IHttpRequest request
			, HttpContext context)
		{
			Args.NotNull(credentials, "Credentials");
			Args.NotNull(request, "HTTP request");
			StringBuilder tmp = new StringBuilder();
			tmp.Append(credentials.GetUserPrincipal().GetName());
			tmp.Append(":");
			tmp.Append((credentials.GetPassword() == null) ? "null" : credentials.GetPassword
				());
			byte[] base64password = base64codec.Encode(EncodingUtils.GetBytes(tmp.ToString(), 
				GetCredentialsCharset(request)));
			CharArrayBuffer buffer = new CharArrayBuffer(32);
			if (IsProxy())
			{
				buffer.Append(AUTH.ProxyAuthResp);
			}
			else
			{
				buffer.Append(AUTH.WwwAuthResp);
			}
			buffer.Append(": Basic ");
			buffer.Append(base64password, 0, base64password.Length);
			return new BufferedHeader(buffer);
		}

		/// <summary>
		/// Returns a basic <tt>Authorization</tt> header value for the given
		/// <see cref="Apache.Http.Auth.Credentials">Apache.Http.Auth.Credentials</see>
		/// and charset.
		/// </summary>
		/// <param name="credentials">The credentials to encode.</param>
		/// <param name="charset">The charset to use for encoding the credentials</param>
		/// <returns>a basic authorization header</returns>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) use Authenticate(Apache.Http.Auth.Credentials, Org.Apache.Http.IHttpRequest, Apache.Http.Protocol.HttpContext) ."
			)]
		public static Header Authenticate(Credentials credentials, string charset, bool proxy
			)
		{
			Args.NotNull(credentials, "Credentials");
			Args.NotNull(charset, "charset");
			StringBuilder tmp = new StringBuilder();
			tmp.Append(credentials.GetUserPrincipal().GetName());
			tmp.Append(":");
			tmp.Append((credentials.GetPassword() == null) ? "null" : credentials.GetPassword
				());
			byte[] base64password = Base64.EncodeBase64(EncodingUtils.GetBytes(tmp.ToString()
				, charset), false);
			CharArrayBuffer buffer = new CharArrayBuffer(32);
			if (proxy)
			{
				buffer.Append(AUTH.ProxyAuthResp);
			}
			else
			{
				buffer.Append(AUTH.WwwAuthResp);
			}
			buffer.Append(": Basic ");
			buffer.Append(base64password, 0, base64password.Length);
			return new BufferedHeader(buffer);
		}
	}
}
