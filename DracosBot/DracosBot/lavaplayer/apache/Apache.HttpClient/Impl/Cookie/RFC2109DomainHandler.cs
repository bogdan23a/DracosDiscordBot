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
	public class RFC2109DomainHandler : CookieAttributeHandler
	{
		public RFC2109DomainHandler() : base()
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
			string host = origin.GetHost();
			string domain = cookie.GetDomain();
			if (domain == null)
			{
				throw new CookieRestrictionViolationException("Cookie domain may not be null");
			}
			if (!domain.Equals(host))
			{
				int dotIndex = domain.IndexOf('.');
				if (dotIndex == -1)
				{
					throw new CookieRestrictionViolationException("Domain attribute \"" + domain + "\" does not match the host \""
						 + host + "\"");
				}
				// domain must start with dot
				if (!domain.StartsWith("."))
				{
					throw new CookieRestrictionViolationException("Domain attribute \"" + domain + "\" violates RFC 2109: domain must start with a dot"
						);
				}
				// domain must have at least one embedded dot
				dotIndex = domain.IndexOf('.', 1);
				if (dotIndex < 0 || dotIndex == domain.Length - 1)
				{
					throw new CookieRestrictionViolationException("Domain attribute \"" + domain + "\" violates RFC 2109: domain must contain an embedded dot"
						);
				}
				host = host.ToLower(Sharpen.Extensions.GetEnglishCulture());
				if (!host.EndsWith(domain))
				{
					throw new CookieRestrictionViolationException("Illegal domain attribute \"" + domain
						 + "\". Domain of origin: \"" + host + "\"");
				}
				// host minus domain may not contain any dots
				string hostWithoutDomain = Sharpen.Runtime.Substring(host, 0, host.Length - domain
					.Length);
				if (hostWithoutDomain.IndexOf('.') != -1)
				{
					throw new CookieRestrictionViolationException("Domain attribute \"" + domain + "\" violates RFC 2109: host minus domain may not contain any dots"
						);
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
			return host.Equals(domain) || (domain.StartsWith(".") && host.EndsWith(domain));
		}
	}
}
