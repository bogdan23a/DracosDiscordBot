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

using Apache.Http.Auth.Params;
using Apache.Http.Client.Params;
using Apache.Http.Conn.Params;
using Apache.Http.Cookie.Params;
using Apache.Http.Params;
using Sharpen;

namespace Apache.Http.Client.Params
{
	/// <summary>Collected parameter names for the HttpClient module.</summary>
	/// <remarks>
	/// Collected parameter names for the HttpClient module.
	/// This interface combines the parameter definitions of the HttpClient
	/// module and all dependency modules or informational units.
	/// It does not define additional parameter names, but references
	/// other interfaces defining parameter names.
	/// <br/>
	/// This interface is meant as a navigation aid for developers.
	/// When referring to parameter names, you should use the interfaces
	/// in which the respective constants are actually defined.
	/// </remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) useApache.Http.Client.Config.RequestConfig ,Apache.Http.Config.ConnectionConfig ,Apache.Http.Config.SocketConfig"
		)]
	public interface AllClientPNames : CoreConnectionPNames, CoreProtocolPNames, ClientPNames
		, AuthPNames, CookieSpecPNames, ConnConnectionPNames, ConnManagerPNames, ConnRoutePNames
	{
		// no additional definitions
	}
}
