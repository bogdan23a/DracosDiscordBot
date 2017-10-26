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
	/// <summary>HTTP POST method.</summary>
	/// <remarks>
	/// HTTP POST method.
	/// <p>
	/// The HTTP POST method is defined in section 9.5 of
	/// <a href="http://www.ietf.org/rfc/rfc2616.txt">RFC2616</a>:
	/// <blockquote>
	/// The POST method is used to request that the origin server accept the entity
	/// enclosed in the request as a new subordinate of the resource identified by
	/// the Request-URI in the Request-Line. POST is designed to allow a uniform
	/// method to cover the following functions:
	/// <ul>
	/// <li>Annotation of existing resources</li>
	/// <li>Posting a message to a bulletin board, newsgroup, mailing list, or
	/// similar group of articles</li>
	/// <li>Providing a block of data, such as the result of submitting a form,
	/// to a data-handling process</li>
	/// <li>Extending a database through an append operation</li>
	/// </ul>
	/// </blockquote>
	/// </p>
	/// </remarks>
	/// <since>4.0</since>
	public class HttpPost : HttpEntityEnclosingRequestBase
	{
		public const string MethodName = "POST";

		public HttpPost() : base()
		{
		}

		public HttpPost(URI uri) : base()
		{
			SetURI(uri);
		}

		/// <exception cref="System.ArgumentException">if the uri is invalid.</exception>
		public HttpPost(string uri) : base()
		{
			SetURI(URI.Create(uri));
		}

		public override string GetMethod()
		{
			return MethodName;
		}
	}
}
