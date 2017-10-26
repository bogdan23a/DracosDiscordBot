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
using System.Globalization;
using Apache.Http.Impl.Client;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// <see cref="Apache.Http.Client.HttpRequestRetryHandler">Apache.Http.Client.HttpRequestRetryHandler
	/// 	</see>
	/// which assumes
	/// that all requested HTTP methods which should be idempotent according
	/// to RFC-2616 are in fact idempotent and can be retried.
	/// <p/>
	/// According to RFC-2616 section 9.1.2 the idempotent HTTP methods are:
	/// GET, HEAD, PUT, DELETE, OPTIONS, and TRACE
	/// </summary>
	/// <since>4.2</since>
	public class StandardHttpRequestRetryHandler : DefaultHttpRequestRetryHandler
	{
		private readonly IDictionary<string, bool> idempotentMethods;

		/// <summary>Default constructor</summary>
		public StandardHttpRequestRetryHandler(int retryCount, bool requestSentRetryEnabled
			) : base(retryCount, requestSentRetryEnabled)
		{
			this.idempotentMethods = new ConcurrentHashMap<string, bool>();
			this.idempotentMethods.Put("GET", true);
			this.idempotentMethods.Put("HEAD", true);
			this.idempotentMethods.Put("PUT", true);
			this.idempotentMethods.Put("DELETE", true);
			this.idempotentMethods.Put("OPTIONS", true);
			this.idempotentMethods.Put("TRACE", true);
		}

		/// <summary>Default constructor</summary>
		public StandardHttpRequestRetryHandler() : this(3, false)
		{
		}

		protected internal override bool HandleAsIdempotent(IHttpRequest request)
		{
			string method = request.GetRequestLine().GetMethod().ToUpper(CultureInfo.InvariantCulture
				);
			bool b = this.idempotentMethods.Get(method);
			return b != null && b;
		}
	}
}
