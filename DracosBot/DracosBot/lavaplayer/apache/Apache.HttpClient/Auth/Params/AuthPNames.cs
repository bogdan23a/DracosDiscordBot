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

using Apache.Http.Auth.Params;
using Sharpen;

namespace Apache.Http.Auth.Params
{
	/// <summary>Parameter names for HTTP authentication classes.</summary>
	/// <remarks>Parameter names for HTTP authentication classes.</remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) use Apache.Http.Client.Config.RequestConfig and constructor parameters ofApache.Http.Auth.AuthSchemeProvider s."
		)]
	public abstract class AuthPNames
	{
		/// <summary>
		/// Defines the charset to be used when encoding
		/// <see cref="Apache.Http.Auth.Credentials">Apache.Http.Auth.Credentials</see>
		/// .
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="string">string</see>
		/// .
		/// </summary>
		public const string CredentialCharset = "http.auth.credential-charset";

		/// <summary>
		/// Defines the order of preference for supported
		/// <see cref="Apache.Http.Auth.AuthScheme">Apache.Http.Auth.AuthScheme</see>
		/// s when authenticating with
		/// the target host.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="System.Collections.ICollection{E}">System.Collections.ICollection&lt;E&gt;
		/// 	</see>
		/// . The
		/// collection is expected to contain
		/// <see cref="string">string</see>
		/// instances representing
		/// a name of an authentication scheme as returned by
		/// <see cref="Apache.Http.Auth.AuthScheme.GetSchemeName()">Apache.Http.Auth.AuthScheme.GetSchemeName()
		/// 	</see>
		/// .
		/// </summary>
		public const string TargetAuthPref = "http.auth.target-scheme-pref";

		/// <summary>
		/// Defines the order of preference for supported
		/// <see cref="Apache.Http.Auth.AuthScheme">Apache.Http.Auth.AuthScheme</see>
		/// s when authenticating with the
		/// proxy host.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="System.Collections.ICollection{E}">System.Collections.ICollection&lt;E&gt;
		/// 	</see>
		/// . The
		/// collection is expected to contain
		/// <see cref="string">string</see>
		/// instances representing
		/// a name of an authentication scheme as returned by
		/// <see cref="Apache.Http.Auth.AuthScheme.GetSchemeName()">Apache.Http.Auth.AuthScheme.GetSchemeName()
		/// 	</see>
		/// .
		/// </summary>
		public const string ProxyAuthPref = "http.auth.proxy-scheme-pref";
	}
}
