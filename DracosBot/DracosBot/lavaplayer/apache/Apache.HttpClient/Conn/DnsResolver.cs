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

using System.Net;
using Apache.Http.Conn;
using Sharpen;

namespace Apache.Http.Conn
{
	/// <summary>
	/// Users may implement this interface to override the normal DNS lookup offered
	/// by the OS.
	/// </summary>
	/// <remarks>
	/// Users may implement this interface to override the normal DNS lookup offered
	/// by the OS.
	/// </remarks>
	/// <since>4.2</since>
	public interface DnsResolver
	{
		/// <summary>
		/// Returns the IP address for the specified host name, or null if the given
		/// host is not recognized or the associated IP address cannot be used to
		/// build an InetAddress instance.
		/// </summary>
		/// <remarks>
		/// Returns the IP address for the specified host name, or null if the given
		/// host is not recognized or the associated IP address cannot be used to
		/// build an InetAddress instance.
		/// </remarks>
		/// <seealso cref="System.Net.IPAddress">System.Net.IPAddress</seealso>
		/// <param name="host">The host name to be resolved by this resolver.</param>
		/// <returns>
		/// The IP address associated to the given host name, or null if the
		/// host name is not known by the implementation class.
		/// </returns>
		/// <exception cref="Sharpen.UnknownHostException"></exception>
		IPAddress[] Resolve(string host);
	}
}
