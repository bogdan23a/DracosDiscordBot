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
using System.IO;
using Apache.Http;
using Apache.Http.Client.Entity;
using Apache.Http.Entity;
using Sharpen;

namespace Apache.Http.Client.Entity
{
	/// <summary>
	/// Builder for
	/// <see cref="Apache.Http.HttpEntity">Apache.Http.HttpEntity</see>
	/// instances.
	/// <p/>
	/// Several setter methods of this builder are mutually exclusive. In case of multiple invocations
	/// of the following methods only the last one will have effect:
	/// <ul>
	/// <li>
	/// <see cref="SetText(string)">SetText(string)</see>
	/// </li>
	/// <li>
	/// <see cref="SetBinary(byte[])">SetBinary(byte[])</see>
	/// </li>
	/// <li>
	/// <see cref="SetStream(System.IO.InputStream)">SetStream(System.IO.InputStream)</see>
	/// </li>
	/// <li>
	/// <see cref="SetSerializable(System.IO.Serializable)">SetSerializable(System.IO.Serializable)
	/// 	</see>
	/// </li>
	/// <li>
	/// <see cref="SetParameters(System.Collections.Generic.IList{E})">SetParameters(System.Collections.Generic.IList&lt;E&gt;)
	/// 	</see>
	/// </li>
	/// <li>
	/// <see cref="SetParameters(Apache.Http.NameValuePair[])">SetParameters(Apache.Http.NameValuePair[])
	/// 	</see>
	/// </li>
	/// <li>
	/// <see cref="SetFile(Sharpen.FilePath)">SetFile(Sharpen.FilePath)</see>
	/// </li>
	/// </ul>
	/// </summary>
	/// <since>4.3</since>
	public class EntityBuilder
	{
		private string text;

		private byte[] binary;

		private InputStream stream;

		private IList<NameValuePair> parameters;

		private Serializable serializable;

		private FilePath file;

		private ContentType contentType;

		private string contentEncoding;

		private bool chunked;

		private bool gzipCompress;

		internal EntityBuilder() : base()
		{
		}

		public static Apache.Http.Client.Entity.EntityBuilder Create()
		{
			return new Apache.Http.Client.Entity.EntityBuilder();
		}

		private void ClearContent()
		{
			this.text = null;
			this.binary = null;
			this.stream = null;
			this.parameters = null;
			this.serializable = null;
			this.file = null;
		}

		/// <summary>
		/// Returns entity content as a string if set using
		/// <see cref="SetText(string)">SetText(string)</see>
		/// method.
		/// </summary>
		public virtual string GetText()
		{
			return text;
		}

		/// <summary>Sets entity content as a string.</summary>
		/// <remarks>
		/// Sets entity content as a string. This method is mutually exclusive with
		/// <see cref="SetBinary(byte[])">SetBinary(byte[])</see>
		/// ,
		/// <see cref="SetStream(System.IO.InputStream)">SetStream(System.IO.InputStream)</see>
		/// ,
		/// <see cref="SetSerializable(System.IO.Serializable)">SetSerializable(System.IO.Serializable)
		/// 	</see>
		/// ,
		/// <see cref="SetParameters(System.Collections.Generic.IList{E})">SetParameters(System.Collections.Generic.IList&lt;E&gt;)
		/// 	</see>
		/// ,
		/// <see cref="SetParameters(Apache.Http.NameValuePair[])">SetParameters(Apache.Http.NameValuePair[])
		/// 	</see>
		/// <see cref="SetFile(Sharpen.FilePath)">SetFile(Sharpen.FilePath)</see>
		/// methods.
		/// </remarks>
		public virtual Apache.Http.Client.Entity.EntityBuilder SetText(string text)
		{
			ClearContent();
			this.text = text;
			return this;
		}

		/// <summary>
		/// Returns entity content as a byte array if set using
		/// <see cref="SetBinary(byte[])">SetBinary(byte[])</see>
		/// method.
		/// </summary>
		public virtual byte[] GetBinary()
		{
			return binary;
		}

