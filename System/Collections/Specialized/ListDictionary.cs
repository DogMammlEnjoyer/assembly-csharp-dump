using System;
using System.Threading;

namespace System.Collections.Specialized
{
	/// <summary>Implements <see langword="IDictionary" /> using a singly linked list. Recommended for collections that typically include fewer than 10 items.</summary>
	[Serializable]
	public class ListDictionary : IDictionary, ICollection, IEnumerable
	{
		/// <summary>Creates an empty <see cref="T:System.Collections.Specialized.ListDictionary" /> using the default comparer.</summary>
		public ListDictionary()
		{
		}

		/// <summary>Creates an empty <see cref="T:System.Collections.Specialized.ListDictionary" /> using the specified comparer.</summary>
		/// <param name="comparer">The <see cref="T:System.Collections.IComparer" /> to use to determine whether two keys are equal.  
		///  -or-  
		///  <see langword="null" /> to use the default comparer, which is each key's implementation of <see cref="M:System.Object.Equals(System.Object)" />.</param>
		public ListDictionary(IComparer comparer)
		{
			this.comparer = comparer;
		}

		/// <summary>Gets or sets the value associated with the specified key.</summary>
		/// <param name="key">The key whose value to get or set.</param>
		/// <returns>The value associated with the specified key. If the specified key is not found, attempting to get it returns <see langword="null" />, and attempting to set it creates a new entry using the specified key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public object this[object key]
		{
			get
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				ListDictionary.DictionaryNode next = this.head;
				if (this.comparer == null)
				{
					while (next != null)
					{
						if (next.key.Equals(key))
						{
							return next.value;
						}
						next = next.next;
					}
				}
				else
				{
					while (next != null)
					{
						object key2 = next.key;
						if (this.comparer.Compare(key2, key) == 0)
						{
							return next.value;
						}
						next = next.next;
					}
				}
				return null;
			}
			set
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				this.version++;
				ListDictionary.DictionaryNode dictionaryNode = null;
				ListDictionary.DictionaryNode next;
				for (next = this.head; next != null; next = next.next)
				{
					object key2 = next.key;
					if ((this.comparer == null) ? key2.Equals(key) : (this.comparer.Compare(key2, key) == 0))
					{
						break;
					}
					dictionaryNode = next;
				}
				if (next != null)
				{
					next.value = value;
					return;
				}
				ListDictionary.DictionaryNode dictionaryNode2 = new ListDictionary.DictionaryNode();
				dictionaryNode2.key = key;
				dictionaryNode2.value = value;
				if (dictionaryNode != null)
				{
					dictionaryNode.next = dictionaryNode2;
				}
				else
				{
					this.head = dictionaryNode2;
				}
				this.count++;
			}
		}

		/// <summary>Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Specialized.ListDictionary" />.</summary>
		/// <returns>The number of key/value pairs contained in the <see cref="T:System.Collections.Specialized.ListDictionary" />.</returns>
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> containing the keys in the <see cref="T:System.Collections.Specialized.ListDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> containing the keys in the <see cref="T:System.Collections.Specialized.ListDictionary" />.</returns>
		public ICollection Keys
		{
			get
			{
				return new ListDictionary.NodeKeyValueCollection(this, true);
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Specialized.ListDictionary" /> is read-only.</summary>
		/// <returns>This property always returns <see langword="false" />.</returns>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Specialized.ListDictionary" /> has a fixed size.</summary>
		/// <returns>This property always returns <see langword="false" />.</returns>
		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Specialized.ListDictionary" /> is synchronized (thread safe).</summary>
		/// <returns>This property always returns <see langword="false" />.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.Specialized.ListDictionary" />.</summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.Specialized.ListDictionary" />.</returns>
		public object SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> containing the values in the <see cref="T:System.Collections.Specialized.ListDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> containing the values in the <see cref="T:System.Collections.Specialized.ListDictionary" />.</returns>
		public ICollection Values
		{
			get
			{
				return new ListDictionary.NodeKeyValueCollection(this, false);
			}
		}

		/// <summary>Adds an entry with the specified key and value into the <see cref="T:System.Collections.Specialized.ListDictionary" />.</summary>
		/// <param name="key">The key of the entry to add.</param>
		/// <param name="value">The value of the entry to add. The value can be <see langword="null" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">An entry with the same key already exists in the <see cref="T:System.Collections.Specialized.ListDictionary" />.</exception>
		public void Add(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.version++;
			ListDictionary.DictionaryNode dictionaryNode = null;
			for (ListDictionary.DictionaryNode next = this.head; next != null; next = next.next)
			{
				object key2 = next.key;
				if ((this.comparer == null) ? key2.Equals(key) : (this.comparer.Compare(key2, key) == 0))
				{
					throw new ArgumentException(SR.Format("An item with the same key has already been added. Key: {0}", key));
				}
				dictionaryNode = next;
			}
			ListDictionary.DictionaryNode dictionaryNode2 = new ListDictionary.DictionaryNode();
			dictionaryNode2.key = key;
			dictionaryNode2.value = value;
			if (dictionaryNode != null)
			{
				dictionaryNode.next = dictionaryNode2;
			}
			else
			{
				this.head = dictionaryNode2;
			}
			this.count++;
		}

		/// <summary>Removes all entries from the <see cref="T:System.Collections.Specialized.ListDictionary" />.</summary>
		public void Clear()
		{
			this.count = 0;
			this.head = null;
			this.version++;
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.Specialized.ListDictionary" /> contains a specific key.</summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Specialized.ListDictionary" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Specialized.ListDictionary" /> contains an entry with the specified key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public bool Contains(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			for (ListDictionary.DictionaryNode next = this.head; next != null; next = next.next)
			{
				object key2 = next.key;
				if ((this.comparer == null) ? key2.Equals(key) : (this.comparer.Compare(key2, key) == 0))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Copies the <see cref="T:System.Collections.Specialized.ListDictionary" /> entries to a one-dimensional <see cref="T:System.Array" /> instance at the specified index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the <see cref="T:System.Collections.DictionaryEntry" /> objects copied from <see cref="T:System.Collections.Specialized.ListDictionary" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional.  
		/// -or-  
		/// The number of elements in the source <see cref="T:System.Collections.Specialized.ListDictionary" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
		/// <exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.Specialized.ListDictionary" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
			}
			if (array.Length - index < this.count)
			{
				throw new ArgumentException("Insufficient space in the target location to copy the information.");
			}
			for (ListDictionary.DictionaryNode next = this.head; next != null; next = next.next)
			{
				array.SetValue(new DictionaryEntry(next.key, next.value), index);
				index++;
			}
		}

		/// <summary>Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> that iterates through the <see cref="T:System.Collections.Specialized.ListDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Collections.IDictionaryEnumerator" /> for the <see cref="T:System.Collections.Specialized.ListDictionary" />.</returns>
		public IDictionaryEnumerator GetEnumerator()
		{
			return new ListDictionary.NodeEnumerator(this);
		}

		/// <summary>Returns an <see cref="T:System.Collections.IEnumerator" /> that iterates through the <see cref="T:System.Collections.Specialized.ListDictionary" />.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> for the <see cref="T:System.Collections.Specialized.ListDictionary" />.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ListDictionary.NodeEnumerator(this);
		}

		/// <summary>Removes the entry with the specified key from the <see cref="T:System.Collections.Specialized.ListDictionary" />.</summary>
		/// <param name="key">The key of the entry to remove.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public void Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			this.version++;
			ListDictionary.DictionaryNode dictionaryNode = null;
			ListDictionary.DictionaryNode next;
			for (next = this.head; next != null; next = next.next)
			{
				object key2 = next.key;
				if ((this.comparer == null) ? key2.Equals(key) : (this.comparer.Compare(key2, key) == 0))
				{
					break;
				}
				dictionaryNode = next;
			}
			if (next == null)
			{
				return;
			}
			if (next == this.head)
			{
				this.head = next.next;
			}
			else
			{
				dictionaryNode.next = next.next;
			}
			this.count--;
		}

		private ListDictionary.DictionaryNode head;

		private int version;

		private int count;

		private readonly IComparer comparer;

		[NonSerialized]
		private object _syncRoot;

		private class NodeEnumerator : IDictionaryEnumerator, IEnumerator
		{
			public NodeEnumerator(ListDictionary list)
			{
				this._list = list;
				this._version = list.version;
				this._start = true;
				this._current = null;
			}

			public object Current
			{
				get
				{
					return this.Entry;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					if (this._current == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return new DictionaryEntry(this._current.key, this._current.value);
				}
			}

			public object Key
			{
				get
				{
					if (this._current == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._current.key;
				}
			}

			public object Value
			{
				get
				{
					if (this._current == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._current.value;
				}
			}

			public bool MoveNext()
			{
				if (this._version != this._list.version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				if (this._start)
				{
					this._current = this._list.head;
					this._start = false;
				}
				else if (this._current != null)
				{
					this._current = this._current.next;
				}
				return this._current != null;
			}

			public void Reset()
			{
				if (this._version != this._list.version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this._start = true;
				this._current = null;
			}

			private ListDictionary _list;

			private ListDictionary.DictionaryNode _current;

			private int _version;

			private bool _start;
		}

		private class NodeKeyValueCollection : ICollection, IEnumerable
		{
			public NodeKeyValueCollection(ListDictionary list, bool isKeys)
			{
				this._list = list;
				this._isKeys = isKeys;
			}

			void ICollection.CopyTo(Array array, int index)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
				}
				for (ListDictionary.DictionaryNode dictionaryNode = this._list.head; dictionaryNode != null; dictionaryNode = dictionaryNode.next)
				{
					array.SetValue(this._isKeys ? dictionaryNode.key : dictionaryNode.value, index);
					index++;
				}
			}

			int ICollection.Count
			{
				get
				{
					int num = 0;
					for (ListDictionary.DictionaryNode dictionaryNode = this._list.head; dictionaryNode != null; dictionaryNode = dictionaryNode.next)
					{
						num++;
					}
					return num;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this._list.SyncRoot;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new ListDictionary.NodeKeyValueCollection.NodeKeyValueEnumerator(this._list, this._isKeys);
			}

			private ListDictionary _list;

			private bool _isKeys;

			private class NodeKeyValueEnumerator : IEnumerator
			{
				public NodeKeyValueEnumerator(ListDictionary list, bool isKeys)
				{
					this._list = list;
					this._isKeys = isKeys;
					this._version = list.version;
					this._start = true;
					this._current = null;
				}

				public object Current
				{
					get
					{
						if (this._current == null)
						{
							throw new InvalidOperationException("Enumeration has either not started or has already finished.");
						}
						if (!this._isKeys)
						{
							return this._current.value;
						}
						return this._current.key;
					}
				}

				public bool MoveNext()
				{
					if (this._version != this._list.version)
					{
						throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
					}
					if (this._start)
					{
						this._current = this._list.head;
						this._start = false;
					}
					else if (this._current != null)
					{
						this._current = this._current.next;
					}
					return this._current != null;
				}

				public void Reset()
				{
					if (this._version != this._list.version)
					{
						throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
					}
					this._start = true;
					this._current = null;
				}

				private ListDictionary _list;

				private ListDictionary.DictionaryNode _current;

				private int _version;

				private bool _isKeys;

				private bool _start;
			}
		}

		[Serializable]
		public class DictionaryNode
		{
			public object key;

			public object value;

			public ListDictionary.DictionaryNode next;
		}
	}
}
