using System;

namespace g3
{
	public class ImplicitSmoothIntersection3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			double num = this.A.Value(ref pt);
			double num2 = this.B.Value(ref pt);
			return 0.6666666666666666 * (num + num2 + Math.Sqrt(num * num + num2 * num2 - num * num2));
		}

		public AxisAlignedBox3d Bounds()
		{
			AxisAlignedBox3d result = this.A.Bounds();
			result.Contain(this.B.Bounds());
			return result;
		}

		public BoundedImplicitFunction3d A;

		public BoundedImplicitFunction3d B;

		private const double mul = 0.6666666666666666;
	}
}
