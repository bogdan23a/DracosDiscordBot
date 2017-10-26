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
using Apache.Http.Entity;
using Apache.Http.Message;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Client.Entity
{
	/// <summary>
	/// Wrapping entity that compresses content when
	/// <see cref="WriteTo(System.IO.OutputStream)">writing</see>
	/// .
	/// </summary>
	/// <since>4.0</since>
	public class GzipCompressingEntity : HttpEntityWrapper
	{
		private const string GzipCodec = "gzip";

		public GzipCompressingEntity(HttpEntity entity) : base(entity)
		{
		}

		public override Header GetContentEncoding()
		{
			return new BasicHeader(HTTP.ContentEncoding, GzipCodec);
		}

		public override long GetContentLength()
		{
			return -1;
		}

		public override bool IsChunked()
		{
			// force content chunking
			return true;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override InputStream GetContent()
		{
			throw new NotSupportedException();
		}

		/// <exception cref="System.IO.IOException"></exception>
		public override void WriteTo(OutputStream outstream)
		{
			Args.NotNull(outstream, "Output stream");
			GZIPOutputStream gzip = new GZIPOutputStream(outstream);
			try
			{
				wrappedEntity.WriteTo(gzip);
			}
			finally
			{
				gzip.Close();
			}
		}
	}
}
