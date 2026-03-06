using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class SafeListBuilder<T>
	{
		public SafeListBuilder()
		{
			this.List = new List<T>();
			this.spinlock = default(SpinLock);
		}

		public void SafeAdd(T value)
		{
			bool flag = false;
			while (!flag)
			{
				this.spinlock.Enter(ref flag);
			}
			this.List.Add(value);
			this.spinlock.Exit();
		}

		public void SafeOperation(Action<List<T>> opF)
		{
			bool flag = false;
			while (!flag)
			{
				this.spinlock.Enter(ref flag);
			}
			opF(this.List);
			this.spinlock.Exit();
		}

		public List<T> Result
		{
			get
			{
				return this.List;
			}
		}

		public List<T> List;

		public SpinLock spinlock;
	}
}
