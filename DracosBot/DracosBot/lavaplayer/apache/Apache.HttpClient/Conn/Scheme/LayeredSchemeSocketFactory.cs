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
using Apache.Http.Conn.Scheme;
using Sharpen;

namespace Apache.Http.Conn.Scheme
{
	/// <summary>
	/// Extended
	/// <see cref="SchemeSocketFactory">SchemeSocketFactory</see>
	/// interface for layered sockets such as SSL/TLS.
	/// </summary>
	/// <since>4.1</since>
	[System.ObsoleteAttribute(@"(4.2)  use SchemeLayeredSocketFactory")]
	public interface LayeredSchemeSocketFactory : SchemeSocketFactory
	{
		/// <summary>
		/// Returns a socket connected to the given host that is layered over an
		/// existing socket.
		/// </summary>
		/// <remarks>
		/// Returns a socket connected to the given host that is layered over an
		/// existing socket.  Used primarily for creating secure sockets through
		/// proxies.
		/// </remarks>
		/// <param name="socket">the existing socket</param>
		/// <param name="target">the name of the target host.</param>
		/// <param name="port">the port to connect to on the target host</param>
		/// <param name="autoClose">
		/// a flag for closing the underling socket when the created
		/// socket is closed
		/// </param>
		/// <returns>Socket a new socket</returns>
		/// <exception cref="System.IO.IOException">if an I/O error occurs while creating the socket
		/// 	</exception>
		/// <exception cref="Sharpen.UnknownHostException">
		/// if the IP address of the host cannot be
		/// determined
		/// </exception>
		Socket CreateLayeredSocket(Socket socket, string target, int port, bool autoClose
			);
	}
}
