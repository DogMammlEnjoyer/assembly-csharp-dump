using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks.Internal
{
	internal class WeakDictionary<TKey, TValue> where TKey : class
	{
		public WeakDictionary(int capacity = 4, float loadFactor = 0.75f, IEqualityComparer<TKey> keyComparer = null)
		{
			int num = WeakDictionary<TKey, TValue>.CalculateCapacity(capacity, loadFactor);
			this.buckets = new WeakDictionary<TKey, TValue>.Entry[num];
			this.loadFactor = loadFactor;
			this.gate = new SpinLock(false);
			this.keyEqualityComparer = (keyComparer ?? EqualityComparer<TKey>.Default);
		}

		public bool TryAdd(TKey key, TValue value)
		{
			bool flag = false;
			bool result;
			try
			{
				this.gate.Enter(ref flag);
				result = this.TryAddInternal(key, value);
			}
			finally
			{
				if (flag)
				{
					this.gate.Exit(false);
				}
			}
			return result;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			bool flag = false;
			bool result;
			try
			{
				this.gate.Enter(ref flag);
				int num;
				WeakDictionary<TKey, TValue>.Entry entry;
				if (this.TryGetEntry(key, out num, out entry))
				{
					value = entry.Value;
					result = true;
				}
				else
				{
					value = default(TValue);
					result = false;
				}
			}
			finally
			{
				if (flag)
				{
					this.gate.Exit(false);
				}
			}
			return result;
		}

		public bool TryRemove(TKey key)
		{
			bool flag = false;
			bool result;
			try
			{
				this.gate.Enter(ref flag);
				int hashIndex;
				WeakDictionary<TKey, TValue>.Entry entry;
				if (this.TryGetEntry(key, out hashIndex, out entry))
				{
					this.Remove(hashIndex, entry);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				if (flag)
				{
					this.gate.Exit(false);
				}
			}
			return result;
		}

		private bool TryAddInternal(TKey key, TValue value)
		{
			int num = WeakDictionary<TKey, TValue>.CalculateCapacity(this.size + 1, this.loadFactor);
			while (this.buckets.Length < num)
			{
				WeakDictionary<TKey, TValue>.Entry[] targetBuckets = new WeakDictionary<TKey, TValue>.Entry[num];
				for (int i = 0; i < this.buckets.Length; i++)
				{
					for (WeakDictionary<TKey, TValue>.Entry entry = this.buckets[i]; entry != null; entry = entry.Next)
					{
						this.AddToBuckets(targetBuckets, key, entry.Value, entry.Hash);
					}
				}
				this.buckets = targetBuckets;
			}
			bool flag = this.AddToBuckets(this.buckets, key, value, this.keyEqualityComparer.GetHashCode(key));
			if (flag)
			{
				this.size++;
			}
			return flag;
		}

		private bool AddToBuckets(WeakDictionary<TKey, TValue>.Entry[] targetBuckets, TKey newKey, TValue value, int keyHash)
		{
			int num = keyHash & targetBuckets.Length - 1;
			IL_0B:
			while (targetBuckets[num] != null)
			{
				WeakDictionary<TKey, TValue>.Entry entry = targetBuckets[num];
				while (entry != null)
				{
					TKey y;
					if (entry.Key.TryGetTarget(out y))
					{
						if (this.keyEqualityComparer.Equals(newKey, y))
						{
							return false;
						}
					}
					else
					{
						this.Remove(num, entry);
						if (targetBuckets[num] == null)
						{
							goto IL_0B;
						}
					}
					if (entry.Next != null)
					{
						entry = entry.Next;
					}
					else
					{
						entry.Next = new WeakDictionary<TKey, TValue>.Entry
						{
							Key = new WeakReference<TKey>(newKey, false),
							Value = value,
							Hash = keyHash
						};
						entry.Next.Prev = entry;
					}
				}
				return false;
			}
			targetBuckets[num] = new WeakDictionary<TKey, TValue>.Entry
			{
				Key = new WeakReference<TKey>(newKey, false),
				Value = value,
				Hash = keyHash
			};
			return true;
		}

		private bool TryGetEntry(TKey key, out int hashIndex, out WeakDictionary<TKey, TValue>.Entry entry)
		{
			WeakDictionary<TKey, TValue>.Entry[] array = this.buckets;
			int hashCode = this.keyEqualityComparer.GetHashCode(key);
			hashIndex = (hashCode & array.Length - 1);
			for (entry = array[hashIndex]; entry != null; entry = entry.Next)
			{
				TKey y;
				if (entry.Key.TryGetTarget(out y))
				{
					if (this.keyEqualityComparer.Equals(key, y))
					{
						return true;
					}
				}
				else
				{
					this.Remove(hashIndex, entry);
				}
			}
			return false;
		}

		private void Remove(int hashIndex, WeakDictionary<TKey, TValue>.Entry entry)
		{
			if (entry.Prev == null && entry.Next == null)
			{
				this.buckets[hashIndex] = null;
			}
			else
			{
				if (entry.Prev == null)
				{
					this.buckets[hashIndex] = entry.Next;
				}
				if (entry.Prev != null)
				{
					entry.Prev.Next = entry.Next;
				}
				if (entry.Next != null)
				{
					entry.Next.Prev = entry.Prev;
				}
			}
			this.size--;
		}

		public List<KeyValuePair<TKey, TValue>> ToList()
		{
			List<KeyValuePair<TKey, TValue>> result = new List<KeyValuePair<TKey, TValue>>(this.size);
			this.ToList(ref result, false);
			return result;
		}

		public int ToList(ref List<KeyValuePair<TKey, TValue>> list, bool clear = true)
		{
			if (clear)
			{
				list.Clear();
			}
			int num = 0;
			bool flag = false;
			try
			{
				for (int i = 0; i < this.buckets.Length; i++)
				{
					for (WeakDictionary<TKey, TValue>.Entry entry = this.buckets[i]; entry != null; entry = entry.Next)
					{
						TKey key;
						if (entry.Key.TryGetTarget(out key))
						{
							KeyValuePair<TKey, TValue> keyValuePair = new KeyValuePair<TKey, TValue>(key, entry.Value);
							if (num < list.Count)
							{
								list[num++] = keyValuePair;
							}
							else
							{
								list.Add(keyValuePair);
								num++;
							}
						}
						else
						{
							this.Remove(i, entry);
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					this.gate.Exit(false);
				}
			}
			return num;
		}

		private static int CalculateCapacity(int collectionSize, float loadFactor)
		{
			int num = (int)((float)collectionSize / loadFactor);
			num--;
			num |= num >> 1;
			num |= num >> 2;
			num |= num >> 4;
			num |= num >> 8;
			num |= num >> 16;
			num++;
			if (num < 8)
			{
				num = 8;
			}
			return num;
		}

		private WeakDictionary<TKey, TValue>.Entry[] buckets;

		private int size;

		private SpinLock gate;

		private readonly float loadFactor;

		private readonly IEqualityComparer<TKey> keyEqualityComparer;

		private class Entry
		{
			public override string ToString()
			{
				TKey tkey;
				if (this.Key.TryGetTarget(out tkey))
				{
					TKey tkey2 = tkey;
					return ((tkey2 != null) ? tkey2.ToString() : null) + "(" + this.Count().ToString() + ")";
				}
				return "(Dead)";
			}

			private int Count()
			{
				int num = 1;
				WeakDictionary<TKey, TValue>.Entry entry = this;
				while (entry.Next != null)
				{
					num++;
					entry = entry.Next;
				}
				return num;
			}

			public WeakReference<TKey> Key;

			public TValue Value;

			public int Hash;

			public WeakDictionary<TKey, TValue>.Entry Prev;

			public WeakDictionary<TKey, TValue>.Entry Next;
		}
	}
}
