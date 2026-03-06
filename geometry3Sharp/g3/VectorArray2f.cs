using System;
using System.Collections.Generic;

namespace g3
{
	public class VectorArray2f : VectorArray2<float>
	{
		public VectorArray2f(int nCount) : base(nCount)
		{
		}

		public VectorArray2f(float[] data) : base(data)
		{
		}

		public Vector2f this[int i]
		{
			get
			{
				return new Vector2f(this.array[2 * i], this.array[2 * i + 1]);
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
