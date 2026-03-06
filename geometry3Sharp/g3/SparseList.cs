using System;
using System.Collections.Generic;

namespace g3
{
	public class SparseList<T> where T : IEquatable<T>
	{
		public SparseList(int MaxIndex, int SubsetCountEst, T ZeroValue)
		{
			this.zeroValue = ZeroValue;
			bool flag = MaxIndex > 0 && MaxIndex < 1024;
			float num = (MaxIndex == 0) ? 0f : ((float)SubsetCountEst / (float)MaxIndex);
			float num2 = 0.1f;
			if (flag || num > num2)
			{
				this.dense = new T[MaxIndex];
				for (int i = 0; i < MaxIndex; i++)
				{
					this.dense[i] = ZeroValue;
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
				return this.zeroValue;
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
					if (!this.dense[i].Equals(this.zeroValue))
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

		private T[] dense;

		private Dictionary<int, T> sparse;

		private T zeroValue;
	}
}