		/// <summary>Sets entity content as a byte array.</summary>
		/// <remarks>
		/// Sets entity content as a byte array. This method is mutually exclusive with
		/// <see cref="SetText(string)">SetText(string)</see>
		/// ,
		/// <see cref="SetStream(System.IO.InputStream)">SetStream(System.IO.InputStream)</see>
		/// ,
		/// <see cref="SetSerializable(System.IO.Serializable)">SetSerializable(System.IO.Serializable)
		/// 	</see>
		/// ,
		/// <see cref="SetParameters(System.Collections.Generic.IList{E})">SetParameters(System.Collections.Generic.IList&lt;E&gt;)
		/// 	</see>
		/// ,
		/// <see cref="SetParameters(Apache.Http.NameValuePair[])">SetParameters(Apache.Http.NameValuePair[])
		/// 	</see>
		/// <see cref="SetFile(Sharpen.FilePath)">SetFile(Sharpen.FilePath)</see>
		/// methods.
		/// </remarks>
		public virtual Apache.Http.Client.Entity.EntityBuilder SetBinary(byte[] binary)
		{
			ClearContent();
			this.binary = binary;
			return this;
		}

		/// <summary>
		/// Returns entity content as a
		/// <see cref="System.IO.InputStream">System.IO.InputStream</see>
		/// if set using
		/// <see cref="SetStream(System.IO.InputStream)">SetStream(System.IO.InputStream)</see>
		/// method.
		/// </summary>
		public virtual InputStream GetStream()
		{
			return stream;
		}

		/// <summary>
		/// Sets entity content as a
		/// <see cref="System.IO.InputStream">System.IO.InputStream</see>
		/// . This method is mutually exclusive with
		/// <see cref="SetText(string)">SetText(string)</see>
		/// ,
		/// <see cref="SetBinary(byte[])">SetBinary(byte[])</see>
		/// ,
		/// <see cref="SetSerializable(System.IO.Serializable)">SetSerializable(System.IO.Serializable)
		/// 	</see>
		/// ,
		/// <see cref="SetParameters(System.Collections.Generic.IList{E})">SetParameters(System.Collections.Generic.IList&lt;E&gt;)
		/// 	</see>
		/// ,
		/// <see cref="SetParameters(Apache.Http.NameValuePair[])">SetParameters(Apache.Http.NameValuePair[])
		/// 	</see>
		/// <see cref="SetFile(Sharpen.FilePath)">SetFile(Sharpen.FilePath)</see>
		/// methods.
		/// </summary>
		public virtual Apache.Http.Client.Entity.EntityBuilder SetStream(InputStream stream
			)
		{
			ClearContent();
			this.stream = stream;
			return this;
		}

		/// <summary>
		/// Returns entity content as a parameter list if set using
		/// <see cref="SetParameters(System.Collections.Generic.IList{E})">SetParameters(System.Collections.Generic.IList&lt;E&gt;)
		/// 	</see>
		/// or
		/// <see cref="SetParameters(Apache.Http.NameValuePair[])">SetParameters(Apache.Http.NameValuePair[])
		/// 	</see>
		/// methods.
		/// </summary>
		public virtual IList<NameValuePair> GetParameters()
		{
			return parameters;
		}

		/// <summary>Sets entity content as a parameter list.</summary>
		/// <remarks>
		/// Sets entity content as a parameter list. This method is mutually exclusive with
		/// <see cref="SetText(string)">SetText(string)</see>
		/// ,
		/// <see cref="SetBinary(byte[])">SetBinary(byte[])</see>
		/// ,
		/// <see cref="SetStream(System.IO.InputStream)">SetStream(System.IO.InputStream)</see>
		/// ,
		/// <see cref="SetSerializable(System.IO.Serializable)">SetSerializable(System.IO.Serializable)
		/// 	</see>
		/// ,
		/// <see cref="SetFile(Sharpen.FilePath)">SetFile(Sharpen.FilePath)</see>
		/// methods.
		/// </remarks>
		public virtual Apache.Http.Client.Entity.EntityBuilder SetParameters(IList<NameValuePair
			> parameters)
		{
			ClearContent();
			this.parameters = parameters;
			return this;
		}

