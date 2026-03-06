using System;
using System.Collections.Generic;

namespace g3
{
	public struct AxisAlignedBox2i : IComparable<AxisAlignedBox2i>, IEquatable<AxisAlignedBox2i>
	{
		public AxisAlignedBox2i(bool bIgnore)
		{
			this.Min = new Vector2i(int.MaxValue, int.MaxValue);
			this.Max = new Vector2i(int.MinValue, int.MinValue);
		}

		public AxisAlignedBox2i(int xmin, int ymin, int xmax, int ymax)
		{
			this.Min = new Vector2i(xmin, ymin);
			this.Max = new Vector2i(xmax, ymax);
		}

		public AxisAlignedBox2i(int fCubeSize)
		{
			this.Min = new Vector2i(0, 0);
			this.Max = new Vector2i(fCubeSize, fCubeSize);
		}

		public AxisAlignedBox2i(int fWidth, int fHeight)
		{
			this.Min = new Vector2i(0, 0);
			this.Max = new Vector2i(fWidth, fHeight);
		}

		public AxisAlignedBox2i(Vector2i vMin, Vector2i vMax)
		{
			this.Min = new Vector2i(Math.Min(vMin.x, vMax.x), Math.Min(vMin.y, vMax.y));
			this.Max = new Vector2i(Math.Max(vMin.x, vMax.x), Math.Max(vMin.y, vMax.y));
		}

		public AxisAlignedBox2i(Vector2i vCenter, int fHalfWidth, int fHalfHeight, int fHalfDepth)
		{
			this.Min = new Vector2i(vCenter.x - fHalfWidth, vCenter.y - fHalfHeight);
			this.Max = new Vector2i(vCenter.x + fHalfWidth, vCenter.y + fHalfHeight);
		}

		public AxisAlignedBox2i(Vector2i vCenter, int fHalfSize)
		{
			this.Min = new Vector2i(vCenter.x - fHalfSize, vCenter.y - fHalfSize);
			this.Max = new Vector2i(vCenter.x + fHalfSize, vCenter.y + fHalfSize);
		}

		public AxisAlignedBox2i(Vector2i vCenter)
		{
			this.Max = vCenter;
			this.Min = vCenter;
		}

		public int Width
		{
			get
			{
				return Math.Max(this.Max.x - this.Min.x, 0);
			}
		}

		public int Height
		{
			get
			{
				return Math.Max(this.Max.y - this.Min.y, 0);
			}
		}

		public int Area
		{
			get
			{
				return this.Width * this.Height;
			}
		}

		public int DiagonalLength
		{
			get
			{
				return (int)Math.Sqrt((double)((this.Max.x - this.Min.x) * (this.Max.x - this.Min.x) + (this.Max.y - this.Min.y) * (this.Max.y - this.Min.y)));
			}
		}

		public int MaxDim
		{
			get
			{
				return Math.Max(this.Width, this.Height);
			}
		}

		public Vector2i Diagonal
		{
			get
			{
				return new Vector2i(this.Max.x - this.Min.x, this.Max.y - this.Min.y);
			}
		}

		public Vector2i Extents
		{
			get
			{
				return new Vector2i((this.Max.x - this.Min.x) / 2, (this.Max.y - this.Min.y) / 2);
			}
		}

		public Vector2i Center
		{
			get
			{
				return new Vector2i((this.Min.x + this.Max.x) / 2, (this.Min.y + this.Max.y) / 2);
			}
		}

		public static bool operator ==(AxisAlignedBox2i a, AxisAlignedBox2i b)
		{
			return a.Min == b.Min && a.Max == b.Max;
		}

		public static bool operator !=(AxisAlignedBox2i a, AxisAlignedBox2i b)
		{
			return a.Min != b.Min || a.Max != b.Max;
		}

		public override bool Equals(object obj)
		{
			return this == (AxisAlignedBox2i)obj;
		}

		public bool Equals(AxisAlignedBox2i other)
		{
			return this == other;
		}

		public int CompareTo(AxisAlignedBox2i other)
		{
			int num = this.Min.CompareTo(other.Min);
			if (num == 0)
			{
				return this.Max.CompareTo(other.Max);
			}
			return num;
		}

		public override int GetHashCode()
		{
			return (-2128831035 * 16777619 ^ this.Min.GetHashCode()) * 16777619 ^ this.Max.GetHashCode();
		}

		public Vector2i GetCorner(int i)
		{
			return new Vector2i((i % 3 == 0) ? this.Min.x : this.Max.x, (i < 2) ? this.Min.y : this.Max.y);
		}

		public void Expand(int nRadius)
		{
			this.Min.x = this.Min.x - nRadius;
			this.Min.y = this.Min.y - nRadius;
			this.Max.x = this.Max.x + nRadius;
			this.Max.y = this.Max.y + nRadius;
		}

		public void Contract(int nRadius)
		{
			this.Min.x = this.Min.x + nRadius;
			this.Min.y = this.Min.y + nRadius;
			this.Max.x = this.Max.x - nRadius;
			this.Max.y = this.Max.y - nRadius;
		}

		public void Scale(int sx, int sy, int sz)
		{
			Vector2i center = this.Center;
			Vector2i extents = this.Extents;
			extents.x *= sx;
			extents.y *= sy;
			this.Min = new Vector2i(center.x - extents.x, center.y - extents.y);
			this.Max = new Vector2i(center.x + extents.x, center.y + extents.y);
		}

		public void Contain(Vector2i v)
		{
			this.Min.x = Math.Min(this.Min.x, v.x);
			this.Min.y = Math.Min(this.Min.y, v.y);
			this.Max.x = Math.Max(this.Max.x, v.x);
			this.Max.y = Math.Max(this.Max.y, v.y);
		}

