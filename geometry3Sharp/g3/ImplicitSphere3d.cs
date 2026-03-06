using System;

namespace g3
{
	public class ImplicitSphere3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			return pt.Distance(ref this.Origin) - this.Radius;
		}

		public AxisAlignedBox3d Bounds()
		{
			return new AxisAlignedBox3d(this.Origin, this.Radius);
		}

		public Vector3d Origin;

		public double Radius;
	}
}
