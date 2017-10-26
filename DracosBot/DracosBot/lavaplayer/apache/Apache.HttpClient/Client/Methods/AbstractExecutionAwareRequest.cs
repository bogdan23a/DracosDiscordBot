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
using Apache.Http.Client.Methods;
using Apache.Http.Client.Utils;
using Apache.Http.Concurrent;
using Apache.Http.Conn;
using Apache.Http.Message;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Client.Methods
{
	public abstract class AbstractExecutionAwareRequest : AbstractHttpMessage, HttpExecutionAware
		, AbortableHttpRequest, ICloneable, IHttpRequest
	{
		private Lock abortLock;

		private volatile bool aborted;

		private volatile Cancellable cancellable;

		protected internal AbstractExecutionAwareRequest() : base()
		{
			this.abortLock = new ReentrantLock();
		}

		[Obsolete]
		public virtual void SetConnectionRequest(ClientConnectionRequest connRequest)
		{
			if (this.aborted)
			{
				return;
			}
			this.abortLock.Lock();
			try
			{
				this.cancellable = new _Cancellable_60(connRequest);
			}
			finally
			{
				this.abortLock.Unlock();
			}
		}

		private sealed class _Cancellable_60 : Cancellable
		{
			public _Cancellable_60(ClientConnectionRequest connRequest)
			{
				this.connRequest = connRequest;
			}

			public bool Cancel()
			{
				connRequest.AbortRequest();
				return true;
			}

			private readonly ClientConnectionRequest connRequest;
		}

		[Obsolete]
		public virtual void SetReleaseTrigger(ConnectionReleaseTrigger releaseTrigger)
		{
			if (this.aborted)
			{
				return;
			}
			this.abortLock.Lock();
			try
			{
				this.cancellable = new _Cancellable_80(releaseTrigger);
			}
			finally
			{
				this.abortLock.Unlock();
			}
		}

		private sealed class _Cancellable_80 : Cancellable
		{
			public _Cancellable_80(ConnectionReleaseTrigger releaseTrigger)
			{
				this.releaseTrigger = releaseTrigger;
			}

			public bool Cancel()
			{
				try
				{
					releaseTrigger.AbortConnection();
					return true;
				}
				catch (IOException)
				{
					return false;
				}
			}

			private readonly ConnectionReleaseTrigger releaseTrigger;
		}

		private void CancelExecution()
		{
			if (this.cancellable != null)
			{
				this.cancellable.Cancel();
				this.cancellable = null;
			}
		}

		public virtual void Abort()
		{
			if (this.aborted)
			{
				return;
			}
			this.abortLock.Lock();
			try
			{
				this.aborted = true;
				CancelExecution();
			}
			finally
			{
				this.abortLock.Unlock();
			}
		}

		public virtual bool IsAborted()
		{
			return this.aborted;
		}

		/// <since>4.2</since>
		public virtual void SetCancellable(Cancellable cancellable)
		{
			if (this.aborted)
			{
				return;
			}
			this.abortLock.Lock();
			try
			{
				this.cancellable = cancellable;
			}
			finally
			{
				this.abortLock.Unlock();
			}
		}

		/// <exception cref="Sharpen.CloneNotSupportedException"></exception>
		public virtual object Clone()
		{
			Apache.Http.Client.Methods.AbstractExecutionAwareRequest clone = (Apache.Http.Client.Methods.AbstractExecutionAwareRequest
				)base.Clone();
			clone.headergroup = CloneUtils.CloneObject(this.headergroup);
			clone.@params = CloneUtils.CloneObject(this.@params);
			clone.abortLock = new ReentrantLock();
			clone.cancellable = null;
			clone.aborted = false;
			return clone;
		}

		/// <since>4.2</since>
		public virtual void Completed()
		{
			this.abortLock.Lock();
			try
			{
				this.cancellable = null;
			}
			finally
			{
				this.abortLock.Unlock();
			}
		}

		/// <summary>Resets internal state of the request making it reusable.</summary>
		/// <remarks>Resets internal state of the request making it reusable.</remarks>
		/// <since>4.2</since>
		public virtual void Reset()
		{
			this.abortLock.Lock();
			try
			{
				CancelExecution();
				this.aborted = false;
			}
			finally
			{
				this.abortLock.Unlock();
			}
		}

		public abstract ProtocolVersion GetProtocolVersion();

		public abstract RequestLine GetRequestLine();
	}
}
