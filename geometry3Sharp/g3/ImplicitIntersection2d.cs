using System;

namespace g3
{
	public class ImplicitIntersection2d : ImplicitNAryOp2d
	{
		public override float Value(float fX, float fY)
		{
			float num = 1E+10f;
			foreach (ImplicitField2d implicitField2d in this.m_vChildren)
			{
				num = Math.Min(num, implicitField2d.Value(fX, fY));
			}
			return num;
		}

		public override void Gradient(float fX, float fY, ref float fGX, ref float fGY)
		{
			float num = 1E+10f;
			int num2 = -1;
			for (int i = 0; i < this.m_vChildren.Count; i++)
			{
				float num3 = this.m_vChildren[i].Value(fX, fY);
				if (num3 < num)
				{
					num2 = i;
				}
				num = num3;
			}
			if (num2 >= 0)
			{
				this.m_vChildren[num2].Gradient(fX, fY, ref fGX, ref fGY);
				return;
			}
			fGX = (fGY = 0f);
		}
	}
}
