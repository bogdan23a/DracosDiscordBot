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

using Apache.Http;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Utils;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Client.Methods
{
	/// <summary>
	/// Basic implementation of an entity enclosing HTTP request
	/// that can be modified
	/// </summary>
	/// <since>4.0</since>
	public abstract class HttpEntityEnclosingRequestBase : HttpRequestBase, HttpEntityEnclosingRequest
	{
		private HttpEntity entity;

		public HttpEntityEnclosingRequestBase() : base()
		{
		}

		// HttpRequestBase is @NotThreadSafe
		public virtual HttpEntity GetEntity()
		{
			return this.entity;
		}

		public virtual void SetEntity(HttpEntity entity)
		{
			this.entity = entity;
		}

		public virtual bool ExpectContinue()
		{
			Header expect = GetFirstHeader(HTTP.ExpectDirective);
			return expect != null && Sharpen.Runtime.EqualsIgnoreCase(HTTP.ExpectContinue, expect
				.GetValue());
		}

		/// <exception cref="Sharpen.CloneNotSupportedException"></exception>
		public override object Clone()
		{
			Apache.Http.Client.Methods.HttpEntityEnclosingRequestBase clone = (Apache.Http.Client.Methods.HttpEntityEnclosingRequestBase
				)base.Clone();
			if (this.entity != null)
			{
				clone.entity = CloneUtils.CloneObject(this.entity);
			}
			return clone;
		}
	}
}
