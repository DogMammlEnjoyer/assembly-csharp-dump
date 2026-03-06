using System;

namespace UnityEngine.ProBuilder
{
	internal sealed class HSVColor
	{
		public HSVColor(float h, float s, float v)
		{
			this.h = h;
			this.s = s;
			this.v = v;
		}

		public HSVColor(float h, float s, float v, float sv_modifier)
		{
			this.h = h;
			this.s = s * sv_modifier;
			this.v = v * sv_modifier;
		}

		public static HSVColor FromRGB(Color col)
		{
			return ColorUtility.RGBtoHSV(col);
		}

		public override string ToString()
		{
			return string.Format("( {0}, {1}, {2} )", this.h, this.s, this.v);
		}

		public float SqrDistance(HSVColor InColor)
		{
			return InColor.h / 360f - this.h / 360f + (InColor.s - this.s) + (InColor.v - this.v);
		}

		public float h;

		public float s;

		public float v;
	}
}
