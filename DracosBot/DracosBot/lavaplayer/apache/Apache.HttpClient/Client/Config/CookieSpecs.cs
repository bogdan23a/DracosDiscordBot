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
	/// <summary>Standard cookie specifications supported by HttpClient.</summary>
	/// <remarks>Standard cookie specifications supported by HttpClient.</remarks>
	/// <since>4.3</since>
	public sealed class CookieSpecs
	{
		/// <summary>
		/// The policy that provides high degree of compatibility
		/// with common cookie management of popular HTTP agents.
		/// </summary>
		/// <remarks>
		/// The policy that provides high degree of compatibility
		/// with common cookie management of popular HTTP agents.
		/// </remarks>
		public const string BrowserCompatibility = "compatibility";

		/// <summary>The Netscape cookie draft compliant policy.</summary>
		/// <remarks>The Netscape cookie draft compliant policy.</remarks>
		public const string Netscape = "netscape";

		/// <summary>The RFC 2965 compliant policy (standard).</summary>
		/// <remarks>The RFC 2965 compliant policy (standard).</remarks>
		public const string Standard = "standard";

		/// <summary>The default 'best match' policy.</summary>
		/// <remarks>The default 'best match' policy.</remarks>
		public const string BestMatch = "best-match";

		/// <summary>The policy that ignores cookies.</summary>
		/// <remarks>The policy that ignores cookies.</remarks>
		public const string IgnoreCookies = "ignoreCookies";

		private CookieSpecs()
		{
		}
	}
}
