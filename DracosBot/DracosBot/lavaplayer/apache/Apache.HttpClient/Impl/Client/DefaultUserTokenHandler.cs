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
using Apache.Http.Client;
using Apache.Http.Client.Protocol;
using Apache.Http.Conn;
using Apache.Http.Impl.Client;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Default implementation of
	/// <see cref="Apache.Http.Client.UserTokenHandler">Apache.Http.Client.UserTokenHandler
	/// 	</see>
	/// . This class will use
	/// an instance of
	/// <see cref="Sharpen.Principal">Sharpen.Principal</see>
	/// as a state object for HTTP connections,
	/// if it can be obtained from the given execution context. This helps ensure
	/// persistent connections created with a particular user identity within
	/// a particular security context can be reused by the same user only.
	/// <p>
	/// DefaultUserTokenHandler will use the user principle of connection
	/// based authentication schemes such as NTLM or that of the SSL session
	/// with the client authentication turned on. If both are unavailable,
	/// <code>null</code> token will be returned.
	/// </summary>
	/// <since>4.0</since>
	public class DefaultUserTokenHandler : UserTokenHandler
	{
		public static readonly DefaultUserTokenHandler Instance = new DefaultUserTokenHandler
			();

		public virtual object GetUserToken(HttpContext context)
		{
			HttpClientContext clientContext = ((HttpClientContext)HttpClientContext.Adapt(context
				));
			Principal userPrincipal = null;
			AuthState targetAuthState = clientContext.GetTargetAuthState();
			if (targetAuthState != null)
			{
				userPrincipal = GetAuthPrincipal(targetAuthState);
				if (userPrincipal == null)
				{
					AuthState proxyAuthState = clientContext.GetProxyAuthState();
					userPrincipal = GetAuthPrincipal(proxyAuthState);
				}
			}
			if (userPrincipal == null)
			{
				HttpConnection conn = clientContext.GetConnection();
				if (conn.IsOpen() && conn is ManagedHttpClientConnection)
				{
					SSLSession sslsession = ((ManagedHttpClientConnection)conn).GetSSLSession();
					if (sslsession != null)
					{
						userPrincipal = sslsession.GetLocalPrincipal();
					}
				}
			}
			return userPrincipal;
		}

		private static Principal GetAuthPrincipal(AuthState authState)
		{
			AuthScheme scheme = authState.GetAuthScheme();
			if (scheme != null && scheme.IsComplete() && scheme.IsConnectionBased())
			{
				Credentials creds = authState.GetCredentials();
				if (creds != null)
				{
					return creds.GetUserPrincipal();
				}
			}
			return null;
		}
	}
}
