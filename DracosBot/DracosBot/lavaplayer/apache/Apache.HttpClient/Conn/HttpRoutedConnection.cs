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
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>Interface to access routing information of a client side connection.</summary>
	/// <remarks>Interface to access routing information of a client side connection.</remarks>
	/// <since>4.1</since>
	[System.ObsoleteAttribute(@"(4.3) replaced by HttpClientConnectionManager .")]
	public interface HttpRoutedConnection : HttpInetConnection
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
	}
}
