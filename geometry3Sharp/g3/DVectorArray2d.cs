using System;
using System.Collections.Generic;

namespace g3
{
	public class DVectorArray2d : DVectorArray2<double>
	{
		public DVectorArray2d(int nCount = 0) : base(nCount)
		{
		}

		public DVectorArray2d(double[] data) : base(data)
		{
		}

		public Vector2d this[int i]
		{
			get
			{
				return new Vector2d(this.vector[2 * i], this.vector[2 * i + 1]);
			}
			set
			{
				base.Set(i, value[0], value[1]);
			}
		}

		public IEnumerable<Vector2d> AsVector2d()
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
