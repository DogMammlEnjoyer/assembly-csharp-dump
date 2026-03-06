using System;
using System.Collections.Generic;

namespace g3
{
	public class VectorArray3d : VectorArray3<double>
	{
		public VectorArray3d(int nCount, bool debug = false) : base(nCount)
		{
		}

		public VectorArray3d(double[] data) : base(data)
		{
		}

		public Vector3d this[int i]
		{
			get
			{
				return new Vector3d(this.array[3 * i], this.array[3 * i + 1], this.array[3 * i + 2]);
			}
			set
			{
				base.Set(i, value[0], value[1], value[2]);
			}
		}

		public IEnumerable<Vector3d> AsVector3d()
		{
			int num;
			for (int i = 0; i < base.Count; i = num)
			{
				yield return this[i];
				num = i + 1;
			}
			yield break;
		}

		private const double invalid_value = -99999999.0;
	}
}
