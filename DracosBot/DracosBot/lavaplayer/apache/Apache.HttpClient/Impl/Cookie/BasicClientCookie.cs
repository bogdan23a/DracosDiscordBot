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
using System.Text;
using Apache.Http.Cookie;
using Apache.Http.Util;
using Sharpen;

namespace Apache.Http.Impl.Cookie
{
	/// <summary>
	/// Default implementation of
	/// <see cref="Apache.Http.Cookie.SetCookie">Apache.Http.Cookie.SetCookie</see>
	/// .
	/// </summary>
	/// <since>4.0</since>
	[System.Serializable]
	public class BasicClientCookie : SetCookie, ClientCookie, ICloneable
	{
		private const long serialVersionUID = -3869795591041535538L;

		/// <summary>Default Constructor taking a name and a value.</summary>
		/// <remarks>Default Constructor taking a name and a value. The value may be null.</remarks>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		public BasicClientCookie(string name, string value) : base()
		{
			Args.NotNull(name, "Name");
			this.name = name;
			this.attribs = new Dictionary<string, string>();
			this.value = value;
		}

		/// <summary>Returns the name.</summary>
		/// <remarks>Returns the name.</remarks>
		/// <returns>String name The name</returns>
		public virtual string GetName()
		{
			return this.name;
		}

		/// <summary>Returns the value.</summary>
		/// <remarks>Returns the value.</remarks>
		/// <returns>String value The current value.</returns>
		public virtual string GetValue()
		{
			return this.value;
		}

		/// <summary>Sets the value</summary>
		/// <param name="value"></param>
		public virtual void SetValue(string value)
		{
			this.value = value;
		}

		/// <summary>
		/// Returns the comment describing the purpose of this cookie, or
		/// <tt>null</tt> if no such comment has been defined.
		/// </summary>
		/// <remarks>
		/// Returns the comment describing the purpose of this cookie, or
		/// <tt>null</tt> if no such comment has been defined.
		/// </remarks>
		/// <returns>comment</returns>
		/// <seealso cref="SetComment(string)">SetComment(string)</seealso>
		public virtual string GetComment()
		{
			return cookieComment;
		}

		/// <summary>
		/// If a user agent (web browser) presents this cookie to a user, the
		/// cookie's purpose will be described using this comment.
		/// </summary>
		/// <remarks>
		/// If a user agent (web browser) presents this cookie to a user, the
		/// cookie's purpose will be described using this comment.
		/// </remarks>
		/// <param name="comment"></param>
		/// <seealso cref="GetComment()">GetComment()</seealso>
		public virtual void SetComment(string comment)
		{
			cookieComment = comment;
		}

		/// <summary>Returns null.</summary>
		/// <remarks>Returns null. Cookies prior to RFC2965 do not set this attribute</remarks>
		public virtual string GetCommentURL()
		{
			return null;
		}

		/// <summary>
		/// Returns the expiration
		/// <see cref="System.DateTime">System.DateTime</see>
		/// of the cookie, or <tt>null</tt>
		/// if none exists.
		/// <p><strong>Note:</strong> the object returned by this method is
		/// considered immutable. Changing it (e.g. using setTime()) could result
		/// in undefined behaviour. Do so at your peril. </p>
		/// </summary>
		/// <returns>
		/// Expiration
		/// <see cref="System.DateTime">System.DateTime</see>
		/// , or <tt>null</tt>.
		/// </returns>
		/// <seealso cref="SetExpiryDate(System.DateTime)">SetExpiryDate(System.DateTime)</seealso>
		public virtual DateTime GetExpiryDate()
		{
			return cookieExpiryDate;
		}

		/// <summary>Sets expiration date.</summary>
		/// <remarks>
		/// Sets expiration date.
		/// <p><strong>Note:</strong> the object returned by this method is considered
		/// immutable. Changing it (e.g. using setTime()) could result in undefined
		/// behaviour. Do so at your peril.</p>
		/// </remarks>
		/// <param name="expiryDate">
		/// the
		/// <see cref="System.DateTime">System.DateTime</see>
		/// after which this cookie is no longer valid.
		/// </param>
		/// <seealso cref="GetExpiryDate()">GetExpiryDate()</seealso>
		public virtual void SetExpiryDate(DateTime expiryDate)
		{
			cookieExpiryDate = expiryDate;
		}

		/// <summary>
		/// Returns <tt>false</tt> if the cookie should be discarded at the end
		/// of the "session"; <tt>true</tt> otherwise.
		/// </summary>
		/// <remarks>
		/// Returns <tt>false</tt> if the cookie should be discarded at the end
		/// of the "session"; <tt>true</tt> otherwise.
		/// </remarks>
		/// <returns>
		/// <tt>false</tt> if the cookie should be discarded at the end
		/// of the "session"; <tt>true</tt> otherwise
		/// </returns>
		public virtual bool IsPersistent()
		{
			return (null != cookieExpiryDate);
		}

		/// <summary>Returns domain attribute of the cookie.</summary>
		/// <remarks>Returns domain attribute of the cookie.</remarks>
		/// <returns>the value of the domain attribute</returns>
		/// <seealso cref="SetDomain(string)">SetDomain(string)</seealso>
		public virtual string GetDomain()
		{
			return cookieDomain;
		}

		/// <summary>Sets the domain attribute.</summary>
		/// <remarks>Sets the domain attribute.</remarks>
		/// <param name="domain">The value of the domain attribute</param>
		/// <seealso cref="GetDomain()">GetDomain()</seealso>
		public virtual void SetDomain(string domain)
		{
			if (domain != null)
			{
				cookieDomain = domain.ToLower(Sharpen.Extensions.GetEnglishCulture());
			}
			else
			{
				cookieDomain = null;
			}
		}

