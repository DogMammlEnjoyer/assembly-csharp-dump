using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class DVectorArray2<T> : IEnumerable<T>, IEnumerable
	{
		public DVectorArray2(int nCount = 0)
		{
			this.vector = new DVector<T>();
			this.vector.resize(nCount * 2);
		}

		public DVectorArray2(T[] data)
		{
			this.vector = new DVector<T>(data);
		}

		public int Count
		{
			get
			{
				return this.vector.Length / 2;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			int num;
			for (int i = 0; i < this.vector.Length; i = num)
			{
				yield return this.vector[i];
				num = i + 1;
			}
			yield break;
		}

		public void Resize(int count)
		{
			this.vector.resize(2 * count);
		}

		public void Set(int i, T a, T b)
		{
			this.vector.insert(a, 2 * i);
			this.vector.insert(b, 2 * i + 1);
		}

		public void Append(T a, T b)
		{
			this.vector.push_back(a);
			this.vector.push_back(b);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public DVector<T> vector;
	}
}
