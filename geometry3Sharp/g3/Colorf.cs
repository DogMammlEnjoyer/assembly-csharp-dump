using System;
using UnityEngine;

namespace g3
{
	public struct Colorf : IComparable<Colorf>, IEquatable<Colorf>
	{
		public Colorf(float greylevel, float a = 1f)
		{
			this.b = greylevel;
			this.g = greylevel;
			this.r = greylevel;
			this.a = a;
		}

		public Colorf(float r, float g, float b, float a = 1f)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public Colorf(int r, int g, int b, int a = 255)
		{
			this.r = MathUtil.Clamp((float)r, 0f, 255f) / 255f;
			this.g = MathUtil.Clamp((float)g, 0f, 255f) / 255f;
			this.b = MathUtil.Clamp((float)b, 0f, 255f) / 255f;
			this.a = MathUtil.Clamp((float)a, 0f, 255f) / 255f;
		}

		public Colorf(float[] v2)
		{
			this.r = v2[0];
			this.g = v2[1];
			this.b = v2[2];
			this.a = v2[3];
		}

		public Colorf(Colorf copy)
		{
			this.r = copy.r;
			this.g = copy.g;
			this.b = copy.b;
			this.a = copy.a;
		}

		public Colorf(Colorf copy, float newAlpha)
		{
			this.r = copy.r;
			this.g = copy.g;
			this.b = copy.b;
			this.a = newAlpha;
		}

		public Colorf Clone(float fAlphaMultiply = 1f)
		{
			return new Colorf(this.r, this.g, this.b, this.a * fAlphaMultiply);
		}

		public float this[int key]
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

		public float SqrDistance(Colorf v2)
		{
			float num = this.r - v2.r;
			float num2 = this.g - v2.g;
			float num3 = num2 - v2.b;
			float num4 = num - v2.a;
			return num * num + num2 * num2 + num3 * num3 + num4 * num4;
		}

		public Vector3f ToRGB()
		{
			return new Vector3f(this.r, this.g, this.b);
		}

		public Colorb ToBytes()
		{
			return new Colorb(this.r, this.g, this.b, this.a);
		}

		public void Set(Colorf o)
		{
			this.r = o.r;
			this.g = o.g;
			this.b = o.b;
			this.a = o.a;
		}

		public void Set(float fR, float fG, float fB, float fA)
		{
			this.r = fR;
			this.g = fG;
			this.b = fB;
			this.a = fA;
		}

		public Colorf SetAlpha(float a)
		{
			this.a = a;
			return this;
		}

		public void Add(Colorf o)
		{
			this.r += o.r;
			this.g += o.g;
			this.b += o.b;
			this.a += o.a;
		}

		public void Subtract(Colorf o)
		{
			this.r -= o.r;
			this.g -= o.g;
			this.b -= o.b;
			this.a -= o.a;
		}

		public Colorf WithAlpha(float newAlpha)
		{
			return new Colorf(this.r, this.g, this.b, newAlpha);
		}

		public static Colorf operator -(Colorf v)
		{
			return new Colorf(-v.r, -v.g, -v.b, -v.a);
		}

		public static Colorf operator *(float f, Colorf v)
		{
			return new Colorf(f * v.r, f * v.g, f * v.b, f * v.a);
		}

		public static Colorf operator *(Colorf v, float f)
		{
			return new Colorf(f * v.r, f * v.g, f * v.b, f * v.a);
		}

		public static Colorf operator +(Colorf v0, Colorf v1)
		{
			return new Colorf(v0.r + v1.r, v0.g + v1.g, v0.b + v1.b, v0.a + v1.a);
		}

		public static Colorf operator +(Colorf v0, float f)
		{
			return new Colorf(v0.r + f, v0.g + f, v0.b + f, v0.a + f);
		}

		public static Colorf operator -(Colorf v0, Colorf v1)
		{
			return new Colorf(v0.r - v1.r, v0.g - v1.g, v0.b - v1.b, v0.a - v1.a);
		}

		public static Colorf operator -(Colorf v0, float f)
		{
			float num = v0.r - f;
			float num2 = v0.g - f;
			float num3 = v0.b - f;
			v0.a = f;
			return new Colorf(num, num2, num3, f);
		}

		public static bool operator ==(Colorf a, Colorf b)
		{
			return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
		}

		public static bool operator !=(Colorf a, Colorf b)
		{
			return a.r != b.r || a.g != b.g || a.b != b.b || a.a != b.a;
		}

		public override bool Equals(object obj)
		{
			return this == (Colorf)obj;
		}

		public override int GetHashCode()
		{
			return (this.r + this.g + this.b + this.a).GetHashCode();
		}

		public int CompareTo(Colorf other)
		{
			if (this.r != other.r)
			{
				if (this.r >= other.r)
				{
					return 1;
				}
				return -1;
			}
			else if (this.g != other.g)
			{
				if (this.g >= other.g)
				{
					return 1;
				}
				return -1;
			}
			else if (this.b != other.b)
			{
				if (this.b >= other.b)
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (this.a == other.a)
				{
					return 0;
				}
				if (this.a >= other.a)
				{
					return 1;
				}
				return -1;
			}
		}

		public bool Equals(Colorf other)
		{
			return this.r == other.r && this.g == other.g && this.b == other.b && this.a == other.a;
		}

		public static Colorf Lerp(Colorf a, Colorf b, float t)
		{
			float num = 1f - t;
			return new Colorf(num * a.r + t * b.r, num * a.g + t * b.g, num * a.b + t * b.b, num * a.a + t * b.a);
		}

