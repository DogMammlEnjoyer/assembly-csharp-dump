using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class SampledArcLengthParam : IArcLengthParam
	{
		public SampledArcLengthParam(IEnumerable<Vector3d> samples, int nCountHint = -1)
		{
			int num = (nCountHint == -1) ? samples.Count<Vector3d>() : nCountHint;
			this.arc_len = new double[num];
			this.arc_len[0] = 0.0;
			this.positions = new Vector3d[num];
			int num2 = 0;
			Vector3d v = Vector3f.Zero;
			foreach (Vector3d vector3d in samples)
			{
				this.positions[num2] = vector3d;
				if (num2 > 0)
				{
					double length = (vector3d - v).Length;
					this.arc_len[num2] = this.arc_len[num2 - 1] + length;
				}
				num2++;
				v = vector3d;
			}
		}

		public double ArcLength
		{
			get
			{
				return this.arc_len[this.arc_len.Length - 1];
			}
		}

		public CurveSample Sample(double f)
		{
			if (f <= 0.0)
			{
				return new CurveSample(new Vector3d(this.positions[0]), this.tangent(0));
			}
			int num = this.arc_len.Length;
			if (f >= this.arc_len[num - 1])
			{
				return new CurveSample(new Vector3d(this.positions[num - 1]), this.tangent(num - 1));
			}
			int i = 0;
			while (i < num)
			{
				if (f < this.arc_len[i])
				{
					int num2 = i - 1;
					int num3 = i;
					if (this.arc_len[num2] == this.arc_len[num3])
					{
						return new CurveSample(new Vector3d(this.positions[num2]), this.tangent(num2));
					}
					double t = (f - this.arc_len[num2]) / (this.arc_len[num3] - this.arc_len[num2]);
					return new CurveSample(Vector3d.Lerp(this.positions[num2], this.positions[num3], t), Vector3d.Lerp(this.tangent(num2), this.tangent(num3), t));
				}
				else
				{
					i++;
				}
			}
			throw new ArgumentException("SampledArcLengthParam.Sample: somehow arc len is outside any possible range");
		}

		protected Vector3d tangent(int i)
		{
			int num = this.arc_len.Length;
			if (i == 0)
			{
				return (this.positions[1] - this.positions[0]).Normalized;
			}
			if (i == num - 1)
			{
				return (this.positions[num - 1] - this.positions[num - 2]).Normalized;
			}
			return (this.positions[i + 1] - this.positions[i - 1]).Normalized;
		}

		private double[] arc_len;

		private Vector3d[] positions;
	}
}
