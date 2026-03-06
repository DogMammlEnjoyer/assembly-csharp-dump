using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class DVectorArray3<T> : IEnumerable<T>, IEnumerable
	{
		public DVectorArray3(int nCount = 0)
		{
			this.vector = new DVector<T>();
			if (nCount > 0)
			{
				this.vector.resize(nCount * 3);
			}
		}

		public DVectorArray3(T[] data)
		{
			this.vector = new DVector<T>(data);
		}

		public int Count
		{
			get
			{
				return this.vector.Length / 3;
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
			this.vector.resize(3 * count);
		}

		public void Set(int i, T a, T b, T c)
		{
			this.vector.insert(a, 3 * i);
			this.vector.insert(b, 3 * i + 1);
			this.vector.insert(c, 3 * i + 2);
		}

		public void Append(T a, T b, T c)
		{
			this.vector.push_back(a);
			this.vector.push_back(b);
			this.vector.push_back(c);
		}

		public void Clear()
		{
			this.vector.Clear();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public DVector<T> vector;
	}
}
