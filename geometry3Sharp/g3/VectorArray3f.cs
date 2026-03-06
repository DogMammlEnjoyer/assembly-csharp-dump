using System;
using System.Collections.Generic;

namespace g3
{
	public class VectorArray3f : VectorArray3<float>
	{
		public VectorArray3f(int nCount) : base(nCount)
		{
		}

		public VectorArray3f(float[] data) : base(data)
		{
		}

		public Vector3f this[int i]
		{
			get
			{
				return new Vector3f(this.array[3 * i], this.array[3 * i + 1], this.array[3 * i + 2]);
			}
			set
			{
				base.Set(i, value[0], value[1], value[2]);
			}
		}

		public IEnumerable<Vector3f> AsVector3f()
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
