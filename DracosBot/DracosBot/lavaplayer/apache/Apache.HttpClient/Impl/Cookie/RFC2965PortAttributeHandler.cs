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
using Apache.Http.Cookie;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary><tt>"Port"</tt> cookie attribute handler for RFC 2965 cookie spec.</summary>
	/// <remarks><tt>"Port"</tt> cookie attribute handler for RFC 2965 cookie spec.</remarks>
	/// <since>4.0</since>
	public class RFC2965PortAttributeHandler : CookieAttributeHandler
	{
		public RFC2965PortAttributeHandler() : base()
		{
		}

		/// <summary>Parses the given Port attribute value (e.g.</summary>
		/// <remarks>
		/// Parses the given Port attribute value (e.g. "8000,8001,8002")
		/// into an array of ports.
		/// </remarks>
		/// <param name="portValue">port attribute value</param>
		/// <returns>parsed array of ports</returns>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException">
		/// if there is a problem in
		/// parsing due to invalid portValue.
		/// </exception>
		private static int[] ParsePortAttribute(string portValue)
		{
			StringTokenizer st = new StringTokenizer(portValue, ",");
			int[] ports = new int[st.CountTokens()];
			try
			{
				int i = 0;
				while (st.HasMoreTokens())
				{
					ports[i] = System.Convert.ToInt32(st.NextToken().Trim());
					if (ports[i] < 0)
					{
						throw new MalformedCookieException("Invalid Port attribute.");
					}
					++i;
				}
			}
			catch (FormatException e)
			{
				throw new MalformedCookieException("Invalid Port " + "attribute: " + e.Message);
			}
			return ports;
		}

		/// <summary>
		/// Returns <tt>true</tt> if the given port exists in the given
		/// ports list.
		/// </summary>
		/// <remarks>
		/// Returns <tt>true</tt> if the given port exists in the given
		/// ports list.
		/// </remarks>
		/// <param name="port">port of host where cookie was received from or being sent to.</param>
		/// <param name="ports">port list</param>
		/// <returns>
		/// true returns <tt>true</tt> if the given port exists in
		/// the given ports list; <tt>false</tt> otherwise.
		/// </returns>
		private static bool PortMatch(int port, int[] ports)
		{
			bool portInList = false;
			foreach (int port2 in ports)
			{
				if (port == port2)
				{
					portInList = true;
					break;
				}
			}
			return portInList;
		}

		/// <summary>Parse cookie port attribute.</summary>
		/// <remarks>Parse cookie port attribute.</remarks>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Parse(SetCookie cookie, string portValue)
		{
			Args.NotNull(cookie, "Cookie");
			if (cookie is SetCookie2)
			{
				SetCookie2 cookie2 = (SetCookie2)cookie;
				if (portValue != null && portValue.Trim().Length > 0)
				{
					int[] ports = ParsePortAttribute(portValue);
					cookie2.SetPorts(ports);
				}
			}
		}

		/// <summary>Validate cookie port attribute.</summary>
		/// <remarks>
		/// Validate cookie port attribute. If the Port attribute was specified
		/// in header, the request port must be in cookie's port list.
		/// </remarks>
		/// <exception cref="Apache.Http.Cookie.MalformedCookieException"></exception>
		public virtual void Validate(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin
			)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			int port = origin.GetPort();
			if (cookie is ClientCookie && ((ClientCookie)cookie).ContainsAttribute(ClientCookie
				.PortAttr))
			{
				if (!PortMatch(port, cookie.GetPorts()))
				{
					throw new CookieRestrictionViolationException("Port attribute violates RFC 2965: "
						 + "Request port not found in cookie's port list.");
				}
			}
		}

		/// <summary>Match cookie port attribute.</summary>
		/// <remarks>
		/// Match cookie port attribute. If the Port attribute is not specified
		/// in header, the cookie can be sent to any port. Otherwise, the request port
		/// must be in the cookie's port list.
		/// </remarks>
		public virtual bool Match(Apache.Http.Cookie.Cookie cookie, CookieOrigin origin)
		{
			Args.NotNull(cookie, "Cookie");
			Args.NotNull(origin, "Cookie origin");
			int port = origin.GetPort();
			if (cookie is ClientCookie && ((ClientCookie)cookie).ContainsAttribute(ClientCookie
				.PortAttr))
			{
				if (cookie.GetPorts() == null)
				{
					// Invalid cookie state: port not specified
					return false;
				}
				if (!PortMatch(port, cookie.GetPorts()))
				{
					return false;
				}
			}
			return true;
		}
	}
}
