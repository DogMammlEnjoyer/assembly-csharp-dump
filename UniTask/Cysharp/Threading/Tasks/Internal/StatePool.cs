using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.Internal
{
	internal static class StatePool<T1>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StateTuple<T1> Create(T1 item1)
		{
			StateTuple<T1> stateTuple;
			if (StatePool<T1>.queue.TryDequeue(out stateTuple))
			{
				stateTuple.Item1 = item1;
				return stateTuple;
			}
			return new StateTuple<T1>
			{
				Item1 = item1
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return(StateTuple<T1> tuple)
		{
			tuple.Item1 = default(T1);
			StatePool<T1>.queue.Enqueue(tuple);
		}

		private static readonly ConcurrentQueue<StateTuple<T1>> queue = new ConcurrentQueue<StateTuple<T1>>();
	}
}
