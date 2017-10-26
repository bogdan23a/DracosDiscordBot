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

using Apache.Http.Client;
using Apache.Http.Protocol;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// A handler for determining if the given execution context is user specific
	/// or not.
	/// </summary>
	/// <remarks>
	/// A handler for determining if the given execution context is user specific
	/// or not. The token object returned by this handler is expected to uniquely
	/// identify the current user if the context is user specific or to be
	/// <code>null</code> if the context does not contain any resources or details
	/// specific to the current user.
	/// <p/>
	/// The user token will be used to ensure that user specific resources will not
	/// be shared with or reused by other users.
	/// </remarks>
	/// <since>4.0</since>
	public interface UserTokenHandler
	{
		/// <summary>
		/// The token object returned by this method is expected to uniquely
		/// identify the current user if the context is user specific or to be
		/// <code>null</code> if it is not.
		/// </summary>
		/// <remarks>
		/// The token object returned by this method is expected to uniquely
		/// identify the current user if the context is user specific or to be
		/// <code>null</code> if it is not.
		/// </remarks>
		/// <param name="context">the execution context</param>
		/// <returns>
		/// user token that uniquely identifies the user or
		/// <code>null</null> if the context is not user specific.
		/// </returns>
		object GetUserToken(HttpContext context);
	}
}
