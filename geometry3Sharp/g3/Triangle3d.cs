using System;

namespace g3
{
	public struct Triangle3d
	{
		public Triangle3d(Vector3d v0, Vector3d v1, Vector3d v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		public Vector3d this[int key]
		{
			get
			{
				if (key == 0)
				{
					return this.V0;
				}
				if (key != 1)
				{
					return this.V2;
				}
				return this.V1;
			}
			set
			{
				if (key == 0)
				{
					this.V0 = value;
					return;
				}
				if (key == 1)
				{
					this.V1 = value;
					return;
				}
				this.V2 = value;
			}
		}

		public Vector3d Normal
		{
			get
			{
				return MathUtil.Normal(ref this.V0, ref this.V1, ref this.V2);
			}
		}

		public double Area
		{
			get
			{
				return MathUtil.Area(ref this.V0, ref this.V1, ref this.V2);
			}
		}

		public double AspectRatio
		{
			get
			{
				return MathUtil.AspectRatio(ref this.V0, ref this.V1, ref this.V2);
			}
		}

		public Vector3d PointAt(double bary0, double bary1, double bary2)
		{
			return bary0 * this.V0 + bary1 * this.V1 + bary2 * this.V2;
		}

		public Vector3d PointAt(Vector3d bary)
		{
			return bary.x * this.V0 + bary.y * this.V1 + bary.z * this.V2;
		}

		public Vector3d BarycentricCoords(Vector3d point)
		{
			return MathUtil.BarycentricCoords(point, this.V0, this.V1, this.V2);
		}

		public static implicit operator Triangle3d(Triangle3f v)
		{
			return new Triangle3d(v.V0, v.V1, v.V2);
		}

		public static explicit operator Triangle3f(Triangle3d v)
		{
			return new Triangle3f((Vector3f)v.V0, (Vector3f)v.V1, (Vector3f)v.V2);
		}

		public Vector3d V0;

		public Vector3d V1;

		public Vector3d V2;
	}
}
