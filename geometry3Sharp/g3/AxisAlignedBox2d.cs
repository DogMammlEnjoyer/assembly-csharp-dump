using System;
using System.Collections.Generic;
using UnityEngine;

namespace g3
{
	public struct AxisAlignedBox2d
	{
		public AxisAlignedBox2d(bool bIgnore)
		{
			this.Min = new Vector2d(double.MaxValue, double.MaxValue);
			this.Max = new Vector2d(double.MinValue, double.MinValue);
		}

		public AxisAlignedBox2d(double xmin, double ymin, double xmax, double ymax)
		{
			this.Min = new Vector2d(xmin, ymin);
			this.Max = new Vector2d(xmax, ymax);
		}

		public AxisAlignedBox2d(double fSquareSize)
		{
			this.Min = new Vector2d(0f, 0f);
			this.Max = new Vector2d(fSquareSize, fSquareSize);
		}

		public AxisAlignedBox2d(double fWidth, double fHeight)
		{
			this.Min = new Vector2d(0f, 0f);
			this.Max = new Vector2d(fWidth, fHeight);
		}

		public AxisAlignedBox2d(Vector2d vMin, Vector2d vMax)
		{
			this.Min = new Vector2d(Math.Min(vMin.x, vMax.x), Math.Min(vMin.y, vMax.y));
			this.Max = new Vector2d(Math.Max(vMin.x, vMax.x), Math.Max(vMin.y, vMax.y));
		}

		public AxisAlignedBox2d(Vector2d vCenter, double fHalfWidth, double fHalfHeight)
		{
			this.Min = new Vector2d(vCenter.x - fHalfWidth, vCenter.y - fHalfHeight);
			this.Max = new Vector2d(vCenter.x + fHalfWidth, vCenter.y + fHalfHeight);
		}

		public AxisAlignedBox2d(Vector2d vCenter, double fHalfWidth)
		{
			this.Min = new Vector2d(vCenter.x - fHalfWidth, vCenter.y - fHalfWidth);
			this.Max = new Vector2d(vCenter.x + fHalfWidth, vCenter.y + fHalfWidth);
		}

		public AxisAlignedBox2d(Vector2d vCenter)
		{
			this.Max = vCenter;
			this.Min = vCenter;
		}

		public AxisAlignedBox2d(AxisAlignedBox2d o)
		{
			this.Min = new Vector2d(o.Min);
			this.Max = new Vector2d(o.Max);
		}

		public double Width
		{
			get
			{
				return Math.Max(this.Max.x - this.Min.x, 0.0);
			}
		}

		public double Height
		{
			get
			{
				return Math.Max(this.Max.y - this.Min.y, 0.0);
			}
		}

		public double Area
		{
			get
			{
				return this.Width * this.Height;
			}
		}

		public double DiagonalLength
		{
			get
			{
				return Math.Sqrt((this.Max.x - this.Min.x) * (this.Max.x - this.Min.x) + (this.Max.y - this.Min.y) * (this.Max.y - this.Min.y));
			}
		}

		public double MaxDim
		{
			get
			{
				return Math.Max(this.Width, this.Height);
			}
		}

		public double MinDim
		{
			get
			{
				return Math.Min(this.Width, this.Height);
			}
		}

		public double MaxUnsignedCoordinate
		{
			get
			{
				return Math.Max(Math.Max(Math.Abs(this.Min.x), Math.Abs(this.Max.x)), Math.Max(Math.Abs(this.Min.y), Math.Abs(this.Max.y)));
			}
		}

		public Vector2d Diagonal
		{
			get
			{
				return new Vector2d(this.Max.x - this.Min.x, this.Max.y - this.Min.y);
			}
		}

		public Vector2d Center
		{
			get
			{
				return new Vector2d(0.5 * (this.Min.x + this.Max.x), 0.5 * (this.Min.y + this.Max.y));
			}
		}

		public Vector2d GetCorner(int i)
		{
			return new Vector2d((i % 3 == 0) ? this.Min.x : this.Max.x, (i < 2) ? this.Min.y : this.Max.y);
		}

		public Vector2d SampleT(double tx, double sy)
		{
			return new Vector2d((1.0 - tx) * this.Min.x + tx * this.Max.x, (1.0 - sy) * this.Min.y + sy * this.Max.y);
		}

		public void Expand(double fRadius)
		{
			this.Min.x = this.Min.x - fRadius;
			this.Min.y = this.Min.y - fRadius;
			this.Max.x = this.Max.x + fRadius;
			this.Max.y = this.Max.y + fRadius;
		}

