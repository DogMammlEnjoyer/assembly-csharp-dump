using System;
using System.Collections;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class NonAllocDictionary<K, V> : IDictionary<K, V>, ICollection<KeyValuePair<K, V>>, IEnumerable<KeyValuePair<K, V>>, IEnumerable where K : IEquatable<K>
	{
		public NonAllocDictionary<K, V>.KeyIterator Keys
		{
			get
			{
				return new NonAllocDictionary<K, V>.KeyIterator(this);
			}
		}

		ICollection<V> IDictionary<!0, !1>.Values
		{
			get
			{
				return this.values;
			}
		}

		ICollection<K> IDictionary<!0, !1>.Keys
		{
			get
			{
				return this.keys;
			}
		}

		public NonAllocDictionary<K, V>.ValueIterator Values
		{
			get
			{
				return new NonAllocDictionary<K, V>.ValueIterator(this);
			}
		}

		public int Count
		{
			get
			{
				return this._usedCount - this._freeCount - 1;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		public uint Capacity
		{
			get
			{
				return this._capacity;
			}
		}

		public NonAllocDictionary(uint capacity = 29U)
		{
			this._capacity = (NonAllocDictionary<K, V>.IsPrimeFromList(capacity) ? capacity : NonAllocDictionary<K, V>.GetNextPrime(capacity));
			this._usedCount = 1;
			this._buckets = new int[this._capacity];
			this._nodes = new NonAllocDictionary<K, V>.Node[this._capacity];
		}

		public bool ContainsKey(K key)
		{
			return this.FindNode(key) != 0;
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			int num = this.FindNode(item.Key);
			return num >= 0 && EqualityComparer<V>.Default.Equals(this._nodes[num].Val, item.Value);
		}

		public bool TryGetValue(K key, out V val)
		{
			int num = this.FindNode(key);
			bool flag = num != 0;
			bool result;
			if (flag)
			{
				val = this._nodes[num].Val;
				result = true;
			}
			else
			{
				val = default(V);
				result = false;
			}
			return result;
		}

		public V this[K key]
		{
			get
			{
				int num = this.FindNode(key);
				bool flag = num != 0;
				if (flag)
				{
					return this._nodes[num].Val;
				}
				string str = "Key does not exist: ";
				K k = key;
				throw new InvalidOperationException(str + ((k != null) ? k.ToString() : null));
			}
			set
			{
				int num = this.FindNode(key);
				bool flag = num == 0;
				if (flag)
				{
					this.Insert(key, value);
				}
				else
				{
					NonAllocDictionary<K, V>.Assert(this._nodes[num].Key.Equals(key));
					this._nodes[num].Val = value;
				}
			}
		}

		public void Set(K key, V val)
		{
			int num = this.FindNode(key);
			bool flag = num == 0;
			if (flag)
			{
				this.Insert(key, val);
			}
			else
			{
				NonAllocDictionary<K, V>.Assert(this._nodes[num].Key.Equals(key));
				this._nodes[num].Val = val;
			}
		}

		public void Add(K key, V val)
		{
			int num = this.FindNode(key);
			bool flag = num == 0;
			if (flag)
			{
				this.Insert(key, val);
				return;
			}
			string str = "Duplicate key ";
			K k = key;
			throw new InvalidOperationException(str + ((k != null) ? k.ToString() : null));
		}

		public void Add(KeyValuePair<K, V> item)
		{
			int num = this.FindNode(item.Key);
			bool flag = num == 0;
			if (flag)
			{
				this.Insert(item.Key, item.Value);
				return;
			}
			string str = "Duplicate key ";
			K key = item.Key;
			throw new InvalidOperationException(str + ((key != null) ? key.ToString() : null));
		}

		public bool Remove(K key)
		{
			uint hashCode = (uint)key.GetHashCode();
			int i = this._buckets[(int)(hashCode % this._capacity)];
			int num = 0;
			while (i != 0)
			{
				bool flag = this._nodes[i].Hash == hashCode && this._nodes[i].Key.Equals(key);
				if (flag)
				{
					bool flag2 = num == 0;
					if (flag2)
					{
						this._buckets[(int)(hashCode % this._capacity)] = this._nodes[i].Next;
					}
					else
					{
						this._nodes[num].Next = this._nodes[i].Next;
					}
					this._nodes[i].Used = false;
					this._nodes[i].Next = this._freeHead;
					this._nodes[i].Val = default(V);
					this._freeHead = i;
					this._freeCount++;
					return true;
				}
				num = i;
				i = this._nodes[i].Next;
			}
			return false;
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			bool flag = this.Contains(item);
			return flag && this.Remove(item.Key);
		}

		IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<!0, !1>>.GetEnumerator()
		{
			return new NonAllocDictionary<K, V>.PairIterator(this);
		}

		public NonAllocDictionary<K, V>.PairIterator GetEnumerator()
		{
			return new NonAllocDictionary<K, V>.PairIterator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new NonAllocDictionary<K, V>.PairIterator(this);
		}

		private int FindNode(K key)
		{
			uint hashCode = (uint)key.GetHashCode();
			for (int i = this._buckets[(int)(hashCode % this._capacity)]; i != 0; i = this._nodes[i].Next)
			{
				bool flag = this._nodes[i].Hash == hashCode && this._nodes[i].Key.Equals(key);
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		private void Insert(K key, V val)
		{
			bool flag = this._freeCount > 0;
			int num;
			if (flag)
			{
				num = this._freeHead;
				this._freeHead = this._nodes[num].Next;
				this._freeCount--;
			}
			else
			{
				bool flag2 = (long)this._usedCount == (long)((ulong)this._capacity);
				if (flag2)
				{
					this.Expand();
				}
				int usedCount = this._usedCount;
				this._usedCount = usedCount + 1;
				num = usedCount;
			}
			uint hashCode = (uint)key.GetHashCode();
			uint num2 = hashCode % this._capacity;
			this._nodes[num].Used = true;
			this._nodes[num].Hash = hashCode;
			this._nodes[num].Next = this._buckets[(int)num2];
			this._nodes[num].Key = key;
			this._nodes[num].Val = val;
			this._buckets[(int)num2] = num;
		}

		private void Expand()
		{
			NonAllocDictionary<K, V>.Assert(this._buckets.Length == this._usedCount);
			uint nextPrime = NonAllocDictionary<K, V>.GetNextPrime(this._capacity);
			NonAllocDictionary<K, V>.Assert(nextPrime > this._capacity);
			int[] array = new int[nextPrime];
			NonAllocDictionary<K, V>.Node[] array2 = new NonAllocDictionary<K, V>.Node[nextPrime];
			Array.Copy(this._nodes, 0, array2, 0, this._nodes.Length);
			for (int i = 1; i < this._nodes.Length; i++)
			{
				NonAllocDictionary<K, V>.Assert(array2[i].Used);
				uint num = array2[i].Hash % nextPrime;
				array2[i].Next = array[(int)num];
				array[(int)num] = i;
			}
			this._nodes = array2;
			this._buckets = array;
			this._capacity = nextPrime;
		}

		public void Clear()
		{
			bool flag = this._usedCount > 1;
			if (flag)
			{
				Array.Clear(this._nodes, 0, this._nodes.Length);
				Array.Clear(this._buckets, 0, this._buckets.Length);
				this._freeHead = 0;
				this._freeCount = 0;
				this._usedCount = 1;
			}
		}

		void ICollection<KeyValuePair<!0, !1>>.CopyTo(KeyValuePair<K, V>[] array, int index)
		{
			bool flag = array == null;
			if (flag)
			{
				throw new ArgumentNullException("array");
			}
			bool flag2 = index < 0 || index > array.Length;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException();
			}
			bool flag3 = array.Length - index < this.Count;
			if (flag3)
			{
				throw new ArgumentException("Array plus offset are too small to fit all items in.");
			}
			for (int i = 1; i < this._nodes.Length; i++)
			{
				bool used = this._nodes[i].Used;
				if (used)
				{
					array[index++] = new KeyValuePair<K, V>(this._nodes[i].Key, this._nodes[i].Val);
				}
			}
		}

		private static bool IsPrimeFromList(uint value)
		{
			for (int i = 0; i < NonAllocDictionary<K, V>._primeTableUInt.Length; i++)
			{
				bool flag = NonAllocDictionary<K, V>._primeTableUInt[i] == value;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		private static uint GetNextPrime(uint value)
		{
			for (int i = 0; i < NonAllocDictionary<K, V>._primeTableUInt.Length; i++)
			{
				bool flag = NonAllocDictionary<K, V>._primeTableUInt[i] > value;
				if (flag)
				{
					return NonAllocDictionary<K, V>._primeTableUInt[i];
				}
			}
			throw new InvalidOperationException("NonAllocDictionary can't get larger than" + NonAllocDictionary<K, V>._primeTableUInt[NonAllocDictionary<K, V>._primeTableUInt.Length - 1].ToString());
		}

		private static void Assert(bool condition)
		{
			bool flag = !condition;
			if (flag)
			{
				throw new InvalidOperationException("Assert Failed");
			}
		}

		private static uint[] _primeTableUInt = new uint[]
		{
			3U,
			7U,
			17U,
			29U,
			53U,
			97U,
			193U,
			389U,
			769U,
			1543U,
			3079U,
			6151U,
			12289U,
			24593U,
			49157U,
			98317U,
			196613U,
			393241U,
			786433U,
			1572869U,
			3145739U,
			6291469U,
			12582917U,
			25165843U,
			50331653U,
			100663319U,
			201326611U,
			402653189U,
			805306457U,
			1610612741U
		};

		private int _freeHead;

		private int _freeCount;

		private int _usedCount;

		private uint _capacity;

		private int[] _buckets;

		private NonAllocDictionary<K, V>.Node[] _nodes;

		private bool isReadOnly;

		private ICollection<K> keys;

		private ICollection<V> values;

		public struct KeyIterator : IEnumerator<K>, IEnumerator, IDisposable
		{
			public KeyIterator(NonAllocDictionary<K, V> dictionary)
			{
				this._index = 0;
				this._dict = dictionary;
			}

			public NonAllocDictionary<K, V>.KeyIterator GetEnumerator()
			{
				return this;
			}

			public void Reset()
			{
				this._index = 0;
			}

			object IEnumerator.Current
			{
				get
				{
					bool flag = this._index == 0;
					if (flag)
					{
						throw new InvalidOperationException();
					}
					return this._dict._nodes[this._index].Key;
				}
			}

			public K Current
			{
				get
				{
					bool flag = this._index == 0;
					K result;
					if (flag)
					{
						result = default(K);
					}
					else
					{
						result = this._dict._nodes[this._index].Key;
					}
					return result;
				}
			}

			public bool MoveNext()
			{
				bool used;
				do
				{
					int num = this._index + 1;
					this._index = num;
					if (num >= this._dict._usedCount)
					{
						goto Block_2;
					}
					used = this._dict._nodes[this._index].Used;
				}
				while (!used);
				return true;
				Block_2:
				this._index = 0;
				return false;
			}

			public void Dispose()
			{
			}

			private int _index;

			private NonAllocDictionary<K, V> _dict;
		}

		public struct ValueIterator : IEnumerator<V>, IEnumerator, IDisposable
		{
			public ValueIterator(NonAllocDictionary<K, V> dictionary)
			{
				this._index = 0;
				this._dict = dictionary;
			}

			public NonAllocDictionary<K, V>.ValueIterator GetEnumerator()
			{
				return this;
			}

			public void Reset()
			{
				this._index = 0;
			}

			public V Current
			{
				get
				{
					bool flag = this._index == 0;
					V result;
					if (flag)
					{
						result = default(V);
					}
					else
					{
						result = this._dict._nodes[this._index].Val;
					}
					return result;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					bool flag = this._index == 0;
					if (flag)
					{
						throw new InvalidOperationException();
					}
					return this._dict._nodes[this._index].Val;
				}
			}

			public bool MoveNext()
			{
				bool used;
				do
				{
					int num = this._index + 1;
					this._index = num;
					if (num >= this._dict._usedCount)
					{
						goto Block_2;
					}
					used = this._dict._nodes[this._index].Used;
				}
				while (!used);
				return true;
				Block_2:
				this._index = 0;
				return false;
			}

			public void Dispose()
			{
			}

			private int _index;

			private NonAllocDictionary<K, V> _dict;
		}

		public struct PairIterator : IEnumerator<KeyValuePair<K, V>>, IEnumerator, IDisposable
		{
			public PairIterator(NonAllocDictionary<K, V> dictionary)
			{
				this._index = 0;
				this._dict = dictionary;
			}

			public void Reset()
			{
				this._index = 0;
			}

			object IEnumerator.Current
			{
				get
				{
					bool flag = this._index == 0;
					if (flag)
					{
						throw new InvalidOperationException();
					}
					return this.Current;
				}
			}

			public KeyValuePair<K, V> Current
			{
				get
				{
					bool flag = this._index == 0;
					KeyValuePair<K, V> result;
					if (flag)
					{
						result = default(KeyValuePair<K, V>);
					}
					else
					{
						result = new KeyValuePair<K, V>(this._dict._nodes[this._index].Key, this._dict._nodes[this._index].Val);
					}
					return result;
				}
			}

			public bool MoveNext()
			{
				bool used;
				do
				{
					int num = this._index + 1;
					this._index = num;
					if (num >= this._dict._usedCount)
					{
						goto Block_2;
					}
					used = this._dict._nodes[this._index].Used;
				}
				while (!used);
				return true;
				Block_2:
				this._index = 0;
				return false;
			}

			public void Dispose()
			{
			}

			private int _index;

			private NonAllocDictionary<K, V> _dict;
		}

		private struct Node
		{
			public bool Used;

			public int Next;

			public uint Hash;

			public K Key;

			public V Val;
		}
	}
}
