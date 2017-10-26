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

using Apache.Http.Conn;
using Apache.Http.Impl.Client;
using Apache.Http.Impl.Conn;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Factory methods for
	/// <see cref="CloseableHttpClient">CloseableHttpClient</see>
	/// instances.
	/// </summary>
	/// <since>4.3</since>
	public class HttpClients
	{
		private HttpClients() : base()
		{
		}

		/// <summary>
		/// Creates builder object for construction of custom
		/// <see cref="CloseableHttpClient">CloseableHttpClient</see>
		/// instances.
		/// </summary>
		public static HttpClientBuilder Custom()
		{
			return HttpClientBuilder.Create();
		}

		/// <summary>
		/// Creates
		/// <see cref="CloseableHttpClient">CloseableHttpClient</see>
		/// instance with default
		/// configuration.
		/// </summary>
		public static CloseableHttpClient CreateDefault()
		{
			return HttpClientBuilder.Create().Build();
		}

		/// <summary>
		/// Creates
		/// <see cref="CloseableHttpClient">CloseableHttpClient</see>
		/// instance with default
		/// configuration based on ssytem properties.
		/// </summary>
		public static CloseableHttpClient CreateSystem()
		{
			return HttpClientBuilder.Create().UseSystemProperties().Build();
		}

		/// <summary>
		/// Creates
		/// <see cref="CloseableHttpClient">CloseableHttpClient</see>
		/// instance that implements
		/// the most basic HTTP protocol support.
		/// </summary>
		public static CloseableHttpClient CreateMinimal()
		{
			return new MinimalHttpClient(new PoolingHttpClientConnectionManager());
		}

		/// <summary>
		/// Creates
		/// <see cref="CloseableHttpClient">CloseableHttpClient</see>
		/// instance that implements
		/// the most basic HTTP protocol support.
		/// </summary>
		public static CloseableHttpClient CreateMinimal(HttpClientConnectionManager connManager
			)
		{
			return new MinimalHttpClient(connManager);
		}
	}
}