		public void Contract(double fRadius)
		{
			this.Min.x = this.Min.x + fRadius;
			this.Min.y = this.Min.y + fRadius;
			this.Max.x = this.Max.x - fRadius;
			this.Max.y = this.Max.y - fRadius;
		}

		[Obsolete("This method name is confusing. Will remove in future. Use Add() instead")]
		public void Pad(double fPadLeft, double fPadRight, double fPadBottom, double fPadTop)
		{
			this.Min.x = this.Min.x + fPadLeft;
			this.Min.y = this.Min.y + fPadBottom;
			this.Max.x = this.Max.x + fPadRight;
			this.Max.y = this.Max.y + fPadTop;
		}

		public void Add(double left, double right, double bottom, double top)
		{
			this.Min.x = this.Min.x + left;
			this.Min.y = this.Min.y + bottom;
			this.Max.x = this.Max.x + right;
			this.Max.y = this.Max.y + top;
		}

		public void SetWidth(double fNewWidth, AxisAlignedBox2d.ScaleMode eScaleMode)
		{
			switch (eScaleMode)
			{
			case AxisAlignedBox2d.ScaleMode.ScaleRight:
				this.Max.x = this.Min.x + fNewWidth;
				return;
			case AxisAlignedBox2d.ScaleMode.ScaleLeft:
				this.Min.x = this.Max.x - fNewWidth;
				return;
			case AxisAlignedBox2d.ScaleMode.ScaleCenter:
			{
				Vector2d center = this.Center;
				this.Min.x = center.x - 0.5 * fNewWidth;
				this.Max.x = center.x + 0.5 * fNewWidth;
				return;
			}
			}
			throw new Exception("Invalid scale mode...");
		}

		public void SetHeight(double fNewHeight, AxisAlignedBox2d.ScaleMode eScaleMode)
		{
			switch (eScaleMode)
			{
			case AxisAlignedBox2d.ScaleMode.ScaleUp:
				this.Max.y = this.Min.y + fNewHeight;
				return;
			case AxisAlignedBox2d.ScaleMode.ScaleDown:
				this.Min.y = this.Max.y - fNewHeight;
				return;
			case AxisAlignedBox2d.ScaleMode.ScaleCenter:
			{
				Vector2d center = this.Center;
				this.Min.y = center.y - 0.5 * fNewHeight;
				this.Max.y = center.y + 0.5 * fNewHeight;
				return;
			}
			default:
				throw new Exception("Invalid scale mode...");
			}
		}

		public void Contain(Vector2d v)
		{
			if (v.x < this.Min.x)
			{
				this.Min.x = v.x;
			}
			if (v.x > this.Max.x)
			{
				this.Max.x = v.x;
			}
			if (v.y < this.Min.y)
			{
				this.Min.y = v.y;
			}
			if (v.y > this.Max.y)
			{
				this.Max.y = v.y;
			}
		}

		public void Contain(ref Vector2d v)
		{
			if (v.x < this.Min.x)
			{
				this.Min.x = v.x;
			}
			if (v.x > this.Max.x)
			{
				this.Max.x = v.x;
			}
			if (v.y < this.Min.y)
			{
				this.Min.y = v.y;
			}
			if (v.y > this.Max.y)
			{
				this.Max.y = v.y;
			}
		}

		public void Contain(AxisAlignedBox2d box)
		{
			if (box.Min.x < this.Min.x)
			{
				this.Min.x = box.Min.x;
			}
			if (box.Max.x > this.Max.x)
			{
				this.Max.x = box.Max.x;
			}
			if (box.Min.y < this.Min.y)
			{
				this.Min.y = box.Min.y;
			}
			if (box.Max.y > this.Max.y)
			{
				this.Max.y = box.Max.y;
			}
		}

		public void Contain(ref AxisAlignedBox2d box)
		{
			if (box.Min.x < this.Min.x)
			{
				this.Min.x = box.Min.x;
			}
			if (box.Max.x > this.Max.x)
			{
				this.Max.x = box.Max.x;
			}
			if (box.Min.y < this.Min.y)
			{
				this.Min.y = box.Min.y;
			}
			if (box.Max.y > this.Max.y)
			{
				this.Max.y = box.Max.y;
			}
		}

