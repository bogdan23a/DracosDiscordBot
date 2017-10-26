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
using System.Net;
using Apache.Http.Conn;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Conn
{
	/// <summary>
	/// In-memory
	/// <see cref="Apache.Http.Conn.DnsResolver">Apache.Http.Conn.DnsResolver</see>
	/// implementation.
	/// </summary>
	/// <since>4.2</since>
	public class InMemoryDnsResolver : DnsResolver
	{
		/// <summary>Logger associated to this class.</summary>
		/// <remarks>Logger associated to this class.</remarks>
		private readonly Log log = LogFactory.GetLog(typeof(Apache.Http.Impl.Conn.InMemoryDnsResolver
			));

		/// <summary>
		/// In-memory collection that will hold the associations between a host name
		/// and an array of InetAddress instances.
		/// </summary>
		/// <remarks>
		/// In-memory collection that will hold the associations between a host name
		/// and an array of InetAddress instances.
		/// </remarks>
		private readonly IDictionary<string, IPAddress[]> dnsMap;

		/// <summary>
		/// Builds a DNS resolver that will resolve the host names against a
		/// collection held in-memory.
		/// </summary>
		/// <remarks>
		/// Builds a DNS resolver that will resolve the host names against a
		/// collection held in-memory.
		/// </remarks>
		public InMemoryDnsResolver()
		{
			dnsMap = new ConcurrentHashMap<string, IPAddress[]>();
		}

		/// <summary>Associates the given array of IP addresses to the given host in this DNS overrider.
		/// 	</summary>
		/// <remarks>
		/// Associates the given array of IP addresses to the given host in this DNS overrider.
		/// The IP addresses are assumed to be already resolved.
		/// </remarks>
		/// <param name="host">The host name to be associated with the given IP.</param>
		/// <param name="ips">
		/// array of IP addresses to be resolved by this DNS overrider to the given
		/// host name.
		/// </param>
		public virtual void Add(string host, params IPAddress[] ips)
		{
			Args.NotNull(host, "Host name");
			Args.NotNull(ips, "Array of IP addresses");
			dnsMap.Put(host, ips);
		}

		/// <summary><inheritDoc></inheritDoc></summary>
		/// <exception cref="Sharpen.UnknownHostException"></exception>
		public virtual IPAddress[] Resolve(string host)
		{
			IPAddress[] resolvedAddresses = dnsMap.Get(host);
			if (log.IsInfoEnabled())
			{
				log.Info("Resolving " + host + " to " + Arrays.DeepToString(resolvedAddresses));
			}
			if (resolvedAddresses == null)
			{
				throw new UnknownHostException(host + " cannot be resolved");
			}
			return resolvedAddresses;
		}
	}
}
