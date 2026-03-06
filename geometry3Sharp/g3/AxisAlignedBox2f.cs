using System;
using UnityEngine;

namespace g3
{
	public struct AxisAlignedBox2f
	{
		public AxisAlignedBox2f(bool bIgnore)
		{
			this.Min = new Vector2f(float.MaxValue, float.MaxValue);
			this.Max = new Vector2f(float.MinValue, float.MinValue);
		}

		public AxisAlignedBox2f(float xmin, float ymin, float xmax, float ymax)
		{
			this.Min = new Vector2f(xmin, ymin);
			this.Max = new Vector2f(xmax, ymax);
		}

		public AxisAlignedBox2f(float fSquareSize)
		{
			this.Min = new Vector2f(0f, 0f);
			this.Max = new Vector2f(fSquareSize, fSquareSize);
		}

		public AxisAlignedBox2f(float fWidth, float fHeight)
		{
			this.Min = new Vector2f(0f, 0f);
			this.Max = new Vector2f(fWidth, fHeight);
		}

		public AxisAlignedBox2f(Vector2f vMin, Vector2f vMax)
		{
			this.Min = new Vector2f(Math.Min(vMin.x, vMax.x), Math.Min(vMin.y, vMax.y));
			this.Max = new Vector2f(Math.Max(vMin.x, vMax.x), Math.Max(vMin.y, vMax.y));
		}

		public AxisAlignedBox2f(Vector2f vCenter, float fHalfWidth, float fHalfHeight)
		{
			this.Min = new Vector2f(vCenter.x - fHalfWidth, vCenter.y - fHalfHeight);
			this.Max = new Vector2f(vCenter.x + fHalfWidth, vCenter.y + fHalfHeight);
		}

		public AxisAlignedBox2f(Vector2f vCenter, float fHalfWidth)
		{
			this.Min = new Vector2f(vCenter.x - fHalfWidth, vCenter.y - fHalfWidth);
			this.Max = new Vector2f(vCenter.x + fHalfWidth, vCenter.y + fHalfWidth);
		}

		public AxisAlignedBox2f(Vector2f vCenter)
		{
			this.Max = vCenter;
			this.Min = vCenter;
		}

		public AxisAlignedBox2f(AxisAlignedBox2f o)
		{
			this.Min = new Vector2f(o.Min);
			this.Max = new Vector2f(o.Max);
		}

		public float Width
		{
			get
			{
				return Math.Max(this.Max.x - this.Min.x, 0f);
			}
		}

		public float Height
		{
			get
			{
				return Math.Max(this.Max.y - this.Min.y, 0f);
			}
		}

		public float Area
		{
			get
			{
				return this.Width * this.Height;
			}
		}

		public float DiagonalLength
		{
			get
			{
				return (float)Math.Sqrt((double)((this.Max.x - this.Min.x) * (this.Max.x - this.Min.x) + (this.Max.y - this.Min.y) * (this.Max.y - this.Min.y)));
			}
		}

		public float MaxDim
		{
			get
			{
				return Math.Max(this.Width, this.Height);
			}
		}

		public Vector2f Diagonal
		{
			get
			{
				return new Vector2f(this.Max.x - this.Min.x, this.Max.y - this.Min.y);
			}
		}

		public Vector2f Center
		{
			get
			{
				return new Vector2f(0.5f * (this.Min.x + this.Max.x), 0.5f * (this.Min.y + this.Max.y));
			}
		}

		public Vector2f BottomLeft
		{
			get
			{
				return this.Min;
			}
		}

		public Vector2f BottomRight
		{
			get
			{
				return new Vector2f(this.Max.x, this.Min.y);
			}
		}

		public Vector2f TopLeft
		{
			get
			{
				return new Vector2f(this.Min.x, this.Max.y);
			}
		}

		public Vector2f TopRight
		{
			get
			{
				return this.Max;
			}
		}

		public Vector2f CenterLeft
		{
			get
			{
				return new Vector2f(this.Min.x, (this.Min.y + this.Max.y) * 0.5f);
			}
		}

		public Vector2f CenterRight
		{
			get
			{
				return new Vector2f(this.Max.x, (this.Min.y + this.Max.y) * 0.5f);
			}
		}

		public Vector2f CenterTop
		{
			get
			{
				return new Vector2f((this.Min.x + this.Max.x) * 0.5f, this.Max.y);
			}
		}

		public Vector2f CenterBottom
		{
			get
			{
				return new Vector2f((this.Min.x + this.Max.x) * 0.5f, this.Min.y);
			}
		}

		public Vector2f GetCorner(int i)
		{
			return new Vector2f((i % 3 == 0) ? this.Min.x : this.Max.x, (i < 2) ? this.Min.y : this.Max.y);
		}

		public void Expand(float fRadius)
		{
			this.Min.x = this.Min.x - fRadius;
			this.Min.y = this.Min.y - fRadius;
			this.Max.x = this.Max.x + fRadius;
			this.Max.y = this.Max.y + fRadius;
		}

		public void Contract(float fRadius)
		{
			this.Min.x = this.Min.x + fRadius;
			this.Min.y = this.Min.y + fRadius;
			this.Max.x = this.Max.x - fRadius;
			this.Max.y = this.Max.y - fRadius;
		}

		[Obsolete("This method name is confusing. Will remove in future. Use Add() instead")]
		public void Pad(float fPadLeft, float fPadRight, float fPadBottom, float fPadTop)
		{
			this.Min.x = this.Min.x + fPadLeft;
			this.Min.y = this.Min.y + fPadBottom;
			this.Max.x = this.Max.x + fPadRight;
			this.Max.y = this.Max.y + fPadTop;
		}

