using System;
using System.Collections.Generic;

namespace System.Collections.Specialized
{
	/// <summary>Implements a hash table with the key and the value strongly typed to be strings rather than objects.</summary>
	[Serializable]
	public class StringDictionary : IEnumerable
	{
		/// <summary>Gets the number of key/value pairs in the <see cref="T:System.Collections.Specialized.StringDictionary" />.</summary>
		/// <returns>The number of key/value pairs in the <see cref="T:System.Collections.Specialized.StringDictionary" />.  
		///  Retrieving the value of this property is an O(1) operation.</returns>
		public virtual int Count
		{
			get
			{
				return this.contents.Count;
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.Specialized.StringDictionary" /> is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Collections.Specialized.StringDictionary" /> is synchronized (thread safe); otherwise, <see langword="false" />.</returns>
		public virtual bool IsSynchronized
		{
			get
			{
				return this.contents.IsSynchronized;
			}
		}

		/// <summary>Gets or sets the value associated with the specified key.</summary>
		/// <param name="key">The key whose value to get or set.</param>
		/// <returns>The value associated with the specified key. If the specified key is not found, Get returns <see langword="null" />, and Set creates a new entry with the specified key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public virtual string this[string key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				return (string)this.contents[key.ToLowerInvariant()];
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				this.contents[key.ToLowerInvariant()] = value;
			}
		}

		/// <summary>Gets a collection of keys in the <see cref="T:System.Collections.Specialized.StringDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> that provides the keys in the <see cref="T:System.Collections.Specialized.StringDictionary" />.</returns>
		public virtual ICollection Keys
		{
			get
			{
				return this.contents.Keys;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.Specialized.StringDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Object" /> that can be used to synchronize access to the <see cref="T:System.Collections.Specialized.StringDictionary" />.</returns>
		public virtual object SyncRoot
		{
			get
			{
				return this.contents.SyncRoot;
			}
		}

		/// <summary>Gets a collection of values in the <see cref="T:System.Collections.Specialized.StringDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> that provides the values in the <see cref="T:System.Collections.Specialized.StringDictionary" />.</returns>
		public virtual ICollection Values
		{
			get
			{
				return this.contents.Values;
			}
		}

		/// <summary>Adds an entry with the specified key and value into the <see cref="T:System.Collections.Specialized.StringDictionary" />.</summary>
		/// <param name="key">The key of the entry to add.</param>
		/// <param name="value">The value of the entry to add. The value can be <see langword="null" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">An entry with the same key already exists in the <see cref="T:System.Collections.Specialized.StringDictionary" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Specialized.StringDictionary" /> is read-only.</exception>
		public virtual void Add(string key, string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.contents.Add(key.ToLowerInvariant(), value);
		}

		/// <summary>Removes all entries from the <see cref="T:System.Collections.Specialized.StringDictionary" />.</summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Specialized.StringDictionary" /> is read-only.</exception>
		public virtual void Clear()
		{
			this.contents.Clear();
		}

		/// <summary>Determines if the <see cref="T:System.Collections.Specialized.StringDictionary" /> contains a specific key.</summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Specialized.StringDictionary" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Specialized.StringDictionary" /> contains an entry with the specified key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The key is <see langword="null" />.</exception>
		public virtual bool ContainsKey(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this.contents.ContainsKey(key.ToLowerInvariant());
		}

		/// <summary>Determines if the <see cref="T:System.Collections.Specialized.StringDictionary" /> contains a specific value.</summary>
		/// <param name="value">The value to locate in the <see cref="T:System.Collections.Specialized.StringDictionary" />. The value can be <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Specialized.StringDictionary" /> contains an element with the specified value; otherwise, <see langword="false" />.</returns>
		public virtual bool ContainsValue(string value)
		{
			return this.contents.ContainsValue(value);
		}

		/// <summary>Copies the string dictionary values to a one-dimensional <see cref="T:System.Array" /> instance at the specified index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the values copied from the <see cref="T:System.Collections.Specialized.StringDictionary" />.</param>
		/// <param name="index">The index in the array where copying begins.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional.  
		/// -or-  
		/// The number of elements in the <see cref="T:System.Collections.Specialized.StringDictionary" /> is greater than the available space from <paramref name="index" /> to the end of <paramref name="array" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than the lower bound of <paramref name="array" />.</exception>
		public virtual void CopyTo(Array array, int index)
		{
			this.contents.CopyTo(array, index);
		}

		/// <summary>Returns an enumerator that iterates through the string dictionary.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that iterates through the string dictionary.</returns>
		public virtual IEnumerator GetEnumerator()
		{
			return this.contents.GetEnumerator();
		}

		/// <summary>Removes the entry with the specified key from the string dictionary.</summary>
		/// <param name="key">The key of the entry to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">The key is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Specialized.StringDictionary" /> is read-only.</exception>
		public virtual void Remove(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.contents.Remove(key.ToLowerInvariant());
		}

		internal void ReplaceHashtable(Hashtable useThisHashtableInstead)
		{
			this.contents = useThisHashtableInstead;
		}

		internal IDictionary<string, string> AsGenericDictionary()
		{
			return new GenericAdapter(this);
		}

		internal Hashtable contents = new Hashtable();
	}
}
