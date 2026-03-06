using System;
using System.Collections.Generic;

namespace UnityEngine.ResourceManagement.Util
{
	public class LRUCacheAllocationStrategy : IAllocationStrategy
	{
		public LRUCacheAllocationStrategy(int poolMaxSize, int poolCapacity, int poolCacheMaxSize, int initialPoolCacheCapacity)
		{
			this.m_poolMaxSize = poolMaxSize;
			this.m_poolInitialCapacity = poolCapacity;
			this.m_poolCacheMaxSize = poolCacheMaxSize;
			for (int i = 0; i < initialPoolCacheCapacity; i++)
			{
				this.m_poolCache.Add(new List<object>(this.m_poolInitialCapacity));
			}
		}

		private List<object> GetPool()
		{
			int count = this.m_poolCache.Count;
			if (count == 0)
			{
				return new List<object>(this.m_poolInitialCapacity);
			}
			List<object> result = this.m_poolCache[count - 1];
			this.m_poolCache.RemoveAt(count - 1);
			return result;
		}

		private void ReleasePool(List<object> pool)
		{
			if (this.m_poolCache.Count < this.m_poolCacheMaxSize)
			{
				this.m_poolCache.Add(pool);
			}
		}

		public object New(Type type, int typeHash)
		{
			List<object> list;
			if (this.m_cache.TryGetValue(typeHash, out list))
			{
				int count = list.Count;
				object result = list[count - 1];
				list.RemoveAt(count - 1);
				if (count == 1)
				{
					this.m_cache.Remove(typeHash);
					this.ReleasePool(list);
				}
				return result;
			}
			return Activator.CreateInstance(type);
		}

		public void Release(int typeHash, object obj)
		{
			List<object> pool;
			if (!this.m_cache.TryGetValue(typeHash, out pool))
			{
				this.m_cache.Add(typeHash, pool = this.GetPool());
			}
			if (pool.Count < this.m_poolMaxSize)
			{
				pool.Add(obj);
			}
		}

		private int m_poolMaxSize;

		private int m_poolInitialCapacity;

		private int m_poolCacheMaxSize;

		private List<List<object>> m_poolCache = new List<List<object>>();

		private Dictionary<int, List<object>> m_cache = new Dictionary<int, List<object>>();
	}
}
