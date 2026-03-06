using System;

namespace g3
{
	public class ImplicitUnion3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			return Math.Min(this.A.Value(ref pt), this.B.Value(ref pt));
		}

		public AxisAlignedBox3d Bounds()
		{
			AxisAlignedBox3d result = this.A.Bounds();
			result.Contain(this.B.Bounds());
			return result;
		}

		public BoundedImplicitFunction3d A;

		public BoundedImplicitFunction3d B;
	}
}
