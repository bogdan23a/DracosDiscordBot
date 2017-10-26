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
using Sharpen;

namespace Apache.Http.Cookie
{
	/// <summary>
	/// This interface represents a <code>Set-Cookie</code> response header sent by the
	/// origin server to the HTTP agent in order to maintain a conversational state.
	/// </summary>
	/// <remarks>
	/// This interface represents a <code>Set-Cookie</code> response header sent by the
	/// origin server to the HTTP agent in order to maintain a conversational state.
	/// </remarks>
	/// <since>4.0</since>
	public interface SetCookie : Apache.Http.Cookie.Cookie
	{
		void SetValue(string value);

		/// <summary>
		/// If a user agent (web browser) presents this cookie to a user, the
		/// cookie's purpose will be described using this comment.
		/// </summary>
		/// <remarks>
		/// If a user agent (web browser) presents this cookie to a user, the
		/// cookie's purpose will be described using this comment.
		/// </remarks>
		/// <param name="comment"></param>
		/// <seealso cref="Cookie.GetComment()">Cookie.GetComment()</seealso>
		void SetComment(string comment);

		/// <summary>Sets expiration date.</summary>
		/// <remarks>
		/// Sets expiration date.
		/// <p><strong>Note:</strong> the object returned by this method is considered
		/// immutable. Changing it (e.g. using setTime()) could result in undefined
		/// behaviour. Do so at your peril.</p>
		/// </remarks>
		/// <param name="expiryDate">
		/// the
		/// <see cref="System.DateTime">System.DateTime</see>
		/// after which this cookie is no longer valid.
		/// </param>
		/// <seealso cref="Cookie.GetExpiryDate()">Cookie.GetExpiryDate()</seealso>
		void SetExpiryDate(DateTime expiryDate);

		/// <summary>Sets the domain attribute.</summary>
		/// <remarks>Sets the domain attribute.</remarks>
		/// <param name="domain">The value of the domain attribute</param>
		/// <seealso cref="Cookie.GetDomain()">Cookie.GetDomain()</seealso>
		void SetDomain(string domain);

		/// <summary>Sets the path attribute.</summary>
		/// <remarks>Sets the path attribute.</remarks>
		/// <param name="path">The value of the path attribute</param>
		/// <seealso cref="Cookie.GetPath()">Cookie.GetPath()</seealso>
		void SetPath(string path);

		/// <summary>Sets the secure attribute of the cookie.</summary>
		/// <remarks>
		/// Sets the secure attribute of the cookie.
		/// <p>
		/// When <tt>true</tt> the cookie should only be sent
		/// using a secure protocol (https).  This should only be set when
		/// the cookie's originating server used a secure protocol to set the
		/// cookie's value.
		/// </remarks>
		/// <param name="secure">The value of the secure attribute</param>
		/// <seealso cref="Cookie.IsSecure()">Cookie.IsSecure()</seealso>
		void SetSecure(bool secure);

		/// <summary>
		/// Sets the version of the cookie specification to which this
		/// cookie conforms.
		/// </summary>
		/// <remarks>
		/// Sets the version of the cookie specification to which this
		/// cookie conforms.
		/// </remarks>
		/// <param name="version">the version of the cookie.</param>
		/// <seealso cref="Cookie.GetVersion()">Cookie.GetVersion()</seealso>
		void SetVersion(int version);
	}
}
