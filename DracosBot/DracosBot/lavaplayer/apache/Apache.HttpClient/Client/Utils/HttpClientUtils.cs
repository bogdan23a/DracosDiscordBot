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

using System;
using System.IO;
using Apache.Http;
using Apache.Http.Client;
using Apache.Http.Client.Methods;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Client.Utils
{
	/// <summary>Convenience methods for closing response and client objects.</summary>
	/// <remarks>Convenience methods for closing response and client objects.</remarks>
	/// <since>4.2</since>
	public class HttpClientUtils
	{
		private HttpClientUtils()
		{
		}

		/// <summary>Unconditionally close a response.</summary>
		/// <remarks>
		/// Unconditionally close a response.
		/// <p>
		/// Example Code:
		/// <pre>
		/// HttpResponse httpResponse = null;
		/// try {
		/// httpResponse = httpClient.execute(httpGet);
		/// } catch (Exception e) {
		/// // error handling
		/// } finally {
		/// HttpClientUtils.closeQuietly(httpResponse);
		/// }
		/// </pre>
		/// </remarks>
		/// <param name="response">
		/// the HttpResponse to release resources, may be null or already
		/// closed.
		/// </param>
		/// <since>4.2</since>
		public static void CloseQuietly(HttpResponse response)
		{
			if (response != null)
			{
				HttpEntity entity = response.GetEntity();
				if (entity != null)
				{
					try
					{
						EntityUtils.Consume(entity);
					}
					catch (IOException)
					{
					}
				}
			}
		}

		/// <summary>Unconditionally close a response.</summary>
		/// <remarks>
		/// Unconditionally close a response.
		/// <p>
		/// Example Code:
		/// <pre>
		/// HttpResponse httpResponse = null;
		/// try {
		/// httpResponse = httpClient.execute(httpGet);
		/// } catch (Exception e) {
		/// // error handling
		/// } finally {
		/// HttpClientUtils.closeQuietly(httpResponse);
		/// }
		/// </pre>
		/// </remarks>
		/// <param name="response">
		/// the HttpResponse to release resources, may be null or already
		/// closed.
		/// </param>
		/// <since>4.3</since>
		public static void CloseQuietly(CloseableHttpResponse response)
		{
			if (response != null)
			{
				try
				{
					try
					{
						EntityUtils.Consume(response.GetEntity());
					}
					finally
					{
						response.Close();
					}
				}
				catch (IOException)
				{
				}
			}
		}

		/// <summary>Unconditionally close a httpClient.</summary>
		/// <remarks>
		/// Unconditionally close a httpClient. Shuts down the underlying connection
		/// manager and releases the resources.
		/// <p>
		/// Example Code:
		/// <pre>
		/// HttpClient httpClient = HttpClients.createDefault();
		/// try {
		/// httpClient.execute(request);
		/// } catch (Exception e) {
		/// // error handling
		/// } finally {
		/// HttpClientUtils.closeQuietly(httpClient);
		/// }
		/// </pre>
		/// </remarks>
		/// <param name="httpClient">the HttpClient to close, may be null or already closed.</param>
		/// <since>4.2</since>
		public static void CloseQuietly(HttpClient httpClient)
		{
			if (httpClient != null)
			{
				if (httpClient is IDisposable)
				{
					try
					{
						((IDisposable)httpClient).Close();
					}
					catch (IOException)
					{
					}
				}
			}
		}
	}
}
