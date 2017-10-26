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
using System.Text;
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Auth.Params;
using Apache.Http.Impl.Auth;
using Apache.Http.Message;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <summary>
	/// Abstract authentication scheme class that lays foundation for all
	/// RFC 2617 compliant authentication schemes and provides capabilities common
	/// to all authentication schemes defined in RFC 2617.
	/// </summary>
	/// <remarks>
	/// Abstract authentication scheme class that lays foundation for all
	/// RFC 2617 compliant authentication schemes and provides capabilities common
	/// to all authentication schemes defined in RFC 2617.
	/// </remarks>
	/// <since>4.0</since>
	public abstract class RFC2617Scheme : AuthSchemeBase
	{
		private readonly IDictionary<string, string> @params;

		private readonly Encoding credentialsCharset;

		/// <summary>
		/// Creates an instance of <tt>RFC2617Scheme</tt> with the given challenge
		/// state.
		/// </summary>
		/// <remarks>
		/// Creates an instance of <tt>RFC2617Scheme</tt> with the given challenge
		/// state.
		/// </remarks>
		/// <since>4.2</since>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) do not use.")]
		public RFC2617Scheme(ChallengeState challengeState) : base(challengeState)
		{
			// AuthSchemeBase, params
			this.@params = new Dictionary<string, string>();
			this.credentialsCharset = Consts.Ascii;
		}

		/// <since>4.3</since>
		public RFC2617Scheme(Encoding credentialsCharset) : base()
		{
			this.@params = new Dictionary<string, string>();
			this.credentialsCharset = credentialsCharset != null ? credentialsCharset : Consts
				.Ascii;
		}

		public RFC2617Scheme() : this(Consts.Ascii)
		{
		}

		/// <since>4.3</since>
		public virtual Encoding GetCredentialsCharset()
		{
			return credentialsCharset;
		}

		internal virtual string GetCredentialsCharset(IHttpRequest request)
		{
			string charset = (string)request.GetParams().GetParameter(AuthPNames.CredentialCharset
				);
			if (charset == null)
			{
				charset = GetCredentialsCharset().Name();
			}
			return charset;
		}

		/// <exception cref="Apache.Http.Auth.MalformedChallengeException"></exception>
		protected internal override void ParseChallenge(CharArrayBuffer buffer, int pos, 
			int len)
		{
			HeaderValueParser parser = BasicHeaderValueParser.Instance;
			ParserCursor cursor = new ParserCursor(pos, buffer.Length());
			HeaderElement[] elements = parser.ParseElements(buffer, cursor);
			if (elements.Length == 0)
			{
				throw new MalformedChallengeException("Authentication challenge is empty");
			}
			this.@params.Clear();
			foreach (HeaderElement element in elements)
			{
				this.@params.Put(element.GetName(), element.GetValue());
			}
		}

		/// <summary>Returns authentication parameters map.</summary>
		/// <remarks>Returns authentication parameters map. Keys in the map are lower-cased.</remarks>
		/// <returns>the map of authentication parameters</returns>
		protected internal virtual IDictionary<string, string> GetParameters()
		{
			return this.@params;
		}

		/// <summary>Returns authentication parameter with the given name, if available.</summary>
		/// <remarks>Returns authentication parameter with the given name, if available.</remarks>
		/// <param name="name">The name of the parameter to be returned</param>
		/// <returns>the parameter with the given name</returns>
		public override string GetParameter(string name)
		{
			if (name == null)
			{
				return null;
			}
			return this.@params.Get(name.ToLower(Sharpen.Extensions.GetEnglishCulture()));
		}

		/// <summary>Returns authentication realm.</summary>
		/// <remarks>Returns authentication realm. The realm may not be null.</remarks>
		/// <returns>the authentication realm</returns>
		public override string GetRealm()
		{
			return GetParameter("realm");
		}
	}
}
