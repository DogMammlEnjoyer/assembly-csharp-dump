using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class VectorArray3<T> : IEnumerable<T>, IEnumerable
	{
		public VectorArray3(int nCount = 0)
		{
			this.array = new T[nCount * 3];
		}

		public VectorArray3(T[] data)
		{
			this.array = data;
		}

		public int Count
		{
			get
			{
				return this.array.Length / 3;
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
			this.array = new T[3 * Count];
		}

		public void Set(int i, T a, T b, T c)
		{
			this.array[3 * i] = a;
			this.array[3 * i + 1] = b;
			this.array[3 * i + 2] = c;
		}

		public void Set(int iStart, int iCount, VectorArray3<T> source)
		{
			Array.Copy(source.array, 0, this.array, 3 * iStart, 3 * iCount);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		public T[] array;
	}
}
