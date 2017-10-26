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
using System.Collections.Generic;
using System.Text;
using Apache.Http;
using Apache.Http.Client.Utils;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Client.Utils
{
	/// <summary>
	/// A collection of utilities for
	/// <see cref="Sharpen.URI">URIs</see>
	/// , to workaround
	/// bugs within the class or for ease-of-use features.
	/// </summary>
	/// <since>4.0</since>
	public class URIUtils
	{
		/// <summary>
		/// Constructs a
		/// <see cref="Sharpen.URI">Sharpen.URI</see>
		/// using all the parameters. This should be
		/// used instead of
		/// <see cref="Sharpen.URI.Uri(string, string, string, int, string, string, string)">Sharpen.URI.Uri(string, string, string, int, string, string, string)
		/// 	</see>
		/// or any of the other URI multi-argument URI constructors.
		/// </summary>
		/// <param name="scheme">Scheme name</param>
		/// <param name="host">Host name</param>
		/// <param name="port">Port number</param>
		/// <param name="path">Path</param>
		/// <param name="query">Query</param>
		/// <param name="fragment">Fragment</param>
		/// <exception cref="Sharpen.URISyntaxException">
		/// If both a scheme and a path are given but the path is
		/// relative, if the URI string constructed from the given
		/// components violates RFC&nbsp;2396, or if the authority
		/// component of the string is present but cannot be parsed
		/// as a server-based authority
		/// </exception>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2) use URIBuilder .")]
		public static URI CreateURI(string scheme, string host, int port, string path, string
			 query, string fragment)
		{
			StringBuilder buffer = new StringBuilder();
			if (host != null)
			{
				if (scheme != null)
				{
					buffer.Append(scheme);
					buffer.Append("://");
				}
				buffer.Append(host);
				if (port > 0)
				{
					buffer.Append(':');
					buffer.Append(port);
				}
			}
			if (path == null || !path.StartsWith("/"))
			{
				buffer.Append('/');
			}
			if (path != null)
			{
				buffer.Append(path);
			}
			if (query != null)
			{
				buffer.Append('?');
				buffer.Append(query);
			}
			if (fragment != null)
			{
				buffer.Append('#');
				buffer.Append(fragment);
			}
			return new URI(buffer.ToString());
		}

		/// <summary>
		/// A convenience method for creating a new
		/// <see cref="Sharpen.URI">Sharpen.URI</see>
		/// whose scheme, host
		/// and port are taken from the target host, but whose path, query and
		/// fragment are taken from the existing URI. The fragment is only used if
		/// dropFragment is false. The path is set to "/" if not explicitly specified.
		/// </summary>
		/// <param name="uri">Contains the path, query and fragment to use.</param>
		/// <param name="target">Contains the scheme, host and port to use.</param>
		/// <param name="dropFragment">True if the fragment should not be copied.</param>
		/// <exception cref="Sharpen.URISyntaxException">If the resulting URI is invalid.</exception>
		public static URI RewriteURI(URI uri, HttpHost target, bool dropFragment)
		{
			Args.NotNull(uri, "URI");
			if (uri.IsOpaque())
			{
				return uri;
			}
			URIBuilder uribuilder = new URIBuilder(uri);
			if (target != null)
			{
				uribuilder.SetScheme(target.GetSchemeName());
				uribuilder.SetHost(target.GetHostName());
				uribuilder.SetPort(target.GetPort());
			}
			else
			{
				uribuilder.SetScheme(null);
				uribuilder.SetHost(null);
				uribuilder.SetPort(-1);
			}
			if (dropFragment)
			{
				uribuilder.SetFragment(null);
			}
			if (TextUtils.IsEmpty(uribuilder.GetPath()))
			{
				uribuilder.SetPath("/");
			}
			return uribuilder.Build();
		}

		/// <summary>
		/// A convenience method for
		/// <see cref="RewriteURI(Sharpen.URI, Apache.Http.HttpHost, bool)">RewriteURI(Sharpen.URI, Apache.Http.HttpHost, bool)
		/// 	</see>
		/// that always keeps the
		/// fragment.
		/// </summary>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public static URI RewriteURI(URI uri, HttpHost target)
		{
			return RewriteURI(uri, target, false);
		}

		/// <summary>
		/// A convenience method that creates a new
		/// <see cref="Sharpen.URI">Sharpen.URI</see>
		/// whose scheme, host, port, path,
		/// query are taken from the existing URI, dropping any fragment or user-information.
		/// The path is set to "/" if not explicitly specified. The existing URI is returned
		/// unmodified if it has no fragment or user-information and has a path.
		/// </summary>
		/// <param name="uri">original URI.</param>
		/// <exception cref="Sharpen.URISyntaxException">If the resulting URI is invalid.</exception>
		public static URI RewriteURI(URI uri)
		{
			Args.NotNull(uri, "URI");
			if (uri.IsOpaque())
			{
				return uri;
			}
			URIBuilder uribuilder = new URIBuilder(uri);
			if (uribuilder.GetUserInfo() != null)
			{
				uribuilder.SetUserInfo(null);
			}
			if (TextUtils.IsEmpty(uribuilder.GetPath()))
			{
				uribuilder.SetPath("/");
			}
			if (uribuilder.GetHost() != null)
			{
				uribuilder.SetHost(uribuilder.GetHost().ToLower(Sharpen.Extensions.GetEnglishCulture()
					));
			}
			uribuilder.SetFragment(null);
			return uribuilder.Build();
		}

		/// <summary>Resolves a URI reference against a base URI.</summary>
		/// <remarks>
		/// Resolves a URI reference against a base URI. Work-around for bug in
		/// java.net.URI (<http://bugs.sun.com/bugdatabase/view_bug.do?bug_id=4708535>)
		/// </remarks>
		/// <param name="baseURI">the base URI</param>
		/// <param name="reference">the URI reference</param>
		/// <returns>the resulting URI</returns>
		public static URI Resolve(URI baseURI, string reference)
		{
			return Apache.Http.Client.Utils.URIUtils.Resolve(baseURI, URI.Create(reference));
		}

		/// <summary>Resolves a URI reference against a base URI.</summary>
		/// <remarks>
		/// Resolves a URI reference against a base URI. Work-around for bugs in
		/// java.net.URI (e.g. <http://bugs.sun.com/bugdatabase/view_bug.do?bug_id=4708535>)
		/// </remarks>
		/// <param name="baseURI">the base URI</param>
		/// <param name="reference">the URI reference</param>
		/// <returns>the resulting URI</returns>
		public static URI Resolve(URI baseURI, URI reference)
		{
			Args.NotNull(baseURI, "Base URI");
			Args.NotNull(reference, "Reference URI");
			URI @ref = reference;
			string s = @ref.ToString();
			if (s.StartsWith("?"))
			{
				return ResolveReferenceStartingWithQueryString(baseURI, @ref);
			}
			bool emptyReference = s.Length == 0;
			if (emptyReference)
			{
				@ref = URI.Create("#");
			}
			URI resolved = baseURI.Resolve(@ref);
			if (emptyReference)
			{
				string resolvedString = resolved.ToString();
				resolved = URI.Create(Sharpen.Runtime.Substring(resolvedString, 0, resolvedString
					.IndexOf('#')));
			}
			return NormalizeSyntax(resolved);
		}

		/// <summary>Resolves a reference starting with a query string.</summary>
		/// <remarks>Resolves a reference starting with a query string.</remarks>
		/// <param name="baseURI">the base URI</param>
		/// <param name="reference">the URI reference starting with a query string</param>
		/// <returns>the resulting URI</returns>
		private static URI ResolveReferenceStartingWithQueryString(URI baseURI, URI reference
			)
		{
			string baseUri = baseURI.ToString();
			baseUri = baseUri.IndexOf('?') > -1 ? Sharpen.Runtime.Substring(baseUri, 0, baseUri
				.IndexOf('?')) : baseUri;
			return URI.Create(baseUri + reference.ToString());
		}

		/// <summary>
		/// Removes dot segments according to RFC 3986, section 5.2.4 and
		/// Syntax-Based Normalization according to RFC 3986, section 6.2.2.
		/// </summary>
		/// <remarks>
		/// Removes dot segments according to RFC 3986, section 5.2.4 and
		/// Syntax-Based Normalization according to RFC 3986, section 6.2.2.
		/// </remarks>
		/// <param name="uri">the original URI</param>
		/// <returns>the URI without dot segments</returns>
		private static URI NormalizeSyntax(URI uri)
		{
			if (uri.IsOpaque() || uri.GetAuthority() == null)
			{
				// opaque and file: URIs
				return uri;
			}
			Args.Check(uri.IsAbsolute(), "Base URI must be absolute");
			string path = uri.GetPath() == null ? string.Empty : uri.GetPath();
			string[] inputSegments = path.Split("/");
			Stack<string> outputSegments = new Stack<string>();
			foreach (string inputSegment in inputSegments)
			{
				if ((inputSegment.Length == 0) || (".".Equals(inputSegment)))
				{
				}
				else
				{
					// Do nothing
					if ("..".Equals(inputSegment))
					{
						if (!outputSegments.IsEmpty())
						{
							outputSegments.Pop();
						}
					}
					else
					{
						outputSegments.Push(inputSegment);
					}
				}
			}
			StringBuilder outputBuffer = new StringBuilder();
			foreach (string outputSegment in outputSegments)
			{
				outputBuffer.Append('/').Append(outputSegment);
			}
			if (path.LastIndexOf('/') == path.Length - 1)
			{
				// path.endsWith("/") || path.equals("")
				outputBuffer.Append('/');
			}
			try
			{
				string scheme = uri.GetScheme().ToLower();
				string auth = uri.GetAuthority().ToLower();
				URI @ref = new URI(scheme, auth, outputBuffer.ToString(), null, null);
				if (uri.GetQuery() == null && uri.GetFragment() == null)
				{
					return @ref;
				}
				StringBuilder normalized = new StringBuilder(@ref.ToASCIIString());
				if (uri.GetQuery() != null)
				{
					// query string passed through unchanged
					normalized.Append('?').Append(uri.GetRawQuery());
				}
				if (uri.GetFragment() != null)
				{
					// fragment passed through unchanged
					normalized.Append('#').Append(uri.GetRawFragment());
				}
				return URI.Create(normalized.ToString());
			}
			catch (URISyntaxException e)
			{
				throw new ArgumentException(e);
			}
		}

		/// <summary>
		/// Extracts target host from the given
		/// <see cref="Sharpen.URI">Sharpen.URI</see>
		/// .
		/// </summary>
		/// <param name="uri"></param>
		/// <returns>
		/// the target host if the URI is absolute or <code>null</null> if the URI is
		/// relative or does not contain a valid host name.
		/// </returns>
		/// <since>4.1</since>
		public static HttpHost ExtractHost(URI uri)
		{
			if (uri == null)
			{
				return null;
			}
			HttpHost target = null;
			if (uri.IsAbsolute())
			{
				int port = uri.GetPort();
				// may be overridden later
				string host = uri.GetHost();
				if (host == null)
				{
					// normal parse failed; let's do it ourselves
					// authority does not seem to care about the valid character-set for host names
					host = uri.GetAuthority();
					if (host != null)
					{
						// Strip off any leading user credentials
						int at = host.IndexOf('@');
						if (at >= 0)
						{
							if (host.Length > at + 1)
							{
								host = Sharpen.Runtime.Substring(host, at + 1);
							}
							else
							{
								host = null;
							}
						}
						// @ on its own
						// Extract the port suffix, if present
						if (host != null)
						{
							int colon = host.IndexOf(':');
							if (colon >= 0)
							{
								int pos = colon + 1;
								int len = 0;
								for (int i = pos; i < host.Length; i++)
								{
									if (char.IsDigit(host[i]))
									{
										len++;
									}
									else
									{
										break;
									}
								}
								if (len > 0)
								{
									try
									{
										port = System.Convert.ToInt32(Sharpen.Runtime.Substring(host, pos, pos + len));
									}
									catch (FormatException)
									{
									}
								}
								host = Sharpen.Runtime.Substring(host, 0, colon);
							}
						}
					}
				}
				string scheme = uri.GetScheme();
				if (host != null)
				{
					target = new HttpHost(host, port, scheme);
				}
			}
			return target;
		}

		/// <summary>
		/// Derives the interpreted (absolute) URI that was used to generate the last
		/// request.
		/// </summary>
		/// <remarks>
		/// Derives the interpreted (absolute) URI that was used to generate the last
		/// request. This is done by extracting the request-uri and target origin for
		/// the last request and scanning all the redirect locations for the last
		/// fragment identifier, then combining the result into a
		/// <see cref="Sharpen.URI">Sharpen.URI</see>
		/// .
		/// </remarks>
		/// <param name="originalURI">original request before any redirects</param>
		/// <param name="target">
		/// if the last URI is relative, it is resolved against this target,
		/// or <code>null</code> if not available.
		/// </param>
		/// <param name="redirects">
		/// collection of redirect locations since the original request
		/// or <code>null</code> if not available.
		/// </param>
		/// <returns>interpreted (absolute) URI</returns>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public static URI Resolve(URI originalURI, HttpHost target, IList<URI> redirects)
		{
			Args.NotNull(originalURI, "Request URI");
			URIBuilder uribuilder;
			if (redirects == null || redirects.IsEmpty())
			{
				uribuilder = new URIBuilder(originalURI);
			}
			else
			{
				uribuilder = new URIBuilder(redirects[redirects.Count - 1]);
				string frag = uribuilder.GetFragment();
				// read interpreted fragment identifier from redirect locations
				for (int i = redirects.Count - 1; frag == null && i >= 0; i--)
				{
					frag = redirects[i].GetFragment();
				}
				uribuilder.SetFragment(frag);
			}
			// read interpreted fragment identifier from original request
			if (uribuilder.GetFragment() == null)
			{
				uribuilder.SetFragment(originalURI.GetFragment());
			}
			// last target origin
			if (target != null && !uribuilder.IsAbsolute())
			{
				uribuilder.SetScheme(target.GetSchemeName());
				uribuilder.SetHost(target.GetHostName());
				uribuilder.SetPort(target.GetPort());
			}
			return uribuilder.Build();
		}

		/// <summary>This class should not be instantiated.</summary>
		/// <remarks>This class should not be instantiated.</remarks>
		private URIUtils()
		{
		}
	}
}
