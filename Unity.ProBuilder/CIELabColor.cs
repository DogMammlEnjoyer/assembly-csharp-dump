using System;

namespace UnityEngine.ProBuilder
{
	internal sealed class CIELabColor
	{
		public CIELabColor(float L, float a, float b)
		{
			this.L = L;
			this.a = a;
			this.b = b;
		}

		public static CIELabColor FromXYZ(XYZColor xyz)
		{
			return ColorUtility.XYZToCIE_Lab(xyz);
		}

		public static CIELabColor FromRGB(Color col)
		{
			return ColorUtility.XYZToCIE_Lab(XYZColor.FromRGB(col));
		}

		public override string ToString()
		{
			return string.Format("( {0}, {1}, {2} )", this.L, this.a, this.b);
		}

		public float L;

		public float a;

		public float b;
	}
}
