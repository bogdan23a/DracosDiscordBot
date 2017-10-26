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
using Apache.Http.Cookie;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Cookie
{
	/// <summary>
	/// Cookie specification registry that can be used to obtain the corresponding
	/// cookie specification implementation for a given type of type or version of
	/// cookie.
	/// </summary>
	/// <remarks>
	/// Cookie specification registry that can be used to obtain the corresponding
	/// cookie specification implementation for a given type of type or version of
	/// cookie.
	/// </remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) use Apache.Http.Config.Registry{I} .")]
	public sealed class CookieSpecRegistry : Apache.Http.Config.Lookup<CookieSpecProvider
		>
	{
		private readonly ConcurrentHashMap<string, CookieSpecFactory> registeredSpecs;

		public CookieSpecRegistry() : base()
		{
			this.registeredSpecs = new ConcurrentHashMap<string, CookieSpecFactory>();
		}

		/// <summary>
		/// Registers a
		/// <see cref="CookieSpecFactory">CookieSpecFactory</see>
		/// with the given identifier.
		/// If a specification with the given name already exists it will be overridden.
		/// This nameis the same one used to retrieve the
		/// <see cref="CookieSpecFactory">CookieSpecFactory</see>
		/// from
		/// <see cref="GetCookieSpec(string)">GetCookieSpec(string)</see>
		/// .
		/// </summary>
		/// <param name="name">the identifier for this specification</param>
		/// <param name="factory">
		/// the
		/// <see cref="CookieSpecFactory">CookieSpecFactory</see>
		/// class to register
		/// </param>
		/// <seealso cref="GetCookieSpec(string)">GetCookieSpec(string)</seealso>
		public void Register(string name, CookieSpecFactory factory)
		{
			Args.NotNull(name, "Name");
			Args.NotNull(factory, "Cookie spec factory");
			registeredSpecs.Put(name.ToLower(Sharpen.Extensions.GetEnglishCulture()), factory
				);
		}

		/// <summary>
		/// Unregisters the
		/// <see cref="CookieSpecFactory">CookieSpecFactory</see>
		/// with the given ID.
		/// </summary>
		/// <param name="id">
		/// the identifier of the
		/// <see cref="CookieSpec">cookie specification</see>
		/// to unregister
		/// </param>
		public void Unregister(string id)
		{
			Args.NotNull(id, "Id");
			Sharpen.Collections.Remove(registeredSpecs, id.ToLower(Sharpen.Extensions.GetEnglishCulture()
				));
		}

		/// <summary>
		/// Gets the
		/// <see cref="CookieSpec">cookie specification</see>
		/// with the given ID.
		/// </summary>
		/// <param name="name">
		/// the
		/// <see cref="CookieSpec">cookie specification</see>
		/// identifier
		/// </param>
		/// <param name="params">
		/// the
		/// <see cref="Apache.Http.Params.HttpParams">HTTP parameters</see>
		/// for the cookie
		/// specification.
		/// </param>
		/// <returns>
		/// 
		/// <see cref="CookieSpec">cookie specification</see>
		/// </returns>
		/// <exception cref="System.InvalidOperationException">if a policy with the given name cannot be found
		/// 	</exception>
		public CookieSpec GetCookieSpec(string name, HttpParams @params)
		{
			Args.NotNull(name, "Name");
			CookieSpecFactory factory = registeredSpecs.Get(name.ToLower(Sharpen.Extensions.GetEnglishCulture()
				));
			if (factory != null)
			{
				return factory.NewInstance(@params);
			}
			else
			{
				throw new InvalidOperationException("Unsupported cookie spec: " + name);
			}
		}

		/// <summary>
		/// Gets the
		/// <see cref="CookieSpec">cookie specification</see>
		/// with the given name.
		/// </summary>
		/// <param name="name">
		/// the
		/// <see cref="CookieSpec">cookie specification</see>
		/// identifier
		/// </param>
		/// <returns>
		/// 
		/// <see cref="CookieSpec">cookie specification</see>
		/// </returns>
		/// <exception cref="System.InvalidOperationException">if a policy with the given name cannot be found
		/// 	</exception>
		public CookieSpec GetCookieSpec(string name)
		{
			return GetCookieSpec(name, null);
		}

		/// <summary>
		/// Obtains a list containing the names of all registered
		/// <see cref="CookieSpec">
		/// cookie
		/// specs
		/// </see>
		/// .
		/// Note that the DEFAULT policy (if present) is likely to be the same
		/// as one of the other policies, but does not have to be.
		/// </summary>
		/// <returns>list of registered cookie spec names</returns>
		public IList<string> GetSpecNames()
		{
			return new AList<string>(registeredSpecs.Keys);
		}

		/// <summary>
		/// Populates the internal collection of registered
		/// <see cref="CookieSpec">
		/// cookie
		/// specs
		/// </see>
		/// with the content of the map passed as a parameter.
		/// </summary>
		/// <param name="map">cookie specs</param>
		public void SetItems(IDictionary<string, CookieSpecFactory> map)
		{
			if (map == null)
			{
				return;
			}
			registeredSpecs.Clear();
			registeredSpecs.PutAll(map);
		}

		public CookieSpecProvider Lookup(string name)
		{
			return new _CookieSpecProvider_156(this, name);
		}

		private sealed class _CookieSpecProvider_156 : CookieSpecProvider
		{
			public _CookieSpecProvider_156(CookieSpecRegistry _enclosing, string name)
			{
				this._enclosing = _enclosing;
				this.name = name;
			}

			public CookieSpec Create(HttpContext context)
			{
				IHttpRequest request = (IHttpRequest)context.GetAttribute(ExecutionContext.HttpRequest
					);
				return this._enclosing.GetCookieSpec(name, request.GetParams());
			}

			private readonly CookieSpecRegistry _enclosing;

			private readonly string name;
		}
	}
}