		public void Add(float left, float right, float bottom, float top)
		{
			this.Min.x = this.Min.x + left;
			this.Min.y = this.Min.y + bottom;
			this.Max.x = this.Max.x + right;
			this.Max.y = this.Max.y + top;
		}

		public void SetWidth(float fNewWidth, AxisAlignedBox2f.ScaleMode eScaleMode)
		{
			switch (eScaleMode)
			{
			case AxisAlignedBox2f.ScaleMode.ScaleRight:
				this.Max.x = this.Min.x + fNewWidth;
				return;
			case AxisAlignedBox2f.ScaleMode.ScaleLeft:
				this.Min.x = this.Max.x - fNewWidth;
				return;
			case AxisAlignedBox2f.ScaleMode.ScaleCenter:
			{
				Vector2f center = this.Center;
				this.Min.x = center.x - 0.5f * fNewWidth;
				this.Max.x = center.x + 0.5f * fNewWidth;
				return;
			}
			}
			throw new Exception("Invalid scale mode...");
		}

		public void SetHeight(float fNewHeight, AxisAlignedBox2f.ScaleMode eScaleMode)
		{
			switch (eScaleMode)
			{
			case AxisAlignedBox2f.ScaleMode.ScaleUp:
				this.Max.y = this.Min.y + fNewHeight;
				return;
			case AxisAlignedBox2f.ScaleMode.ScaleDown:
				this.Min.y = this.Max.y - fNewHeight;
				return;
			case AxisAlignedBox2f.ScaleMode.ScaleCenter:
			{
				Vector2f center = this.Center;
				this.Min.y = center.y - 0.5f * fNewHeight;
				this.Max.y = center.y + 0.5f * fNewHeight;
				return;
			}
			default:
				throw new Exception("Invalid scale mode...");
			}
		}

		public void Contain(Vector2f v)
		{
			this.Min.x = Math.Min(this.Min.x, v.x);
			this.Min.y = Math.Min(this.Min.y, v.y);
			this.Max.x = Math.Max(this.Max.x, v.x);
			this.Max.y = Math.Max(this.Max.y, v.y);
		}

		public void Contain(AxisAlignedBox2f box)
		{
			this.Min.x = Math.Min(this.Min.x, box.Min.x);
			this.Min.y = Math.Min(this.Min.y, box.Min.y);
			this.Max.x = Math.Max(this.Max.x, box.Max.x);
			this.Max.y = Math.Max(this.Max.y, box.Max.y);
		}

		public AxisAlignedBox2f Intersect(AxisAlignedBox2f box)
		{
			AxisAlignedBox2f result = new AxisAlignedBox2f(Math.Max(this.Min.x, box.Min.x), Math.Max(this.Min.y, box.Min.y), Math.Min(this.Max.x, box.Max.x), Math.Min(this.Max.y, box.Max.y));
			if (result.Height <= 0f || result.Width <= 0f)
			{
				return AxisAlignedBox2f.Empty;
			}
			return result;
		}

		public bool Contains(Vector2f v)
		{
			return this.Min.x < v.x && this.Min.y < v.y && this.Max.x > v.x && this.Max.y > v.y;
		}

		public bool Intersects(AxisAlignedBox2f box)
		{
			return box.Max.x >= this.Min.x && box.Min.x <= this.Max.x && box.Max.y >= this.Min.y && box.Min.y <= this.Max.y;
		}

		public float Distance(Vector2f v)
		{
			float num = Math.Abs(v.x - this.Center.x);
			float num2 = Math.Abs(v.y - this.Center.y);
			float num3 = this.Width * 0.5f;
			float num4 = this.Height * 0.5f;
			if (num < num3 && num2 < num4)
			{
				return 0f;
			}
			if (num > num3 && num2 > num4)
			{
				return (float)Math.Sqrt((double)((num - num3) * (num - num3) + (num2 - num4) * (num2 - num4)));
			}
			if (num > num3)
			{
				return num - num3;
			}
			if (num2 > num4)
			{
				return num2 - num4;
			}
			return 0f;
		}

		public void Translate(Vector2f vTranslate)
		{
			this.Min.Add(vTranslate);
			this.Max.Add(vTranslate);
		}

		public void MoveMin(Vector2f vNewMin)
		{
			this.Max.x = vNewMin.x + (this.Max.x - this.Min.x);
			this.Max.y = vNewMin.y + (this.Max.y - this.Min.y);
			this.Min.Set(vNewMin);
		}

		public void MoveMin(float fNewX, float fNewY)
		{
			this.Max.x = fNewX + (this.Max.x - this.Min.x);
			this.Max.y = fNewY + (this.Max.y - this.Min.y);
			this.Min.Set(fNewX, fNewY);
		}

		public override string ToString()
		{
			return string.Format("[{0:F8},{1:F8}] [{2:F8},{3:F8}]", new object[]
			{
				this.Min.x,
				this.Max.x,
				this.Min.y,
				this.Max.y
			});
		}

		public static implicit operator AxisAlignedBox2f(Rect b)
		{
			return new AxisAlignedBox2f(b.min, b.max);
		}

		public static implicit operator Rect(AxisAlignedBox2f b)
		{
			return new Rect
			{
				min = b.Min,
				max = b.Max
			};
		}

		public Vector2f Min;

		public Vector2f Max;

		public static readonly AxisAlignedBox2f Empty = new AxisAlignedBox2f(false);

		public static readonly AxisAlignedBox2f Zero = new AxisAlignedBox2f(0f);

		public static readonly AxisAlignedBox2f UnitPositive = new AxisAlignedBox2f(1f);

		public static readonly AxisAlignedBox2f Infinite = new AxisAlignedBox2f(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);

		public enum ScaleMode
		{
			ScaleRight,
			ScaleLeft,
			ScaleUp,
			ScaleDown,
			ScaleCenter
		}
	}
}
