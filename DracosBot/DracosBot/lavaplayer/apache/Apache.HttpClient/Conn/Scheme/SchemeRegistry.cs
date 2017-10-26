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
using Apache.Http;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Conn.Scheme
{
	/// <summary>
	/// A set of supported protocol
	/// <see cref="Scheme">Scheme</see>
	/// s.
	/// Schemes are identified by lowercase names.
	/// </summary>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) use Apache.Http.Config.Registry{I}")]
	public sealed class SchemeRegistry
	{
		/// <summary>The available schemes in this registry.</summary>
		/// <remarks>The available schemes in this registry.</remarks>
		private readonly ConcurrentHashMap<string, Apache.Http.Conn.Scheme.Scheme> registeredSchemes;

		/// <summary>Creates a new, empty scheme registry.</summary>
		/// <remarks>Creates a new, empty scheme registry.</remarks>
		public SchemeRegistry() : base()
		{
			registeredSchemes = new ConcurrentHashMap<string, Apache.Http.Conn.Scheme.Scheme>
				();
		}

		/// <summary>Obtains a scheme by name.</summary>
		/// <remarks>Obtains a scheme by name.</remarks>
		/// <param name="name">the name of the scheme to look up (in lowercase)</param>
		/// <returns>the scheme, never <code>null</code></returns>
		/// <exception cref="System.InvalidOperationException">if the scheme with the given name is not registered
		/// 	</exception>
		public Apache.Http.Conn.Scheme.Scheme GetScheme(string name)
		{
			Apache.Http.Conn.Scheme.Scheme found = Get(name);
			if (found == null)
			{
				throw new InvalidOperationException("Scheme '" + name + "' not registered.");
			}
			return found;
		}

		/// <summary>Obtains the scheme for a host.</summary>
		/// <remarks>
		/// Obtains the scheme for a host.
		/// Convenience method for <code>getScheme(host.getSchemeName())</pre>
		/// </remarks>
		/// <param name="host">the host for which to obtain the scheme</param>
		/// <returns>the scheme for the given host, never <code>null</code></returns>
		/// <exception cref="System.InvalidOperationException">if a scheme with the respective name is not registered
		/// 	</exception>
		public Apache.Http.Conn.Scheme.Scheme GetScheme(HttpHost host)
		{
			Args.NotNull(host, "Host");
			return GetScheme(host.GetSchemeName());
		}

		/// <summary>Obtains a scheme by name, if registered.</summary>
		/// <remarks>Obtains a scheme by name, if registered.</remarks>
		/// <param name="name">the name of the scheme to look up (in lowercase)</param>
		/// <returns>
		/// the scheme, or
		/// <code>null</code> if there is none by this name
		/// </returns>
		public Apache.Http.Conn.Scheme.Scheme Get(string name)
		{
			Args.NotNull(name, "Scheme name");
			// leave it to the caller to use the correct name - all lowercase
			//name = name.toLowerCase();
			Apache.Http.Conn.Scheme.Scheme found = registeredSchemes.Get(name);
			return found;
		}

		/// <summary>Registers a scheme.</summary>
		/// <remarks>
		/// Registers a scheme.
		/// The scheme can later be retrieved by its name
		/// using
		/// <see cref="GetScheme(string)">getScheme</see>
		/// or
		/// <see cref="Get(string)">get</see>
		/// .
		/// </remarks>
		/// <param name="sch">the scheme to register</param>
		/// <returns>
		/// the scheme previously registered with that name, or
		/// <code>null</code> if none was registered
		/// </returns>
		public Apache.Http.Conn.Scheme.Scheme Register(Apache.Http.Conn.Scheme.Scheme sch
			)
		{
			Args.NotNull(sch, "Scheme");
			Apache.Http.Conn.Scheme.Scheme old = registeredSchemes.Put(sch.GetName(), sch);
			return old;
		}

		/// <summary>Unregisters a scheme.</summary>
		/// <remarks>Unregisters a scheme.</remarks>
		/// <param name="name">the name of the scheme to unregister (in lowercase)</param>
		/// <returns>
		/// the unregistered scheme, or
		/// <code>null</code> if there was none
		/// </returns>
		public Apache.Http.Conn.Scheme.Scheme Unregister(string name)
		{
			Args.NotNull(name, "Scheme name");
			// leave it to the caller to use the correct name - all lowercase
			//name = name.toLowerCase();
			Apache.Http.Conn.Scheme.Scheme gone = Sharpen.Collections.Remove(registeredSchemes
				, name);
			return gone;
		}

		/// <summary>Obtains the names of the registered schemes.</summary>
		/// <remarks>Obtains the names of the registered schemes.</remarks>
		/// <returns>List containing registered scheme names.</returns>
		public IList<string> GetSchemeNames()
		{
			return new AList<string>(registeredSchemes.Keys);
		}

		/// <summary>
		/// Populates the internal collection of registered
		/// <see cref="Scheme">protocol schemes</see>
		/// with the content of the map passed as a parameter.
		/// </summary>
		/// <param name="map">protocol schemes</param>
		public void SetItems(IDictionary<string, Apache.Http.Conn.Scheme.Scheme> map)
		{
			if (map == null)
			{
				return;
			}
			registeredSchemes.Clear();
			registeredSchemes.PutAll(map);
		}
	}
}
