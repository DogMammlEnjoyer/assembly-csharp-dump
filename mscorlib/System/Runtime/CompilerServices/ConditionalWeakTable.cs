using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Threading;

namespace System.Runtime.CompilerServices
{
	/// <summary>Enables compilers to dynamically attach object fields to managed objects.</summary>
	/// <typeparam name="TKey">The reference type to which the field is attached.</typeparam>
	/// <typeparam name="TValue">The field's type. This must be a reference type.</typeparam>
	public sealed class ConditionalWeakTable<TKey, TValue> : IEnumerable<KeyValuePair<!0, !1>>, IEnumerable where TKey : class where TValue : class
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.CompilerServices.ConditionalWeakTable`2" /> class.</summary>
		public ConditionalWeakTable()
		{
			this.data = new Ephemeron[13];
			GC.register_ephemeron_array(this.data);
		}

		/// <summary>Ensures that resources are freed and other cleanup operations are performed when the garbage collector reclaims the <see cref="T:System.Runtime.CompilerServices.ConditionalWeakTable`2" /> object.</summary>
		~ConditionalWeakTable()
		{
		}

		private void RehashWithoutResize()
		{
			int num = this.data.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.data[i].key == GC.EPHEMERON_TOMBSTONE)
				{
					this.data[i].key = null;
				}
			}
			for (int j = 0; j < num; j++)
			{
				object key = this.data[j].key;
				if (key != null)
				{
					int num2 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num;
					while (this.data[num2].key != null)
					{
						if (this.data[num2].key == key)
						{
							goto IL_108;
						}
						if (++num2 == num)
						{
							num2 = 0;
						}
					}
					this.data[num2].key = key;
					this.data[num2].value = this.data[j].value;
					this.data[j].key = null;
					this.data[j].value = null;
				}
				IL_108:;
			}
		}

		private void RecomputeSize()
		{
			this.size = 0;
			for (int i = 0; i < this.data.Length; i++)
			{
				if (this.data[i].key != null)
				{
					this.size++;
				}
			}
		}

		private void Rehash()
		{
			this.RecomputeSize();
			uint prime = (uint)HashHelpers.GetPrime((int)((float)this.size / 0.7f) << 1 | 1);
			if (prime > (float)this.data.Length * 0.5f && prime < (float)this.data.Length * 1.1f)
			{
				this.RehashWithoutResize();
				return;
			}
			Ephemeron[] array = new Ephemeron[prime];
			GC.register_ephemeron_array(array);
			this.size = 0;
			for (int i = 0; i < this.data.Length; i++)
			{
				object key = this.data[i].key;
				object value = this.data[i].value;
				if (key != null && key != GC.EPHEMERON_TOMBSTONE)
				{
					int num = array.Length;
					int num2 = -1;
					int num4;
					int num3 = num4 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num;
					do
					{
						object key2 = array[num4].key;
						if (key2 == null || key2 == GC.EPHEMERON_TOMBSTONE)
						{
							goto IL_D3;
						}
						if (++num4 == num)
						{
							num4 = 0;
						}
					}
					while (num4 != num3);
					IL_ED:
					array[num2].key = key;
					array[num2].value = value;
					this.size++;
					goto IL_118;
					IL_D3:
					num2 = num4;
					goto IL_ED;
				}
				IL_118:;
			}
			this.data = array;
		}

		public void AddOrUpdate(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("Null key", "key");
			}
			object @lock = this._lock;
			lock (@lock)
			{
				if ((float)this.size >= (float)this.data.Length * 0.7f)
				{
					this.Rehash();
				}
				int num = this.data.Length;
				int num2 = -1;
				int num4;
				int num3 = num4 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num;
				for (;;)
				{
					object key2 = this.data[num4].key;
					if (key2 == null)
					{
						break;
					}
					if (key2 == GC.EPHEMERON_TOMBSTONE && num2 == -1)
					{
						num2 = num4;
					}
					else if (key2 == key)
					{
						num2 = num4;
					}
					if (++num4 == num)
					{
						num4 = 0;
					}
					if (num4 == num3)
					{
						goto IL_BA;
					}
				}
				if (num2 == -1)
				{
					num2 = num4;
				}
				IL_BA:
				this.data[num2].key = key;
				this.data[num2].value = value;
				this.size++;
			}
		}

		/// <summary>Adds a key to the table.</summary>
		/// <param name="key">The key to add. <paramref name="key" /> represents the object to which the property is attached.</param>
		/// <param name="value">The key's property value.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="key" /> already exists.</exception>
		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("Null key", "key");
			}
			object @lock = this._lock;
			lock (@lock)
			{
				if ((float)this.size >= (float)this.data.Length * 0.7f)
				{
					this.Rehash();
				}
				int num = this.data.Length;
				int num2 = -1;
				int num4;
				int num3 = num4 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num;
				for (;;)
				{
					object key2 = this.data[num4].key;
					if (key2 == null)
					{
						break;
					}
					if (key2 == GC.EPHEMERON_TOMBSTONE && num2 == -1)
					{
						num2 = num4;
					}
					else if (key2 == key)
					{
						goto Block_9;
					}
					if (++num4 == num)
					{
						num4 = 0;
					}
					if (num4 == num3)
					{
						goto IL_C7;
					}
				}
				if (num2 == -1)
				{
					num2 = num4;
					goto IL_C7;
				}
				goto IL_C7;
				Block_9:
				throw new ArgumentException("Key already in the list", "key");
				IL_C7:
				this.data[num2].key = key;
				this.data[num2].value = value;
				this.size++;
			}
		}

		/// <summary>Removes a key and its value from the table.</summary>
		/// <param name="key">The key to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the key is found and removed; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("Null key", "key");
			}
			object @lock = this._lock;
			lock (@lock)
			{
				int num = this.data.Length;
				int num3;
				int num2 = num3 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num;
				for (;;)
				{
					object key2 = this.data[num3].key;
					if (key2 == key)
					{
						break;
					}
					if (key2 == null)
					{
						goto Block_5;
					}
					if (++num3 == num)
					{
						num3 = 0;
					}
					if (num3 == num2)
					{
						goto Block_7;
					}
				}
				this.data[num3].key = GC.EPHEMERON_TOMBSTONE;
				this.data[num3].value = null;
				return true;
				Block_5:
				Block_7:;
			}
			return false;
		}

		/// <summary>Gets the value of the specified key.</summary>
		/// <param name="key">The key that represents an object with an attached property.</param>
		/// <param name="value">When this method returns, contains the attached property value. If <paramref name="key" /> is not found, <paramref name="value" /> contains the default value.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="key" /> is found; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("Null key", "key");
			}
			value = default(TValue);
			object @lock = this._lock;
			lock (@lock)
			{
				int num = this.data.Length;
				int num3;
				int num2 = num3 = (RuntimeHelpers.GetHashCode(key) & int.MaxValue) % num;
				for (;;)
				{
					object key2 = this.data[num3].key;
					if (key2 == key)
					{
						break;
					}
					if (key2 == null)
					{
						goto Block_5;
					}
					if (++num3 == num)
					{
						num3 = 0;
					}
					if (num3 == num2)
					{
						goto Block_7;
					}
				}
				value = (TValue)((object)this.data[num3].value);
				return true;
				Block_5:
				Block_7:;
			}
			return false;
		}

		/// <summary>Atomically searches for a specified key in the table and returns the corresponding value. If the key does not exist in the table, the method invokes the default constructor of the class that represents the table's value to create a value that is bound to the specified key.</summary>
		/// <param name="key">The key to search for. <paramref name="key" /> represents the object to which the property is attached.</param>
		/// <returns>The value that corresponds to <paramref name="key" />, if <paramref name="key" /> already exists in the table; otherwise, a new value created by the default constructor of the class defined by the <paramref name="TValue" /> generic type parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.MissingMethodException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.MissingMemberException" />, instead.  
		///
		///
		///
		///
		///  The class that represents the table's value does not define a default constructor.</exception>
		public TValue GetOrCreateValue(TKey key)
		{
			return this.GetValue(key, (TKey k) => Activator.CreateInstance<TValue>());
		}

		/// <summary>Atomically searches for a specified key in the table and returns the corresponding value. If the key does not exist in the table, the method invokes a callback method to create a value that is bound to the specified key.</summary>
		/// <param name="key">The key to search for. <paramref name="key" /> represents the object to which the property is attached.</param>
		/// <param name="createValueCallback">A delegate to a method that can create a value for the given <paramref name="key" />. It has a single parameter of type TKey, and returns a value of type TValue.</param>
		/// <returns>The value attached to <paramref name="key" />, if <paramref name="key" /> already exists in the table; otherwise, the new value returned by the <paramref name="createValueCallback" /> delegate.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="key" /> or <paramref name="createValueCallback" /> is <see langword="null" />.</exception>
		public TValue GetValue(TKey key, ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback)
		{
			if (createValueCallback == null)
			{
				throw new ArgumentNullException("Null create delegate", "createValueCallback");
			}
			object @lock = this._lock;
			TValue tvalue;
			lock (@lock)
			{
				if (this.TryGetValue(key, out tvalue))
				{
					return tvalue;
				}
				tvalue = createValueCallback(key);
				this.Add(key, tvalue);
			}
			return tvalue;
		}

		internal TKey FindEquivalentKeyUnsafe(TKey key, out TValue value)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				for (int i = 0; i < this.data.Length; i++)
				{
					Ephemeron ephemeron = this.data[i];
					if (object.Equals(ephemeron.key, key))
					{
						value = (TValue)((object)ephemeron.value);
						return (TKey)((object)ephemeron.key);
					}
				}
			}
			value = default(TValue);
			return default(TKey);
		}

		[SecuritySafeCritical]
		public void Clear()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				for (int i = 0; i < this.data.Length; i++)
				{
					this.data[i].key = null;
					this.data[i].value = null;
				}
				this.size = 0;
			}
		}

		internal ICollection<TKey> Keys
		{
			[SecuritySafeCritical]
			get
			{
				object ephemeron_TOMBSTONE = GC.EPHEMERON_TOMBSTONE;
				List<TKey> list = new List<TKey>(this.data.Length);
				object @lock = this._lock;
				lock (@lock)
				{
					for (int i = 0; i < this.data.Length; i++)
					{
						TKey tkey = (TKey)((object)this.data[i].key);
						if (tkey != null && tkey != ephemeron_TOMBSTONE)
						{
							list.Add(tkey);
						}
					}
				}
				return list;
			}
		}

		internal ICollection<TValue> Values
		{
			[SecuritySafeCritical]
			get
			{
				object ephemeron_TOMBSTONE = GC.EPHEMERON_TOMBSTONE;
				List<TValue> list = new List<TValue>(this.data.Length);
				object @lock = this._lock;
				lock (@lock)
				{
					for (int i = 0; i < this.data.Length; i++)
					{
						Ephemeron ephemeron = this.data[i];
						if (ephemeron.key != null && ephemeron.key != ephemeron_TOMBSTONE)
						{
							list.Add((TValue)((object)ephemeron.value));
						}
					}
				}
				return list;
			}
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			object @lock = this._lock;
			IEnumerator<KeyValuePair<TKey, TValue>> enumerator;
			lock (@lock)
			{
				IEnumerator<KeyValuePair<TKey, TValue>> enumerator2;
				if (this.size != 0)
				{
					enumerator = new ConditionalWeakTable<TKey, TValue>.Enumerator(this);
					enumerator2 = enumerator;
				}
				else
				{
					enumerator2 = ((IEnumerable<KeyValuePair<TKey, TValue>>)Array.Empty<KeyValuePair<TKey, TValue>>()).GetEnumerator();
				}
				enumerator = enumerator2;
			}
			return enumerator;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
		}

		private const int INITIAL_SIZE = 13;

		private const float LOAD_FACTOR = 0.7f;

		private const float COMPACT_FACTOR = 0.5f;

		private const float EXPAND_FACTOR = 1.1f;

		private Ephemeron[] data;

		private object _lock = new object();

		private int size;

		/// <summary>Represents a method that creates a non-default value to add as part of a key/value pair to a <see cref="T:System.Runtime.CompilerServices.ConditionalWeakTable`2" /> object.</summary>
		/// <param name="key">The key that belongs to the value to create.</param>
		/// <typeparam name="TKey" />
		/// <typeparam name="TValue" />
		/// <returns>An instance of a reference type that represents the value to attach to the specified key.</returns>
		public delegate TValue CreateValueCallback(TKey key);

		private sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
		{
			public Enumerator(ConditionalWeakTable<TKey, TValue> table)
			{
				this._table = table;
				this._currentIndex = -1;
			}

			~Enumerator()
			{
				this.Dispose();
			}

			public void Dispose()
			{
				if (Interlocked.Exchange<ConditionalWeakTable<TKey, TValue>>(ref this._table, null) != null)
				{
					this._current = default(KeyValuePair<TKey, TValue>);
					GC.SuppressFinalize(this);
				}
			}

			public bool MoveNext()
			{
				ConditionalWeakTable<TKey, TValue> table = this._table;
				if (table != null)
				{
					object @lock = table._lock;
					lock (@lock)
					{
						object ephemeron_TOMBSTONE = GC.EPHEMERON_TOMBSTONE;
						while (this._currentIndex < table.data.Length - 1)
						{
							this._currentIndex++;
							Ephemeron ephemeron = table.data[this._currentIndex];
							if (ephemeron.key != null && ephemeron.key != ephemeron_TOMBSTONE)
							{
								this._current = new KeyValuePair<TKey, TValue>((TKey)((object)ephemeron.key), (TValue)((object)ephemeron.value));
								return true;
							}
						}
					}
					return false;
				}
				return false;
			}

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					if (this._currentIndex < 0)
					{
						ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
					}
					return this._current;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Reset()
			{
			}

			private ConditionalWeakTable<TKey, TValue> _table;

			private int _currentIndex = -1;

			private KeyValuePair<TKey, TValue> _current;
		}
	}
}
