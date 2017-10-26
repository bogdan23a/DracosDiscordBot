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
	[System.ObsoleteAttribute(@"(4.1) do not use")]
	internal class LayeredSocketFactoryAdaptor : SocketFactoryAdaptor, LayeredSocketFactory
	{
		private readonly LayeredSchemeSocketFactory factory;

		internal LayeredSocketFactoryAdaptor(LayeredSchemeSocketFactory factory) : base(factory
			)
		{
			this.factory = factory;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Sharpen.UnknownHostException"></exception>
		public virtual Socket CreateSocket(Socket socket, string host, int port, bool autoClose
			)
		{
			return this.factory.CreateLayeredSocket(socket, host, port, autoClose);
		}
	}
}
