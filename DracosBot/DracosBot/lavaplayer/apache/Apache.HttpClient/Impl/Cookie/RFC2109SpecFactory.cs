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

using System.Collections.Generic;
using Apache.Http.Cookie;
using Apache.Http.Cookie.Params;
using Apache.Http.Impl.Cookie;
using Apache.Http.Params;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>
	/// <see cref="Apache.Http.Cookie.CookieSpecProvider">Apache.Http.Cookie.CookieSpecProvider
	/// 	</see>
	/// implementation that creates and initializes
	/// <see cref="RFC2109Spec">RFC2109Spec</see>
	/// instances.
	/// </summary>
	/// <since>4.0</since>
	public class RFC2109SpecFactory : CookieSpecFactory, CookieSpecProvider
	{
		private readonly string[] datepatterns;

		private readonly bool oneHeader;

		public RFC2109SpecFactory(string[] datepatterns, bool oneHeader) : base()
		{
			this.datepatterns = datepatterns;
			this.oneHeader = oneHeader;
		}

		public RFC2109SpecFactory() : this(null, false)
		{
		}

		public virtual CookieSpec NewInstance(HttpParams @params)
		{
			if (@params != null)
			{
				string[] patterns = null;
				ICollection<object> param = (ICollection<object>)@params.GetParameter(CookieSpecPNames
					.DatePatterns);
				if (param != null)
				{
					patterns = new string[param.Count];
					patterns = Sharpen.Collections.ToArray(param, patterns);
				}
				bool singleHeader = @params.GetBooleanParameter(CookieSpecPNames.SingleCookieHeader
					, false);
				return new RFC2109Spec(patterns, singleHeader);
			}
			else
			{
				return new RFC2109Spec();
			}
		}

		public virtual CookieSpec Create(HttpContext context)
		{
			return new RFC2109Spec(this.datepatterns, this.oneHeader);
		}
	}
}
