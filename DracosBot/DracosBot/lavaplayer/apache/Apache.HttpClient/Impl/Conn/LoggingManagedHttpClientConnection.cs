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
using System.Net.Sockets;
using Apache.Http;
using Apache.Http.Config;
using Apache.Http.Entity;
using Apache.Http.IO;
using Apache.Http.Impl.Conn;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	internal class LoggingManagedHttpClientConnection : DefaultManagedHttpClientConnection
	{
		private readonly Log log;

		private readonly Log headerlog;

		private readonly Wire wire;

		public LoggingManagedHttpClientConnection(string id, Log log, Log headerlog, Log 
			wirelog, int buffersize, int fragmentSizeHint, CharsetDecoder chardecoder, CharsetEncoder
			 charencoder, MessageConstraints constraints, ContentLengthStrategy incomingContentStrategy
			, ContentLengthStrategy outgoingContentStrategy, HttpMessageWriterFactory<IHttpRequest
			> requestWriterFactory, HttpMessageParserFactory<HttpResponse> responseParserFactory
			) : base(id, buffersize, fragmentSizeHint, chardecoder, charencoder, constraints
			, incomingContentStrategy, outgoingContentStrategy, requestWriterFactory, responseParserFactory
			)
		{
			this.log = log;
			this.headerlog = headerlog;
			this.wire = new Wire(wirelog, id);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Close()
		{
			if (this.log.IsDebugEnabled())
			{
				this.log.Debug(GetId() + ": Close connection");
			}
			base.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void Shutdown()
		{
			if (this.log.IsDebugEnabled())
			{
				this.log.Debug(GetId() + ": Shutdown connection");
			}
			base.Shutdown();
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected override InputStream GetSocketInputStream(Socket socket)
		{
			InputStream @in = base.GetSocketInputStream(socket);
			if (this.wire.Enabled())
			{
				@in = new LoggingInputStream(@in, this.wire);
			}
			return @in;
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected override OutputStream GetSocketOutputStream(Socket socket)
		{
			OutputStream @out = base.GetSocketOutputStream(socket);
			if (this.wire.Enabled())
			{
				@out = new LoggingOutputStream(@out, this.wire);
			}
			return @out;
		}

		protected override void OnResponseReceived(HttpResponse response)
		{
			if (response != null && this.headerlog.IsDebugEnabled())
			{
				this.headerlog.Debug(GetId() + " << " + response.GetStatusLine().ToString());
				Header[] headers = response.GetAllHeaders();
				foreach (Header header in headers)
				{
					this.headerlog.Debug(GetId() + " << " + header.ToString());
				}
			}
		}

		protected override void OnRequestSubmitted(IHttpRequest request)
		{
			if (request != null && this.headerlog.IsDebugEnabled())
			{
				this.headerlog.Debug(GetId() + " >> " + request.GetRequestLine().ToString());
				Header[] headers = request.GetAllHeaders();
				foreach (Header header in headers)
				{
					this.headerlog.Debug(GetId() + " >> " + header.ToString());
				}
			}
		}
	}
}
