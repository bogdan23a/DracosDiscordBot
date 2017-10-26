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

using System;
using System.IO;
using System.Net;
using Apache.Http;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>
	/// A
	/// <see cref="Sharpen.ConnectException">Sharpen.ConnectException</see>
	/// that specifies the
	/// <see cref="Apache.Http.HttpHost">Apache.Http.HttpHost</see>
	/// that was
	/// being connected to.
	/// </summary>
	/// <since>4.0</since>
	[System.Serializable]
	public class HttpHostConnectException : ConnectException
	{
		private const long serialVersionUID = -3194482710275220224L;

		private readonly HttpHost host;

		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) use HttpHostConnectException(System.IO.IOException, Apache.Http.HttpHost, System.Net.IPAddress[])"
			)]
		public HttpHostConnectException(HttpHost host, ConnectException cause) : this(cause
			, host, null)
		{
		}

		/// <summary>
		/// Creates a HttpHostConnectException based on original
		/// <see cref="System.IO.IOException">System.IO.IOException</see>
		/// .
		/// </summary>
		/// <since>4.3</since>
		public HttpHostConnectException(IOException cause, HttpHost host, params IPAddress
			[] remoteAddresses) : base("Connect to " + (host != null ? host.ToHostString() : 
			"remote host") + (remoteAddresses != null && remoteAddresses.Length > 0 ? " " + 
			Arrays.AsList(remoteAddresses) : string.Empty) + ((cause != null && cause.Message
			 != null) ? " failed: " + cause.Message : " refused"))
		{
			this.host = host;
			Sharpen.Extensions.InitCause(this, cause);
		}

		public virtual HttpHost GetHost()
		{
			return this.host;
		}
	}
}
