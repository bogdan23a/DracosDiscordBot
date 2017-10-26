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
	/// <since>4.2</since>
	public sealed class AuthOption
	{
		private readonly AuthScheme authScheme;

		private readonly Credentials creds;

		public AuthOption(AuthScheme authScheme, Credentials creds) : base()
		{
			Args.NotNull(authScheme, "Auth scheme");
			Args.NotNull(creds, "User credentials");
			this.authScheme = authScheme;
			this.creds = creds;
		}

		public AuthScheme GetAuthScheme()
		{
			return this.authScheme;
		}

		public Credentials GetCredentials()
		{
			return this.creds;
		}

		public override string ToString()
		{
			return this.authScheme.ToString();
		}
	}
}
