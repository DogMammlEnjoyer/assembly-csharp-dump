using System;
using System.Diagnostics;

namespace System.Collections.Generic
{
	/// <summary>Represents a collection of key/value pairs that are sorted on the key.</summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(IDictionaryDebugView<, >))]
	[Serializable]
	public class SortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<!0, !1>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> class that is empty and uses the default <see cref="T:System.Collections.Generic.IComparer`1" /> implementation for the key type.</summary>
		public SortedDictionary() : this(null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> class that contains elements copied from the specified <see cref="T:System.Collections.Generic.IDictionary`2" /> and uses the default <see cref="T:System.Collections.Generic.IComparer`1" /> implementation for the key type.</summary>
		/// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="dictionary" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="dictionary" /> contains one or more duplicate keys.</exception>
		public SortedDictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> class that contains elements copied from the specified <see cref="T:System.Collections.Generic.IDictionary`2" /> and uses the specified <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to compare keys.</summary>
		/// <param name="dictionary">The <see cref="T:System.Collections.Generic.IDictionary`2" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.Comparer`1" /> for the type of the key.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="dictionary" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="dictionary" /> contains one or more duplicate keys.</exception>
		public SortedDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			this._set = new TreeSet<KeyValuePair<TKey, TValue>>(new SortedDictionary<TKey, TValue>.KeyValuePairComparer(comparer));
			foreach (KeyValuePair<TKey, TValue> item in dictionary)
			{
				this._set.Add(item);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> class that is empty and uses the specified <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to compare keys.</summary>
		/// <param name="comparer">The <see cref="T:System.Collections.Generic.IComparer`1" /> implementation to use when comparing keys, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.Comparer`1" /> for the type of the key.</param>
		public SortedDictionary(IComparer<TKey> comparer)
		{
			this._set = new TreeSet<KeyValuePair<TKey, TValue>>(new SortedDictionary<TKey, TValue>.KeyValuePairComparer(comparer));
		}

		void ICollection<KeyValuePair<!0, !1>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
		{
			this._set.Add(keyValuePair);
		}

		bool ICollection<KeyValuePair<!0, !1>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
		{
			SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(keyValuePair);
			if (node == null)
			{
				return false;
			}
			if (keyValuePair.Value == null)
			{
				return node.Item.Value == null;
			}
			return EqualityComparer<TValue>.Default.Equals(node.Item.Value, keyValuePair.Value);
		}

		bool ICollection<KeyValuePair<!0, !1>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
		{
			SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(keyValuePair);
			if (node == null)
			{
				return false;
			}
			if (EqualityComparer<TValue>.Default.Equals(node.Item.Value, keyValuePair.Value))
			{
				this._set.Remove(keyValuePair);
				return true;
			}
			return false;
		}

		bool ICollection<KeyValuePair<!0, !1>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets or sets the value associated with the specified key.</summary>
		/// <param name="key">The key of the value to get or set.</param>
		/// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="T:System.Collections.Generic.KeyNotFoundException" />, and a set operation creates a new element with the specified key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> does not exist in the collection.</exception>
		public TValue this[TKey key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(new KeyValuePair<TKey, TValue>(key, default(TValue)));
				if (node == null)
				{
					throw new KeyNotFoundException(SR.Format("The given key '{0}' was not present in the dictionary.", key.ToString()));
				}
				return node.Item.Value;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(new KeyValuePair<TKey, TValue>(key, default(TValue)));
				if (node == null)
				{
					this._set.Add(new KeyValuePair<TKey, TValue>(key, value));
					return;
				}
				node.Item = new KeyValuePair<TKey, TValue>(node.Item.Key, value);
				this._set.UpdateVersion();
			}
		}

		/// <summary>Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
		/// <returns>The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</returns>
		public int Count
		{
			get
			{
				return this._set.Count;
			}
		}

		/// <summary>Gets the <see cref="T:System.Collections.Generic.IComparer`1" /> used to order the elements of the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
		/// <returns>The <see cref="T:System.Collections.Generic.IComparer`1" /> used to order the elements of the <see cref="T:System.Collections.Generic.SortedDictionary`2" /></returns>
		public IComparer<TKey> Comparer
		{
			get
			{
				return ((SortedDictionary<TKey, TValue>.KeyValuePairComparer)this._set.Comparer).keyComparer;
			}
		}

		/// <summary>Gets a collection containing the keys in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
		/// <returns>A <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" /> containing the keys in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</returns>
		public SortedDictionary<TKey, TValue>.KeyCollection Keys
		{
			get
			{
				if (this._keys == null)
				{
					this._keys = new SortedDictionary<TKey, TValue>.KeyCollection(this);
				}
				return this._keys;
			}
		}

		ICollection<TKey> IDictionary<!0, !1>.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		IEnumerable<TKey> IReadOnlyDictionary<!0, !1>.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		/// <summary>Gets a collection containing the values in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
		/// <returns>A <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" /> containing the values in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</returns>
		public SortedDictionary<TKey, TValue>.ValueCollection Values
		{
			get
			{
				if (this._values == null)
				{
					this._values = new SortedDictionary<TKey, TValue>.ValueCollection(this);
				}
				return this._values;
			}
		}

		ICollection<TValue> IDictionary<!0, !1>.Values
		{
			get
			{
				return this.Values;
			}
		}

		IEnumerable<TValue> IReadOnlyDictionary<!0, !1>.Values
		{
			get
			{
				return this.Values;
			}
		}

		/// <summary>Adds an element with the specified key and value into the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
		/// <param name="key">The key of the element to add.</param>
		/// <param name="value">The value of the element to add. The value can be <see langword="null" /> for reference types.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</exception>
		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this._set.Add(new KeyValuePair<TKey, TValue>(key, value));
		}

