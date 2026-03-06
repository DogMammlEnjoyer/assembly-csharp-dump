using System;
using UnityEngine;

namespace g3
{
	public struct AxisAlignedBox3d : IComparable<AxisAlignedBox3d>, IEquatable<AxisAlignedBox3d>
	{
		public AxisAlignedBox3d(bool bIgnore)
		{
			this.Min = new Vector3d(double.MaxValue, double.MaxValue, double.MaxValue);
			this.Max = new Vector3d(double.MinValue, double.MinValue, double.MinValue);
		}

		public AxisAlignedBox3d(double xmin, double ymin, double zmin, double xmax, double ymax, double zmax)
		{
			this.Min = new Vector3d(xmin, ymin, zmin);
			this.Max = new Vector3d(xmax, ymax, zmax);
		}

		public AxisAlignedBox3d(double fCubeSize)
		{
			this.Min = new Vector3d(0.0, 0.0, 0.0);
			this.Max = new Vector3d(fCubeSize, fCubeSize, fCubeSize);
		}

		public AxisAlignedBox3d(double fWidth, double fHeight, double fDepth)
		{
			this.Min = new Vector3d(0.0, 0.0, 0.0);
			this.Max = new Vector3d(fWidth, fHeight, fDepth);
		}

		public AxisAlignedBox3d(Vector3d vMin, Vector3d vMax)
		{
			this.Min = new Vector3d(Math.Min(vMin.x, vMax.x), Math.Min(vMin.y, vMax.y), Math.Min(vMin.z, vMax.z));
			this.Max = new Vector3d(Math.Max(vMin.x, vMax.x), Math.Max(vMin.y, vMax.y), Math.Max(vMin.z, vMax.z));
		}

		public AxisAlignedBox3d(ref Vector3d vMin, ref Vector3d vMax)
		{
			this.Min = new Vector3d(Math.Min(vMin.x, vMax.x), Math.Min(vMin.y, vMax.y), Math.Min(vMin.z, vMax.z));
			this.Max = new Vector3d(Math.Max(vMin.x, vMax.x), Math.Max(vMin.y, vMax.y), Math.Max(vMin.z, vMax.z));
		}

		public AxisAlignedBox3d(Vector3d vCenter, double fHalfWidth, double fHalfHeight, double fHalfDepth)
		{
			this.Min = new Vector3d(vCenter.x - fHalfWidth, vCenter.y - fHalfHeight, vCenter.z - fHalfDepth);
			this.Max = new Vector3d(vCenter.x + fHalfWidth, vCenter.y + fHalfHeight, vCenter.z + fHalfDepth);
		}

		public AxisAlignedBox3d(ref Vector3d vCenter, double fHalfWidth, double fHalfHeight, double fHalfDepth)
		{
			this.Min = new Vector3d(vCenter.x - fHalfWidth, vCenter.y - fHalfHeight, vCenter.z - fHalfDepth);
			this.Max = new Vector3d(vCenter.x + fHalfWidth, vCenter.y + fHalfHeight, vCenter.z + fHalfDepth);
		}

		public AxisAlignedBox3d(Vector3d vCenter, double fHalfSize)
		{
			this.Min = new Vector3d(vCenter.x - fHalfSize, vCenter.y - fHalfSize, vCenter.z - fHalfSize);
			this.Max = new Vector3d(vCenter.x + fHalfSize, vCenter.y + fHalfSize, vCenter.z + fHalfSize);
		}

