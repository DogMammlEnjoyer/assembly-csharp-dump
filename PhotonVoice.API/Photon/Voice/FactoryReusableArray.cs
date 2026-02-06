using System;

namespace Photon.Voice
{
	public class FactoryReusableArray<T> : ObjectFactory<T[], int>, IDisposable
	{
		public FactoryReusableArray(int size)
		{
			this.arr = new T[size];
		}

		public int Info
		{
			get
			{
				return this.arr.Length;
			}
		}

		public T[] New()
		{
			return this.arr;
		}

		public T[] New(int size)
		{
			if (this.arr.Length != size)
			{
				this.arr = new T[size];
			}
			return this.arr;
		}

		public void Free(T[] obj)
		{
		}

		public void Free(T[] obj, int info)
		{
		}

		public void Dispose()
		{
		}

		private T[] arr;
	}
}
