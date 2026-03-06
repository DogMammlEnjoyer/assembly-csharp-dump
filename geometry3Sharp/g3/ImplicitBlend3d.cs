using System;

namespace g3
{
	public class ImplicitBlend3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double WeightA
		{
			get
			{
				return this.weightA;
			}
			set
			{
				this.weightA = MathUtil.Clamp(value, 1E-05, 100000.0);
			}
		}

		public double WeightB
		{
			get
			{
				return this.weightB;
			}
			set
			{
				this.weightB = MathUtil.Clamp(value, 1E-05, 100000.0);
			}
		}

		public double Blend
		{
			get
			{
				return this.blend;
			}
			set
			{
				this.blend = MathUtil.Clamp(value, 0.0, 100000.0);
			}
		}

		public double Value(ref Vector3d pt)
		{
			double num = this.A.Value(ref pt);
			double num2 = this.B.Value(ref pt);
			double num3 = num * num + num2 * num2;
			if (num3 > 1000000000000.0)
			{
				return Math.Min(num, num2);
			}
			double num4 = num / this.weightA;
			double num5 = num2 / this.weightB;
			double num6 = this.blend / (1.0 + num4 * num4 + num5 * num5);
			return 0.666666 * (num + num2 - Math.Sqrt(num3 - num * num2)) - num6;
		}

		public AxisAlignedBox3d Bounds()
		{
			AxisAlignedBox3d result = this.A.Bounds();
			result.Contain(this.B.Bounds());
			result.Expand(this.ExpandBounds * result.MaxDim);
			return result;
		}

		public BoundedImplicitFunction3d A;

		public BoundedImplicitFunction3d B;

		private double weightA = 0.01;

		private double weightB = 0.01;

		private double blend = 2.0;

		public double ExpandBounds = 0.25;
	}
}
