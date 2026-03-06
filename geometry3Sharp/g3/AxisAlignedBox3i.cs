using System;
using System.Collections.Generic;

namespace g3
{
	public struct AxisAlignedBox3i : IComparable<AxisAlignedBox3i>, IEquatable<AxisAlignedBox3i>
	{
		public AxisAlignedBox3i(bool bIgnore)
		{
			this.Min = new Vector3i(int.MaxValue, int.MaxValue, int.MaxValue);
			this.Max = new Vector3i(int.MinValue, int.MinValue, int.MinValue);
		}

		public AxisAlignedBox3i(int xmin, int ymin, int zmin, int xmax, int ymax, int zmax)
		{
			this.Min = new Vector3i(xmin, ymin, zmin);
			this.Max = new Vector3i(xmax, ymax, zmax);
		}

		public AxisAlignedBox3i(int fCubeSize)
		{
			this.Min = new Vector3i(0, 0, 0);
			this.Max = new Vector3i(fCubeSize, fCubeSize, fCubeSize);
		}

		public AxisAlignedBox3i(int fWidth, int fHeight, int fDepth)
		{
			this.Min = new Vector3i(0, 0, 0);
			this.Max = new Vector3i(fWidth, fHeight, fDepth);
		}

		public AxisAlignedBox3i(Vector3i vMin, Vector3i vMax)
		{
			this.Min = new Vector3i(Math.Min(vMin.x, vMax.x), Math.Min(vMin.y, vMax.y), Math.Min(vMin.z, vMax.z));
			this.Max = new Vector3i(Math.Max(vMin.x, vMax.x), Math.Max(vMin.y, vMax.y), Math.Max(vMin.z, vMax.z));
		}

		public AxisAlignedBox3i(Vector3i vCenter, int fHalfWidth, int fHalfHeight, int fHalfDepth)
		{
			this.Min = new Vector3i(vCenter.x - fHalfWidth, vCenter.y - fHalfHeight, vCenter.z - fHalfDepth);
			this.Max = new Vector3i(vCenter.x + fHalfWidth, vCenter.y + fHalfHeight, vCenter.z + fHalfDepth);
		}

		public AxisAlignedBox3i(Vector3i vCenter, int fHalfSize)
		{
			this.Min = new Vector3i(vCenter.x - fHalfSize, vCenter.y - fHalfSize, vCenter.z - fHalfSize);
			this.Max = new Vector3i(vCenter.x + fHalfSize, vCenter.y + fHalfSize, vCenter.z + fHalfSize);
		}

		public AxisAlignedBox3i(Vector3i vCenter)
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

		public int Depth
		{
			get
			{
				return Math.Max(this.Max.z - this.Min.z, 0);
			}
		}

		public int Volume
		{
			get
			{
				return this.Width * this.Height * this.Depth;
			}
		}

		public int DiagonalLength
		{
			get
			{
				return (int)Math.Sqrt((double)((this.Max.x - this.Min.x) * (this.Max.x - this.Min.x) + (this.Max.y - this.Min.y) * (this.Max.y - this.Min.y) + (this.Max.z - this.Min.z) * (this.Max.z - this.Min.z)));
			}
		}

		public int MaxDim
		{
			get
			{
				return Math.Max(this.Width, Math.Max(this.Height, this.Depth));
			}
		}

		public Vector3i Diagonal
		{
			get
			{
				return new Vector3i(this.Max.x - this.Min.x, this.Max.y - this.Min.y, this.Max.z - this.Min.z);
			}
		}

		public Vector3i Extents
		{
			get
			{
				return new Vector3i((this.Max.x - this.Min.x) / 2, (this.Max.y - this.Min.y) / 2, (this.Max.z - this.Min.z) / 2);
			}
		}

		public Vector3i Center
		{
			get
			{
				return new Vector3i((this.Min.x + this.Max.x) / 2, (this.Min.y + this.Max.y) / 2, (this.Min.z + this.Max.z) / 2);
			}
		}

		public static bool operator ==(AxisAlignedBox3i a, AxisAlignedBox3i b)
		{
			return a.Min == b.Min && a.Max == b.Max;
		}

		public static bool operator !=(AxisAlignedBox3i a, AxisAlignedBox3i b)
		{
			return a.Min != b.Min || a.Max != b.Max;
		}

