using System;

namespace g3
{
	public class ImplicitBlend2d : ImplicitNAryOp2d
	{
		public override float Value(float fX, float fY)
		{
			float num = 0f;
			foreach (ImplicitField2d implicitField2d in this.m_vChildren)
			{
				num += implicitField2d.Value(fX, fY);
			}
			return num;
		}

		public override void Gradient(float fX, float fY, ref float fGX, ref float fGY)
		{
			fGX = (fGY = 0f);
			float num = 0f;
			float num2 = 0f;
			foreach (ImplicitField2d implicitField2d in this.m_vChildren)
			{
				implicitField2d.Gradient(fX, fY, ref num, ref num2);
				fGX += num;
				fGY += num2;
			}
		}
	}
}
