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
using System.Reflection;
using Apache.Http;
using Apache.Http.Client.Methods;
using Apache.Http.Util;
using Sharpen;
using Sharpen.Reflect;

namespace Apache.Http.Impl.Client
{
	/// <since>4.3</since>
	internal class CloseableHttpResponseProxy : InvocationHandler
	{
		private readonly HttpResponse original;

		internal CloseableHttpResponseProxy(HttpResponse original) : base()
		{
			this.original = original;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Close()
		{
			HttpEntity entity = this.original.GetEntity();
			EntityUtils.Consume(entity);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual object Invoke(object proxy, MethodInfo method, object[] args)
		{
			string mname = method.Name;
			if (mname.Equals("close"))
			{
				Close();
				return null;
			}
			else
			{
				try
				{
					return method.Invoke(original, args);
				}
				catch (TargetInvocationException ex)
				{
					Exception cause = ex.InnerException;
					if (cause != null)
					{
						throw cause;
					}
					else
					{
						throw;
					}
				}
			}
		}

		public static CloseableHttpResponse NewProxy(HttpResponse original)
		{
			return (CloseableHttpResponse)Proxy.NewProxyInstance(typeof(Apache.Http.Impl.Client.CloseableHttpResponseProxy
				).GetClassLoader(), new Type[] { typeof(CloseableHttpResponse) }, new Apache.Http.Impl.Client.CloseableHttpResponseProxy
				(original));
		}
	}
}
