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

using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Conn;
using Apache.Http.Pool;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <since>4.3</since>
	internal class CPool : AbstractConnPool<HttpRoute, ManagedHttpClientConnection, CPoolEntry
		>
	{
		private static readonly AtomicLong Counter = new AtomicLong();

		private readonly Log log = LogFactory.GetLog(typeof(Apache.Http.Impl.Conn.CPool));

		private readonly long timeToLive;

		private readonly TimeUnit tunit;

		public CPool(ConnFactory<HttpRoute, ManagedHttpClientConnection> connFactory, int
			 defaultMaxPerRoute, int maxTotal, long timeToLive, TimeUnit tunit) : base(connFactory
			, defaultMaxPerRoute, maxTotal)
		{
			this.timeToLive = timeToLive;
			this.tunit = tunit;
		}

		protected override CPoolEntry CreateEntry(HttpRoute route, ManagedHttpClientConnection
			 conn)
		{
			string id = System.Convert.ToString(Counter.GetAndIncrement());
			return new CPoolEntry(this.log, id, route, conn, this.timeToLive, this.tunit);
		}
	}
}
