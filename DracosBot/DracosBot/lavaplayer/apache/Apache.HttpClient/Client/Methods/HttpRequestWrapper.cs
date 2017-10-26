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
using Apache.Http.Client.Methods;
using Apache.Http.Message;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Client.Methods
{
	/// <summary>
	/// A wrapper class for
	/// <see cref="Org.Apache.Http.IHttpRequest">Org.Apache.Http.IHttpRequest</see>
	/// that can be used to change properties of the current
	/// request without modifying the original object.
	/// </summary>
	/// <since>4.3</since>
	public class HttpRequestWrapper : AbstractHttpMessage, IHttpUriRequest
	{
		private readonly IHttpRequest original;

		private readonly string method;

		private ProtocolVersion version;

		private URI uri;

		private HttpRequestWrapper(IHttpRequest request) : base()
		{
			this.original = request;
			this.version = this.original.GetRequestLine().GetProtocolVersion();
			this.method = this.original.GetRequestLine().GetMethod();
			if (request is IHttpUriRequest)
			{
				this.uri = ((IHttpUriRequest)request).GetURI();
			}
			else
			{
				this.uri = null;
			}
			SetHeaders(request.GetAllHeaders());
		}

		public override ProtocolVersion GetProtocolVersion()
		{
			return this.version != null ? this.version : this.original.GetProtocolVersion();
		}

		public virtual void SetProtocolVersion(ProtocolVersion version)
		{
			this.version = version;
		}

		public virtual URI GetURI()
		{
			return this.uri;
		}

		public virtual void SetURI(URI uri)
		{
			this.uri = uri;
		}

		public virtual string GetMethod()
		{
			return method;
		}

		/// <exception cref="System.NotSupportedException"></exception>
		public virtual void Abort()
		{
			throw new NotSupportedException();
		}

		public virtual bool IsAborted()
		{
			return false;
		}

		public virtual RequestLine GetRequestLine()
		{
			string requestUri = null;
			if (this.uri != null)
			{
				requestUri = this.uri.ToASCIIString();
			}
			else
			{
				requestUri = this.original.GetRequestLine().GetUri();
			}
			if (requestUri == null || requestUri.Length == 0)
			{
				requestUri = "/";
			}
			return new BasicRequestLine(this.method, requestUri, GetProtocolVersion());
		}

		public virtual IHttpRequest GetOriginal()
		{
			return this.original;
		}

		public override string ToString()
		{
			return GetRequestLine() + " " + this.headergroup;
		}

		internal class HttpEntityEnclosingRequestWrapper : HttpRequestWrapper, HttpEntityEnclosingRequest
		{
			private HttpEntity entity;

			public HttpEntityEnclosingRequestWrapper(HttpEntityEnclosingRequest request) : base
				(request)
			{
				this.entity = request.GetEntity();
			}

			public virtual HttpEntity GetEntity()
			{
				return this.entity;
			}

			public virtual void SetEntity(HttpEntity entity)
			{
				this.entity = entity;
			}

			public virtual bool ExpectContinue()
			{
				Header expect = GetFirstHeader(HTTP.ExpectDirective);
				return expect != null && Sharpen.Runtime.EqualsIgnoreCase(HTTP.ExpectContinue, expect
					.GetValue());
			}
		}

		public static HttpRequestWrapper Wrap(IHttpRequest request)
		{
			if (request == null)
			{
				return null;
			}
			if (request is HttpEntityEnclosingRequest)
			{
				return new HttpRequestWrapper.HttpEntityEnclosingRequestWrapper((HttpEntityEnclosingRequest
					)request);
			}
			else
			{
				return new HttpRequestWrapper(request);
			}
		}

		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) useApache.Http.Client.Config.RequestConfig .")]
		public override HttpParams GetParams()
		{
			if (this.@params == null)
			{
				this.@params = original.GetParams().Copy();
			}
			return this.@params;
		}
	}
}
