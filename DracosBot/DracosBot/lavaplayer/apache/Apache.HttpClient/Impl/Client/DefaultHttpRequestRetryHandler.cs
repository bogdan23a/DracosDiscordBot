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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Apache.Http;
using Apache.Http.Client;
using Apache.Http.Client.Protocol;
using Apache.Http.Impl.Client;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// The default
	/// <see cref="Apache.Http.Client.HttpRequestRetryHandler">Apache.Http.Client.HttpRequestRetryHandler
	/// 	</see>
	/// used by request executors.
	/// </summary>
	/// <since>4.0</since>
	public class DefaultHttpRequestRetryHandler : HttpRequestRetryHandler
	{
		public static readonly Apache.Http.Impl.Client.DefaultHttpRequestRetryHandler Instance
			 = new Apache.Http.Impl.Client.DefaultHttpRequestRetryHandler();

		/// <summary>the number of times a method will be retried</summary>
		private readonly int retryCount;

		/// <summary>Whether or not methods that have successfully sent their request will be retried
		/// 	</summary>
		private readonly bool requestSentRetryEnabled;

		private readonly ICollection<Type> nonRetriableClasses;

		/// <summary>Create the request retry handler using the specified IOException classes
		/// 	</summary>
		/// <param name="retryCount">how many times to retry; 0 means no retries</param>
		/// <param name="requestSentRetryEnabled">true if it's OK to retry requests that have been sent
		/// 	</param>
		/// <param name="clazzes">the IOException types that should not be retried</param>
		/// <since>4.3</since>
		protected internal DefaultHttpRequestRetryHandler(int retryCount, bool requestSentRetryEnabled
			, ICollection<Type> clazzes) : base()
		{
			this.retryCount = retryCount;
			this.requestSentRetryEnabled = requestSentRetryEnabled;
			this.nonRetriableClasses = new HashSet<Type>();
			foreach (Type clazz in clazzes)
			{
				this.nonRetriableClasses.AddItem(clazz);
			}
		}

		/// <summary>
		/// Create the request retry handler using the following list of
		/// non-retriable IOException classes: <br />
		/// <ul>
		/// <li>InterruptedIOException</li>
		/// <li>UnknownHostException</li>
		/// <li>ConnectException</li>
		/// <li>SSLException</li>
		/// </ul>
		/// </summary>
		/// <param name="retryCount">how many times to retry; 0 means no retries</param>
		/// <param name="requestSentRetryEnabled">true if it's OK to retry requests that have been sent
		/// 	</param>
		public DefaultHttpRequestRetryHandler(int retryCount, bool requestSentRetryEnabled
			) : this(retryCount, requestSentRetryEnabled, Arrays.AsList(typeof(ThreadInterruptedException
			), typeof(UnknownHostException), typeof(ConnectException), typeof(SSLException))
			)
		{
		}

		/// <summary>
		/// Create the request retry handler with a retry count of 3, requestSentRetryEnabled false
		/// and using the following list of non-retriable IOException classes: <br />
		/// <ul>
		/// <li>InterruptedIOException</li>
		/// <li>UnknownHostException</li>
		/// <li>ConnectException</li>
		/// <li>SSLException</li>
		/// </ul>
		/// </summary>
		public DefaultHttpRequestRetryHandler() : this(3, false)
		{
		}

		/// <summary>
		/// Used <code>retryCount</code> and <code>requestSentRetryEnabled</code> to determine
		/// if the given method should be retried.
		/// </summary>
		/// <remarks>
		/// Used <code>retryCount</code> and <code>requestSentRetryEnabled</code> to determine
		/// if the given method should be retried.
		/// </remarks>
		public virtual bool RetryRequest(IOException exception, int executionCount, HttpContext
			 context)
		{
			Args.NotNull(exception, "Exception parameter");
			Args.NotNull(context, "HTTP context");
			if (executionCount > this.retryCount)
			{
				// Do not retry if over max retry count
				return false;
			}
			if (this.nonRetriableClasses.Contains(exception.GetType()))
			{
				return false;
			}
			else
			{
				foreach (Type rejectException in this.nonRetriableClasses)
				{
					if (rejectException.IsInstanceOfType(exception))
					{
						return false;
					}
				}
			}
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			IHttpRequest request = clientContext.GetRequest();
			if (RequestIsAborted(request))
			{
				return false;
			}
			if (HandleAsIdempotent(request))
			{
				// Retry if the request is considered idempotent
				return true;
			}
			if (!clientContext.IsRequestSent() || this.requestSentRetryEnabled)
			{
				// Retry if the request has not been sent fully or
				// if it's OK to retry methods that have been sent
				return true;
			}
			// otherwise do not retry
			return false;
		}

		/// <returns>
		/// <code>true</code> if this handler will retry methods that have
		/// successfully sent their request, <code>false</code> otherwise
		/// </returns>
		public virtual bool IsRequestSentRetryEnabled()
		{
			return requestSentRetryEnabled;
		}

		/// <returns>the maximum number of times a method will be retried</returns>
		public virtual int GetRetryCount()
		{
			return retryCount;
		}

		/// <since>4.2</since>
		protected internal virtual bool HandleAsIdempotent(IHttpRequest request)
		{
			return !(request is HttpEntityEnclosingRequest);
		}

		/// <since>4.2</since>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3)")]
		protected internal virtual bool RequestIsAborted(IHttpRequest request)
		{
			IHttpRequest req = request;
			if (request is RequestWrapper)
			{
				// does not forward request to original
				req = ((RequestWrapper)request).GetOriginal();
			}
			return (req is IHttpUriRequest && ((IHttpUriRequest)req).IsAborted());
		}
	}
}
