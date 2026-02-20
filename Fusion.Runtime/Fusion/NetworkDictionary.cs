using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fusion
{
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(NetworkDictionary<, >.DebuggerProxy))]
	public struct NetworkDictionary<K, V> : IEnumerable<KeyValuePair<K, V>>, IEnumerable, INetworkDictionary
	{
		private unsafe int _free
		{
			get
			{
				return *this._data;
			}
			set
			{
				*this._data = value;
			}
		}

		private unsafe int _freeCount
		{
			get
			{
				return this._data[1];
			}
			set
			{
				this._data[1] = value;
			}
		}

		private unsafe int _usedCount
		{
			get
			{
				return this._data[2];
			}
			set
			{
				this._data[2] = value;
			}
		}

		public unsafe int Count
		{
			get
			{
				Assert.Check((void*)this._data);
				return this._usedCount - this._freeCount - 1;
			}
		}

		public int Capacity
		{
			get
			{
				return this._capacity - 1;
			}
		}

		public V this[K key]
		{
			get
			{
				return this.Get(key);
			}
			set
			{
				this.Set(key, value);
			}
		}

		public unsafe NetworkDictionary(int* data, int capacity, IElementReaderWriter<K> keyReaderWriter, IElementReaderWriter<V> valReaderWriter)
		{
			Assert.Check<int>(Primes.IsPrime(capacity), "Capacity not prime {0}", capacity);
			int elementWordCount = keyReaderWriter.GetElementWordCount();
			int elementWordCount2 = valReaderWriter.GetElementWordCount();
			this._keyReaderWriter = keyReaderWriter;
			this._valReaderWriter = valReaderWriter;
			this._data = data;
			this._capacity = capacity;
			this._nxtOffset = 0;
			this._keyOffset = 1;
			this._valOffset = 1 + elementWordCount;
			this._entryStride = 1 + elementWordCount + elementWordCount2;
			this._bucketsOffset = 3;
			this._entriesOffset = this._bucketsOffset + this._capacity;
			this._equalityComparer = EqualityComparer<K>.Default;
			bool flag = this._usedCount == 0;
			if (flag)
			{
				this._usedCount = 1;
			}
		}

		public NetworkDictionaryReadOnly<K, V> ToReadOnly()
		{
			return new NetworkDictionaryReadOnly<K, V>(this._data, this._capacity, this._keyReaderWriter, this._valReaderWriter);
		}

		public unsafe void Clear()
		{
			Assert.Check((void*)this._data);
			this._usedCount = 1;
			this._free = 0;
			this._freeCount = 0;
			Native.MemClear((void*)(this._data + this._bucketsOffset), this._capacity * 4);
		}

		public bool ContainsKey(K key)
		{
			return this.Find(key) != 0;
		}

		public bool ContainsValue(V value, IEqualityComparer<V> equalityComparer = null)
		{
			NetworkDictionary<K, V>.Enumerator enumerator = this.GetEnumerator();
			bool flag = equalityComparer == null;
			if (flag)
			{
				equalityComparer = EqualityComparer<V>.Default;
			}
			while (enumerator.MoveNext())
			{
				IEqualityComparer<V> equalityComparer2 = equalityComparer;
				KeyValuePair<K, V> keyValuePair = enumerator.Current;
				bool flag2 = equalityComparer2.Equals(keyValuePair.Value, value);
				if (flag2)
				{
					return true;
				}
			}
			enumerator.Dispose();
			return false;
		}

		public V Get(K key)
		{
			V result;
			bool flag = this.TryGet(key, out result);
			if (flag)
			{
				return result;
			}
			throw new KeyNotFoundException();
		}

		public V Set(K key, V value)
		{
			int num = this.Find(key);
			bool flag = num == 0;
			if (flag)
			{
				this.Insert(key, value);
			}
			else
			{
				this.SetVal(num, value);
			}
			return value;
		}

		public unsafe bool Add(K key, V value)
		{
			Assert.Check((void*)this._data);
			int num = this.Find(key);
			bool flag = num == 0;
			bool result;
			if (flag)
			{
				this.Insert(key, value);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public unsafe bool TryGet(K key, out V value)
		{
			Assert.Check((void*)this._data);
			int num = this.Find(key);
			bool flag = num != 0;
			bool result;
			if (flag)
			{
				value = this.GetVal(num);
				result = true;
			}
			else
			{
				value = default(V);
				result = false;
			}
			return result;
		}

		public bool Remove(K key)
		{
			V v;
			return this.Remove(key, out v);
		}

		public unsafe bool Remove(K key, out V value)
		{
			Assert.Check((void*)this._data);
			int* ptr = this._data + this._bucketsOffset;
			uint bucketFromHashCode = this.GetBucketFromHashCode(this.GetKeyHashCode(key));
			int i = ptr[(ulong)bucketFromHashCode * 4UL / 4UL];
			int num = 0;
			while (i != 0)
			{
				bool flag = this._equalityComparer.Equals(this.GetKey(i), key);
				if (flag)
				{
					bool flag2 = num == 0;
					if (flag2)
					{
						ptr[(ulong)bucketFromHashCode * 4UL / 4UL] = this.GetNxt(i);
					}
					else
					{
						this.SetNxt(num, this.GetNxt(i));
					}
					value = this.GetVal(i);
					this.SetNxt(i, this._free);
					this._free = i;
					this._freeCount++;
					return true;
				}
				num = i;
				i = this.GetNxt(i);
			}
			value = default(V);
			return false;
		}

		private unsafe int Insert(K key, V val)
		{
			bool flag = this._free != 0;
			int num;
			if (flag)
			{
				Assert.Check(this._freeCount > 0);
				num = this._free;
				this._free = this.GetNxt(num);
				this._freeCount--;
			}
			else
			{
				bool flag2 = this._usedCount == this._capacity;
				if (flag2)
				{
					Assert.AlwaysFail("networked dictionary is full");
				}
				Assert.Check(this._usedCount < this._capacity);
				int usedCount = this._usedCount;
				this._usedCount = usedCount + 1;
				num = usedCount;
			}
			int* ptr = this._data + this._bucketsOffset;
			uint bucketFromHashCode = this.GetBucketFromHashCode(this.GetKeyHashCode(key));
			this.SetKey(num, key);
			this.SetVal(num, val);
			this.SetNxt(num, ptr[(ulong)bucketFromHashCode * 4UL / 4UL]);
			ptr[(ulong)bucketFromHashCode * 4UL / 4UL] = num;
			return num;
		}

		private unsafe int Find(K key)
		{
			Assert.Check(this._capacity > 0);
			int* ptr = this._data + this._bucketsOffset;
			uint bucketFromHashCode = this.GetBucketFromHashCode(this.GetKeyHashCode(key));
			for (int i = ptr[(ulong)bucketFromHashCode * 4UL / 4UL]; i != 0; i = this.GetNxt(i))
			{
				bool flag = this._equalityComparer.Equals(this.GetKey(i), key);
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private uint GetBucketFromHashCode(int hash)
		{
			return (uint)(hash % this._capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void ClrEntry(int entry)
		{
			Native.MemClear((void*)(this._data + this._entriesOffset + this._entryStride * entry), this._entryStride * 4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe K GetKey(int entry)
		{
			return this._keyReaderWriter.Read((byte*)(this._data + this._entriesOffset + (this._entryStride * entry + this._keyOffset)), 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void SetKey(int entry, K key)
		{
			this._keyReaderWriter.Write((byte*)(this._data + this._entriesOffset + (this._entryStride * entry + this._keyOffset)), 0, key);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe V GetVal(int entry)
		{
			return this._valReaderWriter.Read((byte*)(this._data + this._entriesOffset + (this._entryStride * entry + this._valOffset)), 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void SetVal(int entry, V val)
		{
			this._valReaderWriter.Write((byte*)(this._data + this._entriesOffset + (this._entryStride * entry + this._valOffset)), 0, val);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe int GetNxt(int entry)
		{
			return (this._data + this._entriesOffset)[this._entryStride * entry + this._nxtOffset];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void SetNxt(int entry, int next)
		{
			(this._data + this._entriesOffset)[this._entryStride * entry + this._nxtOffset] = next;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetKeyHashCode(K key)
		{
			return this._keyReaderWriter.GetElementHashCode(key);
		}

		public NetworkDictionary<K, V>.Enumerator GetEnumerator()
		{
			return new NetworkDictionary<K, V>.Enumerator(this);
		}

		IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void INetworkDictionary.Add(object item)
		{
			KeyValuePair<K, V> keyValuePair = (KeyValuePair<K, V>)item;
			this.Add(keyValuePair.Key, keyValuePair.Value);
		}

		public static implicit operator NetworkDictionaryReadOnly<K, V>(NetworkDictionary<K, V> value)
		{
			return new NetworkDictionaryReadOnly<K, V>(value._data, value.Capacity, value._keyReaderWriter, value._valReaderWriter);
		}

		public const int META_WORD_COUNT = 3;

		private const int FREE_OFFSET = 0;

		private const int FREE_COUNT_OFFSET = 1;

		private const int USED_COUNT_OFFSET = 2;

		private const int INVALID_ENTRY = 0;

		private unsafe int* _data;

		private int _capacity;

		private int _nxtOffset;

		private int _keyOffset;

		private int _valOffset;

		private int _entryStride;

		private int _bucketsOffset;

		private int _entriesOffset;

		private IElementReaderWriter<K> _keyReaderWriter;

		private IElementReaderWriter<V> _valReaderWriter;

		private EqualityComparer<K> _equalityComparer;

		internal class DebuggerProxy : Dictionary<K, V>
		{
			public DebuggerProxy(NetworkDictionary<K, V> dict)
			{
				this._items = new Lazy<KeyValuePair<K, V>[]>(() => (dict._data == null) ? Array.Empty<KeyValuePair<K, V>>() : dict.ToArray<KeyValuePair<K, V>>());
			}

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public KeyValuePair<K, V>[] Items
			{
				get
				{
					return this._items.Value;
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public Lazy<KeyValuePair<K, V>[]> _items;
		}

		public struct Enumerator : IEnumerator<KeyValuePair<K, V>>, IEnumerator, IDisposable
		{
			internal Enumerator(NetworkDictionary<K, V> dict)
			{
				this._dict = dict;
				this._entry = 0;
				this._bucket = -1;
			}

			public unsafe bool MoveNext()
			{
				for (;;)
				{
					bool flag = this._entry == 0;
					if (flag)
					{
						bool flag2 = this._bucket + 1 < this._dict._capacity;
						if (!flag2)
						{
							goto IL_77;
						}
						this._bucket++;
						this._entry = (this._dict._data + this._dict._bucketsOffset)[this._bucket];
						bool flag3 = this._entry == 0;
						if (!flag3)
						{
							break;
						}
					}
					else
					{
						this._entry = this._dict.GetNxt(this._entry);
						bool flag4 = this._entry == 0;
						if (!flag4)
						{
							goto IL_A7;
						}
					}
				}
				return true;
				IL_77:
				return false;
				IL_A7:
				return true;
			}

			public void Reset()
			{
				this._bucket = -1;
				this._entry = 0;
			}

			public KeyValuePair<K, V> Current
			{
				get
				{
					bool flag = this._entry > 0 && this._entry < this._dict._capacity;
					if (flag)
					{
						return new KeyValuePair<K, V>(this._dict.GetKey(this._entry), this._dict.GetVal(this._entry));
					}
					throw new InvalidOperationException();
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
				this._dict = default(NetworkDictionary<K, V>);
				this._entry = -1;
				this._bucket = -1;
			}

			private int _bucket;

			private int _entry;

			private NetworkDictionary<K, V> _dict;
		}
	}
}
