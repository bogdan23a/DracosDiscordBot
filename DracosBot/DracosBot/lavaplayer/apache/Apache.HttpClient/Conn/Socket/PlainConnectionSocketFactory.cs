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
using Apache.Http;
using Apache.Http.Conn.Socket;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Conn.Socket
{
	/// <summary>The default class for creating plain (unencrypted) sockets.</summary>
	/// <remarks>The default class for creating plain (unencrypted) sockets.</remarks>
	/// <since>4.3</since>
	public class PlainConnectionSocketFactory : ConnectionSocketFactory
	{
		public static readonly Apache.Http.Conn.Socket.PlainConnectionSocketFactory Instance
			 = new Apache.Http.Conn.Socket.PlainConnectionSocketFactory();

		public static Apache.Http.Conn.Socket.PlainConnectionSocketFactory GetSocketFactory
			()
		{
			return Instance;
		}

		public PlainConnectionSocketFactory() : base()
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual System.Net.Sockets.Socket CreateSocket(HttpContext context)
		{
			return new System.Net.Sockets.Socket();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual System.Net.Sockets.Socket ConnectSocket(int connectTimeout, System.Net.Sockets.Socket
			 socket, HttpHost host, IPEndPoint remoteAddress, IPEndPoint localAddress, HttpContext
			 context)
		{
			System.Net.Sockets.Socket sock = socket != null ? socket : CreateSocket(context);
			if (localAddress != null)
			{
				sock.Bind2(localAddress);
			}
			try
			{
				sock.Connect(remoteAddress, connectTimeout);
			}
			catch (IOException ex)
			{
				try
				{
					sock.Close();
				}
				catch (IOException)
				{
				}
				throw;
			}
			return sock;
		}
	}
}
