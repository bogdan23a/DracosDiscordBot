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

using Apache.Http.Client;
using Apache.Http.Conn.Routing;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// Represents a controller that dynamically adjusts the size
	/// of an available connection pool based on feedback from
	/// using the connections.
	/// </summary>
	/// <remarks>
	/// Represents a controller that dynamically adjusts the size
	/// of an available connection pool based on feedback from
	/// using the connections.
	/// </remarks>
	/// <since>4.2</since>
	public interface BackoffManager
	{
		/// <summary>
		/// Called when we have decided that the result of
		/// using a connection should be interpreted as a
		/// backoff signal.
		/// </summary>
		/// <remarks>
		/// Called when we have decided that the result of
		/// using a connection should be interpreted as a
		/// backoff signal.
		/// </remarks>
		void BackOff(HttpRoute route);

		/// <summary>
		/// Called when we have determined that the result of
		/// using a connection has succeeded and that we may
		/// probe for more connections.
		/// </summary>
		/// <remarks>
		/// Called when we have determined that the result of
		/// using a connection has succeeded and that we may
		/// probe for more connections.
		/// </remarks>
		void Probe(HttpRoute route);
	}
}
