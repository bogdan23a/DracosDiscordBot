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
using Sharpen;

namespace Apache.Http.Client.Entity
{
	/// <summary>
	/// <see cref="Apache.Http.Entity.HttpEntityWrapper">Apache.Http.Entity.HttpEntityWrapper
	/// 	</see>
	/// responsible for handling
	/// deflate Content Coded responses. In RFC2616 terms, <code>deflate</code>
	/// means a <code>zlib</code> stream as defined in RFC1950. Some server
	/// implementations have misinterpreted RFC2616 to mean that a
	/// <code>deflate</code> stream as defined in RFC1951 should be used
	/// (or maybe they did that since that's how IE behaves?). It's confusing
	/// that <code>deflate</code> in HTTP 1.1 means <code>zlib</code> streams
	/// rather than <code>deflate</code> streams. We handle both types in here,
	/// since that's what is seen on the internet. Moral - prefer
	/// <code>gzip</code>!
	/// </summary>
	/// <seealso cref="GzipDecompressingEntity">GzipDecompressingEntity</seealso>
	/// <since>4.1</since>
	public class DeflateDecompressingEntity : DecompressingEntity
	{
		/// <summary>
		/// Creates a new
		/// <see cref="DeflateDecompressingEntity">DeflateDecompressingEntity</see>
		/// which will wrap the specified
		/// <see cref="Apache.Http.HttpEntity">Apache.Http.HttpEntity</see>
		/// .
		/// </summary>
		/// <param name="entity">
		/// a non-null
		/// <see cref="Apache.Http.HttpEntity">Apache.Http.HttpEntity</see>
		/// to be wrapped
		/// </param>
		public DeflateDecompressingEntity(HttpEntity entity) : base(entity)
		{
		}

		/// <summary>
		/// Returns the non-null InputStream that should be returned to by all requests to
		/// <see cref="DecompressingEntity.GetContent()">DecompressingEntity.GetContent()</see>
		/// .
		/// </summary>
		/// <returns>a non-null InputStream</returns>
		/// <exception cref="System.IO.IOException">if there was a problem</exception>
		internal override InputStream Decorate(InputStream wrapped)
		{
			return new DeflateInputStream(wrapped);
		}

		/// <summary><inheritDoc></inheritDoc></summary>
		public override Header GetContentEncoding()
		{
			return null;
		}

		/// <summary><inheritDoc></inheritDoc></summary>
		public override long GetContentLength()
		{
			return -1;
		}
	}
}
