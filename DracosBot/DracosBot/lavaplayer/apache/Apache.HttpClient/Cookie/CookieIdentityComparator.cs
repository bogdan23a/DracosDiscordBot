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
using Sharpen;

namespace Apache.Http.Cookie
{
	/// <summary>This cookie comparator can be used to compare identity of cookies.</summary>
	/// <remarks>
	/// This cookie comparator can be used to compare identity of cookies.
	/// <p>
	/// Cookies are considered identical if their names are equal and
	/// their domain attributes match ignoring case.
	/// </remarks>
	/// <since>4.0</since>
	[System.Serializable]
	public class CookieIdentityComparator : IComparer<Apache.Http.Cookie.Cookie>
	{
		private const long serialVersionUID = 4466565437490631532L;

		public virtual int Compare(Apache.Http.Cookie.Cookie c1, Apache.Http.Cookie.Cookie
			 c2)
		{
			int res = Sharpen.Runtime.CompareOrdinal(c1.GetName(), c2.GetName());
			if (res == 0)
			{
				// do not differentiate empty and null domains
				string d1 = c1.GetDomain();
				if (d1 == null)
				{
					d1 = string.Empty;
				}
				else
				{
					if (d1.IndexOf('.') == -1)
					{
						d1 = d1 + ".local";
					}
				}
				string d2 = c2.GetDomain();
				if (d2 == null)
				{
					d2 = string.Empty;
				}
				else
				{
					if (d2.IndexOf('.') == -1)
					{
						d2 = d2 + ".local";
					}
				}
				res = d1.CompareToIgnoreCase(d2);
			}
			if (res == 0)
			{
				string p1 = c1.GetPath();
				if (p1 == null)
				{
					p1 = "/";
				}
				string p2 = c2.GetPath();
				if (p2 == null)
				{
					p2 = "/";
				}
				res = Sharpen.Runtime.CompareOrdinal(p1, p2);
			}
			return res;
		}
	}
}
