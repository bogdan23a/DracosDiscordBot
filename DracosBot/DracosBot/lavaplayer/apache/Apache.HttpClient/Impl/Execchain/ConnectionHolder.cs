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
using Apache.Http;
using Apache.Http.Concurrent;
using Apache.Http.Conn;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>Internal connection holder.</summary>
	/// <remarks>Internal connection holder.</remarks>
	/// <since>4.3</since>
	internal class ConnectionHolder : ConnectionReleaseTrigger, Cancellable, IDisposable
	{
		private readonly Log log;

		private readonly HttpClientConnectionManager manager;

		private readonly HttpClientConnection managedConn;

		private volatile bool reusable;

		private volatile object state;

		private volatile long validDuration;

		private volatile TimeUnit tunit;

		private volatile bool released;

		public ConnectionHolder(Log log, HttpClientConnectionManager manager, HttpClientConnection
			 managedConn) : base()
		{
			this.log = log;
			this.manager = manager;
			this.managedConn = managedConn;
		}

		public virtual bool IsReusable()
		{
			return this.reusable;
		}

		public virtual void MarkReusable()
		{
			this.reusable = true;
		}

		public virtual void MarkNonReusable()
		{
			this.reusable = false;
		}

		public virtual void SetState(object state)
		{
			this.state = state;
		}

		public virtual void SetValidFor(long duration, TimeUnit tunit)
		{
			lock (this.managedConn)
			{
				this.validDuration = duration;
				this.tunit = tunit;
			}
		}

		public virtual void ReleaseConnection()
		{
			lock (this.managedConn)
			{
				if (this.released)
				{
					return;
				}
				this.released = true;
				if (this.reusable)
				{
					this.manager.ReleaseConnection(this.managedConn, this.state, this.validDuration, 
						this.tunit);
				}
				else
				{
					try
					{
						this.managedConn.Close();
						log.Debug("Connection discarded");
					}
					catch (IOException ex)
					{
						if (this.log.IsDebugEnabled())
						{
							this.log.Debug(ex.Message, ex);
						}
					}
					finally
					{
						this.manager.ReleaseConnection(this.managedConn, null, 0, TimeUnit.Milliseconds);
					}
				}
			}
		}

		public virtual void AbortConnection()
		{
			lock (this.managedConn)
			{
				if (this.released)
				{
					return;
				}
				this.released = true;
				try
				{
					this.managedConn.Shutdown();
					log.Debug("Connection discarded");
				}
				catch (IOException ex)
				{
					if (this.log.IsDebugEnabled())
					{
						this.log.Debug(ex.Message, ex);
					}
				}
				finally
				{
					this.manager.ReleaseConnection(this.managedConn, null, 0, TimeUnit.Milliseconds);
				}
			}
		}

		public virtual bool Cancel()
		{
			bool alreadyReleased = this.released;
			log.Debug("Cancelling request execution");
			AbortConnection();
			return !alreadyReleased;
		}

		public virtual bool IsReleased()
		{
			return this.released;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Close()
		{
			AbortConnection();
		}
	}
}
