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
using System.Globalization;
using Apache.Http;
using Apache.Http.Client;
using Apache.Http.Client.Config;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Protocol;
using Apache.Http.Client.Utils;
using Apache.Http.Impl.Client;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Org.Apache.Http;
using Org.Apache.Http.Client.Methods;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Default implementation of
	/// <see cref="Apache.Http.Client.RedirectStrategy">Apache.Http.Client.RedirectStrategy
	/// 	</see>
	/// . This strategy honors the restrictions
	/// on automatic redirection of entity enclosing methods such as POST and PUT imposed by the
	/// HTTP specification. <tt>302 Moved Temporarily</tt>, <tt>301 Moved Permanently</tt> and
	/// <tt>307 Temporary Redirect</tt> status codes will result in an automatic redirect of
	/// HEAD and GET methods only. POST and PUT methods will not be automatically redirected
	/// as requiring user confirmation.
	/// <p/>
	/// The restriction on automatic redirection of POST methods can be relaxed by using
	/// <see cref="LaxRedirectStrategy">LaxRedirectStrategy</see>
	/// instead of
	/// <see cref="DefaultRedirectStrategy">DefaultRedirectStrategy</see>
	/// .
	/// </summary>
	/// <seealso cref="LaxRedirectStrategy">LaxRedirectStrategy</seealso>
	/// <since>4.1</since>
	public class DefaultRedirectStrategy : RedirectStrategy
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		[System.ObsoleteAttribute(@"(4.3) use Apache.Http.Client.Protocol.HttpClientContext.RedirectLocations ."
			)]
		[Obsolete]
		public const string RedirectLocations = "http.protocol.redirect-locations";

		public static readonly Apache.Http.Impl.Client.DefaultRedirectStrategy Instance = 
			new Apache.Http.Impl.Client.DefaultRedirectStrategy();

		/// <summary>Redirectable methods.</summary>
		/// <remarks>Redirectable methods.</remarks>
		private static readonly string[] RedirectMethods = new string[] { HttpGet.MethodName
			, HttpHead.MethodName };

		public DefaultRedirectStrategy() : base()
		{
		}

		/// <exception cref="Apache.Http.ProtocolException"></exception>
		public virtual bool IsRedirected(IHttpRequest request, HttpResponse response, HttpContext
			 context)
		{
			Args.NotNull(request, "HTTP request");
			Args.NotNull(response, "HTTP response");
			int statusCode = response.GetStatusLine().GetStatusCode();
			string method = request.GetRequestLine().GetMethod();
			Header locationHeader = response.GetFirstHeader("location");
			switch (statusCode)
			{
				case HttpStatus.ScMovedTemporarily:
				{
					return IsRedirectable(method) && locationHeader != null;
				}

				case HttpStatus.ScMovedPermanently:
				case HttpStatus.ScTemporaryRedirect:
				{
					return IsRedirectable(method);
				}

				case HttpStatus.ScSeeOther:
				{
					return true;
				}

				default:
				{
					return false;
				}
			}
		}

		//end of switch
		/// <exception cref="Apache.Http.ProtocolException"></exception>
		public virtual URI GetLocationURI(IHttpRequest request, HttpResponse response, HttpContext
			 context)
		{
			Args.NotNull(request, "HTTP request");
			Args.NotNull(response, "HTTP response");
			Args.NotNull(context, "HTTP context");
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			//get the location header to find out where to redirect to
			Header locationHeader = response.GetFirstHeader("location");
			if (locationHeader == null)
			{
				// got a redirect response, but no location header
				throw new ProtocolException("Received redirect response " + response.GetStatusLine
					() + " but no location header");
			}
			string location = locationHeader.GetValue();
			if (this.log.IsDebugEnabled())
			{
				this.log.Debug("Redirect requested to location '" + location + "'");
			}
			RequestConfig config = clientContext.GetRequestConfig();
			URI uri = CreateLocationURI(location);
			// rfc2616 demands the location value be a complete URI
			// Location       = "Location" ":" absoluteURI
			try
			{
				if (!uri.IsAbsolute())
				{
					if (!config.IsRelativeRedirectsAllowed())
					{
						throw new ProtocolException("Relative redirect location '" + uri + "' not allowed"
							);
					}
					// Adjust location URI
					HttpHost target = clientContext.GetTargetHost();
					Asserts.NotNull(target, "Target host");
					URI requestURI = new URI(request.GetRequestLine().GetUri());
					URI absoluteRequestURI = URIUtils.RewriteURI(requestURI, target, false);
					uri = URIUtils.Resolve(absoluteRequestURI, uri);
				}
			}
			catch (URISyntaxException ex)
			{
				throw new ProtocolException(ex.Message, ex);
			}
			RedirectLocations redirectLocations = (RedirectLocations)clientContext.GetAttribute
				(HttpClientContext.RedirectLocations);
			if (redirectLocations == null)
			{
				redirectLocations = new RedirectLocations();
				context.SetAttribute(HttpClientContext.RedirectLocations, redirectLocations);
			}
			if (!config.IsCircularRedirectsAllowed())
			{
				if (redirectLocations.Contains(uri))
				{
					throw new CircularRedirectException("Circular redirect to '" + uri + "'");
				}
			}
			redirectLocations.Add(uri);
			return uri;
		}

		/// <since>4.1</since>
		/// <exception cref="Apache.Http.ProtocolException"></exception>
		protected internal virtual URI CreateLocationURI(string location)
		{
			try
			{
				URIBuilder b = new URIBuilder(new URI(location).Normalize());
				string host = b.GetHost();
				if (host != null)
				{
					b.SetHost(host.ToLower(CultureInfo.InvariantCulture));
				}
				string path = b.GetPath();
				if (TextUtils.IsEmpty(path))
				{
					b.SetPath("/");
				}
				return b.Build();
			}
			catch (URISyntaxException ex)
			{
				throw new ProtocolException("Invalid redirect URI: " + location, ex);
			}
		}

		/// <since>4.2</since>
		protected internal virtual bool IsRedirectable(string method)
		{
			foreach (string m in RedirectMethods)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(m, method))
				{
					return true;
				}
			}
			return false;
		}

		/// <exception cref="Apache.Http.ProtocolException"></exception>
		public virtual IHttpUriRequest GetRedirect(IHttpRequest request, HttpResponse response
			, HttpContext context)
		{
			URI uri = GetLocationURI(request, response, context);
			string method = request.GetRequestLine().GetMethod();
			if (Sharpen.Runtime.EqualsIgnoreCase(method, HttpHead.MethodName))
			{
				return new HttpHead(uri);
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(method, HttpGet.MethodName))
				{
					return new HttpGet(uri);
				}
				else
				{
					int status = response.GetStatusLine().GetStatusCode();
					if (status == HttpStatus.ScTemporaryRedirect)
					{
						return RequestBuilder.Copy(request).SetUri(uri).Build();
					}
					else
					{
						return new HttpGet(uri);
					}
				}
			}
		}
	}
}
