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

using Apache.Http.Impl.Auth;
using Sharpen;

namespace Apache.Http.Impl.Auth
{
	/// <summary>Abstract NTLM authentication engine.</summary>
	/// <remarks>
	/// Abstract NTLM authentication engine. The engine can be used to
	/// generate Type1 messages and Type3 messages in response to a
	/// Type2 challenge.
	/// </remarks>
	/// <since>4.0</since>
	public interface NTLMEngine
	{
		/// <summary>Generates a Type1 message given the domain and workstation.</summary>
		/// <remarks>Generates a Type1 message given the domain and workstation.</remarks>
		/// <param name="domain">Optional Windows domain name. Can be <code>null</code>.</param>
		/// <param name="workstation">
		/// Optional Windows workstation name. Can be
		/// <code>null</code>.
		/// </param>
		/// <returns>Type1 message</returns>
		/// <exception cref="NTLMEngineException">NTLMEngineException</exception>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		string GenerateType1Msg(string domain, string workstation);

		/// <summary>
		/// Generates a Type3 message given the user credentials and the
		/// authentication challenge.
		/// </summary>
		/// <remarks>
		/// Generates a Type3 message given the user credentials and the
		/// authentication challenge.
		/// </remarks>
		/// <param name="username">Windows user name</param>
		/// <param name="password">Password</param>
		/// <param name="domain">Windows domain name</param>
		/// <param name="workstation">Windows workstation name</param>
		/// <param name="challenge">Type2 challenge.</param>
		/// <returns>Type3 response.</returns>
		/// <exception cref="NTLMEngineException">NTLMEngineException</exception>
		/// <exception cref="Apache.Http.Impl.Auth.NTLMEngineException"></exception>
		string GenerateType3Msg(string username, string password, string domain, string workstation
			, string challenge);
	}
}
