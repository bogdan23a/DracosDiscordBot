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
using Sharpen;

namespace Apache.Http.Cookie
{
	/// <summary>Defines the cookie management specification.</summary>
	/// <remarks>
	/// Defines the cookie management specification.
	/// <p>Cookie management specification must define
	/// <ul>
	/// <li> rules of parsing "Set-Cookie" header
	/// <li> rules of validation of parsed cookies
	/// <li>  formatting of "Cookie" header
	/// </ul>
	/// for a given host, port and path of origin
	/// </remarks>
	/// <since>4.0</since>
	public interface CookieSpec
	{
		/// <summary>
		/// Returns version of the state management this cookie specification
		/// conforms to.
		/// </summary>
		/// <remarks>
		/// Returns version of the state management this cookie specification
		/// conforms to.
		/// </remarks>
		/// <returns>version of the state management specification</returns>
		int GetVersion();

		/// <summary>Parse the <tt>"Set-Cookie"</tt> Header into an array of Cookies.</summary>
		/// <remarks>
		/// Parse the <tt>"Set-Cookie"</tt> Header into an array of Cookies.
		/// <p>This method will not perform the validation of the resultant
		/// <see cref="Cookie">Cookie</see>
		/// s</p>
		/// </remarks>
		/// <seealso cref="Validate(Cookie, CookieOrigin)">Validate(Cookie, CookieOrigin)</seealso>
		/// <param name="header">the <tt>Set-Cookie</tt> received from the server</param>
		/// <param name="origin">details of the cookie origin</param>
		/// <returns>an array of <tt>Cookie</tt>s parsed from the header</returns>
		/// <exception cref="MalformedCookieException">if an exception occurs during parsing</exception>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		IList<Apache.Http.Cookie.Cookie> Parse(Header header, CookieOrigin origin);

		/// <summary>
		/// Validate the cookie according to validation rules defined by the
		/// cookie specification.
		/// </summary>
		/// <remarks>
		/// Validate the cookie according to validation rules defined by the
		/// cookie specification.
		/// </remarks>
		/// <param name="cookie">the Cookie to validate</param>
		/// <param name="origin">details of the cookie origin</param>
		/// <exception cref="MalformedCookieException">if the cookie is invalid</exception>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin);

		/// <summary>Determines if a Cookie matches the target location.</summary>
		/// <remarks>Determines if a Cookie matches the target location.</remarks>
		/// <param name="cookie">the Cookie to be matched</param>
		/// <param name="origin">the target to test against</param>
		/// <returns>
		/// <tt>true</tt> if the cookie should be submitted with a request
		/// with given attributes, <tt>false</tt> otherwise.
		/// </returns>
		bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin);

		/// <summary>Create <tt>"Cookie"</tt> headers for an array of Cookies.</summary>
		/// <remarks>Create <tt>"Cookie"</tt> headers for an array of Cookies.</remarks>
		/// <param name="cookies">the Cookies format into a Cookie header</param>
		/// <returns>a Header for the given Cookies.</returns>
		/// <exception cref="System.ArgumentException">if an input parameter is illegal</exception>
		IList<Header> FormatCookies(IList<Apache.Http.Cookie.Cookie> cookies);

		/// <summary>
		/// Returns a request header identifying what version of the state management
		/// specification is understood.
		/// </summary>
		/// <remarks>
		/// Returns a request header identifying what version of the state management
		/// specification is understood. May be <code>null</code> if the cookie
		/// specification does not support <tt>Cookie2</tt> header.
		/// </remarks>
		Header GetVersionHeader();
	}
}
