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

using System.IO;
using Apache.Http.Client;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// A handler for determining if an HttpRequest should be retried after a
	/// recoverable exception during execution.
	/// </summary>
	/// <remarks>
	/// A handler for determining if an HttpRequest should be retried after a
	/// recoverable exception during execution.
	/// <p>
	/// Implementations of this interface must be thread-safe. Access to shared
	/// data must be synchronized as methods of this interface may be executed
	/// from multiple threads.
	/// </remarks>
	/// <since>4.0</since>
	public interface HttpRequestRetryHandler
	{
		/// <summary>
		/// Determines if a method should be retried after an IOException
		/// occurs during execution.
		/// </summary>
		/// <remarks>
		/// Determines if a method should be retried after an IOException
		/// occurs during execution.
		/// </remarks>
		/// <param name="exception">the exception that occurred</param>
		/// <param name="executionCount">
		/// the number of times this method has been
		/// unsuccessfully executed
		/// </param>
		/// <param name="context">the context for the request execution</param>
		/// <returns>
		/// <code>true</code> if the method should be retried, <code>false</code>
		/// otherwise
		/// </returns>
		bool RetryRequest(IOException exception, int executionCount, HttpContext context);
	}
}
