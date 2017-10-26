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

using System.Globalization;
using Apache.Http;
using Apache.Http.Client.Entity;
using Apache.Http.Client.Protocol;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Client.Protocol
{
	/// <summary>
	/// <see cref="Apache.Http.HttpResponseInterceptor">Apache.Http.HttpResponseInterceptor
	/// 	</see>
	/// responsible for processing Content-Encoding
	/// responses.
	/// <p>
	/// Instances of this class are stateless and immutable, therefore threadsafe.
	/// </summary>
	/// <since>4.1</since>
	public class ResponseContentEncoding : HttpResponseInterceptor
	{
		public const string Uncompressed = "http.client.response.uncompressed";

		/// <summary>
		/// Handles the following
		/// <code>Content-Encoding</code>
		/// s by
		/// using the appropriate decompressor to wrap the response Entity:
		/// <ul>
		/// <li>gzip - see
		/// <see cref="Apache.Http.Client.Entity.GzipDecompressingEntity">Apache.Http.Client.Entity.GzipDecompressingEntity
		/// 	</see>
		/// </li>
		/// <li>deflate - see
		/// <see cref="Apache.Http.Client.Entity.DeflateDecompressingEntity">Apache.Http.Client.Entity.DeflateDecompressingEntity
		/// 	</see>
		/// </li>
		/// <li>identity - no action needed</li>
		/// </ul>
		/// </summary>
		/// <param name="response">the response which contains the entity</param>
		/// <param name="context">not currently used</param>
		/// <exception cref="Apache.Http.HttpException">
		/// if the
		/// <code>Content-Encoding</code>
		/// is none of the above
		/// </exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Process(HttpResponse response, HttpContext context)
		{
			HttpEntity entity = response.GetEntity();
			// entity can be null in case of 304 Not Modified, 204 No Content or similar
			// check for zero length entity.
			if (entity != null && entity.GetContentLength() != 0)
			{
				Header ceheader = entity.GetContentEncoding();
				if (ceheader != null)
				{
					HeaderElement[] codecs = ceheader.GetElements();
					bool uncompressed = false;
					foreach (HeaderElement codec in codecs)
					{
						string codecname = codec.GetName().ToLower(CultureInfo.InvariantCulture);
						if ("gzip".Equals(codecname) || "x-gzip".Equals(codecname))
						{
							response.SetEntity(new GzipDecompressingEntity(response.GetEntity()));
							uncompressed = true;
							break;
						}
						else
						{
							if ("deflate".Equals(codecname))
							{
								response.SetEntity(new DeflateDecompressingEntity(response.GetEntity()));
								uncompressed = true;
								break;
							}
							else
							{
								if ("identity".Equals(codecname))
								{
									return;
								}
								else
								{
									throw new HttpException("Unsupported Content-Coding: " + codec.GetName());
								}
							}
						}
					}
					if (uncompressed)
					{
						response.RemoveHeaders("Content-Length");
						response.RemoveHeaders("Content-Encoding");
						response.RemoveHeaders("Content-MD5");
					}
				}
			}
		}
	}
}
