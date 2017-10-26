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
	/// This
	/// <see cref="Apache.Http.Cookie.CookieSpec">Apache.Http.Cookie.CookieSpec</see>
	/// implementation conforms to
	/// the original draft specification published by Netscape Communications.
	/// It should be avoided unless absolutely necessary for compatibility with
	/// legacy applications.
	/// </summary>
	/// <since>4.0</since>
	public class NetscapeDraftSpec : CookieSpecBase
	{
		protected internal const string ExpiresPattern = "EEE, dd-MMM-yy HH:mm:ss z";

		private readonly string[] datepatterns;

		/// <summary>Default constructor</summary>
		public NetscapeDraftSpec(string[] datepatterns) : base()
		{
			// superclass is @NotThreadSafe
			if (datepatterns != null)
			{
				this.datepatterns = datepatterns.Clone();
			}
			else
			{
				this.datepatterns = new string[] { ExpiresPattern };
			}
			RegisterAttribHandler(ClientCookie.PathAttr, new BasicPathHandler());
			RegisterAttribHandler(ClientCookie.DomainAttr, new NetscapeDomainHandler());
			RegisterAttribHandler(ClientCookie.MaxAgeAttr, new BasicMaxAgeHandler());
			RegisterAttribHandler(ClientCookie.SecureAttr, new BasicSecureHandler());
			RegisterAttribHandler(ClientCookie.CommentAttr, new BasicCommentHandler());
			RegisterAttribHandler(ClientCookie.ExpiresAttr, new BasicExpiresHandler(this.datepatterns
				));
		}

		/// <summary>Default constructor</summary>
		public NetscapeDraftSpec() : this(null)
		{
		}

		/// <summary>Parses the Set-Cookie value into an array of <tt>Cookie</tt>s.</summary>
		/// <remarks>
		/// Parses the Set-Cookie value into an array of <tt>Cookie</tt>s.
		/// <p>Syntax of the Set-Cookie HTTP Response Header:</p>
		/// <p>This is the format a CGI script would use to add to
		/// the HTTP headers a new piece of data which is to be stored by
		/// the client for later retrieval.</p>
		/// <PRE>
		/// Set-Cookie: NAME=VALUE; expires=DATE; path=PATH; domain=DOMAIN_NAME; secure
		/// </PRE>
		/// <p>Please note that the Netscape draft specification does not fully conform to the HTTP
		/// header format. Comma character if present in <code>Set-Cookie</code> will not be treated
		/// as a header element separator</p>
		/// </remarks>
		/// <seealso><a href="http://web.archive.org/web/20020803110822/http://wp.netscape.com/newsref/std/cookie_spec.html">
		/// *  The Cookie Spec.</a></seealso>
		/// <param name="header">the <tt>Set-Cookie</tt> received from the server</param>
		/// <returns>an array of <tt>Cookie</tt>s parsed from the Set-Cookie value</returns>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException">if an exception occurs during parsing
		/// 	</exception>
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
			return Parse(new HeaderElement[] { parser.ParseHeader(buffer, cursor) }, origin);
		}

		public override IList<Header> FormatCookies(IList<Apache.Http.Cookie.Cookie> cookies
			)
		{
			Args.NotEmpty(cookies, "List of cookies");
			CharArrayBuffer buffer = new CharArrayBuffer(20 * cookies.Count);
			buffer.Append(SM.Cookie);
			buffer.Append(": ");
			for (int i = 0; i < cookies.Count; i++)
			{
				Apache.Http.Cookie.Cookie cookie = cookies[i];
				if (i > 0)
				{
					buffer.Append("; ");
				}
				buffer.Append(cookie.GetName());
				string s = cookie.GetValue();
				if (s != null)
				{
					buffer.Append("=");
					buffer.Append(s);
				}
			}
			IList<Header> headers = new AList<Header>(1);
			headers.AddItem(new BufferedHeader(buffer));
			return headers;
		}

		public override int GetVersion()
		{
			return 0;
		}

		public override Header GetVersionHeader()
		{
			return null;
		}

		public override string ToString()
		{
			return "netscape";
		}
	}
}
