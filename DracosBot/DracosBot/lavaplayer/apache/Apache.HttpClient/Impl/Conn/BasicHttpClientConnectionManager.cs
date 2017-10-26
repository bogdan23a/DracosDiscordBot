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
using Apache.Http.Config;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.Conn.Socket;
using Apache.Http.Conn.Ssl;
using Apache.Http.Impl.Conn;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>A connection manager for a single connection.</summary>
	/// <remarks>
	/// A connection manager for a single connection. This connection manager maintains only one active
	/// connection. Even though this class is fully thread-safe it ought to be used by one execution
	/// thread only, as only one thread a time can lease the connection at a time.
	/// <p/>
	/// This connection manager will make an effort to reuse the connection for subsequent requests
	/// with the same
	/// <see cref="Apache.Http.Conn.Routing.HttpRoute">route</see>
	/// . It will, however, close the existing connection and
	/// open it for the given route, if the route of the persistent connection does not match that
	/// of the connection request. If the connection has been already been allocated
	/// <see cref="System.InvalidOperationException">System.InvalidOperationException</see>
	/// is thrown.
	/// <p/>
	/// This connection manager implementation should be used inside an EJB container instead of
	/// <see cref="PoolingHttpClientConnectionManager">PoolingHttpClientConnectionManager
	/// 	</see>
	/// .
	/// </remarks>
	/// <since>4.3</since>
	public class BasicHttpClientConnectionManager : HttpClientConnectionManager, IDisposable
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly HttpClientConnectionOperator connectionOperator;

		private readonly HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> connFactory;

		private ManagedHttpClientConnection conn;

		private HttpRoute route;

		private object state;

		private long updated;

		private long expiry;

		private bool leased;

		private SocketConfig socketConfig;

		private ConnectionConfig connConfig;

		private volatile bool shutdown;

		private static Registry<ConnectionSocketFactory> GetDefaultRegistry()
		{
			return RegistryBuilder.Create<ConnectionSocketFactory>().Register("http", PlainConnectionSocketFactory
				.GetSocketFactory()).Register("https", SSLConnectionSocketFactory.GetSocketFactory
				()).Build();
		}

		public BasicHttpClientConnectionManager(Lookup<ConnectionSocketFactory> socketFactoryRegistry
			, HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> connFactory, SchemePortResolver
			 schemePortResolver, DnsResolver dnsResolver) : base()
		{
			this.connectionOperator = new HttpClientConnectionOperator(socketFactoryRegistry, 
				schemePortResolver, dnsResolver);
			this.connFactory = connFactory != null ? connFactory : ManagedHttpClientConnectionFactory
				.Instance;
			this.expiry = long.MaxValue;
			this.socketConfig = SocketConfig.Default;
			this.connConfig = ConnectionConfig.Default;
		}

		public BasicHttpClientConnectionManager(Lookup<ConnectionSocketFactory> socketFactoryRegistry
			, HttpConnectionFactory<HttpRoute, ManagedHttpClientConnection> connFactory) : this
			(socketFactoryRegistry, connFactory, null, null)
		{
		}

		public BasicHttpClientConnectionManager(Lookup<ConnectionSocketFactory> socketFactoryRegistry
			) : this(socketFactoryRegistry, null, null, null)
		{
		}

		public BasicHttpClientConnectionManager() : this(GetDefaultRegistry(), null, null
			, null)
		{
		}

		~BasicHttpClientConnectionManager()
		{
			try
			{
				Shutdown();
			}
			finally
			{
				// Make sure we call overridden method even if shutdown barfs
				base.Finalize();
			}
		}

		public virtual void Close()
		{
			Shutdown();
		}

		internal virtual HttpRoute GetRoute()
		{
			return route;
		}

		internal virtual object GetState()
		{
			return state;
		}

		public virtual SocketConfig GetSocketConfig()
		{
			lock (this)
			{
				return socketConfig;
			}
		}

		public virtual void SetSocketConfig(SocketConfig socketConfig)
		{
			lock (this)
			{
				this.socketConfig = socketConfig != null ? socketConfig : SocketConfig.Default;
			}
		}

		public virtual ConnectionConfig GetConnectionConfig()
		{
			lock (this)
			{
				return connConfig;
			}
		}

		public virtual void SetConnectionConfig(ConnectionConfig connConfig)
		{
			lock (this)
			{
				this.connConfig = connConfig != null ? connConfig : ConnectionConfig.Default;
			}
		}

		public ConnectionRequest RequestConnection(HttpRoute route, object state)
		{
			Args.NotNull(route, "Route");
			return new _ConnectionRequest_190(this, route, state);
		}

		private sealed class _ConnectionRequest_190 : ConnectionRequest
		{
			public _ConnectionRequest_190(BasicHttpClientConnectionManager _enclosing, HttpRoute
				 route, object state)
			{
				this._enclosing = _enclosing;
				this.route = route;
				this.state = state;
			}

			public bool Cancel()
			{
				// Nothing to abort, since requests are immediate.
				return false;
			}

			public HttpClientConnection Get(long timeout, TimeUnit tunit)
			{
				return this._enclosing.GetConnection(route, state);
			}

			private readonly BasicHttpClientConnectionManager _enclosing;

			private readonly HttpRoute route;

			private readonly object state;
		}

		private void CloseConnection()
		{
			if (this.conn != null)
			{
				this.log.Debug("Closing connection");
				try
				{
					this.conn.Close();
				}
				catch (IOException iox)
				{
					if (this.log.IsDebugEnabled())
					{
						this.log.Debug("I/O exception closing connection", iox);
					}
				}
				this.conn = null;
			}
		}

		private void ShutdownConnection()
		{
			if (this.conn != null)
			{
				this.log.Debug("Shutting down connection");
				try
				{
					this.conn.Shutdown();
				}
				catch (IOException iox)
				{
					if (this.log.IsDebugEnabled())
					{
						this.log.Debug("I/O exception shutting down connection", iox);
					}
				}
				this.conn = null;
			}
		}

		private void CheckExpiry()
		{
			if (this.conn != null && Runtime.CurrentTimeMillis() >= this.expiry)
			{
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Connection expired @ " + Sharpen.Extensions.CreateDate(this.expiry
						));
				}
				CloseConnection();
			}
		}

		internal virtual HttpClientConnection GetConnection(HttpRoute route, object state
			)
		{
			lock (this)
			{
				Asserts.Check(!this.shutdown, "Connection manager has been shut down");
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Get connection for route " + route);
				}
				Asserts.Check(!this.leased, "Connection is still allocated");
				if (!LangUtils.Equals(this.route, route) || !LangUtils.Equals(this.state, state))
				{
					CloseConnection();
				}
				this.route = route;
				this.state = state;
				CheckExpiry();
				if (this.conn == null)
				{
					this.conn = this.connFactory.Create(route, this.connConfig);
				}
				this.leased = true;
				return this.conn;
			}
		}

		public virtual void ReleaseConnection(HttpClientConnection conn, object state, long
			 keepalive, TimeUnit tunit)
		{
			lock (this)
			{
				Args.NotNull(conn, "Connection");
				Asserts.Check(conn == this.conn, "Connection not obtained from this manager");
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Releasing connection " + conn);
				}
				if (this.shutdown)
				{
					ShutdownConnection();
					return;
				}
				try
				{
					this.updated = Runtime.CurrentTimeMillis();
					if (!this.conn.IsOpen())
					{
						this.conn = null;
						this.route = null;
						this.conn = null;
						this.expiry = long.MaxValue;
					}
					else
					{
						this.state = state;
						if (this.log.IsDebugEnabled())
						{
							string s;
							if (keepalive > 0)
							{
								s = "for " + keepalive + " " + tunit;
							}
							else
							{
								s = "indefinitely";
							}
							this.log.Debug("Connection can be kept alive " + s);
						}
						if (keepalive > 0)
						{
							this.expiry = this.updated + tunit.ToMillis(keepalive);
						}
						else
						{
							this.expiry = long.MaxValue;
						}
					}
				}
				finally
				{
					this.leased = false;
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Connect(HttpClientConnection conn, HttpRoute route, int connectTimeout
			, HttpContext context)
		{
			Args.NotNull(conn, "Connection");
			Args.NotNull(route, "HTTP route");
			Asserts.Check(conn == this.conn, "Connection not obtained from this manager");
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
			this.connectionOperator.Connect(this.conn, host, localAddress, connectTimeout, this
				.socketConfig, context);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Upgrade(HttpClientConnection conn, HttpRoute route, HttpContext
			 context)
		{
			Args.NotNull(conn, "Connection");
			Args.NotNull(route, "HTTP route");
			Asserts.Check(conn == this.conn, "Connection not obtained from this manager");
			this.connectionOperator.Upgrade(this.conn, route.GetTargetHost(), context);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void RouteComplete(HttpClientConnection conn, HttpRoute route, HttpContext
			 context)
		{
		}

		public virtual void CloseExpiredConnections()
		{
			lock (this)
			{
				if (this.shutdown)
				{
					return;
				}
				if (!this.leased)
				{
					CheckExpiry();
				}
			}
		}

		public virtual void CloseIdleConnections(long idletime, TimeUnit tunit)
		{
			lock (this)
			{
				Args.NotNull(tunit, "Time unit");
				if (this.shutdown)
				{
					return;
				}
				if (!this.leased)
				{
					long time = tunit.ToMillis(idletime);
					if (time < 0)
					{
						time = 0;
					}
					long deadline = Runtime.CurrentTimeMillis() - time;
					if (this.updated <= deadline)
					{
						CloseConnection();
					}
				}
			}
		}

		public virtual void Shutdown()
		{
			lock (this)
			{
				if (this.shutdown)
				{
					return;
				}
				this.shutdown = true;
				ShutdownConnection();
			}
		}
	}
}
