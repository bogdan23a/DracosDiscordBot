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
	/// The ALLOW_ALL HostnameVerifier essentially turns hostname verification
	/// off.
	/// </summary>
	/// <remarks>
	/// The ALLOW_ALL HostnameVerifier essentially turns hostname verification
	/// off. This implementation is a no-op, and never throws the SSLException.
	/// </remarks>
	/// <since>4.0</since>
	public class AllowAllHostnameVerifier : AbstractVerifier
	{
		public sealed override void Verify(string host, string[] cns, string[] subjectAlts
			)
		{
		}

		// Allow everything - so never blowup.
		public sealed override string ToString()
		{
			return "ALLOW_ALL";
		}
	}
}
