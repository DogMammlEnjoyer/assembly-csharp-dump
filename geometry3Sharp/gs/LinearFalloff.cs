using System;
using g3;

namespace gs
{
	public class LinearFalloff : IFalloffFunction
	{
		public double FalloffT(double t)
		{
			t = MathUtil.Clamp(t, 0.0, 1.0);
			if (this.ConstantRange <= 0.0)
			{
				return 1.0 - t;
			}
			if (t >= this.ConstantRange)
			{
				return 1.0 - (t - this.ConstantRange) / (1.0 - this.ConstantRange);
			}
			return 1.0;
		}

		public IFalloffFunction Duplicate()
		{
			return new WyvillFalloff
			{
				ConstantRange = this.ConstantRange
			};
		}

		public double ConstantRange;
	}
}
