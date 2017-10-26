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
using Apache.Http.Message;
using Apache.Http.Params;
using Apache.Http.Util;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// A wrapper class for
	/// <see cref="Org.Apache.Http.IHttpRequest">Org.Apache.Http.IHttpRequest</see>
	/// s that can be used to change
	/// properties of the current request without modifying the original
	/// object.
	/// </p>
	/// This class is also capable of resetting the request headers to
	/// the state of the original request.
	/// </summary>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) do not use.")]
	public class RequestWrapper : AbstractHttpMessage, IHttpUriRequest
	{
		private readonly IHttpRequest original;

		private URI uri;

		private string method;

		private ProtocolVersion version;

		private int execCount;

		/// <exception cref="Apache.Http.ProtocolException"></exception>
		public RequestWrapper(IHttpRequest request) : base()
		{
			Args.NotNull(request, "HTTP request");
			this.original = request;
			SetParams(request.GetParams());
			SetHeaders(request.GetAllHeaders());
			// Make a copy of the original URI
			if (request is IHttpUriRequest)
			{
				this.uri = ((IHttpUriRequest)request).GetURI();
				this.method = ((IHttpUriRequest)request).GetMethod();
				this.version = null;
			}
			else
			{
				RequestLine requestLine = request.GetRequestLine();
				try
				{
					this.uri = new URI(requestLine.GetUri());
				}
				catch (URISyntaxException ex)
				{
					throw new ProtocolException("Invalid request URI: " + requestLine.GetUri(), ex);
				}
				this.method = requestLine.GetMethod();
				this.version = request.GetProtocolVersion();
			}
			this.execCount = 0;
		}

		public virtual void ResetHeaders()
		{
			// Make a copy of original headers
			this.headergroup.Clear();
			SetHeaders(this.original.GetAllHeaders());
		}

		public virtual string GetMethod()
		{
			return this.method;
		}

		public virtual void SetMethod(string method)
		{
			Args.NotNull(method, "Method name");
			this.method = method;
		}

		public override ProtocolVersion GetProtocolVersion()
		{
			if (this.version == null)
			{
				this.version = HttpProtocolParams.GetVersion(GetParams());
			}
			return this.version;
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

		public virtual RequestLine GetRequestLine()
		{
			string method = GetMethod();
			ProtocolVersion ver = GetProtocolVersion();
			string uritext = null;
			if (uri != null)
			{
				uritext = uri.ToASCIIString();
			}
			if (uritext == null || uritext.Length == 0)
			{
				uritext = "/";
			}
			return new BasicRequestLine(method, uritext, ver);
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

		public virtual IHttpRequest GetOriginal()
		{
			return this.original;
		}

		public virtual bool IsRepeatable()
		{
			return true;
		}

		public virtual int GetExecCount()
		{
			return this.execCount;
		}

		public virtual void IncrementExecCount()
		{
			this.execCount++;
		}
	}
}
