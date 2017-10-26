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
using System.Reflection;
using Apache.Http;
using Apache.Http.Conn;
using Apache.Http.Impl.Conn;
using Apache.Http.Protocol;
using Sharpen;
using Sharpen.Reflect;

namespace Apache.Http.Impl.Conn
{
	/// <since>4.3</since>
	internal class CPoolProxy : InvocationHandler
	{
		private static readonly MethodInfo CloseMethod;

		private static readonly MethodInfo ShutdownMethod;

		private static readonly MethodInfo IsOpenMethod;

		private static readonly MethodInfo IsStaleMethod;

		static CPoolProxy()
		{
			try
			{
				CloseMethod = typeof(HttpConnection).GetMethod("close");
				ShutdownMethod = typeof(HttpConnection).GetMethod("shutdown");
				IsOpenMethod = typeof(HttpConnection).GetMethod("isOpen");
				IsStaleMethod = typeof(HttpConnection).GetMethod("isStale");
			}
			catch (NoSuchMethodException ex)
			{
				throw new Error(ex);
			}
		}

		private volatile CPoolEntry poolEntry;

		internal CPoolProxy(CPoolEntry entry) : base()
		{
			this.poolEntry = entry;
		}

		internal virtual CPoolEntry GetPoolEntry()
		{
			return this.poolEntry;
		}

		internal virtual CPoolEntry Detach()
		{
			CPoolEntry local = this.poolEntry;
			this.poolEntry = null;
			return local;
		}

		internal virtual HttpClientConnection GetConnection()
		{
			CPoolEntry local = this.poolEntry;
			if (local == null)
			{
				return null;
			}
			return local.GetConnection();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Close()
		{
			CPoolEntry local = this.poolEntry;
			if (local != null)
			{
				local.CloseConnection();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Shutdown()
		{
			CPoolEntry local = this.poolEntry;
			if (local != null)
			{
				local.ShutdownConnection();
			}
		}

		public virtual bool IsOpen()
		{
			CPoolEntry local = this.poolEntry;
			if (local != null)
			{
				return !local.IsClosed();
			}
			else
			{
				return false;
			}
		}

		public virtual bool IsStale()
		{
			HttpClientConnection conn = GetConnection();
			if (conn != null)
			{
				return conn.IsStale();
			}
			else
			{
				return true;
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual object Invoke(object proxy, MethodInfo method, object[] args)
		{
			if (method.Equals(CloseMethod))
			{
				Close();
				return null;
			}
			else
			{
				if (method.Equals(ShutdownMethod))
				{
					Shutdown();
					return null;
				}
				else
				{
					if (method.Equals(IsOpenMethod))
					{
						return Sharpen.Extensions.ValueOf(IsOpen());
					}
					else
					{
						if (method.Equals(IsStaleMethod))
						{
							return Sharpen.Extensions.ValueOf(IsStale());
						}
						else
						{
							HttpClientConnection conn = GetConnection();
							if (conn == null)
							{
								throw new ConnectionShutdownException();
							}
							try
							{
								return method.Invoke(conn, args);
							}
							catch (TargetInvocationException ex)
							{
								Exception cause = ex.InnerException;
								if (cause != null)
								{
									throw cause;
								}
								else
								{
									throw;
								}
							}
						}
					}
				}
			}
		}

		public static HttpClientConnection NewProxy(CPoolEntry poolEntry)
		{
			return (HttpClientConnection)Proxy.NewProxyInstance(typeof(Apache.Http.Impl.Conn.CPoolProxy
				).GetClassLoader(), new Type[] { typeof(ManagedHttpClientConnection), typeof(HttpContext
				) }, new Apache.Http.Impl.Conn.CPoolProxy(poolEntry));
		}

		private static Apache.Http.Impl.Conn.CPoolProxy GetHandler(HttpClientConnection proxy
			)
		{
			InvocationHandler handler = Proxy.GetInvocationHandler(proxy);
			if (!typeof(Apache.Http.Impl.Conn.CPoolProxy).IsInstanceOfType(handler))
			{
				throw new InvalidOperationException("Unexpected proxy handler class: " + handler);
			}
			return typeof(Apache.Http.Impl.Conn.CPoolProxy).Cast(handler);
		}

		public static CPoolEntry GetPoolEntry(HttpClientConnection proxy)
		{
			CPoolEntry entry = GetHandler(proxy).GetPoolEntry();
			if (entry == null)
			{
				throw new ConnectionShutdownException();
			}
			return entry;
		}

		public static CPoolEntry Detach(HttpClientConnection proxy)
		{
			return GetHandler(proxy).Detach();
		}
	}
}
