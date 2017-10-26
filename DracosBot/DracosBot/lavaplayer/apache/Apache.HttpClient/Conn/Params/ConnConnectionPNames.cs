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
using Apache.Http.Conn.Params;
using Sharpen;

namespace Apache.Http.Conn.Params
{
	/// <summary>Parameter names for HTTP client connections.</summary>
	/// <remarks>Parameter names for HTTP client connections.</remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.1) use custom Apache.Http.Impl.Conn.DefaultHttpResponseParser implementation."
		)]
	public abstract class ConnConnectionPNames
	{
		/// <summary>
		/// Defines the maximum number of ignorable lines before we expect
		/// a HTTP response's status line.
		/// </summary>
		/// <remarks>
		/// Defines the maximum number of ignorable lines before we expect
		/// a HTTP response's status line.
		/// <p>
		/// With HTTP/1.1 persistent connections, the problem arises that
		/// broken scripts could return a wrong Content-Length
		/// (there are more bytes sent than specified).
		/// Unfortunately, in some cases, this cannot be detected after the
		/// bad response, but only before the next one.
		/// So HttpClient must be able to skip those surplus lines this way.
		/// </p>
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="int">int</see>
		/// .
		/// 0 disallows all garbage/empty lines before the status line.
		/// Use
		/// <see cref="int.MaxValue">int.MaxValue</see>
		/// for unlimited number.
		/// </p>
		/// </remarks>
		[System.ObsoleteAttribute(@"(4.1) Use custom Apache.Http.Impl.Conn.DefaultHttpResponseParser implementation"
			)]
		[Obsolete]
		public const string MaxStatusLineGarbage = "http.connection.max-status-line-garbage";
	}
}
