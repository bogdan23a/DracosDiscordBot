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
using Apache.Http.Auth;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Auth
{
	/// <summary>
	/// <see cref="Credentials">Credentials</see>
	/// implementation for Microsoft Windows platforms that includes
	/// Windows specific attributes such as name of the domain the user belongs to.
	/// </summary>
	/// <since>4.0</since>
	[System.Serializable]
	public class NTCredentials : Credentials
	{
		private const long serialVersionUID = -7385699315228907265L;

		/// <summary>The user principal</summary>
		private readonly NTUserPrincipal principal;

		/// <summary>Password</summary>
		private readonly string password;

		/// <summary>The host the authentication request is originating from.</summary>
		/// <remarks>The host the authentication request is originating from.</remarks>
		private readonly string workstation;

		/// <summary>
		/// The constructor with the fully qualified username and password combined
		/// string argument.
		/// </summary>
		/// <remarks>
		/// The constructor with the fully qualified username and password combined
		/// string argument.
		/// </remarks>
		/// <param name="usernamePassword">the domain/username:password formed string</param>
		public NTCredentials(string usernamePassword) : base()
		{
			Args.NotNull(usernamePassword, "Username:password string");
			string username;
			int atColon = usernamePassword.IndexOf(':');
			if (atColon >= 0)
			{
				username = Sharpen.Runtime.Substring(usernamePassword, 0, atColon);
				this.password = Sharpen.Runtime.Substring(usernamePassword, atColon + 1);
			}
			else
			{
				username = usernamePassword;
				this.password = null;
			}
			int atSlash = username.IndexOf('/');
			if (atSlash >= 0)
			{
				this.principal = new NTUserPrincipal(Sharpen.Runtime.Substring(username, 0, atSlash
					).ToUpper(Sharpen.Extensions.GetEnglishCulture()), Sharpen.Runtime.Substring(username
					, atSlash + 1));
			}
			else
			{
				this.principal = new NTUserPrincipal(null, Sharpen.Runtime.Substring(username, atSlash
					 + 1));
			}
			this.workstation = null;
		}

		/// <summary>Constructor.</summary>
		/// <remarks>Constructor.</remarks>
		/// <param name="userName">
		/// The user name.  This should not include the domain to authenticate with.
		/// For example: "user" is correct whereas "DOMAIN\\user" is not.
		/// </param>
		/// <param name="password">The password.</param>
		/// <param name="workstation">
		/// The workstation the authentication request is originating from.
		/// Essentially, the computer name for this machine.
		/// </param>
		/// <param name="domain">The domain to authenticate within.</param>
		public NTCredentials(string userName, string password, string workstation, string
			 domain) : base()
		{
			Args.NotNull(userName, "User name");
			this.principal = new NTUserPrincipal(domain, userName);
			this.password = password;
			if (workstation != null)
			{
				this.workstation = workstation.ToUpper(Sharpen.Extensions.GetEnglishCulture());
			}
			else
			{
				this.workstation = null;
			}
		}

		public virtual Principal GetUserPrincipal()
		{
			return this.principal;
		}

		public virtual string GetUserName()
		{
			return this.principal.GetUsername();
		}

		public virtual string GetPassword()
		{
			return this.password;
		}

		/// <summary>Retrieves the name to authenticate with.</summary>
		/// <remarks>Retrieves the name to authenticate with.</remarks>
		/// <returns>String the domain these credentials are intended to authenticate with.</returns>
		public virtual string GetDomain()
		{
			return this.principal.GetDomain();
		}

		/// <summary>Retrieves the workstation name of the computer originating the request.</summary>
		/// <remarks>Retrieves the workstation name of the computer originating the request.</remarks>
		/// <returns>String the workstation the user is logged into.</returns>
		public virtual string GetWorkstation()
		{
			return this.workstation;
		}

		public override int GetHashCode()
		{
			int hash = LangUtils.HashSeed;
			hash = LangUtils.HashCode(hash, this.principal);
			hash = LangUtils.HashCode(hash, this.workstation);
			return hash;
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o is Apache.Http.Auth.NTCredentials)
			{
				Apache.Http.Auth.NTCredentials that = (Apache.Http.Auth.NTCredentials)o;
				if (LangUtils.Equals(this.principal, that.principal) && LangUtils.Equals(this.workstation
					, that.workstation))
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
			buffer.Append(this.principal);
			buffer.Append("][workstation: ");
			buffer.Append(this.workstation);
			buffer.Append("]");
			return buffer.ToString();
		}
	}
}
