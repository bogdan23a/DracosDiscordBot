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
	/// <see cref="Sharpen.SSLContext">Sharpen.SSLContext</see>
	/// factory methods.
	/// </summary>
	/// <since>4.3</since>
	public class SSLContexts
	{
		/// <summary>
		/// Creates default factory based on the standard JSSE trust material
		/// (<code>cacerts</code> file in the security properties directory).
		/// </summary>
		/// <remarks>
		/// Creates default factory based on the standard JSSE trust material
		/// (<code>cacerts</code> file in the security properties directory). System properties
		/// are not taken into consideration.
		/// </remarks>
		/// <returns>the default SSL socket factory</returns>
		/// <exception cref="Apache.Http.Conn.Ssl.SSLInitializationException"></exception>
		public static SSLContext CreateDefault()
		{
			try
			{
				SSLContext sslcontext = SSLContext.GetInstance(SSLContextBuilder.Tls);
				sslcontext.Init(null, null, null);
				return sslcontext;
			}
			catch (NoSuchAlgorithmException ex)
			{
				throw new SSLInitializationException(ex.Message, ex);
			}
			catch (KeyManagementException ex)
			{
				throw new SSLInitializationException(ex.Message, ex);
			}
		}

		/// <summary>Creates default SSL context based on system properties.</summary>
		/// <remarks>
		/// Creates default SSL context based on system properties. This method obtains
		/// default SSL context by calling <code>SSLContext.getInstance("Default")</code>.
		/// Please note that <code>Default</code> algorithm is supported as of Java 6.
		/// This method will fall back onto
		/// <see cref="CreateDefault()">CreateDefault()</see>
		/// when
		/// <code>Default</code> algorithm is not available.
		/// </remarks>
		/// <returns>default system SSL context</returns>
		/// <exception cref="Apache.Http.Conn.Ssl.SSLInitializationException"></exception>
		public static SSLContext CreateSystemDefault()
		{
			try
			{
				return SSLContext.GetInstance("Default");
			}
			catch (NoSuchAlgorithmException)
			{
				return CreateDefault();
			}
		}

		/// <summary>Creates custom SSL context.</summary>
		/// <remarks>Creates custom SSL context.</remarks>
		/// <returns>default system SSL context</returns>
		public static SSLContextBuilder Custom()
		{
			return new SSLContextBuilder();
		}
	}
}
