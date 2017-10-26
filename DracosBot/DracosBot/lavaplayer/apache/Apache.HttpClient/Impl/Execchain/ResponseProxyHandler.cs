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
using Apache.Http.Impl.Execchain;
using Sharpen;
using Sharpen.Reflect;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>
	/// A proxy class for
	/// <see cref="Apache.Http.HttpResponse">Apache.Http.HttpResponse</see>
	/// that can be used to release client connection
	/// associated with the original response.
	/// </summary>
	/// <since>4.3</since>
	internal class ResponseProxyHandler : InvocationHandler
	{
		private static readonly MethodInfo CloseMethod;

		static ResponseProxyHandler()
		{
			try
			{
				CloseMethod = typeof(IDisposable).GetMethod("close");
			}
			catch (NoSuchMethodException ex)
			{
				throw new Error(ex);
			}
		}

		private readonly HttpResponse original;

		private readonly ConnectionHolder connHolder;

		internal ResponseProxyHandler(HttpResponse original, ConnectionHolder connHolder)
			 : base()
		{
			this.original = original;
			this.connHolder = connHolder;
			HttpEntity entity = original.GetEntity();
			if (entity != null && entity.IsStreaming() && connHolder != null)
			{
				this.original.SetEntity(new ResponseEntityWrapper(entity, connHolder));
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Close()
		{
			if (this.connHolder != null)
			{
				this.connHolder.AbortConnection();
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual object Invoke(object proxy, MethodInfo method, object[] args)
		{
			if (method.Equals(CloseMethod))
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
	}
}
