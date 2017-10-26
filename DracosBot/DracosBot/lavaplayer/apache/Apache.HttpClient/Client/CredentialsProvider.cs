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
using Apache.Http.Client;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// Abstract credentials provider that maintains a collection of user
	/// credentials.
	/// </summary>
	/// <remarks>
	/// Abstract credentials provider that maintains a collection of user
	/// credentials.
	/// <p>
	/// Implementations of this interface must be thread-safe. Access to shared
	/// data must be synchronized as methods of this interface may be executed
	/// from multiple threads.
	/// </remarks>
	/// <since>4.0</since>
	public interface CredentialsProvider
	{
		/// <summary>
		/// Sets the
		/// <see cref="Apache.Http.Auth.Credentials">credentials</see>
		/// for the given authentication
		/// scope. Any previous credentials for the given scope will be overwritten.
		/// </summary>
		/// <param name="authscope">
		/// the
		/// <see cref="Apache.Http.Auth.AuthScope">authentication scope</see>
		/// </param>
		/// <param name="credentials">
		/// the authentication
		/// <see cref="Apache.Http.Auth.Credentials">credentials</see>
		/// for the given scope.
		/// </param>
		/// <seealso cref="GetCredentials(Apache.Http.Auth.AuthScope)">GetCredentials(Apache.Http.Auth.AuthScope)
		/// 	</seealso>
		void SetCredentials(AuthScope authscope, Credentials credentials);

		/// <summary>
		/// Get the
		/// <see cref="Apache.Http.Auth.Credentials">credentials</see>
		/// for the given authentication scope.
		/// </summary>
		/// <param name="authscope">
		/// the
		/// <see cref="Apache.Http.Auth.AuthScope">authentication scope</see>
		/// </param>
		/// <returns>the credentials</returns>
		/// <seealso cref="SetCredentials(Apache.Http.Auth.AuthScope, Apache.Http.Auth.Credentials)
		/// 	">SetCredentials(Apache.Http.Auth.AuthScope, Apache.Http.Auth.Credentials)</seealso>
		Credentials GetCredentials(AuthScope authscope);

		/// <summary>Clears all credentials.</summary>
		/// <remarks>Clears all credentials.</remarks>
		void Clear();
	}
}
