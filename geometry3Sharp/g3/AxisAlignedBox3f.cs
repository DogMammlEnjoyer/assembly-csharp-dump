using System;
using UnityEngine;

namespace g3
{
	public struct AxisAlignedBox3f : IComparable<AxisAlignedBox3f>, IEquatable<AxisAlignedBox3f>
	{
		public AxisAlignedBox3f(bool bIgnore)
		{
			this.Min = new Vector3f(float.MaxValue, float.MaxValue, float.MaxValue);
			this.Max = new Vector3f(float.MinValue, float.MinValue, float.MinValue);
		}

		public AxisAlignedBox3f(float xmin, float ymin, float zmin, float xmax, float ymax, float zmax)
		{
			this.Min = new Vector3f(xmin, ymin, zmin);
			this.Max = new Vector3f(xmax, ymax, zmax);
		}

		public AxisAlignedBox3f(float fCubeSize)
		{
			this.Min = new Vector3f(0f, 0f, 0f);
			this.Max = new Vector3f(fCubeSize, fCubeSize, fCubeSize);
		}

		public AxisAlignedBox3f(float fWidth, float fHeight, float fDepth)
		{
			this.Min = new Vector3f(0f, 0f, 0f);
			this.Max = new Vector3f(fWidth, fHeight, fDepth);
		}

		public AxisAlignedBox3f(Vector3f vMin, Vector3f vMax)
		{
			this.Min = new Vector3f(Math.Min(vMin.x, vMax.x), Math.Min(vMin.y, vMax.y), Math.Min(vMin.z, vMax.z));
			this.Max = new Vector3f(Math.Max(vMin.x, vMax.x), Math.Max(vMin.y, vMax.y), Math.Max(vMin.z, vMax.z));
		}

		public AxisAlignedBox3f(ref Vector3f vMin, ref Vector3f vMax)
		{
			this.Min = new Vector3f(Math.Min(vMin.x, vMax.x), Math.Min(vMin.y, vMax.y), Math.Min(vMin.z, vMax.z));
			this.Max = new Vector3f(Math.Max(vMin.x, vMax.x), Math.Max(vMin.y, vMax.y), Math.Max(vMin.z, vMax.z));
		}

		public AxisAlignedBox3f(Vector3f vCenter, float fHalfWidth, float fHalfHeight, float fHalfDepth)
		{
			this.Min = new Vector3f(vCenter.x - fHalfWidth, vCenter.y - fHalfHeight, vCenter.z - fHalfDepth);
			this.Max = new Vector3f(vCenter.x + fHalfWidth, vCenter.y + fHalfHeight, vCenter.z + fHalfDepth);
		}

		public AxisAlignedBox3f(ref Vector3f vCenter, float fHalfWidth, float fHalfHeight, float fHalfDepth)
		{
			this.Min = new Vector3f(vCenter.x - fHalfWidth, vCenter.y - fHalfHeight, vCenter.z - fHalfDepth);
			this.Max = new Vector3f(vCenter.x + fHalfWidth, vCenter.y + fHalfHeight, vCenter.z + fHalfDepth);
		}

		public AxisAlignedBox3f(Vector3f vCenter, float fHalfSize)
		{
			this.Min = new Vector3f(vCenter.x - fHalfSize, vCenter.y - fHalfSize, vCenter.z - fHalfSize);
			this.Max = new Vector3f(vCenter.x + fHalfSize, vCenter.y + fHalfSize, vCenter.z + fHalfSize);
		}

