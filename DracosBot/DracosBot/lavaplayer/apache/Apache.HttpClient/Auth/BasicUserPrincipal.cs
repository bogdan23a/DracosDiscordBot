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

using System.Text;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Auth
{
	/// <summary>Basic user principal used for HTTP authentication</summary>
	/// <since>4.0</since>
	[System.Serializable]
	public sealed class BasicUserPrincipal : Principal
	{
		private const long serialVersionUID = -2266305184969850467L;

		private readonly string username;

		public BasicUserPrincipal(string username) : base()
		{
			Args.NotNull(username, "User name");
			this.username = username;
		}

		public string GetName()
		{
			return this.username;
		}

		public override int GetHashCode()
		{
			int hash = LangUtils.HashSeed;
			hash = LangUtils.HashCode(hash, this.username);
			return hash;
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o is Apache.Http.Auth.BasicUserPrincipal)
			{
				Apache.Http.Auth.BasicUserPrincipal that = (Apache.Http.Auth.BasicUserPrincipal)o;
				if (LangUtils.Equals(this.username, that.username))
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append("[principal: ");
			buffer.Append(this.username);
			buffer.Append("]");
			return buffer.ToString();
		}
	}
}
