using System;

namespace g3
{
	public class DistanceFieldToSkeletalField : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public AxisAlignedBox3d Bounds()
		{
			AxisAlignedBox3d result = this.DistanceField.Bounds();
			result.Expand(this.FalloffDistance);
			return result;
		}

		public double Value(ref Vector3d pt)
		{
			double num = this.DistanceField.Value(ref pt);
			if (num > this.FalloffDistance)
			{
				return 0.0;
			}
			if (num < -this.FalloffDistance)
			{
				return 1.0;
			}
			double num2 = (num + this.FalloffDistance) / (2.0 * this.FalloffDistance);
			double num3 = 1.0 - num2 * num2;
			return num3 * num3 * num3;
		}

		public BoundedImplicitFunction3d DistanceField;

		public double FalloffDistance;

		public const double ZeroIsocontour = 0.421875;
	}
}
