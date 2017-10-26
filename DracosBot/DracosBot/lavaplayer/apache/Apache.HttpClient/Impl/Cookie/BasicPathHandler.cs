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
	public class BasicPathHandler : CookieAttributeHandler
	{
		public BasicPathHandler() : base()
		{
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Parse(SetCookie cookie, string value)
		{
			Args.NotNull(cookie, "Cookie");
			cookie.SetPath(!TextUtils.IsBlank(value) ? value : "/");
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
			if (!Match(cookie, origin))
			{
				throw new CookieRestrictionViolationException("Illegal path attribute \"" + cookie
					.GetPath() + "\". Path of origin: \"" + origin.GetPath() + "\"");
			}
		}

		public virtual bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			string targetpath = origin.GetPath();
			string topmostPath = cookie.GetPath();
			if (topmostPath == null)
			{
				topmostPath = "/";
			}
			if (topmostPath.Length > 1 && topmostPath.EndsWith("/"))
			{
				topmostPath = Sharpen.Runtime.Substring(topmostPath, 0, topmostPath.Length - 1);
			}
			bool match = targetpath.StartsWith(topmostPath);
			// if there is a match and these values are not exactly the same we have
			// to make sure we're not matcing "/foobar" and "/foo"
			if (match && targetpath.Length != topmostPath.Length)
			{
				if (!topmostPath.EndsWith("/"))
				{
					match = (targetpath[topmostPath.Length] == '/');
				}
			}
			return match;
		}
	}
}
