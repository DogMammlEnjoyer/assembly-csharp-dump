using System;

namespace g3
{
	public class ImplicitAxisAlignedBox3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			return this.AABox.SignedDistance(pt);
		}

		public AxisAlignedBox3d Bounds()
		{
			return this.AABox;
		}

		public AxisAlignedBox3d AABox;
	}
}