		public override string ToString()
		{
			return string.Format("{0:F8} {1:F8} {2:F8} {3:F8}", new object[]
			{
				this.r,
				this.g,
				this.b,
				this.a
			});
		}

		public string ToString(string fmt)
		{
			return string.Format("{0} {1} {2} {3}", new object[]
			{
				this.r.ToString(fmt),
				this.g.ToString(fmt),
				this.b.ToString(fmt),
				this.a.ToString(fmt)
			});
		}

		public static implicit operator Vector3f(Colorf c)
		{
			return new Vector3f(c.r, c.g, c.b);
		}

		public static implicit operator Colorf(Vector3f c)
		{
			return new Colorf(c.x, c.y, c.z, 1f);
		}

		public static implicit operator Colorf(Color c)
		{
			return new Colorf(c.r, c.g, c.b, c.a);
		}

		public static implicit operator Color(Colorf c)
		{
			return new Color(c.r, c.g, c.b, c.a);
		}

		public static implicit operator Color32(Colorf c)
		{
			Colorb colorb = c.ToBytes();
			return new Color32(colorb.r, colorb.g, colorb.b, colorb.a);
		}

		public float r;

		public float g;

		public float b;

		public float a;

		public static readonly Colorf TransparentWhite = new Colorf(255, 255, 255, 0);

		public static readonly Colorf TransparentBlack = new Colorf(0, 0, 0, 0);

		public static readonly Colorf White = new Colorf(255, 255, 255, 255);

		public static readonly Colorf Black = new Colorf(0, 0, 0, 255);

		public static readonly Colorf Blue = new Colorf(0, 0, 255, 255);

		public static readonly Colorf Green = new Colorf(0, 255, 0, 255);

		public static readonly Colorf Red = new Colorf(255, 0, 0, 255);

		public static readonly Colorf Yellow = new Colorf(255, 255, 0, 255);

		public static readonly Colorf Cyan = new Colorf(0, 255, 255, 255);

		public static readonly Colorf Magenta = new Colorf(255, 0, 255, 255);

		public static readonly Colorf VideoWhite = new Colorf(235, 235, 235, 255);

		public static readonly Colorf VideoBlack = new Colorf(16, 16, 16, 255);

		public static readonly Colorf VideoBlue = new Colorf(16, 16, 235, 255);

		public static readonly Colorf VideoGreen = new Colorf(16, 235, 16, 255);

		public static readonly Colorf VideoRed = new Colorf(235, 16, 16, 255);

		public static readonly Colorf VideoYellow = new Colorf(235, 235, 16, 255);

		public static readonly Colorf VideoCyan = new Colorf(16, 235, 235, 255);

		public static readonly Colorf VideoMagenta = new Colorf(235, 16, 235, 255);

		public static readonly Colorf Purple = new Colorf(161, 16, 193, 255);

		public static readonly Colorf DarkRed = new Colorf(128, 16, 16, 255);

		public static readonly Colorf FireBrick = new Colorf(178, 34, 34, 255);

		public static readonly Colorf HotPink = new Colorf(255, 105, 180, 255);

		public static readonly Colorf LightPink = new Colorf(255, 182, 193, 255);

		public static readonly Colorf DarkBlue = new Colorf(16, 16, 139, 255);

		public static readonly Colorf BlueMetal = new Colorf(176, 197, 235, 255);

		public static readonly Colorf Navy = new Colorf(16, 16, 128, 255);

		public static readonly Colorf CornflowerBlue = new Colorf(100, 149, 237, 255);

		public static readonly Colorf LightSteelBlue = new Colorf(176, 196, 222, 255);

		public static readonly Colorf DarkSlateBlue = new Colorf(72, 61, 139, 255);

		public static readonly Colorf Teal = new Colorf(16, 128, 128, 255);

		public static readonly Colorf ForestGreen = new Colorf(16, 139, 16, 255);

		public static readonly Colorf LightGreen = new Colorf(144, 238, 144, 255);

		public static readonly Colorf Orange = new Colorf(230, 73, 16, 255);

		public static readonly Colorf Gold = new Colorf(235, 115, 63, 255);

		public static readonly Colorf DarkYellow = new Colorf(235, 200, 95, 255);

		public static readonly Colorf SiennaBrown = new Colorf(160, 82, 45, 255);

		public static readonly Colorf SaddleBrown = new Colorf(139, 69, 19, 255);

		public static readonly Colorf Goldenrod = new Colorf(218, 165, 32, 255);

		public static readonly Colorf Wheat = new Colorf(245, 222, 179, 255);

		public static readonly Colorf LightGrey = new Colorf(211, 211, 211, 255);

		public static readonly Colorf Silver = new Colorf(192, 192, 192, 255);

		public static readonly Colorf LightSlateGrey = new Colorf(119, 136, 153, 255);

		public static readonly Colorf Grey = new Colorf(128, 128, 128, 255);

		public static readonly Colorf DarkGrey = new Colorf(169, 169, 169, 255);

		public static readonly Colorf SlateGrey = new Colorf(112, 128, 144, 255);

		public static readonly Colorf DimGrey = new Colorf(105, 105, 105, 255);

		public static readonly Colorf DarkSlateGrey = new Colorf(47, 79, 79, 255);

		public static readonly Colorf StandardBeige = new Colorf(0.75f, 0.75f, 0.5f, 1f);

		public static readonly Colorf SelectionGold = new Colorf(1f, 0.6f, 0.05f, 1f);

		public static readonly Colorf PivotYellow = new Colorf(1f, 1f, 0.05f, 1f);
	}
}
