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
using Apache.Http;
using Apache.Http.Config;
using Apache.Http.IO;
using Apache.Http.Impl;
using Apache.Http.Impl.IO;
using Apache.Http.Message;
using Apache.Http.Params;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>
	/// Lenient HTTP response parser implementation that can skip malformed data until
	/// a valid HTTP response message head is encountered.
	/// </summary>
	/// <remarks>
	/// Lenient HTTP response parser implementation that can skip malformed data until
	/// a valid HTTP response message head is encountered.
	/// </remarks>
	/// <since>4.2</since>
	public class DefaultHttpResponseParser : AbstractMessageParser<HttpResponse>
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly HttpResponseFactory responseFactory;

		private readonly CharArrayBuffer lineBuf;

		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) use DefaultHttpResponseParser(Apache.Http.IO.SessionInputBuffer, Apache.Http.Message.LineParser, Apache.Http.HttpResponseFactory, Apache.Http.Config.MessageConstraints)"
			)]
		public DefaultHttpResponseParser(SessionInputBuffer buffer, LineParser parser, HttpResponseFactory
			 responseFactory, HttpParams @params) : base(buffer, parser, @params)
		{
			Args.NotNull(responseFactory, "Response factory");
			this.responseFactory = responseFactory;
			this.lineBuf = new CharArrayBuffer(128);
		}

		/// <summary>Creates new instance of DefaultHttpResponseParser.</summary>
		/// <remarks>Creates new instance of DefaultHttpResponseParser.</remarks>
		/// <param name="buffer">the session input buffer.</param>
		/// <param name="lineParser">
		/// the line parser. If <code>null</code>
		/// <see cref="Apache.Http.Message.BasicLineParser.Instance">Apache.Http.Message.BasicLineParser.Instance
		/// 	</see>
		/// will be used.
		/// </param>
		/// <param name="responseFactory">
		/// HTTP response factory. If <code>null</code>
		/// <see cref="Apache.Http.Impl.DefaultHttpResponseFactory.Instance">Apache.Http.Impl.DefaultHttpResponseFactory.Instance
		/// 	</see>
		/// will be used.
		/// </param>
		/// <param name="constraints">
		/// the message constraints. If <code>null</code>
		/// <see cref="Apache.Http.Config.MessageConstraints.Default">Apache.Http.Config.MessageConstraints.Default
		/// 	</see>
		/// will be used.
		/// </param>
		/// <since>4.3</since>
		public DefaultHttpResponseParser(SessionInputBuffer buffer, LineParser lineParser
			, HttpResponseFactory responseFactory, MessageConstraints constraints) : base(buffer
			, lineParser, constraints)
		{
			this.responseFactory = responseFactory != null ? responseFactory : DefaultHttpResponseFactory
				.Instance;
			this.lineBuf = new CharArrayBuffer(128);
		}

		/// <summary>Creates new instance of DefaultHttpResponseParser.</summary>
		/// <remarks>Creates new instance of DefaultHttpResponseParser.</remarks>
		/// <param name="buffer">the session input buffer.</param>
		/// <param name="constraints">
		/// the message constraints. If <code>null</code>
		/// <see cref="Apache.Http.Config.MessageConstraints.Default">Apache.Http.Config.MessageConstraints.Default
		/// 	</see>
		/// will be used.
		/// </param>
		/// <since>4.3</since>
		public DefaultHttpResponseParser(SessionInputBuffer buffer, MessageConstraints constraints
			) : this(buffer, null, null, constraints)
		{
		}

		/// <summary>Creates new instance of DefaultHttpResponseParser.</summary>
		/// <remarks>Creates new instance of DefaultHttpResponseParser.</remarks>
		/// <param name="buffer">the session input buffer.</param>
		/// <since>4.3</since>
		public DefaultHttpResponseParser(SessionInputBuffer buffer) : this(buffer, null, 
			null, MessageConstraints.Default)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.HttpException"></exception>
		protected override HttpResponse ParseHead(SessionInputBuffer sessionBuffer)
		{
			//read out the HTTP status string
			int count = 0;
			ParserCursor cursor = null;
			do
			{
				// clear the buffer
				this.lineBuf.Clear();
				int i = sessionBuffer.ReadLine(this.lineBuf);
				if (i == -1 && count == 0)
				{
					// The server just dropped connection on us
					throw new NoHttpResponseException("The target server failed to respond");
				}
				cursor = new ParserCursor(0, this.lineBuf.Length());
				if (lineParser.HasProtocolVersion(this.lineBuf, cursor))
				{
					// Got one
					break;
				}
				else
				{
					if (i == -1 || Reject(this.lineBuf, count))
					{
						// Giving up
						throw new ProtocolException("The server failed to respond with a " + "valid HTTP response"
							);
					}
				}
				if (this.log.IsDebugEnabled())
				{
					this.log.Debug("Garbage in response: " + this.lineBuf.ToString());
				}
				count++;
			}
			while (true);
			//create the status line from the status string
			StatusLine statusline = lineParser.ParseStatusLine(this.lineBuf, cursor);
			return this.responseFactory.NewHttpResponse(statusline, null);
		}

		protected internal virtual bool Reject(CharArrayBuffer line, int count)
		{
			return false;
		}
	}
}
