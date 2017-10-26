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
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Impl.Client;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Implementation of
	/// <see cref="Apache.Http.Client.CredentialsProvider">Apache.Http.Client.CredentialsProvider
	/// 	</see>
	/// backed by standard
	/// JRE
	/// <see cref="Sharpen.Authenticator">Sharpen.Authenticator</see>
	/// .
	/// </summary>
	/// <since>4.3</since>
	public class SystemDefaultCredentialsProvider : CredentialsProvider
	{
		private static readonly IDictionary<string, string> SchemeMap;

		static SystemDefaultCredentialsProvider()
		{
			SchemeMap = new ConcurrentHashMap<string, string>();
			SchemeMap.Put(AuthSchemes.Basic.ToUpper(Sharpen.Extensions.GetEnglishCulture()), 
				"Basic");
			SchemeMap.Put(AuthSchemes.Digest.ToUpper(Sharpen.Extensions.GetEnglishCulture()), 
				"Digest");
			SchemeMap.Put(AuthSchemes.Ntlm.ToUpper(Sharpen.Extensions.GetEnglishCulture()), "NTLM"
				);
			SchemeMap.Put(AuthSchemes.Spnego.ToUpper(Sharpen.Extensions.GetEnglishCulture()), 
				"SPNEGO");
			SchemeMap.Put(AuthSchemes.Kerberos.ToUpper(Sharpen.Extensions.GetEnglishCulture()
				), "Kerberos");
		}

		private static string TranslateScheme(string key)
		{
			if (key == null)
			{
				return null;
			}
			string s = SchemeMap.Get(key);
			return s != null ? s : key;
		}

		private readonly BasicCredentialsProvider @internal;

		/// <summary>Default constructor.</summary>
		/// <remarks>Default constructor.</remarks>
		public SystemDefaultCredentialsProvider() : base()
		{
			this.@internal = new BasicCredentialsProvider();
		}

		public virtual void SetCredentials(AuthScope authscope, Credentials credentials)
		{
			@internal.SetCredentials(authscope, credentials);
		}

		private static PasswordAuthentication GetSystemCreds(AuthScope authscope, Authenticator.RequestorType
			 requestorType)
		{
			return Authenticator.RequestPasswordAuthentication(authscope.GetHost(), null, authscope
				.GetPort(), "http", null, TranslateScheme(authscope.GetScheme()), null, requestorType
				);
		}

		public virtual Credentials GetCredentials(AuthScope authscope)
		{
			Args.NotNull(authscope, "Auth scope");
			Credentials localcreds = @internal.GetCredentials(authscope);
			if (localcreds != null)
			{
				return localcreds;
			}
			if (authscope.GetHost() != null)
			{
				PasswordAuthentication systemcreds = GetSystemCreds(authscope, Authenticator.RequestorType
					.Server);
				if (systemcreds == null)
				{
					systemcreds = GetSystemCreds(authscope, Authenticator.RequestorType.Proxy);
				}
				if (systemcreds != null)
				{
					return new UsernamePasswordCredentials(systemcreds.GetUserName(), new string(systemcreds
						.GetPassword()));
				}
			}
			return null;
		}

		public virtual void Clear()
		{
			@internal.Clear();
		}
	}
}
