using System;
using System.Collections.Generic;

namespace g3
{
	public class IndexArray4i : VectorArray4<int>
	{
		public IndexArray4i(int nCount) : base(nCount)
		{
		}

		public IndexArray4i(int[] data) : base(data)
		{
		}

		public Index4i this[int i]
		{
			get
			{
				int num = 4 * i;
				return new Index4i(this.array[num], this.array[num + 1], this.array[num + 2], this.array[num + 3]);
			}
			set
			{
				base.Set(i, value[0], value[1], value[2], value[4]);
			}
		}

		public IEnumerable<Index4i> AsIndex4i()
		{
			int num;
			for (int i = 0; i < base.Count; i = num)
			{
				yield return this[i];
				num = i + 1;
			}
			yield break;
		}
	}
}
