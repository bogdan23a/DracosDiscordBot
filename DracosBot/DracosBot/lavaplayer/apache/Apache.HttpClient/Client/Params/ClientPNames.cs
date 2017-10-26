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

using Apache.Http.Client.Params;
using Sharpen;

namespace Apache.Http.Client.Params
{
	/// <summary>Parameter names for HTTP client parameters.</summary>
	/// <remarks>Parameter names for HTTP client parameters.</remarks>
	/// <since>4.0</since>
	[System.ObsoleteAttribute(@"(4.3) use Apache.Http.Client.Config.RequestConfig .")]
	public abstract class ClientPNames
	{
		public const string ConnectionManagerFactoryClassName = "http.connection-manager.factory-class-name";

		/// <summary>
		/// Defines whether redirects should be handled automatically
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="bool">bool</see>
		/// .
		/// </p>
		/// </summary>
		public const string HandleRedirects = "http.protocol.handle-redirects";

		/// <summary>Defines whether relative redirects should be rejected.</summary>
		/// <remarks>
		/// Defines whether relative redirects should be rejected. HTTP specification
		/// requires the location value be an absolute URI.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="bool">bool</see>
		/// .
		/// </p>
		/// </remarks>
		public const string RejectRelativeRedirect = "http.protocol.reject-relative-redirect";

		/// <summary>Defines the maximum number of redirects to be followed.</summary>
		/// <remarks>
		/// Defines the maximum number of redirects to be followed.
		/// The limit on number of redirects is intended to prevent infinite loops.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="int">int</see>
		/// .
		/// </p>
		/// </remarks>
		public const string MaxRedirects = "http.protocol.max-redirects";

		/// <summary>Defines whether circular redirects (redirects to the same location) should be allowed.
		/// 	</summary>
		/// <remarks>
		/// Defines whether circular redirects (redirects to the same location) should be allowed.
		/// The HTTP spec is not sufficiently clear whether circular redirects are permitted,
		/// therefore optionally they can be enabled
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="bool">bool</see>
		/// .
		/// </p>
		/// </remarks>
		public const string AllowCircularRedirects = "http.protocol.allow-circular-redirects";

		/// <summary>Defines whether authentication should be handled automatically.</summary>
		/// <remarks>
		/// Defines whether authentication should be handled automatically.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="bool">bool</see>
		/// .
		/// </p>
		/// </remarks>
		public const string HandleAuthentication = "http.protocol.handle-authentication";

		/// <summary>Defines the name of the cookie specification to be used for HTTP state management.
		/// 	</summary>
		/// <remarks>
		/// Defines the name of the cookie specification to be used for HTTP state management.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="string">string</see>
		/// .
		/// </p>
		/// </remarks>
		public const string CookiePolicy = "http.protocol.cookie-policy";

		/// <summary>
		/// Defines the virtual host to be used in the <code>Host</code>
		/// request header instead of the physical host.
		/// </summary>
		/// <remarks>
		/// Defines the virtual host to be used in the <code>Host</code>
		/// request header instead of the physical host.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="Apache.Http.HttpHost">Apache.Http.HttpHost</see>
		/// .
		/// </p>
		/// If a port is not provided, it will be derived from the request URL.
		/// </remarks>
		public const string VirtualHost = "http.virtual-host";

		/// <summary>Defines the request headers to be sent per default with each request.</summary>
		/// <remarks>
		/// Defines the request headers to be sent per default with each request.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="System.Collections.ICollection{E}">System.Collections.ICollection&lt;E&gt;
		/// 	</see>
		/// . The
		/// collection is expected to contain
		/// <see cref="Apache.Http.Header">Apache.Http.Header</see>
		/// s.
		/// </p>
		/// </remarks>
		public const string DefaultHeaders = "http.default-headers";

		/// <summary>Defines the default host.</summary>
		/// <remarks>
		/// Defines the default host. The default value will be used if the target host is
		/// not explicitly specified in the request URI.
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="Apache.Http.HttpHost">Apache.Http.HttpHost</see>
		/// .
		/// </p>
		/// </remarks>
		public const string DefaultHost = "http.default-host";

		/// <summary>
		/// Defines the timeout in milliseconds used when retrieving an instance of
		/// <see cref="Apache.Http.Conn.ManagedClientConnection">Apache.Http.Conn.ManagedClientConnection
		/// 	</see>
		/// from the
		/// <see cref="Apache.Http.Conn.ClientConnectionManager">Apache.Http.Conn.ClientConnectionManager
		/// 	</see>
		/// .
		/// <p>
		/// This parameter expects a value of type
		/// <see cref="long">long</see>
		/// .
		/// <p>
		/// </summary>
		/// <since>4.2</since>
		public const string ConnManagerTimeout = "http.conn-manager.timeout";
	}
}
