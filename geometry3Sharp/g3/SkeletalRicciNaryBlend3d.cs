using System;
using System.Collections.Generic;

namespace g3
{
	public class SkeletalRicciNaryBlend3d : BoundedImplicitFunction3d, ImplicitFunction3d
	{
		public double Value(ref Vector3d pt)
		{
			int count = this.Children.Count;
			double num = 0.0;
			if (this.BlendPower == 1.0)
			{
				for (int i = 0; i < count; i++)
				{
					num += this.Children[i].Value(ref pt);
				}
			}
			else if (this.BlendPower == 2.0)
			{
				for (int j = 0; j < count; j++)
				{
					double num2 = this.Children[j].Value(ref pt);
					num += num2 * num2;
				}
				num = Math.Sqrt(num);
			}
			else
			{
				for (int k = 0; k < count; k++)
				{
					double x = this.Children[k].Value(ref pt);
					num += Math.Pow(x, this.BlendPower);
				}
				num = Math.Pow(num, 1.0 / this.BlendPower);
			}
			return num + this.FieldShift;
		}

		public AxisAlignedBox3d Bounds()
		{
			AxisAlignedBox3d result = this.Children[0].Bounds();
			int count = this.Children.Count;
			for (int i = 1; i < count; i++)
			{
				result.Contain(this.Children[i].Bounds());
			}
			result.Expand(0.25 * result.MaxDim);
			return result;
		}

		public List<BoundedImplicitFunction3d> Children;

		public double BlendPower = 2.0;

		public double FieldShift;
	}
}
