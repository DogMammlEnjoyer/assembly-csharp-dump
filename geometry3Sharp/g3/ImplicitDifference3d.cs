using System;

namespace g3
{
	public class ImplicitDifference3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			return Math.Max(this.A.Value(ref pt), -this.B.Value(ref pt));
		}

		public AxisAlignedBox3d Bounds()
		{
			return this.A.Bounds();
		}

		public BoundedImplicitFunction3d A;

		public BoundedImplicitFunction3d B;
	}
}
