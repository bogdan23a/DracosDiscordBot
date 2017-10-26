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
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>An exception to indicate an error parsing a date string.</summary>
	/// <remarks>An exception to indicate an error parsing a date string.</remarks>
	/// <seealso cref="DateUtils">DateUtils</seealso>
	/// <since>4.0</since>
	[System.Serializable]
	public class DateParseException : Exception
	{
		private const long serialVersionUID = 4417696455000643370L;

		public DateParseException() : base()
		{
		}

		/// <param name="message">the exception message</param>
		public DateParseException(string message) : base(message)
		{
		}
	}
}
