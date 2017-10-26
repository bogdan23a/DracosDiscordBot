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
using Sharpen;

namespace Apache.Http.Client.Utils
{
	/// <summary>A collection of utilities to workaround limitations of Java clone framework.
	/// 	</summary>
	/// <remarks>A collection of utilities to workaround limitations of Java clone framework.
	/// 	</remarks>
	/// <since>4.0</since>
	public class CloneUtils
	{
		/// <since>4.3</since>
		/// <exception cref="Sharpen.CloneNotSupportedException"></exception>
		public static T CloneObject<T>(T obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is ICloneable)
			{
				Type clazz = obj.GetType();
				MethodInfo m;
				try
				{
					m = clazz.GetMethod("clone", (Type[])null);
				}
				catch (NoSuchMethodException ex)
				{
					throw new MissingMethodException(ex.Message);
				}
				try
				{
					T result = (T)m.Invoke(obj, (object[])null);
					// OK because clone() preserves the class
					return result;
				}
				catch (TargetInvocationException ex)
				{
					Exception cause = ex.InnerException;
					if (cause is CloneNotSupportedException)
					{
						throw ((CloneNotSupportedException)cause);
					}
					else
					{
						throw new Error("Unexpected exception", cause);
					}
				}
				catch (MemberAccessException ex)
				{
					throw new IllegalAccessError(ex.Message);
				}
			}
			else
			{
				throw new CloneNotSupportedException();
			}
		}

		/// <exception cref="Sharpen.CloneNotSupportedException"></exception>
		public static object Clone(object obj)
		{
			return CloneObject(obj);
		}

		/// <summary>This class should not be instantiated.</summary>
		/// <remarks>This class should not be instantiated.</remarks>
		private CloneUtils()
		{
		}
	}
}
