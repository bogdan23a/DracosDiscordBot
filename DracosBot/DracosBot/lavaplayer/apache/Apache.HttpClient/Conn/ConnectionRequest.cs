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
using Apache.Http.Concurrent;
using Apache.Http.Conn;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>
	/// Represents a request for a
	/// <see cref="Apache.Http.HttpClientConnection">Apache.Http.HttpClientConnection</see>
	/// whose life cycle
	/// is managed by a connection manager.
	/// </summary>
	/// <since>4.3</since>
	public interface ConnectionRequest : Cancellable
	{
		/// <summary>Obtains a connection within a given time.</summary>
		/// <remarks>
		/// Obtains a connection within a given time.
		/// This method will block until a connection becomes available,
		/// the timeout expires, or the connection manager is shut down.
		/// Timeouts are handled with millisecond precision.
		/// If
		/// <see cref="Apache.Http.Concurrent.Cancellable.Cancel()">Apache.Http.Concurrent.Cancellable.Cancel()
		/// 	</see>
		/// is called while this is blocking or
		/// before this began, an
		/// <see cref="System.Exception">System.Exception</see>
		/// will
		/// be thrown.
		/// </remarks>
		/// <param name="timeout">the timeout, 0 or negative for no timeout</param>
		/// <param name="tunit">
		/// the unit for the <code>timeout</code>,
		/// may be <code>null</code> only if there is no timeout
		/// </param>
		/// <returns>
		/// a connection that can be used to communicate
		/// along the given route
		/// </returns>
		/// <exception cref="ConnectionPoolTimeoutException">in case of a timeout</exception>
		/// <exception cref="System.Exception">if the calling thread is interrupted while waiting
		/// 	</exception>
		/// <exception cref="Sharpen.ExecutionException"></exception>
		/// <exception cref="Apache.Http.Conn.ConnectionPoolTimeoutException"></exception>
		HttpClientConnection Get(long timeout, TimeUnit tunit);
	}
}
