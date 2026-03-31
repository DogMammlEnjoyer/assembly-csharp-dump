using System;

namespace Cysharp.Threading.Tasks.Internal
{
	internal class StateTuple<T1, T2, T3> : IDisposable
	{
		public void Deconstruct(out T1 item1, out T2 item2, out T3 item3)
		{
			item1 = this.Item1;
			item2 = this.Item2;
			item3 = this.Item3;
		}

		public void Dispose()
		{
			StatePool<T1, T2, T3>.Return(this);
		}

		public T1 Item1;

		public T2 Item2;

		public T3 Item3;
	}
}
