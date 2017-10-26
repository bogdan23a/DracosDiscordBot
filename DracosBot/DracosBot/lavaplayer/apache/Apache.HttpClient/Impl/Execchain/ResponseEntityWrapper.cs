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
using System.Net.Sockets;
using Apache.Http;
using Apache.Http.Conn;
using Apache.Http.Entity;
using Apache.Http.Impl.Execchain;
using Sharpen;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>
	/// A wrapper class for
	/// <see cref="Apache.Http.HttpEntity">Apache.Http.HttpEntity</see>
	/// enclosed in a response message.
	/// </summary>
	/// <since>4.3</since>
	internal class ResponseEntityWrapper : HttpEntityWrapper, EofSensorWatcher
	{
		private readonly ConnectionHolder connReleaseTrigger;

		public ResponseEntityWrapper(HttpEntity entity, ConnectionHolder connReleaseTrigger
			) : base(entity)
		{
			this.connReleaseTrigger = connReleaseTrigger;
		}

		private void Cleanup()
		{
			if (this.connReleaseTrigger != null)
			{
				this.connReleaseTrigger.AbortConnection();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void ReleaseConnection()
		{
			if (this.connReleaseTrigger != null)
			{
				try
				{
					if (this.connReleaseTrigger.IsReusable())
					{
						this.connReleaseTrigger.ReleaseConnection();
					}
				}
				finally
				{
					Cleanup();
				}
			}
		}

		public override bool IsRepeatable()
		{
			return false;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override InputStream GetContent()
		{
			return new EofSensorInputStream(this.wrappedEntity.GetContent(), this);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[Obsolete]
		public override void ConsumeContent()
		{
			ReleaseConnection();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void WriteTo(OutputStream outstream)
		{
			try
			{
				this.wrappedEntity.WriteTo(outstream);
				ReleaseConnection();
			}
			finally
			{
				Cleanup();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual bool EofDetected(InputStream wrapped)
		{
			try
			{
				// there may be some cleanup required, such as
				// reading trailers after the response body:
				wrapped.Close();
				ReleaseConnection();
			}
			finally
			{
				Cleanup();
			}
			return false;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual bool StreamClosed(InputStream wrapped)
		{
			try
			{
				bool open = connReleaseTrigger != null && !connReleaseTrigger.IsReleased();
				// this assumes that closing the stream will
				// consume the remainder of the response body:
				try
				{
					wrapped.Close();
					ReleaseConnection();
				}
				catch (SocketException ex)
				{
					if (open)
					{
						throw;
					}
				}
			}
			finally
			{
				Cleanup();
			}
			return false;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual bool StreamAbort(InputStream wrapped)
		{
			Cleanup();
			return false;
		}
	}
}
