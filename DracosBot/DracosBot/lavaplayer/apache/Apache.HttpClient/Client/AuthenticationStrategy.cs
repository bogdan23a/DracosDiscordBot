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
using Apache.Http.Client;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// /
	/// A handler for determining if an HTTP response represents an authentication challenge that was
	/// sent back to the client as a result of authentication failure.
	/// </summary>
	/// <remarks>
	/// /
	/// A handler for determining if an HTTP response represents an authentication challenge that was
	/// sent back to the client as a result of authentication failure.
	/// <p>
	/// Implementations of this interface must be thread-safe. Access to shared data must be
	/// synchronized as methods of this interface may be executed from multiple threads.
	/// </remarks>
	/// <since>4.2</since>
	public interface AuthenticationStrategy
	{
		/// <summary>
		/// Determines if the given HTTP response response represents
		/// an authentication challenge that was sent back as a result
		/// of authentication failure.
		/// </summary>
		/// <remarks>
		/// Determines if the given HTTP response response represents
		/// an authentication challenge that was sent back as a result
		/// of authentication failure.
		/// </remarks>
		/// <param name="authhost">authentication host.</param>
		/// <param name="response">HTTP response.</param>
		/// <param name="context">HTTP context.</param>
		/// <returns>
		/// <code>true</code> if user authentication is required,
		/// <code>false</code> otherwise.
		/// </returns>
		bool IsAuthenticationRequested(HttpHost authhost, HttpResponse response, HttpContext
			 context);

		/// <summary>
		/// Extracts from the given HTTP response a collection of authentication
		/// challenges, each of which represents an authentication scheme supported
		/// by the authentication host.
		/// </summary>
		/// <remarks>
		/// Extracts from the given HTTP response a collection of authentication
		/// challenges, each of which represents an authentication scheme supported
		/// by the authentication host.
		/// </remarks>
		/// <param name="authhost">authentication host.</param>
		/// <param name="response">HTTP response.</param>
		/// <param name="context">HTTP context.</param>
		/// <returns>
		/// a collection of challenges keyed by names of corresponding
		/// authentication schemes.
		/// </returns>
		/// <exception cref="Apache.Http.Auth.MalformedChallengeException">
		/// if one of the authentication
		/// challenges is not valid or malformed.
		/// </exception>
		IDictionary<string, Header> GetChallenges(HttpHost authhost, HttpResponse response
			, HttpContext context);

		/// <summary>
		/// Selects one authentication challenge out of all available and
		/// creates and generates
		/// <see cref="Apache.Http.Auth.AuthOption">Apache.Http.Auth.AuthOption</see>
		/// instance capable of
		/// processing that challenge.
		/// </summary>
		/// <param name="challenges">collection of challenges.</param>
		/// <param name="authhost">authentication host.</param>
		/// <param name="response">HTTP response.</param>
		/// <param name="context">HTTP context.</param>
		/// <returns>authentication auth schemes that can be used for authentication. Can be empty.
		/// 	</returns>
		/// <exception cref="Apache.Http.Auth.MalformedChallengeException">
		/// if one of the authentication
		/// challenges is not valid or malformed.
		/// </exception>
		Queue<AuthOption> Select(IDictionary<string, Header> challenges, HttpHost authhost
			, HttpResponse response, HttpContext context);

		/// <summary>Callback invoked in case of successful authentication.</summary>
		/// <remarks>Callback invoked in case of successful authentication.</remarks>
		/// <param name="authhost">authentication host.</param>
		/// <param name="authScheme">authentication scheme used.</param>
		/// <param name="context">HTTP context.</param>
		void AuthSucceeded(HttpHost authhost, AuthScheme authScheme, HttpContext context);

		/// <summary>Callback invoked in case of unsuccessful authentication.</summary>
		/// <remarks>Callback invoked in case of unsuccessful authentication.</remarks>
		/// <param name="authhost">authentication host.</param>
		/// <param name="authScheme">authentication scheme used.</param>
		/// <param name="context">HTTP context.</param>
		void AuthFailed(HttpHost authhost, AuthScheme authScheme, HttpContext context);
	}
}
