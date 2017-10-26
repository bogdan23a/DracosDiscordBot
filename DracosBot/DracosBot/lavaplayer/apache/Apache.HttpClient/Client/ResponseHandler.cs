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
using Apache.Http.Client;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// Handler that encapsulates the process of generating a response object
	/// from a
	/// <see cref="Apache.Http.HttpResponse">Apache.Http.HttpResponse</see>
	/// .
	/// </summary>
	/// <since>4.0</since>
	public interface ResponseHandler<T>
	{
		/// <summary>
		/// Processes an
		/// <see cref="Apache.Http.HttpResponse">Apache.Http.HttpResponse</see>
		/// and returns some value
		/// corresponding to that response.
		/// </summary>
		/// <param name="response">The response to process</param>
		/// <returns>A value determined by the response</returns>
		/// <exception cref="ClientProtocolException">in case of an http protocol error</exception>
		/// <exception cref="System.IO.IOException">in case of a problem or the connection was aborted
		/// 	</exception>
		/// <exception cref="Apache.Http.Client.ClientProtocolException"></exception>
		T HandleResponse(HttpResponse response);
	}
}
