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
using System.Collections.Generic;
using Apache.Http.Client;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// This interface represents an abstract store for
	/// <see cref="Apache.Http.Cookie.Cookie">Apache.Http.Cookie.Cookie</see>
	/// objects.
	/// </summary>
	/// <since>4.0</since>
	public interface CookieStore
	{
		/// <summary>
		/// Adds an
		/// <see cref="Apache.Http.Cookie.Cookie">Apache.Http.Cookie.Cookie</see>
		/// , replacing any existing equivalent cookies.
		/// If the given cookie has already expired it will not be added, but existing
		/// values will still be removed.
		/// </summary>
		/// <param name="cookie">
		/// the
		/// <see cref="Apache.Http.Cookie.Cookie">cookie</see>
		/// to be added
		/// </param>
		void AddCookie(Apache.Http.Cookie.Cookie cookie);

		/// <summary>Returns all cookies contained in this store.</summary>
		/// <remarks>Returns all cookies contained in this store.</remarks>
		/// <returns>all cookies</returns>
		IList<Apache.Http.Cookie.Cookie> GetCookies();

		/// <summary>
		/// Removes all of
		/// <see cref="Apache.Http.Cookie.Cookie">Apache.Http.Cookie.Cookie</see>
		/// s in this store that have expired by
		/// the specified
		/// <see cref="System.DateTime">System.DateTime</see>
		/// .
		/// </summary>
		/// <returns>true if any cookies were purged.</returns>
		bool ClearExpired(DateTime date);

		/// <summary>Clears all cookies.</summary>
		/// <remarks>Clears all cookies.</remarks>
		void Clear();
	}
}
