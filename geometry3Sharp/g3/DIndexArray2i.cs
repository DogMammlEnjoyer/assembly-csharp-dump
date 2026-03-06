using System;
using System.Collections.Generic;

namespace g3
{
	public class DIndexArray2i : DVectorArray2<int>
	{
		public DIndexArray2i(int nCount = 0) : base(nCount)
		{
		}

		public DIndexArray2i(int[] data) : base(data)
		{
		}

		public Index2i this[int i]
		{
			get
			{
				return new Index2i(this.vector[2 * i], this.vector[2 * i + 1]);
			}
			set
			{
				base.Set(i, value[0], value[1]);
			}
		}

		public IEnumerable<Index2i> AsIndex2i()
		{
			int num;
			for (int i = 0; i < base.Count; i = num)
			{
				yield return new Index2i(this.vector[2 * i], this.vector[2 * i + 1]);
				num = i + 1;
			}
			yield break;
		}
	}
}
