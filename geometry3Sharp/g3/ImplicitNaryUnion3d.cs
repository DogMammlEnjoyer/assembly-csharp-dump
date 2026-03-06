using System;
using System.Collections.Generic;

namespace g3
{
	public class ImplicitNaryUnion3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			double num = this.Children[0].Value(ref pt);
			int count = this.Children.Count;
			for (int i = 1; i < count; i++)
			{
				num = Math.Min(num, this.Children[i].Value(ref pt));
			}
			return num;
		}

		public AxisAlignedBox3d Bounds()
		{
			AxisAlignedBox3d result = this.Children[0].Bounds();
			int count = this.Children.Count;
			for (int i = 1; i < count; i++)
			{
				result.Contain(this.Children[i].Bounds());
			}
			return result;
		}

		public List<BoundedImplicitFunction3d> Children;
	}
}
