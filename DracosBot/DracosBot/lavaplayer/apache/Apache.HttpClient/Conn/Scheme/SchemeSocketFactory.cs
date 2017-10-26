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
using System.Net.Sockets;
using Apache.Http.Conn.Scheme;
using Apache.Http.Params;
using Sharpen;

namespace Apache.Http.Conn.Scheme
{
	/// <summary>A factory for creating, initializing and connecting sockets.</summary>
	/// <remarks>
	/// A factory for creating, initializing and connecting sockets. The factory encapsulates the logic
	/// for establishing a socket connection.
	/// </remarks>
	/// <since>4.1</since>
	[System.ObsoleteAttribute(@"(4.3) use Apache.Http.Conn.Socket.ConnectionSocketFactory"
		)]
	public interface SchemeSocketFactory
	{
		/// <summary>Creates a new, unconnected socket.</summary>
		/// <remarks>
		/// Creates a new, unconnected socket. The socket should subsequently be passed to
		/// <see cref="ConnectSocket(System.Net.Sockets.Socket, System.Net.IPEndPoint, System.Net.IPEndPoint, Apache.Http.Params.HttpParams)
		/// 	">ConnectSocket(System.Net.Sockets.Socket, System.Net.IPEndPoint, System.Net.IPEndPoint, Apache.Http.Params.HttpParams)
		/// 	</see>
		/// .
		/// </remarks>
		/// <param name="params">
		/// Optional
		/// <see cref="Apache.Http.Params.HttpParams">parameters</see>
		/// . In most cases these parameters
		/// will not be required and will have no effect, as usually socket
		/// initialization should take place in the
		/// <see cref="ConnectSocket(System.Net.Sockets.Socket, System.Net.IPEndPoint, System.Net.IPEndPoint, Apache.Http.Params.HttpParams)
		/// 	">ConnectSocket(System.Net.Sockets.Socket, System.Net.IPEndPoint, System.Net.IPEndPoint, Apache.Http.Params.HttpParams)
		/// 	</see>
		/// method. However, in rare cases one may want to pass additional parameters
		/// to this method in order to create a customized
		/// <see cref="System.Net.Sockets.Socket">System.Net.Sockets.Socket</see>
		/// instance,
		/// for instance bound to a SOCKS proxy server.
		/// </param>
		/// <returns>a new socket</returns>
		/// <exception cref="System.IO.IOException">if an I/O error occurs while creating the socket
		/// 	</exception>
		Socket CreateSocket(HttpParams @params);

		/// <summary>Connects a socket to the target host with the given remote address.</summary>
		/// <remarks>
		/// Connects a socket to the target host with the given remote address.
		/// <p/>
		/// Please note that
		/// <see cref="Apache.Http.Conn.HttpInetSocketAddress">Apache.Http.Conn.HttpInetSocketAddress
		/// 	</see>
		/// class should
		/// be used in order to pass the target remote address along with the original
		/// <see cref="Apache.Http.HttpHost">Apache.Http.HttpHost</see>
		/// value used to resolve the address. The use of
		/// <see cref="Apache.Http.Conn.HttpInetSocketAddress">Apache.Http.Conn.HttpInetSocketAddress
		/// 	</see>
		/// can also ensure that no reverse
		/// DNS lookup will be performed if the target remote address was specified
		/// as an IP address.
		/// </remarks>
		/// <param name="sock">
		/// the socket to connect, as obtained from
		/// <see cref="CreateSocket(Apache.Http.Params.HttpParams)">createSocket</see>
		/// .
		/// <code>null</code> indicates that a new socket
		/// should be created and connected.
		/// </param>
		/// <param name="remoteAddress">the remote address to connect to.</param>
		/// <param name="localAddress">
		/// the local address to bind the socket to, or
		/// <code>null</code> for any
		/// </param>
		/// <param name="params">
		/// additional
		/// <see cref="Apache.Http.Params.HttpParams">parameters</see>
		/// for connecting
		/// </param>
		/// <returns>
		/// the connected socket. The returned object may be different
		/// from the <code>sock</code> argument if this factory supports
		/// a layered protocol.
		/// </returns>
		/// <exception cref="System.IO.IOException">if an I/O error occurs</exception>
		/// <exception cref="Sharpen.UnknownHostException">
		/// if the IP address of the target host
		/// can not be determined
		/// </exception>
		/// <exception cref="Apache.Http.Conn.ConnectTimeoutException">
		/// if the socket cannot be connected
		/// within the time limit defined in the <code>params</code>
		/// </exception>
		/// <seealso cref="Apache.Http.Conn.HttpInetSocketAddress">Apache.Http.Conn.HttpInetSocketAddress
		/// 	</seealso>
		Socket ConnectSocket(Socket sock, IPEndPoint remoteAddress, IPEndPoint localAddress
			, HttpParams @params);

		/// <summary>Checks whether a socket provides a secure connection.</summary>
		/// <remarks>
		/// Checks whether a socket provides a secure connection. The socket must be
		/// <see cref="ConnectSocket(System.Net.Sockets.Socket, System.Net.IPEndPoint, System.Net.IPEndPoint, Apache.Http.Params.HttpParams)
		/// 	">connected</see>
		/// by this factory. The factory will <i>not</i> perform I/O operations in this method.
		/// <p>
		/// As a rule of thumb, plain sockets are not secure and TLS/SSL sockets are secure. However,
		/// there may be application specific deviations. For example, a plain socket to a host in the
		/// same intranet ("trusted zone") could be considered secure. On the other hand, a TLS/SSL
		/// socket could be considered insecure based on the cipher suite chosen for the connection.
		/// </remarks>
		/// <param name="sock">the connected socket to check</param>
		/// <returns>
		/// <code>true</code> if the connection of the socket
		/// should be considered secure, or
		/// <code>false</code> if it should not
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// if the argument is invalid, for example because it is
		/// not a connected socket or was created by a different
		/// socket factory.
		/// Note that socket factories are <i>not</i> required to
		/// check these conditions, they may simply return a default
		/// value when called with an invalid socket argument.
		/// </exception>
		bool IsSecure(Socket sock);
	}
}
