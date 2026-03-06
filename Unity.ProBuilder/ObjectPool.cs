using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	internal sealed class ObjectPool<T> : IDisposable
	{
		public ObjectPool(int initialSize, int desiredSize, Func<T> constructor, Action<T> destructor, bool lazyInitialization = false)
		{
			if (constructor == null)
			{
				throw new ArgumentNullException("constructor");
			}
			if (destructor == null)
			{
				throw new ArgumentNullException("destructor");
			}
			this.constructor = constructor;
			this.destructor = destructor;
			this.desiredSize = desiredSize;
			int num = 0;
			while (num < initialSize && num < desiredSize && !lazyInitialization)
			{
				this.m_Pool.Enqueue(constructor());
				num++;
			}
		}

		public T Dequeue()
		{
			if (this.m_Pool.Count > 0)
			{
				return this.m_Pool.Dequeue();
			}
			return this.constructor();
		}

		public void Enqueue(T obj)
		{
			if (this.m_Pool.Count < this.desiredSize)
			{
				this.m_Pool.Enqueue(obj);
				return;
			}
			this.destructor(obj);
		}

		public void Empty()
		{
			int count = this.m_Pool.Count;
			for (int i = 0; i < count; i++)
			{
				this.destructor(this.m_Pool.Dequeue());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !this.m_IsDisposed)
			{
				this.Empty();
				this.m_IsDisposed = true;
			}
		}

		private bool m_IsDisposed;

		private Queue<T> m_Pool = new Queue<T>();

		public int desiredSize;

		public Func<T> constructor;

		public Action<T> destructor;
	}
}
