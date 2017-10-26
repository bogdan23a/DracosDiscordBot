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

using Apache.Http;
using Apache.Http.Client.Config;
using Apache.Http.Client.Methods;
using Apache.Http.Message;
using Apache.Http.Params;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Client.Methods
{
	/// <summary>
	/// Base implementation of
	/// <see cref="IHttpUriRequest">IHttpUriRequest</see>
	/// .
	/// </summary>
	/// <since>4.0</since>
	public abstract class HttpRequestBase : AbstractExecutionAwareRequest, IHttpUriRequest
		, Configurable
	{
		private ProtocolVersion version;

		private URI uri;

		private RequestConfig config;

		public abstract string GetMethod();

		/// <since>4.3</since>
		public virtual void SetProtocolVersion(ProtocolVersion version)
		{
			this.version = version;
		}

		public override ProtocolVersion GetProtocolVersion()
		{
			return version != null ? version : HttpProtocolParams.GetVersion(GetParams());
		}

		/// <summary>Returns the original request URI.</summary>
		/// <remarks>
		/// Returns the original request URI.
		/// <p>
		/// Please note URI remains unchanged in the course of request execution and
		/// is not updated if the request is redirected to another location.
		/// </remarks>
		public virtual URI GetURI()
		{
			return this.uri;
		}

		public override RequestLine GetRequestLine()
		{
			string method = GetMethod();
			ProtocolVersion ver = GetProtocolVersion();
			URI uri = GetURI();
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

		public virtual RequestConfig GetConfig()
		{
			return config;
		}

		public virtual void SetConfig(RequestConfig config)
		{
			this.config = config;
		}

		public virtual void SetURI(URI uri)
		{
			this.uri = uri;
		}

		/// <since>4.2</since>
		public virtual void Started()
		{
		}

		/// <summary>A convenience method to simplify migration from HttpClient 3.1 API.</summary>
		/// <remarks>
		/// A convenience method to simplify migration from HttpClient 3.1 API. This method is
		/// equivalent to
		/// <see cref="AbstractExecutionAwareRequest.Reset()">AbstractExecutionAwareRequest.Reset()
		/// 	</see>
		/// .
		/// </remarks>
		/// <since>4.2</since>
		public virtual void ReleaseConnection()
		{
			Reset();
		}

		public override string ToString()
		{
			return GetMethod() + " " + GetURI() + " " + GetProtocolVersion();
		}
	}
}
