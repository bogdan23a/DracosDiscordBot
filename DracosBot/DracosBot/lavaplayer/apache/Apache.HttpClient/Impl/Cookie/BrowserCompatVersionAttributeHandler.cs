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

using System;
using Apache.Http.Cookie;
using Apache.Http.Impl.Cookie;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary><tt>"Version"</tt> cookie attribute handler for BrowserCompat cookie spec.
	/// 	</summary>
	/// <remarks><tt>"Version"</tt> cookie attribute handler for BrowserCompat cookie spec.
	/// 	</remarks>
	/// <since>4.3</since>
	public class BrowserCompatVersionAttributeHandler : AbstractCookieAttributeHandler
	{
		public BrowserCompatVersionAttributeHandler() : base()
		{
		}

		/// <summary>Parse cookie version attribute.</summary>
		/// <remarks>Parse cookie version attribute.</remarks>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public override void Parse(SetCookie cookie, string value)
		{
			Args.NotNull(cookie, "Cookie");
			if (value == null)
			{
				throw new MalformedCookieException("Missing value for version attribute");
			}
			int version = 0;
			try
			{
				version = System.Convert.ToInt32(value);
			}
			catch (FormatException)
			{
			}
			// Just ignore invalid versions
			cookie.SetVersion(version);
		}
	}
}
