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
using Apache.Http.Auth;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Auth
{
	/// <summary>
	/// Authentication scheme registry that can be used to obtain the corresponding
	/// authentication scheme implementation for a given type of authorization challenge.
	/// </summary>
	/// <remarks>
	/// Authentication scheme registry that can be used to obtain the corresponding
	/// authentication scheme implementation for a given type of authorization challenge.
	/// </remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) use Apache.Http.Config.Registry{I}")]
	public sealed class AuthSchemeRegistry : Apache.Http.Config.Lookup<AuthSchemeProvider
		>
	{
		private readonly ConcurrentHashMap<string, AuthSchemeFactory> registeredSchemes;

		public AuthSchemeRegistry() : base()
		{
			this.registeredSchemes = new ConcurrentHashMap<string, AuthSchemeFactory>();
		}

		/// <summary>
		/// Registers a
		/// <see cref="AuthSchemeFactory">AuthSchemeFactory</see>
		/// with  the given identifier. If a factory with the
		/// given name already exists it will be overridden. This name is the same one used to
		/// retrieve the
		/// <see cref="AuthScheme">authentication scheme</see>
		/// from
		/// <see cref="GetAuthScheme(string, Apache.Http.Params.HttpParams)">GetAuthScheme(string, Apache.Http.Params.HttpParams)
		/// 	</see>
		/// .
		/// <p>
		/// Please note that custom authentication preferences, if used, need to be updated accordingly
		/// for the new
		/// <see cref="AuthScheme">authentication scheme</see>
		/// to take effect.
		/// </p>
		/// </summary>
		/// <param name="name">the identifier for this scheme</param>
		/// <param name="factory">
		/// the
		/// <see cref="AuthSchemeFactory">AuthSchemeFactory</see>
		/// class to register
		/// </param>
		/// <seealso cref="GetAuthScheme(string, Apache.Http.Params.HttpParams)">GetAuthScheme(string, Apache.Http.Params.HttpParams)
		/// 	</seealso>
		public void Register(string name, AuthSchemeFactory factory)
		{
			Args.NotNull(name, "Name");
			Args.NotNull(factory, "Authentication scheme factory");
			registeredSchemes.Put(name.ToLower(Sharpen.Extensions.GetEnglishCulture()), factory
				);
		}

		/// <summary>
		/// Unregisters the class implementing an
		/// <see cref="AuthScheme">authentication scheme</see>
		/// with
		/// the given name.
		/// </summary>
		/// <param name="name">the identifier of the class to unregister</param>
		public void Unregister(string name)
		{
			Args.NotNull(name, "Name");
			Sharpen.Collections.Remove(registeredSchemes, name.ToLower(Sharpen.Extensions.GetEnglishCulture()
				));
		}

		/// <summary>
		/// Gets the
		/// <see cref="AuthScheme">authentication scheme</see>
		/// with the given name.
		/// </summary>
		/// <param name="name">
		/// the
		/// <see cref="AuthScheme">authentication scheme</see>
		/// identifier
		/// </param>
		/// <param name="params">
		/// the
		/// <see cref="Apache.Http.Params.HttpParams">HTTP parameters</see>
		/// for the authentication
		/// scheme.
		/// </param>
		/// <returns>
		/// 
		/// <see cref="AuthScheme">authentication scheme</see>
		/// </returns>
		/// <exception cref="System.InvalidOperationException">if a scheme with the given name cannot be found
		/// 	</exception>
		public AuthScheme GetAuthScheme(string name, HttpParams @params)
		{
			Args.NotNull(name, "Name");
			AuthSchemeFactory factory = registeredSchemes.Get(name.ToLower(Sharpen.Extensions.GetEnglishCulture()
				));
			if (factory != null)
			{
				return factory.NewInstance(@params);
			}
			else
			{
				throw new InvalidOperationException("Unsupported authentication scheme: " + name);
			}
		}

		/// <summary>
		/// Obtains a list containing the names of all registered
		/// <see cref="AuthScheme">
		/// authentication
		/// schemes
		/// </see>
		/// </summary>
		/// <returns>list of registered scheme names</returns>
		public IList<string> GetSchemeNames()
		{
			return new AList<string>(registeredSchemes.Keys);
		}

		/// <summary>
		/// Populates the internal collection of registered
		/// <see cref="AuthScheme">authentication schemes</see>
		/// with the content of the map passed as a parameter.
		/// </summary>
		/// <param name="map">authentication schemes</param>
		public void SetItems(IDictionary<string, AuthSchemeFactory> map)
		{
			if (map == null)
			{
				return;
			}
			registeredSchemes.Clear();
			registeredSchemes.PutAll(map);
		}

		public AuthSchemeProvider Lookup(string name)
		{
			return new _AuthSchemeProvider_144(this, name);
		}

		private sealed class _AuthSchemeProvider_144 : AuthSchemeProvider
		{
			public _AuthSchemeProvider_144(AuthSchemeRegistry _enclosing, string name)
			{
				this._enclosing = _enclosing;
				this.name = name;
			}

			public AuthScheme Create(HttpContext context)
			{
				IHttpRequest request = (IHttpRequest)context.GetAttribute(ExecutionContext.HttpRequest
					);
				return this._enclosing.GetAuthScheme(name, request.GetParams());
			}

			private readonly AuthSchemeRegistry _enclosing;

			private readonly string name;
		}
	}
}
