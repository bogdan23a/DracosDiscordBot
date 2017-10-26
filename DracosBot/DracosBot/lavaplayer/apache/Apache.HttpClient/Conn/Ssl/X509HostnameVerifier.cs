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
	/// Interface for checking if a hostname matches the names stored inside the
	/// server's X.509 certificate.
	/// </summary>
	/// <remarks>
	/// Interface for checking if a hostname matches the names stored inside the
	/// server's X.509 certificate.  This interface extends
	/// <see cref="Sharpen.HostnameVerifier">Sharpen.HostnameVerifier</see>
	/// , but it is recommended to use
	/// methods added by X509HostnameVerifier.
	/// </remarks>
	/// <since>4.0</since>
	public interface X509HostnameVerifier : HostnameVerifier
	{
		/// <summary>
		/// Verifies that the host name is an acceptable match with the server's
		/// authentication scheme based on the given
		/// <see cref="Sharpen.SSLSocket">Sharpen.SSLSocket</see>
		/// .
		/// </summary>
		/// <param name="host">the host.</param>
		/// <param name="ssl">the SSL socket.</param>
		/// <exception cref="System.IO.IOException">
		/// if an I/O error occurs or the verification process
		/// fails.
		/// </exception>
		void Verify(string host, SSLSocket ssl);

		/// <summary>
		/// Verifies that the host name is an acceptable match with the server's
		/// authentication scheme based on the given
		/// <see cref="Sharpen.X509Certificate">Sharpen.X509Certificate</see>
		/// .
		/// </summary>
		/// <param name="host">the host.</param>
		/// <param name="cert">the certificate.</param>
		/// <exception cref="Sharpen.SSLException">if the verification process fails.</exception>
		void Verify(string host, X509Certificate cert);

		/// <summary>
		/// Checks to see if the supplied hostname matches any of the supplied CNs
		/// or "DNS" Subject-Alts.
		/// </summary>
		/// <remarks>
		/// Checks to see if the supplied hostname matches any of the supplied CNs
		/// or "DNS" Subject-Alts.  Most implementations only look at the first CN,
		/// and ignore any additional CNs.  Most implementations do look at all of
		/// the "DNS" Subject-Alts. The CNs or Subject-Alts may contain wildcards
		/// according to RFC 2818.
		/// </remarks>
		/// <param name="cns">
		/// CN fields, in order, as extracted from the X.509
		/// certificate.
		/// </param>
		/// <param name="subjectAlts">
		/// Subject-Alt fields of type 2 ("DNS"), as extracted
		/// from the X.509 certificate.
		/// </param>
		/// <param name="host">The hostname to verify.</param>
		/// <exception cref="Sharpen.SSLException">if the verification process fails.</exception>
		void Verify(string host, string[] cns, string[] subjectAlts);
	}
}
