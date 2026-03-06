using System;
using System.Collections.Generic;

namespace UnityEngine.ResourceManagement.Util
{
	internal struct LRUCache<TKey, TValue> where TKey : IEquatable<TKey>
	{
		public LRUCache(int limit)
		{
			this.requestHits = (this.requestCount = 0);
			this.entryLimit = limit;
			this.cache = new Dictionary<LRUCache<TKey, TValue>.Key, LRUCache<TKey, TValue>.Entry>(limit);
			this.lru = new LinkedList<LRUCache<TKey, TValue>.Key>();
		}

		public bool TryAdd(TKey id, TValue obj)
		{
			if (obj == null || this.entryLimit <= 0)
			{
				return false;
			}
			LRUCache<TKey, TValue>.Key key = new LRUCache<TKey, TValue>.Key(id, obj.GetType());
			LinkedListNode<LRUCache<TKey, TValue>.Key> linkedListNode = new LinkedListNode<LRUCache<TKey, TValue>.Key>(key);
			if (!this.cache.TryAdd(key, new LRUCache<TKey, TValue>.Entry
			{
				Value = obj,
				lruNode = linkedListNode
			}))
			{
				return false;
			}
			this.lru.AddFirst(linkedListNode);
			while (this.lru.Count > this.entryLimit)
			{
				this.cache.Remove(this.lru.Last.Value);
				LinkedListNode<LRUCache<TKey, TValue>.Key> last = this.lru.Last;
				this.lru.RemoveLast();
			}
			return true;
		}

		public bool TryGet(Type type, TKey id, out TValue val)
		{
			this.requestCount++;
			LRUCache<TKey, TValue>.Key key = new LRUCache<TKey, TValue>.Key(id, type);
			LRUCache<TKey, TValue>.Entry entry;
			if (this.cache.TryGetValue(key, out entry))
			{
				val = entry.Value;
				if (entry.lruNode.Previous != null)
				{
					this.lru.Remove(entry.lruNode);
					this.lru.AddFirst(entry.lruNode);
				}
				this.requestHits++;
				return true;
			}
			val = default(TValue);
			return false;
		}

		public int requestHits;

		public int requestCount;

		private int entryLimit;

		private Dictionary<LRUCache<TKey, TValue>.Key, LRUCache<TKey, TValue>.Entry> cache;

		private LinkedList<LRUCache<TKey, TValue>.Key> lru;

		public struct Key : IEquatable<LRUCache<TKey, TValue>.Key>
		{
			public Key(TKey k, Type t)
			{
				this.key = k;
				this.type = t;
				if (LRUCache<TKey, TValue>.Key.typeType.IsAssignableFrom(this.type))
				{
					this.type = LRUCache<TKey, TValue>.Key.typeType;
				}
			}

			bool IEquatable<LRUCache<!0, !1>.Key>.Equals(LRUCache<TKey, TValue>.Key other)
			{
				return this.key.Equals(other.key) && this.type == other.type;
			}

			public override int GetHashCode()
			{
				return this.key.GetHashCode() ^ this.type.GetHashCode();
			}

			private static Type typeType = typeof(Type);

			public TKey key;

			public Type type;
		}

		public struct Entry : IEquatable<LRUCache<TKey, TValue>.Entry>
		{
			public bool Equals(LRUCache<TKey, TValue>.Entry other)
			{
				return this.Value.Equals(other);
			}

			public override int GetHashCode()
			{
				return this.Value.GetHashCode();
			}

			public LinkedListNode<LRUCache<TKey, TValue>.Key> lruNode;

			public TValue Value;
		}
	}
}
