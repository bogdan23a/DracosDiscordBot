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

using System.Net;
using Apache.Http;
using Apache.Http.Conn.Socket;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Conn.Socket
{
	/// <summary>A factory for creating and connecting connection sockets.</summary>
	/// <remarks>A factory for creating and connecting connection sockets.</remarks>
	/// <since>4.3</since>
	public interface ConnectionSocketFactory
	{
		/// <summary>Creates new, unconnected socket.</summary>
		/// <remarks>
		/// Creates new, unconnected socket. The socket should subsequently be passed to
		/// <see cref="ConnectSocket(int, System.Net.Sockets.Socket, Apache.Http.HttpHost, System.Net.IPEndPoint, System.Net.IPEndPoint, Apache.Http.Protocol.HttpContext)
		/// 	">connectSocket</see>
		/// method.
		/// </remarks>
		/// <returns>a new socket</returns>
		/// <exception cref="System.IO.IOException">if an I/O error occurs while creating the socket
		/// 	</exception>
		System.Net.Sockets.Socket CreateSocket(HttpContext context);

		/// <summary>Connects the socket to the target host with the given resolved remote address.
		/// 	</summary>
		/// <remarks>Connects the socket to the target host with the given resolved remote address.
		/// 	</remarks>
		/// <param name="connectTimeout">connect timeout.</param>
		/// <param name="sock">
		/// the socket to connect, as obtained from
		/// <see cref="CreateSocket(Apache.Http.Protocol.HttpContext)">CreateSocket(Apache.Http.Protocol.HttpContext)
		/// 	</see>
		/// .
		/// <code>null</code> indicates that a new socket should be created and connected.
		/// </param>
		/// <param name="host">target host as specified by the caller (end user).</param>
		/// <param name="remoteAddress">the resolved remote address to connect to.</param>
		/// <param name="localAddress">the local address to bind the socket to, or <code>null</code> for any.
		/// 	</param>
		/// <param name="context">the actual HTTP context.</param>
		/// <returns>
		/// the connected socket. The returned object may be different
		/// from the <code>sock</code> argument if this factory supports
		/// a layered protocol.
		/// </returns>
		/// <exception cref="System.IO.IOException">if an I/O error occurs</exception>
		System.Net.Sockets.Socket ConnectSocket(int connectTimeout, System.Net.Sockets.Socket
			 sock, HttpHost host, IPEndPoint remoteAddress, IPEndPoint localAddress, HttpContext
			 context);
	}
}
