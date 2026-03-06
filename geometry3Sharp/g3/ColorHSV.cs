using System;

namespace g3
{
	public class ColorHSV
	{
		public ColorHSV(float h, float s, float v, float a = 1f)
		{
			this.h = h;
			this.s = s;
			this.v = v;
			this.a = a;
		}

		public ColorHSV(Colorf rgb)
		{
			this.ConvertFromRGB(rgb);
		}

		public Colorf RGBA
		{
			get
			{
				return this.ConvertToRGB();
			}
			set
			{
				this.ConvertFromRGB(value);
			}
		}

		public Colorf ConvertToRGB()
		{
			float num = this.h;
			float num2 = this.s;
			float f = this.v;
			if (num > 360f)
			{
				num -= 360f;
			}
			if (num < 0f)
			{
				num += 360f;
			}
			num = MathUtil.Clamp(num, 0f, 360f);
			num2 = MathUtil.Clamp(num2, 0f, 1f);
			float num3 = MathUtil.Clamp(f, 0f, 1f);
			float num4 = num3 * num2;
			float num5 = num4 * (1f - Math.Abs(num / 60f % 2f - 1f));
			float num6 = num3 - num4;
			float num7;
			float num8;
			float num9;
			switch ((int)(num / 60f))
			{
			case 0:
				num7 = num4;
				num8 = num5;
				num9 = 0f;
				break;
			case 1:
				num7 = num5;
				num8 = num4;
				num9 = 0f;
				break;
			case 2:
				num7 = 0f;
				num8 = num4;
				num9 = num5;
				break;
			case 3:
				num7 = 0f;
				num8 = num5;
				num9 = num4;
				break;
			case 4:
				num7 = num5;
				num8 = 0f;
				num9 = num4;
				break;
			default:
				num7 = num4;
				num8 = 0f;
				num9 = num5;
				break;
			}
			return new Colorf(MathUtil.Clamp(num7 + num6, 0f, 1f), MathUtil.Clamp(num8 + num6, 0f, 1f), MathUtil.Clamp(num9 + num6, 0f, 1f), this.a);
		}

		public void ConvertFromRGB(Colorf rgb)
		{
			this.a = rgb.a;
			float r = rgb.r;
			float g = rgb.g;
			float b = rgb.b;
			float num = r;
			int num2 = 0;
			if (g > num)
			{
				num = g;
				num2 = 1;
			}
			if (b > num)
			{
				num = b;
				num2 = 2;
			}
			float num3 = r;
			if (g < num3)
			{
				num3 = g;
			}
			if (b < num3)
			{
				num3 = b;
			}
			float num4 = num - num3;
			if (num4 == 0f)
			{
				this.h = 0f;
			}
			else
			{
				switch (num2)
				{
				case 0:
					this.h = 60f * ((g - b) / num4 % 6f);
					break;
				case 1:
					this.h = 60f * ((b - r) / num4 + 2f);
					break;
				case 2:
					this.h = 60f * ((r - g) / num4 + 4f);
					break;
				}
				if (this.h < 0f)
				{
					this.h += 360f;
				}
			}
			this.v = num;
			if (num == 0f)
			{
				this.s = 0f;
				return;
			}
			this.s = num4 / num;
		}

		public float h;

		public float s;

		public float v;

		public float a;
	}
}
