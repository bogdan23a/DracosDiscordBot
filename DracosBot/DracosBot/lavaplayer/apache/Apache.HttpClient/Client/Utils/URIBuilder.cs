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
using Apache.Http.Conn.Util;
using Apache.Http.Message;
using Sharpen;

namespace Apache.Http.Client.Utils
{
	/// <summary>
	/// Builder for
	/// <see cref="Sharpen.URI">Sharpen.URI</see>
	/// instances.
	/// </summary>
	/// <since>4.2</since>
	public class URIBuilder
	{
		private string scheme;

		private string encodedSchemeSpecificPart;

		private string encodedAuthority;

		private string userInfo;

		private string encodedUserInfo;

		private string host;

		private int port;

		private string path;

		private string encodedPath;

		private string encodedQuery;

		private IList<NameValuePair> queryParams;

		private string query;

		private string fragment;

		private string encodedFragment;

		/// <summary>Constructs an empty instance.</summary>
		/// <remarks>Constructs an empty instance.</remarks>
		public URIBuilder() : base()
		{
			this.port = -1;
		}

		/// <summary>Construct an instance from the string which must be a valid URI.</summary>
		/// <remarks>Construct an instance from the string which must be a valid URI.</remarks>
		/// <param name="string">a valid URI in string form</param>
		/// <exception cref="Sharpen.URISyntaxException">if the input is not a valid URI</exception>
		public URIBuilder(string @string) : base()
		{
			DigestURI(new URI(@string));
		}

		/// <summary>Construct an instance from the provided URI.</summary>
		/// <remarks>Construct an instance from the provided URI.</remarks>
		/// <param name="uri"></param>
		public URIBuilder(URI uri) : base()
		{
			DigestURI(uri);
		}

		private IList<NameValuePair> ParseQuery(string query, Encoding charset)
		{
			if (query != null && query.Length > 0)
			{
				return URLEncodedUtils.Parse(query, charset);
			}
			return null;
		}

		/// <summary>
		/// Builds a
		/// <see cref="Sharpen.URI">Sharpen.URI</see>
		/// instance.
		/// </summary>
		/// <exception cref="Sharpen.URISyntaxException"></exception>
		public virtual URI Build()
		{
			return new URI(BuildString());
		}

		private string BuildString()
		{
			StringBuilder sb = new StringBuilder();
			if (this.scheme != null)
			{
				sb.Append(this.scheme).Append(':');
			}
			if (this.encodedSchemeSpecificPart != null)
			{
				sb.Append(this.encodedSchemeSpecificPart);
			}
			else
			{
				if (this.encodedAuthority != null)
				{
					sb.Append("//").Append(this.encodedAuthority);
				}
				else
				{
					if (this.host != null)
					{
						sb.Append("//");
						if (this.encodedUserInfo != null)
						{
							sb.Append(this.encodedUserInfo).Append("@");
						}
						else
						{
							if (this.userInfo != null)
							{
								sb.Append(EncodeUserInfo(this.userInfo)).Append("@");
							}
						}
						if (InetAddressUtils.IsIPv6Address(this.host))
						{
							sb.Append("[").Append(this.host).Append("]");
						}
						else
						{
							sb.Append(this.host);
						}
						if (this.port >= 0)
						{
							sb.Append(":").Append(this.port);
						}
					}
				}
				if (this.encodedPath != null)
				{
					sb.Append(NormalizePath(this.encodedPath));
				}
				else
				{
					if (this.path != null)
					{
						sb.Append(EncodePath(NormalizePath(this.path)));
					}
				}
				if (this.encodedQuery != null)
				{
					sb.Append("?").Append(this.encodedQuery);
				}
				else
				{
					if (this.queryParams != null)
					{
						sb.Append("?").Append(EncodeUrlForm(this.queryParams));
					}
					else
					{
						if (this.query != null)
						{
							sb.Append("?").Append(EncodeUric(this.query));
						}
					}
				}
			}
			if (this.encodedFragment != null)
			{
				sb.Append("#").Append(this.encodedFragment);
			}
			else
			{
				if (this.fragment != null)
				{
					sb.Append("#").Append(EncodeUric(this.fragment));
				}
			}
			return sb.ToString();
		}

		private void DigestURI(URI uri)
		{
			this.scheme = uri.GetScheme();
			this.encodedSchemeSpecificPart = uri.GetRawSchemeSpecificPart();
			this.encodedAuthority = uri.GetRawAuthority();
			this.host = uri.GetHost();
			this.port = uri.GetPort();
			this.encodedUserInfo = uri.GetRawUserInfo();
			this.userInfo = uri.GetUserInfo();
			this.encodedPath = uri.GetRawPath();
			this.path = uri.GetPath();
			this.encodedQuery = uri.GetRawQuery();
			this.queryParams = ParseQuery(uri.GetRawQuery(), Consts.Utf8);
			this.encodedFragment = uri.GetRawFragment();
			this.fragment = uri.GetFragment();
		}

		private string EncodeUserInfo(string userInfo)
		{
			return URLEncodedUtils.EncUserInfo(userInfo, Consts.Utf8);
		}

