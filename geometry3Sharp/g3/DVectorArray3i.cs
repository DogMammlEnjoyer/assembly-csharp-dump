using System;
using System.Collections.Generic;

namespace g3
{
	public class DVectorArray3i : DVectorArray3<int>
	{
		public DVectorArray3i(int nCount = 0) : base(nCount)
		{
		}

		public DVectorArray3i(int[] data) : base(data)
		{
		}

		public Vector3i this[int i]
		{
			get
			{
				return new Vector3i(this.vector[3 * i], this.vector[3 * i + 1], this.vector[3 * i + 2]);
			}
			set
			{
				this.Set(i, value[0], value[1], value[2], false);
			}
		}

		public void Set(int i, int a, int b, int c, bool bCycle = false)
		{
			this.vector[3 * i] = a;
			if (bCycle)
			{
				this.vector[3 * i + 1] = c;
				this.vector[3 * i + 2] = b;
				return;
			}
			this.vector[3 * i + 1] = b;
			this.vector[3 * i + 2] = c;
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
