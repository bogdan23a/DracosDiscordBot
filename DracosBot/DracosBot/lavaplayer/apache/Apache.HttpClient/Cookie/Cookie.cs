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
using Sharpen;

namespace Apache.Http.Cookie
{
	/// <summary>
	/// Cookie interface represents a token or short packet of state information
	/// (also referred to as "magic-cookie") that the HTTP agent and the target
	/// server can exchange to maintain a session.
	/// </summary>
	/// <remarks>
	/// Cookie interface represents a token or short packet of state information
	/// (also referred to as "magic-cookie") that the HTTP agent and the target
	/// server can exchange to maintain a session. In its simples form an HTTP
	/// cookie is merely a name / value pair.
	/// </remarks>
	/// <since>4.0</since>
	public interface Cookie
	{
		/// <summary>Returns the name.</summary>
		/// <remarks>Returns the name.</remarks>
		/// <returns>String name The name</returns>
		string GetName();

		/// <summary>Returns the value.</summary>
		/// <remarks>Returns the value.</remarks>
		/// <returns>String value The current value.</returns>
		string GetValue();

		/// <summary>
		/// Returns the comment describing the purpose of this cookie, or
		/// <tt>null</tt> if no such comment has been defined.
		/// </summary>
		/// <remarks>
		/// Returns the comment describing the purpose of this cookie, or
		/// <tt>null</tt> if no such comment has been defined.
		/// </remarks>
		/// <returns>comment</returns>
		string GetComment();

		/// <summary>
		/// If a user agent (web browser) presents this cookie to a user, the
		/// cookie's purpose will be described by the information at this URL.
		/// </summary>
		/// <remarks>
		/// If a user agent (web browser) presents this cookie to a user, the
		/// cookie's purpose will be described by the information at this URL.
		/// </remarks>
		string GetCommentURL();

		/// <summary>
		/// Returns the expiration
		/// <see cref="System.DateTime">System.DateTime</see>
		/// of the cookie, or <tt>null</tt>
		/// if none exists.
		/// <p><strong>Note:</strong> the object returned by this method is
		/// considered immutable. Changing it (e.g. using setTime()) could result
		/// in undefined behaviour. Do so at your peril. </p>
		/// </summary>
		/// <returns>
		/// Expiration
		/// <see cref="System.DateTime">System.DateTime</see>
		/// , or <tt>null</tt>.
		/// </returns>
		DateTime GetExpiryDate();

		/// <summary>
		/// Returns <tt>false</tt> if the cookie should be discarded at the end
		/// of the "session"; <tt>true</tt> otherwise.
		/// </summary>
		/// <remarks>
		/// Returns <tt>false</tt> if the cookie should be discarded at the end
		/// of the "session"; <tt>true</tt> otherwise.
		/// </remarks>
		/// <returns>
		/// <tt>false</tt> if the cookie should be discarded at the end
		/// of the "session"; <tt>true</tt> otherwise
		/// </returns>
		bool IsPersistent();

		/// <summary>Returns domain attribute of the cookie.</summary>
		/// <remarks>
		/// Returns domain attribute of the cookie. The value of the Domain
		/// attribute specifies the domain for which the cookie is valid.
		/// </remarks>
		/// <returns>the value of the domain attribute.</returns>
		string GetDomain();

		/// <summary>Returns the path attribute of the cookie.</summary>
		/// <remarks>
		/// Returns the path attribute of the cookie. The value of the Path
		/// attribute specifies the subset of URLs on the origin server to which
		/// this cookie applies.
		/// </remarks>
		/// <returns>The value of the path attribute.</returns>
		string GetPath();

		/// <summary>Get the Port attribute.</summary>
		/// <remarks>
		/// Get the Port attribute. It restricts the ports to which a cookie
		/// may be returned in a Cookie request header.
		/// </remarks>
		int[] GetPorts();

		/// <summary>Indicates whether this cookie requires a secure connection.</summary>
		/// <remarks>Indicates whether this cookie requires a secure connection.</remarks>
		/// <returns>
		/// <code>true</code> if this cookie should only be sent
		/// over secure connections, <code>false</code> otherwise.
		/// </returns>
		bool IsSecure();

		/// <summary>
		/// Returns the version of the cookie specification to which this
		/// cookie conforms.
		/// </summary>
		/// <remarks>
		/// Returns the version of the cookie specification to which this
		/// cookie conforms.
		/// </remarks>
		/// <returns>the version of the cookie.</returns>
		int GetVersion();

		/// <summary>Returns true if this cookie has expired.</summary>
		/// <remarks>Returns true if this cookie has expired.</remarks>
		/// <param name="date">Current time</param>
		/// <returns><tt>true</tt> if the cookie has expired.</returns>
		bool IsExpired(DateTime date);
	}
}
