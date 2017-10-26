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

using Apache.Http.Auth;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Auth
{
	/// <summary>
	/// Simple
	/// <see cref="Credentials">Credentials</see>
	/// implementation based on a user name / password
	/// pair.
	/// </summary>
	/// <since>4.0</since>
	[System.Serializable]
	public class UsernamePasswordCredentials : Credentials
	{
		private const long serialVersionUID = 243343858802739403L;

		private readonly BasicUserPrincipal principal;

		private readonly string password;

		/// <summary>The constructor with the username and password combined string argument.
		/// 	</summary>
		/// <remarks>The constructor with the username and password combined string argument.
		/// 	</remarks>
		/// <param name="usernamePassword">the username:password formed string</param>
		/// <seealso cref="ToString()">ToString()</seealso>
		public UsernamePasswordCredentials(string usernamePassword) : base()
		{
			Args.NotNull(usernamePassword, "Username:password string");
			int atColon = usernamePassword.IndexOf(':');
			if (atColon >= 0)
			{
				this.principal = new BasicUserPrincipal(Sharpen.Runtime.Substring(usernamePassword
					, 0, atColon));
				this.password = Sharpen.Runtime.Substring(usernamePassword, atColon + 1);
			}
			else
			{
				this.principal = new BasicUserPrincipal(usernamePassword);
				this.password = null;
			}
		}

		/// <summary>The constructor with the username and password arguments.</summary>
		/// <remarks>The constructor with the username and password arguments.</remarks>
		/// <param name="userName">the user name</param>
		/// <param name="password">the password</param>
		public UsernamePasswordCredentials(string userName, string password) : base()
		{
			Args.NotNull(userName, "Username");
			this.principal = new BasicUserPrincipal(userName);
			this.password = password;
		}

		public virtual Principal GetUserPrincipal()
		{
			return this.principal;
		}

		public virtual string GetUserName()
		{
			return this.principal.GetName();
		}

		public virtual string GetPassword()
		{
			return password;
		}

		public override int GetHashCode()
		{
			return this.principal.GetHashCode();
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o is Apache.Http.Auth.UsernamePasswordCredentials)
			{
				Apache.Http.Auth.UsernamePasswordCredentials that = (Apache.Http.Auth.UsernamePasswordCredentials
					)o;
				if (LangUtils.Equals(this.principal, that.principal))
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			return this.principal.ToString();
		}
	}
}
