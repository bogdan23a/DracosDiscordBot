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
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary><tt>"Discard"</tt> cookie attribute handler for RFC 2965 cookie spec.</summary>
	/// <remarks><tt>"Discard"</tt> cookie attribute handler for RFC 2965 cookie spec.</remarks>
	/// <since>4.0</since>
	public class RFC2965DiscardAttributeHandler : CookieAttributeHandler
	{
		public RFC2965DiscardAttributeHandler() : base()
		{
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Parse(SetCookie cookie, string commenturl)
		{
			if (cookie is SetCookie2)
			{
				SetCookie2 cookie2 = (SetCookie2)cookie;
				cookie2.SetDiscard(true);
			}
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
		}

		public virtual bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			return true;
		}
	}
}
