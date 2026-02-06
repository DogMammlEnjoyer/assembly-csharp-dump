using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class Pool<T> where T : class
	{
		public Pool(Func<T> createFunction, Action<T> resetFunction, int poolCapacity)
		{
			this.createFunction = createFunction;
			this.resetFunction = resetFunction;
			this.pool = new Queue<T>();
			this.CreatePoolItems(poolCapacity);
		}

		public Pool(Func<T> createFunction, int poolCapacity) : this(createFunction, null, poolCapacity)
		{
		}

		public int Count
		{
			get
			{
				Queue<T> obj = this.pool;
				int count;
				lock (obj)
				{
					count = this.pool.Count;
				}
				return count;
			}
		}

		private void CreatePoolItems(int numItems)
		{
			for (int i = 0; i < numItems; i++)
			{
				T item = this.createFunction();
				this.pool.Enqueue(item);
			}
		}

		[Obsolete("Use Release() rather than Push()")]
		public void Push(T item)
		{
			bool flag = item == null;
			if (flag)
			{
				throw new ArgumentNullException("Pushing null as item is not allowed.");
			}
			bool flag2 = this.resetFunction != null;
			if (flag2)
			{
				this.resetFunction(item);
			}
			Queue<T> obj = this.pool;
			lock (obj)
			{
				this.pool.Enqueue(item);
			}
		}

		public void Release(T item)
		{
			bool flag = item == null;
			if (flag)
			{
				throw new ArgumentNullException("Pushing null as item is not allowed.");
			}
			bool flag2 = this.resetFunction != null;
			if (flag2)
			{
				this.resetFunction(item);
			}
			Queue<T> obj = this.pool;
			lock (obj)
			{
				this.pool.Enqueue(item);
			}
		}

		[Obsolete("Use Acquire() rather than Pop()")]
		public T Pop()
		{
			Queue<T> obj = this.pool;
			T result;
			lock (obj)
			{
				bool flag2 = this.pool.Count == 0;
				if (flag2)
				{
					return this.createFunction();
				}
				result = this.pool.Dequeue();
			}
			return result;
		}

		public T Acquire()
		{
			Queue<T> obj = this.pool;
			T result;
			lock (obj)
			{
				bool flag2 = this.pool.Count == 0;
				if (flag2)
				{
					return this.createFunction();
				}
				result = this.pool.Dequeue();
			}
			return result;
		}

		private readonly Func<T> createFunction;

		private readonly Queue<T> pool;

		private readonly Action<T> resetFunction;
	}
}
