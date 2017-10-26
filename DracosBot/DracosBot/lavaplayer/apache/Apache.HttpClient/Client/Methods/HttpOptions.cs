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

using System.Collections.Generic;
using Apache.Http;
using Apache.Http.Client.Methods;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Client.Methods
{
	/// <summary>HTTP OPTIONS method.</summary>
	/// <remarks>
	/// HTTP OPTIONS method.
	/// <p>
	/// The HTTP OPTIONS method is defined in section 9.2 of
	/// <a href="http://www.ietf.org/rfc/rfc2616.txt">RFC2616</a>:
	/// <blockquote>
	/// The OPTIONS method represents a request for information about the
	/// communication options available on the request/response chain
	/// identified by the Request-URI. This method allows the client to
	/// determine the options and/or requirements associated with a resource,
	/// or the capabilities of a server, without implying a resource action
	/// or initiating a resource retrieval.
	/// </blockquote>
	/// </p>
	/// </remarks>
	/// <since>4.0</since>
	public class HttpOptions : HttpRequestBase
	{
		public const string MethodName = "OPTIONS";

		public HttpOptions() : base()
		{
		}

		public HttpOptions(URI uri) : base()
		{
			SetURI(uri);
		}

		/// <exception cref="System.ArgumentException">if the uri is invalid.</exception>
		public HttpOptions(string uri) : base()
		{
			SetURI(URI.Create(uri));
		}

		public override string GetMethod()
		{
			return MethodName;
		}

		public virtual ICollection<string> GetAllowedMethods(HttpResponse response)
		{
			Args.NotNull(response, "HTTP response");
			HeaderIterator it = response.HeaderIterator("Allow");
			ICollection<string> methods = new HashSet<string>();
			while (it.HasNext())
			{
				Header header = it.NextHeader();
				HeaderElement[] elements = header.GetElements();
				foreach (HeaderElement element in elements)
				{
					methods.AddItem(element.GetName());
				}
			}
			return methods;
		}
	}
}
