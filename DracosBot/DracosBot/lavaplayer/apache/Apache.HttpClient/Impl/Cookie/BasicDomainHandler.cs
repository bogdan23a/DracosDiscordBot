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
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <since>4.0</since>
	public class BasicDomainHandler : CookieAttributeHandler
	{
		public BasicDomainHandler() : base()
		{
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Parse(SetCookie cookie, string value)
		{
			Args.NotNull(cookie, "Cookie");
			if (value == null)
			{
				throw new MalformedCookieException("Missing value for domain attribute");
			}
			if (value.Trim().Length == 0)
			{
				throw new MalformedCookieException("Blank value for domain attribute");
			}
			cookie.SetDomain(value);
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			// Validate the cookies domain attribute.  NOTE:  Domains without
			// any dots are allowed to support hosts on private LANs that don't
			// have DNS names.  Since they have no dots, to domain-match the
			// request-host and domain must be identical for the cookie to sent
			// back to the origin-server.
			string host = origin.GetHost();
			string domain = cookie.GetDomain();
			if (domain == null)
			{
				throw new CookieRestrictionViolationException("Cookie domain may not be null");
			}
			if (host.Contains("."))
			{
				// Not required to have at least two dots.  RFC 2965.
				// A Set-Cookie2 with Domain=ajax.com will be accepted.
				// domain must match host
				if (!host.EndsWith(domain))
				{
					if (domain.StartsWith("."))
					{
						domain = Sharpen.Runtime.Substring(domain, 1, domain.Length);
					}
					if (!host.Equals(domain))
					{
						throw new CookieRestrictionViolationException("Illegal domain attribute \"" + domain
							 + "\". Domain of origin: \"" + host + "\"");
					}
				}
			}
			else
			{
				if (!host.Equals(domain))
				{
					throw new CookieRestrictionViolationException("Illegal domain attribute \"" + domain
						 + "\". Domain of origin: \"" + host + "\"");
				}
			}
		}

		public virtual bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			string host = origin.GetHost();
			string domain = cookie.GetDomain();
			if (domain == null)
			{
				return false;
			}
			if (host.Equals(domain))
			{
				return true;
			}
			if (!domain.StartsWith("."))
			{
				domain = '.' + domain;
			}
			return host.EndsWith(domain) || host.Equals(Sharpen.Runtime.Substring(domain, 1));
		}
	}
}
