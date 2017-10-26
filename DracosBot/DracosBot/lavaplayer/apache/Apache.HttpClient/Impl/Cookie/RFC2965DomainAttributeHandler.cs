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
	/// <summary><tt>"Domain"</tt> cookie attribute handler for RFC 2965 cookie spec.</summary>
	/// <remarks><tt>"Domain"</tt> cookie attribute handler for RFC 2965 cookie spec.</remarks>
	/// <since>3.1</since>
	public class RFC2965DomainAttributeHandler : CookieAttributeHandler
	{
		public RFC2965DomainAttributeHandler() : base()
		{
		}

		/// <summary>Parse cookie domain attribute.</summary>
		/// <remarks>Parse cookie domain attribute.</remarks>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Parse(SetCookie cookie, string domain)
		{
			Args.NotNull(cookie, "Cookie");
			if (domain == null)
			{
				throw new MalformedCookieException("Missing value for domain attribute");
			}
			if (domain.Trim().Length == 0)
			{
				throw new MalformedCookieException("Blank value for domain attribute");
			}
			string s = domain;
			s = s.ToLower(Sharpen.Extensions.GetEnglishCulture());
			if (!domain.StartsWith("."))
			{
				// Per RFC 2965 section 3.2.2
				// "... If an explicitly specified value does not start with
				// a dot, the user agent supplies a leading dot ..."
				// That effectively implies that the domain attribute
				// MAY NOT be an IP address of a host name
				s = '.' + s;
			}
			cookie.SetDomain(s);
		}

		/// <summary>Performs domain-match as defined by the RFC2965.</summary>
		/// <remarks>
		/// Performs domain-match as defined by the RFC2965.
		/// <p>
		/// Host A's name domain-matches host B's if
		/// <ol>
		/// <ul>their host name strings string-compare equal; or</ul>
		/// <ul>A is a HDN string and has the form NB, where N is a non-empty
		/// name string, B has the form .B', and B' is a HDN string.  (So,
		/// x.y.com domain-matches .Y.com but not Y.com.)</ul>
		/// </ol>
		/// </remarks>
		/// <param name="host">host name where cookie is received from or being sent to.</param>
		/// <param name="domain">The cookie domain attribute.</param>
		/// <returns>true if the specified host matches the given domain.</returns>
		public virtual bool DomainMatch(string host, string domain)
		{
			bool match = host.Equals(domain) || (domain.StartsWith(".") && host.EndsWith(domain
				));
			return match;
		}

		/// <summary>Validate cookie domain attribute.</summary>
		/// <remarks>Validate cookie domain attribute.</remarks>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			string host = origin.GetHost().ToLower(Sharpen.Extensions.GetEnglishCulture());
			if (cookie.GetDomain() == null)
			{
				throw new CookieRestrictionViolationException("Invalid cookie state: " + "domain not specified"
					);
			}
			string cookieDomain = cookie.GetDomain().ToLower(Sharpen.Extensions.GetEnglishCulture()
				);
			if (cookie is ClientCookie && ((ClientCookie)cookie).ContainsAttribute(ClientCookie
				.DomainAttr))
			{
				// Domain attribute must start with a dot
				if (!cookieDomain.StartsWith("."))
				{
					throw new CookieRestrictionViolationException("Domain attribute \"" + cookie.GetDomain
						() + "\" violates RFC 2109: domain must start with a dot");
				}
				// Domain attribute must contain at least one embedded dot,
				// or the value must be equal to .local.
				int dotIndex = cookieDomain.IndexOf('.', 1);
				if (((dotIndex < 0) || (dotIndex == cookieDomain.Length - 1)) && (!cookieDomain.Equals
					(".local")))
				{
					throw new CookieRestrictionViolationException("Domain attribute \"" + cookie.GetDomain
						() + "\" violates RFC 2965: the value contains no embedded dots " + "and the value is not .local"
						);
				}
				// The effective host name must domain-match domain attribute.
				if (!DomainMatch(host, cookieDomain))
				{
					throw new CookieRestrictionViolationException("Domain attribute \"" + cookie.GetDomain
						() + "\" violates RFC 2965: effective host name does not " + "domain-match domain attribute."
						);
				}
				// effective host name minus domain must not contain any dots
				string effectiveHostWithoutDomain = Sharpen.Runtime.Substring(host, 0, host.Length
					 - cookieDomain.Length);
				if (effectiveHostWithoutDomain.IndexOf('.') != -1)
				{
					throw new CookieRestrictionViolationException("Domain attribute \"" + cookie.GetDomain
						() + "\" violates RFC 2965: " + "effective host minus domain may not contain any dots"
						);
				}
			}
			else
			{
				// Domain was not specified in header. In this case, domain must
				// string match request host (case-insensitive).
				if (!cookie.GetDomain().Equals(host))
				{
					throw new CookieRestrictionViolationException("Illegal domain attribute: \"" + cookie
						.GetDomain() + "\"." + "Domain of origin: \"" + host + "\"");
				}
			}
		}

		/// <summary>Match cookie domain attribute.</summary>
		/// <remarks>Match cookie domain attribute.</remarks>
		public virtual bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			string host = origin.GetHost().ToLower(Sharpen.Extensions.GetEnglishCulture());
			string cookieDomain = cookie.GetDomain();
			// The effective host name MUST domain-match the Domain
			// attribute of the cookie.
			if (!DomainMatch(host, cookieDomain))
			{
				return false;
			}
			// effective host name minus domain must not contain any dots
			string effectiveHostWithoutDomain = Sharpen.Runtime.Substring(host, 0, host.Length
				 - cookieDomain.Length);
			return effectiveHostWithoutDomain.IndexOf('.') == -1;
		}
	}
}
