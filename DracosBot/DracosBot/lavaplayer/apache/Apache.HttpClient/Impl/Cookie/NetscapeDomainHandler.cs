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

using Apache.Http.Cookie;
using Apache.Http.Impl.Cookie;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <since>4.0</since>
	public class NetscapeDomainHandler : BasicDomainHandler
	{
		public NetscapeDomainHandler() : base()
		{
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public override void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
			base.Validate(cookie, origin);
			// Perform Netscape Cookie draft specific validation
			string host = origin.GetHost();
			string domain = cookie.GetDomain();
			if (host.Contains("."))
			{
				int domainParts = new StringTokenizer(domain, ".").CountTokens();
				if (IsSpecialDomain(domain))
				{
					if (domainParts < 2)
					{
						throw new CookieRestrictionViolationException("Domain attribute \"" + domain + "\" violates the Netscape cookie specification for "
							 + "special domains");
					}
				}
				else
				{
					if (domainParts < 3)
					{
						throw new CookieRestrictionViolationException("Domain attribute \"" + domain + "\" violates the Netscape cookie specification"
							);
					}
				}
			}
		}

		/// <summary>
		/// Checks if the given domain is in one of the seven special
		/// top level domains defined by the Netscape cookie specification.
		/// </summary>
		/// <remarks>
		/// Checks if the given domain is in one of the seven special
		/// top level domains defined by the Netscape cookie specification.
		/// </remarks>
		/// <param name="domain">The domain.</param>
		/// <returns>True if the specified domain is "special"</returns>
		private static bool IsSpecialDomain(string domain)
		{
			string ucDomain = domain.ToUpper(Sharpen.Extensions.GetEnglishCulture());
			return ucDomain.EndsWith(".COM") || ucDomain.EndsWith(".EDU") || ucDomain.EndsWith
				(".NET") || ucDomain.EndsWith(".GOV") || ucDomain.EndsWith(".MIL") || ucDomain.EndsWith
				(".ORG") || ucDomain.EndsWith(".INT");
		}

		public override bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			string host = origin.GetHost();
			string domain = cookie.GetDomain();
			if (domain == null)
			{
				return false;
			}
			return host.EndsWith(domain);
		}
	}
}
