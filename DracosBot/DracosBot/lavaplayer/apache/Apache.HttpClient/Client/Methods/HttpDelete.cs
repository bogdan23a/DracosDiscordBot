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

using Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Client.Methods
{
	/// <summary>
	/// HTTP DELETE method
	/// <p>
	/// The HTTP DELETE method is defined in section 9.7 of
	/// <a href="http://www.ietf.org/rfc/rfc2616.txt">RFC2616</a>:
	/// <blockquote>
	/// The DELETE method requests that the origin server delete the resource
	/// identified by the Request-URI.
	/// </summary>
	/// <remarks>
	/// HTTP DELETE method
	/// <p>
	/// The HTTP DELETE method is defined in section 9.7 of
	/// <a href="http://www.ietf.org/rfc/rfc2616.txt">RFC2616</a>:
	/// <blockquote>
	/// The DELETE method requests that the origin server delete the resource
	/// identified by the Request-URI. [...] The client cannot
	/// be guaranteed that the operation has been carried out, even if the
	/// status code returned from the origin server indicates that the action
	/// has been completed successfully.
	/// </blockquote>
	/// </remarks>
	/// <since>4.0</since>
	public class HttpDelete : HttpRequestBase
	{
		public const string MethodName = "DELETE";

		public HttpDelete() : base()
		{
		}

		public HttpDelete(URI uri) : base()
		{
			// HttpRequestBase is @NotThreadSafe
			SetURI(uri);
		}

		/// <exception cref="System.ArgumentException">if the uri is invalid.</exception>
		public HttpDelete(string uri) : base()
		{
			SetURI(URI.Create(uri));
		}

		public override string GetMethod()
		{
			return MethodName;
		}
	}
}