		public void Contain(IList<Vector2d> points)
		{
			int count = points.Count;
			if (count > 0)
			{
				Vector2d vector2d = points[0];
				this.Contain(ref vector2d);
				for (int i = 1; i < count; i++)
				{
					vector2d = points[i];
					if (vector2d.x < this.Min.x)
					{
						this.Min.x = vector2d.x;
					}
					else if (vector2d.x > this.Max.x)
					{
						this.Max.x = vector2d.x;
					}
					if (vector2d.y < this.Min.y)
					{
						this.Min.y = vector2d.y;
					}
					else if (vector2d.y > this.Max.y)
					{
						this.Max.y = vector2d.y;
					}
				}
			}
		}

		public AxisAlignedBox2d Intersect(AxisAlignedBox2d box)
		{
			AxisAlignedBox2d result = new AxisAlignedBox2d(Math.Max(this.Min.x, box.Min.x), Math.Max(this.Min.y, box.Min.y), Math.Min(this.Max.x, box.Max.x), Math.Min(this.Max.y, box.Max.y));
			if (result.Height <= 0.0 || result.Width <= 0.0)
			{
				return AxisAlignedBox2d.Empty;
			}
			return result;
		}

		public bool Contains(Vector2d v)
		{
			return this.Min.x < v.x && this.Min.y < v.y && this.Max.x > v.x && this.Max.y > v.y;
		}

		public bool Contains(ref Vector2d v)
		{
			return this.Min.x < v.x && this.Min.y < v.y && this.Max.x > v.x && this.Max.y > v.y;
		}

		public bool Contains(AxisAlignedBox2d box2)
		{
			return this.Contains(ref box2.Min) && this.Contains(ref box2.Max);
		}

		public bool Contains(ref AxisAlignedBox2d box2)
		{
			return this.Contains(ref box2.Min) && this.Contains(ref box2.Max);
		}

		public bool Intersects(AxisAlignedBox2d box)
		{
			return box.Max.x >= this.Min.x && box.Min.x <= this.Max.x && box.Max.y >= this.Min.y && box.Min.y <= this.Max.y;
		}

		public bool Intersects(ref AxisAlignedBox2d box)
		{
			return box.Max.x >= this.Min.x && box.Min.x <= this.Max.x && box.Max.y >= this.Min.y && box.Min.y <= this.Max.y;
		}

		public double Distance(Vector2d v)
		{
			double num = Math.Abs(v.x - this.Center.x);
			double num2 = Math.Abs(v.y - this.Center.y);
			double num3 = this.Width * 0.5;
			double num4 = this.Height * 0.5;
			if (num < num3 && num2 < num4)
			{
				return 0.0;
			}
			if (num > num3 && num2 > num4)
			{
				return Math.Sqrt((num - num3) * (num - num3) + (num2 - num4) * (num2 - num4));
			}
			if (num > num3)
			{
				return num - num3;
			}
			if (num2 > num4)
			{
				return num2 - num4;
			}
			return 0.0;
		}

		public void Translate(Vector2d vTranslate)
		{
			this.Min.Add(vTranslate);
			this.Max.Add(vTranslate);
		}

		public void Scale(double scale)
		{
			this.Min *= scale;
			this.Max *= scale;
		}

		public void Scale(double scale, Vector2d origin)
		{
			this.Min = (this.Min - origin) * scale + origin;
			this.Max = (this.Max - origin) * scale + origin;
		}

		public void MoveMin(Vector2d vNewMin)
		{
			this.Max.x = vNewMin.x + (this.Max.x - this.Min.x);
			this.Max.y = vNewMin.y + (this.Max.y - this.Min.y);
			this.Min.Set(vNewMin);
		}

		public void MoveMin(double fNewX, double fNewY)
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

		public static implicit operator AxisAlignedBox2d(Rect b)
		{
			return new AxisAlignedBox2d(b.min, b.max);
		}

		public static explicit operator Rect(AxisAlignedBox2d b)
		{
			return new Rect
			{
				min = (Vector2)b.Min,
				max = (Vector2)b.Max
			};
		}

		public Vector2d Min;

		public Vector2d Max;

		public static readonly AxisAlignedBox2d Empty = new AxisAlignedBox2d(false);

		public static readonly AxisAlignedBox2d Zero = new AxisAlignedBox2d(0.0);

		public static readonly AxisAlignedBox2d UnitPositive = new AxisAlignedBox2d(1.0);

		public static readonly AxisAlignedBox2d Infinite = new AxisAlignedBox2d(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue);

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
