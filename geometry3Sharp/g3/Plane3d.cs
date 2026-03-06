using System;

namespace g3
{
	public struct Plane3d
	{
		public Plane3d(Vector3d normal, double constant)
		{
			this.Normal = normal;
			this.Constant = constant;
		}

		public Plane3d(Vector3d normal, Vector3d point)
		{
			this.Normal = normal;
			this.Constant = this.Normal.Dot(point);
		}

		public Plane3d(Vector3d p0, Vector3d p1, Vector3d p2)
		{
			Vector3d vector3d = p1 - p0;
			Vector3d v = p2 - p0;
			this.Normal = vector3d.UnitCross(v);
			this.Constant = this.Normal.Dot(p0);
		}

		public double DistanceTo(Vector3d p)
		{
			return this.Normal.Dot(p) - this.Constant;
		}

		public int WhichSide(Vector3d p)
		{
			double num = this.DistanceTo(p);
			if (num < 0.0)
			{
				return -1;
			}
			if (num > 0.0)
			{
				return 1;
			}
			return 0;
		}

		public Vector3d Normal;

		public double Constant;
	}
}
