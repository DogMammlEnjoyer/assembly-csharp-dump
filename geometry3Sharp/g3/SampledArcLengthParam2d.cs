using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class SampledArcLengthParam2d : IArcLengthParam2d
	{
		public SampledArcLengthParam2d(IEnumerable<Vector2d> samples, int nCountHint = -1)
		{
			int num = (nCountHint == -1) ? samples.Count<Vector2d>() : nCountHint;
			this.arc_len = new double[num];
			this.arc_len[0] = 0.0;
			this.positions = new Vector2d[num];
			int num2 = 0;
			Vector2d o = Vector2d.Zero;
			foreach (Vector2d vector2d in samples)
			{
				this.positions[num2] = vector2d;
				if (num2 > 0)
				{
					double length = (vector2d - o).Length;
					this.arc_len[num2] = this.arc_len[num2 - 1] + length;
				}
				num2++;
				o = vector2d;
			}
		}

		public double ArcLength
		{
			get
			{
				return this.arc_len[this.arc_len.Length - 1];
			}
		}

		public CurveSample2d Sample(double f)
		{
			if (f <= 0.0)
			{
				return new CurveSample2d(new Vector2d(this.positions[0]), this.tangent(0));
			}
			int num = this.arc_len.Length;
			if (f >= this.arc_len[num - 1])
			{
				return new CurveSample2d(new Vector2d(this.positions[num - 1]), this.tangent(num - 1));
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
						return new CurveSample2d(new Vector2d(this.positions[num2]), this.tangent(num2));
					}
					double t = (f - this.arc_len[num2]) / (this.arc_len[num3] - this.arc_len[num2]);
					return new CurveSample2d(Vector2d.Lerp(this.positions[num2], this.positions[num3], t), Vector2d.Lerp(this.tangent(num2), this.tangent(num3), t));
				}
				else
				{
					i++;
				}
			}
			throw new ArgumentException("SampledArcLengthParam2d.Sample: somehow arc len is outside any possible range");
		}

		protected Vector2d tangent(int i)
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

		private Vector2d[] positions;
	}
}
