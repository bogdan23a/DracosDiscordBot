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

using Apache.Http.Conn.Ssl;
using Sharpen;

namespace Apache.Http.Conn.Ssl
{
	/// <summary>
	/// A strategy to establish trustworthiness of certificates without consulting the trust manager
	/// configured in the actual SSL context.
	/// </summary>
	/// <remarks>
	/// A strategy to establish trustworthiness of certificates without consulting the trust manager
	/// configured in the actual SSL context. This interface can be used to override the standard
	/// JSSE certificate verification process.
	/// </remarks>
	/// <since>4.1</since>
	public interface TrustStrategy
	{
		/// <summary>
		/// Determines whether the certificate chain can be trusted without consulting the trust manager
		/// configured in the actual SSL context.
		/// </summary>
		/// <remarks>
		/// Determines whether the certificate chain can be trusted without consulting the trust manager
		/// configured in the actual SSL context. This method can be used to override the standard JSSE
		/// certificate verification process.
		/// <p>
		/// Please note that, if this method returns <code>false</code>, the trust manager configured
		/// in the actual SSL context can still clear the certificate as trusted.
		/// </remarks>
		/// <param name="chain">the peer certificate chain</param>
		/// <param name="authType">the authentication type based on the client certificate</param>
		/// <returns>
		/// <code>true</code> if the certificate can be trusted without verification by
		/// the trust manager, <code>false</code> otherwise.
		/// </returns>
		/// <exception cref="Sharpen.CertificateException">thrown if the certificate is not trusted or invalid.
		/// 	</exception>
		bool IsTrusted(X509Certificate[] chain, string authType);
	}
}
