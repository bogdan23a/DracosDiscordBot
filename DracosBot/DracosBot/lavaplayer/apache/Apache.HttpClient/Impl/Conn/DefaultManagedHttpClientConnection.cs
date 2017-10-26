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

using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Apache.Http;
using Apache.Http.Config;
using Apache.Http.Conn;
using Apache.Http.Entity;
using Apache.Http.IO;
using Apache.Http.Impl;
using Apache.Http.Protocol;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>
	/// Default
	/// <see cref="Apache.Http.Conn.ManagedHttpClientConnection">Apache.Http.Conn.ManagedHttpClientConnection
	/// 	</see>
	/// implementation.
	/// </summary>
	/// <since>4.3</since>
	public class DefaultManagedHttpClientConnection : DefaultBHttpClientConnection, ManagedHttpClientConnection
		, HttpContext
	{
		private readonly string id;

		private readonly IDictionary<string, object> attributes;

		private volatile bool shutdown;

		public DefaultManagedHttpClientConnection(string id, int buffersize, int fragmentSizeHint
			, CharsetDecoder chardecoder, CharsetEncoder charencoder, MessageConstraints constraints
			, ContentLengthStrategy incomingContentStrategy, ContentLengthStrategy outgoingContentStrategy
			, HttpMessageWriterFactory<IHttpRequest> requestWriterFactory, HttpMessageParserFactory
			<HttpResponse> responseParserFactory) : base(buffersize, fragmentSizeHint, chardecoder
			, charencoder, constraints, incomingContentStrategy, outgoingContentStrategy, requestWriterFactory
			, responseParserFactory)
		{
			this.id = id;
			this.attributes = new ConcurrentHashMap<string, object>();
		}

		public DefaultManagedHttpClientConnection(string id, int buffersize) : this(id, buffersize
			, buffersize, null, null, null, null, null, null, null)
		{
		}

		public virtual string GetId()
		{
			return this.id;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Shutdown()
		{
			this.shutdown = true;
			base.Shutdown();
		}

		public virtual object GetAttribute(string id)
		{
			return this.attributes.Get(id);
		}

		public virtual object RemoveAttribute(string id)
		{
			return Sharpen.Collections.Remove(this.attributes, id);
		}

		public virtual void SetAttribute(string id, object obj)
		{
			this.attributes.Put(id, obj);
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected override void Bind(Socket socket)
		{
			if (this.shutdown)
			{
				socket.Close();
				// allow this to throw...
				// ...but if it doesn't, explicitly throw one ourselves.
				throw new ThreadInterruptedException("Connection already shutdown");
			}
			base.Bind(socket);
		}

		protected override Socket GetSocket()
		{
			return base.GetSocket();
		}

		public virtual SSLSession GetSSLSession()
		{
			Socket socket = base.GetSocket();
			if (socket is SSLSocket)
			{
				return ((SSLSocket)socket).GetSession();
			}
			else
			{
				return null;
			}
		}
	}
}
