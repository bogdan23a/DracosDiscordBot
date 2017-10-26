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

using System.Collections.Generic;
using Apache.Http.Auth;
using Apache.Http.Client;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// Default implementation of
	/// <see cref="Apache.Http.Client.CredentialsProvider">Apache.Http.Client.CredentialsProvider
	/// 	</see>
	/// .
	/// </summary>
	/// <since>4.0</since>
	public class BasicCredentialsProvider : CredentialsProvider
	{
		private readonly ConcurrentHashMap<AuthScope, Credentials> credMap;

		/// <summary>Default constructor.</summary>
		/// <remarks>Default constructor.</remarks>
		public BasicCredentialsProvider() : base()
		{
			this.credMap = new ConcurrentHashMap<AuthScope, Credentials>();
		}

		public virtual void SetCredentials(AuthScope authscope, Credentials credentials)
		{
			Args.NotNull(authscope, "Authentication scope");
			credMap.Put(authscope, credentials);
		}

		/// <summary>
		/// Find matching
		/// <see cref="Apache.Http.Auth.Credentials">credentials</see>
		/// for the given authentication scope.
		/// </summary>
		/// <param name="map">the credentials hash map</param>
		/// <param name="authscope">
		/// the
		/// <see cref="Apache.Http.Auth.AuthScope">authentication scope</see>
		/// </param>
		/// <returns>the credentials</returns>
		private static Credentials MatchCredentials(IDictionary<AuthScope, Credentials> map
			, AuthScope authscope)
		{
			// see if we get a direct hit
			Credentials creds = map.Get(authscope);
			if (creds == null)
			{
				// Nope.
				// Do a full scan
				int bestMatchFactor = -1;
				AuthScope bestMatch = null;
				foreach (AuthScope current in map.Keys)
				{
					int factor = authscope.Match(current);
					if (factor > bestMatchFactor)
					{
						bestMatchFactor = factor;
						bestMatch = current;
					}
				}
				if (bestMatch != null)
				{
					creds = map.Get(bestMatch);
				}
			}
			return creds;
		}

		public virtual Credentials GetCredentials(AuthScope authscope)
		{
			Args.NotNull(authscope, "Authentication scope");
			return MatchCredentials(this.credMap, authscope);
		}

		public virtual void Clear()
		{
			this.credMap.Clear();
		}

		public override string ToString()
		{
			return credMap.ToString();
		}
	}
}
