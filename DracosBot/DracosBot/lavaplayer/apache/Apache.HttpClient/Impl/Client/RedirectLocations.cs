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
using Sharpen;

namespace Apache.Http.Impl.Client
{
	/// <summary>
	/// This class represents a collection of
	/// <see cref="Sharpen.URI">Sharpen.URI</see>
	/// s used
	/// as redirect locations.
	/// </summary>
	/// <since>4.0</since>
	public class RedirectLocations : AbstractList<object>
	{
		private readonly ICollection<URI> unique;

		private readonly IList<URI> all;

		public RedirectLocations() : base()
		{
			// HashSet/ArrayList are not synch.
			this.unique = new HashSet<URI>();
			this.all = new AList<URI>();
		}

		/// <summary>Test if the URI is present in the collection.</summary>
		/// <remarks>Test if the URI is present in the collection.</remarks>
		public virtual bool Contains(URI uri)
		{
			return this.unique.Contains(uri);
		}

		/// <summary>Adds a new URI to the collection.</summary>
		/// <remarks>Adds a new URI to the collection.</remarks>
		public virtual void Add(URI uri)
		{
			this.unique.AddItem(uri);
			this.all.AddItem(uri);
		}

		/// <summary>Removes a URI from the collection.</summary>
		/// <remarks>Removes a URI from the collection.</remarks>
		public virtual bool Remove(URI uri)
		{
			bool removed = this.unique.Remove(uri);
			if (removed)
			{
				IEnumerator<URI> it = this.all.GetEnumerator();
				while (it.HasNext())
				{
					URI current = it.Next();
					if (current.Equals(uri))
					{
						it.Remove();
					}
				}
			}
			return removed;
		}

		/// <summary>
		/// Returns all redirect
		/// <see cref="Sharpen.URI">Sharpen.URI</see>
		/// s in the order they were added to the collection.
		/// </summary>
		/// <returns>list of all URIs</returns>
		/// <since>4.1</since>
		public virtual IList<URI> GetAll()
		{
			return new AList<URI>(this.all);
		}

		/// <summary>Returns the URI at the specified position in this list.</summary>
		/// <remarks>Returns the URI at the specified position in this list.</remarks>
		/// <param name="index">index of the location to return</param>
		/// <returns>the URI at the specified position in this list</returns>
		/// <exception cref="System.IndexOutOfRangeException">
		/// if the index is out of range (
		/// <tt>index &lt; 0 || index &gt;= size()</tt>)
		/// </exception>
		/// <since>4.3</since>
		public override object Get(int index)
		{
			return this.all[index];
		}

		/// <summary>Returns the number of elements in this list.</summary>
		/// <remarks>
		/// Returns the number of elements in this list. If this list contains more
		/// than <tt>Integer.MAX_VALUE</tt> elements, returns
		/// <tt>Integer.MAX_VALUE</tt>.
		/// </remarks>
		/// <returns>the number of elements in this list</returns>
		/// <since>4.3</since>
		public override int Count
		{
			get
			{
				return this.all.Count;
			}
		}

		/// <summary>
		/// Replaces the URI at the specified position in this list with the
		/// specified element (must be a URI).
		/// </summary>
		/// <remarks>
		/// Replaces the URI at the specified position in this list with the
		/// specified element (must be a URI).
		/// </remarks>
		/// <param name="index">index of the element to replace</param>
		/// <param name="element">URI to be stored at the specified position</param>
		/// <returns>the URI previously at the specified position</returns>
		/// <exception cref="System.NotSupportedException">if the <tt>set</tt> operation is not supported by this list
		/// 	</exception>
		/// <exception cref="System.InvalidCastException">
		/// if the element is not a
		/// <see cref="Sharpen.URI">Sharpen.URI</see>
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// if the specified element is null and this list does not
		/// permit null elements
		/// </exception>
		/// <exception cref="System.IndexOutOfRangeException">
		/// if the index is out of range (
		/// <tt>index &lt; 0 || index &gt;= size()</tt>)
		/// </exception>
		/// <since>4.3</since>
		public override object Set(int index, object element)
		{
			URI removed = this.all.Set(index, (URI)element);
			this.unique.Remove(removed);
			this.unique.AddItem((URI)element);
			if (this.all.Count != this.unique.Count)
			{
				Sharpen.Collections.AddAll(this.unique, this.all);
			}
			return removed;
		}

		/// <summary>
		/// Inserts the specified element at the specified position in this list
		/// (must be a URI).
		/// </summary>
		/// <remarks>
		/// Inserts the specified element at the specified position in this list
		/// (must be a URI). Shifts the URI currently at that position (if any) and
		/// any subsequent URIs to the right (adds one to their indices).
		/// </remarks>
		/// <param name="index">index at which the specified element is to be inserted</param>
		/// <param name="element">URI to be inserted</param>
		/// <exception cref="System.NotSupportedException">if the <tt>add</tt> operation is not supported by this list
		/// 	</exception>
		/// <exception cref="System.InvalidCastException">
		/// if the element is not a
		/// <see cref="Sharpen.URI">Sharpen.URI</see>
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// if the specified element is null and this list does not
		/// permit null elements
		/// </exception>
		/// <exception cref="System.IndexOutOfRangeException">
		/// if the index is out of range (
		/// <tt>index &lt; 0 || index &gt; size()</tt>)
		/// </exception>
		/// <since>4.3</since>
		public override void Add(int index, object element)
		{
			this.all.Add(index, (URI)element);
			this.unique.AddItem((URI)element);
		}

		/// <summary>Removes the URI at the specified position in this list.</summary>
		/// <remarks>
		/// Removes the URI at the specified position in this list. Shifts any
		/// subsequent URIs to the left (subtracts one from their indices). Returns
		/// the URI that was removed from the list.
		/// </remarks>
		/// <param name="index">the index of the URI to be removed</param>
		/// <returns>the URI previously at the specified position</returns>
		/// <exception cref="System.IndexOutOfRangeException">
		/// if the index is out of range (
		/// <tt>index &lt; 0 || index &gt;= size()</tt>)
		/// </exception>
		/// <since>4.3</since>
		public override object Remove(int index)
		{
			URI removed = this.all.Remove(index);
			this.unique.Remove(removed);
			if (this.all.Count != this.unique.Count)
			{
				Sharpen.Collections.AddAll(this.unique, this.all);
			}
			return removed;
		}

		/// <summary>Returns <tt>true</tt> if this collection contains the specified element.
		/// 	</summary>
		/// <remarks>
		/// Returns <tt>true</tt> if this collection contains the specified element.
		/// More formally, returns <tt>true</tt> if and only if this collection
		/// contains at least one element <tt>e</tt> such that
		/// <tt>(o==null&nbsp;?&nbsp;e==null&nbsp;:&nbsp;o.equals(e))</tt>.
		/// </remarks>
		/// <param name="o">element whose presence in this collection is to be tested</param>
		/// <returns>
		/// <tt>true</tt> if this collection contains the specified
		/// element
		/// </returns>
		public override bool Contains(object o)
		{
			return this.unique.Contains(o);
		}
	}
}
