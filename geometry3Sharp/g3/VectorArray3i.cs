using System;
using System.Collections.Generic;

namespace g3
{
	public class VectorArray3i : VectorArray3<int>
	{
		public VectorArray3i(int nCount) : base(nCount)
		{
		}

		public VectorArray3i(int[] data) : base(data)
		{
		}

		public Vector3i this[int i]
		{
			get
			{
				return new Vector3i(this.array[3 * i], this.array[3 * i + 1], this.array[3 * i + 2]);
			}
			set
			{
				this.Set(i, value[0], value[1], value[2], false);
			}
		}

		public void Set(int i, int a, int b, int c, bool bCycle = false)
		{
			this.array[3 * i] = a;
			if (bCycle)
			{
				this.array[3 * i + 1] = c;
				this.array[3 * i + 2] = b;
				return;
			}
			this.array[3 * i + 1] = b;
			this.array[3 * i + 2] = c;
		}

		public IEnumerable<Vector3i> AsVector3i()
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
