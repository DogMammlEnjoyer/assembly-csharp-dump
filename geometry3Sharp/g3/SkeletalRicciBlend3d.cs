using System;

namespace g3
{
	public class SkeletalRicciBlend3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			double num = this.A.Value(ref pt);
			double num2 = this.B.Value(ref pt);
			if (this.BlendPower == 1.0)
			{
				return num + num2;
			}
			if (this.BlendPower == 2.0)
			{
				return Math.Sqrt(num * num + num2 * num2);
			}
			return Math.Pow(Math.Pow(num, this.BlendPower) + Math.Pow(num2, this.BlendPower), 1.0 / this.BlendPower);
		}

		public AxisAlignedBox3d Bounds()
		{
			AxisAlignedBox3d result = this.A.Bounds();
			result.Contain(this.B.Bounds());
			result.Expand(0.25 * result.MaxDim);
			return result;
		}

		public BoundedImplicitFunction3d A;

		public BoundedImplicitFunction3d B;

		public double BlendPower = 2.0;
	}
}
