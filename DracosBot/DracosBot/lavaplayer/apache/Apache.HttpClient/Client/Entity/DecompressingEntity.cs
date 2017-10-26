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
using Apache.Http;
using Apache.Http.Client.Entity;
using Apache.Http.Entity;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Client.Entity
{
	/// <summary>
	/// Common base class for decompressing
	/// <see cref="Apache.Http.HttpEntity">Apache.Http.HttpEntity</see>
	/// implementations.
	/// </summary>
	/// <since>4.1</since>
	internal abstract class DecompressingEntity : HttpEntityWrapper
	{
		/// <summary>Default buffer size.</summary>
		/// <remarks>Default buffer size.</remarks>
		private const int BufferSize = 1024 * 2;

		/// <summary>
		/// <see cref="GetContent()">GetContent()</see>
		/// method must return the same
		/// <see cref="System.IO.InputStream">System.IO.InputStream</see>
		/// instance when DecompressingEntity is wrapping a streaming entity.
		/// </summary>
		private InputStream content;

		/// <summary>
		/// Creates a new
		/// <see cref="DecompressingEntity">DecompressingEntity</see>
		/// .
		/// </summary>
		/// <param name="wrapped">
		/// the non-null
		/// <see cref="Apache.Http.HttpEntity">Apache.Http.HttpEntity</see>
		/// to be wrapped
		/// </param>
		public DecompressingEntity(HttpEntity wrapped) : base(wrapped)
		{
		}

		/// <exception cref="System.IO.IOException"></exception>
		internal abstract InputStream Decorate(InputStream wrapped);

		/// <exception cref="System.IO.IOException"></exception>
		private InputStream GetDecompressingStream()
		{
			InputStream @in = wrappedEntity.GetContent();
			return new LazyDecompressingInputStream(@in, this);
		}

		/// <summary><inheritDoc></inheritDoc></summary>
		/// <exception cref="System.IO.IOException"></exception>
		public override InputStream GetContent()
		{
			if (wrappedEntity.IsStreaming())
			{
				if (content == null)
				{
					content = GetDecompressingStream();
				}
				return content;
			}
			else
			{
				return GetDecompressingStream();
			}
		}

		/// <summary><inheritDoc></inheritDoc></summary>
		/// <exception cref="System.IO.IOException"></exception>
		public override void WriteTo(OutputStream outstream)
		{
			Args.NotNull(outstream, "Output stream");
			InputStream instream = GetContent();
			try
			{
				byte[] buffer = new byte[BufferSize];
				int l;
				while ((l = instream.Read(buffer)) != -1)
				{
					outstream.Write(buffer, 0, l);
				}
			}
			finally
			{
				instream.Close();
			}
		}
	}
}