		/// <summary>Sets entity content as a parameter list.</summary>
		/// <remarks>
		/// Sets entity content as a parameter list. This method is mutually exclusive with
		/// <see cref="SetText(string)">SetText(string)</see>
		/// ,
		/// <see cref="SetBinary(byte[])">SetBinary(byte[])</see>
		/// ,
		/// <see cref="SetStream(System.IO.InputStream)">SetStream(System.IO.InputStream)</see>
		/// ,
		/// <see cref="SetSerializable(System.IO.Serializable)">SetSerializable(System.IO.Serializable)
		/// 	</see>
		/// ,
		/// <see cref="SetFile(Sharpen.FilePath)">SetFile(Sharpen.FilePath)</see>
		/// methods.
		/// </remarks>
		public virtual Apache.Http.Client.Entity.EntityBuilder SetParameters(params NameValuePair
			[] parameters)
		{
			return SetParameters(Arrays.AsList(parameters));
		}

		/// <summary>
		/// Returns entity content as a
		/// <see cref="System.IO.Serializable">System.IO.Serializable</see>
		/// if set using
		/// <see cref="SetSerializable(System.IO.Serializable)">SetSerializable(System.IO.Serializable)
		/// 	</see>
		/// method.
		/// </summary>
		public virtual Serializable GetSerializable()
		{
			return serializable;
		}

		/// <summary>
		/// Sets entity content as a
		/// <see cref="System.IO.Serializable">System.IO.Serializable</see>
		/// . This method is mutually exclusive with
		/// <see cref="SetText(string)">SetText(string)</see>
		/// ,
		/// <see cref="SetBinary(byte[])">SetBinary(byte[])</see>
		/// ,
		/// <see cref="SetStream(System.IO.InputStream)">SetStream(System.IO.InputStream)</see>
		/// ,
		/// <see cref="SetParameters(System.Collections.Generic.IList{E})">SetParameters(System.Collections.Generic.IList&lt;E&gt;)
		/// 	</see>
		/// ,
		/// <see cref="SetParameters(Apache.Http.NameValuePair[])">SetParameters(Apache.Http.NameValuePair[])
		/// 	</see>
		/// <see cref="SetFile(Sharpen.FilePath)">SetFile(Sharpen.FilePath)</see>
		/// methods.
		/// </summary>
		public virtual Apache.Http.Client.Entity.EntityBuilder SetSerializable(Serializable
			 serializable)
		{
			ClearContent();
			this.serializable = serializable;
			return this;
		}

		/// <summary>
		/// Returns entity content as a
		/// <see cref="Sharpen.FilePath">Sharpen.FilePath</see>
		/// if set using
		/// <see cref="SetFile(Sharpen.FilePath)">SetFile(Sharpen.FilePath)</see>
		/// method.
		/// </summary>
		public virtual FilePath GetFile()
		{
			return file;
		}

		/// <summary>
		/// Sets entity content as a
		/// <see cref="Sharpen.FilePath">Sharpen.FilePath</see>
		/// . This method is mutually exclusive with
		/// <see cref="SetText(string)">SetText(string)</see>
		/// ,
		/// <see cref="SetBinary(byte[])">SetBinary(byte[])</see>
		/// ,
		/// <see cref="SetStream(System.IO.InputStream)">SetStream(System.IO.InputStream)</see>
		/// ,
		/// <see cref="SetParameters(System.Collections.Generic.IList{E})">SetParameters(System.Collections.Generic.IList&lt;E&gt;)
		/// 	</see>
		/// ,
		/// <see cref="SetParameters(Apache.Http.NameValuePair[])">SetParameters(Apache.Http.NameValuePair[])
		/// 	</see>
		/// <see cref="SetSerializable(System.IO.Serializable)">SetSerializable(System.IO.Serializable)
		/// 	</see>
		/// methods.
		/// </summary>
		public virtual Apache.Http.Client.Entity.EntityBuilder SetFile(FilePath file)
		{
			ClearContent();
			this.file = file;
			return this;
		}

