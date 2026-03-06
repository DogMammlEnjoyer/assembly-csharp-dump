using System;

namespace g3
{
	public class ImplicitHalfSpace3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			return (pt - this.Origin).Dot(this.Normal);
		}

		public AxisAlignedBox3d Bounds()
		{
			return new AxisAlignedBox3d(this.Origin, 2.220446049250313E-16);
		}

		public Vector3d Origin;

		public Vector3d Normal;
	}
}
