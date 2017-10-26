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
	/// <summary>Microsoft Windows specific user principal implementation.</summary>
	/// <remarks>Microsoft Windows specific user principal implementation.</remarks>
	/// <since>4.0</since>
	[System.Serializable]
	public class NTUserPrincipal : Principal
	{
		private const long serialVersionUID = -6870169797924406894L;

		private readonly string username;

		private readonly string domain;

		private readonly string ntname;

		public NTUserPrincipal(string domain, string username) : base()
		{
			Args.NotNull(username, "User name");
			this.username = username;
			if (domain != null)
			{
				this.domain = domain.ToUpper(Sharpen.Extensions.GetEnglishCulture());
			}
			else
			{
				this.domain = null;
			}
			if (this.domain != null && this.domain.Length > 0)
			{
				StringBuilder buffer = new StringBuilder();
				buffer.Append(this.domain);
				buffer.Append('\\');
				buffer.Append(this.username);
				this.ntname = buffer.ToString();
			}
			else
			{
				this.ntname = this.username;
			}
		}

		public virtual string GetName()
		{
			return this.ntname;
		}

		public virtual string GetDomain()
		{
			return this.domain;
		}

		public virtual string GetUsername()
		{
			return this.username;
		}

		public override int GetHashCode()
		{
			int hash = LangUtils.HashSeed;
			hash = LangUtils.HashCode(hash, this.username);
			hash = LangUtils.HashCode(hash, this.domain);
			return hash;
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o is Apache.Http.Auth.NTUserPrincipal)
			{
				Apache.Http.Auth.NTUserPrincipal that = (Apache.Http.Auth.NTUserPrincipal)o;
				if (LangUtils.Equals(this.username, that.username) && LangUtils.Equals(this.domain
					, that.domain))
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			return this.ntname;
		}
	}
}
