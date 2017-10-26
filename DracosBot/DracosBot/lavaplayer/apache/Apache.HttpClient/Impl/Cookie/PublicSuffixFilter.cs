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
using Apache.Http.Client.Utils;
using Apache.Http.Cookie;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>
	/// Wraps a CookieAttributeHandler and leverages its match method
	/// to never match a suffix from a black list.
	/// </summary>
	/// <remarks>
	/// Wraps a CookieAttributeHandler and leverages its match method
	/// to never match a suffix from a black list. May be used to provide
	/// additional security for cross-site attack types by preventing
	/// cookies from apparent domains that are not publicly available.
	/// An uptodate list of suffixes can be obtained from
	/// <a href="http://publicsuffix.org/">publicsuffix.org</a>
	/// </remarks>
	/// <since>4.0</since>
	public class PublicSuffixFilter : CookieAttributeHandler
	{
		private readonly CookieAttributeHandler wrapped;

		private ICollection<string> exceptions;

		private ICollection<string> suffixes;

		public PublicSuffixFilter(CookieAttributeHandler wrapped)
		{
			this.wrapped = wrapped;
		}

		/// <summary>Sets the suffix blacklist patterns.</summary>
		/// <remarks>
		/// Sets the suffix blacklist patterns.
		/// A pattern can be "com", "*.jp"
		/// TODO add support for patterns like "lib.*.us"
		/// </remarks>
		/// <param name="suffixes"></param>
		public virtual void SetPublicSuffixes(ICollection<string> suffixes)
		{
			this.suffixes = new HashSet<string>(suffixes);
		}

		/// <summary>Sets the exceptions from the blacklist.</summary>
		/// <remarks>
		/// Sets the exceptions from the blacklist. Exceptions can not be patterns.
		/// TODO add support for patterns
		/// </remarks>
		/// <param name="exceptions"></param>
		public virtual void SetExceptions(ICollection<string> exceptions)
		{
			this.exceptions = new HashSet<string>(exceptions);
		}

		/// <summary>Never matches if the cookie's domain is from the blacklist.</summary>
		/// <remarks>Never matches if the cookie's domain is from the blacklist.</remarks>
		public virtual bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			if (IsForPublicSuffix(cookie))
			{
				return false;
			}
			return wrapped.Match(cookie, origin);
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Parse(SetCookie cookie, string value)
		{
			wrapped.Parse(cookie, value);
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
			wrapped.Validate(cookie, origin);
		}

		private bool IsForPublicSuffix(Apache.Http.Cookie.Cookie cookie)
		{
			string domain = cookie.GetDomain();
			if (domain.StartsWith("."))
			{
				domain = Sharpen.Runtime.Substring(domain, 1);
			}
			domain = Punycode.ToUnicode(domain);
			// An exception rule takes priority over any other matching rule.
			if (this.exceptions != null)
			{
				if (this.exceptions.Contains(domain))
				{
					return false;
				}
			}
			if (this.suffixes == null)
			{
				return false;
			}
			do
			{
				if (this.suffixes.Contains(domain))
				{
					return true;
				}
				// patterns
				if (domain.StartsWith("*."))
				{
					domain = Sharpen.Runtime.Substring(domain, 2);
				}
				int nextdot = domain.IndexOf('.');
				if (nextdot == -1)
				{
					break;
				}
				domain = "*" + Sharpen.Runtime.Substring(domain, nextdot);
			}
			while (domain.Length > 0);
			return false;
		}
	}
}
