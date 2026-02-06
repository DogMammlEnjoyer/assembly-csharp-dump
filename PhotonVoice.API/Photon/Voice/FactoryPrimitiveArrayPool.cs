using System;

namespace Photon.Voice
{
	public class FactoryPrimitiveArrayPool<T> : ObjectFactory<T[], int>, IDisposable
	{
		public FactoryPrimitiveArrayPool(int capacity, string name)
		{
			this.pool = new PrimitiveArrayPool<T>(capacity, name);
		}

		public FactoryPrimitiveArrayPool(int capacity, string name, int info)
		{
			this.pool = new PrimitiveArrayPool<T>(capacity, name, info);
		}

		public int Info
		{
			get
			{
				return this.pool.Info;
			}
		}

		public T[] New()
		{
			return this.pool.AcquireOrCreate();
		}

		public T[] New(int size)
		{
			return this.pool.AcquireOrCreate(size);
		}

		public void Free(T[] obj)
		{
			this.pool.Release(obj);
		}

		public void Free(T[] obj, int info)
		{
			this.pool.Release(obj, info);
		}

		public void Dispose()
		{
			this.pool.Dispose();
		}

		private PrimitiveArrayPool<T> pool;
	}
}
