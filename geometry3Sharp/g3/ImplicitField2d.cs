using System;

namespace g3
{
	public interface ImplicitField2d
	{
		float Value(float fX, float fY);

		void Gradient(float fX, float fY, ref float fGX, ref float fGY);

		AxisAlignedBox2f Bounds { get; }
	}
}
