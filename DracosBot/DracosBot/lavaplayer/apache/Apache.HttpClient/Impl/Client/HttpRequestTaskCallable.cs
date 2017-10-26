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
using Apache.Http.Client;
using Apache.Http.Concurrent;
using Apache.Http.Impl.Client;
using Apache.Http.Protocol;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	internal class HttpRequestTaskCallable<V> : Callable<V>
	{
		private readonly IHttpUriRequest request;

		private readonly HttpClient httpclient;

		private readonly AtomicBoolean cancelled = new AtomicBoolean(false);

		private readonly long scheduled = Runtime.CurrentTimeMillis();

		private long started = -1;

		private long ended = -1;

		private readonly HttpContext context;

		private readonly ResponseHandler<V> responseHandler;

		private readonly FutureCallback<V> callback;

		private readonly FutureRequestExecutionMetrics metrics;

		internal HttpRequestTaskCallable(HttpClient httpClient, IHttpUriRequest request, 
			HttpContext context, ResponseHandler<V> responseHandler, FutureCallback<V> callback
			, FutureRequestExecutionMetrics metrics)
		{
			this.httpclient = httpClient;
			this.responseHandler = responseHandler;
			this.request = request;
			this.context = context;
			this.callback = callback;
			this.metrics = metrics;
		}

		public virtual long GetScheduled()
		{
			return scheduled;
		}

		public virtual long GetStarted()
		{
			return started;
		}

		public virtual long GetEnded()
		{
			return ended;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual V Call()
		{
			if (!cancelled.Get())
			{
				try
				{
					metrics.GetActiveConnections().IncrementAndGet();
					started = Runtime.CurrentTimeMillis();
					try
					{
						metrics.GetScheduledConnections().DecrementAndGet();
						V result = httpclient.Execute(request, responseHandler, context);
						ended = Runtime.CurrentTimeMillis();
						metrics.GetSuccessfulConnections().Increment(started);
						if (callback != null)
						{
							callback.Completed(result);
						}
						return result;
					}
					catch (Exception e)
					{
						metrics.GetFailedConnections().Increment(started);
						ended = Runtime.CurrentTimeMillis();
						if (callback != null)
						{
							callback.Failed(e);
						}
						throw;
					}
				}
				finally
				{
					metrics.GetRequests().Increment(started);
					metrics.GetTasks().Increment(started);
					metrics.GetActiveConnections().DecrementAndGet();
				}
			}
			else
			{
				throw new InvalidOperationException("call has been cancelled for request " + request
					.GetURI());
			}
		}

		public virtual void Cancel()
		{
			cancelled.Set(true);
			if (callback != null)
			{
				callback.Cancelled();
			}
		}
	}
}
