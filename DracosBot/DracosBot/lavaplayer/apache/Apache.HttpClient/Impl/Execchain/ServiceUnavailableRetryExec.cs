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
using System.Threading;
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
	/// for making a decision whether a request that received a non-2xx response
	/// from the target server should be re-executed.
	/// </summary>
	/// <remarks>
	/// Request executor in the request execution chain that is responsible
	/// for making a decision whether a request that received a non-2xx response
	/// from the target server should be re-executed.
	/// <p/>
	/// Further responsibilities such as communication with the opposite
	/// endpoint is delegated to the next executor in the request execution
	/// chain.
	/// </remarks>
	/// <since>4.3</since>
	public class ServiceUnavailableRetryExec : ClientExecChain
	{
		private readonly Log log = LogFactory.GetLog(GetType());

		private readonly ClientExecChain requestExecutor;

		private readonly ServiceUnavailableRetryStrategy retryStrategy;

		public ServiceUnavailableRetryExec(ClientExecChain requestExecutor, ServiceUnavailableRetryStrategy
			 retryStrategy) : base()
		{
			Args.NotNull(requestExecutor, "HTTP request executor");
			Args.NotNull(retryStrategy, "Retry strategy");
			this.requestExecutor = requestExecutor;
			this.retryStrategy = retryStrategy;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.HttpException"></exception>
		public virtual CloseableHttpResponse Execute(HttpRoute route, HttpRequestWrapper 
			request, HttpClientContext context, HttpExecutionAware execAware)
		{
			for (int c = 1; ; c++)
			{
				CloseableHttpResponse response = this.requestExecutor.Execute(route, request, context
					, execAware);
				try
				{
					if (this.retryStrategy.RetryRequest(response, c, context))
					{
						response.Close();
						long nextInterval = this.retryStrategy.GetRetryInterval();
						if (nextInterval > 0)
						{
							try
							{
								this.log.Trace("Wait for " + nextInterval);
								Sharpen.Thread.Sleep(nextInterval);
							}
							catch (Exception)
							{
								Sharpen.Thread.CurrentThread().Interrupt();
								throw new ThreadInterruptedException();
							}
						}
					}
					else
					{
						return response;
					}
				}
				catch (RuntimeException ex)
				{
					response.Close();
					throw;
				}
			}
		}
	}
}
