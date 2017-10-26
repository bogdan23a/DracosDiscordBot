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
	/// <summary>
	/// The Strict HostnameVerifier works the same way as Sun Java 1.4, Sun
	/// Java 5, Sun Java 6-rc.
	/// </summary>
	/// <remarks>
	/// The Strict HostnameVerifier works the same way as Sun Java 1.4, Sun
	/// Java 5, Sun Java 6-rc.  It's also pretty close to IE6.  This
	/// implementation appears to be compliant with RFC 2818 for dealing with
	/// wildcards.
	/// <p/>
	/// The hostname must match either the first CN, or any of the subject-alts.
	/// A wildcard can occur in the CN, and in any of the subject-alts.  The
	/// one divergence from IE6 is how we only check the first CN.  IE6 allows
	/// a match against any of the CNs present.  We decided to follow in
	/// Sun Java 1.4's footsteps and only check the first CN.  (If you need
	/// to check all the CN's, feel free to write your own implementation!).
	/// <p/>
	/// A wildcard such as "*.foo.com" matches only subdomains in the same
	/// level, for example "a.foo.com".  It does not match deeper subdomains
	/// such as "a.b.foo.com".
	/// </remarks>
	/// <since>4.0</since>
	public class StrictHostnameVerifier : AbstractVerifier
	{
		/// <exception cref="Sharpen.SSLException"></exception>
		public sealed override void Verify(string host, string[] cns, string[] subjectAlts
			)
		{
			Verify(host, cns, subjectAlts, true);
		}

		public sealed override string ToString()
		{
			return "STRICT";
		}
	}
}