		public void Contain(AxisAlignedBox2i box)
		{
			this.Min.x = Math.Min(this.Min.x, box.Min.x);
			this.Min.y = Math.Min(this.Min.y, box.Min.y);
			this.Max.x = Math.Max(this.Max.x, box.Max.x);
			this.Max.y = Math.Max(this.Max.y, box.Max.y);
		}

		public void Contain(Vector3d v)
		{
			this.Min.x = Math.Min(this.Min.x, (int)v.x);
			this.Min.y = Math.Min(this.Min.y, (int)v.y);
			this.Max.x = Math.Max(this.Max.x, (int)v.x);
			this.Max.y = Math.Max(this.Max.y, (int)v.y);
		}

		public void Contain(AxisAlignedBox3d box)
		{
			this.Min.x = Math.Min(this.Min.x, (int)box.Min.x);
			this.Min.y = Math.Min(this.Min.y, (int)box.Min.y);
			this.Max.x = Math.Max(this.Max.x, (int)box.Max.x);
			this.Max.y = Math.Max(this.Max.y, (int)box.Max.y);
		}

		public AxisAlignedBox2i Intersect(AxisAlignedBox2i box)
		{
			AxisAlignedBox2i result = new AxisAlignedBox2i(Math.Max(this.Min.x, box.Min.x), Math.Max(this.Min.y, box.Min.y), Math.Min(this.Max.x, box.Max.x), Math.Min(this.Max.y, box.Max.y));
			if (result.Height <= 0 || result.Width <= 0)
			{
				return AxisAlignedBox2i.Empty;
			}
			return result;
		}

		public bool Contains(Vector2i v)
		{
			return this.Min.x <= v.x && this.Min.y <= v.y && this.Max.x >= v.x && this.Max.y >= v.y;
		}

		public bool Contains(ref Vector2i v)
		{
			return this.Min.x <= v.x && this.Min.y <= v.y && this.Max.x >= v.x && this.Max.y >= v.y;
		}

		public bool Contains(AxisAlignedBox2i box)
		{
			return this.Contains(ref box.Min) && this.Contains(ref box.Max);
		}

		public bool Contains(ref AxisAlignedBox2i box)
		{
			return this.Contains(ref box.Min) && this.Contains(ref box.Max);
		}

		public bool Intersects(AxisAlignedBox2i box)
		{
			return box.Max.x > this.Min.x && box.Min.x < this.Max.x && box.Max.y > this.Min.y && box.Min.y < this.Max.y;
		}

		public double DistanceSquared(Vector2i v)
		{
			object obj = (v.x < this.Min.x) ? (this.Min.x - v.x) : ((v.x > this.Max.x) ? (v.x - this.Max.x) : 0);
			int num = (v.y < this.Min.y) ? (this.Min.y - v.y) : ((v.y > this.Max.y) ? (v.y - this.Max.y) : 0);
			object obj2 = obj;
			return obj2 * obj2 + num * num;
		}

		public int Distance(Vector2i v)
		{
			return (int)Math.Sqrt(this.DistanceSquared(v));
		}

		public Vector2i NearestPoint(Vector2i v)
		{
			int x = (v.x < this.Min.x) ? this.Min.x : ((v.x > this.Max.x) ? this.Max.x : v.x);
			int y = (v.y < this.Min.y) ? this.Min.y : ((v.y > this.Max.y) ? this.Max.y : v.y);
			return new Vector2i(x, y);
		}

		public void Translate(Vector2i vTranslate)
		{
			this.Min += vTranslate;
			this.Max += vTranslate;
		}

		public void MoveMin(Vector2i vNewMin)
		{
			this.Max.x = vNewMin.x + (this.Max.x - this.Min.x);
			this.Max.y = vNewMin.y + (this.Max.y - this.Min.y);
			this.Min = vNewMin;
		}

		public void MoveMin(int fNewX, int fNewY)
		{
			this.Max.x = fNewX + (this.Max.x - this.Min.x);
			this.Max.y = fNewY + (this.Max.y - this.Min.y);
			this.Min = new Vector2i(fNewX, fNewY);
		}

		public IEnumerable<Vector2i> IndicesInclusive()
		{
			int num;
			for (int yi = this.Min.y; yi <= this.Max.y; yi = num)
			{
				for (int xi = this.Min.x; xi <= this.Max.x; xi = num)
				{
					yield return new Vector2i(xi, yi);
					num = xi + 1;
				}
				num = yi + 1;
			}
			yield break;
		}

		public IEnumerable<Vector2i> IndicesExclusive()
		{
			int num;
			for (int yi = this.Min.y; yi < this.Max.y; yi = num)
			{
				for (int xi = this.Min.x; xi < this.Max.x; xi = num)
				{
					yield return new Vector2i(xi, yi);
					num = xi + 1;
				}
				num = yi + 1;
			}
			yield break;
		}

		public override string ToString()
		{
			return string.Format("x[{0},{1}] y[{2},{3}]", new object[]
			{
				this.Min.x,
				this.Max.x,
				this.Min.y,
				this.Max.y
			});
		}

		public Vector2i Min;

		public Vector2i Max;

		public static readonly AxisAlignedBox2i Empty = new AxisAlignedBox2i(false);

		public static readonly AxisAlignedBox2i Zero = new AxisAlignedBox2i(0);

		public static readonly AxisAlignedBox2i UnitPositive = new AxisAlignedBox2i(1);

		public static readonly AxisAlignedBox2i Infinite = new AxisAlignedBox2i(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
	}
}
