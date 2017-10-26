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
using Apache.Http;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Pool;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <since>4.3</since>
	internal class CPoolEntry : PoolEntry<HttpRoute, ManagedHttpClientConnection>
	{
		private readonly Log log;

		private volatile bool routeComplete;

		public CPoolEntry(Log log, string id, HttpRoute route, ManagedHttpClientConnection
			 conn, long timeToLive, TimeUnit tunit) : base(id, route, conn, timeToLive, tunit
			)
		{
			this.log = log;
		}

		public virtual void MarkRouteComplete()
		{
			this.routeComplete = true;
		}

		public virtual bool IsRouteComplete()
		{
			return this.routeComplete;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void CloseConnection()
		{
			HttpClientConnection conn = GetConnection();
			conn.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void ShutdownConnection()
		{
			HttpClientConnection conn = GetConnection();
			conn.Shutdown();
		}

		public override bool IsExpired(long now)
		{
			bool expired = base.IsExpired(now);
			if (expired && this.log.IsDebugEnabled())
			{
				this.log.Debug("Connection " + this + " expired @ " + Sharpen.Extensions.CreateDate
					(GetExpiry()));
			}
			return expired;
		}

		public override bool IsClosed()
		{
			HttpClientConnection conn = GetConnection();
			return !conn.IsOpen();
		}

		public override void Close()
		{
			try
			{
				CloseConnection();
			}
			catch (IOException ex)
			{
				this.log.Debug("I/O error closing connection", ex);
			}
		}
	}
}
