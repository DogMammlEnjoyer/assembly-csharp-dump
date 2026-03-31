using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.Internal
{
	internal static class StatePool<T1, T2, T3>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StateTuple<T1, T2, T3> Create(T1 item1, T2 item2, T3 item3)
		{
			StateTuple<T1, T2, T3> stateTuple;
			if (StatePool<T1, T2, T3>.queue.TryDequeue(out stateTuple))
			{
				stateTuple.Item1 = item1;
				stateTuple.Item2 = item2;
				stateTuple.Item3 = item3;
				return stateTuple;
			}
			return new StateTuple<T1, T2, T3>
			{
				Item1 = item1,
				Item2 = item2,
				Item3 = item3
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return(StateTuple<T1, T2, T3> tuple)
		{
			tuple.Item1 = default(T1);
			tuple.Item2 = default(T2);
			tuple.Item3 = default(T3);
			StatePool<T1, T2, T3>.queue.Enqueue(tuple);
		}

		private static readonly ConcurrentQueue<StateTuple<T1, T2, T3>> queue = new ConcurrentQueue<StateTuple<T1, T2, T3>>();
	}
}
