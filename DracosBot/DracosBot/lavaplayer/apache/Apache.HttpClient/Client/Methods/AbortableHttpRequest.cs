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

using Apache.Http.Client.Methods;
using Apache.Http.Conn;
using Sharpen;

namespace Apache.Http.Client.Methods
{
	/// <summary>
	/// Interface representing an HTTP request that can be aborted by shutting
	/// down the underlying HTTP connection.
	/// </summary>
	/// <remarks>
	/// Interface representing an HTTP request that can be aborted by shutting
	/// down the underlying HTTP connection.
	/// </remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) use HttpExecutionAware")]
	public interface AbortableHttpRequest
	{
		/// <summary>
		/// Sets the
		/// <see cref="Apache.Http.Conn.ClientConnectionRequest">Apache.Http.Conn.ClientConnectionRequest
		/// 	</see>
		/// callback that can be used to abort a long-lived request for a connection.
		/// If the request is already aborted, throws an
		/// <see cref="System.IO.IOException">System.IO.IOException</see>
		/// .
		/// </summary>
		/// <seealso cref="Apache.Http.Conn.ClientConnectionManager">Apache.Http.Conn.ClientConnectionManager
		/// 	</seealso>
		/// <exception cref="System.IO.IOException"></exception>
		void SetConnectionRequest(ClientConnectionRequest connRequest);

		/// <summary>
		/// Sets the
		/// <see cref="Apache.Http.Conn.ConnectionReleaseTrigger">Apache.Http.Conn.ConnectionReleaseTrigger
		/// 	</see>
		/// callback that can
		/// be used to abort an active connection.
		/// Typically, this will be the
		/// <see cref="Apache.Http.Conn.ManagedClientConnection">Apache.Http.Conn.ManagedClientConnection
		/// 	</see>
		/// itself.
		/// If the request is already aborted, throws an
		/// <see cref="System.IO.IOException">System.IO.IOException</see>
		/// .
		/// </summary>
		/// <exception cref="System.IO.IOException"></exception>
		void SetReleaseTrigger(ConnectionReleaseTrigger releaseTrigger);

		/// <summary>Aborts this http request.</summary>
		/// <remarks>
		/// Aborts this http request. Any active execution of this method should
		/// return immediately. If the request has not started, it will abort after
		/// the next execution. Aborting this request will cause all subsequent
		/// executions with this request to fail.
		/// </remarks>
		/// <seealso cref="Apache.Http.Client.HttpClient.Execute(IHttpUriRequest)">Apache.Http.Client.HttpClient.Execute(IHttpUriRequest)
		/// 	</seealso>
		/// <seealso cref="Apache.Http.Client.HttpClient.Execute(Apache.Http.HttpHost, Org.Apache.Http.IHttpRequest)
		/// 	">Apache.Http.Client.HttpClient.Execute(Apache.Http.HttpHost, Org.Apache.Http.IHttpRequest)
		/// 	</seealso>
		/// <seealso cref="Apache.Http.Client.HttpClient.Execute(IHttpUriRequest, Apache.Http.Protocol.HttpContext)
		/// 	">Apache.Http.Client.HttpClient.Execute(IHttpUriRequest, Apache.Http.Protocol.HttpContext)
		/// 	</seealso>
		/// <seealso cref="Apache.Http.Client.HttpClient.Execute(Apache.Http.HttpHost, Org.Apache.Http.IHttpRequest, Apache.Http.Protocol.HttpContext)
		/// 	">Apache.Http.Client.HttpClient.Execute(Apache.Http.HttpHost, Org.Apache.Http.IHttpRequest, Apache.Http.Protocol.HttpContext)
		/// 	</seealso>
		void Abort();
	}
}
