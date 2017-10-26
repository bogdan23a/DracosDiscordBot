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

using Apache.Http.Cookie;
using Sharpen;

namespace Apache.Http.Cookie
{
	/// <summary>
	/// ClientCookie extends the standard
	/// <see cref="Cookie">Cookie</see>
	/// interface with
	/// additional client specific functionality such ability to retrieve
	/// original cookie attributes exactly as they were specified by the
	/// origin server. This is important for generating the <tt>Cookie</tt>
	/// header because some cookie specifications require that the
	/// <tt>Cookie</tt> header should include certain attributes only if
	/// they were specified in the <tt>Set-Cookie</tt> header.
	/// </summary>
	/// <since>4.0</since>
	public abstract class ClientCookie : Apache.Http.Cookie.Cookie
	{
		public const string VersionAttr = "version";

		public const string PathAttr = "path";

		public const string DomainAttr = "domain";

		public const string MaxAgeAttr = "max-age";

		public const string SecureAttr = "secure";

		public const string CommentAttr = "comment";

		public const string ExpiresAttr = "expires";

		public const string PortAttr = "port";

		public const string CommenturlAttr = "commenturl";

		public const string DiscardAttr = "discard";

		// RFC2109 attributes
		// RFC2965 attributes
		public abstract string GetAttribute(string name);

		public abstract bool ContainsAttribute(string name);
	}
}
