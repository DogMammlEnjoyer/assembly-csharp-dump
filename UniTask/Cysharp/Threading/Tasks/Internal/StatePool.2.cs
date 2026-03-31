using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.Internal
{
	internal static class StatePool<T1, T2>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StateTuple<T1, T2> Create(T1 item1, T2 item2)
		{
			StateTuple<T1, T2> stateTuple;
			if (StatePool<T1, T2>.queue.TryDequeue(out stateTuple))
			{
				stateTuple.Item1 = item1;
				stateTuple.Item2 = item2;
				return stateTuple;
			}
			return new StateTuple<T1, T2>
			{
				Item1 = item1,
				Item2 = item2
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Return(StateTuple<T1, T2> tuple)
		{
			tuple.Item1 = default(T1);
			tuple.Item2 = default(T2);
			StatePool<T1, T2>.queue.Enqueue(tuple);
		}

		private static readonly ConcurrentQueue<StateTuple<T1, T2>> queue = new ConcurrentQueue<StateTuple<T1, T2>>();
	}
}
