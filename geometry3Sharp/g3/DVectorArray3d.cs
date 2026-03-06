using System;
using System.Collections.Generic;

namespace g3
{
	public class DVectorArray3d : DVectorArray3<double>
	{
		public DVectorArray3d(int nCount = 0) : base(nCount)
		{
		}

		public DVectorArray3d(double[] data) : base(data)
		{
		}

		public Vector3d this[int i]
		{
			get
			{
				return new Vector3d(this.vector[3 * i], this.vector[3 * i + 1], this.vector[3 * i + 2]);
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
