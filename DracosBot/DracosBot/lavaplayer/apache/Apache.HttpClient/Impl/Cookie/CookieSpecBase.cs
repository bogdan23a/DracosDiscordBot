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
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>Cookie management functions shared by all specification.</summary>
	/// <remarks>Cookie management functions shared by all specification.</remarks>
	/// <since>4.0</since>
	public abstract class CookieSpecBase : AbstractCookieSpec
	{
		// AbstractCookieSpec is not thread-safe
		protected internal static string GetDefaultPath(CookieOrigin origin)
		{
			string defaultPath = origin.GetPath();
			int lastSlashIndex = defaultPath.LastIndexOf('/');
			if (lastSlashIndex >= 0)
			{
				if (lastSlashIndex == 0)
				{
					//Do not remove the very first slash
					lastSlashIndex = 1;
				}
				defaultPath = Sharpen.Runtime.Substring(defaultPath, 0, lastSlashIndex);
			}
			return defaultPath;
		}

		protected internal static string GetDefaultDomain(CookieOrigin origin)
		{
			return origin.GetHost();
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		protected internal virtual IList<Apache.Http.Cookie.Cookie> Parse(HeaderElement[]
			 elems, CookieOrigin origin)
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
				BasicClientCookie cookie = new BasicClientCookie(name, value);
				cookie.SetPath(GetDefaultPath(origin));
				cookie.SetDomain(GetDefaultDomain(origin));
				// cycle through the parameters
				NameValuePair[] attribs = headerelement.GetParameters();
				for (int j = attribs.Length - 1; j >= 0; j--)
				{
					NameValuePair attrib = attribs[j];
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
			foreach (CookieAttributeHandler handler in GetAttribHandlers())
			{
				handler.Validate(cookie, origin);
			}
		}

		public override bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			foreach (CookieAttributeHandler handler in GetAttribHandlers())
			{
				if (!handler.Match(cookie, origin))
				{
					return false;
				}
			}
			return true;
		}
	}
}
