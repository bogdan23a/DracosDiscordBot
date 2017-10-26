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
using System.Text;
using Apache.Http;
using Apache.Http.Client.Utils;
using Apache.Http.Entity;
using Apache.Http.Message;
using Apache.Http.Protocol;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Client.Utils
{
	/// <summary>A collection of utilities for encoding URLs.</summary>
	/// <remarks>A collection of utilities for encoding URLs.</remarks>
	/// <since>4.0</since>
	public class URLEncodedUtils
	{
		/// <summary>The default HTML form content type.</summary>
		/// <remarks>The default HTML form content type.</remarks>
		public const string ContentType = "application/x-www-form-urlencoded";

		private const char QpSepA = '&';

		private const char QpSepS = ';';

		private const string NameValueSeparator = "=";

		/// <summary>
		/// Returns a list of
		/// <see cref="Apache.Http.NameValuePair">NameValuePairs</see>
		/// as built from the URI's query portion. For example, a URI
		/// of http://example.org/path/to/file?a=1&b=2&c=3 would return a list of three NameValuePairs, one for a=1, one for
		/// b=2, and one for c=3. By convention,
		/// <code>'&'</code>
		/// and
		/// <code>';'</code>
		/// are accepted as parameter separators.
		/// <p>
		/// This is typically useful while parsing an HTTP PUT.
		/// This API is currently only used for testing.
		/// </summary>
		/// <param name="uri">URI to parse</param>
		/// <param name="charset">Charset name to use while parsing the query</param>
		/// <returns>
		/// a list of
		/// <see cref="Apache.Http.NameValuePair">Apache.Http.NameValuePair</see>
		/// as built from the URI's query portion.
		/// </returns>
		public static IList<NameValuePair> Parse(URI uri, string charset)
		{
			string query = uri.GetRawQuery();
			if (query != null && query.Length > 0)
			{
				IList<NameValuePair> result = new AList<NameValuePair>();
				Scanner scanner = new Scanner(query);
				Parse(result, scanner, QpSepPattern, charset);
				return result;
			}
			return Sharpen.Collections.EmptyList();
		}

		/// <summary>
		/// Returns a list of
		/// <see cref="Apache.Http.NameValuePair">NameValuePairs</see>
		/// as parsed from an
		/// <see cref="Apache.Http.HttpEntity">Apache.Http.HttpEntity</see>
		/// . The encoding is
		/// taken from the entity's Content-Encoding header.
		/// <p>
		/// This is typically used while parsing an HTTP POST.
		/// </summary>
		/// <param name="entity">The entity to parse</param>
		/// <returns>
		/// a list of
		/// <see cref="Apache.Http.NameValuePair">Apache.Http.NameValuePair</see>
		/// as built from the URI's query portion.
		/// </returns>
		/// <exception cref="System.IO.IOException">If there was an exception getting the entity's data.
		/// 	</exception>
		public static IList<NameValuePair> Parse(HttpEntity entity)
		{
			ContentType contentType = ContentType.Get(entity);
			if (contentType != null && Sharpen.Runtime.EqualsIgnoreCase(contentType.GetMimeType
				(), ContentType))
			{
				string content = EntityUtils.ToString(entity, Consts.Ascii);
				if (content != null && content.Length > 0)
				{
					Encoding charset = contentType.GetCharset();
					if (charset == null)
					{
						charset = HTTP.DefContentCharset;
					}
					return Parse(content, charset, QpSeps);
				}
			}
			return Sharpen.Collections.EmptyList();
		}

		/// <summary>
		/// Returns true if the entity's Content-Type header is
		/// <code>application/x-www-form-urlencoded</code>.
		/// </summary>
		/// <remarks>
		/// Returns true if the entity's Content-Type header is
		/// <code>application/x-www-form-urlencoded</code>.
		/// </remarks>
		public static bool IsEncoded(HttpEntity entity)
		{
			Header h = entity.GetContentType();
			if (h != null)
			{
				HeaderElement[] elems = h.GetElements();
				if (elems.Length > 0)
				{
					string contentType = elems[0].GetName();
					return Sharpen.Runtime.EqualsIgnoreCase(contentType, ContentType);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Adds all parameters within the Scanner to the list of <code>parameters</code>, as encoded by
		/// <code>encoding</code>.
		/// </summary>
		/// <remarks>
		/// Adds all parameters within the Scanner to the list of <code>parameters</code>, as encoded by
		/// <code>encoding</code>. For example, a scanner containing the string <code>a=1&b=2&c=3</code> would add the
		/// <see cref="Apache.Http.NameValuePair">NameValuePairs</see>
		/// a=1, b=2, and c=3 to the list of parameters. By convention,
		/// <code>'&'</code>
		/// and
		/// <code>';'</code>
		/// are accepted as parameter separators.
		/// </remarks>
		/// <param name="parameters">List to add parameters to.</param>
		/// <param name="scanner">Input that contains the parameters to parse.</param>
		/// <param name="charset">Encoding to use when decoding the parameters.</param>
		public static void Parse(IList<NameValuePair> parameters, Scanner scanner, string
			 charset)
		{
			Parse(parameters, scanner, QpSepPattern, charset);
		}

		/// <summary>
		/// Adds all parameters within the Scanner to the list of
		/// <code>parameters</code>, as encoded by <code>encoding</code>.
		/// </summary>
		/// <remarks>
		/// Adds all parameters within the Scanner to the list of
		/// <code>parameters</code>, as encoded by <code>encoding</code>. For
		/// example, a scanner containing the string <code>a=1&b=2&c=3</code> would
		/// add the
		/// <see cref="Apache.Http.NameValuePair">NameValuePairs</see>
		/// a=1, b=2, and c=3 to the
		/// list of parameters.
		/// </remarks>
		/// <param name="parameters">List to add parameters to.</param>
		/// <param name="scanner">Input that contains the parameters to parse.</param>
		/// <param name="parameterSepartorPattern">
		/// The Pattern string for parameter separators, by convention
		/// <code>"[&;]"</code>
		/// </param>
		/// <param name="charset">Encoding to use when decoding the parameters.</param>
		public static void Parse(IList<NameValuePair> parameters, Scanner scanner, string
			 parameterSepartorPattern, string charset)
		{
			scanner.UseDelimiter(parameterSepartorPattern);
			while (scanner.HasNext())
			{
				string name = null;
				string value = null;
				string token = scanner.Next();
				int i = token.IndexOf(NameValueSeparator);
				if (i != -1)
				{
					name = DecodeFormFields(Sharpen.Runtime.Substring(token, 0, i).Trim(), charset);
					value = DecodeFormFields(Sharpen.Runtime.Substring(token, i + 1).Trim(), charset);
				}
				else
				{
					name = DecodeFormFields(token.Trim(), charset);
				}
				parameters.AddItem(new BasicNameValuePair(name, value));
			}
		}

		/// <summary>Query parameter separators.</summary>
		/// <remarks>Query parameter separators.</remarks>
		private static readonly char[] QpSeps = new char[] { QpSepA, QpSepS };

		/// <summary>Query parameter separator pattern.</summary>
		/// <remarks>Query parameter separator pattern.</remarks>
		private static readonly string QpSepPattern = "[" + new string(QpSeps) + "]";

		/// <summary>
		/// Returns a list of
		/// <see cref="Apache.Http.NameValuePair">NameValuePairs</see>
		/// as parsed from the given string using the given character
		/// encoding. By convention,
		/// <code>'&'</code>
		/// and
		/// <code>';'</code>
		/// are accepted as parameter separators.
		/// </summary>
		/// <param name="s">text to parse.</param>
		/// <param name="charset">Encoding to use when decoding the parameters.</param>
		/// <returns>
		/// a list of
		/// <see cref="Apache.Http.NameValuePair">Apache.Http.NameValuePair</see>
		/// as built from the URI's query portion.
		/// </returns>
		/// <since>4.2</since>
		public static IList<NameValuePair> Parse(string s, Encoding charset)
		{
			return Parse(s, charset, QpSeps);
		}

		/// <summary>
		/// Returns a list of
		/// <see cref="Apache.Http.NameValuePair">NameValuePairs</see>
		/// as parsed from the given string using the given character
		/// encoding.
		/// </summary>
		/// <param name="s">text to parse.</param>
		/// <param name="charset">Encoding to use when decoding the parameters.</param>
		/// <param name="parameterSeparator">
		/// The characters used to separate parameters, by convention,
		/// <code>'&'</code>
		/// and
		/// <code>';'</code>
		/// .
		/// </param>
		/// <returns>
		/// a list of
		/// <see cref="Apache.Http.NameValuePair">Apache.Http.NameValuePair</see>
		/// as built from the URI's query portion.
		/// </returns>
		/// <since>4.3</since>
		public static IList<NameValuePair> Parse(string s, Encoding charset, params char[]
			 parameterSeparator)
		{
			if (s == null)
			{
				return Sharpen.Collections.EmptyList();
			}
			BasicHeaderValueParser parser = BasicHeaderValueParser.Instance;
			CharArrayBuffer buffer = new CharArrayBuffer(s.Length);
			buffer.Append(s);
			ParserCursor cursor = new ParserCursor(0, buffer.Length());
			IList<NameValuePair> list = new AList<NameValuePair>();
			while (!cursor.AtEnd())
			{
				NameValuePair nvp = parser.ParseNameValuePair(buffer, cursor, parameterSeparator);
				if (nvp.GetName().Length > 0)
				{
					list.AddItem(new BasicNameValuePair(DecodeFormFields(nvp.GetName(), charset), DecodeFormFields
						(nvp.GetValue(), charset)));
				}
			}
			return list;
		}

		/// <summary>
		/// Returns a String that is suitable for use as an
		/// <code>application/x-www-form-urlencoded</code>
		/// list of parameters in an HTTP PUT or HTTP POST.
		/// </summary>
		/// <param name="parameters">The parameters to include.</param>
		/// <param name="charset">The encoding to use.</param>
		/// <returns>
		/// An
		/// <code>application/x-www-form-urlencoded</code>
		/// string
		/// </returns>
		public static string Format<_T0>(IList<_T0> parameters, string charset) where _T0:
			NameValuePair
		{
			return Format(parameters, QpSepA, charset);
		}

		/// <summary>
		/// Returns a String that is suitable for use as an
		/// <code>application/x-www-form-urlencoded</code>
		/// list of parameters in an HTTP PUT or HTTP POST.
		/// </summary>
		/// <param name="parameters">The parameters to include.</param>
		/// <param name="parameterSeparator">
		/// The parameter separator, by convention,
		/// <code>'&'</code>
		/// or
		/// <code>';'</code>
		/// .
		/// </param>
		/// <param name="charset">The encoding to use.</param>
		/// <returns>
		/// An
		/// <code>application/x-www-form-urlencoded</code>
		/// string
		/// </returns>
		/// <since>4.3</since>
		public static string Format<_T0>(IList<_T0> parameters, char parameterSeparator, 
			string charset) where _T0:NameValuePair
		{
			StringBuilder result = new StringBuilder();
			foreach (NameValuePair parameter in parameters)
			{
				string encodedName = EncodeFormFields(parameter.GetName(), charset);
				string encodedValue = EncodeFormFields(parameter.GetValue(), charset);
				if (result.Length > 0)
				{
					result.Append(parameterSeparator);
				}
				result.Append(encodedName);
				if (encodedValue != null)
				{
					result.Append(NameValueSeparator);
					result.Append(encodedValue);
				}
			}
			return result.ToString();
		}

		/// <summary>
		/// Returns a String that is suitable for use as an
		/// <code>application/x-www-form-urlencoded</code>
		/// list of parameters in an HTTP PUT or HTTP POST.
		/// </summary>
		/// <param name="parameters">The parameters to include.</param>
		/// <param name="charset">The encoding to use.</param>
		/// <returns>
		/// An
		/// <code>application/x-www-form-urlencoded</code>
		/// string
		/// </returns>
		/// <since>4.2</since>
		public static string Format<_T0>(IEnumerable<_T0> parameters, Encoding charset) where 
			_T0:NameValuePair
		{
			return Format(parameters, QpSepA, charset);
		}

		/// <summary>
		/// Returns a String that is suitable for use as an
		/// <code>application/x-www-form-urlencoded</code>
		/// list of parameters in an HTTP PUT or HTTP POST.
		/// </summary>
		/// <param name="parameters">The parameters to include.</param>
		/// <param name="parameterSeparator">
		/// The parameter separator, by convention,
		/// <code>'&'</code>
		/// or
		/// <code>';'</code>
		/// .
		/// </param>
		/// <param name="charset">The encoding to use.</param>
		/// <returns>
		/// An
		/// <code>application/x-www-form-urlencoded</code>
		/// string
		/// </returns>
		/// <since>4.3</since>
		public static string Format<_T0>(IEnumerable<_T0> parameters, char parameterSeparator
			, Encoding charset) where _T0:NameValuePair
		{
			StringBuilder result = new StringBuilder();
			foreach (NameValuePair parameter in parameters)
			{
				string encodedName = EncodeFormFields(parameter.GetName(), charset);
				string encodedValue = EncodeFormFields(parameter.GetValue(), charset);
				if (result.Length > 0)
				{
					result.Append(parameterSeparator);
				}
				result.Append(encodedName);
				if (encodedValue != null)
				{
					result.Append(NameValueSeparator);
					result.Append(encodedValue);
				}
			}
			return result.ToString();
		}

		/// <summary>Unreserved characters, i.e.</summary>
		/// <remarks>
		/// Unreserved characters, i.e. alphanumeric, plus:
		/// <code>_ - ! . ~ ' ( ) *</code>
		/// <p>
		/// This list is the same as the
		/// <code>unreserved</code>
		/// list in
		/// <a href="http://www.ietf.org/rfc/rfc2396.txt">RFC 2396</a>
		/// </remarks>
		private static readonly BitSet Unreserved = new BitSet(256);

		/// <summary>
		/// Punctuation characters: , ; : $ & + =
		/// <p>
		/// These are the additional characters allowed by userinfo.
		/// </summary>
		/// <remarks>
		/// Punctuation characters: , ; : $ & + =
		/// <p>
		/// These are the additional characters allowed by userinfo.
		/// </remarks>
		private static readonly BitSet Punct = new BitSet(256);

		/// <summary>
		/// Characters which are safe to use in userinfo,
		/// i.e.
		/// </summary>
		/// <remarks>
		/// Characters which are safe to use in userinfo,
		/// i.e.
		/// <see cref="Unreserved">Unreserved</see>
		/// plus
		/// <see cref="Punct">Punct</see>
		/// uation
		/// </remarks>
		private static readonly BitSet Userinfo = new BitSet(256);

		/// <summary>
		/// Characters which are safe to use in a path,
		/// i.e.
		/// </summary>
		/// <remarks>
		/// Characters which are safe to use in a path,
		/// i.e.
		/// <see cref="Unreserved">Unreserved</see>
		/// plus
		/// <see cref="Punct">Punct</see>
		/// uation plus / @
		/// </remarks>
		private static readonly BitSet Pathsafe = new BitSet(256);

		/// <summary>
		/// Characters which are safe to use in a query or a fragment,
		/// i.e.
		/// </summary>
		/// <remarks>
		/// Characters which are safe to use in a query or a fragment,
		/// i.e.
		/// <see cref="Reserved">Reserved</see>
		/// plus
		/// <see cref="Unreserved">Unreserved</see>
		/// 
		/// </remarks>
		private static readonly BitSet Uric = new BitSet(256);

		/// <summary>Reserved characters, i.e.</summary>
		/// <remarks>
		/// Reserved characters, i.e.
		/// <code>;/?:@&=+$,[]</code>
		/// <p>
		/// This list is the same as the
		/// <code>reserved</code>
		/// list in
		/// <a href="http://www.ietf.org/rfc/rfc2396.txt">RFC 2396</a>
		/// as augmented by
		/// <a href="http://www.ietf.org/rfc/rfc2732.txt">RFC 2732</a>
		/// </remarks>
		private static readonly BitSet Reserved = new BitSet(256);

		/// <summary>
		/// Safe characters for x-www-form-urlencoded data, as per java.net.URLEncoder and browser behaviour,
		/// i.e.
		/// </summary>
		/// <remarks>
		/// Safe characters for x-www-form-urlencoded data, as per java.net.URLEncoder and browser behaviour,
		/// i.e. alphanumeric plus
		/// <code>"-", "_", ".", "*"</code>
		/// </remarks>
		private static readonly BitSet Urlencoder = new BitSet(256);

		static URLEncodedUtils()
		{
			// unreserved chars
			// alpha characters
			for (int i = 'a'; i <= 'z'; i++)
			{
				Unreserved.Set(i);
			}
			for (int i_1 = 'A'; i_1 <= 'Z'; i_1++)
			{
				Unreserved.Set(i_1);
			}
			// numeric characters
			for (int i_2 = '0'; i_2 <= '9'; i_2++)
			{
				Unreserved.Set(i_2);
			}
			Unreserved.Set('_');
			// these are the charactes of the "mark" list
			Unreserved.Set('-');
			Unreserved.Set('.');
			Unreserved.Set('*');
			Urlencoder.Or(Unreserved);
			// skip remaining unreserved characters
			Unreserved.Set('!');
			Unreserved.Set('~');
			Unreserved.Set('\'');
			Unreserved.Set('(');
			Unreserved.Set(')');
			// punct chars
			Punct.Set(',');
			Punct.Set(';');
			Punct.Set(':');
			Punct.Set('$');
			Punct.Set('&');
			Punct.Set('+');
			Punct.Set('=');
			// Safe for userinfo
			Userinfo.Or(Unreserved);
			Userinfo.Or(Punct);
			// URL path safe
			Pathsafe.Or(Unreserved);
			Pathsafe.Set('/');
			// segment separator
			Pathsafe.Set(';');
			// param separator
			Pathsafe.Set(':');
			// rest as per list in 2396, i.e. : @ & = + $ ,
			Pathsafe.Set('@');
			Pathsafe.Set('&');
			Pathsafe.Set('=');
			Pathsafe.Set('+');
			Pathsafe.Set('$');
			Pathsafe.Set(',');
			Reserved.Set(';');
			Reserved.Set('/');
			Reserved.Set('?');
			Reserved.Set(':');
			Reserved.Set('@');
			Reserved.Set('&');
			Reserved.Set('=');
			Reserved.Set('+');
			Reserved.Set('$');
			Reserved.Set(',');
			Reserved.Set('[');
			// added by RFC 2732
			Reserved.Set(']');
			// added by RFC 2732
			Uric.Or(Reserved);
			Uric.Or(Unreserved);
		}

		private const int Radix = 16;

		private static string UrlEncode(string content, Encoding charset, BitSet safechars
			, bool blankAsPlus)
		{
			if (content == null)
			{
				return null;
			}
			StringBuilder buf = new StringBuilder();
			ByteBuffer bb = charset.Encode(content);
			while (bb.HasRemaining())
			{
				int b = bb.Get() & unchecked((int)(0xff));
				if (safechars.Get(b))
				{
					buf.Append((char)b);
				}
				else
				{
					if (blankAsPlus && b == ' ')
					{
						buf.Append('+');
					}
					else
					{
						buf.Append("%");
						char hex1 = System.Char.ToUpper(char.ForDigit((b >> 4) & unchecked((int)(0xF)), Radix
							));
						char hex2 = System.Char.ToUpper(char.ForDigit(b & unchecked((int)(0xF)), Radix));
						buf.Append(hex1);
						buf.Append(hex2);
					}
				}
			}
			return buf.ToString();
		}

		/// <summary>
		/// Decode/unescape a portion of a URL, to use with the query part ensure
		/// <code>plusAsBlank</code>
		/// is true.
		/// </summary>
		/// <param name="content">the portion to decode</param>
		/// <param name="charset">the charset to use</param>
		/// <param name="plusAsBlank">
		/// if
		/// <code>true</code>
		/// , then convert '+' to space (e.g. for www-url-form-encoded content), otherwise leave as is.
		/// </param>
		/// <returns>encoded string</returns>
		private static string UrlDecode(string content, Encoding charset, bool plusAsBlank
			)
		{
			if (content == null)
			{
				return null;
			}
			ByteBuffer bb = ByteBuffer.Allocate(content.Length);
			CharBuffer cb = CharBuffer.Wrap(content);
			while (cb.HasRemaining())
			{
				char c = cb.Get();
				if (c == '%' && cb.Remaining() >= 2)
				{
					char uc = cb.Get();
					char lc = cb.Get();
					int u = char.Digit(uc, 16);
					int l = char.Digit(lc, 16);
					if (u != -1 && l != -1)
					{
						bb.Put(unchecked((byte)((u << 4) + l)));
					}
					else
					{
						bb.Put(unchecked((byte)'%'));
						bb.Put(unchecked((byte)uc));
						bb.Put(unchecked((byte)lc));
					}
				}
				else
				{
					if (plusAsBlank && c == '+')
					{
						bb.Put(unchecked((byte)' '));
					}
					else
					{
						bb.Put(unchecked((byte)c));
					}
				}
			}
			bb.Flip();
			return charset.Decode(bb).ToString();
		}

		/// <summary>Decode/unescape www-url-form-encoded content.</summary>
		/// <remarks>Decode/unescape www-url-form-encoded content.</remarks>
		/// <param name="content">the content to decode, will decode '+' as space</param>
		/// <param name="charset">the charset to use</param>
		/// <returns>encoded string</returns>
		private static string DecodeFormFields(string content, string charset)
		{
			if (content == null)
			{
				return null;
			}
			return UrlDecode(content, charset != null ? Sharpen.Extensions.GetEncoding(charset
				) : Consts.Utf8, true);
		}

		/// <summary>Decode/unescape www-url-form-encoded content.</summary>
		/// <remarks>Decode/unescape www-url-form-encoded content.</remarks>
		/// <param name="content">the content to decode, will decode '+' as space</param>
		/// <param name="charset">the charset to use</param>
		/// <returns>encoded string</returns>
		private static string DecodeFormFields(string content, Encoding charset)
		{
			if (content == null)
			{
				return null;
			}
			return UrlDecode(content, charset != null ? charset : Consts.Utf8, true);
		}

		/// <summary>Encode/escape www-url-form-encoded content.</summary>
		/// <remarks>
		/// Encode/escape www-url-form-encoded content.
		/// <p>
		/// Uses the
		/// <see cref="Urlencoder">Urlencoder</see>
		/// set of characters, rather than
		/// the
		/// <see cref="#UNRSERVED">#UNRSERVED</see>
		/// set; this is for compatibilty with previous
		/// releases, URLEncoder.encode() and most browsers.
		/// </remarks>
		/// <param name="content">the content to encode, will convert space to '+'</param>
		/// <param name="charset">the charset to use</param>
		/// <returns>encoded string</returns>
		private static string EncodeFormFields(string content, string charset)
		{
			if (content == null)
			{
				return null;
			}
			return UrlEncode(content, charset != null ? Sharpen.Extensions.GetEncoding(charset
				) : Consts.Utf8, Urlencoder, true);
		}

		/// <summary>Encode/escape www-url-form-encoded content.</summary>
		/// <remarks>
		/// Encode/escape www-url-form-encoded content.
		/// <p>
		/// Uses the
		/// <see cref="Urlencoder">Urlencoder</see>
		/// set of characters, rather than
		/// the
		/// <see cref="#UNRSERVED">#UNRSERVED</see>
		/// set; this is for compatibilty with previous
		/// releases, URLEncoder.encode() and most browsers.
		/// </remarks>
		/// <param name="content">the content to encode, will convert space to '+'</param>
		/// <param name="charset">the charset to use</param>
		/// <returns>encoded string</returns>
		private static string EncodeFormFields(string content, Encoding charset)
		{
			if (content == null)
			{
				return null;
			}
			return UrlEncode(content, charset != null ? charset : Consts.Utf8, Urlencoder, true
				);
		}

		/// <summary>
		/// Encode a String using the
		/// <see cref="Userinfo">Userinfo</see>
		/// set of characters.
		/// <p>
		/// Used by URIBuilder to encode the userinfo segment.
		/// </summary>
		/// <param name="content">the string to encode, does not convert space to '+'</param>
		/// <param name="charset">the charset to use</param>
		/// <returns>the encoded string</returns>
		internal static string EncUserInfo(string content, Encoding charset)
		{
			return UrlEncode(content, charset, Userinfo, false);
		}

		/// <summary>
		/// Encode a String using the
		/// <see cref="Uric">Uric</see>
		/// set of characters.
		/// <p>
		/// Used by URIBuilder to encode the query and fragment segments.
		/// </summary>
		/// <param name="content">the string to encode, does not convert space to '+'</param>
		/// <param name="charset">the charset to use</param>
		/// <returns>the encoded string</returns>
		internal static string EncUric(string content, Encoding charset)
		{
			return UrlEncode(content, charset, Uric, false);
		}

		/// <summary>
		/// Encode a String using the
		/// <see cref="Pathsafe">Pathsafe</see>
		/// set of characters.
		/// <p>
		/// Used by URIBuilder to encode path segments.
		/// </summary>
		/// <param name="content">the string to encode, does not convert space to '+'</param>
		/// <param name="charset">the charset to use</param>
		/// <returns>the encoded string</returns>
		internal static string EncPath(string content, Encoding charset)
		{
			return UrlEncode(content, charset, Pathsafe, false);
		}
	}
}
