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
using System.Text;
using Apache.Http.Conn.Scheme;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Conn.Scheme
{
	/// <summary>Encapsulates specifics of a protocol scheme such as "http" or "https".</summary>
	/// <remarks>
	/// Encapsulates specifics of a protocol scheme such as "http" or "https". Schemes are identified
	/// by lowercase names. Supported schemes are typically collected in a
	/// <see cref="SchemeRegistry">SchemeRegistry</see>
	/// .
	/// <p/>
	/// For example, to configure support for "https://" URLs, you could write code like the following:
	/// <pre>
	/// Scheme https = new Scheme("https", 443, new MySecureSocketFactory());
	/// SchemeRegistry registry = new SchemeRegistry();
	/// registry.register(https);
	/// </pre>
	/// </remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) use Apache.Http.Conn.SchemePortResolver for default port resolution and Apache.Http.Config.Registry{I} for socket factory lookups."
		)]
	public sealed class Scheme
	{
		/// <summary>The name of this scheme, in lowercase.</summary>
		/// <remarks>The name of this scheme, in lowercase. (e.g. http, https)</remarks>
		private readonly string name;

		/// <summary>The socket factory for this scheme</summary>
		private readonly SchemeSocketFactory socketFactory;

		/// <summary>The default port for this scheme</summary>
		private readonly int defaultPort;

		/// <summary>Indicates whether this scheme allows for layered connections</summary>
		private readonly bool layered;

		/// <summary>
		/// A string representation, for
		/// <see cref="ToString()">toString</see>
		/// .
		/// </summary>
		private string stringRep;

		/// <summary>Creates a new scheme.</summary>
		/// <remarks>
		/// Creates a new scheme.
		/// Whether the created scheme allows for layered connections
		/// depends on the class of <code>factory</code>.
		/// </remarks>
		/// <param name="name">
		/// the scheme name, for example "http".
		/// The name will be converted to lowercase.
		/// </param>
		/// <param name="port">the default port for this scheme</param>
		/// <param name="factory">
		/// the factory for creating sockets for communication
		/// with this scheme
		/// </param>
		/// <since>4.1</since>
		public Scheme(string name, int port, SchemeSocketFactory factory)
		{
			Args.NotNull(name, "Scheme name");
			Args.Check(port > 0 && port <= unchecked((int)(0xffff)), "Port is invalid");
			Args.NotNull(factory, "Socket factory");
			this.name = name.ToLower(Sharpen.Extensions.GetEnglishCulture());
			this.defaultPort = port;
			if (factory is SchemeLayeredSocketFactory)
			{
				this.layered = true;
				this.socketFactory = factory;
			}
			else
			{
				if (factory is LayeredSchemeSocketFactory)
				{
					this.layered = true;
					this.socketFactory = new SchemeLayeredSocketFactoryAdaptor2((LayeredSchemeSocketFactory
						)factory);
				}
				else
				{
					this.layered = false;
					this.socketFactory = factory;
				}
			}
		}

		/// <summary>Creates a new scheme.</summary>
		/// <remarks>
		/// Creates a new scheme.
		/// Whether the created scheme allows for layered connections
		/// depends on the class of <code>factory</code>.
		/// </remarks>
		/// <param name="name">
		/// the scheme name, for example "http".
		/// The name will be converted to lowercase.
		/// </param>
		/// <param name="factory">
		/// the factory for creating sockets for communication
		/// with this scheme
		/// </param>
		/// <param name="port">the default port for this scheme</param>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.1)  Use Scheme(string, int, SchemeSocketFactory)")]
		public Scheme(string name, SocketFactory factory, int port)
		{
			Args.NotNull(name, "Scheme name");
			Args.NotNull(factory, "Socket factory");
			Args.Check(port > 0 && port <= unchecked((int)(0xffff)), "Port is invalid");
			this.name = name.ToLower(Sharpen.Extensions.GetEnglishCulture());
			if (factory is LayeredSocketFactory)
			{
				this.socketFactory = new SchemeLayeredSocketFactoryAdaptor((LayeredSocketFactory)
					factory);
				this.layered = true;
			}
			else
			{
				this.socketFactory = new SchemeSocketFactoryAdaptor(factory);
				this.layered = false;
			}
			this.defaultPort = port;
		}

		/// <summary>Obtains the default port.</summary>
		/// <remarks>Obtains the default port.</remarks>
		/// <returns>the default port for this scheme</returns>
		public int GetDefaultPort()
		{
			return defaultPort;
		}

		/// <summary>Obtains the socket factory.</summary>
		/// <remarks>
		/// Obtains the socket factory.
		/// If this scheme is
		/// <see cref="IsLayered()">layered</see>
		/// , the factory implements
		/// <see cref="LayeredSocketFactory">LayeredSocketFactory</see>
		/// .
		/// </remarks>
		/// <returns>the socket factory for this scheme</returns>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.1)  Use GetSchemeSocketFactory()")]
		public SocketFactory GetSocketFactory()
		{
			if (this.socketFactory is SchemeSocketFactoryAdaptor)
			{
				return ((SchemeSocketFactoryAdaptor)this.socketFactory).GetFactory();
			}
			else
			{
				if (this.layered)
				{
					return new LayeredSocketFactoryAdaptor((LayeredSchemeSocketFactory)this.socketFactory
						);
				}
				else
				{
					return new SocketFactoryAdaptor(this.socketFactory);
				}
			}
		}

		/// <summary>Obtains the socket factory.</summary>
		/// <remarks>
		/// Obtains the socket factory.
		/// If this scheme is
		/// <see cref="IsLayered()">layered</see>
		/// , the factory implements
		/// <see cref="LayeredSocketFactory">LayeredSchemeSocketFactory</see>
		/// .
		/// </remarks>
		/// <returns>the socket factory for this scheme</returns>
		/// <since>4.1</since>
		public SchemeSocketFactory GetSchemeSocketFactory()
		{
			return this.socketFactory;
		}

		/// <summary>Obtains the scheme name.</summary>
		/// <remarks>Obtains the scheme name.</remarks>
		/// <returns>the name of this scheme, in lowercase</returns>
		public string GetName()
		{
			return name;
		}

		/// <summary>Indicates whether this scheme allows for layered connections.</summary>
		/// <remarks>Indicates whether this scheme allows for layered connections.</remarks>
		/// <returns>
		/// <code>true</code> if layered connections are possible,
		/// <code>false</code> otherwise
		/// </returns>
		public bool IsLayered()
		{
			return layered;
		}

		/// <summary>Resolves the correct port for this scheme.</summary>
		/// <remarks>
		/// Resolves the correct port for this scheme.
		/// Returns the given port if it is valid, the default port otherwise.
		/// </remarks>
		/// <param name="port">
		/// the port to be resolved,
		/// a negative number to obtain the default port
		/// </param>
		/// <returns>the given port or the defaultPort</returns>
		public int ResolvePort(int port)
		{
			return port <= 0 ? defaultPort : port;
		}

		/// <summary>Return a string representation of this object.</summary>
		/// <remarks>Return a string representation of this object.</remarks>
		/// <returns>a human-readable string description of this scheme</returns>
		public sealed override string ToString()
		{
			if (stringRep == null)
			{
				StringBuilder buffer = new StringBuilder();
				buffer.Append(this.name);
				buffer.Append(':');
				buffer.Append(Sharpen.Extensions.ToString(this.defaultPort));
				stringRep = buffer.ToString();
			}
			return stringRep;
		}

		public sealed override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj is Apache.Http.Conn.Scheme.Scheme)
			{
				Apache.Http.Conn.Scheme.Scheme that = (Apache.Http.Conn.Scheme.Scheme)obj;
				return this.name.Equals(that.name) && this.defaultPort == that.defaultPort && this
					.layered == that.layered;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			int hash = LangUtils.HashSeed;
			hash = LangUtils.HashCode(hash, this.defaultPort);
			hash = LangUtils.HashCode(hash, this.name);
			hash = LangUtils.HashCode(hash, this.layered);
			return hash;
		}
	}
}
