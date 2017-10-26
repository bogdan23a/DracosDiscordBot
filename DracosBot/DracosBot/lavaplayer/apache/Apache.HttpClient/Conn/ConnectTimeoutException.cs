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
using System.Net;
using System.Threading;
using Apache.Http;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>
	/// A timeout while connecting to an HTTP server or waiting for an
	/// available connection from an HttpConnectionManager.
	/// </summary>
	/// <remarks>
	/// A timeout while connecting to an HTTP server or waiting for an
	/// available connection from an HttpConnectionManager.
	/// </remarks>
	/// <since>4.0</since>
	[System.Serializable]
	public class ConnectTimeoutException : ThreadInterruptedException
	{
		private const long serialVersionUID = -4816682903149535989L;

		private readonly HttpHost host;

		/// <summary>Creates a ConnectTimeoutException with a <tt>null</tt> detail message.</summary>
		/// <remarks>Creates a ConnectTimeoutException with a <tt>null</tt> detail message.</remarks>
		public ConnectTimeoutException() : base()
		{
			this.host = null;
		}

		/// <summary>Creates a ConnectTimeoutException with the specified detail message.</summary>
		/// <remarks>Creates a ConnectTimeoutException with the specified detail message.</remarks>
		public ConnectTimeoutException(string message) : base(message)
		{
			this.host = null;
		}

		/// <summary>
		/// Creates a ConnectTimeoutException based on original
		/// <see cref="System.IO.IOException">System.IO.IOException</see>
		/// .
		/// </summary>
		/// <since>4.3</since>
		public ConnectTimeoutException(IOException cause, HttpHost host, params IPAddress
			[] remoteAddresses) : base("Connect to " + (host != null ? host.ToHostString() : 
			"remote host") + (remoteAddresses != null && remoteAddresses.Length > 0 ? " " + 
			Arrays.AsList(remoteAddresses) : string.Empty) + ((cause != null && cause.Message
			 != null) ? " failed: " + cause.Message : " timed out"))
		{
			this.host = host;
			Sharpen.Extensions.InitCause(this, cause);
		}

		/// <since>4.3</since>
		public virtual HttpHost GetHost()
		{
			return host;
		}
	}
}
