using System;
using System.Collections.Generic;

namespace g3
{
	public class IndexMap : IIndexMap
	{
		public IndexMap(bool bForceSparse, int MaxIndex = -1)
		{
			if (bForceSparse)
			{
				this.sparse_map = new Dictionary<int, int>();
			}
			else
			{
				this.dense_map = new int[MaxIndex];
			}
			this.MaxIndex = MaxIndex;
			this.SetToInvalid();
		}

		public IndexMap(int[] use_dense_map, int MaxIndex = -1)
		{
			this.dense_map = use_dense_map;
			this.MaxIndex = MaxIndex;
		}

		public IndexMap(int MaxIndex, int SubsetCountEst)
		{
			bool flag = MaxIndex < 32000;
			float num = (float)SubsetCountEst / (float)MaxIndex;
			float num2 = 0.1f;
			if (flag || num > num2)
			{
				this.dense_map = new int[MaxIndex];
			}
			else
			{
				this.sparse_map = new Dictionary<int, int>();
			}
			this.MaxIndex = MaxIndex;
			this.SetToInvalid();
		}

		public void SetToInvalid()
		{
			if (this.dense_map != null)
			{
				for (int i = 0; i < this.dense_map.Length; i++)
				{
					this.dense_map[i] = this.InvalidIndex;
				}
			}
		}

		public bool Contains(int index)
		{
			if (this.MaxIndex > 0 && index >= this.MaxIndex)
			{
				return false;
			}
			if (this.dense_map != null)
			{
				return this.dense_map[index] != this.InvalidIndex;
			}
			return this.sparse_map.ContainsKey(index);
		}

		public int this[int index]
		{
			get
			{
				if (this.dense_map != null)
				{
					return this.dense_map[index];
				}
				int result;
				if (this.sparse_map.TryGetValue(index, out result))
				{
					return result;
				}
				return this.InvalidIndex;
			}
			set
			{
				if (this.dense_map != null)
				{
					this.dense_map[index] = value;
					return;
				}
				this.sparse_map[index] = value;
			}
		}

		public readonly int InvalidIndex = int.MinValue;

		private int[] dense_map;

		private Dictionary<int, int> sparse_map;

		private int MaxIndex;
	}
}
