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
using System.IO;
using System.Text;
using Apache.Http.Impl.Cookie;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>
	/// Parses the list from <a href="http://publicsuffix.org/">publicsuffix.org</a>
	/// and configures a PublicSuffixFilter.
	/// </summary>
	/// <remarks>
	/// Parses the list from <a href="http://publicsuffix.org/">publicsuffix.org</a>
	/// and configures a PublicSuffixFilter.
	/// </remarks>
	/// <since>4.0</since>
	public class PublicSuffixListParser
	{
		private const int MaxLineLen = 256;

		private readonly PublicSuffixFilter filter;

		internal PublicSuffixListParser(PublicSuffixFilter filter)
		{
			this.filter = filter;
		}

		/// <summary>Parses the public suffix list format.</summary>
		/// <remarks>
		/// Parses the public suffix list format.
		/// When creating the reader from the file, make sure to
		/// use the correct encoding (the original list is in UTF-8).
		/// </remarks>
		/// <param name="list">the suffix list. The caller is responsible for closing the reader.
		/// 	</param>
		/// <exception cref="System.IO.IOException">on error while reading from list</exception>
		public virtual void Parse(StreamReader list)
		{
			ICollection<string> rules = new AList<string>();
			ICollection<string> exceptions = new AList<string>();
			BufferedReader r = new BufferedReader(list);
			StringBuilder sb = new StringBuilder(256);
			bool more = true;
			while (more)
			{
				more = ReadLine(r, sb);
				string line = sb.ToString();
				if (line.Length == 0)
				{
					continue;
				}
				if (line.StartsWith("//"))
				{
					continue;
				}
				//entire lines can also be commented using //
				if (line.StartsWith("."))
				{
					line = Sharpen.Runtime.Substring(line, 1);
				}
				// A leading dot is optional
				// An exclamation mark (!) at the start of a rule marks an exception to a previous wildcard rule
				bool isException = line.StartsWith("!");
				if (isException)
				{
					line = Sharpen.Runtime.Substring(line, 1);
				}
				if (isException)
				{
					exceptions.AddItem(line);
				}
				else
				{
					rules.AddItem(line);
				}
			}
			filter.SetPublicSuffixes(rules);
			filter.SetExceptions(exceptions);
		}

		/// <param name="r"></param>
		/// <param name="sb"></param>
		/// <returns>false when the end of the stream is reached</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		private bool ReadLine(StreamReader r, StringBuilder sb)
		{
			sb.Length = 0;
			int b;
			bool hitWhitespace = false;
			while ((b = r.Read()) != -1)
			{
				char c = (char)b;
				if (c == '\n')
				{
					break;
				}
				// Each line is only read up to the first whitespace
				if (char.IsWhiteSpace(c))
				{
					hitWhitespace = true;
				}
				if (!hitWhitespace)
				{
					sb.Append(c);
				}
				if (sb.Length > MaxLineLen)
				{
					throw new IOException("Line too long");
				}
			}
			// prevent excess memory usage
			return (b != -1);
		}
	}
}
