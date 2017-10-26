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

using System.IO;
using Apache.Http;
using Apache.Http.Entity;
using Apache.Http.Impl.Client;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// A wrapper class for
	/// <see cref="Apache.Http.HttpEntityEnclosingRequest">Apache.Http.HttpEntityEnclosingRequest
	/// 	</see>
	/// s that can
	/// be used to change properties of the current request without
	/// modifying the original object.
	/// </p>
	/// This class is also capable of resetting the request headers to
	/// the state of the original request.
	/// </summary>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) do not use.")]
	public class EntityEnclosingRequestWrapper : RequestWrapper, HttpEntityEnclosingRequest
	{
		private HttpEntity entity;

		private bool consumed;

		/// <exception cref="Apache.Http.ProtocolException"></exception>
		public EntityEnclosingRequestWrapper(HttpEntityEnclosingRequest request) : base(request
			)
		{
			// e.g. [gs]etEntity()
			SetEntity(request.GetEntity());
		}

		public virtual HttpEntity GetEntity()
		{
			return this.entity;
		}

		public virtual void SetEntity(HttpEntity entity)
		{
			this.entity = entity != null ? new EntityEnclosingRequestWrapper.EntityWrapper(this
				, entity) : null;
			this.consumed = false;
		}

		public virtual bool ExpectContinue()
		{
			Header expect = GetFirstHeader(HTTP.ExpectDirective);
			return expect != null && Sharpen.Runtime.EqualsIgnoreCase(HTTP.ExpectContinue, expect
				.GetValue());
		}

		public override bool IsRepeatable()
		{
			return this.entity == null || this.entity.IsRepeatable() || !this.consumed;
		}

		internal class EntityWrapper : HttpEntityWrapper
		{
			internal EntityWrapper(EntityEnclosingRequestWrapper _enclosing, HttpEntity entity
				) : base(entity)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void ConsumeContent()
			{
				this._enclosing.consumed = true;
				base.ConsumeContent();
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override InputStream GetContent()
			{
				this._enclosing.consumed = true;
				return base.GetContent();
			}

			/// <exception cref="System.IO.IOException"></exception>
			public override void WriteTo(OutputStream outstream)
			{
				this._enclosing.consumed = true;
				base.WriteTo(outstream);
			}

			private readonly EntityEnclosingRequestWrapper _enclosing;
		}
	}
}
