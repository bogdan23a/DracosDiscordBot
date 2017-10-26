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

using Apache.Http.Cookie.Params;
using Sharpen;

namespace Apache.Http.Cookie.Params
{
	/// <summary>Parameter names for HTTP cookie management classes.</summary>
	/// <remarks>Parameter names for HTTP cookie management classes.</remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) use constructor parameters of Apache.Http.Cookie.CookieSpecProvider s."
		)]
	public abstract class CookieSpecPNames
	{
		/// <summary>
		/// Defines valid date patterns to be used for parsing non-standard
		/// <code>expires</code> attribute.
		/// </summary>
		/// <remarks>
		/// Defines valid date patterns to be used for parsing non-standard
		/// <code>expires</code> attribute. Only required for compatibility
		/// with non-compliant servers that still use <code>expires</code>
		/// defined in the Netscape draft instead of the standard
		/// <code>max-age</code> attribute.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="System.Collections.ICollection{E}">System.Collections.ICollection&lt;E&gt;
		/// 	</see>
		/// .
		/// The collection elements must be of type
		/// <see cref="string">string</see>
		/// compatible
		/// with the syntax of
		/// <see cref="Sharpen.SimpleDateFormat">Sharpen.SimpleDateFormat</see>
		/// .
		/// </p>
		/// </remarks>
		public const string DatePatterns = "http.protocol.cookie-datepatterns";

		/// <summary>
		/// Defines whether cookies should be forced into a single
		/// <code>Cookie</code> request header.
		/// </summary>
		/// <remarks>
		/// Defines whether cookies should be forced into a single
		/// <code>Cookie</code> request header. Otherwise, each cookie is formatted
		/// as a separate <code>Cookie</code> header.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="bool">bool</see>
		/// .
		/// </p>
		/// </remarks>
		public const string SingleCookieHeader = "http.protocol.single-cookie-header";
	}
}
