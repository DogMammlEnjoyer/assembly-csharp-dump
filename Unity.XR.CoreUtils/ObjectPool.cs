using System;
using System.Collections.Generic;

namespace Unity.XR.CoreUtils
{
	public class ObjectPool<T> where T : class, new()
	{
		public virtual T Get()
		{
			if (this.PooledQueue.Count != 0)
			{
				return this.PooledQueue.Dequeue();
			}
			return Activator.CreateInstance<T>();
		}

		public void Recycle(T instance)
		{
			this.ClearInstance(instance);
			this.PooledQueue.Enqueue(instance);
		}

		protected virtual void ClearInstance(T instance)
		{
		}

		protected readonly Queue<T> PooledQueue = new Queue<T>();
	}
}
