using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class VectorArray4<T> : IEnumerable<T>, IEnumerable
	{
		public VectorArray4(int nCount = 0)
		{
			this.array = new T[nCount * 4];
		}

		public VectorArray4(T[] data)
		{
			this.array = data;
		}

		public int Count
		{
			get
			{
				return this.array.Length / 4;
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
			this.array = new T[4 * Count];
		}

		public void Set(int i, T a, T b, T c, T d)
		{
			int num = 4 * i;
			this.array[num] = a;
			this.array[num + 1] = b;
			this.array[num + 2] = c;
			this.array[num + 3] = d;
		}

		public void Set(int iStart, int iCount, VectorArray4<T> source)
		{
			Array.Copy(source.array, 0, this.array, 4 * iStart, 4 * iCount);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		public T[] array;
	}
}