		/// <summary>
		/// Returns
		/// <see cref="Apache.Http.Entity.ContentType">Apache.Http.Entity.ContentType</see>
		/// of the entity, if set.
		/// </summary>
		public virtual ContentType GetContentType()
		{
			return contentType;
		}

		/// <summary>
		/// Sets
		/// <see cref="Apache.Http.Entity.ContentType">Apache.Http.Entity.ContentType</see>
		/// of the entity.
		/// </summary>
		public virtual Apache.Http.Client.Entity.EntityBuilder SetContentType(ContentType
			 contentType)
		{
			this.contentType = contentType;
			return this;
		}

		/// <summary>Returns content encoding of the entity, if set.</summary>
		/// <remarks>Returns content encoding of the entity, if set.</remarks>
		public virtual string GetContentEncoding()
		{
			return contentEncoding;
		}

		/// <summary>Sets content encoding of the entity.</summary>
		/// <remarks>Sets content encoding of the entity.</remarks>
		public virtual Apache.Http.Client.Entity.EntityBuilder SetContentEncoding(string 
			contentEncoding)
		{
			this.contentEncoding = contentEncoding;
			return this;
		}

		/// <summary>Returns <code>true</code> if entity is to be chunk coded, <code>false</code> otherwise.
		/// 	</summary>
		/// <remarks>Returns <code>true</code> if entity is to be chunk coded, <code>false</code> otherwise.
		/// 	</remarks>
		public virtual bool IsChunked()
		{
			return chunked;
		}

		/// <summary>Makes entity chunk coded.</summary>
		/// <remarks>Makes entity chunk coded.</remarks>
		public virtual Apache.Http.Client.Entity.EntityBuilder Chunked()
		{
			this.chunked = true;
			return this;
		}

		/// <summary>Returns <code>true</code> if entity is to be GZIP compressed, <code>false</code> otherwise.
		/// 	</summary>
		/// <remarks>Returns <code>true</code> if entity is to be GZIP compressed, <code>false</code> otherwise.
		/// 	</remarks>
		public virtual bool IsGzipCompress()
		{
			return gzipCompress;
		}

		/// <summary>Makes entity GZIP compressed.</summary>
		/// <remarks>Makes entity GZIP compressed.</remarks>
		public virtual Apache.Http.Client.Entity.EntityBuilder GzipCompress()
		{
			this.gzipCompress = true;
			return this;
		}

		private ContentType GetContentOrDefault(ContentType def)
		{
			return this.contentType != null ? this.contentType : def;
		}

		/// <summary>
		/// Creates new instance of
		/// <see cref="Apache.Http.HttpEntity">Apache.Http.HttpEntity</see>
		/// based on the current state.
		/// </summary>
		public virtual HttpEntity Build()
		{
			AbstractHttpEntity e;
			if (this.text != null)
			{
				e = new StringEntity(this.text, GetContentOrDefault(ContentType.DefaultText));
			}
			else
			{
				if (this.binary != null)
				{
					e = new ByteArrayEntity(this.binary, GetContentOrDefault(ContentType.DefaultBinary
						));
				}
				else
				{
					if (this.stream != null)
					{
						e = new InputStreamEntity(this.stream, 1, GetContentOrDefault(ContentType.DefaultBinary
							));
					}
					else
					{
						if (this.parameters != null)
						{
							e = new UrlEncodedFormEntity(this.parameters, this.contentType != null ? this.contentType
								.GetCharset() : null);
						}
						else
						{
							if (this.serializable != null)
							{
								e = new SerializableEntity(this.serializable);
								e.SetContentType(ContentType.DefaultBinary.ToString());
							}
							else
							{
								if (this.file != null)
								{
									e = new FileEntity(this.file, GetContentOrDefault(ContentType.DefaultBinary));
								}
								else
								{
									e = new BasicHttpEntity();
								}
							}
						}
					}
				}
			}
			if (e.GetContentType() != null && this.contentType != null)
			{
				e.SetContentType(this.contentType.ToString());
			}
			e.SetContentEncoding(this.contentEncoding);
			e.SetChunked(this.chunked);
			if (this.gzipCompress)
			{
				return new GzipCompressingEntity(e);
			}
			return e;
		}
	}
}
