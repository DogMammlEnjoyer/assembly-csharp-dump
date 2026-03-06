using System;
using System.Collections.Generic;

namespace g3
{
	public class SparseObjectList<T> where T : class
	{
		public SparseObjectList(int MaxIndex, int SubsetCountEst)
		{
			bool flag = MaxIndex < 1024;
			float num = (float)SubsetCountEst / (float)MaxIndex;
			float num2 = 0.1f;
			if (flag || num > num2)
			{
				this.dense = new T[MaxIndex];
				for (int i = 0; i < MaxIndex; i++)
				{
					this.dense[i] = default(T);
				}
				return;
			}
			this.sparse = new Dictionary<int, T>();
		}

		public T this[int idx]
		{
			get
			{
				if (this.dense != null)
				{
					return this.dense[idx];
				}
				T result;
				if (this.sparse.TryGetValue(idx, out result))
				{
					return result;
				}
				return default(T);
			}
			set
			{
				if (this.dense != null)
				{
					this.dense[idx] = value;
					return;
				}
				this.sparse[idx] = value;
			}
		}

		public int Count(Func<T, bool> CountF)
		{
			int num = 0;
			if (this.dense != null)
			{
				for (int i = 0; i < this.dense.Length; i++)
				{
					if (CountF(this.dense[i]))
					{
						num++;
					}
				}
			}
			else
			{
				foreach (KeyValuePair<int, T> keyValuePair in this.sparse)
				{
					if (CountF(keyValuePair.Value))
					{
						num++;
					}
				}
			}
			return num;
		}

		public IEnumerable<KeyValuePair<int, T>> Values()
		{
			if (this.dense != null)
			{
				int num;
				for (int i = 0; i < this.dense.Length; i = num)
				{
					yield return new KeyValuePair<int, T>(i, this.dense[i]);
					num = i + 1;
				}
			}
			else
			{
				foreach (KeyValuePair<int, T> keyValuePair in this.sparse)
				{
					yield return keyValuePair;
				}
				Dictionary<int, T>.Enumerator enumerator = default(Dictionary<int, T>.Enumerator);
			}
			yield break;
			yield break;
		}

		public IEnumerable<KeyValuePair<int, T>> NonZeroValues()
		{
			if (this.dense != null)
			{
				int num;
				for (int i = 0; i < this.dense.Length; i = num)
				{
					if (this.dense[i] != null)
					{
						yield return new KeyValuePair<int, T>(i, this.dense[i]);
					}
					num = i + 1;
				}
			}
			else
			{
				foreach (KeyValuePair<int, T> keyValuePair in this.sparse)
				{
					yield return keyValuePair;
				}
				Dictionary<int, T>.Enumerator enumerator = default(Dictionary<int, T>.Enumerator);
			}
			yield break;
			yield break;
		}

		public void Clear()
		{
			if (this.dense != null)
			{
				Array.Clear(this.dense, 0, this.dense.Length);
				return;
			}
			this.sparse.Clear();
		}

		private T[] dense;

		private Dictionary<int, T> sparse;
	}
}
