using System;

namespace g3
{
	public class ImplicitOffset3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			return this.A.Value(ref pt) - this.Offset;
		}

		public AxisAlignedBox3d Bounds()
		{
			AxisAlignedBox3d result = this.A.Bounds();
			result.Expand(this.Offset);
			return result;
		}

		public BoundedImplicitFunction3d A;

		public double Offset;
	}
}
