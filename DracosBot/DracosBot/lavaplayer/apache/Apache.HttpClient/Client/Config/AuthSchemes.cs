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

using Sharpen;

namespace Apache.Http.Client.Config
{
	/// <summary>Standard authentication schemes supported by HttpClient.</summary>
	/// <remarks>Standard authentication schemes supported by HttpClient.</remarks>
	/// <since>4.3</since>
	public sealed class AuthSchemes
	{
		/// <summary>
		/// Basic authentication scheme as defined in RFC2617 (considered inherently
		/// insecure, but most widely supported)
		/// </summary>
		public const string Basic = "Basic";

		/// <summary>Digest authentication scheme as defined in RFC2617.</summary>
		/// <remarks>Digest authentication scheme as defined in RFC2617.</remarks>
		public const string Digest = "Digest";

		/// <summary>
		/// The NTLM scheme is a proprietary Microsoft Windows Authentication
		/// protocol (considered to be the most secure among currently supported
		/// authentication schemes).
		/// </summary>
		/// <remarks>
		/// The NTLM scheme is a proprietary Microsoft Windows Authentication
		/// protocol (considered to be the most secure among currently supported
		/// authentication schemes).
		/// </remarks>
		public const string Ntlm = "NTLM";

		/// <summary>SPNEGO Authentication scheme.</summary>
		/// <remarks>SPNEGO Authentication scheme.</remarks>
		public const string Spnego = "negotiate";

		/// <summary>Kerberos Authentication scheme.</summary>
		/// <remarks>Kerberos Authentication scheme.</remarks>
		public const string Kerberos = "Kerberos";

		private AuthSchemes()
		{
		}
	}
}
