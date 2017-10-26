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

using Apache.Http.Conn;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>Interface for releasing a connection.</summary>
	/// <remarks>
	/// Interface for releasing a connection. This can be implemented by various
	/// "trigger" objects which are associated with a connection, for example
	/// a
	/// <see cref="EofSensorInputStream">stream</see>
	/// or an
	/// <see cref="BasicManagedEntity">entity</see>
	/// or the
	/// <see cref="ManagedClientConnection">connection</see>
	/// itself.
	/// <p>
	/// The methods in this interface can safely be called multiple times.
	/// The first invocation releases the connection, subsequent calls
	/// are ignored.
	/// </remarks>
	/// <since>4.0</since>
	public interface ConnectionReleaseTrigger
	{
		/// <summary>Releases the connection with the option of keep-alive.</summary>
		/// <remarks>
		/// Releases the connection with the option of keep-alive. This is a
		/// "graceful" release and may cause IO operations for consuming the
		/// remainder of a response entity. Use
		/// <see cref="AbortConnection()">abortConnection</see>
		/// for a hard release. The
		/// connection may be reused as specified by the duration.
		/// </remarks>
		/// <exception cref="System.IO.IOException">
		/// in case of an IO problem. The connection will be released
		/// anyway.
		/// </exception>
		void ReleaseConnection();

		/// <summary>Releases the connection without the option of keep-alive.</summary>
		/// <remarks>
		/// Releases the connection without the option of keep-alive.
		/// This is a "hard" release that implies a shutdown of the connection.
		/// Use
		/// <see cref="ReleaseConnection()">ReleaseConnection()</see>
		/// for a graceful release.
		/// </remarks>
		/// <exception cref="System.IO.IOException">
		/// in case of an IO problem.
		/// The connection will be released anyway.
		/// </exception>
		void AbortConnection();
	}
}
