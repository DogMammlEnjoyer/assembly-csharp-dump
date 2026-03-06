using System;
using UnityEngine;

namespace g3
{
	public struct Colorb
	{
		public Colorb(byte greylevel, byte a = 1)
		{
			this.b = greylevel;
			this.g = greylevel;
			this.r = greylevel;
			this.a = a;
		}

		public Colorb(byte r, byte g, byte b, byte a = 1)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public Colorb(float r, float g, float b, float a = 1f)
		{
			this.r = (byte)MathUtil.Clamp((int)(r * 255f), 0, 255);
			this.g = (byte)MathUtil.Clamp((int)(g * 255f), 0, 255);
			this.b = (byte)MathUtil.Clamp((int)(b * 255f), 0, 255);
			this.a = (byte)MathUtil.Clamp((int)(a * 255f), 0, 255);
		}

		public Colorb(byte[] v2)
		{
			this.r = v2[0];
			this.g = v2[1];
			this.b = v2[2];
			this.a = v2[3];
		}

		public Colorb(Colorb copy)
		{
			this.r = copy.r;
			this.g = copy.g;
			this.b = copy.b;
			this.a = copy.a;
		}

		public Colorb(Colorb copy, byte newAlpha)
		{
			this.r = copy.r;
			this.g = copy.g;
			this.b = copy.b;
			this.a = newAlpha;
		}

		public byte this[int key]
		{
			get
			{
				if (key == 0)
				{
					return this.r;
				}
				if (key == 1)
				{
					return this.g;
				}
				if (key == 2)
				{
					return this.b;
				}
				return this.a;
			}
			set
			{
				if (key == 0)
				{
					this.r = value;
					return;
				}
				if (key == 1)
				{
					this.g = value;
					return;
				}
				if (key == 2)
				{
					this.b = value;
					return;
				}
				this.a = value;
			}
		}

		public static implicit operator Colorb(Color32 c)
		{
			return new Colorb(c.r, c.g, c.b, c.a);
		}

		public static implicit operator Color32(Colorb c)
		{
			return new Color32(c.r, c.g, c.b, c.a);
		}

		public byte r;

		public byte g;

		public byte b;

		public byte a;
	}
}
