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
using Apache.Http.Client.Utils;
using Apache.Http.Cookie;
using Apache.Http.Impl.Cookie;
using Apache.Http.Message;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>
	/// RFC 2109 compliant
	/// <see cref="Apache.Http.Cookie.CookieSpec">Apache.Http.Cookie.CookieSpec</see>
	/// implementation.
	/// This is an older version of the official HTTP state management specification
	/// superseded by RFC 2965.
	/// </summary>
	/// <seealso cref="RFC2965Spec">RFC2965Spec</seealso>
	/// <since>4.0</since>
	public class RFC2109Spec : CookieSpecBase
	{
		private static readonly CookiePathComparator PathComparator = new CookiePathComparator
			();

		private static readonly string[] DatePatterns = new string[] { DateUtils.PatternRfc1123
			, DateUtils.PatternRfc1036, DateUtils.PatternAsctime };

		private readonly string[] datepatterns;

		private readonly bool oneHeader;

		/// <summary>Default constructor</summary>
		public RFC2109Spec(string[] datepatterns, bool oneHeader) : base()
		{
			// superclass is @NotThreadSafe
			if (datepatterns != null)
			{
				this.datepatterns = datepatterns.Clone();
			}
			else
			{
				this.datepatterns = DatePatterns;
			}
			this.oneHeader = oneHeader;
			RegisterAttribHandler(ClientCookie.VersionAttr, new RFC2109VersionHandler());
			RegisterAttribHandler(ClientCookie.PathAttr, new BasicPathHandler());
			RegisterAttribHandler(ClientCookie.DomainAttr, new RFC2109DomainHandler());
			RegisterAttribHandler(ClientCookie.MaxAgeAttr, new BasicMaxAgeHandler());
			RegisterAttribHandler(ClientCookie.SecureAttr, new BasicSecureHandler());
			RegisterAttribHandler(ClientCookie.CommentAttr, new BasicCommentHandler());
			RegisterAttribHandler(ClientCookie.ExpiresAttr, new BasicExpiresHandler(this.datepatterns
				));
		}

		/// <summary>Default constructor</summary>
		public RFC2109Spec() : this(null, false)
		{
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public override IList<Apache.Http.Cookie.Cookie> Parse(Header header, CookieOrigin
			 origin)
		{
			Args.NotNull(header, "Header");
			Args.NotNull(origin, "Cookie origin");
			if (!Sharpen.Runtime.EqualsIgnoreCase(header.GetName(), SM.SetCookie))
			{
				throw new MalformedCookieException("Unrecognized cookie header '" + header.ToString
					() + "'");
			}
			HeaderElement[] elems = header.GetElements();
			return Parse(elems, origin);
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public override void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
			Args.NotNull(cookie, "Cookie");
			string name = cookie.GetName();
			if (name.IndexOf(' ') != -1)
			{
				throw new CookieRestrictionViolationException("Cookie name may not contain blanks"
					);
			}
			if (name.StartsWith("$"))
			{
				throw new CookieRestrictionViolationException("Cookie name may not start with $");
			}
			base.Validate(cookie, origin);
		}

		public override IList<Header> FormatCookies(IList<Apache.Http.Cookie.Cookie> cookies
			)
		{
			Args.NotEmpty(cookies, "List of cookies");
			IList<Apache.Http.Cookie.Cookie> cookieList;
			if (cookies.Count > 1)
			{
				// Create a mutable copy and sort the copy.
				cookieList = new AList<Apache.Http.Cookie.Cookie>(cookies);
				cookieList.Sort(PathComparator);
			}
			else
			{
				cookieList = cookies;
			}
			if (this.oneHeader)
			{
				return DoFormatOneHeader(cookieList);
			}
			else
			{
				return DoFormatManyHeaders(cookieList);
			}
		}

		private IList<Header> DoFormatOneHeader(IList<Apache.Http.Cookie.Cookie> cookies)
		{
			int version = int.MaxValue;
			// Pick the lowest common denominator
			foreach (Apache.Http.Cookie.Cookie cookie in cookies)
			{
				if (cookie.GetVersion() < version)
				{
					version = cookie.GetVersion();
				}
			}
			CharArrayBuffer buffer = new CharArrayBuffer(40 * cookies.Count);
			buffer.Append(SM.Cookie);
			buffer.Append(": ");
			buffer.Append("$Version=");
			buffer.Append(Sharpen.Extensions.ToString(version));
			foreach (Apache.Http.Cookie.Cookie cooky in cookies)
			{
				buffer.Append("; ");
				Apache.Http.Cookie.Cookie cookie_1 = cooky;
				FormatCookieAsVer(buffer, cookie_1, version);
			}
			IList<Header> headers = new AList<Header>(1);
			headers.AddItem(new BufferedHeader(buffer));
			return headers;
		}

		private IList<Header> DoFormatManyHeaders(IList<Apache.Http.Cookie.Cookie> cookies
			)
		{
			IList<Header> headers = new AList<Header>(cookies.Count);
			foreach (Apache.Http.Cookie.Cookie cookie in cookies)
			{
				int version = cookie.GetVersion();
				CharArrayBuffer buffer = new CharArrayBuffer(40);
				buffer.Append("Cookie: ");
				buffer.Append("$Version=");
				buffer.Append(Sharpen.Extensions.ToString(version));
				buffer.Append("; ");
				FormatCookieAsVer(buffer, cookie, version);
				headers.AddItem(new BufferedHeader(buffer));
			}
			return headers;
		}

		/// <summary>
		/// Return a name/value string suitable for sending in a <tt>"Cookie"</tt>
		/// header as defined in RFC 2109 for backward compatibility with cookie
		/// version 0
		/// </summary>
		/// <param name="buffer">The char array buffer to use for output</param>
		/// <param name="name">The cookie name</param>
		/// <param name="value">The cookie value</param>
		/// <param name="version">The cookie version</param>
		protected internal virtual void FormatParamAsVer(CharArrayBuffer buffer, string name
			, string value, int version)
		{
			buffer.Append(name);
			buffer.Append("=");
			if (value != null)
			{
				if (version > 0)
				{
					buffer.Append('\"');
					buffer.Append(value);
					buffer.Append('\"');
				}
				else
				{
					buffer.Append(value);
				}
			}
		}

		/// <summary>
		/// Return a string suitable for sending in a <tt>"Cookie"</tt> header
		/// as defined in RFC 2109 for backward compatibility with cookie version 0
		/// </summary>
		/// <param name="buffer">The char array buffer to use for output</param>
		/// <param name="cookie">
		/// The
		/// <see cref="Apache.Http.Cookie.Cookie">Apache.Http.Cookie.Cookie</see>
		/// to be formatted as string
		/// </param>
		/// <param name="version">The version to use.</param>
		protected internal virtual void FormatCookieAsVer(CharArrayBuffer buffer, Apache.Http.Cookie.Cookie
			 cookie, int version)
		{
			FormatParamAsVer(buffer, cookie.GetName(), cookie.GetValue(), version);
			if (cookie.GetPath() != null)
			{
				if (cookie is ClientCookie && ((ClientCookie)cookie).ContainsAttribute(ClientCookie
					.PathAttr))
				{
					buffer.Append("; ");
					FormatParamAsVer(buffer, "$Path", cookie.GetPath(), version);
				}
			}
			if (cookie.GetDomain() != null)
			{
				if (cookie is ClientCookie && ((ClientCookie)cookie).ContainsAttribute(ClientCookie
					.DomainAttr))
				{
					buffer.Append("; ");
					FormatParamAsVer(buffer, "$Domain", cookie.GetDomain(), version);
				}
			}
		}

		public override int GetVersion()
		{
			return 1;
		}

		public override Header GetVersionHeader()
		{
			return null;
		}

		public override string ToString()
		{
			return "rfc2109";
		}
	}
}