		private string EncodePath(string path)
		{
			return URLEncodedUtils.EncPath(path, Consts.Utf8);
		}

		private string EncodeUrlForm(IList<NameValuePair> @params)
		{
			return URLEncodedUtils.Format(@params, Consts.Utf8);
		}

		private string EncodeUric(string fragment)
		{
			return URLEncodedUtils.EncUric(fragment, Consts.Utf8);
		}

		/// <summary>Sets URI scheme.</summary>
		/// <remarks>Sets URI scheme.</remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder SetScheme(string scheme)
		{
			this.scheme = scheme;
			return this;
		}

		/// <summary>Sets URI user info.</summary>
		/// <remarks>
		/// Sets URI user info. The value is expected to be unescaped and may contain non ASCII
		/// characters.
		/// </remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder SetUserInfo(string userInfo)
		{
			this.userInfo = userInfo;
			this.encodedSchemeSpecificPart = null;
			this.encodedAuthority = null;
			this.encodedUserInfo = null;
			return this;
		}

		/// <summary>Sets URI user info as a combination of username and password.</summary>
		/// <remarks>
		/// Sets URI user info as a combination of username and password. These values are expected to
		/// be unescaped and may contain non ASCII characters.
		/// </remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder SetUserInfo(string username, string
			 password)
		{
			return SetUserInfo(username + ':' + password);
		}

		/// <summary>Sets URI host.</summary>
		/// <remarks>Sets URI host.</remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder SetHost(string host)
		{
			this.host = host;
			this.encodedSchemeSpecificPart = null;
			this.encodedAuthority = null;
			return this;
		}

		/// <summary>Sets URI port.</summary>
		/// <remarks>Sets URI port.</remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder SetPort(int port)
		{
			this.port = port < 0 ? -1 : port;
			this.encodedSchemeSpecificPart = null;
			this.encodedAuthority = null;
			return this;
		}

		/// <summary>Sets URI path.</summary>
		/// <remarks>Sets URI path. The value is expected to be unescaped and may contain non ASCII characters.
		/// 	</remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder SetPath(string path)
		{
			this.path = path;
			this.encodedSchemeSpecificPart = null;
			this.encodedPath = null;
			return this;
		}

		/// <summary>Removes URI query.</summary>
		/// <remarks>Removes URI query.</remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder RemoveQuery()
		{
			this.queryParams = null;
			this.query = null;
			this.encodedQuery = null;
			this.encodedSchemeSpecificPart = null;
			return this;
		}

