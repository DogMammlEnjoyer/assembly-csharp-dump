using System;

namespace g3
{
	public struct Triangle2d
	{
		public Triangle2d(Vector2d v0, Vector2d v1, Vector2d v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		public Vector2d this[int key]
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

		public Vector2d PointAt(double bary0, double bary1, double bary2)
		{
			return bary0 * this.V0 + bary1 * this.V1 + bary2 * this.V2;
		}

		public Vector2d PointAt(Vector3d bary)
		{
			return bary.x * this.V0 + bary.y * this.V1 + bary.z * this.V2;
		}

		public Vector2d Centroid()
		{
			return this.PointAt(0.3, 0.3, 0.3);
		}

		public static implicit operator Triangle2d(Triangle2f v)
		{
			return new Triangle2d(v.V0, v.V1, v.V2);
		}

		public static explicit operator Triangle2f(Triangle2d v)
		{
			return new Triangle2f((Vector2f)v.V0, (Vector2f)v.V1, (Vector2f)v.V2);
		}

		public Vector2d V0;

		public Vector2d V1;

		public Vector2d V2;
	}
}