		public AxisAlignedBox3f(Vector3f vCenter)
		{
			this.Max = vCenter;
			this.Min = vCenter;
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

		public float Depth
		{
			get
			{
				return Math.Max(this.Max.z - this.Min.z, 0f);
			}
		}

		public float Volume
		{
			get
			{
				return this.Width * this.Height * this.Depth;
			}
		}

		public float DiagonalLength
		{
			get
			{
				return (float)Math.Sqrt((double)((this.Max.x - this.Min.x) * (this.Max.x - this.Min.x) + (this.Max.y - this.Min.y) * (this.Max.y - this.Min.y) + (this.Max.z - this.Min.z) * (this.Max.z - this.Min.z)));
			}
		}

		public float MaxDim
		{
			get
			{
				return Math.Max(this.Width, Math.Max(this.Height, this.Depth));
			}
		}

		public Vector3f Diagonal
		{
			get
			{
				return new Vector3f(this.Max.x - this.Min.x, this.Max.y - this.Min.y, this.Max.z - this.Min.z);
			}
		}

		public Vector3f Extents
		{
			get
			{
				return new Vector3f((double)(this.Max.x - this.Min.x) * 0.5, (double)(this.Max.y - this.Min.y) * 0.5, (double)(this.Max.z - this.Min.z) * 0.5);
			}
		}

		public Vector3f Center
		{
			get
			{
				return new Vector3f(0.5 * (double)(this.Min.x + this.Max.x), 0.5 * (double)(this.Min.y + this.Max.y), 0.5 * (double)(this.Min.z + this.Max.z));
			}
		}

		public static bool operator ==(AxisAlignedBox3f a, AxisAlignedBox3f b)
		{
			return a.Min == b.Min && a.Max == b.Max;
		}

		public static bool operator !=(AxisAlignedBox3f a, AxisAlignedBox3f b)
		{
			return a.Min != b.Min || a.Max != b.Max;
		}

		public override bool Equals(object obj)
		{
			return this == (AxisAlignedBox3f)obj;
		}

		public bool Equals(AxisAlignedBox3f other)
		{
			return this == other;
		}

		public int CompareTo(AxisAlignedBox3f other)
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

		public Vector3f Corner(int i)
		{
			float x = ((i & 1) != 0 ^ (i & 2) != 0) ? this.Max.x : this.Min.x;
			float y = (i / 2 % 2 == 0) ? this.Min.y : this.Max.y;
			float z = (i < 4) ? this.Min.z : this.Max.z;
			return new Vector3f(x, y, z);
		}

		public Vector3f Point(int xi, int yi, int zi)
		{
			float x = (xi < 0) ? this.Min.x : ((xi == 0) ? (0.5f * (this.Min.x + this.Max.x)) : this.Max.x);
			float y = (yi < 0) ? this.Min.y : ((yi == 0) ? (0.5f * (this.Min.y + this.Max.y)) : this.Max.y);
			float z = (zi < 0) ? this.Min.z : ((zi == 0) ? (0.5f * (this.Min.z + this.Max.z)) : this.Max.z);
			return new Vector3f(x, y, z);
		}

		public void Expand(float fRadius)
		{
			this.Min.x = this.Min.x - fRadius;
			this.Min.y = this.Min.y - fRadius;
			this.Min.z = this.Min.z - fRadius;
			this.Max.x = this.Max.x + fRadius;
			this.Max.y = this.Max.y + fRadius;
			this.Max.z = this.Max.z + fRadius;
		}

		public void Contract(float fRadius)
		{
			this.Min.x = this.Min.x + fRadius;
			this.Min.y = this.Min.y + fRadius;
			this.Min.z = this.Min.z + fRadius;
			this.Max.x = this.Max.x - fRadius;
			this.Max.y = this.Max.y - fRadius;
			this.Max.z = this.Max.z - fRadius;
		}

		public void Scale(float sx, float sy, float sz)
		{
			Vector3f center = this.Center;
			Vector3f extents = this.Extents;
			extents.x *= sx;
			extents.y *= sy;
			extents.z *= sz;
			this.Min = new Vector3f(center.x - extents.x, center.y - extents.y, center.z - extents.z);
			this.Max = new Vector3f(center.x + extents.x, center.y + extents.y, center.z + extents.z);
		}

		public void Contain(Vector3f v)
		{
			this.Min.x = Math.Min(this.Min.x, v.x);
			this.Min.y = Math.Min(this.Min.y, v.y);
			this.Min.z = Math.Min(this.Min.z, v.z);
			this.Max.x = Math.Max(this.Max.x, v.x);
			this.Max.y = Math.Max(this.Max.y, v.y);
			this.Max.z = Math.Max(this.Max.z, v.z);
		}

		public void Contain(AxisAlignedBox3f box)
		{
			this.Min.x = Math.Min(this.Min.x, box.Min.x);
			this.Min.y = Math.Min(this.Min.y, box.Min.y);
			this.Min.z = Math.Min(this.Min.z, box.Min.z);
			this.Max.x = Math.Max(this.Max.x, box.Max.x);
			this.Max.y = Math.Max(this.Max.y, box.Max.y);
			this.Max.z = Math.Max(this.Max.z, box.Max.z);
		}

		public void Contain(Vector3d v)
		{
			this.Min.x = Math.Min(this.Min.x, (float)v.x);
			this.Min.y = Math.Min(this.Min.y, (float)v.y);
			this.Min.z = Math.Min(this.Min.z, (float)v.z);
			this.Max.x = Math.Max(this.Max.x, (float)v.x);
			this.Max.y = Math.Max(this.Max.y, (float)v.y);
			this.Max.z = Math.Max(this.Max.z, (float)v.z);
		}

		public void Contain(AxisAlignedBox3d box)
		{
			this.Min.x = Math.Min(this.Min.x, (float)box.Min.x);
			this.Min.y = Math.Min(this.Min.y, (float)box.Min.y);
			this.Min.z = Math.Min(this.Min.z, (float)box.Min.z);
			this.Max.x = Math.Max(this.Max.x, (float)box.Max.x);
			this.Max.y = Math.Max(this.Max.y, (float)box.Max.y);
			this.Max.z = Math.Max(this.Max.z, (float)box.Max.z);
		}

		public AxisAlignedBox3f Intersect(AxisAlignedBox3f box)
		{
			AxisAlignedBox3f result = new AxisAlignedBox3f(Math.Max(this.Min.x, box.Min.x), Math.Max(this.Min.y, box.Min.y), Math.Max(this.Min.z, box.Min.z), Math.Min(this.Max.x, box.Max.x), Math.Min(this.Max.y, box.Max.y), Math.Min(this.Max.z, box.Max.z));
			if (result.Height <= 0f || result.Width <= 0f || result.Depth <= 0f)
			{
				return AxisAlignedBox3f.Empty;
			}
			return result;
		}

		public bool Contains(Vector3f v)
		{
			return this.Min.x <= v.x && this.Min.y <= v.y && this.Min.z <= v.z && this.Max.x >= v.x && this.Max.y >= v.y && this.Max.z >= v.z;
		}

		public bool Intersects(AxisAlignedBox3f box)
		{
			return box.Max.x > this.Min.x && box.Min.x < this.Max.x && box.Max.y > this.Min.y && box.Min.y < this.Max.y && box.Max.z > this.Min.z && box.Min.z < this.Max.z;
		}

		public double DistanceSquared(Vector3f v)
		{
			object obj = (v.x < this.Min.x) ? (this.Min.x - v.x) : ((v.x > this.Max.x) ? (v.x - this.Max.x) : 0f);
			float num = (v.y < this.Min.y) ? (this.Min.y - v.y) : ((v.y > this.Max.y) ? (v.y - this.Max.y) : 0f);
			float num2 = (v.z < this.Min.z) ? (this.Min.z - v.z) : ((v.z > this.Max.z) ? (v.z - this.Max.z) : 0f);
			object obj2 = obj;
			return obj2 * obj2 + num * num + num2 * num2;
		}

		public float Distance(Vector3f v)
		{
			return (float)Math.Sqrt(this.DistanceSquared(v));
		}

		public Vector3f NearestPoint(Vector3f v)
		{
			float x = (v.x < this.Min.x) ? this.Min.x : ((v.x > this.Max.x) ? this.Max.x : v.x);
			float y = (v.y < this.Min.y) ? this.Min.y : ((v.y > this.Max.y) ? this.Max.y : v.y);
			float z = (v.z < this.Min.z) ? this.Min.z : ((v.z > this.Max.z) ? this.Max.z : v.z);
			return new Vector3f(x, y, z);
		}

		public void Translate(Vector3f vTranslate)
		{
			this.Min.Add(vTranslate);
			this.Max.Add(vTranslate);
		}

		public void MoveMin(Vector3f vNewMin)
		{
			this.Max.x = vNewMin.x + (this.Max.x - this.Min.x);
			this.Max.y = vNewMin.y + (this.Max.y - this.Min.y);
			this.Max.z = vNewMin.z + (this.Max.z - this.Min.z);
			this.Min.Set(vNewMin);
		}

		public void MoveMin(float fNewX, float fNewY, float fNewZ)
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

		public static implicit operator AxisAlignedBox3f(Bounds b)
		{
			return new AxisAlignedBox3f(b.min, b.max);
		}

		public static implicit operator Bounds(AxisAlignedBox3f b)
		{
			Bounds result = default(Bounds);
			result.SetMinMax(b.Min, b.Max);
			return result;
		}

		public Vector3f Min;

		public Vector3f Max;

		public static readonly AxisAlignedBox3f Empty = new AxisAlignedBox3f(false);

		public static readonly AxisAlignedBox3f Zero = new AxisAlignedBox3f(0f);

		public static readonly AxisAlignedBox3f UnitPositive = new AxisAlignedBox3f(1f);

		public static readonly AxisAlignedBox3f Infinite = new AxisAlignedBox3f(float.MinValue, float.MinValue, float.MinValue, float.MaxValue, float.MaxValue, float.MaxValue);
	}
}