		public override bool Equals(object obj)
		{
			return this == (AxisAlignedBox3i)obj;
		}

		public bool Equals(AxisAlignedBox3i other)
		{
			return this == other;
		}

		public int CompareTo(AxisAlignedBox3i other)
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

		public void Expand(int nRadius)
		{
			this.Min.x = this.Min.x - nRadius;
			this.Min.y = this.Min.y - nRadius;
			this.Min.z = this.Min.z - nRadius;
			this.Max.x = this.Max.x + nRadius;
			this.Max.y = this.Max.y + nRadius;
			this.Max.z = this.Max.z + nRadius;
		}

		public void Contract(int nRadius)
		{
			this.Min.x = this.Min.x + nRadius;
			this.Min.y = this.Min.y + nRadius;
			this.Min.z = this.Min.z + nRadius;
			this.Max.x = this.Max.x - nRadius;
			this.Max.y = this.Max.y - nRadius;
			this.Max.z = this.Max.z - nRadius;
		}

		public void Scale(int sx, int sy, int sz)
		{
			Vector3i center = this.Center;
			Vector3i extents = this.Extents;
			extents.x *= sx;
			extents.y *= sy;
			extents.z *= sz;
			this.Min = new Vector3i(center.x - extents.x, center.y - extents.y, center.z - extents.z);
			this.Max = new Vector3i(center.x + extents.x, center.y + extents.y, center.z + extents.z);
		}

		public void Contain(Vector3i v)
		{
			this.Min.x = Math.Min(this.Min.x, v.x);
			this.Min.y = Math.Min(this.Min.y, v.y);
			this.Min.z = Math.Min(this.Min.z, v.z);
			this.Max.x = Math.Max(this.Max.x, v.x);
			this.Max.y = Math.Max(this.Max.y, v.y);
			this.Max.z = Math.Max(this.Max.z, v.z);
		}

		public void Contain(AxisAlignedBox3i box)
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
			this.Min.x = Math.Min(this.Min.x, (int)v.x);
			this.Min.y = Math.Min(this.Min.y, (int)v.y);
			this.Min.z = Math.Min(this.Min.z, (int)v.z);
			this.Max.x = Math.Max(this.Max.x, (int)v.x);
			this.Max.y = Math.Max(this.Max.y, (int)v.y);
			this.Max.z = Math.Max(this.Max.z, (int)v.z);
		}

		public void Contain(AxisAlignedBox3d box)
		{
			this.Min.x = Math.Min(this.Min.x, (int)box.Min.x);
			this.Min.y = Math.Min(this.Min.y, (int)box.Min.y);
			this.Min.z = Math.Min(this.Min.z, (int)box.Min.z);
			this.Max.x = Math.Max(this.Max.x, (int)box.Max.x);
			this.Max.y = Math.Max(this.Max.y, (int)box.Max.y);
			this.Max.z = Math.Max(this.Max.z, (int)box.Max.z);
		}

		public AxisAlignedBox3i Intersect(AxisAlignedBox3i box)
		{
			AxisAlignedBox3i result = new AxisAlignedBox3i(Math.Max(this.Min.x, box.Min.x), Math.Max(this.Min.y, box.Min.y), Math.Max(this.Min.z, box.Min.z), Math.Min(this.Max.x, box.Max.x), Math.Min(this.Max.y, box.Max.y), Math.Min(this.Max.z, box.Max.z));
			if (result.Height <= 0 || result.Width <= 0 || result.Depth <= 0)
			{
				return AxisAlignedBox3i.Empty;
			}
			return result;
		}

		public bool Contains(Vector3i v)
		{
			return this.Min.x <= v.x && this.Min.y <= v.y && this.Min.z <= v.z && this.Max.x >= v.x && this.Max.y >= v.y && this.Max.z >= v.z;
		}

		public bool Intersects(AxisAlignedBox3i box)
		{
			return box.Max.x > this.Min.x && box.Min.x < this.Max.x && box.Max.y > this.Min.y && box.Min.y < this.Max.y && box.Max.z > this.Min.z && box.Min.z < this.Max.z;
		}

