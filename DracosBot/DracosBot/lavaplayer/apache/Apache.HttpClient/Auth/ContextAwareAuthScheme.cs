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

using Apache.Http;
using Apache.Http.Auth;
using Apache.Http.Protocol;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Auth
{
	/// <summary>
	/// This interface represents an extended  authentication scheme
	/// that requires access to
	/// <see cref="Apache.Http.Protocol.HttpContext">Apache.Http.Protocol.HttpContext</see>
	/// in order to
	/// generate an authorization string.
	/// TODO: Fix AuthScheme interface in the next major version
	/// </summary>
	/// <since>4.1</since>
	public interface ContextAwareAuthScheme : AuthScheme
	{
		/// <summary>
		/// Produces an authorization string for the given set of
		/// <see cref="Credentials">Credentials</see>
		/// .
		/// </summary>
		/// <param name="credentials">The set of credentials to be used for athentication</param>
		/// <param name="request">The request being authenticated</param>
		/// <param name="context">HTTP context</param>
		/// <exception cref="AuthenticationException">
		/// if authorization string cannot
		/// be generated due to an authentication failure
		/// </exception>
		/// <returns>the authorization string</returns>
		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		Header Authenticate(Credentials credentials, IHttpRequest request, HttpContext context
			);
	}
}
