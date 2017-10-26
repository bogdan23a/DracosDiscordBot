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
using Apache.Http.Impl.Client;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// A
	/// <see cref="Apache.Http.Client.ResponseHandler{T}">Apache.Http.Client.ResponseHandler&lt;T&gt;
	/// 	</see>
	/// that returns the response body as a String
	/// for successful (2xx) responses. If the response code was &gt;= 300, the response
	/// body is consumed and an
	/// <see cref="Apache.Http.Client.HttpResponseException">Apache.Http.Client.HttpResponseException
	/// 	</see>
	/// is thrown.
	/// <p/>
	/// If this is used with
	/// <see cref="Apache.Http.Client.HttpClient.Execute{T}(Org.Apache.Http.Client.Methods.IHttpUriRequest, Apache.Http.Client.ResponseHandler{T})
	/// 	">Apache.Http.Client.HttpClient.Execute&lt;T&gt;(Org.Apache.Http.Client.Methods.IHttpUriRequest, Apache.Http.Client.ResponseHandler&lt;T&gt;)
	/// 	</see>
	/// ,
	/// HttpClient may handle redirects (3xx responses) internally.
	/// </summary>
	/// <since>4.0</since>
	public class BasicResponseHandler : ResponseHandler<string>
	{
		/// <summary>
		/// Returns the response body as a String if the response was successful (a
		/// 2xx status code).
		/// </summary>
		/// <remarks>
		/// Returns the response body as a String if the response was successful (a
		/// 2xx status code). If no response body exists, this returns null. If the
		/// response was unsuccessful (&gt;= 300 status code), throws an
		/// <see cref="Apache.Http.Client.HttpResponseException">Apache.Http.Client.HttpResponseException
		/// 	</see>
		/// .
		/// </remarks>
		/// <exception cref="Apache.Http.Client.HttpResponseException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		public virtual string HandleResponse(HttpResponse response)
		{
			StatusLine statusLine = response.GetStatusLine();
			HttpEntity entity = response.GetEntity();
			if (statusLine.GetStatusCode() >= 300)
			{
				EntityUtils.Consume(entity);
				throw new HttpResponseException(statusLine.GetStatusCode(), statusLine.GetReasonPhrase
					());
			}
			return entity == null ? null : EntityUtils.ToString(entity);
		}
	}
}
