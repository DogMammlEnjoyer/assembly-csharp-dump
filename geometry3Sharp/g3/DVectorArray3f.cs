using System;
using System.Collections.Generic;

namespace g3
{
	public class DVectorArray3f : DVectorArray3<float>
	{
		public DVectorArray3f(int nCount = 0) : base(nCount)
		{
		}

		public DVectorArray3f(float[] data) : base(data)
		{
		}

		public Vector3f this[int i]
		{
			get
			{
				return new Vector3f(this.vector[3 * i], this.vector[3 * i + 1], this.vector[3 * i + 2]);
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
