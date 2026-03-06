using System;
using System.Text;
using UnityEngine.Pool;

namespace UnityEngine.Localization
{
	internal static class StringBuilderPool
	{
		public static StringBuilder Get()
		{
			return StringBuilderPool.s_Pool.Get();
		}

		public static PooledObject<StringBuilder> Get(out StringBuilder value)
		{
			return StringBuilderPool.s_Pool.Get(out value);
		}

		public static void Release(StringBuilder toRelease)
		{
			StringBuilderPool.s_Pool.Release(toRelease);
		}

		internal static readonly ObjectPool<StringBuilder> s_Pool = new ObjectPool<StringBuilder>(() => new StringBuilder(), null, delegate(StringBuilder sb)
		{
			sb.Clear();
		}, null, false, 10, 10000);
	}
}
