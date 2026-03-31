using System;

namespace Cysharp.Threading.Tasks.Internal
{
	internal class StateTuple<T1> : IDisposable
	{
		public void Deconstruct(out T1 item1)
		{
			item1 = this.Item1;
		}

		public void Dispose()
		{
			StatePool<T1>.Return(this);
		}

		public T1 Item1;
	}
}
