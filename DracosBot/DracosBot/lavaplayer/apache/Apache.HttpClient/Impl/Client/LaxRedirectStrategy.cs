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

using Apache.Http.Client.Methods;
using Apache.Http.Impl.Client;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Lax
	/// <see cref="Apache.Http.Client.RedirectStrategy">Apache.Http.Client.RedirectStrategy
	/// 	</see>
	/// implementation
	/// that automatically redirects all HEAD, GET and POST requests.
	/// This strategy relaxes restrictions on automatic redirection of
	/// POST methods imposed by the HTTP specification.
	/// </summary>
	/// <since>4.2</since>
	public class LaxRedirectStrategy : DefaultRedirectStrategy
	{
		/// <summary>Redirectable methods.</summary>
		/// <remarks>Redirectable methods.</remarks>
		private static readonly string[] RedirectMethods = new string[] { HttpGet.MethodName
			, HttpPost.MethodName, HttpHead.MethodName };

		protected internal override bool IsRedirectable(string method)
		{
			foreach (string m in RedirectMethods)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(m, method))
				{
					return true;
				}
			}
			return false;
		}
	}
}
