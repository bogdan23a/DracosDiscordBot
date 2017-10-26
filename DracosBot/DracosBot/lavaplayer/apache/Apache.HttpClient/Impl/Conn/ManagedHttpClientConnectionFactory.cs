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

using System.Text;
using Apache.Http;
using Apache.Http.Config;
using Apache.Http.Conn;
using Apache.Http.Conn.Routing;
using Apache.Http.IO;
using Apache.Http.Impl.Conn;
using Apache.Http.Impl.IO;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>
	/// Factory for
	/// <see cref="Apache.Http.Conn.ManagedHttpClientConnection">Apache.Http.Conn.ManagedHttpClientConnection
	/// 	</see>
	/// instances.
	/// </summary>
	/// <since>4.3</since>
	public class ManagedHttpClientConnectionFactory : HttpConnectionFactory<HttpRoute
		, ManagedHttpClientConnection>
	{
		private static readonly AtomicLong Counter = new AtomicLong();

		public static readonly Apache.Http.Impl.Conn.ManagedHttpClientConnectionFactory Instance
			 = new Apache.Http.Impl.Conn.ManagedHttpClientConnectionFactory();

		private readonly Log log = LogFactory.GetLog(typeof(DefaultManagedHttpClientConnection
			));

		private readonly Log headerlog = LogFactory.GetLog("org.apache.http.headers");

		private readonly Log wirelog = LogFactory.GetLog("org.apache.http.wire");

		private readonly HttpMessageWriterFactory<IHttpRequest> requestWriterFactory;

		private readonly HttpMessageParserFactory<HttpResponse> responseParserFactory;

		public ManagedHttpClientConnectionFactory(HttpMessageWriterFactory<IHttpRequest> 
			requestWriterFactory, HttpMessageParserFactory<HttpResponse> responseParserFactory
			) : base()
		{
			this.requestWriterFactory = requestWriterFactory != null ? requestWriterFactory : 
				DefaultHttpRequestWriterFactory.Instance;
			this.responseParserFactory = responseParserFactory != null ? responseParserFactory
				 : DefaultHttpResponseParserFactory.Instance;
		}

		public ManagedHttpClientConnectionFactory(HttpMessageParserFactory<HttpResponse> 
			responseParserFactory) : this(null, responseParserFactory)
		{
		}

		public ManagedHttpClientConnectionFactory() : this(null, null)
		{
		}

		public virtual ManagedHttpClientConnection Create(HttpRoute route, ConnectionConfig
			 config)
		{
			ConnectionConfig cconfig = config != null ? config : ConnectionConfig.Default;
			CharsetDecoder chardecoder = null;
			CharsetEncoder charencoder = null;
			Encoding charset = cconfig.GetCharset();
			CodingErrorAction malformedInputAction = cconfig.GetMalformedInputAction() != null
				 ? cconfig.GetMalformedInputAction() : CodingErrorAction.Report;
			CodingErrorAction unmappableInputAction = cconfig.GetUnmappableInputAction() != null
				 ? cconfig.GetUnmappableInputAction() : CodingErrorAction.Report;
			if (charset != null)
			{
				chardecoder = charset.NewDecoder();
				chardecoder.OnMalformedInput(malformedInputAction);
				chardecoder.OnUnmappableCharacter(unmappableInputAction);
				charencoder = charset.NewEncoder();
				charencoder.OnMalformedInput(malformedInputAction);
				charencoder.OnUnmappableCharacter(unmappableInputAction);
			}
			string id = "http-outgoing-" + System.Convert.ToString(Counter.GetAndIncrement());
			return new LoggingManagedHttpClientConnection(id, log, headerlog, wirelog, cconfig
				.GetBufferSize(), cconfig.GetFragmentSizeHint(), chardecoder, charencoder, cconfig
				.GetMessageConstraints(), null, null, requestWriterFactory, responseParserFactory
				);
		}
	}
}
