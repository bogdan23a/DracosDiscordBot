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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Apache.Http;
using Apache.Http.Config;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Conn.Socket;
using Apache.Http.Conn.Ssl;
using Apache.Http.Impl.Conn;
using Apache.Http.Pool;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>
	/// <tt>ClientConnectionPoolManager</tt> maintains a pool of
	/// <see cref="Apache.Http.HttpClientConnection">Apache.Http.HttpClientConnection</see>
	/// s and is able to service connection requests
	/// from multiple execution threads. Connections are pooled on a per route
	/// basis. A request for a route which already the manager has persistent
	/// connections for available in the pool will be services by leasing
	/// a connection from the pool rather than creating a brand new connection.
	/// <p/>
	/// <tt>ClientConnectionPoolManager</tt> maintains a maximum limit of connection
	/// on a per route basis and in total. Per default this implementation will
	/// create no more than than 2 concurrent connections per given route
	/// and no more 20 connections in total. For many real-world applications
	/// these limits may prove too constraining, especially if they use HTTP
	/// as a transport protocol for their services. Connection limits, however,
	/// can be adjusted using
	/// <see cref="Apache.Http.Pool.ConnPoolControl{T}">Apache.Http.Pool.ConnPoolControl&lt;T&gt;
	/// 	</see>
	/// methods.
	/// </summary>
	/// <since>4.3</since>
	public class PoolingHttpClientConnectionManager : HttpClientConnectionManager, ConnPoolControl
		<HttpRoute>, IDisposable
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly PoolingHttpClientConnectionManager.ConfigData configData;

		private readonly CPool pool;

		private readonly HttpClientConnectionOperator connectionOperator;

		private static Registry<ConnectionSocketFactory> GetDefaultRegistry()
		{
			return RegistryBuilder.Create<ConnectionSocketFactory>().Register("http", PlainConnectionSocketFactory
				.GetSocketFactory()).Register("https", SSLConnectionSocketFactory.GetSocketFactory
				()).Build();
		}

		public PoolingHttpClientConnectionManager() : this(GetDefaultRegistry())
		{
		}

		public PoolingHttpClientConnectionManager(long timeToLive, TimeUnit tunit) : this
			(GetDefaultRegistry(), null, null, null, timeToLive, tunit)
		{
		}

		public PoolingHttpClientConnectionManager(Registry<ConnectionSocketFactory> socketFactoryRegistry
			) : this(socketFactoryRegistry, null, null)
		{
		}

		public PoolingHttpClientConnectionManager(Registry<ConnectionSocketFactory> socketFactoryRegistry
			, DnsResolver dnsResolver) : this(socketFactoryRegistry, null, dnsResolver)
		{
		}

		public PoolingHttpClientConnectionManager(Registry<ConnectionSocketFactory> socketFactoryRegistry
			, HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> connFactory) : this
			(socketFactoryRegistry, connFactory, null)
		{
		}

		public PoolingHttpClientConnectionManager(HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection
			> connFactory) : this(GetDefaultRegistry(), connFactory, null)
		{
		}

		public PoolingHttpClientConnectionManager(Registry<ConnectionSocketFactory> socketFactoryRegistry
			, HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> connFactory, DnsResolver
			 dnsResolver) : this(socketFactoryRegistry, connFactory, null, dnsResolver, -1, 
			TimeUnit.Milliseconds)
		{
		}

		public PoolingHttpClientConnectionManager(Registry<ConnectionSocketFactory> socketFactoryRegistry
			, HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> connFactory, SchemePortResolver
			 schemePortResolver, DnsResolver dnsResolver, long timeToLive, TimeUnit tunit) : 
			base()
		{
			this.configData = new PoolingHttpClientConnectionManager.ConfigData();
			this.pool = new CPool(new PoolingHttpClientConnectionManager.InternalConnectionFactory
				(this.configData, connFactory), 2, 20, timeToLive, tunit);
			this.connectionOperator = new HttpClientConnectionOperator(socketFactoryRegistry, 
				schemePortResolver, dnsResolver);
		}

		internal PoolingHttpClientConnectionManager(CPool pool, Lookup<ConnectionSocketFactory
			> socketFactoryRegistry, SchemePortResolver schemePortResolver, DnsResolver dnsResolver
			) : base()
		{
			this.configData = new PoolingHttpClientConnectionManager.ConfigData();
			this.pool = pool;
			this.connectionOperator = new HttpClientConnectionOperator(socketFactoryRegistry, 
				schemePortResolver, dnsResolver);
		}

		~PoolingHttpClientConnectionManager()
		{
			try
			{
				Shutdown();
			}
			finally
			{
				base.Finalize();
			}
		}

		public virtual void Close()
		{
			Shutdown();
		}

		private string Format(HttpRoute route, object state)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("[route: ").Append(route).Append("]");
			if (state != null)
			{
				buf.Append("[state: ").Append(state).Append("]");
			}
			return buf.ToString();
		}

		private string FormatStats(HttpRoute route)
		{
			StringBuilder buf = new StringBuilder();
			PoolStats totals = this.pool.GetTotalStats();
			PoolStats stats = this.pool.GetStats(route);
			buf.Append("[total kept alive: ").Append(totals.GetAvailable()).Append("; ");
			buf.Append("route allocated: ").Append(stats.GetLeased() + stats.GetAvailable());
			buf.Append(" of ").Append(stats.GetMax()).Append("; ");
			buf.Append("total allocated: ").Append(totals.GetLeased() + totals.GetAvailable()
				);
			buf.Append(" of ").Append(totals.GetMax()).Append("]");
			return buf.ToString();
		}

		private string Format(CPoolEntry entry)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("[id: ").Append(entry.GetId()).Append("]");
			buf.Append("[route: ").Append(entry.GetRoute()).Append("]");
			object state = entry.GetState();
			if (state != null)
			{
				buf.Append("[state: ").Append(state).Append("]");
			}
			return buf.ToString();
		}

		public virtual ConnectionRequest RequestConnection(HttpRoute route, object state)
		{
			Args.NotNull(route, "HTTP route");
			if (this.log.IsDebugEnabled())
			{
				this.log.Debug("Connection request: " + Format(route, state) + FormatStats(route)
					);
			}
			Future<CPoolEntry> future = this.pool.Lease(route, state, null);
			return new _ConnectionRequest_218(this, future);
		}

		private sealed class _ConnectionRequest_218 : ConnectionRequest
		{
			public _ConnectionRequest_218(PoolingHttpClientConnectionManager _enclosing, Future
				<CPoolEntry> future)
			{
				this._enclosing = _enclosing;
				this.future = future;
			}

			public bool Cancel()
			{
				return future.Cancel(true);
			}

			/// <exception cref="System.Exception"></exception>
			/// <exception cref="Sharpen.ExecutionException"></exception>
			/// <exception cref="Apache.Http.Conn.ConnectionPoolTimeoutException"></exception>
			public HttpClientConnection Get(long timeout, TimeUnit tunit)
			{
				return this._enclosing.LeaseConnection(future, timeout, tunit);
			}

			private readonly PoolingHttpClientConnectionManager _enclosing;

			private readonly Future<CPoolEntry> future;
		}

		/// <exception cref="System.Exception"></exception>
		/// <exception cref="Sharpen.ExecutionException"></exception>
		/// <exception cref="Apache.Http.Conn.ConnectionPoolTimeoutException"></exception>
		protected internal virtual HttpClientConnection LeaseConnection(Future<CPoolEntry
			> future, long timeout, TimeUnit tunit)
		{
			CPoolEntry entry;
			try
			{
				entry = future.Get(timeout, tunit);
				if (entry == null || future.IsCancelled())
				{
					throw new Exception();
				}
				Asserts.Check(entry.GetConnection() != null, "Pool entry with no connection");
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Connection leased: " + Format(entry) + FormatStats(entry.GetRoute
						()));
				}
				return CPoolProxy.NewProxy(entry);
			}
			catch (TimeoutException)
			{
				throw new ConnectionPoolTimeoutException("Timeout waiting for connection from pool"
					);
			}
		}

		public virtual void ReleaseConnection(HttpClientConnection managedConn, object state
			, long keepalive, TimeUnit tunit)
		{
			Args.NotNull(managedConn, "Managed connection");
			lock (managedConn)
			{
				CPoolEntry entry = CPoolProxy.Detach(managedConn);
				if (entry == null)
				{
					return;
				}
				ManagedHttpClientConnection conn = entry.GetConnection();
				try
				{
					if (conn.IsOpen())
					{
						entry.SetState(state);
						entry.UpdateExpiry(keepalive, tunit != null ? tunit : TimeUnit.Milliseconds);
						if (this.log.IsDebugEnabled())
						{
							string s;
							if (keepalive > 0)
							{
								s = "for " + (double)keepalive / 1000 + " seconds";
							}
							else
							{
								s = "indefinitely";
							}
							this.log.Debug("Connection " + Format(entry) + " can be kept alive " + s);
						}
					}
				}
				finally
				{
					this.pool.Release(entry, conn.IsOpen() && entry.IsRouteComplete());
					if (this.log.IsDebugEnabled())
					{
						this.log.Debug("Connection released: " + Format(entry) + FormatStats(entry.GetRoute
							()));
					}
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Connect(HttpClientConnection managedConn, HttpRoute route, int
			 connectTimeout, HttpContext context)
		{
			Args.NotNull(managedConn, "Managed Connection");
			Args.NotNull(route, "HTTP route");
			ManagedHttpClientConnection conn;
			lock (managedConn)
			{
				CPoolEntry entry = CPoolProxy.GetPoolEntry(managedConn);
				conn = entry.GetConnection();
			}
			HttpHost host;
			if (route.GetProxyHost() != null)
			{
				host = route.GetProxyHost();
			}
			else
			{
				host = route.GetTargetHost();
			}
			IPEndPoint localAddress = route.GetLocalSocketAddress();
			SocketConfig socketConfig = this.configData.GetSocketConfig(host);
			if (socketConfig == null)
			{
				socketConfig = this.configData.GetDefaultSocketConfig();
			}
			if (socketConfig == null)
			{
				socketConfig = SocketConfig.Default;
			}
			this.connectionOperator.Connect(conn, host, localAddress, connectTimeout, socketConfig
				, context);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Upgrade(HttpClientConnection managedConn, HttpRoute route, HttpContext
			 context)
		{
			Args.NotNull(managedConn, "Managed Connection");
			Args.NotNull(route, "HTTP route");
			ManagedHttpClientConnection conn;
			lock (managedConn)
			{
				CPoolEntry entry = CPoolProxy.GetPoolEntry(managedConn);
				conn = entry.GetConnection();
			}
			this.connectionOperator.Upgrade(conn, route.GetTargetHost(), context);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void RouteComplete(HttpClientConnection managedConn, HttpRoute route
			, HttpContext context)
		{
			Args.NotNull(managedConn, "Managed Connection");
			Args.NotNull(route, "HTTP route");
			lock (managedConn)
			{
				CPoolEntry entry = CPoolProxy.GetPoolEntry(managedConn);
				entry.MarkRouteComplete();
			}
		}

		public virtual void Shutdown()
		{
			this.log.Debug("Connection manager is shutting down");
			try
			{
				this.pool.Shutdown();
			}
			catch (IOException ex)
			{
				this.log.Debug("I/O exception shutting down connection manager", ex);
			}
			this.log.Debug("Connection manager shut down");
		}

		public virtual void CloseIdleConnections(long idleTimeout, TimeUnit tunit)
		{
			if (this.log.IsDebugEnabled())
			{
				this.log.Debug("Closing connections idle longer than " + idleTimeout + " " + tunit
					);
			}
			this.pool.CloseIdle(idleTimeout, tunit);
		}

		public virtual void CloseExpiredConnections()
		{
			this.log.Debug("Closing expired connections");
			this.pool.CloseExpired();
		}

		public virtual int GetMaxTotal()
		{
			return this.pool.GetMaxTotal();
		}

		public virtual void SetMaxTotal(int max)
		{
			this.pool.SetMaxTotal(max);
		}

		public virtual int GetDefaultMaxPerRoute()
		{
			return this.pool.GetDefaultMaxPerRoute();
		}

		public virtual void SetDefaultMaxPerRoute(int max)
		{
			this.pool.SetDefaultMaxPerRoute(max);
		}

		public virtual int GetMaxPerRoute(HttpRoute route)
		{
			return this.pool.GetMaxPerRoute(route);
		}

		public virtual void SetMaxPerRoute(HttpRoute route, int max)
		{
			this.pool.SetMaxPerRoute(route, max);
		}

		public virtual PoolStats GetTotalStats()
		{
			return this.pool.GetTotalStats();
		}

		public virtual PoolStats GetStats(HttpRoute route)
		{
			return this.pool.GetStats(route);
		}

		public virtual SocketConfig GetDefaultSocketConfig()
		{
			return this.configData.GetDefaultSocketConfig();
		}

		public virtual void SetDefaultSocketConfig(SocketConfig defaultSocketConfig)
		{
			this.configData.SetDefaultSocketConfig(defaultSocketConfig);
		}

		public virtual ConnectionConfig GetDefaultConnectionConfig()
		{
			return this.configData.GetDefaultConnectionConfig();
		}

		public virtual void SetDefaultConnectionConfig(ConnectionConfig defaultConnectionConfig
			)
		{
			this.configData.SetDefaultConnectionConfig(defaultConnectionConfig);
		}

		public virtual SocketConfig GetSocketConfig(HttpHost host)
		{
			return this.configData.GetSocketConfig(host);
		}

		public virtual void SetSocketConfig(HttpHost host, SocketConfig socketConfig)
		{
			this.configData.SetSocketConfig(host, socketConfig);
		}

		public virtual ConnectionConfig GetConnectionConfig(HttpHost host)
		{
			return this.configData.GetConnectionConfig(host);
		}

		public virtual void SetConnectionConfig(HttpHost host, ConnectionConfig connectionConfig
			)
		{
			this.configData.SetConnectionConfig(host, connectionConfig);
		}

		internal class ConfigData
		{
			private readonly IDictionary<HttpHost, SocketConfig> socketConfigMap;

			private readonly IDictionary<HttpHost, ConnectionConfig> connectionConfigMap;

			private volatile SocketConfig defaultSocketConfig;

			private volatile ConnectionConfig defaultConnectionConfig;

			internal ConfigData() : base()
			{
				this.socketConfigMap = new ConcurrentHashMap<HttpHost, SocketConfig>();
				this.connectionConfigMap = new ConcurrentHashMap<HttpHost, ConnectionConfig>();
			}

			public virtual SocketConfig GetDefaultSocketConfig()
			{
				return this.defaultSocketConfig;
			}

			public virtual void SetDefaultSocketConfig(SocketConfig defaultSocketConfig)
			{
				this.defaultSocketConfig = defaultSocketConfig;
			}

			public virtual ConnectionConfig GetDefaultConnectionConfig()
			{
				return this.defaultConnectionConfig;
			}

			public virtual void SetDefaultConnectionConfig(ConnectionConfig defaultConnectionConfig
				)
			{
				this.defaultConnectionConfig = defaultConnectionConfig;
			}

			public virtual SocketConfig GetSocketConfig(HttpHost host)
			{
				return this.socketConfigMap.Get(host);
			}

			public virtual void SetSocketConfig(HttpHost host, SocketConfig socketConfig)
			{
				this.socketConfigMap.Put(host, socketConfig);
			}

			public virtual ConnectionConfig GetConnectionConfig(HttpHost host)
			{
				return this.connectionConfigMap.Get(host);
			}

			public virtual void SetConnectionConfig(HttpHost host, ConnectionConfig connectionConfig
				)
			{
				this.connectionConfigMap.Put(host, connectionConfig);
			}
		}

		internal class InternalConnectionFactory : ConnFactory<HttpRoute, ManagedHttpClientConnection
			>
		{
			private readonly PoolingHttpClientConnectionManager.ConfigData configData;

			private readonly HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> connFactory;

			internal InternalConnectionFactory(PoolingHttpClientConnectionManager.ConfigData 
				configData, HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> connFactory
				) : base()
			{
				this.configData = configData != null ? configData : new PoolingHttpClientConnectionManager.ConfigData
					();
				this.connFactory = connFactory != null ? connFactory : ManagedHttpClientConnectionFactory
					.Instance;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public virtual ManagedHttpClientConnection Create(HttpRoute route)
			{
				ConnectionConfig config = null;
				if (route.GetProxyHost() != null)
				{
					config = this.configData.GetConnectionConfig(route.GetProxyHost());
				}
				if (config == null)
				{
					config = this.configData.GetConnectionConfig(route.GetTargetHost());
				}
				if (config == null)
				{
					config = this.configData.GetDefaultConnectionConfig();
				}
				if (config == null)
				{
					config = ConnectionConfig.Default;
				}
				return this.connFactory.Create(route, config);
			}
		}
	}
}
