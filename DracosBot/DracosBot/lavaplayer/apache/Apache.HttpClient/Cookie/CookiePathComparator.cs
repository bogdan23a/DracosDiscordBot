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
	/// <summary>
	/// This cookie comparator ensures that multiple cookies satisfying
	/// a common criteria are ordered in the <tt>Cookie</tt> header such
	/// that those with more specific Path attributes precede those with
	/// less specific.
	/// </summary>
	/// <remarks>
	/// This cookie comparator ensures that multiple cookies satisfying
	/// a common criteria are ordered in the <tt>Cookie</tt> header such
	/// that those with more specific Path attributes precede those with
	/// less specific.
	/// <p>
	/// This comparator assumes that Path attributes of two cookies
	/// path-match a commmon request-URI. Otherwise, the result of the
	/// comparison is undefined.
	/// </p>
	/// </remarks>
	/// <since>4.0</since>
	[System.Serializable]
	public class CookiePathComparator : IComparer<Apache.Http.Cookie.Cookie>
	{
		private const long serialVersionUID = 7523645369616405818L;

		private string NormalizePath(Apache.Http.Cookie.Cookie cookie)
		{
			string path = cookie.GetPath();
			if (path == null)
			{
				path = "/";
			}
			if (!path.EndsWith("/"))
			{
				path = path + '/';
			}
			return path;
		}

		public virtual int Compare(Apache.Http.Cookie.Cookie c1, Apache.Http.Cookie.Cookie
			 c2)
		{
			string path1 = NormalizePath(c1);
			string path2 = NormalizePath(c2);
			if (path1.Equals(path2))
			{
				return 0;
			}
			else
			{
				if (path1.StartsWith(path2))
				{
					return -1;
				}
				else
				{
					if (path2.StartsWith(path1))
					{
						return 1;
					}
					else
					{
						// Does not really matter
						return 0;
					}
				}
			}
		}
	}
}
