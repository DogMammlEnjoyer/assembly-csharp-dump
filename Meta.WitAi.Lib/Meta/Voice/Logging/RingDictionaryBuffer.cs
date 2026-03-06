using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.Voice.Logging
{
	internal class RingDictionaryBuffer<TKey, TValue>
	{
		public RingDictionaryBuffer(int capacity)
		{
			this._capacity = capacity;
		}

		public ICollection<TValue> this[TKey key]
		{
			get
			{
				return this._dictionary[key];
			}
		}

		public bool Add(TKey key, TValue value, bool unique = false)
		{
			LinkedList<TValue> linkedList;
			if (!this._dictionary.TryGetValue(key, out linkedList))
			{
				linkedList = new LinkedList<TValue>();
				this._dictionary[key] = linkedList;
			}
			object obj;
			if (!this._valueLocks.TryGetValue(key, out obj))
			{
				obj = new object();
				this._valueLocks[key] = obj;
			}
			bool result = true;
			object obj2 = obj;
			lock (obj2)
			{
				if (unique && linkedList.Contains(value))
				{
					result = false;
					linkedList.Remove(value);
				}
				linkedList.AddFirst(value);
				while (linkedList.Count > Mathf.Max(0, this._capacity))
				{
					linkedList.RemoveLast();
				}
			}
			return result;
		}

		public bool ContainsKey(TKey key)
		{
			return this._dictionary.ContainsKey(key);
		}

		public IEnumerable<TValue> Extract(TKey key)
		{
			object obj;
			this._valueLocks.TryRemove(key, out obj);
			LinkedList<TValue> result;
			if (!this._dictionary.TryRemove(key, out result))
			{
				return new TValue[0];
			}
			return result;
		}

		public IEnumerable<TValue> ExtractAll()
		{
			List<TValue> list = new List<TValue>();
			foreach (TKey key in new List<TKey>(this._dictionary.Keys))
			{
				list.AddRange(this.Extract(key));
			}
			return list;
		}

		public void Clear()
		{
			this._dictionary.Clear();
			this._valueLocks.Clear();
		}

		private readonly int _capacity;

		private readonly ConcurrentDictionary<TKey, LinkedList<TValue>> _dictionary = new ConcurrentDictionary<TKey, LinkedList<TValue>>();

		private readonly ConcurrentDictionary<TKey, object> _valueLocks = new ConcurrentDictionary<TKey, object>();
	}
}
