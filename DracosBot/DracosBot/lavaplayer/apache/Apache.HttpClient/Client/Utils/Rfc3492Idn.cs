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
using System.Text;
using Apache.Http.Client.Utils;
using Sharpen;

namespace Apache.Http.Client.Utils
{
	/// <summary>Implementation from pseudo code in RFC 3492.</summary>
	/// <remarks>Implementation from pseudo code in RFC 3492.</remarks>
	/// <since>4.0</since>
	public class Rfc3492Idn : Idn
	{
		private const int @base = 36;

		private const int tmin = 1;

		private const int tmax = 26;

		private const int skew = 38;

		private const int damp = 700;

		private const int initial_bias = 72;

		private const int initial_n = 128;

		private const char delimiter = '-';

		private const string AcePrefix = "xn--";

		private int Adapt(int delta, int numpoints, bool firsttime)
		{
			int d = delta;
			if (firsttime)
			{
				d = d / damp;
			}
			else
			{
				d = d / 2;
			}
			d = d + (d / numpoints);
			int k = 0;
			while (d > ((@base - tmin) * tmax) / 2)
			{
				d = d / (@base - tmin);
				k = k + @base;
			}
			return k + (((@base - tmin + 1) * d) / (d + skew));
		}

		private int Digit(char c)
		{
			if ((c >= 'A') && (c <= 'Z'))
			{
				return (c - 'A');
			}
			if ((c >= 'a') && (c <= 'z'))
			{
				return (c - 'a');
			}
			if ((c >= '0') && (c <= '9'))
			{
				return (c - '0') + 26;
			}
			throw new ArgumentException("illegal digit: " + c);
		}

		public virtual string ToUnicode(string punycode)
		{
			StringBuilder unicode = new StringBuilder(punycode.Length);
			StringTokenizer tok = new StringTokenizer(punycode, ".");
			while (tok.HasMoreTokens())
			{
				string t = tok.NextToken();
				if (unicode.Length > 0)
				{
					unicode.Append('.');
				}
				if (t.StartsWith(AcePrefix))
				{
					t = Decode(Sharpen.Runtime.Substring(t, 4));
				}
				unicode.Append(t);
			}
			return unicode.ToString();
		}

		protected internal virtual string Decode(string s)
		{
			string input = s;
			int n = initial_n;
			int i = 0;
			int bias = initial_bias;
			StringBuilder output = new StringBuilder(input.Length);
			int lastdelim = input.LastIndexOf(delimiter);
			if (lastdelim != -1)
			{
				output.Append(input.SubSequence(0, lastdelim));
				input = Sharpen.Runtime.Substring(input, lastdelim + 1);
			}
			while (input.Length > 0)
			{
				int oldi = i;
				int w = 1;
				for (int k = @base; ; k += @base)
				{
					if (input.Length == 0)
					{
						break;
					}
					char c = input[0];
					input = Sharpen.Runtime.Substring(input, 1);
					int digit = Digit(c);
					i = i + digit * w;
					// FIXME fail on overflow
					int t;
					if (k <= bias + tmin)
					{
						t = tmin;
					}
					else
					{
						if (k >= bias + tmax)
						{
							t = tmax;
						}
						else
						{
							t = k - bias;
						}
					}
					if (digit < t)
					{
						break;
					}
					w = w * (@base - t);
				}
				// FIXME fail on overflow
				bias = Adapt(i - oldi, output.Length + 1, (oldi == 0));
				n = n + i / (output.Length + 1);
				// FIXME fail on overflow
				i = i % (output.Length + 1);
				// {if n is a basic code point then fail}
				output.Insert(i, (char)n);
				i++;
			}
			return output.ToString();
		}
	}
}
