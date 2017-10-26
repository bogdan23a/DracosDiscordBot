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

using Sharpen;

namespace Apache.Http.Conn.Util
{
	/// <summary>A collection of utilities relating to InetAddresses.</summary>
	/// <remarks>A collection of utilities relating to InetAddresses.</remarks>
	/// <since>4.0</since>
	public class InetAddressUtils
	{
		private InetAddressUtils()
		{
		}

		private const string Ipv4BasicPatternString = "(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}"
			 + "([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])";

		private static readonly Sharpen.Pattern Ipv4Pattern = Sharpen.Pattern.Compile("^"
			 + Ipv4BasicPatternString + "$");

		private static readonly Sharpen.Pattern Ipv4MappedIpv6Pattern = Sharpen.Pattern.Compile
			("^::[fF]{4}:" + Ipv4BasicPatternString + "$");

		private static readonly Sharpen.Pattern Ipv6StdPattern = Sharpen.Pattern.Compile(
			"^[0-9a-fA-F]{1,4}(:[0-9a-fA-F]{1,4}){7}$");

		private static readonly Sharpen.Pattern Ipv6HexCompressedPattern = Sharpen.Pattern
			.Compile("^(([0-9A-Fa-f]{1,4}(:[0-9A-Fa-f]{1,4}){0,5})?)" + "::" + "(([0-9A-Fa-f]{1,4}(:[0-9A-Fa-f]{1,4}){0,5})?)$"
			);

		private const char ColonChar = ':';

		private const int MaxColonCount = 7;

		// initial 3 fields, 0-255 followed by .
		// final field, 0-255
		// TODO does not allow for redundant leading zeros
		// 0-6 hex fields
		// 0-6 hex fields
		// Must not have more than 7 colons (i.e. 8 fields)
		/// <summary>Checks whether the parameter is a valid IPv4 address</summary>
		/// <param name="input">the address string to check for validity</param>
		/// <returns>true if the input parameter is a valid IPv4 address</returns>
		public static bool IsIPv4Address(string input)
		{
			return Ipv4Pattern.Matcher(input).Matches();
		}

		public static bool IsIPv4MappedIPv64Address(string input)
		{
			return Ipv4MappedIpv6Pattern.Matcher(input).Matches();
		}

		/// <summary>Checks whether the parameter is a valid standard (non-compressed) IPv6 address
		/// 	</summary>
		/// <param name="input">the address string to check for validity</param>
		/// <returns>true if the input parameter is a valid standard (non-compressed) IPv6 address
		/// 	</returns>
		public static bool IsIPv6StdAddress(string input)
		{
			return Ipv6StdPattern.Matcher(input).Matches();
		}

		/// <summary>Checks whether the parameter is a valid compressed IPv6 address</summary>
		/// <param name="input">the address string to check for validity</param>
		/// <returns>true if the input parameter is a valid compressed IPv6 address</returns>
		public static bool IsIPv6HexCompressedAddress(string input)
		{
			int colonCount = 0;
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == ColonChar)
				{
					colonCount++;
				}
			}
			return colonCount <= MaxColonCount && Ipv6HexCompressedPattern.Matcher(input).Matches
				();
		}

		/// <summary>Checks whether the parameter is a valid IPv6 address (including compressed).
		/// 	</summary>
		/// <remarks>Checks whether the parameter is a valid IPv6 address (including compressed).
		/// 	</remarks>
		/// <param name="input">the address string to check for validity</param>
		/// <returns>true if the input parameter is a valid standard or compressed IPv6 address
		/// 	</returns>
		public static bool IsIPv6Address(string input)
		{
			return IsIPv6StdAddress(input) || IsIPv6HexCompressedAddress(input);
		}
	}
}
