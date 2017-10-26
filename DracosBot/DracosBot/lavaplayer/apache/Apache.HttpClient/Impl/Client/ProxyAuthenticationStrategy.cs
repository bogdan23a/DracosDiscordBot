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
using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Client.Config;
using Apache.Http.Impl.Client;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Default
	/// <see cref="Apache.Http.Client.AuthenticationStrategy">Apache.Http.Client.AuthenticationStrategy
	/// 	</see>
	/// implementation
	/// for proxy host authentication.
	/// </summary>
	/// <since>4.2</since>
	public class ProxyAuthenticationStrategy : AuthenticationStrategyImpl
	{
		public static readonly Apache.Http.Impl.Client.ProxyAuthenticationStrategy Instance
			 = new Apache.Http.Impl.Client.ProxyAuthenticationStrategy();

		public ProxyAuthenticationStrategy() : base(HttpStatus.ScProxyAuthenticationRequired
			, AUTH.ProxyAuth)
		{
		}

		internal override ICollection<string> GetPreferredAuthSchemes(RequestConfig config
			)
		{
			return config.GetProxyPreferredAuthSchemes();
		}
	}
}
