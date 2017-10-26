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
using Apache.Http.Impl.Auth;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <summary>
	/// SPNEGO (Simple and Protected GSSAPI Negotiation Mechanism) authentication
	/// scheme.
	/// </summary>
	/// <remarks>
	/// SPNEGO (Simple and Protected GSSAPI Negotiation Mechanism) authentication
	/// scheme.
	/// </remarks>
	/// <since>4.2</since>
	public class SPNegoScheme : GGSSchemeBase
	{
		private const string SpnegoOid = "1.3.6.1.5.5.2";

		public SPNegoScheme(bool stripPort) : base(stripPort)
		{
		}

		public SPNegoScheme() : base(false)
		{
		}

		public override string GetSchemeName()
		{
			return "Negotiate";
		}

		/// <summary>
		/// Produces SPNEGO authorization Header based on token created by
		/// processChallenge.
		/// </summary>
		/// <remarks>
		/// Produces SPNEGO authorization Header based on token created by
		/// processChallenge.
		/// </remarks>
		/// <param name="credentials">not used by the SPNEGO scheme.</param>
		/// <param name="request">The request being authenticated</param>
		/// <exception cref="Apache.Http.Auth.AuthenticationException">
		/// if authentication string cannot
		/// be generated due to an authentication failure
		/// </exception>
		/// <returns>SPNEGO authentication Header</returns>
		public override Header Authenticate(Credentials credentials, IHttpRequest request
			, HttpContext context)
		{
			return base.Authenticate(credentials, request, context);
		}

		/// <exception cref="Sharpen.GSSException"></exception>
		protected internal override byte[] GenerateToken(byte[] input, string authServer)
		{
			return GenerateGSSToken(input, new Oid(SpnegoOid), authServer);
		}

		/// <summary>
		/// There are no valid parameters for SPNEGO authentication so this
		/// method always returns <code>null</code>.
		/// </summary>
		/// <remarks>
		/// There are no valid parameters for SPNEGO authentication so this
		/// method always returns <code>null</code>.
		/// </remarks>
		/// <returns><code>null</code></returns>
		public override string GetParameter(string name)
		{
			Args.NotNull(name, "Parameter name");
			return null;
		}

		/// <summary>
		/// The concept of an authentication realm is not supported by the Negotiate
		/// authentication scheme.
		/// </summary>
		/// <remarks>
		/// The concept of an authentication realm is not supported by the Negotiate
		/// authentication scheme. Always returns <code>null</code>.
		/// </remarks>
		/// <returns><code>null</code></returns>
		public override string GetRealm()
		{
			return null;
		}

		/// <summary>Returns <tt>true</tt>.</summary>
		/// <remarks>Returns <tt>true</tt>. SPNEGO authentication scheme is connection based.
		/// 	</remarks>
		/// <returns><tt>true</tt>.</returns>
		public override bool IsConnectionBased()
		{
			return true;
		}
	}
}
