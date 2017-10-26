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
using System.Net;
using Apache.Http;
using Apache.Http.Auth.Params;
using Apache.Http.Client.Config;
using Apache.Http.Client.Params;
using Apache.Http.Conn.Params;
using Apache.Http.Params;
using Sharpen;

namespace Apache.Http.Client.Params
{
	/// <since>4.3</since>
	[System.ObsoleteAttribute(@"(4.3) provided for compatibility with Apache.Http.Params.HttpParams . Do not use."
		)]
	public sealed class HttpClientParamConfig
	{
		private HttpClientParamConfig()
		{
		}

		public static RequestConfig GetRequestConfig(HttpParams @params)
		{
			return RequestConfig.Custom().SetSocketTimeout(@params.GetIntParameter(CoreConnectionPNames
				.SoTimeout, 0)).SetStaleConnectionCheckEnabled(@params.GetBooleanParameter(CoreConnectionPNames
				.StaleConnectionCheck, true)).SetConnectTimeout(@params.GetIntParameter(CoreConnectionPNames
				.ConnectionTimeout, 0)).SetExpectContinueEnabled(@params.GetBooleanParameter(CoreProtocolPNames
				.UseExpectContinue, false)).SetProxy((HttpHost)@params.GetParameter(ConnRoutePNames
				.DefaultProxy)).SetLocalAddress((IPAddress)@params.GetParameter(ConnRoutePNames.
				LocalAddress)).SetProxyPreferredAuthSchemes((ICollection<string>)@params.GetParameter
				(AuthPNames.ProxyAuthPref)).SetTargetPreferredAuthSchemes((ICollection<string>)@params
				.GetParameter(AuthPNames.TargetAuthPref)).SetAuthenticationEnabled(@params.GetBooleanParameter
				(ClientPNames.HandleAuthentication, true)).SetCircularRedirectsAllowed(@params.GetBooleanParameter
				(ClientPNames.AllowCircularRedirects, false)).SetConnectionRequestTimeout((int)@params
				.GetLongParameter(ClientPNames.ConnManagerTimeout, 0)).SetCookieSpec((string)@params
				.GetParameter(ClientPNames.CookiePolicy)).SetMaxRedirects(@params.GetIntParameter
				(ClientPNames.MaxRedirects, 50)).SetRedirectsEnabled(@params.GetBooleanParameter
				(ClientPNames.HandleRedirects, true)).SetRelativeRedirectsAllowed(!@params.GetBooleanParameter
				(ClientPNames.RejectRelativeRedirect, false)).Build();
		}
	}
}
