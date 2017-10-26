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
	/// RFC 2965 compliant
	/// <see cref="Apache.Http.Cookie.CookieSpec">Apache.Http.Cookie.CookieSpec</see>
	/// implementation.
	/// </summary>
	/// <since>4.0</since>
	public class RFC2965Spec : RFC2109Spec
	{
		/// <summary>Default constructor</summary>
		public RFC2965Spec() : this(null, false)
		{
		}

		public RFC2965Spec(string[] datepatterns, bool oneHeader) : base(datepatterns, oneHeader
			)
		{
			// superclass is @NotThreadSafe
			RegisterAttribHandler(ClientCookie.DomainAttr, new RFC2965DomainAttributeHandler(
				));
			RegisterAttribHandler(ClientCookie.PortAttr, new RFC2965PortAttributeHandler());
			RegisterAttribHandler(ClientCookie.CommenturlAttr, new RFC2965CommentUrlAttributeHandler
				());
			RegisterAttribHandler(ClientCookie.DiscardAttr, new RFC2965DiscardAttributeHandler
				());
			RegisterAttribHandler(ClientCookie.VersionAttr, new RFC2965VersionAttributeHandler
				());
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public override IList<Apache.Http.Cookie.Cookie> Parse(Header header, CookieOrigin
			 origin)
		{
			Args.NotNull(header, "Header");
			Args.NotNull(origin, "Cookie origin");
			if (!Sharpen.Runtime.EqualsIgnoreCase(header.GetName(), SM.SetCookie2))
			{
				throw new MalformedCookieException("Unrecognized cookie header '" + header.ToString
					() + "'");
			}
			HeaderElement[] elems = header.GetElements();
			return CreateCookies(elems, AdjustEffectiveHost(origin));
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		protected internal override IList<Apache.Http.Cookie.Cookie> Parse(HeaderElement[]
			 elems, CookieOrigin origin)
		{
			return CreateCookies(elems, AdjustEffectiveHost(origin));
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		private IList<Apache.Http.Cookie.Cookie> CreateCookies(HeaderElement[] elems, CookieOrigin
			 origin)
		{
			IList<Apache.Http.Cookie.Cookie> cookies = new AList<Apache.Http.Cookie.Cookie>(elems
				.Length);
			foreach (HeaderElement headerelement in elems)
			{
				string name = headerelement.GetName();
				string value = headerelement.GetValue();
				if (name == null || name.Length == 0)
				{
					throw new MalformedCookieException("Cookie name may not be empty");
				}
				BasicClientCookie2 cookie = new BasicClientCookie2(name, value);
				cookie.SetPath(GetDefaultPath(origin));
				cookie.SetDomain(GetDefaultDomain(origin));
				cookie.SetPorts(new int[] { origin.GetPort() });
				// cycle through the parameters
				NameValuePair[] attribs = headerelement.GetParameters();
				// Eliminate duplicate attributes. The first occurrence takes precedence
				// See RFC2965: 3.2  Origin Server Role
				IDictionary<string, NameValuePair> attribmap = new Dictionary<string, NameValuePair
					>(attribs.Length);
				for (int j = attribs.Length - 1; j >= 0; j--)
				{
					NameValuePair param = attribs[j];
					attribmap.Put(param.GetName().ToLower(Sharpen.Extensions.GetEnglishCulture()), param
						);
				}
				foreach (KeyValuePair<string, NameValuePair> entry in attribmap.EntrySet())
				{
					NameValuePair attrib = entry.Value;
					string s = attrib.GetName().ToLower(Sharpen.Extensions.GetEnglishCulture());
					cookie.SetAttribute(s, attrib.GetValue());
					CookieAttributeHandler handler = FindAttribHandler(s);
					if (handler != null)
					{
						handler.Parse(cookie, attrib.GetValue());
					}
				}
				cookies.AddItem(cookie);
			}
			return cookies;
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public override void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			base.Validate(cookie, AdjustEffectiveHost(origin));
		}

		public override bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			return base.Match(cookie, AdjustEffectiveHost(origin));
		}

		/// <summary>Adds valid Port attribute value, e.g.</summary>
		/// <remarks>Adds valid Port attribute value, e.g. "8000,8001,8002"</remarks>
		protected internal override void FormatCookieAsVer(CharArrayBuffer buffer, Apache.Http.Cookie.Cookie
			 cookie, int version)
		{
			base.FormatCookieAsVer(buffer, cookie, version);
			// format port attribute
			if (cookie is ClientCookie)
			{
				// Test if the port attribute as set by the origin server is not blank
				string s = ((ClientCookie)cookie).GetAttribute(ClientCookie.PortAttr);
				if (s != null)
				{
					buffer.Append("; $Port");
					buffer.Append("=\"");
					if (s.Trim().Length > 0)
					{
						int[] ports = cookie.GetPorts();
						if (ports != null)
						{
							int len = ports.Length;
							for (int i = 0; i < len; i++)
							{
								if (i > 0)
								{
									buffer.Append(",");
								}
								buffer.Append(Sharpen.Extensions.ToString(ports[i]));
							}
						}
					}
					buffer.Append("\"");
				}
			}
		}

		/// <summary>Set 'effective host name' as defined in RFC 2965.</summary>
		/// <remarks>
		/// Set 'effective host name' as defined in RFC 2965.
		/// <p>
		/// If a host name contains no dots, the effective host name is
		/// that name with the string .local appended to it.  Otherwise
		/// the effective host name is the same as the host name.  Note
		/// that all effective host names contain at least one dot.
		/// </remarks>
		/// <param name="origin">origin where cookie is received from or being sent to.</param>
		private static CookieOrigin AdjustEffectiveHost(CookieOrigin origin)
		{
			string host = origin.GetHost();
			// Test if the host name appears to be a fully qualified DNS name,
			// IPv4 address or IPv6 address
			bool isLocalHost = true;
			for (int i = 0; i < host.Length; i++)
			{
				char ch = host[i];
				if (ch == '.' || ch == ':')
				{
					isLocalHost = false;
					break;
				}
			}
			if (isLocalHost)
			{
				host += ".local";
				return new CookieOrigin(host, origin.GetPort(), origin.GetPath(), origin.IsSecure
					());
			}
			else
			{
				return origin;
			}
		}

		public override int GetVersion()
		{
			return 1;
		}

		public override Header GetVersionHeader()
		{
			CharArrayBuffer buffer = new CharArrayBuffer(40);
			buffer.Append(SM.Cookie2);
			buffer.Append(": ");
			buffer.Append("$Version=");
			buffer.Append(Sharpen.Extensions.ToString(GetVersion()));
			return new BufferedHeader(buffer);
		}

		public override string ToString()
		{
			return "rfc2965";
		}
	}
}
