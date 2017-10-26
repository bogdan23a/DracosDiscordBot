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
	/// <summary>HTTP PATCH method.</summary>
	/// <remarks>
	/// HTTP PATCH method.
	/// <p>
	/// The HTTP PATCH method is defined in &lt;a
	/// href="http://tools.ietf.org/html/rfc5789"&gt;RF5789</a>: <blockquote> The PATCH
	/// method requests that a set of changes described in the request entity be
	/// applied to the resource identified by the Request- URI. Differs from the PUT
	/// method in the way the server processes the enclosed entity to modify the
	/// resource identified by the Request-URI. In a PUT request, the enclosed entity
	/// origin server, and the client is requesting that the stored version be
	/// replaced. With PATCH, however, the enclosed entity contains a set of
	/// instructions describing how a resource currently residing on the origin
	/// server should be modified to produce a new version. </blockquote>
	/// </p>
	/// </remarks>
	/// <since>4.2</since>
	public class HttpPatch : HttpEntityEnclosingRequestBase
	{
		public const string MethodName = "PATCH";

		public HttpPatch() : base()
		{
		}

		public HttpPatch(URI uri) : base()
		{
			SetURI(uri);
		}

		public HttpPatch(string uri) : base()
		{
			SetURI(URI.Create(uri));
		}

		public override string GetMethod()
		{
			return MethodName;
		}
	}
}
