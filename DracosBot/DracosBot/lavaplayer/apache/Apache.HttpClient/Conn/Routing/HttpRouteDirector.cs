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

using Apache.Http.Conn.Routing;
using Sharpen;

namespace Apache.Http.Conn.Routing
{
	/// <summary>Provides directions on establishing a route.</summary>
	/// <remarks>
	/// Provides directions on establishing a route.
	/// Implementations of this interface compare a planned route with
	/// a tracked route and indicate the next step required.
	/// </remarks>
	/// <since>4.0</since>
	public abstract class HttpRouteDirector
	{
		/// <summary>Indicates that the route can not be established at all.</summary>
		/// <remarks>Indicates that the route can not be established at all.</remarks>
		public const int Unreachable = -1;

		/// <summary>Indicates that the route is complete.</summary>
		/// <remarks>Indicates that the route is complete.</remarks>
		public const int Complete = 0;

		/// <summary>Step: open connection to target.</summary>
		/// <remarks>Step: open connection to target.</remarks>
		public const int ConnectTarget = 1;

		/// <summary>Step: open connection to proxy.</summary>
		/// <remarks>Step: open connection to proxy.</remarks>
		public const int ConnectProxy = 2;

		/// <summary>Step: tunnel through proxy to target.</summary>
		/// <remarks>Step: tunnel through proxy to target.</remarks>
		public const int TunnelTarget = 3;

		/// <summary>Step: tunnel through proxy to other proxy.</summary>
		/// <remarks>Step: tunnel through proxy to other proxy.</remarks>
		public const int TunnelProxy = 4;

		/// <summary>Step: layer protocol (over tunnel).</summary>
		/// <remarks>Step: layer protocol (over tunnel).</remarks>
		public const int LayerProtocol = 5;

		/// <summary>Provides the next step.</summary>
		/// <remarks>Provides the next step.</remarks>
		/// <param name="plan">the planned route</param>
		/// <param name="fact">
		/// the currently established route, or
		/// <code>null</code> if nothing is established
		/// </param>
		/// <returns>
		/// one of the constants defined in this interface, indicating
		/// either the next step to perform, or success, or failure.
		/// 0 is for success, a negative value for failure.
		/// </returns>
		public abstract int NextStep(RouteInfo plan, RouteInfo fact);
	}
}
