using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class IndexFlagSet : IEnumerable<int>, IEnumerable
	{
		public IndexFlagSet(bool bForceSparse, int MaxIndex = -1)
		{
			if (bForceSparse)
			{
				this.hash = new HashSet<int>();
			}
			else
			{
				this.bits = new BitArray(MaxIndex);
			}
			this.count = 0;
		}

		public IndexFlagSet(int MaxIndex, int SubsetCountEst)
		{
			bool flag = MaxIndex < 128000;
			float num = (float)SubsetCountEst / (float)MaxIndex;
			float num2 = 0.05f;
			if (flag || num > num2)
			{
				this.bits = new BitArray(MaxIndex);
			}
			else
			{
				this.hash = new HashSet<int>();
			}
			this.count = 0;
		}

		public bool Contains(int i)
		{
			return this[i];
		}

		public void Add(int i)
		{
			this[i] = true;
		}

		public int Count
		{
			get
			{
				if (this.bits != null)
				{
					return this.count;
				}
				return this.hash.Count;
			}
		}

		public bool this[int key]
		{
			get
			{
				if (this.bits == null)
				{
					return this.hash.Contains(key);
				}
				return this.bits[key];
			}
			set
			{
				if (this.bits != null)
				{
					if (this.bits[key] != value)
					{
						this.bits[key] = value;
						if (!value)
						{
							this.count--;
							return;
						}
						this.count++;
						return;
					}
				}
				else
				{
					if (value)
					{
						this.hash.Add(key);
						return;
					}
					if (!value && this.hash.Contains(key))
					{
						this.hash.Remove(key);
					}
				}
			}
		}

		public IEnumerator<int> GetEnumerator()
		{
			if (this.bits != null)
			{
				int num;
				for (int i = 0; i < this.bits.Length; i = num)
				{
					if (this.bits[i])
					{
						yield return i;
					}
					num = i + 1;
				}
			}
			else
			{
				foreach (int num2 in this.hash)
				{
					yield return num2;
				}
				HashSet<int>.Enumerator enumerator = default(HashSet<int>.Enumerator);
			}
			yield break;
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private BitArray bits;

		private HashSet<int> hash;

		private int count;
	}
}
