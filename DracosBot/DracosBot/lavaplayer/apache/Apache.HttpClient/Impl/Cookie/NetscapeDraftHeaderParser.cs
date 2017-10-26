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
using Apache.Http;
using Apache.Http.Message;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <since>4.0</since>
	public class NetscapeDraftHeaderParser
	{
		public static readonly Apache.Http.Impl.Cookie.NetscapeDraftHeaderParser Default = 
			new Apache.Http.Impl.Cookie.NetscapeDraftHeaderParser();

		public NetscapeDraftHeaderParser() : base()
		{
		}

		/// <exception cref="Apache.Http.ParseException"></exception>
		public virtual HeaderElement ParseHeader(CharArrayBuffer buffer, ParserCursor cursor
			)
		{
			Args.NotNull(buffer, "Char array buffer");
			Args.NotNull(cursor, "Parser cursor");
			NameValuePair nvp = ParseNameValuePair(buffer, cursor);
			IList<NameValuePair> @params = new AList<NameValuePair>();
			while (!cursor.AtEnd())
			{
				NameValuePair param = ParseNameValuePair(buffer, cursor);
				@params.AddItem(param);
			}
			return new BasicHeaderElement(nvp.GetName(), nvp.GetValue(), Sharpen.Collections.ToArray
				(@params, new NameValuePair[@params.Count]));
		}

		private NameValuePair ParseNameValuePair(CharArrayBuffer buffer, ParserCursor cursor
			)
		{
			bool terminated = false;
			int pos = cursor.GetPos();
			int indexFrom = cursor.GetPos();
			int indexTo = cursor.GetUpperBound();
			// Find name
			string name = null;
			while (pos < indexTo)
			{
				char ch = buffer.CharAt(pos);
				if (ch == '=')
				{
					break;
				}
				if (ch == ';')
				{
					terminated = true;
					break;
				}
				pos++;
			}
			if (pos == indexTo)
			{
				terminated = true;
				name = buffer.SubstringTrimmed(indexFrom, indexTo);
			}
			else
			{
				name = buffer.SubstringTrimmed(indexFrom, pos);
				pos++;
			}
			if (terminated)
			{
				cursor.UpdatePos(pos);
				return new BasicNameValuePair(name, null);
			}
			// Find value
			string value = null;
			int i1 = pos;
			while (pos < indexTo)
			{
				char ch = buffer.CharAt(pos);
				if (ch == ';')
				{
					terminated = true;
					break;
				}
				pos++;
			}
			int i2 = pos;
			// Trim leading white spaces
			while (i1 < i2 && (HTTP.IsWhitespace(buffer.CharAt(i1))))
			{
				i1++;
			}
			// Trim trailing white spaces
			while ((i2 > i1) && (HTTP.IsWhitespace(buffer.CharAt(i2 - 1))))
			{
				i2--;
			}
			value = buffer.Substring(i1, i2);
			if (terminated)
			{
				pos++;
			}
			cursor.UpdatePos(pos);
			return new BasicNameValuePair(name, value);
		}
	}
}
