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
	/// <summary>HTTP GET method.</summary>
	/// <remarks>
	/// HTTP GET method.
	/// <p>
	/// The HTTP GET method is defined in section 9.3 of
	/// <a href="http://www.ietf.org/rfc/rfc2616.txt">RFC2616</a>:
	/// <blockquote>
	/// The GET method means retrieve whatever information (in the form of an
	/// entity) is identified by the Request-URI. If the Request-URI refers
	/// to a data-producing process, it is the produced data which shall be
	/// returned as the entity in the response and not the source text of the
	/// process, unless that text happens to be the output of the process.
	/// </blockquote>
	/// </p>
	/// </remarks>
	/// <since>4.0</since>
	public class HttpGet : HttpRequestBase
	{
		public const string MethodName = "GET";

		public HttpGet() : base()
		{
		}

		public HttpGet(URI uri) : base()
		{
			SetURI(uri);
		}

		/// <exception cref="System.ArgumentException">if the uri is invalid.</exception>
		public HttpGet(string uri) : base()
		{
			SetURI(URI.Create(uri));
		}

		public override string GetMethod()
		{
			return MethodName;
		}
	}
}
