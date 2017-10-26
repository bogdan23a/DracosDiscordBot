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
using System.Reflection;
using System.Security;
using Apache.Http.Client.Utils;
using Sharpen;

namespace Apache.Http.Client.Utils
{
	/// <summary>Uses the java.net.IDN class through reflection.</summary>
	/// <remarks>Uses the java.net.IDN class through reflection.</remarks>
	/// <since>4.0</since>
	public class JdkIdn : Idn
	{
		private readonly MethodInfo toUnicode;

		/// <exception cref="System.TypeLoadException">if java.net.IDN is not available</exception>
		public JdkIdn()
		{
			Type clazz = Sharpen.Runtime.GetType("java.net.IDN");
			try
			{
				toUnicode = clazz.GetMethod("toUnicode", typeof(string));
			}
			catch (SecurityException e)
			{
				// doesn't happen
				throw new InvalidOperationException(e.Message, e);
			}
			catch (NoSuchMethodException e)
			{
				// doesn't happen
				throw new InvalidOperationException(e.Message, e);
			}
		}

		public virtual string ToUnicode(string punycode)
		{
			try
			{
				return (string)toUnicode.Invoke(null, punycode);
			}
			catch (MemberAccessException e)
			{
				throw new InvalidOperationException(e.Message, e);
			}
			catch (TargetInvocationException e)
			{
				Exception t = e.InnerException;
				throw new RuntimeException(t.Message, t);
			}
		}
	}
}
