using System;

namespace UnityEngine.Rendering
{
	public static class UnsafeGenericPool<T> where T : new()
	{
		public static T Get()
		{
			return UnsafeGenericPool<T>.s_Pool.Get();
		}

		public static ObjectPool<T>.PooledObject Get(out T value)
		{
			return UnsafeGenericPool<T>.s_Pool.Get(out value);
		}

		public static void Release(T toRelease)
		{
			UnsafeGenericPool<T>.s_Pool.Release(toRelease);
		}

		private static readonly ObjectPool<T> s_Pool = new ObjectPool<T>(null, null, false);
	}
}
