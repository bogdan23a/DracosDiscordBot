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

namespace Apache.Http.Cookie
{
	/// <summary>
	/// This interface represents a cookie attribute handler responsible
	/// for parsing, validating, and matching a specific cookie attribute,
	/// such as path, domain, port, etc.
	/// </summary>
	/// <remarks>
	/// This interface represents a cookie attribute handler responsible
	/// for parsing, validating, and matching a specific cookie attribute,
	/// such as path, domain, port, etc.
	/// Different cookie specifications can provide a specific
	/// implementation for this class based on their cookie handling
	/// rules.
	/// </remarks>
	/// <since>4.0</since>
	public interface CookieAttributeHandler
	{
		/// <summary>
		/// Parse the given cookie attribute value and update the corresponding
		/// <see cref="Cookie">Cookie</see>
		/// property.
		/// </summary>
		/// <param name="cookie">
		/// 
		/// <see cref="Cookie">Cookie</see>
		/// to be updated
		/// </param>
		/// <param name="value">cookie attribute value from the cookie response header</param>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		void Parse(SetCookie cookie, string value);

		/// <summary>Peforms cookie validation for the given attribute value.</summary>
		/// <remarks>Peforms cookie validation for the given attribute value.</remarks>
		/// <param name="cookie">
		/// 
		/// <see cref="Cookie">Cookie</see>
		/// to validate
		/// </param>
		/// <param name="origin">the cookie source to validate against</param>
		/// <exception cref="MalformedCookieException">if cookie validation fails for this attribute
		/// 	</exception>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin);

		/// <summary>
		/// Matches the given value (property of the destination host where request is being
		/// submitted) with the corresponding cookie attribute.
		/// </summary>
		/// <remarks>
		/// Matches the given value (property of the destination host where request is being
		/// submitted) with the corresponding cookie attribute.
		/// </remarks>
		/// <param name="cookie">
		/// 
		/// <see cref="Cookie">Cookie</see>
		/// to match
		/// </param>
		/// <param name="origin">the cookie source to match against</param>
		/// <returns><tt>true</tt> if the match is successful; <tt>false</tt> otherwise</returns>
		bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin);
	}
}
