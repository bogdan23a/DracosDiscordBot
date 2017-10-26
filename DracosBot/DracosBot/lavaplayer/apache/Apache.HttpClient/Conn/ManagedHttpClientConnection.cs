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

using System.Net.Sockets;
using Apache.Http;
using Apache.Http.Conn;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>
	/// Represents a managed connection whose state and life cycle is managed by
	/// a connection manager.
	/// </summary>
	/// <remarks>
	/// Represents a managed connection whose state and life cycle is managed by
	/// a connection manager. This interface extends
	/// <see cref="Apache.Http.HttpClientConnection">Apache.Http.HttpClientConnection</see>
	/// with methods to bind the connection to an arbitrary socket and
	/// to obtain SSL session details.
	/// </remarks>
	/// <since>4.3</since>
	public interface ManagedHttpClientConnection : HttpClientConnection, HttpInetConnection
	{
		/// <summary>
		/// Returns connection ID which is expected to be unique
		/// for the life span of the connection manager.
		/// </summary>
		/// <remarks>
		/// Returns connection ID which is expected to be unique
		/// for the life span of the connection manager.
		/// </remarks>
		string GetId();

		/// <summary>Binds this connection to the given socket.</summary>
		/// <remarks>
		/// Binds this connection to the given socket. The connection
		/// is considered open if it is bound and the underlying socket
		/// is connection to a remote host.
		/// </remarks>
		/// <param name="socket">the socket to bind the connection to.</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		void Bind(Socket socket);

		/// <summary>Returns the underlying socket.</summary>
		/// <remarks>Returns the underlying socket.</remarks>
		Socket GetSocket();

		/// <summary>Obtains the SSL session of the underlying connection, if any.</summary>
		/// <remarks>
		/// Obtains the SSL session of the underlying connection, if any.
		/// If this connection is open, and the underlying socket is an
		/// <see cref="Sharpen.SSLSocket">SSLSocket</see>
		/// , the SSL session of
		/// that socket is obtained. This is a potentially blocking operation.
		/// </remarks>
		/// <returns>
		/// the underlying SSL session if available,
		/// <code>null</code> otherwise
		/// </returns>
		SSLSession GetSSLSession();
	}
}
