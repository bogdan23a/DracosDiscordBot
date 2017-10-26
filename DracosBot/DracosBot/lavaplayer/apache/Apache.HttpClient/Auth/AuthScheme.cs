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
using Apache.Http;
using Apache.Http.Auth;
using Org.Apache.Http;
using Sharpen;

namespace Apache.Http.Auth
{
	/// <summary>
	/// This interface represents an abstract challenge-response oriented
	/// authentication scheme.
	/// </summary>
	/// <remarks>
	/// This interface represents an abstract challenge-response oriented
	/// authentication scheme.
	/// <p>
	/// An authentication scheme should be able to support the following
	/// functions:
	/// <ul>
	/// <li>Parse and process the challenge sent by the target server
	/// in response to request for a protected resource
	/// <li>Provide its textual designation
	/// <li>Provide its parameters, if available
	/// <li>Provide the realm this authentication scheme is applicable to,
	/// if available
	/// <li>Generate authorization string for the given set of credentials
	/// and the HTTP request in response to the authorization challenge.
	/// </ul>
	/// <p>
	/// Authentication schemes may be stateful involving a series of
	/// challenge-response exchanges.
	/// <p>
	/// IMPORTANT: implementations of this interface MUST also implement
	/// <see cref="ContextAwareAuthScheme">ContextAwareAuthScheme</see>
	/// interface in order to remain API compatible with newer versions of HttpClient.
	/// </remarks>
	/// <since>4.0</since>
	public interface AuthScheme
	{
		/// <summary>Processes the given challenge token.</summary>
		/// <remarks>
		/// Processes the given challenge token. Some authentication schemes
		/// may involve multiple challenge-response exchanges. Such schemes must be able
		/// to maintain the state information when dealing with sequential challenges
		/// </remarks>
		/// <param name="header">the challenge header</param>
		/// <exception cref="Apache.Http.Auth.MalformedChallengeException"></exception>
		void ProcessChallenge(Header header);

		/// <summary>Returns textual designation of the given authentication scheme.</summary>
		/// <remarks>Returns textual designation of the given authentication scheme.</remarks>
		/// <returns>the name of the given authentication scheme</returns>
		string GetSchemeName();

		/// <summary>Returns authentication parameter with the given name, if available.</summary>
		/// <remarks>Returns authentication parameter with the given name, if available.</remarks>
		/// <param name="name">The name of the parameter to be returned</param>
		/// <returns>the parameter with the given name</returns>
		string GetParameter(string name);

		/// <summary>Returns authentication realm.</summary>
		/// <remarks>
		/// Returns authentication realm. If the concept of an authentication
		/// realm is not applicable to the given authentication scheme, returns
		/// <code>null</code>.
		/// </remarks>
		/// <returns>the authentication realm</returns>
		string GetRealm();

		/// <summary>
		/// Tests if the authentication scheme is provides authorization on a per
		/// connection basis instead of usual per request basis
		/// </summary>
		/// <returns>
		/// <tt>true</tt> if the scheme is connection based, <tt>false</tt>
		/// if the scheme is request based.
		/// </returns>
		bool IsConnectionBased();

		/// <summary>Authentication process may involve a series of challenge-response exchanges.
		/// 	</summary>
		/// <remarks>
		/// Authentication process may involve a series of challenge-response exchanges.
		/// This method tests if the authorization process has been completed, either
		/// successfully or unsuccessfully, that is, all the required authorization
		/// challenges have been processed in their entirety.
		/// </remarks>
		/// <returns>
		/// <tt>true</tt> if the authentication process has been completed,
		/// <tt>false</tt> otherwise.
		/// </returns>
		bool IsComplete();

		/// <summary>
		/// Produces an authorization string for the given set of
		/// <see cref="Credentials">Credentials</see>
		/// .
		/// </summary>
		/// <param name="credentials">The set of credentials to be used for athentication</param>
		/// <param name="request">The request being authenticated</param>
		/// <exception cref="AuthenticationException">
		/// if authorization string cannot
		/// be generated due to an authentication failure
		/// </exception>
		/// <returns>the authorization string</returns>
		/// <exception cref="Apache.Http.Auth.AuthenticationException"></exception>
		[Obsolete]
		[System.ObsoleteAttribute(@"(4.1)  Use ContextAwareAuthScheme.Authenticate(Credentials, Org.Apache.Http.IHttpRequest, Apache.Http.Protocol.HttpContext)"
			)]
		Header Authenticate(Credentials credentials, IHttpRequest request);
	}
}
