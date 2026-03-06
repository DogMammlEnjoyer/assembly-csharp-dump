using System;

namespace g3
{
	public class MemoryPool<T> where T : class, new()
	{
		public MemoryPool()
		{
			this.Allocated = new DVector<T>();
			this.Free = new DVector<T>();
		}

		public T Allocate()
		{
			if (this.Free.size > 0)
			{
				T result = this.Free[this.Free.size - 1];
				this.Free.pop_back();
				return result;
			}
			T t = Activator.CreateInstance<T>();
			this.Allocated.Add(t);
			return t;
		}

		public void Return(T obj)
		{
			this.Free.Add(obj);
		}

		public void ReturnAll()
		{
			this.Free = new DVector<T>(this.Allocated);
		}

		public void FreeAll()
		{
			this.Allocated = new DVector<T>();
			this.Free = new DVector<T>();
		}

		private DVector<T> Allocated;

		private DVector<T> Free;
	}
}
