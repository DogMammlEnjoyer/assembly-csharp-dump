using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class VectorArray2<T> : IEnumerable<T>, IEnumerable
	{
		public VectorArray2(int nCount = 0)
		{
			this.array = new T[nCount * 2];
		}

		public VectorArray2(T[] data)
		{
			this.array = data;
		}

		public int Count
		{
			get
			{
				return this.array.Length / 2;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			int num;
			for (int i = 0; i < this.array.Length; i = num)
			{
				yield return this.array[i];
				num = i + 1;
			}
			yield break;
		}

		public void Resize(int Count)
		{
			this.array = new T[2 * Count];
		}

		public void Set(int i, T a, T b)
		{
			this.array[2 * i] = a;
			this.array[2 * i + 1] = b;
		}

		public void Set(int iStart, int iCount, VectorArray2<T> source)
		{
			Array.Copy(source.array, 0, this.array, 2 * iStart, 2 * iCount);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		public T[] array;
	}
}
