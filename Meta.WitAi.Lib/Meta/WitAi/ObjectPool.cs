using System;
using System.Collections.Concurrent;

namespace Meta.WitAi
{
	public class ObjectPool<T> : IDisposable
	{
		public ObjectPool(Func<T> generator, int preload = 0)
		{
			if (generator == null)
			{
				throw new ArgumentNullException("generator");
			}
			this._generator = generator;
			this._available = new ConcurrentBag<T>();
			this.Preload(preload);
		}

		~ObjectPool()
		{
			this.Dispose();
		}

		public T Get()
		{
			T result;
			if (this._available.TryTake(out result))
			{
				return result;
			}
			return this._generator();
		}

		public void Return(T item)
		{
			this._available.Add(item);
		}

		public void Preload(int total)
		{
			if (total <= 0)
			{
				return;
			}
			for (int i = 0; i < total; i++)
			{
				this.Return(this._generator());
			}
		}

		public void Dispose()
		{
			try
			{
				ConcurrentBag<T> available = this._available;
				if (available != null)
				{
					available.Clear();
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		private readonly Func<T> _generator;

		private readonly ConcurrentBag<T> _available;
	}
}
