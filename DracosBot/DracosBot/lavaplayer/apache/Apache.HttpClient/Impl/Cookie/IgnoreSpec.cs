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
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>CookieSpec that ignores all cookies</summary>
	/// <since>4.1</since>
	public class IgnoreSpec : CookieSpecBase
	{
		// superclass is @NotThreadSafe
		public override int GetVersion()
		{
			return 0;
		}

		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public override IList<Apache.Http.Cookie.Cookie> Parse(Header header, CookieOrigin
			 origin)
		{
			return Sharpen.Collections.EmptyList();
		}

		public override IList<Header> FormatCookies(IList<Apache.Http.Cookie.Cookie> cookies
			)
		{
			return Sharpen.Collections.EmptyList();
		}

		public override Header GetVersionHeader()
		{
			return null;
		}
	}
}
