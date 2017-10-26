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
	[System.ObsoleteAttribute(@"(4.2) do not use")]
	internal class SchemeLayeredSocketFactoryAdaptor2 : SchemeLayeredSocketFactory
	{
		private readonly LayeredSchemeSocketFactory factory;

		internal SchemeLayeredSocketFactoryAdaptor2(LayeredSchemeSocketFactory factory) : 
			base()
		{
			this.factory = factory;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual Socket CreateSocket(HttpParams @params)
		{
			return this.factory.CreateSocket(@params);
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Sharpen.UnknownHostException"></exception>
		/// <exception cref="Apache.Http.Conn.ConnectTimeoutException"></exception>
		public virtual Socket ConnectSocket(Socket sock, IPEndPoint remoteAddress, IPEndPoint
			 localAddress, HttpParams @params)
		{
			return this.factory.ConnectSocket(sock, remoteAddress, localAddress, @params);
		}

		/// <exception cref="System.ArgumentException"></exception>
		public virtual bool IsSecure(Socket sock)
		{
			return this.factory.IsSecure(sock);
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Sharpen.UnknownHostException"></exception>
		public virtual Socket CreateLayeredSocket(Socket socket, string target, int port, 
			HttpParams @params)
		{
			return this.factory.CreateLayeredSocket(socket, target, port, true);
		}
	}
}
