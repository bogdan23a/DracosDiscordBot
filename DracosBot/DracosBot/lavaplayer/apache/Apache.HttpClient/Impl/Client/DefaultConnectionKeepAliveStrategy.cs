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
using Apache.Http;
using Apache.Http.Conn;
using Apache.Http.Impl.Client;
using Apache.Http.Message;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Default implementation of a strategy deciding duration
	/// that a connection can remain idle.
	/// </summary>
	/// <remarks>
	/// Default implementation of a strategy deciding duration
	/// that a connection can remain idle.
	/// The default implementation looks solely at the 'Keep-Alive'
	/// header's timeout token.
	/// </remarks>
	/// <since>4.0</since>
	public class DefaultConnectionKeepAliveStrategy : ConnectionKeepAliveStrategy
	{
		public static readonly DefaultConnectionKeepAliveStrategy Instance = new DefaultConnectionKeepAliveStrategy
			();

		public virtual long GetKeepAliveDuration(HttpResponse response, HttpContext context
			)
		{
			Args.NotNull(response, "HTTP response");
			HeaderElementIterator it = new BasicHeaderElementIterator(response.HeaderIterator
				(HTTP.ConnKeepAlive));
			while (it.HasNext())
			{
				HeaderElement he = it.NextElement();
				string param = he.GetName();
				string value = he.GetValue();
				if (value != null && Sharpen.Runtime.EqualsIgnoreCase(param, "timeout"))
				{
					try
					{
						return long.Parse(value) * 1000;
					}
					catch (FormatException)
					{
					}
				}
			}
			return -1;
		}
	}
}
