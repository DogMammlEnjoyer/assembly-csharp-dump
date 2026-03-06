using System;

namespace g3
{
	public struct Plane3f
	{
		public Plane3f(Vector3f normal, float constant)
		{
			this.Normal = normal;
			this.Constant = constant;
		}

		public Plane3f(Vector3f normal, Vector3f point)
		{
			this.Normal = normal;
			this.Constant = this.Normal.Dot(point);
		}

		public Plane3f(Vector3f p0, Vector3f p1, Vector3f p2)
		{
			Vector3f vector3f = p1 - p0;
			Vector3f v = p2 - p0;
			this.Normal = vector3f.UnitCross(v);
			this.Constant = this.Normal.Dot(p0);
		}

		public float DistanceTo(Vector3f p)
		{
			return this.Normal.Dot(p) - this.Constant;
		}

		public int WhichSide(Vector3f p)
		{
			float num = this.DistanceTo(p);
			if (num < 0f)
			{
				return -1;
			}
			if (num > 0f)
			{
				return 1;
			}
			return 0;
		}

		public Vector3f Normal;

		public float Constant;
	}
}
