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
	/// <summary>
	/// HttpAsyncClientWithFuture wraps calls to execute with a
	/// <see cref="HttpRequestFutureTask{V}">HttpRequestFutureTask&lt;V&gt;</see>
	/// and schedules them using the provided executor service. Scheduled calls may be cancelled.
	/// </summary>
	public class FutureRequestExecutionService : IDisposable
	{
		private readonly HttpClient httpclient;

		private readonly ExecutorService executorService;

		private readonly FutureRequestExecutionMetrics metrics = new FutureRequestExecutionMetrics
			();

		private readonly AtomicBoolean closed = new AtomicBoolean(false);

		/// <summary>Create a new FutureRequestExecutionService.</summary>
		/// <remarks>Create a new FutureRequestExecutionService.</remarks>
		/// <param name="httpclient">
		/// you should tune your httpclient instance to match your needs. You should
		/// align the max number of connections in the pool and the number of threads
		/// in the executor; it doesn't make sense to have more threads than connections
		/// and if you have less connections than threads, the threads will just end up
		/// blocking on getting a connection from the pool.
		/// </param>
		/// <param name="executorService">
		/// any executorService will do here. E.g.
		/// <see cref="Sharpen.Executors.NewFixedThreadPool(int)">Sharpen.Executors.NewFixedThreadPool(int)
		/// 	</see>
		/// </param>
		public FutureRequestExecutionService(HttpClient httpclient, ExecutorService executorService
			)
		{
			this.httpclient = httpclient;
			this.executorService = executorService;
		}

		/// <summary>Schedule a request for execution.</summary>
		/// <remarks>Schedule a request for execution.</remarks>
		/// <?></?>
		/// <param name="request">request to execute</param>
		/// <param name="responseHandler">handler that will process the response.</param>
		/// <returns>HttpAsyncClientFutureTask for the scheduled request.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual HttpRequestFutureTask<T> Execute<T>(IHttpUriRequest request, HttpContext
			 context, ResponseHandler<T> responseHandler)
		{
			return Execute(request, context, responseHandler, null);
		}

		/// <summary>Schedule a request for execution.</summary>
		/// <remarks>Schedule a request for execution.</remarks>
		/// <?></?>
		/// <param name="request">request to execute</param>
		/// <param name="context">optional context; use null if not needed.</param>
		/// <param name="responseHandler">handler that will process the response.</param>
		/// <param name="callback">
		/// callback handler that will be called when the request is scheduled,
		/// started, completed, failed, or cancelled.
		/// </param>
		/// <returns>HttpAsyncClientFutureTask for the scheduled request.</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		public virtual HttpRequestFutureTask<T> Execute<T>(IHttpUriRequest request, HttpContext
			 context, ResponseHandler<T> responseHandler, FutureCallback<T> callback)
		{
			if (closed.Get())
			{
				throw new InvalidOperationException("Close has been called on this httpclient instance."
					);
			}
			metrics.GetScheduledConnections().IncrementAndGet();
			HttpRequestTaskCallable<T> callable = new HttpRequestTaskCallable<T>(httpclient, 
				request, context, responseHandler, callback, metrics);
			HttpRequestFutureTask<T> httpRequestFutureTask = new HttpRequestFutureTask<T>(request
				, callable);
			executorService.Execute(httpRequestFutureTask);
			return httpRequestFutureTask;
		}

		/// <returns>metrics gathered for this instance.</returns>
		/// <seealso cref="FutureRequestExecutionMetrics">FutureRequestExecutionMetrics</seealso>
		public virtual FutureRequestExecutionMetrics Metrics()
		{
			return metrics;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Close()
		{
			closed.Set(true);
			executorService.ShutdownNow();
			if (httpclient is IDisposable)
			{
				((IDisposable)httpclient).Close();
			}
		}
	}
}
