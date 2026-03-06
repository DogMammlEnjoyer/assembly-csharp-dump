using System;

namespace UnityEngine.ProBuilder
{
	internal sealed class XYZColor
	{
		public XYZColor(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static XYZColor FromRGB(Color col)
		{
			return ColorUtility.RGBToXYZ(col);
		}

		public static XYZColor FromRGB(float R, float G, float B)
		{
			return ColorUtility.RGBToXYZ(R, G, B);
		}

		public override string ToString()
		{
			return string.Format("( {0}, {1}, {2} )", this.x, this.y, this.z);
		}

		public float x;

		public float y;

		public float z;
	}
}
