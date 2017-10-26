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
using Apache.Http;
using Apache.Http.Client.Config;
using Apache.Http.Client.Entity;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Utils;
using Apache.Http.Message;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Client.Methods
{
	/// <summary>
	/// Builder for
	/// <see cref="IHttpUriRequest">IHttpUriRequest</see>
	/// instances.
	/// <p/>
	/// Please note that this class treats parameters differently depending on composition
	/// of the request: if the request has a content entity explicitly set with
	/// <see cref="SetEntity(Apache.Http.HttpEntity)">SetEntity(Apache.Http.HttpEntity)</see>
	/// or it is not an entity enclosing method
	/// (such as POST or PUT), parameters will be added to the query component of the request URI.
	/// Otherwise, parameters will be added as a URL encoded
	/// <see cref="Apache.Http.Client.Entity.UrlEncodedFormEntity">entity</see>
	/// .
	/// </summary>
	/// <since>4.3</since>
	public class RequestBuilder
	{
		private string method;

		private ProtocolVersion version;

		private URI uri;

		private HeaderGroup headergroup;

		private HttpEntity entity;

		private List<NameValuePair> parameters;

		private RequestConfig config;

		internal RequestBuilder(string method) : base()
		{
			this.method = method;
		}

		internal RequestBuilder() : this(null)
		{
		}

		public static Apache.Http.Client.Methods.RequestBuilder Create(string method)
		{
			Args.NotBlank(method, "HTTP method");
			return new Apache.Http.Client.Methods.RequestBuilder(method);
		}

		public static Apache.Http.Client.Methods.RequestBuilder Get()
		{
			return new Apache.Http.Client.Methods.RequestBuilder(HttpGet.MethodName);
		}

		public static Apache.Http.Client.Methods.RequestBuilder Head()
		{
			return new Apache.Http.Client.Methods.RequestBuilder(HttpHead.MethodName);
		}

		public static Apache.Http.Client.Methods.RequestBuilder Post()
		{
			return new Apache.Http.Client.Methods.RequestBuilder(HttpPost.MethodName);
		}

		public static Apache.Http.Client.Methods.RequestBuilder Put()
		{
			return new Apache.Http.Client.Methods.RequestBuilder(HttpPut.MethodName);
		}

		public static Apache.Http.Client.Methods.RequestBuilder Delete()
		{
			return new Apache.Http.Client.Methods.RequestBuilder(HttpDelete.MethodName);
		}

		public static Apache.Http.Client.Methods.RequestBuilder Trace()
		{
			return new Apache.Http.Client.Methods.RequestBuilder(HttpTrace.MethodName);
		}

		public static Apache.Http.Client.Methods.RequestBuilder Options()
		{
			return new Apache.Http.Client.Methods.RequestBuilder(HttpOptions.MethodName);
		}

		public static Apache.Http.Client.Methods.RequestBuilder Copy(IHttpRequest request
			)
		{
			Args.NotNull(request, "HTTP request");
			return new Apache.Http.Client.Methods.RequestBuilder().DoCopy(request);
		}

		private Apache.Http.Client.Methods.RequestBuilder DoCopy(IHttpRequest request)
		{
			if (request == null)
			{
				return this;
			}
			method = request.GetRequestLine().GetMethod();
			version = request.GetRequestLine().GetProtocolVersion();
			if (request is IHttpUriRequest)
			{
				uri = ((IHttpUriRequest)request).GetURI();
			}
			else
			{
				uri = URI.Create(request.GetRequestLine().GetMethod());
			}
			if (headergroup == null)
			{
				headergroup = new HeaderGroup();
			}
			headergroup.Clear();
			headergroup.SetHeaders(request.GetAllHeaders());
			if (request is HttpEntityEnclosingRequest)
			{
				entity = ((HttpEntityEnclosingRequest)request).GetEntity();
			}
			else
			{
				entity = null;
			}
			if (request is Configurable)
			{
				this.config = ((Configurable)request).GetConfig();
			}
			else
			{
				this.config = null;
			}
			this.parameters = null;
			return this;
		}

		public virtual string GetMethod()
		{
			return method;
		}

		public virtual ProtocolVersion GetVersion()
		{
			return version;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder SetVersion(ProtocolVersion
			 version)
		{
			this.version = version;
			return this;
		}

		public virtual URI GetUri()
		{
			return uri;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder SetUri(URI uri)
		{
			this.uri = uri;
			return this;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder SetUri(string uri)
		{
			this.uri = uri != null ? URI.Create(uri) : null;
			return this;
		}

		public virtual Header GetFirstHeader(string name)
		{
			return headergroup != null ? headergroup.GetFirstHeader(name) : null;
		}

		public virtual Header GetLastHeader(string name)
		{
			return headergroup != null ? headergroup.GetLastHeader(name) : null;
		}

		public virtual Header[] GetHeaders(string name)
		{
			return headergroup != null ? headergroup.GetHeaders(name) : null;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder AddHeader(Header header)
		{
			if (headergroup == null)
			{
				headergroup = new HeaderGroup();
			}
			headergroup.AddHeader(header);
			return this;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder AddHeader(string name, string
			 value)
		{
			if (headergroup == null)
			{
				headergroup = new HeaderGroup();
			}
			this.headergroup.AddHeader(new BasicHeader(name, value));
			return this;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder RemoveHeader(Header header
			)
		{
			if (headergroup == null)
			{
				headergroup = new HeaderGroup();
			}
			headergroup.RemoveHeader(header);
			return this;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder RemoveHeaders(string name
			)
		{
			if (name == null || headergroup == null)
			{
				return this;
			}
			for (HeaderIterator i = headergroup.Iterator(); i.HasNext(); )
			{
				Header header = i.NextHeader();
				if (Sharpen.Runtime.EqualsIgnoreCase(name, header.GetName()))
				{
					i.Remove();
				}
			}
			return this;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder SetHeader(Header header)
		{
			if (headergroup == null)
			{
				headergroup = new HeaderGroup();
			}
			this.headergroup.UpdateHeader(header);
			return this;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder SetHeader(string name, string
			 value)
		{
			if (headergroup == null)
			{
				headergroup = new HeaderGroup();
			}
			this.headergroup.UpdateHeader(new BasicHeader(name, value));
			return this;
		}

		public virtual HttpEntity GetEntity()
		{
			return entity;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder SetEntity(HttpEntity entity
			)
		{
			this.entity = entity;
			return this;
		}

		public virtual IList<NameValuePair> GetParameters()
		{
			return parameters != null ? new AList<NameValuePair>(parameters) : new AList<NameValuePair
				>();
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder AddParameter(NameValuePair
			 nvp)
		{
			Args.NotNull(nvp, "Name value pair");
			if (parameters == null)
			{
				parameters = new List<NameValuePair>();
			}
			parameters.AddItem(nvp);
			return this;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder AddParameter(string name
			, string value)
		{
			return AddParameter(new BasicNameValuePair(name, value));
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder AddParameters(params NameValuePair
			[] nvps)
		{
			foreach (NameValuePair nvp in nvps)
			{
				AddParameter(nvp);
			}
			return this;
		}

		public virtual RequestConfig GetConfig()
		{
			return config;
		}

		public virtual Apache.Http.Client.Methods.RequestBuilder SetConfig(RequestConfig 
			config)
		{
			this.config = config;
			return this;
		}

		public virtual IHttpUriRequest Build()
		{
			HttpRequestBase result;
			URI uri = this.uri != null ? this.uri : URI.Create("/");
			HttpEntity entity = this.entity;
			if (parameters != null && !parameters.IsEmpty())
			{
				if (entity == null && (Sharpen.Runtime.EqualsIgnoreCase(HttpPost.MethodName, method
					) || Sharpen.Runtime.EqualsIgnoreCase(HttpPut.MethodName, method)))
				{
					entity = new UrlEncodedFormEntity(parameters, HTTP.DefContentCharset);
				}
				else
				{
					try
					{
						uri = new URIBuilder(uri).AddParameters(parameters).Build();
					}
					catch (URISyntaxException)
					{
					}
				}
			}
			// should never happen
			if (entity == null)
			{
				result = new RequestBuilder.InternalRequest(method);
			}
			else
			{
				RequestBuilder.InternalEntityEclosingRequest request = new RequestBuilder.InternalEntityEclosingRequest
					(method);
				request.SetEntity(entity);
				result = request;
			}
			result.SetProtocolVersion(this.version);
			result.SetURI(uri);
			if (this.headergroup != null)
			{
				result.SetHeaders(this.headergroup.GetAllHeaders());
			}
			result.SetConfig(this.config);
			return result;
		}

		internal class InternalRequest : HttpRequestBase
		{
			private readonly string method;

			internal InternalRequest(string method) : base()
			{
				this.method = method;
			}

			public override string GetMethod()
			{
				return this.method;
			}
		}

		internal class InternalEntityEclosingRequest : HttpEntityEnclosingRequestBase
		{
			private readonly string method;

			internal InternalEntityEclosingRequest(string method) : base()
			{
				this.method = method;
			}

			public override string GetMethod()
			{
				return this.method;
			}
		}
	}
}
