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
using Apache.Http.Cookie;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>
	/// Abstract cookie specification which can delegate the job of parsing,
	/// validation or matching cookie attributes to a number of arbitrary
	/// <see cref="Apache.Http.Cookie.CookieAttributeHandler">Apache.Http.Cookie.CookieAttributeHandler
	/// 	</see>
	/// s.
	/// </summary>
	/// <since>4.0</since>
	public abstract class AbstractCookieSpec : CookieSpec
	{
		/// <summary>Stores attribute name -&gt; attribute handler mappings</summary>
		private readonly IDictionary<string, CookieAttributeHandler> attribHandlerMap;

		/// <summary>Default constructor</summary>
		public AbstractCookieSpec() : base()
		{
			// HashMap is not thread-safe
			this.attribHandlerMap = new Dictionary<string, CookieAttributeHandler>(10);
		}

		public virtual void RegisterAttribHandler(string name, CookieAttributeHandler handler
			)
		{
			Args.NotNull(name, "Attribute name");
			Args.NotNull(handler, "Attribute handler");
			this.attribHandlerMap.Put(name, handler);
		}

		/// <summary>
		/// Finds an attribute handler
		/// <see cref="Apache.Http.Cookie.CookieAttributeHandler">Apache.Http.Cookie.CookieAttributeHandler
		/// 	</see>
		/// for the
		/// given attribute. Returns <tt>null</tt> if no attribute handler is
		/// found for the specified attribute.
		/// </summary>
		/// <param name="name">attribute name. e.g. Domain, Path, etc.</param>
		/// <returns>an attribute handler or <tt>null</tt></returns>
		protected internal virtual CookieAttributeHandler FindAttribHandler(string name)
		{
			return this.attribHandlerMap.Get(name);
		}

		/// <summary>
		/// Gets attribute handler
		/// <see cref="Apache.Http.Cookie.CookieAttributeHandler">Apache.Http.Cookie.CookieAttributeHandler
		/// 	</see>
		/// for the
		/// given attribute.
		/// </summary>
		/// <param name="name">attribute name. e.g. Domain, Path, etc.</param>
		/// <exception cref="System.InvalidOperationException">
		/// if handler not found for the
		/// specified attribute.
		/// </exception>
		protected internal virtual CookieAttributeHandler GetAttribHandler(string name)
		{
			CookieAttributeHandler handler = FindAttribHandler(name);
			if (handler == null)
			{
				throw new InvalidOperationException("Handler not registered for " + name + " attribute."
					);
			}
			else
			{
				return handler;
			}
		}

		protected internal virtual ICollection<CookieAttributeHandler> GetAttribHandlers(
			)
		{
			return this.attribHandlerMap.Values;
		}

		public abstract IList<Header> FormatCookies(IList<Apache.Http.Cookie.Cookie> arg1
			);

		public abstract int GetVersion();

		public abstract Header GetVersionHeader();

		public abstract bool Match(Apache.Http.Cookie.Cookie arg1, CookieOrigin arg2);

		public abstract IList<Apache.Http.Cookie.Cookie> Parse(Header arg1, CookieOrigin 
			arg2);

		public abstract void Validate(Apache.Http.Cookie.Cookie arg1, CookieOrigin arg2);
	}
}
