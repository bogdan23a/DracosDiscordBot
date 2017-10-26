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
using Apache.Http;
using Sharpen;

namespace Apache.Http.Client
{
	/// <summary>
	/// Signals failure to retry the request due to non-repeatable request
	/// entity.
	/// </summary>
	/// <remarks>
	/// Signals failure to retry the request due to non-repeatable request
	/// entity.
	/// </remarks>
	/// <since>4.0</since>
	[System.Serializable]
	public class NonRepeatableRequestException : ProtocolException
	{
		private const long serialVersionUID = 82685265288806048L;

		/// <summary>Creates a new NonRepeatableEntityException with a <tt>null</tt> detail message.
		/// 	</summary>
		/// <remarks>Creates a new NonRepeatableEntityException with a <tt>null</tt> detail message.
		/// 	</remarks>
		public NonRepeatableRequestException() : base()
		{
		}

		/// <summary>Creates a new NonRepeatableEntityException with the specified detail message.
		/// 	</summary>
		/// <remarks>Creates a new NonRepeatableEntityException with the specified detail message.
		/// 	</remarks>
		/// <param name="message">The exception detail message</param>
		public NonRepeatableRequestException(string message) : base(message)
		{
		}

		/// <summary>Creates a new NonRepeatableEntityException with the specified detail message.
		/// 	</summary>
		/// <remarks>Creates a new NonRepeatableEntityException with the specified detail message.
		/// 	</remarks>
		/// <param name="message">The exception detail message</param>
		/// <param name="cause">the cause</param>
		public NonRepeatableRequestException(string message, Exception cause) : base(message
			, cause)
		{
		}
	}
}