		/// <summary>Returns the path attribute of the cookie</summary>
		/// <returns>The value of the path attribute.</returns>
		/// <seealso cref="SetPath(string)">SetPath(string)</seealso>
		public virtual string GetPath()
		{
			return cookiePath;
		}

		/// <summary>Sets the path attribute.</summary>
		/// <remarks>Sets the path attribute.</remarks>
		/// <param name="path">The value of the path attribute</param>
		/// <seealso cref="GetPath()">GetPath()</seealso>
		public virtual void SetPath(string path)
		{
			cookiePath = path;
		}

		/// <returns><code>true</code> if this cookie should only be sent over secure connections.
		/// 	</returns>
		/// <seealso cref="SetSecure(bool)">SetSecure(bool)</seealso>
		public virtual bool IsSecure()
		{
			return isSecure;
		}

		/// <summary>Sets the secure attribute of the cookie.</summary>
		/// <remarks>
		/// Sets the secure attribute of the cookie.
		/// <p>
		/// When <tt>true</tt> the cookie should only be sent
		/// using a secure protocol (https).  This should only be set when
		/// the cookie's originating server used a secure protocol to set the
		/// cookie's value.
		/// </remarks>
		/// <param name="secure">The value of the secure attribute</param>
		/// <seealso cref="IsSecure()">IsSecure()</seealso>
		public virtual void SetSecure(bool secure)
		{
			isSecure = secure;
		}

		/// <summary>Returns null.</summary>
		/// <remarks>Returns null. Cookies prior to RFC2965 do not set this attribute</remarks>
		public virtual int[] GetPorts()
		{
			return null;
		}

		/// <summary>
		/// Returns the version of the cookie specification to which this
		/// cookie conforms.
		/// </summary>
		/// <remarks>
		/// Returns the version of the cookie specification to which this
		/// cookie conforms.
		/// </remarks>
		/// <returns>the version of the cookie.</returns>
		/// <seealso cref="SetVersion(int)">SetVersion(int)</seealso>
		public virtual int GetVersion()
		{
			return cookieVersion;
		}

		/// <summary>
		/// Sets the version of the cookie specification to which this
		/// cookie conforms.
		/// </summary>
		/// <remarks>
		/// Sets the version of the cookie specification to which this
		/// cookie conforms.
		/// </remarks>
		/// <param name="version">the version of the cookie.</param>
		/// <seealso cref="GetVersion()">GetVersion()</seealso>
		public virtual void SetVersion(int version)
		{
			cookieVersion = version;
		}

		/// <summary>Returns true if this cookie has expired.</summary>
		/// <remarks>Returns true if this cookie has expired.</remarks>
		/// <param name="date">Current time</param>
		/// <returns><tt>true</tt> if the cookie has expired.</returns>
		public virtual bool IsExpired(DateTime date)
		{
			Args.NotNull(date, "Date");
			return (cookieExpiryDate != null && cookieExpiryDate.GetTime() <= date.GetTime());
		}

		public virtual void SetAttribute(string name, string value)
		{
			this.attribs.Put(name, value);
		}

		public virtual string GetAttribute(string name)
		{
			return this.attribs.Get(name);
		}

		public virtual bool ContainsAttribute(string name)
		{
			return this.attribs.Get(name) != null;
		}

		/// <exception cref="Sharpen.CloneNotSupportedException"></exception>
		public virtual object Clone()
		{
			Apache.Http.Impl.Cookie.BasicClientCookie clone = (Apache.Http.Impl.Cookie.BasicClientCookie
				)base.Clone();
			clone.attribs = new Dictionary<string, string>(this.attribs);
			return clone;
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append("[version: ");
			buffer.Append(Sharpen.Extensions.ToString(this.cookieVersion));
			buffer.Append("]");
			buffer.Append("[name: ");
			buffer.Append(this.name);
			buffer.Append("]");
			buffer.Append("[value: ");
			buffer.Append(this.value);
			buffer.Append("]");
			buffer.Append("[domain: ");
			buffer.Append(this.cookieDomain);
			buffer.Append("]");
			buffer.Append("[path: ");
			buffer.Append(this.cookiePath);
			buffer.Append("]");
			buffer.Append("[expiry: ");
			buffer.Append(this.cookieExpiryDate);
			buffer.Append("]");
			return buffer.ToString();
		}

		/// <summary>Cookie name</summary>
		private readonly string name;

		/// <summary>Cookie attributes as specified by the origin server</summary>
		private IDictionary<string, string> attribs;

		/// <summary>Cookie value</summary>
		private string value;

		/// <summary>Comment attribute.</summary>
		/// <remarks>Comment attribute.</remarks>
		private string cookieComment;

		/// <summary>Domain attribute.</summary>
		/// <remarks>Domain attribute.</remarks>
		private string cookieDomain;

		/// <summary>
		/// Expiration
		/// <see cref="System.DateTime">System.DateTime</see>
		/// .
		/// </summary>
		private DateTime cookieExpiryDate;

		/// <summary>Path attribute.</summary>
		/// <remarks>Path attribute.</remarks>
		private string cookiePath;

		/// <summary>My secure flag.</summary>
		/// <remarks>My secure flag.</remarks>
		private bool isSecure;

		/// <summary>The version of the cookie specification I was created from.</summary>
		/// <remarks>The version of the cookie specification I was created from.</remarks>
		private int cookieVersion;
		// ----------------------------------------------------- Instance Variables
	}
}
