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
using Apache.Http.Client.Protocol;
using Apache.Http.Conn.Routing;
using Apache.Http.Impl.Execchain;
using Apache.Http.Util;
using Sharpen;
using Sharpen.Reflect;

namespace Apache.Http.Impl.Execchain
{
	/// <since>4.3</since>
	public class BackoffStrategyExec : ClientExecChain
	{
		private readonly ClientExecChain requestExecutor;

		private readonly ConnectionBackoffStrategy connectionBackoffStrategy;

		private readonly BackoffManager backoffManager;

		public BackoffStrategyExec(ClientExecChain requestExecutor, ConnectionBackoffStrategy
			 connectionBackoffStrategy, BackoffManager backoffManager) : base()
		{
			Args.NotNull(requestExecutor, "HTTP client request executor");
			Args.NotNull(connectionBackoffStrategy, "Connection backoff strategy");
			Args.NotNull(backoffManager, "Backoff manager");
			this.requestExecutor = requestExecutor;
			this.connectionBackoffStrategy = connectionBackoffStrategy;
			this.backoffManager = backoffManager;
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Apache.Http.HttpException"></exception>
		public virtual CloseableHttpResponse Execute(HttpRoute route, HttpRequestWrapper 
			request, HttpClientContext context, HttpExecutionAware execAware)
		{
			Args.NotNull(route, "HTTP route");
			Args.NotNull(request, "HTTP request");
			Args.NotNull(context, "HTTP context");
			CloseableHttpResponse @out = null;
			try
			{
				@out = this.requestExecutor.Execute(route, request, context, execAware);
			}
			catch (Exception ex)
			{
				if (@out != null)
				{
					@out.Close();
				}
				if (this.connectionBackoffStrategy.ShouldBackoff(ex))
				{
					this.backoffManager.BackOff(route);
				}
				if (ex is RuntimeException)
				{
					throw (RuntimeException)ex;
				}
				if (ex is HttpException)
				{
					throw (HttpException)ex;
				}
				if (ex is IOException)
				{
					throw (IOException)ex;
				}
				throw new UndeclaredThrowableException(ex);
			}
			if (this.connectionBackoffStrategy.ShouldBackoff(@out))
			{
				this.backoffManager.BackOff(route);
			}
			else
			{
				this.backoffManager.Probe(route);
			}
			return @out;
		}
	}
}
