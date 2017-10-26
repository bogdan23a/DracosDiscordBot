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
using System.Text;
using Apache.Http;
using Apache.Http.Client.Utils;
using Apache.Http.Entity;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Client.Entity
{
	/// <summary>An entity composed of a list of url-encoded pairs.</summary>
	/// <remarks>
	/// An entity composed of a list of url-encoded pairs.
	/// This is typically useful while sending an HTTP POST request.
	/// </remarks>
	/// <since>4.0</since>
	public class UrlEncodedFormEntity : StringEntity
	{
		/// <summary>
		/// Constructs a new
		/// <see cref="UrlEncodedFormEntity">UrlEncodedFormEntity</see>
		/// with the list
		/// of parameters in the specified encoding.
		/// </summary>
		/// <param name="parameters">list of name/value pairs</param>
		/// <param name="charset">encoding the name/value pairs be encoded with</param>
		/// <exception cref="System.IO.UnsupportedEncodingException">if the encoding isn't supported
		/// 	</exception>
		public UrlEncodedFormEntity(IList<NameValuePair> parameters, string charset) : base
			(URLEncodedUtils.Format(parameters, charset != null ? charset : HTTP.DefContentCharset
			.Name()), ContentType.Create(URLEncodedUtils.ContentType, charset))
		{
		}

		/// <summary>
		/// Constructs a new
		/// <see cref="UrlEncodedFormEntity">UrlEncodedFormEntity</see>
		/// with the list
		/// of parameters in the specified encoding.
		/// </summary>
		/// <param name="parameters">iterable collection of name/value pairs</param>
		/// <param name="charset">encoding the name/value pairs be encoded with</param>
		/// <since>4.2</since>
		public UrlEncodedFormEntity(IEnumerable<NameValuePair> parameters, Encoding charset
			) : base(URLEncodedUtils.Format(parameters, charset != null ? charset : HTTP.DefContentCharset
			), ContentType.Create(URLEncodedUtils.ContentType, charset))
		{
		}

		/// <summary>
		/// Constructs a new
		/// <see cref="UrlEncodedFormEntity">UrlEncodedFormEntity</see>
		/// with the list
		/// of parameters with the default encoding of
		/// <see cref="Apache.Http.Protocol.HTTP.DefaultContentCharset">Apache.Http.Protocol.HTTP.DefaultContentCharset
		/// 	</see>
		/// </summary>
		/// <param name="parameters">list of name/value pairs</param>
		/// <exception cref="System.IO.UnsupportedEncodingException">if the default encoding isn't supported
		/// 	</exception>
		public UrlEncodedFormEntity(IList<NameValuePair> parameters) : this(parameters, (
			Encoding)null)
		{
		}

		/// <summary>
		/// Constructs a new
		/// <see cref="UrlEncodedFormEntity">UrlEncodedFormEntity</see>
		/// with the list
		/// of parameters with the default encoding of
		/// <see cref="Apache.Http.Protocol.HTTP.DefaultContentCharset">Apache.Http.Protocol.HTTP.DefaultContentCharset
		/// 	</see>
		/// </summary>
		/// <param name="parameters">iterable collection of name/value pairs</param>
		/// <since>4.2</since>
		public UrlEncodedFormEntity(IEnumerable<NameValuePair> parameters) : this(parameters
			, null)
		{
		}
		// AbstractHttpEntity is not thread-safe
	}
}
