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
	[System.ObsoleteAttribute(@"(4.1) do not use")]
	internal class SchemeSocketFactoryAdaptor : SchemeSocketFactory
	{
		private readonly SocketFactory factory;

		internal SchemeSocketFactoryAdaptor(SocketFactory factory) : base()
		{
			this.factory = factory;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Sharpen.UnknownHostException"></exception>
		/// <exception cref="Apache.Http.Conn.ConnectTimeoutException"></exception>
		public virtual Socket ConnectSocket(Socket sock, IPEndPoint remoteAddress, IPEndPoint
			 localAddress, HttpParams @params)
		{
			string host = remoteAddress.GetHostName();
			int port = remoteAddress.Port;
			IPAddress local = null;
			int localPort = 0;
			if (localAddress != null)
			{
				local = localAddress.Address;
				localPort = localAddress.Port;
			}
			return this.factory.ConnectSocket(sock, host, port, local, localPort, @params);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual Socket CreateSocket(HttpParams @params)
		{
			return this.factory.CreateSocket();
		}

		/// <exception cref="System.ArgumentException"></exception>
		public virtual bool IsSecure(Socket sock)
		{
			return this.factory.IsSecure(sock);
		}

		public virtual SocketFactory GetFactory()
		{
			return this.factory;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj is Apache.Http.Conn.Scheme.SchemeSocketFactoryAdaptor)
			{
				return this.factory.Equals(((Apache.Http.Conn.Scheme.SchemeSocketFactoryAdaptor)obj
					).factory);
			}
			else
			{
				return this.factory.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return this.factory.GetHashCode();
		}
	}
}
