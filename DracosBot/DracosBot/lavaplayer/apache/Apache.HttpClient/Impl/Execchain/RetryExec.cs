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

using System.IO;
using Apache.Http;
using Apache.Http.Client;
using Apache.Http.Client.Methods;
using Apache.Http.Client.Protocol;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Execchain;
using Apache.Http.Util;
using Org.Apache.Commons.Logging;
using Sharpen;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>
	/// Request executor in the request execution chain that is responsible
	/// for making a decision whether a request failed due to an I/O error
	/// should be re-executed.
	/// </summary>
	/// <remarks>
	/// Request executor in the request execution chain that is responsible
	/// for making a decision whether a request failed due to an I/O error
	/// should be re-executed.
	/// <p/>
	/// Further responsibilities such as communication with the opposite
	/// endpoint is delegated to the next executor in the request execution
	/// chain.
	/// </remarks>
	/// <since>4.3</since>
	public class RetryExec : ClientExecChain
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly ClientExecChain requestExecutor;

		private readonly HttpRequestRetryHandler retryHandler;

		public RetryExec(ClientExecChain requestExecutor, HttpRequestRetryHandler retryHandler
			)
		{
			Args.NotNull(requestExecutor, "HTTP request executor");
			Args.NotNull(retryHandler, "HTTP request retry handler");
			this.requestExecutor = requestExecutor;
			this.retryHandler = retryHandler;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.HttpException"></exception>
		public virtual CloseableHttpResponse Execute(HttpRoute route, HttpRequestWrapper 
			request, HttpClientContext context, HttpExecutionAware execAware)
		{
			Args.NotNull(route, "HTTP route");
			Args.NotNull(request, "HTTP request");
			Args.NotNull(context, "HTTP context");
			Header[] origheaders = request.GetAllHeaders();
			for (int execCount = 1; ; execCount++)
			{
				try
				{
					return this.requestExecutor.Execute(route, request, context, execAware);
				}
				catch (IOException ex)
				{
					if (execAware != null && execAware.IsAborted())
					{
						this.log.Debug("Request has been aborted");
						throw;
					}
					if (retryHandler.RetryRequest(ex, execCount, context))
					{
						if (this.log.IsInfoEnabled())
						{
							this.log.Info("I/O exception (" + ex.GetType().FullName + ") caught when processing request: "
								 + ex.Message);
						}
						if (this.log.IsDebugEnabled())
						{
							this.log.Debug(ex.Message, ex);
						}
						if (!Proxies.IsRepeatable(request))
						{
							this.log.Debug("Cannot retry non-repeatable request");
							throw new NonRepeatableRequestException("Cannot retry request " + "with a non-repeatable request entity"
								, ex);
						}
						request.SetHeaders(origheaders);
						this.log.Info("Retrying request");
					}
					else
					{
						throw;
					}
				}
			}
		}
	}
}
