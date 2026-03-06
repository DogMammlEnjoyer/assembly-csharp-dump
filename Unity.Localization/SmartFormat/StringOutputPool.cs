using System;
using UnityEngine.Localization.SmartFormat.Core.Output;
using UnityEngine.Pool;

namespace UnityEngine.Localization.SmartFormat
{
	internal static class StringOutputPool
	{
		public static StringOutput Get(int capacity)
		{
			StringOutput stringOutput = StringOutputPool.s_Pool.Get();
			stringOutput.SetCapacity(capacity);
			return stringOutput;
		}

		public static PooledObject<StringOutput> Get(int capacity, out StringOutput value)
		{
			PooledObject<StringOutput> result = StringOutputPool.s_Pool.Get(out value);
			value.SetCapacity(capacity);
			return result;
		}

		public static void Release(StringOutput toRelease)
		{
			StringOutputPool.s_Pool.Release(toRelease);
		}

		internal static readonly ObjectPool<StringOutput> s_Pool = new ObjectPool<StringOutput>(() => new StringOutput(), null, delegate(StringOutput so)
		{
			so.Clear();
		}, null, true, 10, 10000);
	}
}
