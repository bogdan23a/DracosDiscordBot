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

using System.Text;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Cookie
{
	/// <summary>
	/// CookieOrigin class encapsulates details of an origin server that
	/// are relevant when parsing, validating or matching HTTP cookies.
	/// </summary>
	/// <remarks>
	/// CookieOrigin class encapsulates details of an origin server that
	/// are relevant when parsing, validating or matching HTTP cookies.
	/// </remarks>
	/// <since>4.0</since>
	public sealed class CookieOrigin
	{
		private readonly string host;

		private readonly int port;

		private readonly string path;

		private readonly bool secure;

		public CookieOrigin(string host, int port, string path, bool secure) : base()
		{
			Args.NotBlank(host, "Host");
			Args.NotNegative(port, "Port");
			Args.NotNull(path, "Path");
			this.host = host.ToLower(Sharpen.Extensions.GetEnglishCulture());
			this.port = port;
			if (path.Trim().Length != 0)
			{
				this.path = path;
			}
			else
			{
				this.path = "/";
			}
			this.secure = secure;
		}

		public string GetHost()
		{
			return this.host;
		}

		public string GetPath()
		{
			return this.path;
		}

		public int GetPort()
		{
			return this.port;
		}

		public bool IsSecure()
		{
			return this.secure;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append('[');
			if (this.secure)
			{
				buffer.Append("(secure)");
			}
			buffer.Append(this.host);
			buffer.Append(':');
			buffer.Append(Sharpen.Extensions.ToString(this.port));
			buffer.Append(this.path);
			buffer.Append(']');
			return buffer.ToString();
		}
	}
}