		/// <summary>Sets URI query.</summary>
		/// <remarks>
		/// Sets URI query.
		/// <p>
		/// The value is expected to be encoded form data.
		/// </remarks>
		/// <seealso cref="URLEncodedUtils.Parse(Apache.Http.HttpEntity)">URLEncodedUtils.Parse(Apache.Http.HttpEntity)
		/// 	</seealso>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.3) use SetParameters(System.Collections.Generic.IList{E}) or SetParameters(Apache.Http.NameValuePair[])"
			)]
		public virtual Apache.Http.Client.Utils.URIBuilder SetQuery(string query)
		{
			this.queryParams = ParseQuery(query, Consts.Utf8);
			this.query = null;
			this.encodedQuery = null;
			this.encodedSchemeSpecificPart = null;
			return this;
		}

		/// <summary>Sets URI query parameters.</summary>
		/// <remarks>
		/// Sets URI query parameters. The parameter name / values are expected to be unescaped
		/// and may contain non ASCII characters.
		/// <p/>
		/// Please note query parameters and custom query component are mutually exclusive. This method
		/// will remove custom query if present.
		/// </remarks>
		/// <since>4.3</since>
		public virtual Apache.Http.Client.Utils.URIBuilder SetParameters(IList<NameValuePair
			> nvps)
		{
			if (this.queryParams == null)
			{
				this.queryParams = new AList<NameValuePair>();
			}
			else
			{
				this.queryParams.Clear();
			}
			Sharpen.Collections.AddAll(this.queryParams, nvps);
			this.encodedQuery = null;
			this.encodedSchemeSpecificPart = null;
			this.query = null;
			return this;
		}

		/// <summary>Adds URI query parameters.</summary>
		/// <remarks>
		/// Adds URI query parameters. The parameter name / values are expected to be unescaped
		/// and may contain non ASCII characters.
		/// <p/>
		/// Please note query parameters and custom query component are mutually exclusive. This method
		/// will remove custom query if present.
		/// </remarks>
		/// <since>4.3</since>
		public virtual Apache.Http.Client.Utils.URIBuilder AddParameters(IList<NameValuePair
			> nvps)
		{
			if (this.queryParams == null)
			{
				this.queryParams = new AList<NameValuePair>();
			}
			Sharpen.Collections.AddAll(this.queryParams, nvps);
			this.encodedQuery = null;
			this.encodedSchemeSpecificPart = null;
			this.query = null;
			return this;
		}

		/// <summary>Sets URI query parameters.</summary>
		/// <remarks>
		/// Sets URI query parameters. The parameter name / values are expected to be unescaped
		/// and may contain non ASCII characters.
		/// <p/>
		/// Please note query parameters and custom query component are mutually exclusive. This method
		/// will remove custom query if present.
		/// </remarks>
		/// <since>4.3</since>
		public virtual Apache.Http.Client.Utils.URIBuilder SetParameters(params NameValuePair
			[] nvps)
		{
			if (this.queryParams == null)
			{
				this.queryParams = new AList<NameValuePair>();
			}
			else
			{
				this.queryParams.Clear();
			}
			foreach (NameValuePair nvp in nvps)
			{
				this.queryParams.AddItem(nvp);
			}
			this.encodedQuery = null;
			this.encodedSchemeSpecificPart = null;
			this.query = null;
			return this;
		}

		/// <summary>Adds parameter to URI query.</summary>
		/// <remarks>
		/// Adds parameter to URI query. The parameter name and value are expected to be unescaped
		/// and may contain non ASCII characters.
		/// <p/>
		/// Please note query parameters and custom query component are mutually exclusive. This method
		/// will remove custom query if present.
		/// </remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder AddParameter(string param, string
			 value)
		{
			if (this.queryParams == null)
			{
				this.queryParams = new AList<NameValuePair>();
			}
			this.queryParams.AddItem(new BasicNameValuePair(param, value));
			this.encodedQuery = null;
			this.encodedSchemeSpecificPart = null;
			this.query = null;
			return this;
		}

		/// <summary>Sets parameter of URI query overriding existing value if set.</summary>
		/// <remarks>
		/// Sets parameter of URI query overriding existing value if set. The parameter name and value
		/// are expected to be unescaped and may contain non ASCII characters.
		/// <p/>
		/// Please note query parameters and custom query component are mutually exclusive. This method
		/// will remove custom query if present.
		/// </remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder SetParameter(string param, string
			 value)
		{
			if (this.queryParams == null)
			{
				this.queryParams = new AList<NameValuePair>();
			}
			if (!this.queryParams.IsEmpty())
			{
				for (IEnumerator<NameValuePair> it = this.queryParams.GetEnumerator(); it.HasNext
					(); )
				{
					NameValuePair nvp = it.Next();
					if (nvp.GetName().Equals(param))
					{
						it.Remove();
					}
				}
			}
			this.queryParams.AddItem(new BasicNameValuePair(param, value));
			this.encodedQuery = null;
			this.encodedSchemeSpecificPart = null;
			this.query = null;
			return this;
		}

		/// <summary>Clears URI query parameters.</summary>
		/// <remarks>Clears URI query parameters.</remarks>
		/// <since>4.3</since>
		public virtual Apache.Http.Client.Utils.URIBuilder ClearParameters()
		{
			this.queryParams = null;
			this.encodedQuery = null;
			this.encodedSchemeSpecificPart = null;
			return this;
		}

		/// <summary>Sets custom URI query.</summary>
		/// <remarks>
		/// Sets custom URI query. The value is expected to be unescaped and may contain non ASCII
		/// characters.
		/// <p/>
		/// Please note query parameters and custom query component are mutually exclusive. This method
		/// will remove query parameters if present.
		/// </remarks>
		/// <since>4.3</since>
		public virtual Apache.Http.Client.Utils.URIBuilder SetCustomQuery(string query)
		{
			this.query = query;
			this.encodedQuery = null;
			this.encodedSchemeSpecificPart = null;
			this.queryParams = null;
			return this;
		}

		/// <summary>Sets URI fragment.</summary>
		/// <remarks>
		/// Sets URI fragment. The value is expected to be unescaped and may contain non ASCII
		/// characters.
		/// </remarks>
		public virtual Apache.Http.Client.Utils.URIBuilder SetFragment(string fragment)
		{
			this.fragment = fragment;
			this.encodedFragment = null;
			return this;
		}

		/// <since>4.3</since>
		public virtual bool IsAbsolute()
		{
			return this.scheme != null;
		}

		/// <since>4.3</since>
		public virtual bool IsOpaque()
		{
			return this.path == null;
		}

		public virtual string GetScheme()
		{
			return this.scheme;
		}

		public virtual string GetUserInfo()
		{
			return this.userInfo;
		}

		public virtual string GetHost()
		{
			return this.host;
		}

		public virtual int GetPort()
		{
			return this.port;
		}

		public virtual string GetPath()
		{
			return this.path;
		}

		public virtual IList<NameValuePair> GetQueryParams()
		{
			if (this.queryParams != null)
			{
				return new AList<NameValuePair>(this.queryParams);
			}
			else
			{
				return new AList<NameValuePair>();
			}
		}

		public virtual string GetFragment()
		{
			return this.fragment;
		}

		public override string ToString()
		{
			return BuildString();
		}

		private static string NormalizePath(string path)
		{
			string s = path;
			if (s == null)
			{
				return null;
			}
			int n = 0;
			for (; n < s.Length; n++)
			{
				if (s[n] != '/')
				{
					break;
				}
			}
			if (n > 1)
			{
				s = Sharpen.Runtime.Substring(s, n - 1);
			}
			return s;
		}
	}
}
