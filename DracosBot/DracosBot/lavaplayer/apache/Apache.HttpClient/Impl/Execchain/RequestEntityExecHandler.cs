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
using System.Reflection;
using Apache.Http;
using Sharpen;
using Sharpen.Reflect;

namespace Apache.Http.Impl.Execchain
{
	/// <summary>
	/// A wrapper class for
	/// <see cref="Apache.Http.HttpEntity">Apache.Http.HttpEntity</see>
	/// enclosed in a request message.
	/// </summary>
	/// <since>4.3</since>
	internal class RequestEntityExecHandler : InvocationHandler
	{
		private static readonly MethodInfo WriteToMethod;

		static RequestEntityExecHandler()
		{
			try
			{
				WriteToMethod = typeof(HttpEntity).GetMethod("writeTo", typeof(OutputStream));
			}
			catch (NoSuchMethodException ex)
			{
				throw new Error(ex);
			}
		}

		private readonly HttpEntity original;

		private bool consumed = false;

		internal RequestEntityExecHandler(HttpEntity original) : base()
		{
			this.original = original;
		}

		public virtual HttpEntity GetOriginal()
		{
			return original;
		}

		public virtual bool IsConsumed()
		{
			return consumed;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual object Invoke(object proxy, MethodInfo method, object[] args)
		{
			try
			{
				if (method.Equals(WriteToMethod))
				{
					this.consumed = true;
				}
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
