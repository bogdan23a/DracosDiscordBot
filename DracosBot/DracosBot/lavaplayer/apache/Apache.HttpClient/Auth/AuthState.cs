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
using System.Text;
using Apache.Http.Auth;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Auth
{
	/// <summary>This class provides detailed information about the state of the authentication process.
	/// 	</summary>
	/// <remarks>This class provides detailed information about the state of the authentication process.
	/// 	</remarks>
	/// <since>4.0</since>
	public class AuthState
	{
		/// <summary>Actual state of authentication protocol</summary>
		private AuthProtocolState state;

		/// <summary>Actual authentication scheme</summary>
		private AuthScheme authScheme;

		/// <summary>Actual authentication scope</summary>
		private AuthScope authScope;

		/// <summary>Credentials selected for authentication</summary>
		private Credentials credentials;

		/// <summary>Available auth options</summary>
		private Queue<AuthOption> authOptions;

		public AuthState() : base()
		{
			this.state = AuthProtocolState.Unchallenged;
		}

		/// <summary>Resets the auth state.</summary>
		/// <remarks>Resets the auth state.</remarks>
		/// <since>4.2</since>
		public virtual void Reset()
		{
			this.state = AuthProtocolState.Unchallenged;
			this.authOptions = null;
			this.authScheme = null;
			this.authScope = null;
			this.credentials = null;
		}

		/// <since>4.2</since>
		public virtual AuthProtocolState GetState()
		{
			return this.state;
		}

		/// <since>4.2</since>
		public virtual void SetState(AuthProtocolState state)
		{
			this.state = state != null ? state : AuthProtocolState.Unchallenged;
		}

		/// <summary>
		/// Returns actual
		/// <see cref="AuthScheme">AuthScheme</see>
		/// . May be null.
		/// </summary>
		public virtual AuthScheme GetAuthScheme()
		{
			return this.authScheme;
		}

		/// <summary>
		/// Returns actual
		/// <see cref="Credentials">Credentials</see>
		/// . May be null.
		/// </summary>
		public virtual Credentials GetCredentials()
		{
			return this.credentials;
		}

		/// <summary>
		/// Updates the auth state with
		/// <see cref="AuthScheme">AuthScheme</see>
		/// and
		/// <see cref="Credentials">Credentials</see>
		/// .
		/// </summary>
		/// <param name="authScheme">auth scheme. May not be null.</param>
		/// <param name="credentials">user crednetials. May not be null.</param>
		/// <since>4.2</since>
		public virtual void Update(AuthScheme authScheme, Credentials credentials)
		{
			Args.NotNull(authScheme, "Auth scheme");
			Args.NotNull(credentials, "Credentials");
			this.authScheme = authScheme;
			this.credentials = credentials;
			this.authOptions = null;
		}

		/// <summary>
		/// Returns available
		/// <see cref="AuthOption">AuthOption</see>
		/// s. May be null.
		/// </summary>
		/// <since>4.2</since>
		public virtual Queue<AuthOption> GetAuthOptions()
		{
			return this.authOptions;
		}

		/// <summary>
		/// Returns <code>true</code> if
		/// <see cref="AuthOption">AuthOption</see>
		/// s are available, <code>false</code>
		/// otherwise.
		/// </summary>
		/// <since>4.2</since>
		public virtual bool HasAuthOptions()
		{
			return this.authOptions != null && !this.authOptions.IsEmpty();
		}

		/// <summary>
		/// Updates the auth state with a queue of
		/// <see cref="AuthOption">AuthOption</see>
		/// s.
		/// </summary>
		/// <param name="authOptions">a queue of auth options. May not be null or empty.</param>
		/// <since>4.2</since>
		public virtual void Update(Queue<AuthOption> authOptions)
		{
			Args.NotEmpty(authOptions, "Queue of auth options");
			this.authOptions = authOptions;
			this.authScheme = null;
			this.credentials = null;
		}

		/// <summary>Invalidates the authentication state by resetting its parameters.</summary>
		/// <remarks>Invalidates the authentication state by resetting its parameters.</remarks>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2)  use Reset()")]
		public virtual void Invalidate()
		{
			Reset();
		}

		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2) do not use")]
		public virtual bool IsValid()
		{
			return this.authScheme != null;
		}

		/// <summary>
		/// Assigns the given
		/// <see cref="AuthScheme">authentication scheme</see>
		/// .
		/// </summary>
		/// <param name="authScheme">
		/// the
		/// <see cref="AuthScheme">authentication scheme</see>
		/// </param>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2)  use Update(AuthScheme, Credentials)")]
		public virtual void SetAuthScheme(AuthScheme authScheme)
		{
			if (authScheme == null)
			{
				Reset();
				return;
			}
			this.authScheme = authScheme;
		}

		/// <summary>
		/// Sets user
		/// <see cref="Credentials">Credentials</see>
		/// to be used for authentication
		/// </summary>
		/// <param name="credentials">User credentials</param>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2)  use Update(AuthScheme, Credentials)")]
		public virtual void SetCredentials(Credentials credentials)
		{
			this.credentials = credentials;
		}

		/// <summary>
		/// Returns actual
		/// <see cref="AuthScope">AuthScope</see>
		/// if available
		/// </summary>
		/// <returns>actual authentication scope if available, <code>null&lt;/code otherwise</returns>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2)  do not use.")]
		public virtual AuthScope GetAuthScope()
		{
			return this.authScope;
		}

		/// <summary>
		/// Sets actual
		/// <see cref="AuthScope">AuthScope</see>
		/// .
		/// </summary>
		/// <param name="authScope">Authentication scope</param>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.2)  do not use.")]
		public virtual void SetAuthScope(AuthScope authScope)
		{
			this.authScope = authScope;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append("state:").Append(this.state).Append(";");
			if (this.authScheme != null)
			{
				buffer.Append("auth scheme:").Append(this.authScheme.GetSchemeName()).Append(";");
			}
			if (this.credentials != null)
			{
				buffer.Append("credentials present");
			}
			return buffer.ToString();
		}
	}
}
