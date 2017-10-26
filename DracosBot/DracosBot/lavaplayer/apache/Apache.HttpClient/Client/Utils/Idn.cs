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

using Apache.Http.Client.Utils;
using Sharpen;

namespace Apache.Http.Client.Utils
{
	/// <summary>Abstraction of international domain name (IDN) conversion.</summary>
	/// <remarks>Abstraction of international domain name (IDN) conversion.</remarks>
	/// <since>4.0</since>
	public interface Idn
	{
		/// <summary>Converts a name from its punycode representation to Unicode.</summary>
		/// <remarks>
		/// Converts a name from its punycode representation to Unicode.
		/// The name may be a single hostname or a dot-separated qualified domain name.
		/// </remarks>
		/// <param name="punycode">the Punycode representation</param>
		/// <returns>the Unicode domain name</returns>
		string ToUnicode(string punycode);
	}
}
