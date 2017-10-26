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

using Apache.Http.Client.Protocol;
using Apache.Http.Protocol;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Client.Protocol
{
	/// <summary>Class responsible for handling Content Encoding requests in HTTP.</summary>
	/// <remarks>
	/// Class responsible for handling Content Encoding requests in HTTP.
	/// <p>
	/// Instances of this class are stateless, therefore they're thread-safe and immutable.
	/// </remarks>
	/// <seealso>"http://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.5"</seealso>
	/// <since>4.1</since>
	public class RequestAcceptEncoding : IHttpRequestInterceptor
	{
		/// <summary>
		/// Adds the header
		/// <code>"Accept-Encoding: gzip,deflate"</code>
		/// to the request.
		/// </summary>
		/// <exception cref="Apache.Http.HttpException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Process(IHttpRequest request, HttpContext context)
		{
			if (!request.ContainsHeader("Accept-Encoding"))
			{
				request.AddHeader("Accept-Encoding", "gzip,deflate");
			}
		}
	}
}
