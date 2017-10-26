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

using System.Text;
using Apache.Http.Impl.Client;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Collection of different counters used to gather metrics for
	/// <see cref="FutureRequestExecutionService">FutureRequestExecutionService</see>
	/// .
	/// </summary>
	public sealed class FutureRequestExecutionMetrics
	{
		private readonly AtomicLong activeConnections = new AtomicLong();

		private readonly AtomicLong scheduledConnections = new AtomicLong();

		private readonly FutureRequestExecutionMetrics.DurationCounter successfulConnections
			 = new FutureRequestExecutionMetrics.DurationCounter();

		private readonly FutureRequestExecutionMetrics.DurationCounter failedConnections = 
			new FutureRequestExecutionMetrics.DurationCounter();

		private readonly FutureRequestExecutionMetrics.DurationCounter requests = new FutureRequestExecutionMetrics.DurationCounter
			();

		private readonly FutureRequestExecutionMetrics.DurationCounter tasks = new FutureRequestExecutionMetrics.DurationCounter
			();

		internal FutureRequestExecutionMetrics()
		{
		}

		internal AtomicLong GetActiveConnections()
		{
			return activeConnections;
		}

		internal AtomicLong GetScheduledConnections()
		{
			return scheduledConnections;
		}

		internal FutureRequestExecutionMetrics.DurationCounter GetSuccessfulConnections()
		{
			return successfulConnections;
		}

		internal FutureRequestExecutionMetrics.DurationCounter GetFailedConnections()
		{
			return failedConnections;
		}

		internal FutureRequestExecutionMetrics.DurationCounter GetRequests()
		{
			return requests;
		}

		internal FutureRequestExecutionMetrics.DurationCounter GetTasks()
		{
			return tasks;
		}

		public long GetActiveConnectionCount()
		{
			return activeConnections.Get();
		}

		public long GetScheduledConnectionCount()
		{
			return scheduledConnections.Get();
		}

		public long GetSuccessfulConnectionCount()
		{
			return successfulConnections.Count();
		}

		public long GetSuccessfulConnectionAverageDuration()
		{
			return successfulConnections.AverageDuration();
		}

		public long GetFailedConnectionCount()
		{
			return failedConnections.Count();
		}

		public long GetFailedConnectionAverageDuration()
		{
			return failedConnections.AverageDuration();
		}

		public long GetRequestCount()
		{
			return requests.Count();
		}

		public long GetRequestAverageDuration()
		{
			return requests.AverageDuration();
		}

		public long GetTaskCount()
		{
			return tasks.Count();
		}

		public long GetTaskAverageDuration()
		{
			return tasks.AverageDuration();
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("[activeConnections=").Append(activeConnections).Append(", scheduledConnections="
				).Append(scheduledConnections).Append(", successfulConnections=").Append(successfulConnections
				).Append(", failedConnections=").Append(failedConnections).Append(", requests=")
				.Append(requests).Append(", tasks=").Append(tasks).Append("]");
			return builder.ToString();
		}

		/// <summary>A counter that can measure duration and number of events.</summary>
		/// <remarks>A counter that can measure duration and number of events.</remarks>
		internal class DurationCounter
		{
			private readonly AtomicLong count = new AtomicLong(0);

			private readonly AtomicLong cumulativeDuration = new AtomicLong(0);

			public virtual void Increment(long startTime)
			{
				count.IncrementAndGet();
				cumulativeDuration.AddAndGet(Runtime.CurrentTimeMillis() - startTime);
			}

			public virtual long Count()
			{
				return count.Get();
			}

			public virtual long AverageDuration()
			{
				long counter = count.Get();
				return counter > 0 ? cumulativeDuration.Get() / counter : 0;
			}

			public override string ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("[count=").Append(Count()).Append(", averageDuration=").Append(AverageDuration
					()).Append("]");
				return builder.ToString();
			}
		}
	}
}
