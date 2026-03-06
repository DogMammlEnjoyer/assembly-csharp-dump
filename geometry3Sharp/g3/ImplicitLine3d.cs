using System;

namespace g3
{
	public class ImplicitLine3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			return Math.Sqrt(this.Segment.DistanceSquared(pt)) - this.Radius;
		}

		public AxisAlignedBox3d Bounds()
		{
			Vector3d v = this.Radius * Vector3d.One;
			Vector3d p = this.Segment.P0;
			Vector3d p2 = this.Segment.P1;
			AxisAlignedBox3d result = new AxisAlignedBox3d(p - v, p + v);
			result.Contain(p2 - v);
			result.Contain(p2 + v);
			return result;
		}

		public Segment3d Segment;

		public double Radius;
	}
}
