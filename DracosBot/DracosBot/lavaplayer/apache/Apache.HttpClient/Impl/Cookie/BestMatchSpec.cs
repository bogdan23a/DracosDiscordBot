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
using Apache.Http;
using Apache.Http.Cookie;
using Apache.Http.Impl.Cookie;
using Apache.Http.Message;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>
	/// 'Meta' cookie specification that picks up a cookie policy based on
	/// the format of cookies sent with the HTTP response.
	/// </summary>
	/// <remarks>
	/// 'Meta' cookie specification that picks up a cookie policy based on
	/// the format of cookies sent with the HTTP response.
	/// </remarks>
	/// <since>4.0</since>
	public class BestMatchSpec : CookieSpec
	{
		private readonly string[] datepatterns;

		private readonly bool oneHeader;

		private RFC2965Spec strict;

		private RFC2109Spec obsoleteStrict;

		private BrowserCompatSpec compat;

		public BestMatchSpec(string[] datepatterns, bool oneHeader) : base()
		{
			// CookieSpec fields are @NotThreadSafe
			// Cached values of CookieSpec instances
			// @NotThreadSafe
			// @NotThreadSafe
			// @NotThreadSafe
			this.datepatterns = datepatterns == null ? null : datepatterns.Clone();
			this.oneHeader = oneHeader;
		}

		public BestMatchSpec() : this(null, false)
		{
		}

		private RFC2965Spec GetStrict()
		{
			if (this.strict == null)
			{
				this.strict = new RFC2965Spec(this.datepatterns, this.oneHeader);
			}
			return strict;
		}

		private RFC2109Spec GetObsoleteStrict()
		{
			if (this.obsoleteStrict == null)
			{
				this.obsoleteStrict = new RFC2109Spec(this.datepatterns, this.oneHeader);
			}
			return obsoleteStrict;
		}

		private BrowserCompatSpec GetCompat()
		{
			if (this.compat == null)
			{
				this.compat = new BrowserCompatSpec(this.datepatterns);
			}
			return compat;
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual IList<Apache.Http.Cookie.Cookie> Parse(Header header, CookieOrigin
			 origin)
		{
			Args.NotNull(header, "Header");
			Args.NotNull(origin, "Cookie origin");
			HeaderElement[] helems = header.GetElements();
			bool versioned = false;
			bool netscape = false;
			foreach (HeaderElement helem in helems)
			{
				if (helem.GetParameterByName("version") != null)
				{
					versioned = true;
				}
				if (helem.GetParameterByName("expires") != null)
				{
					netscape = true;
				}
			}
			if (netscape || !versioned)
			{
				// Need to parse the header again, because Netscape style cookies do not correctly
				// support multiple header elements (comma cannot be treated as an element separator)
				NetscapeDraftHeaderParser parser = NetscapeDraftHeaderParser.Default;
				CharArrayBuffer buffer;
				ParserCursor cursor;
				if (header is FormattedHeader)
				{
					buffer = ((FormattedHeader)header).GetBuffer();
					cursor = new ParserCursor(((FormattedHeader)header).GetValuePos(), buffer.Length(
						));
				}
				else
				{
					string s = header.GetValue();
					if (s == null)
					{
						throw new MalformedCookieException("Header value is null");
					}
					buffer = new CharArrayBuffer(s.Length);
					buffer.Append(s);
					cursor = new ParserCursor(0, buffer.Length());
				}
				helems = new HeaderElement[] { parser.ParseHeader(buffer, cursor) };
				return GetCompat().Parse(helems, origin);
			}
			else
			{
				if (SM.SetCookie2.Equals(header.GetName()))
				{
					return GetStrict().Parse(helems, origin);
				}
				else
				{
					return GetObsoleteStrict().Parse(helems, origin);
				}
			}
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			if (cookie.GetVersion() > 0)
			{
				if (cookie is SetCookie2)
				{
					GetStrict().Validate(cookie, origin);
				}
				else
				{
					GetObsoleteStrict().Validate(cookie, origin);
				}
			}
			else
			{
				GetCompat().Validate(cookie, origin);
			}
		}

		public virtual bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			if (cookie.GetVersion() > 0)
			{
				if (cookie is SetCookie2)
				{
					return GetStrict().Match(cookie, origin);
				}
				else
				{
					return GetObsoleteStrict().Match(cookie, origin);
				}
			}
			else
			{
				return GetCompat().Match(cookie, origin);
			}
		}

		public virtual IList<Header> FormatCookies(IList<Apache.Http.Cookie.Cookie> cookies
			)
		{
			Args.NotNull(cookies, "List of cookies");
			int version = int.MaxValue;
			bool isSetCookie2 = true;
			foreach (Apache.Http.Cookie.Cookie cookie in cookies)
			{
				if (!(cookie is SetCookie2))
				{
					isSetCookie2 = false;
				}
				if (cookie.GetVersion() < version)
				{
					version = cookie.GetVersion();
				}
			}
			if (version > 0)
			{
				if (isSetCookie2)
				{
					return GetStrict().FormatCookies(cookies);
				}
				else
				{
					return GetObsoleteStrict().FormatCookies(cookies);
				}
			}
			else
			{
				return GetCompat().FormatCookies(cookies);
			}
		}

		public virtual int GetVersion()
		{
			return GetStrict().GetVersion();
		}

		public virtual Header GetVersionHeader()
		{
			return GetStrict().GetVersionHeader();
		}

		public override string ToString()
		{
			return "best-match";
		}
	}
}
