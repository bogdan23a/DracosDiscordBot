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
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Conn.Routing
{
	/// <summary>
	/// Basic
	/// <see cref="HttpRouteDirector">HttpRouteDirector</see>
	/// implementation.
	/// </summary>
	/// <since>4.0</since>
	public class BasicRouteDirector : HttpRouteDirector
	{
		/// <summary>Provides the next step.</summary>
		/// <remarks>Provides the next step.</remarks>
		/// <param name="plan">the planned route</param>
		/// <param name="fact">
		/// the currently established route, or
		/// <code>null</code> if nothing is established
		/// </param>
		/// <returns>
		/// one of the constants defined in this class, indicating
		/// either the next step to perform, or success, or failure.
		/// 0 is for success, a negative value for failure.
		/// </returns>
		public override int NextStep(RouteInfo plan, RouteInfo fact)
		{
			Args.NotNull(plan, "Planned route");
			int step = Unreachable;
			if ((fact == null) || (fact.GetHopCount() < 1))
			{
				step = FirstStep(plan);
			}
			else
			{
				if (plan.GetHopCount() > 1)
				{
					step = ProxiedStep(plan, fact);
				}
				else
				{
					step = DirectStep(plan, fact);
				}
			}
			return step;
		}

		// nextStep
		/// <summary>Determines the first step to establish a route.</summary>
		/// <remarks>Determines the first step to establish a route.</remarks>
		/// <param name="plan">the planned route</param>
		/// <returns>the first step</returns>
		protected internal virtual int FirstStep(RouteInfo plan)
		{
			return (plan.GetHopCount() > 1) ? ConnectProxy : ConnectTarget;
		}

		/// <summary>Determines the next step to establish a direct connection.</summary>
		/// <remarks>Determines the next step to establish a direct connection.</remarks>
		/// <param name="plan">the planned route</param>
		/// <param name="fact">the currently established route</param>
		/// <returns>
		/// one of the constants defined in this class, indicating
		/// either the next step to perform, or success, or failure
		/// </returns>
		protected internal virtual int DirectStep(RouteInfo plan, RouteInfo fact)
		{
			if (fact.GetHopCount() > 1)
			{
				return Unreachable;
			}
			if (!plan.GetTargetHost().Equals(fact.GetTargetHost()))
			{
				return Unreachable;
			}
			// If the security is too low, we could now suggest to layer
			// a secure protocol on the direct connection. Layering on direct
			// connections has not been supported in HttpClient 3.x, we don't
			// consider it here until there is a real-life use case for it.
			// Should we tolerate if security is better than planned?
			// (plan.isSecure() && !fact.isSecure())
			if (plan.IsSecure() != fact.IsSecure())
			{
				return Unreachable;
			}
			// Local address has to match only if the plan specifies one.
			if ((plan.GetLocalAddress() != null) && !plan.GetLocalAddress().Equals(fact.GetLocalAddress
				()))
			{
				return Unreachable;
			}
			return Complete;
		}

		/// <summary>Determines the next step to establish a connection via proxy.</summary>
		/// <remarks>Determines the next step to establish a connection via proxy.</remarks>
		/// <param name="plan">the planned route</param>
		/// <param name="fact">the currently established route</param>
		/// <returns>
		/// one of the constants defined in this class, indicating
		/// either the next step to perform, or success, or failure
		/// </returns>
		protected internal virtual int ProxiedStep(RouteInfo plan, RouteInfo fact)
		{
			if (fact.GetHopCount() <= 1)
			{
				return Unreachable;
			}
			if (!plan.GetTargetHost().Equals(fact.GetTargetHost()))
			{
				return Unreachable;
			}
			int phc = plan.GetHopCount();
			int fhc = fact.GetHopCount();
			if (phc < fhc)
			{
				return Unreachable;
			}
			for (int i = 0; i < fhc - 1; i++)
			{
				if (!plan.GetHopTarget(i).Equals(fact.GetHopTarget(i)))
				{
					return Unreachable;
				}
			}
			// now we know that the target matches and proxies so far are the same
			if (phc > fhc)
			{
				return TunnelProxy;
			}
			// need to extend the proxy chain
			// proxy chain and target are the same, check tunnelling and layering
			if ((fact.IsTunnelled() && !plan.IsTunnelled()) || (fact.IsLayered() && !plan.IsLayered
				()))
			{
				return Unreachable;
			}
			if (plan.IsTunnelled() && !fact.IsTunnelled())
			{
				return TunnelTarget;
			}
			if (plan.IsLayered() && !fact.IsLayered())
			{
				return LayerProtocol;
			}
			// tunnel and layering are the same, remains to check the security
			// Should we tolerate if security is better than planned?
			// (plan.isSecure() && !fact.isSecure())
			if (plan.IsSecure() != fact.IsSecure())
			{
				return Unreachable;
			}
			return Complete;
		}
	}
}
