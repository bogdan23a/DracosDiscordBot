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
	/// Cookie specification that strives to closely mimic (mis)behavior of
	/// common web browser applications such as Microsoft Internet Explorer
	/// and Mozilla FireFox.
	/// </summary>
	/// <remarks>
	/// Cookie specification that strives to closely mimic (mis)behavior of
	/// common web browser applications such as Microsoft Internet Explorer
	/// and Mozilla FireFox.
	/// </remarks>
	/// <since>4.0</since>
	public class BrowserCompatSpec : CookieSpecBase
	{
		private static readonly string[] DefaultDatePatterns = new string[] { DateUtils.PatternRfc1123
			, DateUtils.PatternRfc1036, DateUtils.PatternAsctime, "EEE, dd-MMM-yyyy HH:mm:ss z"
			, "EEE, dd-MMM-yyyy HH-mm-ss z", "EEE, dd MMM yy HH:mm:ss z", "EEE dd-MMM-yyyy HH:mm:ss z"
			, "EEE dd MMM yyyy HH:mm:ss z", "EEE dd-MMM-yyyy HH-mm-ss z", "EEE dd-MMM-yy HH:mm:ss z"
			, "EEE dd MMM yy HH:mm:ss z", "EEE,dd-MMM-yy HH:mm:ss z", "EEE,dd-MMM-yyyy HH:mm:ss z"
			, "EEE, dd-MM-yyyy HH:mm:ss z" };

		private readonly string[] datepatterns;

		/// <summary>Default constructor</summary>
		public BrowserCompatSpec(string[] datepatterns, BrowserCompatSpecFactory.SecurityLevel
			 securityLevel) : base()
		{
			// superclass is @NotThreadSafe
			if (datepatterns != null)
			{
				this.datepatterns = datepatterns.Clone();
			}
			else
			{
				this.datepatterns = DefaultDatePatterns;
			}
			switch (securityLevel)
			{
				case BrowserCompatSpecFactory.SecurityLevel.SecuritylevelDefault:
				{
					RegisterAttribHandler(ClientCookie.PathAttr, new BasicPathHandler());
					break;
				}

				case BrowserCompatSpecFactory.SecurityLevel.SecuritylevelIeMedium:
				{
					RegisterAttribHandler(ClientCookie.PathAttr, new _BasicPathHandler_95());
					// No validation
					break;
				}

				default:
				{
					throw new RuntimeException("Unknown security level");
				}
			}
			RegisterAttribHandler(ClientCookie.DomainAttr, new BasicDomainHandler());
			RegisterAttribHandler(ClientCookie.MaxAgeAttr, new BasicMaxAgeHandler());
			RegisterAttribHandler(ClientCookie.SecureAttr, new BasicSecureHandler());
			RegisterAttribHandler(ClientCookie.CommentAttr, new BasicCommentHandler());
			RegisterAttribHandler(ClientCookie.ExpiresAttr, new BasicExpiresHandler(this.datepatterns
				));
			RegisterAttribHandler(ClientCookie.VersionAttr, new BrowserCompatVersionAttributeHandler
				());
		}

		private sealed class _BasicPathHandler_95 : BasicPathHandler
		{
			public _BasicPathHandler_95()
			{
			}

			/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
			public override void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
				)
			{
			}
		}

		/// <summary>Default constructor</summary>
		public BrowserCompatSpec(string[] datepatterns) : this(datepatterns, BrowserCompatSpecFactory.SecurityLevel
			.SecuritylevelDefault)
		{
		}

		/// <summary>Default constructor</summary>
		public BrowserCompatSpec() : this(null, BrowserCompatSpecFactory.SecurityLevel.SecuritylevelDefault
			)
		{
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public override IList<Apache.Http.Cookie.Cookie> Parse(Header header, CookieOrigin
			 origin)
		{
			Args.NotNull(header, "Header");
			Args.NotNull(origin, "Cookie origin");
			string headername = header.GetName();
			if (!Sharpen.Runtime.EqualsIgnoreCase(headername, SM.SetCookie))
			{
				throw new MalformedCookieException("Unrecognized cookie header '" + header.ToString
					() + "'");
			}
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
			}
			return Parse(helems, origin);
		}

		private static bool IsQuoteEnclosed(string s)
		{
			return s != null && s.StartsWith("\"") && s.EndsWith("\"");
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
				string cookieName = cookie.GetName();
				string cookieValue = cookie.GetValue();
				if (cookie.GetVersion() > 0 && !IsQuoteEnclosed(cookieValue))
				{
					BasicHeaderValueFormatter.Instance.FormatHeaderElement(buffer, new BasicHeaderElement
						(cookieName, cookieValue), false);
				}
				else
				{
					// Netscape style cookies do not support quoted values
					buffer.Append(cookieName);
					buffer.Append("=");
					if (cookieValue != null)
					{
						buffer.Append(cookieValue);
					}
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
			return "compatibility";
		}
	}
}
