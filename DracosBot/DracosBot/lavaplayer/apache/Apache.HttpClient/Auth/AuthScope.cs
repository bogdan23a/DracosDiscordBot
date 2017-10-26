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

using System.Text;
using Apache.Http;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Auth
{
	/// <summary>
	/// The class represents an authentication scope consisting of a host name,
	/// a port number, a realm name and an authentication scheme name which
	/// <see cref="Credentials">Credentials</see>
	/// apply to.
	/// </summary>
	/// <since>4.0</since>
	public class AuthScope
	{
		/// <summary>The <tt>null</tt> value represents any host.</summary>
		/// <remarks>
		/// The <tt>null</tt> value represents any host. In the future versions of
		/// HttpClient the use of this parameter will be discontinued.
		/// </remarks>
		public static readonly string AnyHost = null;

		/// <summary>The <tt>-1</tt> value represents any port.</summary>
		/// <remarks>The <tt>-1</tt> value represents any port.</remarks>
		public const int AnyPort = -1;

		/// <summary>The <tt>null</tt> value represents any realm.</summary>
		/// <remarks>The <tt>null</tt> value represents any realm.</remarks>
		public static readonly string AnyRealm = null;

		/// <summary>The <tt>null</tt> value represents any authentication scheme.</summary>
		/// <remarks>The <tt>null</tt> value represents any authentication scheme.</remarks>
		public static readonly string AnyScheme = null;

		/// <summary>Default scope matching any host, port, realm and authentication scheme.</summary>
		/// <remarks>
		/// Default scope matching any host, port, realm and authentication scheme.
		/// In the future versions of HttpClient the use of this parameter will be
		/// discontinued.
		/// </remarks>
		public static readonly Apache.Http.Auth.AuthScope Any = new Apache.Http.Auth.AuthScope
			(AnyHost, AnyPort, AnyRealm, AnyScheme);

		/// <summary>The authentication scheme the credentials apply to.</summary>
		/// <remarks>The authentication scheme the credentials apply to.</remarks>
		private readonly string scheme;

		/// <summary>The realm the credentials apply to.</summary>
		/// <remarks>The realm the credentials apply to.</remarks>
		private readonly string realm;

		/// <summary>The host the credentials apply to.</summary>
		/// <remarks>The host the credentials apply to.</remarks>
		private readonly string host;

		/// <summary>The port the credentials apply to.</summary>
		/// <remarks>The port the credentials apply to.</remarks>
		private readonly int port;

		/// <summary>
		/// Creates a new credentials scope for the given
		/// <tt>host</tt>, <tt>port</tt>, <tt>realm</tt>, and
		/// <tt>authentication scheme</tt>.
		/// </summary>
		/// <remarks>
		/// Creates a new credentials scope for the given
		/// <tt>host</tt>, <tt>port</tt>, <tt>realm</tt>, and
		/// <tt>authentication scheme</tt>.
		/// </remarks>
		/// <param name="host">
		/// the host the credentials apply to. May be set
		/// to <tt>null</tt> if credentials are applicable to
		/// any host.
		/// </param>
		/// <param name="port">
		/// the port the credentials apply to. May be set
		/// to negative value if credentials are applicable to
		/// any port.
		/// </param>
		/// <param name="realm">
		/// the realm the credentials apply to. May be set
		/// to <tt>null</tt> if credentials are applicable to
		/// any realm.
		/// </param>
		/// <param name="scheme">
		/// the authentication scheme the credentials apply to.
		/// May be set to <tt>null</tt> if credentials are applicable to
		/// any authentication scheme.
		/// </param>
		public AuthScope(string host, int port, string realm, string scheme)
		{
			this.host = (host == null) ? AnyHost : host.ToLower(Sharpen.Extensions.GetEnglishCulture()
				);
			this.port = (port < 0) ? AnyPort : port;
			this.realm = (realm == null) ? AnyRealm : realm;
			this.scheme = (scheme == null) ? AnyScheme : scheme.ToUpper(Sharpen.Extensions.GetEnglishCulture()
				);
		}

		/// <since>4.2</since>
		public AuthScope(HttpHost host, string realm, string schemeName) : this(host.GetHostName
			(), host.GetPort(), realm, schemeName)
		{
		}

		/// <since>4.2</since>
		public AuthScope(HttpHost host) : this(host, AnyRealm, AnyScheme)
		{
		}

		/// <summary>
		/// Creates a new credentials scope for the given
		/// <tt>host</tt>, <tt>port</tt>, <tt>realm</tt>, and any
		/// authentication scheme.
		/// </summary>
		/// <remarks>
		/// Creates a new credentials scope for the given
		/// <tt>host</tt>, <tt>port</tt>, <tt>realm</tt>, and any
		/// authentication scheme.
		/// </remarks>
		/// <param name="host">
		/// the host the credentials apply to. May be set
		/// to <tt>null</tt> if credentials are applicable to
		/// any host.
		/// </param>
		/// <param name="port">
		/// the port the credentials apply to. May be set
		/// to negative value if credentials are applicable to
		/// any port.
		/// </param>
		/// <param name="realm">
		/// the realm the credentials apply to. May be set
		/// to <tt>null</tt> if credentials are applicable to
		/// any realm.
		/// </param>
		public AuthScope(string host, int port, string realm) : this(host, port, realm, AnyScheme
			)
		{
		}

		/// <summary>
		/// Creates a new credentials scope for the given
		/// <tt>host</tt>, <tt>port</tt>, any realm name, and any
		/// authentication scheme.
		/// </summary>
		/// <remarks>
		/// Creates a new credentials scope for the given
		/// <tt>host</tt>, <tt>port</tt>, any realm name, and any
		/// authentication scheme.
		/// </remarks>
		/// <param name="host">
		/// the host the credentials apply to. May be set
		/// to <tt>null</tt> if credentials are applicable to
		/// any host.
		/// </param>
		/// <param name="port">
		/// the port the credentials apply to. May be set
		/// to negative value if credentials are applicable to
		/// any port.
		/// </param>
		public AuthScope(string host, int port) : this(host, port, AnyRealm, AnyScheme)
		{
		}

		/// <summary>Creates a copy of the given credentials scope.</summary>
		/// <remarks>Creates a copy of the given credentials scope.</remarks>
		public AuthScope(Apache.Http.Auth.AuthScope authscope) : base()
		{
			Args.NotNull(authscope, "Scope");
			this.host = authscope.GetHost();
			this.port = authscope.GetPort();
			this.realm = authscope.GetRealm();
			this.scheme = authscope.GetScheme();
		}

		/// <returns>the host</returns>
		public virtual string GetHost()
		{
			return this.host;
		}

		/// <returns>the port</returns>
		public virtual int GetPort()
		{
			return this.port;
		}

		/// <returns>the realm name</returns>
		public virtual string GetRealm()
		{
			return this.realm;
		}

		/// <returns>the scheme type</returns>
		public virtual string GetScheme()
		{
			return this.scheme;
		}

		/// <summary>Tests if the authentication scopes match.</summary>
		/// <remarks>Tests if the authentication scopes match.</remarks>
		/// <returns>
		/// the match factor. Negative value signifies no match.
		/// Non-negative signifies a match. The greater the returned value
		/// the closer the match.
		/// </returns>
		public virtual int Match(Apache.Http.Auth.AuthScope that)
		{
			int factor = 0;
			if (LangUtils.Equals(this.scheme, that.scheme))
			{
				factor += 1;
			}
			else
			{
				if (this.scheme != AnyScheme && that.scheme != AnyScheme)
				{
					return -1;
				}
			}
			if (LangUtils.Equals(this.realm, that.realm))
			{
				factor += 2;
			}
			else
			{
				if (this.realm != AnyRealm && that.realm != AnyRealm)
				{
					return -1;
				}
			}
			if (this.port == that.port)
			{
				factor += 4;
			}
			else
			{
				if (this.port != AnyPort && that.port != AnyPort)
				{
					return -1;
				}
			}
			if (LangUtils.Equals(this.host, that.host))
			{
				factor += 8;
			}
			else
			{
				if (this.host != AnyHost && that.host != AnyHost)
				{
					return -1;
				}
			}
			return factor;
		}

		/// <seealso cref="object.Equals(object)">object.Equals(object)</seealso>
		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			if (o == this)
			{
				return true;
			}
			if (!(o is Apache.Http.Auth.AuthScope))
			{
				return base.Equals(o);
			}
			Apache.Http.Auth.AuthScope that = (Apache.Http.Auth.AuthScope)o;
			return LangUtils.Equals(this.host, that.host) && this.port == that.port && LangUtils
				.Equals(this.realm, that.realm) && LangUtils.Equals(this.scheme, that.scheme);
		}

		/// <seealso cref="object.ToString()">object.ToString()</seealso>
		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			if (this.scheme != null)
			{
				buffer.Append(this.scheme.ToUpper(Sharpen.Extensions.GetEnglishCulture()));
				buffer.Append(' ');
			}
			if (this.realm != null)
			{
				buffer.Append('\'');
				buffer.Append(this.realm);
				buffer.Append('\'');
			}
			else
			{
				buffer.Append("<any realm>");
			}
			if (this.host != null)
			{
				buffer.Append('@');
				buffer.Append(this.host);
				if (this.port >= 0)
				{
					buffer.Append(':');
					buffer.Append(this.port);
				}
			}
			return buffer.ToString();
		}

		/// <seealso cref="object.GetHashCode()">object.GetHashCode()</seealso>
		public override int GetHashCode()
		{
			int hash = LangUtils.HashSeed;
			hash = LangUtils.HashCode(hash, this.host);
			hash = LangUtils.HashCode(hash, this.port);
			hash = LangUtils.HashCode(hash, this.realm);
			hash = LangUtils.HashCode(hash, this.scheme);
			return hash;
		}
	}
}
