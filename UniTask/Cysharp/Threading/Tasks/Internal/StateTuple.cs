using System;

namespace Cysharp.Threading.Tasks.Internal
{
	internal static class StateTuple
	{
		public static StateTuple<T1> Create<T1>(T1 item1)
		{
			return StatePool<T1>.Create(item1);
		}

		public static StateTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
			return StatePool<T1, T2>.Create(item1, item2);
		}

		public static StateTuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
		{
			return StatePool<T1, T2, T3>.Create(item1, item2, item3);
		}
	}
}
