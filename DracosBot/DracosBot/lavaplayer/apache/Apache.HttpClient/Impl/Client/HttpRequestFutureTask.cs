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
using Apache.Http.Impl.Client;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// FutureTask implementation that wraps a HttpAsyncClientCallable and exposes various task
	/// specific metrics.
	/// </summary>
	/// <remarks>
	/// FutureTask implementation that wraps a HttpAsyncClientCallable and exposes various task
	/// specific metrics.
	/// </remarks>
	/// <?></?>
	public class HttpRequestFutureTask<V> : FutureTask<V>
	{
		private readonly IHttpUriRequest request;

		private readonly HttpRequestTaskCallable<V> callable;

		public HttpRequestFutureTask(IHttpUriRequest request, HttpRequestTaskCallable<V> 
			httpCallable) : base(httpCallable)
		{
			this.request = request;
			this.callable = httpCallable;
		}

		public override bool Cancel(bool mayInterruptIfRunning)
		{
			callable.Cancel();
			if (mayInterruptIfRunning)
			{
				request.Abort();
			}
			return base.Cancel(mayInterruptIfRunning);
		}

		/// <returns>the time in millis the task was scheduled.</returns>
		public virtual long ScheduledTime()
		{
			return callable.GetScheduled();
		}

		/// <returns>the time in millis the task was started.</returns>
		public virtual long StartedTime()
		{
			return callable.GetStarted();
		}

		/// <returns>the time in millis the task was finished/cancelled.</returns>
		public virtual long EndedTime()
		{
			if (IsDone())
			{
				return callable.GetEnded();
			}
			else
			{
				throw new InvalidOperationException("Task is not done yet");
			}
		}

		/// <returns>
		/// the time in millis it took to make the request (excluding the time it was
		/// scheduled to be executed).
		/// </returns>
		public virtual long RequestDuration()
		{
			if (IsDone())
			{
				return EndedTime() - StartedTime();
			}
			else
			{
				throw new InvalidOperationException("Task is not done yet");
			}
		}

		/// <returns>the time in millis it took to execute the task from the moment it was scheduled.
		/// 	</returns>
		public virtual long TaskDuration()
		{
			if (IsDone())
			{
				return EndedTime() - ScheduledTime();
			}
			else
			{
				throw new InvalidOperationException("Task is not done yet");
			}
		}

		public override string ToString()
		{
			return request.GetRequestLine().GetUri();
		}
	}
}
