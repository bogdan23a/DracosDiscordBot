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
using Apache.Http.Conn;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>
	/// A watcher for
	/// <see cref="EofSensorInputStream">EofSensorInputStream</see>
	/// . Each stream will notify its
	/// watcher at most once.
	/// </summary>
	/// <since>4.0</since>
	public interface EofSensorWatcher
	{
		/// <summary>Indicates that EOF is detected.</summary>
		/// <remarks>Indicates that EOF is detected.</remarks>
		/// <param name="wrapped">the underlying stream which has reached EOF</param>
		/// <returns>
		/// <code>true</code> if <code>wrapped</code> should be closed,
		/// <code>false</code> if it should be left alone
		/// </returns>
		/// <exception cref="System.IO.IOException">
		/// in case of an IO problem, for example if the watcher itself
		/// closes the underlying stream. The caller will leave the
		/// wrapped stream alone, as if <code>false</code> was returned.
		/// </exception>
		bool EofDetected(InputStream wrapped);

		/// <summary>
		/// Indicates that the
		/// <see cref="EofSensorInputStream">stream</see>
		/// is closed.
		/// This method will be called only if EOF was <i>not</i> detected
		/// before closing. Otherwise,
		/// <see cref="EofDetected(System.IO.InputStream)">eofDetected</see>
		/// is called.
		/// </summary>
		/// <param name="wrapped">the underlying stream which has not reached EOF</param>
		/// <returns>
		/// <code>true</code> if <code>wrapped</code> should be closed,
		/// <code>false</code> if it should be left alone
		/// </returns>
		/// <exception cref="System.IO.IOException">
		/// in case of an IO problem, for example if the watcher itself
		/// closes the underlying stream. The caller will leave the
		/// wrapped stream alone, as if <code>false</code> was returned.
		/// </exception>
		bool StreamClosed(InputStream wrapped);

		/// <summary>
		/// Indicates that the
		/// <see cref="EofSensorInputStream">stream</see>
		/// is aborted.
		/// This method will be called only if EOF was <i>not</i> detected
		/// before aborting. Otherwise,
		/// <see cref="EofDetected(System.IO.InputStream)">eofDetected</see>
		/// is called.
		/// <p/>
		/// This method will also be invoked when an input operation causes an
		/// IOException to be thrown to make sure the input stream gets shut down.
		/// </summary>
		/// <param name="wrapped">the underlying stream which has not reached EOF</param>
		/// <returns>
		/// <code>true</code> if <code>wrapped</code> should be closed,
		/// <code>false</code> if it should be left alone
		/// </returns>
		/// <exception cref="System.IO.IOException">
		/// in case of an IO problem, for example if the watcher itself
		/// closes the underlying stream. The caller will leave the
		/// wrapped stream alone, as if <code>false</code> was returned.
		/// </exception>
		bool StreamAbort(InputStream wrapped);
	}
}
