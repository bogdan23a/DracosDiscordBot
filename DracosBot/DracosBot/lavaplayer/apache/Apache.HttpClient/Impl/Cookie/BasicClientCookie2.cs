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
using Apache.Http.Cookie;
using Apache.Http.Impl.Cookie;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>
	/// Default implementation of
	/// <see cref="Apache.Http.Cookie.SetCookie2">Apache.Http.Cookie.SetCookie2</see>
	/// .
	/// </summary>
	/// <since>4.0</since>
	[System.Serializable]
	public class BasicClientCookie2 : BasicClientCookie, SetCookie2
	{
		private const long serialVersionUID = -7744598295706617057L;

		private string commentURL;

		private int[] ports;

		private bool discard;

		/// <summary>Default Constructor taking a name and a value.</summary>
		/// <remarks>Default Constructor taking a name and a value. The value may be null.</remarks>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public BasicClientCookie2(string name, string value) : base(name, value)
		{
		}

		public override int[] GetPorts()
		{
			return this.ports;
		}

		public virtual void SetPorts(int[] ports)
		{
			this.ports = ports;
		}

		public override string GetCommentURL()
		{
			return this.commentURL;
		}

		public virtual void SetCommentURL(string commentURL)
		{
			this.commentURL = commentURL;
		}

		public virtual void SetDiscard(bool discard)
		{
			this.discard = discard;
		}

		public override bool IsPersistent()
		{
			return !this.discard && base.IsPersistent();
		}

		public override bool IsExpired(DateTime date)
		{
			return this.discard || base.IsExpired(date);
		}

		/// <exception cref="Sharpen.CloneNotSupportedException"></exception>
		public override object Clone()
		{
			Apache.Http.Impl.Cookie.BasicClientCookie2 clone = (Apache.Http.Impl.Cookie.BasicClientCookie2
				)base.Clone();
			if (this.ports != null)
			{
				clone.ports = this.ports.Clone();
			}
			return clone;
		}
	}
}