		public double DistanceSquared(Vector3i v)
		{
			object obj = (v.x < this.Min.x) ? (this.Min.x - v.x) : ((v.x > this.Max.x) ? (v.x - this.Max.x) : 0);
			int num = (v.y < this.Min.y) ? (this.Min.y - v.y) : ((v.y > this.Max.y) ? (v.y - this.Max.y) : 0);
			int num2 = (v.z < this.Min.z) ? (this.Min.z - v.z) : ((v.z > this.Max.z) ? (v.z - this.Max.z) : 0);
			object obj2 = obj;
			return obj2 * obj2 + num * num + num2 * num2;
		}

		public int Distance(Vector3i v)
		{
			return (int)Math.Sqrt(this.DistanceSquared(v));
		}

		public Vector3i NearestPoint(Vector3i v)
		{
			int x = (v.x < this.Min.x) ? this.Min.x : ((v.x > this.Max.x) ? this.Max.x : v.x);
			int y = (v.y < this.Min.y) ? this.Min.y : ((v.y > this.Max.y) ? this.Max.y : v.y);
			int z = (v.z < this.Min.z) ? this.Min.z : ((v.z > this.Max.z) ? this.Max.z : v.z);
			return new Vector3i(x, y, z);
		}

		public Vector3i ClampInclusive(Vector3i v)
		{
			return new Vector3i(MathUtil.Clamp(v.x, this.Min.x, this.Max.x), MathUtil.Clamp(v.y, this.Min.y, this.Max.y), MathUtil.Clamp(v.z, this.Min.z, this.Max.z));
		}

		public Vector3i ClampExclusive(Vector3i v)
		{
			return new Vector3i(MathUtil.Clamp(v.x, this.Min.x, this.Max.x - 1), MathUtil.Clamp(v.y, this.Min.y, this.Max.y - 1), MathUtil.Clamp(v.z, this.Min.z, this.Max.z - 1));
		}

		public void Translate(Vector3i vTranslate)
		{
			this.Min.Add(vTranslate);
			this.Max.Add(vTranslate);
		}

		public void MoveMin(Vector3i vNewMin)
		{
			this.Max.x = vNewMin.x + (this.Max.x - this.Min.x);
			this.Max.y = vNewMin.y + (this.Max.y - this.Min.y);
			this.Max.z = vNewMin.z + (this.Max.z - this.Min.z);
			this.Min.Set(vNewMin);
		}

		public void MoveMin(int fNewX, int fNewY, int fNewZ)
		{
			this.Max.x = fNewX + (this.Max.x - this.Min.x);
			this.Max.y = fNewY + (this.Max.y - this.Min.y);
			this.Max.z = fNewZ + (this.Max.z - this.Min.z);
			this.Min.Set(fNewX, fNewY, fNewZ);
		}

		public IEnumerable<Vector3i> IndicesInclusive()
		{
			int num;
			for (int zi = this.Min.z; zi <= this.Max.z; zi = num)
			{
				for (int yi = this.Min.y; yi <= this.Max.y; yi = num)
				{
					for (int xi = this.Min.x; xi <= this.Max.x; xi = num)
					{
						yield return new Vector3i(xi, yi, zi);
						num = xi + 1;
					}
					num = yi + 1;
				}
				num = zi + 1;
			}
			yield break;
		}

		public IEnumerable<Vector3i> IndicesExclusive()
		{
			int num;
			for (int zi = this.Min.z; zi < this.Max.z; zi = num)
			{
				for (int yi = this.Min.y; yi < this.Max.y; yi = num)
				{
					for (int xi = this.Min.x; xi < this.Max.x; xi = num)
					{
						yield return new Vector3i(xi, yi, zi);
						num = xi + 1;
					}
					num = yi + 1;
				}
				num = zi + 1;
			}
			yield break;
		}

		public override string ToString()
		{
			return string.Format("x[{0},{1}] y[{2},{3}] z[{4},{5}]", new object[]
			{
				this.Min.x,
				this.Max.x,
				this.Min.y,
				this.Max.y,
				this.Min.z,
				this.Max.z
			});
		}

		public Vector3i Min;

		public Vector3i Max;

		public static readonly AxisAlignedBox3i Empty = new AxisAlignedBox3i(false);

		public static readonly AxisAlignedBox3i Zero = new AxisAlignedBox3i(0);

		public static readonly AxisAlignedBox3i UnitPositive = new AxisAlignedBox3i(1);

		public static readonly AxisAlignedBox3i Infinite = new AxisAlignedBox3i(int.MinValue, int.MinValue, int.MinValue, int.MaxValue, int.MaxValue, int.MaxValue);
	}
}
