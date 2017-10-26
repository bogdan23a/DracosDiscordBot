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
using System.Net.Sockets;
using Apache.Http.Conn.Ssl;
using Sharpen;

namespace Apache.Http.Conn.Ssl
{
	/// <summary>A strategy allowing for a choice of an alias during SSL authentication.</summary>
	/// <remarks>A strategy allowing for a choice of an alias during SSL authentication.</remarks>
	/// <since>4.3</since>
	public interface PrivateKeyStrategy
	{
		/// <summary>Determines what key material to use for SSL authentication.</summary>
		/// <remarks>Determines what key material to use for SSL authentication.</remarks>
		string ChooseAlias(IDictionary<string, PrivateKeyDetails> aliases, Socket socket);
	}
}
