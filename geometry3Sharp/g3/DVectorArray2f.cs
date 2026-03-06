using System;
using System.Collections.Generic;

namespace g3
{
	public class DVectorArray2f : DVectorArray2<float>
	{
		public DVectorArray2f(int nCount = 0) : base(nCount)
		{
		}

		public DVectorArray2f(float[] data) : base(data)
		{
		}

		public Vector2f this[int i]
		{
			get
			{
				return new Vector2f(this.vector[2 * i], this.vector[2 * i + 1]);
			}
			set
			{
				base.Set(i, value[0], value[1]);
			}
		}

		public IEnumerable<Vector2d> AsVector2f()
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
