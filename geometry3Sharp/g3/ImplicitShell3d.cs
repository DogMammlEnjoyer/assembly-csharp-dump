using System;

namespace g3
{
	public class ImplicitShell3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			double num = this.A.Value(ref pt);
			if (num < this.Inside.a)
			{
				num = this.Inside.a - num;
			}
			else if (num > this.Inside.b)
			{
				num -= this.Inside.b;
			}
			else
			{
				num = -Math.Min(Math.Abs(num - this.Inside.a), Math.Abs(num - this.Inside.b));
			}
			return num;
		}

		public AxisAlignedBox3d Bounds()
		{
			AxisAlignedBox3d result = this.A.Bounds();
			result.Expand(Math.Max(0.0, this.Inside.b));
			return result;
		}

		public BoundedImplicitFunction3d A;

		public Interval1d Inside;
	}
}