		public AxisAlignedBox3d(Vector3d vCenter)
		{
			this.Max = vCenter;
			this.Min = vCenter;
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

		public double Depth
		{
			get
			{
				return Math.Max(this.Max.z - this.Min.z, 0.0);
			}
		}

		public double Volume
		{
			get
			{
				return this.Width * this.Height * this.Depth;
			}
		}

		public double DiagonalLength
		{
			get
			{
				return Math.Sqrt((this.Max.x - this.Min.x) * (this.Max.x - this.Min.x) + (this.Max.y - this.Min.y) * (this.Max.y - this.Min.y) + (this.Max.z - this.Min.z) * (this.Max.z - this.Min.z));
			}
		}

		public double MaxDim
		{
			get
			{
				return Math.Max(this.Width, Math.Max(this.Height, this.Depth));
			}
		}

		public Vector3d Diagonal
		{
			get
			{
				return new Vector3d(this.Max.x - this.Min.x, this.Max.y - this.Min.y, this.Max.z - this.Min.z);
			}
		}

		public Vector3d Extents
		{
			get
			{
				return new Vector3d((this.Max.x - this.Min.x) * 0.5, (this.Max.y - this.Min.y) * 0.5, (this.Max.z - this.Min.z) * 0.5);
			}
		}

		public Vector3d Center
		{
			get
			{
				return new Vector3d(0.5 * (this.Min.x + this.Max.x), 0.5 * (this.Min.y + this.Max.y), 0.5 * (this.Min.z + this.Max.z));
			}
		}

		public static bool operator ==(AxisAlignedBox3d a, AxisAlignedBox3d b)
		{
			return a.Min == b.Min && a.Max == b.Max;
		}

		public static bool operator !=(AxisAlignedBox3d a, AxisAlignedBox3d b)
		{
			return a.Min != b.Min || a.Max != b.Max;
		}

		public override bool Equals(object obj)
		{
			return this == (AxisAlignedBox3d)obj;
		}

		public bool Equals(AxisAlignedBox3d other)
		{
			return this == other;
		}

		public int CompareTo(AxisAlignedBox3d other)
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

		public Vector3d Corner(int i)
		{
			double x = ((i & 1) != 0 ^ (i & 2) != 0) ? this.Max.x : this.Min.x;
			double y = (i / 2 % 2 == 0) ? this.Min.y : this.Max.y;
			double z = (i < 4) ? this.Min.z : this.Max.z;
			return new Vector3d(x, y, z);
		}

		public Vector3d Point(int xi, int yi, int zi)
		{
			double x = (xi < 0) ? this.Min.x : ((xi == 0) ? (0.5 * (this.Min.x + this.Max.x)) : this.Max.x);
			double y = (yi < 0) ? this.Min.y : ((yi == 0) ? (0.5 * (this.Min.y + this.Max.y)) : this.Max.y);
			double z = (zi < 0) ? this.Min.z : ((zi == 0) ? (0.5 * (this.Min.z + this.Max.z)) : this.Max.z);
			return new Vector3d(x, y, z);
		}

		public void Expand(double fRadius)
		{
			this.Min.x = this.Min.x - fRadius;
			this.Min.y = this.Min.y - fRadius;
			this.Min.z = this.Min.z - fRadius;
			this.Max.x = this.Max.x + fRadius;
			this.Max.y = this.Max.y + fRadius;
			this.Max.z = this.Max.z + fRadius;
		}

		public AxisAlignedBox3d Expanded(double fRadius)
		{
			return new AxisAlignedBox3d(this.Min.x - fRadius, this.Min.y - fRadius, this.Min.z - fRadius, this.Max.x + fRadius, this.Max.y + fRadius, this.Max.z + fRadius);
		}

		public void Contract(double fRadius)
		{
			double num = 2.0 * fRadius;
			if (num > this.Max.x - this.Min.x)
			{
				this.Min.x = (this.Max.x = 0.5 * (this.Min.x + this.Max.x));
			}
			else
			{
				this.Min.x = this.Min.x + fRadius;
				this.Max.x = this.Max.x - fRadius;
			}
			if (num > this.Max.y - this.Min.y)
			{
				this.Min.y = (this.Max.y = 0.5 * (this.Min.y + this.Max.y));
			}
			else
			{
				this.Min.y = this.Min.y + fRadius;
				this.Max.y = this.Max.y - fRadius;
			}
			if (num > this.Max.z - this.Min.z)
			{
				this.Min.z = (this.Max.z = 0.5 * (this.Min.z + this.Max.z));
				return;
			}
			this.Min.z = this.Min.z + fRadius;
			this.Max.z = this.Max.z - fRadius;
		}

		public AxisAlignedBox3d Contracted(double fRadius)
		{
			AxisAlignedBox3d axisAlignedBox3d = new AxisAlignedBox3d(this.Min.x + fRadius, this.Min.y + fRadius, this.Min.z + fRadius, this.Max.x - fRadius, this.Max.y - fRadius, this.Max.z - fRadius);
			if (axisAlignedBox3d.Min.x > axisAlignedBox3d.Max.x)
			{
				axisAlignedBox3d.Min.x = (axisAlignedBox3d.Max.x = 0.5 * (this.Min.x + this.Max.x));
			}
			if (axisAlignedBox3d.Min.y > axisAlignedBox3d.Max.y)
			{
				axisAlignedBox3d.Min.y = (axisAlignedBox3d.Max.y = 0.5 * (this.Min.y + this.Max.y));
			}
			if (axisAlignedBox3d.Min.z > axisAlignedBox3d.Max.z)
			{
				axisAlignedBox3d.Min.z = (axisAlignedBox3d.Max.z = 0.5 * (this.Min.z + this.Max.z));
			}
			return axisAlignedBox3d;
		}

		public void Scale(double sx, double sy, double sz)
		{
			Vector3d center = this.Center;
			Vector3d extents = this.Extents;
			extents.x *= sx;
			extents.y *= sy;
			extents.z *= sz;
			this.Min = new Vector3d(center.x - extents.x, center.y - extents.y, center.z - extents.z);
			this.Max = new Vector3d(center.x + extents.x, center.y + extents.y, center.z + extents.z);
		}

		public void Contain(Vector3d v)
		{
			this.Min.x = Math.Min(this.Min.x, v.x);
			this.Min.y = Math.Min(this.Min.y, v.y);
			this.Min.z = Math.Min(this.Min.z, v.z);
			this.Max.x = Math.Max(this.Max.x, v.x);
			this.Max.y = Math.Max(this.Max.y, v.y);
			this.Max.z = Math.Max(this.Max.z, v.z);
		}

		public void Contain(ref Vector3d v)
		{
			this.Min.x = Math.Min(this.Min.x, v.x);
			this.Min.y = Math.Min(this.Min.y, v.y);
			this.Min.z = Math.Min(this.Min.z, v.z);
			this.Max.x = Math.Max(this.Max.x, v.x);
			this.Max.y = Math.Max(this.Max.y, v.y);
			this.Max.z = Math.Max(this.Max.z, v.z);
		}

		public void Contain(AxisAlignedBox3d box)
		{
			this.Min.x = Math.Min(this.Min.x, box.Min.x);
			this.Min.y = Math.Min(this.Min.y, box.Min.y);
			this.Min.z = Math.Min(this.Min.z, box.Min.z);
			this.Max.x = Math.Max(this.Max.x, box.Max.x);
			this.Max.y = Math.Max(this.Max.y, box.Max.y);
			this.Max.z = Math.Max(this.Max.z, box.Max.z);
		}

		public void Contain(ref AxisAlignedBox3d box)
		{
			this.Min.x = Math.Min(this.Min.x, box.Min.x);
			this.Min.y = Math.Min(this.Min.y, box.Min.y);
			this.Min.z = Math.Min(this.Min.z, box.Min.z);
			this.Max.x = Math.Max(this.Max.x, box.Max.x);
			this.Max.y = Math.Max(this.Max.y, box.Max.y);
			this.Max.z = Math.Max(this.Max.z, box.Max.z);
		}

		public AxisAlignedBox3d Intersect(AxisAlignedBox3d box)
		{
			AxisAlignedBox3d result = new AxisAlignedBox3d(Math.Max(this.Min.x, box.Min.x), Math.Max(this.Min.y, box.Min.y), Math.Max(this.Min.z, box.Min.z), Math.Min(this.Max.x, box.Max.x), Math.Min(this.Max.y, box.Max.y), Math.Min(this.Max.z, box.Max.z));
			if (result.Height <= 0.0 || result.Width <= 0.0 || result.Depth <= 0.0)
			{
				return AxisAlignedBox3d.Empty;
			}
			return result;
		}

		public bool Contains(Vector3d v)
		{
			return this.Min.x <= v.x && this.Min.y <= v.y && this.Min.z <= v.z && this.Max.x >= v.x && this.Max.y >= v.y && this.Max.z >= v.z;
		}

		public bool Contains(ref Vector3d v)
		{
			return this.Min.x <= v.x && this.Min.y <= v.y && this.Min.z <= v.z && this.Max.x >= v.x && this.Max.y >= v.y && this.Max.z >= v.z;
		}

		public bool Contains(AxisAlignedBox3d box2)
		{
			return this.Contains(ref box2.Min) && this.Contains(ref box2.Max);
		}

		public bool Contains(ref AxisAlignedBox3d box2)
		{
			return this.Contains(ref box2.Min) && this.Contains(ref box2.Max);
		}

		public bool Intersects(AxisAlignedBox3d box)
		{
			return box.Max.x > this.Min.x && box.Min.x < this.Max.x && box.Max.y > this.Min.y && box.Min.y < this.Max.y && box.Max.z > this.Min.z && box.Min.z < this.Max.z;
		}

		public double DistanceSquared(Vector3d v)
		{
			object obj = (v.x < this.Min.x) ? (this.Min.x - v.x) : ((v.x > this.Max.x) ? (v.x - this.Max.x) : 0.0);
			double num = (v.y < this.Min.y) ? (this.Min.y - v.y) : ((v.y > this.Max.y) ? (v.y - this.Max.y) : 0.0);
			double num2 = (v.z < this.Min.z) ? (this.Min.z - v.z) : ((v.z > this.Max.z) ? (v.z - this.Max.z) : 0.0);
			object obj2 = obj;
			return obj2 * obj2 + num * num + num2 * num2;
		}

		public double Distance(Vector3d v)
		{
			return Math.Sqrt(this.DistanceSquared(v));
		}

		public double SignedDistance(Vector3d v)
		{
			if (!this.Contains(v))
			{
				return this.Distance(v);
			}
			double a = Math.Min(Math.Abs(v.x - this.Min.x), Math.Abs(v.x - this.Max.x));
			double b = Math.Min(Math.Abs(v.y - this.Min.y), Math.Abs(v.y - this.Max.y));
			double c = Math.Min(Math.Abs(v.z - this.Min.z), Math.Abs(v.z - this.Max.z));
			return -MathUtil.Min(a, b, c);
		}

		public double DistanceSquared(ref AxisAlignedBox3d box2)
		{
			double num = Math.Abs(box2.Min.x + box2.Max.x - (this.Min.x + this.Max.x)) - (this.Max.x - this.Min.x + (box2.Max.x - box2.Min.x));
			if (num < 0.0)
			{
				num = 0.0;
			}
			double num2 = Math.Abs(box2.Min.y + box2.Max.y - (this.Min.y + this.Max.y)) - (this.Max.y - this.Min.y + (box2.Max.y - box2.Min.y));
			if (num2 < 0.0)
			{
				num2 = 0.0;
			}
			double num3 = Math.Abs(box2.Min.z + box2.Max.z - (this.Min.z + this.Max.z)) - (this.Max.z - this.Min.z + (box2.Max.z - box2.Min.z));
			if (num3 < 0.0)
			{
				num3 = 0.0;
			}
			return 0.25 * (num * num + num2 * num2 + num3 * num3);
		}

		public void Translate(Vector3d vTranslate)
		{
			this.Min.Add(vTranslate);
			this.Max.Add(vTranslate);
		}

		public void MoveMin(Vector3d vNewMin)
		{
			this.Max.x = vNewMin.x + (this.Max.x - this.Min.x);
			this.Max.y = vNewMin.y + (this.Max.y - this.Min.y);
			this.Max.z = vNewMin.z + (this.Max.z - this.Min.z);
			this.Min.Set(vNewMin);
		}

		public void MoveMin(double fNewX, double fNewY, double fNewZ)
		{
			this.Max.x = fNewX + (this.Max.x - this.Min.x);
			this.Max.y = fNewY + (this.Max.y - this.Min.y);
			this.Max.z = fNewZ + (this.Max.z - this.Min.z);
			this.Min.Set(fNewX, fNewY, fNewZ);
		}

		public override string ToString()
		{
			return string.Format("x[{0:F8},{1:F8}] y[{2:F8},{3:F8}] z[{4:F8},{5:F8}]", new object[]
			{
				this.Min.x,
				this.Max.x,
				this.Min.y,
				this.Max.y,
				this.Min.z,
				this.Max.z
			});
		}

		public static implicit operator AxisAlignedBox3d(AxisAlignedBox3f v)
		{
			return new AxisAlignedBox3d(v.Min, v.Max);
		}

		public static explicit operator AxisAlignedBox3f(AxisAlignedBox3d v)
		{
			return new AxisAlignedBox3f((Vector3f)v.Min, (Vector3f)v.Max);
		}

		public static implicit operator AxisAlignedBox3d(Bounds b)
		{
			return new AxisAlignedBox3f(b.min, b.max);
		}

		public static explicit operator Bounds(AxisAlignedBox3d b)
		{
			Bounds result = default(Bounds);
			result.SetMinMax((Vector3)b.Min, (Vector3)b.Max);
			return result;
		}

		public Vector3d Min;

		public Vector3d Max;

		public static readonly AxisAlignedBox3d Empty = new AxisAlignedBox3d(false);

		public static readonly AxisAlignedBox3d Zero = new AxisAlignedBox3d(0.0);

		public static readonly AxisAlignedBox3d UnitPositive = new AxisAlignedBox3d(1.0);

		public static readonly AxisAlignedBox3d Infinite = new AxisAlignedBox3d(double.MinValue, double.MinValue, double.MinValue, double.MaxValue, double.MaxValue, double.MaxValue);
	}
}
