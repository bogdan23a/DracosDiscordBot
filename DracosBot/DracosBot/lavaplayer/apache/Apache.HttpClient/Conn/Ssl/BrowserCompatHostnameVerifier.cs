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

using Apache.Http.Conn.Ssl;
using Sharpen;

namespace Apache.Http.Conn.Ssl
{
	/// <summary>The HostnameVerifier that works the same way as Curl and Firefox.</summary>
	/// <remarks>
	/// The HostnameVerifier that works the same way as Curl and Firefox.
	/// <p/>
	/// The hostname must match either the first CN, or any of the subject-alts.
	/// A wildcard can occur in the CN, and in any of the subject-alts.
	/// <p/>
	/// The only difference between BROWSER_COMPATIBLE and STRICT is that a wildcard
	/// (such as "*.foo.com") with BROWSER_COMPATIBLE matches all subdomains,
	/// including "a.b.foo.com".
	/// </remarks>
	/// <since>4.0</since>
	public class BrowserCompatHostnameVerifier : AbstractVerifier
	{
		/// <exception cref="Sharpen.SSLException"></exception>
		public sealed override void Verify(string host, string[] cns, string[] subjectAlts
			)
		{
			Verify(host, cns, subjectAlts, false);
		}

		internal override bool ValidCountryWildcard(string cn)
		{
			return true;
		}

		public sealed override string ToString()
		{
			return "BROWSER_COMPATIBLE";
		}
	}
}
