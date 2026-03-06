using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public static class HashSetPool<T>
	{
		public static HashSet<T> Get()
		{
			return HashSetPool<T>.s_Pool.Get();
		}

		public static ObjectPool<HashSet<T>>.PooledObject Get(out HashSet<T> value)
		{
			return HashSetPool<T>.s_Pool.Get(out value);
		}

		public static void Release(HashSet<T> toRelease)
		{
			HashSetPool<T>.s_Pool.Release(toRelease);
		}

		private static readonly ObjectPool<HashSet<T>> s_Pool = new ObjectPool<HashSet<T>>(null, delegate(HashSet<T> l)
		{
			l.Clear();
		}, true);
	}
}
