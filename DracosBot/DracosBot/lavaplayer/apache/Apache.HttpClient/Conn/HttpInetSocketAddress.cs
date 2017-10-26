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
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>
	/// Extended
	/// <see cref="System.Net.IPEndPoint">System.Net.IPEndPoint</see>
	/// implementation that also provides access to the original
	/// <see cref="Apache.Http.HttpHost">Apache.Http.HttpHost</see>
	/// used to resolve the address.
	/// </summary>
	/// <since>4.2 no longer used.</since>
	[System.Serializable]
	[System.ObsoleteAttribute(@"(4.3)")]
	public class HttpInetSocketAddress : IPEndPoint
	{
		private const long serialVersionUID = -6650701828361907957L;

		private readonly HttpHost httphost;

		public HttpInetSocketAddress(HttpHost httphost, IPAddress addr, int port) : base(
			addr, port)
		{
			Args.NotNull(httphost, "HTTP host");
			this.httphost = httphost;
		}

		public virtual HttpHost GetHttpHost()
		{
			return this.httphost;
		}

		public override string ToString()
		{
			return this.httphost.GetHostName() + ":" + Port;
		}
	}
}
