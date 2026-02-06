using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public readonly ref struct NetworkDictionaryReadOnly<K, V>
	{
		private unsafe int _free
		{
			get
			{
				return *this._data;
			}
		}

		private unsafe int _freeCount
		{
			get
			{
				return this._data[1];
			}
		}

		private unsafe int _usedCount
		{
			get
			{
				return this._data[2];
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

		internal unsafe NetworkDictionaryReadOnly(int* data, int capacity, IElementReaderWriter<K> keyReaderWriter, IElementReaderWriter<V> valReaderWriter)
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

		private unsafe int Find(K key)
		{
			Assert.Check(this._capacity > 0);
			int* ptr = this._data + this._bucketsOffset;
			uint bucketFromHashCode = this.GetBucketFromHashCode(key.GetHashCode());
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
		private unsafe K GetKey(int entry)
		{
			return this._keyReaderWriter.Read((byte*)(this._data + this._entriesOffset + (this._entryStride * entry + this._keyOffset)), 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe V GetVal(int entry)
		{
			return this._valReaderWriter.Read((byte*)(this._data + this._entriesOffset + (this._entryStride * entry + this._valOffset)), 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe int GetNxt(int entry)
		{
			return (this._data + this._entriesOffset)[this._entryStride * entry + this._nxtOffset];
		}

		private const int INVALID_ENTRY = 0;

		private const int FREE_OFFSET = 0;

		private const int FREE_COUNT_OFFSET = 1;

		private const int USED_COUNT_OFFSET = 2;

		private unsafe readonly int* _data;

		private readonly int _capacity;

		private readonly int _nxtOffset;

		private readonly int _keyOffset;

		private readonly int _valOffset;

		private readonly int _entryStride;

		private readonly int _bucketsOffset;

		private readonly int _entriesOffset;

		private readonly IElementReaderWriter<K> _keyReaderWriter;

		private readonly IElementReaderWriter<V> _valReaderWriter;

		private readonly EqualityComparer<K> _equalityComparer;
	}
}
