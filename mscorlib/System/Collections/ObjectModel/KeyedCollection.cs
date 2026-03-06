using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Collections.ObjectModel
{
	/// <summary>Provides the abstract base class for a collection whose keys are embedded in the values.</summary>
	/// <typeparam name="TKey">The type of keys in the collection.</typeparam>
	/// <typeparam name="TItem">The type of items in the collection.</typeparam>
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public abstract class KeyedCollection<TKey, TItem> : Collection<TItem>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" /> class that uses the default equality comparer.</summary>
		protected KeyedCollection() : this(null, 0)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" /> class that uses the specified equality comparer.</summary>
		/// <param name="comparer">The implementation of the <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> generic interface to use when comparing keys, or <see langword="null" /> to use the default equality comparer for the type of the key, obtained from <see cref="P:System.Collections.Generic.EqualityComparer`1.Default" />.</param>
		protected KeyedCollection(IEqualityComparer<TKey> comparer) : this(comparer, 0)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" /> class that uses the specified equality comparer and creates a lookup dictionary when the specified threshold is exceeded.</summary>
		/// <param name="comparer">The implementation of the <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> generic interface to use when comparing keys, or <see langword="null" /> to use the default equality comparer for the type of the key, obtained from <see cref="P:System.Collections.Generic.EqualityComparer`1.Default" />.</param>
		/// <param name="dictionaryCreationThreshold">The number of elements the collection can hold without creating a lookup dictionary (0 creates the lookup dictionary when the first item is added), or -1 to specify that a lookup dictionary is never created.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="dictionaryCreationThreshold" /> is less than -1.</exception>
		protected KeyedCollection(IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold) : base(new List<TItem>())
		{
			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}
			if (dictionaryCreationThreshold == -1)
			{
				dictionaryCreationThreshold = int.MaxValue;
			}
			if (dictionaryCreationThreshold < -1)
			{
				throw new ArgumentOutOfRangeException("dictionaryCreationThreshold", "The specified threshold for creating dictionary is out of range.");
			}
			this.comparer = comparer;
			this.threshold = dictionaryCreationThreshold;
		}

		private new List<TItem> Items
		{
			get
			{
				return (List<TItem>)base.Items;
			}
		}

		/// <summary>Gets the generic equality comparer that is used to determine equality of keys in the collection.</summary>
		/// <returns>The implementation of the <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> generic interface that is used to determine equality of keys in the collection.</returns>
		public IEqualityComparer<TKey> Comparer
		{
			get
			{
				return this.comparer;
			}
		}

		/// <summary>Gets the element with the specified key.</summary>
		/// <param name="key">The key of the element to get.</param>
		/// <returns>The element with the specified key. If an element with the specified key is not found, an exception is thrown.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">An element with the specified key does not exist in the collection.</exception>
		public TItem this[TKey key]
		{
			get
			{
				TItem result;
				if (this.TryGetValue(key, out result))
				{
					return result;
				}
				throw new KeyNotFoundException(SR.Format("The given key '{0}' was not present in the dictionary.", key.ToString()));
			}
		}

		/// <summary>Determines whether the collection contains an element with the specified key.</summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" /> contains an element with the specified key; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public bool Contains(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.dict != null)
			{
				return this.dict.ContainsKey(key);
			}
			foreach (TItem item in this.Items)
			{
				if (this.comparer.Equals(this.GetKeyForItem(item), key))
				{
					return true;
				}
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TItem item)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.dict != null)
			{
				return this.dict.TryGetValue(key, out item);
			}
			foreach (TItem titem in this.Items)
			{
				TKey keyForItem = this.GetKeyForItem(titem);
				if (keyForItem != null && this.comparer.Equals(key, keyForItem))
				{
					item = titem;
					return true;
				}
			}
			item = default(TItem);
			return false;
		}

		private bool ContainsItem(TItem item)
		{
			TKey keyForItem;
			if (this.dict == null || (keyForItem = this.GetKeyForItem(item)) == null)
			{
				return this.Items.Contains(item);
			}
			TItem x;
			return this.dict.TryGetValue(keyForItem, out x) && EqualityComparer<TItem>.Default.Equals(x, item);
		}

		/// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />.</summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the element is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="key" /> is not found in the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.dict != null)
			{
				TItem item;
				return this.dict.TryGetValue(key, out item) && base.Remove(item);
			}
			for (int i = 0; i < this.Items.Count; i++)
			{
				if (this.comparer.Equals(this.GetKeyForItem(this.Items[i]), key))
				{
					this.RemoveItem(i);
					return true;
				}
			}
			return false;
		}

		/// <summary>Gets the lookup dictionary of the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />.</summary>
		/// <returns>The lookup dictionary of the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />, if it exists; otherwise, <see langword="null" />.</returns>
		protected IDictionary<TKey, TItem> Dictionary
		{
			get
			{
				return this.dict;
			}
		}

		/// <summary>Changes the key associated with the specified element in the lookup dictionary.</summary>
		/// <param name="item">The element to change the key of.</param>
		/// <param name="newKey">The new key for <paramref name="item" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="item" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="item" /> is not found.  
		/// -or-  
		/// <paramref name="key" /> already exists in the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />.</exception>
		protected void ChangeItemKey(TItem item, TKey newKey)
		{
			if (!this.ContainsItem(item))
			{
				throw new ArgumentException("The specified item does not exist in this KeyedCollection.");
			}
			TKey keyForItem = this.GetKeyForItem(item);
			if (!this.comparer.Equals(keyForItem, newKey))
			{
				if (newKey != null)
				{
					this.AddKey(newKey, item);
				}
				if (keyForItem != null)
				{
					this.RemoveKey(keyForItem);
				}
			}
		}

		/// <summary>Removes all elements from the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />.</summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			if (this.dict != null)
			{
				this.dict.Clear();
			}
			this.keyCount = 0;
		}

		/// <summary>When implemented in a derived class, extracts the key from the specified element.</summary>
		/// <param name="item">The element from which to extract the key.</param>
		/// <returns>The key for the specified element.</returns>
		protected abstract TKey GetKeyForItem(TItem item);

		/// <summary>Inserts an element into the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" /> at the specified index.</summary>
		/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
		/// <param name="item">The object to insert.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than 0.  
		/// -or-  
		/// <paramref name="index" /> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.</exception>
		protected override void InsertItem(int index, TItem item)
		{
			TKey keyForItem = this.GetKeyForItem(item);
			if (keyForItem != null)
			{
				this.AddKey(keyForItem, item);
			}
			base.InsertItem(index, item);
		}

		/// <summary>Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />.</summary>
		/// <param name="index">The index of the element to remove.</param>
		protected override void RemoveItem(int index)
		{
			TKey keyForItem = this.GetKeyForItem(this.Items[index]);
			if (keyForItem != null)
			{
				this.RemoveKey(keyForItem);
			}
			base.RemoveItem(index);
		}

		/// <summary>Replaces the item at the specified index with the specified item.</summary>
		/// <param name="index">The zero-based index of the item to be replaced.</param>
		/// <param name="item">The new item.</param>
		protected override void SetItem(int index, TItem item)
		{
			TKey keyForItem = this.GetKeyForItem(item);
			TKey keyForItem2 = this.GetKeyForItem(this.Items[index]);
			if (this.comparer.Equals(keyForItem2, keyForItem))
			{
				if (keyForItem != null && this.dict != null)
				{
					this.dict[keyForItem] = item;
				}
			}
			else
			{
				if (keyForItem != null)
				{
					this.AddKey(keyForItem, item);
				}
				if (keyForItem2 != null)
				{
					this.RemoveKey(keyForItem2);
				}
			}
			base.SetItem(index, item);
		}

		private void AddKey(TKey key, TItem item)
		{
			if (this.dict != null)
			{
				this.dict.Add(key, item);
				return;
			}
			if (this.keyCount == this.threshold)
			{
				this.CreateDictionary();
				this.dict.Add(key, item);
				return;
			}
			if (this.Contains(key))
			{
				throw new ArgumentException(SR.Format("An item with the same key has already been added. Key: {0}", key));
			}
			this.keyCount++;
		}

		private void CreateDictionary()
		{
			this.dict = new Dictionary<TKey, TItem>(this.comparer);
			foreach (TItem titem in this.Items)
			{
				TKey keyForItem = this.GetKeyForItem(titem);
				if (keyForItem != null)
				{
					this.dict.Add(keyForItem, titem);
				}
			}
		}

		private void RemoveKey(TKey key)
		{
			if (this.dict != null)
			{
				this.dict.Remove(key);
				return;
			}
			this.keyCount--;
		}

		private const int defaultThreshold = 0;

		private readonly IEqualityComparer<TKey> comparer;

		private Dictionary<TKey, TItem> dict;

		private int keyCount;

		private readonly int threshold;
	}
}
