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

using Apache.Http;
using Apache.Http.Client;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Default implementation of the
	/// <see cref="Apache.Http.Client.ServiceUnavailableRetryStrategy">Apache.Http.Client.ServiceUnavailableRetryStrategy
	/// 	</see>
	/// interface.
	/// that retries <code>503</code> (Service Unavailable) responses for a fixed number of times
	/// at a fixed interval.
	/// </summary>
	/// <since>4.2</since>
	public class DefaultServiceUnavailableRetryStrategy : ServiceUnavailableRetryStrategy
	{
		/// <summary>
		/// Maximum number of allowed retries if the server responds with a HTTP code
		/// in our retry code list.
		/// </summary>
		/// <remarks>
		/// Maximum number of allowed retries if the server responds with a HTTP code
		/// in our retry code list. Default value is 1.
		/// </remarks>
		private readonly int maxRetries;

		/// <summary>Retry interval between subsequent requests, in milliseconds.</summary>
		/// <remarks>
		/// Retry interval between subsequent requests, in milliseconds. Default
		/// value is 1 second.
		/// </remarks>
		private readonly long retryInterval;

		public DefaultServiceUnavailableRetryStrategy(int maxRetries, int retryInterval) : 
			base()
		{
			Args.Positive(maxRetries, "Max retries");
			Args.Positive(retryInterval, "Retry interval");
			this.maxRetries = maxRetries;
			this.retryInterval = retryInterval;
		}

		public DefaultServiceUnavailableRetryStrategy() : this(1, 1000)
		{
		}

		public virtual bool RetryRequest(HttpResponse response, int executionCount, HttpContext
			 context)
		{
			return executionCount <= maxRetries && response.GetStatusLine().GetStatusCode() ==
				 HttpStatus.ScServiceUnavailable;
		}

		public virtual long GetRetryInterval()
		{
			return retryInterval;
		}
	}
}
