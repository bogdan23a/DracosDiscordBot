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
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>A client-side connection with advanced connection logic.</summary>
	/// <remarks>
	/// A client-side connection with advanced connection logic.
	/// Instances are typically obtained from a connection manager.
	/// </remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) replaced by HttpClientConnectionManager .")]
	public interface ManagedClientConnection : HttpClientConnection, HttpRoutedConnection
		, ManagedHttpClientConnection, ConnectionReleaseTrigger
	{
		/// <summary>Indicates whether this connection is secure.</summary>
		/// <remarks>
		/// Indicates whether this connection is secure.
		/// The return value is well-defined only while the connection is open.
		/// It may change even while the connection is open.
		/// </remarks>
		/// <returns>
		/// <code>true</code> if this connection is secure,
		/// <code>false</code> otherwise
		/// </returns>
		bool IsSecure();

		/// <summary>Obtains the current route of this connection.</summary>
		/// <remarks>Obtains the current route of this connection.</remarks>
		/// <returns>
		/// the route established so far, or
		/// <code>null</code> if not connected
		/// </returns>
		HttpRoute GetRoute();

		/// <summary>Obtains the SSL session of the underlying connection, if any.</summary>
		/// <remarks>
		/// Obtains the SSL session of the underlying connection, if any.
		/// If this connection is open, and the underlying socket is an
		/// <see cref="Sharpen.SSLSocket">SSLSocket</see>
		/// , the SSL session of
		/// that socket is obtained. This is a potentially blocking operation.
		/// <br/>
		/// <b>Note:</b> Whether the underlying socket is an SSL socket
		/// can not necessarily be determined via
		/// <see cref="IsSecure()">IsSecure()</see>
		/// .
		/// Plain sockets may be considered secure, for example if they are
		/// connected to a known host in the same network segment.
		/// On the other hand, SSL sockets may be considered insecure,
		/// for example depending on the chosen cipher suite.
		/// </remarks>
		/// <returns>
		/// the underlying SSL session if available,
		/// <code>null</code> otherwise
		/// </returns>
		SSLSession GetSSLSession();

		/// <summary>Opens this connection according to the given route.</summary>
		/// <remarks>Opens this connection according to the given route.</remarks>
		/// <param name="route">
		/// the route along which to open. It will be opened to
		/// the first proxy if present, or directly to the target.
		/// </param>
		/// <param name="context">the context for opening this connection</param>
		/// <param name="params">the parameters for opening this connection</param>
		/// <exception cref="System.IO.IOException">in case of a problem</exception>
		void Open(HttpRoute route, HttpContext context, HttpParams @params);

		/// <summary>Indicates that a tunnel to the target has been established.</summary>
		/// <remarks>
		/// Indicates that a tunnel to the target has been established.
		/// The route is the one previously passed to
		/// <see cref="Open(Apache.Http.Conn.Routing.HttpRoute, Apache.Http.Protocol.HttpContext, Apache.Http.Params.HttpParams)
		/// 	">open</see>
		/// .
		/// Subsequently,
		/// <see cref="LayerProtocol(Apache.Http.Protocol.HttpContext, Apache.Http.Params.HttpParams)
		/// 	">layerProtocol</see>
		/// can be called
		/// to layer the TLS/SSL protocol on top of the tunnelled connection.
		/// <br/>
		/// <b>Note:</b> In HttpClient 3, a call to the corresponding method
		/// would automatically trigger the layering of the TLS/SSL protocol.
		/// This is not the case anymore, you can establish a tunnel without
		/// layering a new protocol over the connection.
		/// </remarks>
		/// <param name="secure">
		/// <code>true</code> if the tunnel should be considered
		/// secure, <code>false</code> otherwise
		/// </param>
		/// <param name="params">the parameters for tunnelling this connection</param>
		/// <exception cref="System.IO.IOException">in case of a problem</exception>
		void TunnelTarget(bool secure, HttpParams @params);

		/// <summary>Indicates that a tunnel to an intermediate proxy has been established.</summary>
		/// <remarks>
		/// Indicates that a tunnel to an intermediate proxy has been established.
		/// This is used exclusively for so-called <i>proxy chains</i>, where
		/// a request has to pass through multiple proxies before reaching the
		/// target. In that case, all proxies but the last need to be tunnelled
		/// when establishing the connection. Tunnelling of the last proxy to the
		/// target is optional and would be indicated via
		/// <see cref="TunnelTarget(bool, Apache.Http.Params.HttpParams)">TunnelTarget(bool, Apache.Http.Params.HttpParams)
		/// 	</see>
		/// .
		/// </remarks>
		/// <param name="next">
		/// the proxy to which the tunnel was established.
		/// This is <i>not</i> the proxy <i>through</i> which
		/// the tunnel was established, but the new end point
		/// of the tunnel. The tunnel does <i>not</i> yet
		/// reach to the target, use
		/// <see cref="TunnelTarget(bool, Apache.Http.Params.HttpParams)">TunnelTarget(bool, Apache.Http.Params.HttpParams)
		/// 	</see>
		/// to indicate an end-to-end tunnel.
		/// </param>
		/// <param name="secure">
		/// <code>true</code> if the connection should be
		/// considered secure, <code>false</code> otherwise
		/// </param>
		/// <param name="params">the parameters for tunnelling this connection</param>
		/// <exception cref="System.IO.IOException">in case of a problem</exception>
		void TunnelProxy(HttpHost next, bool secure, HttpParams @params);

		/// <summary>
		/// Layers a new protocol on top of a
		/// <see cref="TunnelTarget(bool, Apache.Http.Params.HttpParams)">tunnelled</see>
		/// connection. This is typically used to create a TLS/SSL connection
		/// through a proxy.
		/// The route is the one previously passed to
		/// <see cref="Open(Apache.Http.Conn.Routing.HttpRoute, Apache.Http.Protocol.HttpContext, Apache.Http.Params.HttpParams)
		/// 	">open</see>
		/// .
		/// It is not guaranteed that the layered connection is
		/// <see cref="IsSecure()">secure</see>
		/// .
		/// </summary>
		/// <param name="context">the context for layering on top of this connection</param>
		/// <param name="params">the parameters for layering on top of this connection</param>
		/// <exception cref="System.IO.IOException">in case of a problem</exception>
		void LayerProtocol(HttpContext context, HttpParams @params);

		/// <summary>Marks this connection as being in a reusable communication state.</summary>
		/// <remarks>
		/// Marks this connection as being in a reusable communication state.
		/// The checkpoints for reuseable communication states (in the absence
		/// of pipelining) are before sending a request and after receiving
		/// the response in its entirety.
		/// The connection will automatically clear the checkpoint when
		/// used for communication. A call to this method indicates that
		/// the next checkpoint has been reached.
		/// <br/>
		/// A reusable communication state is necessary but not sufficient
		/// for the connection to be reused.
		/// A
		/// <see cref="GetRoute()">route</see>
		/// mismatch, the connection being closed,
		/// or other circumstances might prevent reuse.
		/// </remarks>
		void MarkReusable();

		/// <summary>Marks this connection as not being in a reusable state.</summary>
		/// <remarks>
		/// Marks this connection as not being in a reusable state.
		/// This can be used immediately before releasing this connection
		/// to prevent its reuse. Reasons for preventing reuse include
		/// error conditions and the evaluation of a
		/// <see cref="Apache.Http.ConnectionReuseStrategy">reuse strategy</see>
		/// .
		/// <br/>
		/// <b>Note:</b>
		/// It is <i>not</i> necessary to call here before writing to
		/// or reading from this connection. Communication attempts will
		/// automatically unmark the state as non-reusable. It can then
		/// be switched back using
		/// <see cref="MarkReusable()">markReusable</see>
		/// .
		/// </remarks>
		void UnmarkReusable();

		/// <summary>Indicates whether this connection is in a reusable communication state.</summary>
		/// <remarks>
		/// Indicates whether this connection is in a reusable communication state.
		/// See
		/// <see cref="MarkReusable()">markReusable</see>
		/// and
		/// <see cref="UnmarkReusable()">unmarkReusable</see>
		/// for details.
		/// </remarks>
		/// <returns>
		/// <code>true</code> if this connection is marked as being in
		/// a reusable communication state,
		/// <code>false</code> otherwise
		/// </returns>
		bool IsMarkedReusable();

		/// <summary>Assigns a state object to this connection.</summary>
		/// <remarks>
		/// Assigns a state object to this connection. Connection managers may make
		/// use of the connection state when allocating persistent connections.
		/// </remarks>
		/// <param name="state">The state object</param>
		void SetState(object state);

		/// <summary>Returns the state object associated with this connection.</summary>
		/// <remarks>Returns the state object associated with this connection.</remarks>
		/// <returns>The state object</returns>
		object GetState();

		/// <summary>
		/// Sets the duration that this connection can remain idle before it is
		/// reused.
		/// </summary>
		/// <remarks>
		/// Sets the duration that this connection can remain idle before it is
		/// reused. The connection should not be used again if this time elapses. The
		/// idle duration must be reset after each request sent over this connection.
		/// The elapsed time starts counting when the connection is released, which
		/// is typically after the headers (and any response body, if present) is
		/// fully consumed.
		/// </remarks>
		void SetIdleDuration(long duration, TimeUnit unit);
	}
}
