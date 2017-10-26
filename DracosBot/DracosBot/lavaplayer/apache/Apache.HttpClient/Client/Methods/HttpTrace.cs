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
	/// <summary>HTTP TRACE method.</summary>
	/// <remarks>
	/// HTTP TRACE method.
	/// <p>
	/// The HTTP TRACE method is defined in section 9.6 of
	/// <a href="http://www.ietf.org/rfc/rfc2616.txt">RFC2616</a>:
	/// <blockquote>
	/// The TRACE method is used to invoke a remote, application-layer loop-
	/// back of the request message. The final recipient of the request
	/// SHOULD reflect the message received back to the client as the
	/// entity-body of a 200 (OK) response. The final recipient is either the
	/// origin server or the first proxy or gateway to receive a Max-Forwards
	/// value of zero (0) in the request (see section 14.31). A TRACE request
	/// MUST NOT include an entity.
	/// </blockquote>
	/// </p>
	/// </remarks>
	/// <since>4.0</since>
	public class HttpTrace : HttpRequestBase
	{
		public const string MethodName = "TRACE";

		public HttpTrace() : base()
		{
		}

		public HttpTrace(URI uri) : base()
		{
			SetURI(uri);
		}

		/// <exception cref="System.ArgumentException">if the uri is invalid.</exception>
		public HttpTrace(string uri) : base()
		{
			SetURI(URI.Create(uri));
		}

		public override string GetMethod()
		{
			return MethodName;
		}
	}
}
