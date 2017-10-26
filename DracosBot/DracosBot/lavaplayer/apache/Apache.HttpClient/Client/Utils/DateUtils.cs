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
using System.Collections.Generic;
using System.Globalization;
using Apache.Http.Client.Utils;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Client.Utils
{
	/// <summary>
	/// A utility class for parsing and formatting HTTP dates as used in cookies and
	/// other headers.
	/// </summary>
	/// <remarks>
	/// A utility class for parsing and formatting HTTP dates as used in cookies and
	/// other headers.  This class handles dates as defined by RFC 2616 section
	/// 3.3.1 as well as some other common non-standard formats.
	/// </remarks>
	/// <since>4.3</since>
	public sealed class DateUtils
	{
		/// <summary>Date format pattern used to parse HTTP date headers in RFC 1123 format.</summary>
		/// <remarks>Date format pattern used to parse HTTP date headers in RFC 1123 format.</remarks>
		public const string PatternRfc1123 = "EEE, dd MMM yyyy HH:mm:ss zzz";

		/// <summary>Date format pattern used to parse HTTP date headers in RFC 1036 format.</summary>
		/// <remarks>Date format pattern used to parse HTTP date headers in RFC 1036 format.</remarks>
		public const string PatternRfc1036 = "EEE, dd-MMM-yy HH:mm:ss zzz";

		/// <summary>
		/// Date format pattern used to parse HTTP date headers in ANSI C
		/// <code>asctime()</code> format.
		/// </summary>
		/// <remarks>
		/// Date format pattern used to parse HTTP date headers in ANSI C
		/// <code>asctime()</code> format.
		/// </remarks>
		public const string PatternAsctime = "EEE MMM d HH:mm:ss yyyy";

		private static readonly string[] DefaultPatterns = new string[] { PatternRfc1123, 
			PatternRfc1036, PatternAsctime };

		private static readonly DateTime DefaultTwoDigitYearStart;

		public static readonly TimeZoneInfo Gmt = Sharpen.Extensions.GetTimeZone("GMT");

		static DateUtils()
		{
			Calendar calendar = Calendar.GetInstance();
			calendar.SetTimeZone(Gmt);
			calendar.Set(2000, Calendar.January, 1, 0, 0, 0);
			calendar.Set(Calendar.Millisecond, 0);
			DefaultTwoDigitYearStart = calendar.GetTime();
		}

		/// <summary>Parses a date value.</summary>
		/// <remarks>
		/// Parses a date value.  The formats used for parsing the date value are retrieved from
		/// the default http params.
		/// </remarks>
		/// <param name="dateValue">the date value to parse</param>
		/// <returns>the parsed date or null if input could not be parsed</returns>
		public static DateTime ParseDate(string dateValue)
		{
			return ParseDate(dateValue, null, null);
		}

		/// <summary>Parses the date value using the given date formats.</summary>
		/// <remarks>Parses the date value using the given date formats.</remarks>
		/// <param name="dateValue">the date value to parse</param>
		/// <param name="dateFormats">the date formats to use</param>
		/// <returns>the parsed date or null if input could not be parsed</returns>
		public static DateTime ParseDate(string dateValue, string[] dateFormats)
		{
			return ParseDate(dateValue, dateFormats, null);
		}

		/// <summary>Parses the date value using the given date formats.</summary>
		/// <remarks>Parses the date value using the given date formats.</remarks>
		/// <param name="dateValue">the date value to parse</param>
		/// <param name="dateFormats">the date formats to use</param>
		/// <param name="startDate">
		/// During parsing, two digit years will be placed in the range
		/// <code>startDate</code> to <code>startDate + 100 years</code>. This value may
		/// be <code>null</code>. When <code>null</code> is given as a parameter, year
		/// <code>2000</code> will be used.
		/// </param>
		/// <returns>the parsed date or null if input could not be parsed</returns>
		public static DateTime ParseDate(string dateValue, string[] dateFormats, DateTime
			 startDate)
		{
			Args.NotNull(dateValue, "Date value");
			string[] localDateFormats = dateFormats != null ? dateFormats : DefaultPatterns;
			DateTime localStartDate = startDate != null ? startDate : DefaultTwoDigitYearStart;
			string v = dateValue;
			// trim single quotes around date if present
			// see issue #5279
			if (v.Length > 1 && v.StartsWith("'") && v.EndsWith("'"))
			{
				v = Sharpen.Runtime.Substring(v, 1, v.Length - 1);
			}
			foreach (string dateFormat in localDateFormats)
			{
				SimpleDateFormat dateParser = DateUtils.DateFormatHolder.FormatFor(dateFormat);
				dateParser.Set2DigitYearStart(localStartDate);
				ParsePosition pos = new ParsePosition(0);
				DateTime result = dateParser.Parse(v, pos);
				if (pos.GetIndex() != 0)
				{
					return result;
				}
			}
			return null;
		}

		/// <summary>Formats the given date according to the RFC 1123 pattern.</summary>
		/// <remarks>Formats the given date according to the RFC 1123 pattern.</remarks>
		/// <param name="date">The date to format.</param>
		/// <returns>An RFC 1123 formatted date string.</returns>
		/// <seealso cref="PatternRfc1123">PatternRfc1123</seealso>
		public static string FormatDate(DateTime date)
		{
			return FormatDate(date, PatternRfc1123);
		}

		/// <summary>Formats the given date according to the specified pattern.</summary>
		/// <remarks>
		/// Formats the given date according to the specified pattern.  The pattern
		/// must conform to that used by the
		/// <see cref="Sharpen.SimpleDateFormat">
		/// simple date
		/// format
		/// </see>
		/// class.
		/// </remarks>
		/// <param name="date">The date to format.</param>
		/// <param name="pattern">The pattern to use for formatting the date.</param>
		/// <returns>A formatted date string.</returns>
		/// <exception cref="System.ArgumentException">If the given date pattern is invalid.</exception>
		/// <seealso cref="Sharpen.SimpleDateFormat">Sharpen.SimpleDateFormat</seealso>
		public static string FormatDate(DateTime date, string pattern)
		{
			Args.NotNull(date, "Date");
			Args.NotNull(pattern, "Pattern");
			SimpleDateFormat formatter = DateUtils.DateFormatHolder.FormatFor(pattern);
			return formatter.Format(date);
		}

		/// <summary>
		/// Clears thread-local variable containing
		/// <see cref="Sharpen.DateFormat">Sharpen.DateFormat</see>
		/// cache.
		/// </summary>
		/// <since>4.3</since>
		public static void ClearThreadLocal()
		{
			DateUtils.DateFormatHolder.ClearThreadLocal();
		}

		/// <summary>This class should not be instantiated.</summary>
		/// <remarks>This class should not be instantiated.</remarks>
		private DateUtils()
		{
		}

		/// <summary>
		/// A factory for
		/// <see cref="Sharpen.SimpleDateFormat">Sharpen.SimpleDateFormat</see>
		/// s. The instances are stored in a
		/// threadlocal way because SimpleDateFormat is not threadsafe as noted in
		/// <see cref="Sharpen.SimpleDateFormat">its javadoc</see>
		/// .
		/// </summary>
		internal sealed class DateFormatHolder
		{
			private sealed class _ThreadLocal_203 : ThreadLocal<SoftReference<IDictionary<string
				, SimpleDateFormat>>>
			{
				public _ThreadLocal_203()
				{
				}

				protected override SoftReference<IDictionary<string, SimpleDateFormat>> InitialValue
					()
				{
					return new SoftReference<IDictionary<string, SimpleDateFormat>>(new Dictionary<string
						, SimpleDateFormat>());
				}
			}

			private static readonly ThreadLocal<SoftReference<IDictionary<string, SimpleDateFormat
				>>> ThreadlocalFormats = new _ThreadLocal_203();

			/// <summary>
			/// creates a
			/// <see cref="Sharpen.SimpleDateFormat">Sharpen.SimpleDateFormat</see>
			/// for the requested format string.
			/// </summary>
			/// <param name="pattern">
			/// a non-<code>null</code> format String according to
			/// <see cref="Sharpen.SimpleDateFormat">Sharpen.SimpleDateFormat</see>
			/// . The format is not checked against
			/// <code>null</code> since all paths go through
			/// <see cref="DateUtils">DateUtils</see>
			/// .
			/// </param>
			/// <returns>
			/// the requested format. This simple dateformat should not be used
			/// to
			/// <see cref="Sharpen.SimpleDateFormat.ApplyPattern(string)">apply</see>
			/// to a
			/// different pattern.
			/// </returns>
			public static SimpleDateFormat FormatFor(string pattern)
			{
				SoftReference<IDictionary<string, SimpleDateFormat>> @ref = ThreadlocalFormats.Get
					();
				IDictionary<string, SimpleDateFormat> formats = @ref.Get();
				if (formats == null)
				{
					formats = new Dictionary<string, SimpleDateFormat>();
					ThreadlocalFormats.Set(new SoftReference<IDictionary<string, SimpleDateFormat>>(formats
						));
				}
				SimpleDateFormat format = formats.Get(pattern);
				if (format == null)
				{
					format = new SimpleDateFormat(pattern, CultureInfo.InvariantCulture);
					format.SetTimeZone(Sharpen.Extensions.GetTimeZone("GMT"));
					formats.Put(pattern, format);
				}
				return format;
			}

			public static void ClearThreadLocal()
			{
				ThreadlocalFormats.Remove();
			}
		}
	}
}