		/// <summary>Removes all elements from the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
		public void Clear()
		{
			this._set.Clear();
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> contains an element with the specified key.</summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public bool ContainsKey(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this._set.Contains(new KeyValuePair<TKey, TValue>(key, default(TValue)));
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> contains an element with the specified value.</summary>
		/// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />. The value can be <see langword="null" /> for reference types.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> contains an element with the specified value; otherwise, <see langword="false" />.</returns>
		public bool ContainsValue(TValue value)
		{
			bool found = false;
			if (value == null)
			{
				this._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
				{
					if (node.Item.Value == null)
					{
						found = true;
						return false;
					}
					return true;
				});
			}
			else
			{
				EqualityComparer<TValue> valueComparer = EqualityComparer<TValue>.Default;
				this._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
				{
					if (valueComparer.Equals(node.Item.Value, value))
					{
						found = true;
						return false;
					}
					return true;
				});
			}
			return found;
		}

		/// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> to the specified array of <see cref="T:System.Collections.Generic.KeyValuePair`2" /> structures, starting at the specified index.</summary>
		/// <param name="array">The one-dimensional array of <see cref="T:System.Collections.Generic.KeyValuePair`2" /> structures that is the destination of the elements copied from the current <see cref="T:System.Collections.Generic.SortedDictionary`2" /> The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.SortedDictionary`2" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			this._set.CopyTo(array, index);
		}

		/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
		/// <returns>A <see cref="T:System.Collections.Generic.SortedDictionary`2.Enumerator" /> for the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</returns>
		public SortedDictionary<TKey, TValue>.Enumerator GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this, 1);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this, 1);
		}

		/// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="key" /> is not found in the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return this._set.Remove(new KeyValuePair<TKey, TValue>(key, default(TValue)));
		}

		/// <summary>Gets the value associated with the specified key.</summary>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			SortedSet<KeyValuePair<TKey, TValue>>.Node node = this._set.FindNode(new KeyValuePair<TKey, TValue>(key, default(TValue)));
			if (node == null)
			{
				value = default(TValue);
				return false;
			}
			value = node.Item.Value;
			return true;
		}

		/// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an array, starting at the specified array index.</summary>
		/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.ICollection`1" />. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional.  
		/// -or-  
		/// <paramref name="array" /> does not have zero-based indexing.  
		/// -or-  
		/// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.  
		/// -or-  
		/// The type of the source <see cref="T:System.Collections.Generic.ICollection`1" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this._set).CopyTo(array, index);
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> has a fixed size.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.IDictionary" /> has a fixed size; otherwise, <see langword="false" />.  In the default implementation of <see cref="T:System.Collections.Generic.SortedDictionary`2" />, this property always returns <see langword="false" />.</returns>
		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.IDictionary" /> is read-only; otherwise, <see langword="false" />.  In the default implementation of <see cref="T:System.Collections.Generic.SortedDictionary`2" />, this property always returns <see langword="false" />.</returns>
		bool IDictionary.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> containing the keys of the <see cref="T:System.Collections.IDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> containing the keys of the <see cref="T:System.Collections.IDictionary" />.</returns>
		ICollection IDictionary.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> containing the values in the <see cref="T:System.Collections.IDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> containing the values in the <see cref="T:System.Collections.IDictionary" />.</returns>
		ICollection IDictionary.Values
		{
			get
			{
				return this.Values;
			}
		}

		/// <summary>Gets or sets the element with the specified key.</summary>
		/// <param name="key">The key of the element to get.</param>
		/// <returns>The element with the specified key, or <see langword="null" /> if <paramref name="key" /> is not in the dictionary or <paramref name="key" /> is of a type that is not assignable to the key type <paramref name="TKey" /> of the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">A value is being assigned, and <paramref name="key" /> is of a type that is not assignable to the key type <paramref name="TKey" /> of the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.  
		///  -or-  
		///  A value is being assigned, and <paramref name="value" /> is of a type that is not assignable to the value type <paramref name="TValue" /> of the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</exception>
		object IDictionary.this[object key]
		{
			get
			{
				TValue tvalue;
				if (SortedDictionary<TKey, TValue>.IsCompatibleKey(key) && this.TryGetValue((TKey)((object)key), out tvalue))
				{
					return tvalue;
				}
				return null;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				if (value == null && default(TValue) != null)
				{
					throw new ArgumentNullException("value");
				}
				try
				{
					TKey key2 = (TKey)((object)key);
					try
					{
						this[key2] = (TValue)((object)value);
					}
					catch (InvalidCastException)
					{
						throw new ArgumentException(SR.Format("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.", value, typeof(TValue)), "value");
					}
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(SR.Format("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.", key, typeof(TKey)), "key");
				}
			}
		}

		/// <summary>Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" />.</summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value of the element to add.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="key" /> is of a type that is not assignable to the key type <paramref name="TKey" /> of the <see cref="T:System.Collections.IDictionary" />.  
		/// -or-  
		/// <paramref name="value" /> is of a type that is not assignable to the value type <paramref name="TValue" /> of the <see cref="T:System.Collections.IDictionary" />.  
		/// -or-  
		/// An element with the same key already exists in the <see cref="T:System.Collections.IDictionary" />.</exception>
		void IDictionary.Add(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (value == null && default(TValue) != null)
			{
				throw new ArgumentNullException("value");
			}
			try
			{
				TKey key2 = (TKey)((object)key);
				try
				{
					this.Add(key2, (TValue)((object)value));
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(SR.Format("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.", value, typeof(TValue)), "value");
				}
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException(SR.Format("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.", key, typeof(TKey)), "key");
			}
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.IDictionary" /> contains an element with the specified key.</summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.IDictionary" /> contains an element with the key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		bool IDictionary.Contains(object key)
		{
			return SortedDictionary<TKey, TValue>.IsCompatibleKey(key) && this.ContainsKey((TKey)((object)key));
		}

		private static bool IsCompatibleKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return key is TKey;
		}

		/// <summary>Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> for the <see cref="T:System.Collections.IDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Collections.IDictionaryEnumerator" /> for the <see cref="T:System.Collections.IDictionary" />.</returns>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this, 2);
		}

		/// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" />.</summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		void IDictionary.Remove(object key)
		{
			if (SortedDictionary<TKey, TValue>.IsCompatibleKey(key))
			{
				this.Remove((TKey)((object)key));
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.  In the default implementation of <see cref="T:System.Collections.Generic.SortedDictionary`2" />, this property always returns <see langword="false" />.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this._set).SyncRoot;
			}
		}

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SortedDictionary<TKey, TValue>.Enumerator(this, 1);
		}

		[NonSerialized]
		private SortedDictionary<TKey, TValue>.KeyCollection _keys;

		[NonSerialized]
		private SortedDictionary<TKey, TValue>.ValueCollection _values;

		private TreeSet<KeyValuePair<TKey, TValue>> _set;

		/// <summary>Enumerates the elements of a <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
		/// <typeparam name="TKey" />
		/// <typeparam name="TValue" />
		public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator, IDictionaryEnumerator
		{
			internal Enumerator(SortedDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
			{
				this._treeEnum = dictionary._set.GetEnumerator();
				this._getEnumeratorRetType = getEnumeratorRetType;
			}

			/// <summary>Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
			/// <returns>
			///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			public bool MoveNext()
			{
				return this._treeEnum.MoveNext();
			}

			/// <summary>Releases all resources used by the <see cref="T:System.Collections.Generic.SortedDictionary`2.Enumerator" />.</summary>
			public void Dispose()
			{
				this._treeEnum.Dispose();
			}

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the <see cref="T:System.Collections.Generic.SortedDictionary`2" /> at the current position of the enumerator.</returns>
			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return this._treeEnum.Current;
				}
			}

			internal bool NotStartedOrEnded
			{
				get
				{
					return this._treeEnum.NotStartedOrEnded;
				}
			}

			internal void Reset()
			{
				this._treeEnum.Reset();
			}

			/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			void IEnumerator.Reset()
			{
				this._treeEnum.Reset();
			}

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the collection at the current position of the enumerator.</returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
			object IEnumerator.Current
			{
				get
				{
					if (this.NotStartedOrEnded)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					KeyValuePair<TKey, TValue> keyValuePair;
					if (this._getEnumeratorRetType == 2)
					{
						keyValuePair = this.Current;
						object key = keyValuePair.Key;
						keyValuePair = this.Current;
						return new DictionaryEntry(key, keyValuePair.Value);
					}
					keyValuePair = this.Current;
					TKey key2 = keyValuePair.Key;
					keyValuePair = this.Current;
					return new KeyValuePair<TKey, TValue>(key2, keyValuePair.Value);
				}
			}

			/// <summary>Gets the key of the element at the current position of the enumerator.</summary>
			/// <returns>The key of the element in the collection at the current position of the enumerator.</returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
			object IDictionaryEnumerator.Key
			{
				get
				{
					if (this.NotStartedOrEnded)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					KeyValuePair<TKey, TValue> keyValuePair = this.Current;
					return keyValuePair.Key;
				}
			}

			/// <summary>Gets the value of the element at the current position of the enumerator.</summary>
			/// <returns>The value of the element in the collection at the current position of the enumerator.</returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
			object IDictionaryEnumerator.Value
			{
				get
				{
					if (this.NotStartedOrEnded)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					KeyValuePair<TKey, TValue> keyValuePair = this.Current;
					return keyValuePair.Value;
				}
			}

			/// <summary>Gets the element at the current position of the enumerator as a <see cref="T:System.Collections.DictionaryEntry" /> structure.</summary>
			/// <returns>The element in the collection at the current position of the dictionary, as a <see cref="T:System.Collections.DictionaryEntry" /> structure.</returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					if (this.NotStartedOrEnded)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					KeyValuePair<TKey, TValue> keyValuePair = this.Current;
					object key = keyValuePair.Key;
					keyValuePair = this.Current;
					return new DictionaryEntry(key, keyValuePair.Value);
				}
			}

			private SortedSet<KeyValuePair<TKey, TValue>>.Enumerator _treeEnum;

			private int _getEnumeratorRetType;

			internal const int KeyValuePair = 1;

			internal const int DictEntry = 2;
		}

		/// <summary>Represents the collection of keys in a <see cref="T:System.Collections.Generic.SortedDictionary`2" />. This class cannot be inherited.</summary>
		/// <typeparam name="TKey" />
		/// <typeparam name="TValue" />
		[DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<, >))]
		[DebuggerDisplay("Count = {Count}")]
		[Serializable]
		public sealed class KeyCollection : ICollection<!0>, IEnumerable<!0>, IEnumerable, ICollection, IReadOnlyCollection<TKey>
		{
			/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" /> class that reflects the keys in the specified <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
			/// <param name="dictionary">The <see cref="T:System.Collections.Generic.SortedDictionary`2" /> whose keys are reflected in the new <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="dictionary" /> is <see langword="null" />.</exception>
			public KeyCollection(SortedDictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this._dictionary = dictionary;
			}

			/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />.</summary>
			/// <returns>A <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection.Enumerator" /> structure for the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />.</returns>
			public SortedDictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dictionary);
			}

			IEnumerator<TKey> IEnumerable<!0>.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dictionary);
			}

			/// <summary>Returns an enumerator that iterates through the collection.</summary>
			/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.KeyCollection.Enumerator(this._dictionary);
			}

			/// <summary>Copies the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" /> elements to an existing one-dimensional array, starting at the specified array index.</summary>
			/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />. The array must have zero-based indexing.</param>
			/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="array" /> is <see langword="null" />.</exception>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			///   <paramref name="index" /> is less than 0.</exception>
			/// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
			public void CopyTo(TKey[] array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
				}
				if (array.Length - index < this.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				this._dictionary._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
				{
					TKey[] array2 = array;
					int index2 = index;
					index = index2 + 1;
					array2[index2] = node.Item.Key;
					return true;
				});
			}

			/// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an array, starting at a particular array index.</summary>
			/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.ICollection" />. The array must have zero-based indexing.</param>
			/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="array" /> is <see langword="null" />.</exception>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			///   <paramref name="index" /> is less than 0.</exception>
			/// <exception cref="T:System.ArgumentException">
			///   <paramref name="array" /> is multidimensional.  
			/// -or-  
			/// <paramref name="array" /> does not have zero-based indexing.  
			/// -or-  
			/// The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.  
			/// -or-  
			/// The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
			void ICollection.CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (array.Rank != 1)
				{
					throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
				}
				if (array.GetLowerBound(0) != 0)
				{
					throw new ArgumentException("The lower bound of target array must be zero.", "array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
				}
				if (array.Length - index < this._dictionary.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				TKey[] array2 = array as TKey[];
				if (array2 != null)
				{
					this.CopyTo(array2, index);
					return;
				}
				try
				{
					object[] objects = (object[])array;
					this._dictionary._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
					{
						object[] objects = objects;
						int index2 = index;
						index = index2 + 1;
						objects[index2] = node.Item.Key;
						return true;
					});
				}
				catch (ArrayTypeMismatchException)
				{
					throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", "array");
				}
			}

			/// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />.</summary>
			/// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />.</returns>
			public int Count
			{
				get
				{
					return this._dictionary.Count;
				}
			}

			bool ICollection<!0>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			void ICollection<!0>.Add(TKey item)
			{
				throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
			}

			void ICollection<!0>.Clear()
			{
				throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
			}

			bool ICollection<!0>.Contains(TKey item)
			{
				return this._dictionary.ContainsKey(item);
			}

			bool ICollection<!0>.Remove(TKey item)
			{
				throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
			}

			/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
			/// <returns>
			///   <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.  In the default implementation of <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />, this property always returns <see langword="false" />.</returns>
			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
			/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.  In the default implementation of <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />, this property always returns the current instance.</returns>
			object ICollection.SyncRoot
			{
				get
				{
					return ((ICollection)this._dictionary).SyncRoot;
				}
			}

			private SortedDictionary<TKey, TValue> _dictionary;

			/// <summary>Enumerates the elements of a <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />.</summary>
			/// <typeparam name="TKey" />
			/// <typeparam name="TValue" />
			public struct Enumerator : IEnumerator<!0>, IDisposable, IEnumerator
			{
				internal Enumerator(SortedDictionary<TKey, TValue> dictionary)
				{
					this._dictEnum = dictionary.GetEnumerator();
				}

				/// <summary>Releases all resources used by the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection.Enumerator" />.</summary>
				public void Dispose()
				{
					this._dictEnum.Dispose();
				}

				/// <summary>Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" />.</summary>
				/// <returns>
				///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
				/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
				public bool MoveNext()
				{
					return this._dictEnum.MoveNext();
				}

				/// <summary>Gets the element at the current position of the enumerator.</summary>
				/// <returns>The element in the <see cref="T:System.Collections.Generic.SortedDictionary`2.KeyCollection" /> at the current position of the enumerator.</returns>
				public TKey Current
				{
					get
					{
						KeyValuePair<TKey, TValue> keyValuePair = this._dictEnum.Current;
						return keyValuePair.Key;
					}
				}

				/// <summary>Gets the element at the current position of the enumerator.</summary>
				/// <returns>The element in the collection at the current position of the enumerator.</returns>
				/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
				object IEnumerator.Current
				{
					get
					{
						if (this._dictEnum.NotStartedOrEnded)
						{
							throw new InvalidOperationException("Enumeration has either not started or has already finished.");
						}
						return this.Current;
					}
				}

				/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
				/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
				void IEnumerator.Reset()
				{
					this._dictEnum.Reset();
				}

				private SortedDictionary<TKey, TValue>.Enumerator _dictEnum;
			}
		}

		/// <summary>Represents the collection of values in a <see cref="T:System.Collections.Generic.SortedDictionary`2" />. This class cannot be inherited</summary>
		/// <typeparam name="TKey" />
		/// <typeparam name="TValue" />
		[DebuggerDisplay("Count = {Count}")]
		[DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<, >))]
		[Serializable]
		public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection, IReadOnlyCollection<TValue>
		{
			/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" /> class that reflects the values in the specified <see cref="T:System.Collections.Generic.SortedDictionary`2" />.</summary>
			/// <param name="dictionary">The <see cref="T:System.Collections.Generic.SortedDictionary`2" /> whose values are reflected in the new <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="dictionary" /> is <see langword="null" />.</exception>
			public ValueCollection(SortedDictionary<TKey, TValue> dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException("dictionary");
				}
				this._dictionary = dictionary;
			}

			/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />.</summary>
			/// <returns>A <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection.Enumerator" /> structure for the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />.</returns>
			public SortedDictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dictionary);
			}

			IEnumerator<TValue> IEnumerable<!1>.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dictionary);
			}

			/// <summary>Returns an enumerator that iterates through the collection.</summary>
			/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the collection.</returns>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return new SortedDictionary<TKey, TValue>.ValueCollection.Enumerator(this._dictionary);
			}

			/// <summary>Copies the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" /> elements to an existing one-dimensional array, starting at the specified array index.</summary>
			/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />. The array must have zero-based indexing.</param>
			/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="array" /> is <see langword="null" />.</exception>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			///   <paramref name="index" /> is less than 0.</exception>
			/// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
			public void CopyTo(TValue[] array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
				}
				if (array.Length - index < this.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				this._dictionary._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
				{
					TValue[] array2 = array;
					int index2 = index;
					index = index2 + 1;
					array2[index2] = node.Item.Value;
					return true;
				});
			}

			/// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an array, starting at a particular array index.</summary>
			/// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.ICollection" />. The array must have zero-based indexing.</param>
			/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="array" /> is <see langword="null" />.</exception>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			///   <paramref name="index" /> is less than 0.</exception>
			/// <exception cref="T:System.ArgumentException">
			///   <paramref name="array" /> is multidimensional.  
			/// -or-  
			/// <paramref name="array" /> does not have zero-based indexing.  
			/// -or-  
			/// The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.  
			/// -or-  
			/// The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
			void ICollection.CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (array.Rank != 1)
				{
					throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
				}
				if (array.GetLowerBound(0) != 0)
				{
					throw new ArgumentException("The lower bound of target array must be zero.", "array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
				}
				if (array.Length - index < this._dictionary.Count)
				{
					throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
				}
				TValue[] array2 = array as TValue[];
				if (array2 != null)
				{
					this.CopyTo(array2, index);
					return;
				}
				try
				{
					object[] objects = (object[])array;
					this._dictionary._set.InOrderTreeWalk(delegate(SortedSet<KeyValuePair<TKey, TValue>>.Node node)
					{
						object[] objects = objects;
						int index2 = index;
						index = index2 + 1;
						objects[index2] = node.Item.Value;
						return true;
					});
				}
				catch (ArrayTypeMismatchException)
				{
					throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", "array");
				}
			}

			/// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />.</summary>
			/// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />.</returns>
			public int Count
			{
				get
				{
					return this._dictionary.Count;
				}
			}

			bool ICollection<!1>.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			void ICollection<!1>.Add(TValue item)
			{
				throw new NotSupportedException("Mutating a value collection derived from a dictionary is not allowed.");
			}

			void ICollection<!1>.Clear()
			{
				throw new NotSupportedException("Mutating a value collection derived from a dictionary is not allowed.");
			}

			bool ICollection<!1>.Contains(TValue item)
			{
				return this._dictionary.ContainsValue(item);
			}

			bool ICollection<!1>.Remove(TValue item)
			{
				throw new NotSupportedException("Mutating a value collection derived from a dictionary is not allowed.");
			}

			/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
			/// <returns>
			///   <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.  In the default implementation of <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />, this property always returns <see langword="false" />.</returns>
			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
			/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.  In the default implementation of <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />, this property always returns the current instance.</returns>
			object ICollection.SyncRoot
			{
				get
				{
					return ((ICollection)this._dictionary).SyncRoot;
				}
			}

			private SortedDictionary<TKey, TValue> _dictionary;

			/// <summary>Enumerates the elements of a <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />.</summary>
			/// <typeparam name="TKey" />
			/// <typeparam name="TValue" />
			public struct Enumerator : IEnumerator<TValue>, IDisposable, IEnumerator
			{
				internal Enumerator(SortedDictionary<TKey, TValue> dictionary)
				{
					this._dictEnum = dictionary.GetEnumerator();
				}

				/// <summary>Releases all resources used by the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection.Enumerator" />.</summary>
				public void Dispose()
				{
					this._dictEnum.Dispose();
				}

				/// <summary>Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" />.</summary>
				/// <returns>
				///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
				/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
				public bool MoveNext()
				{
					return this._dictEnum.MoveNext();
				}

				/// <summary>Gets the element at the current position of the enumerator.</summary>
				/// <returns>The element in the <see cref="T:System.Collections.Generic.SortedDictionary`2.ValueCollection" /> at the current position of the enumerator.</returns>
				public TValue Current
				{
					get
					{
						KeyValuePair<TKey, TValue> keyValuePair = this._dictEnum.Current;
						return keyValuePair.Value;
					}
				}

				/// <summary>Gets the element at the current position of the enumerator.</summary>
				/// <returns>The element in the collection at the current position of the enumerator.</returns>
				/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
				object IEnumerator.Current
				{
					get
					{
						if (this._dictEnum.NotStartedOrEnded)
						{
							throw new InvalidOperationException("Enumeration has either not started or has already finished.");
						}
						return this.Current;
					}
				}

				/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
				/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
				void IEnumerator.Reset()
				{
					this._dictEnum.Reset();
				}

				private SortedDictionary<TKey, TValue>.Enumerator _dictEnum;
			}
		}

		[Serializable]
		internal sealed class KeyValuePairComparer : Comparer<KeyValuePair<TKey, TValue>>
		{
			public KeyValuePairComparer(IComparer<TKey> keyComparer)
			{
				if (keyComparer == null)
				{
					this.keyComparer = Comparer<TKey>.Default;
					return;
				}
				this.keyComparer = keyComparer;
			}

			public override int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
			{
				return this.keyComparer.Compare(x.Key, y.Key);
			}

			internal IComparer<TKey> keyComparer;
		}
	}
}
