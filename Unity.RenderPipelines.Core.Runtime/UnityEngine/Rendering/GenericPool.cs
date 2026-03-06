using System;

namespace UnityEngine.Rendering
{
	public static class GenericPool<T> where T : new()
	{
		public static T Get()
		{
			return GenericPool<T>.s_Pool.Get();
		}

		public static ObjectPool<T>.PooledObject Get(out T value)
		{
			return GenericPool<T>.s_Pool.Get(out value);
		}

		public static void Release(T toRelease)
		{
			GenericPool<T>.s_Pool.Release(toRelease);
		}

		private static readonly ObjectPool<T> s_Pool = new ObjectPool<T>(null, null, true);
	}
}
