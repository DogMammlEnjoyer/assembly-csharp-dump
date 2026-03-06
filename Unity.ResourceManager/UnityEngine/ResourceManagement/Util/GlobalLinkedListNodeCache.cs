using System;
using System.Collections.Generic;

namespace UnityEngine.ResourceManagement.Util
{
	internal static class GlobalLinkedListNodeCache<T>
	{
		public static bool CacheExists
		{
			get
			{
				return GlobalLinkedListNodeCache<T>.m_globalCache != null;
			}
		}

		public static void SetCacheSize(int length)
		{
			if (GlobalLinkedListNodeCache<T>.m_globalCache == null)
			{
				GlobalLinkedListNodeCache<T>.m_globalCache = new LinkedListNodeCache<T>();
			}
			GlobalLinkedListNodeCache<T>.m_globalCache.CachedNodeCount = length;
		}

		public static LinkedListNode<T> Acquire(T val)
		{
			if (GlobalLinkedListNodeCache<T>.m_globalCache == null)
			{
				GlobalLinkedListNodeCache<T>.m_globalCache = new LinkedListNodeCache<T>();
			}
			return GlobalLinkedListNodeCache<T>.m_globalCache.Acquire(val);
		}

		public static void Release(LinkedListNode<T> node)
		{
			if (GlobalLinkedListNodeCache<T>.m_globalCache == null)
			{
				GlobalLinkedListNodeCache<T>.m_globalCache = new LinkedListNodeCache<T>();
			}
			GlobalLinkedListNodeCache<T>.m_globalCache.Release(node);
		}

		private static LinkedListNodeCache<T> m_globalCache;
	}
}
