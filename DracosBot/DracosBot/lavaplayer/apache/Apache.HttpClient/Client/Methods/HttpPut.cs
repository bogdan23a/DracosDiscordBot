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
	/// <summary>HTTP PUT method.</summary>
	/// <remarks>
	/// HTTP PUT method.
	/// <p>
	/// The HTTP PUT method is defined in section 9.6 of
	/// <a href="http://www.ietf.org/rfc/rfc2616.txt">RFC2616</a>:
	/// <blockquote>
	/// The PUT method requests that the enclosed entity be stored under the
	/// supplied Request-URI. If the Request-URI refers to an already
	/// existing resource, the enclosed entity SHOULD be considered as a
	/// modified version of the one residing on the origin server.
	/// </blockquote>
	/// </p>
	/// </remarks>
	/// <since>4.0</since>
	public class HttpPut : HttpEntityEnclosingRequestBase
	{
		public const string MethodName = "PUT";

		public HttpPut() : base()
		{
		}

		public HttpPut(URI uri) : base()
		{
			SetURI(uri);
		}

		/// <exception cref="System.ArgumentException">if the uri is invalid.</exception>
		public HttpPut(string uri) : base()
		{
			SetURI(URI.Create(uri));
		}

		public override string GetMethod()
		{
			return MethodName;
		}
	}
}
