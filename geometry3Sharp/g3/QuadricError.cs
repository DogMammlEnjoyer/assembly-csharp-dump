using System;

namespace g3
{
	public struct QuadricError
	{
		public QuadricError(ref Vector3d n, ref Vector3d p)
		{
			this.Axx = n.x * n.x;
			this.Axy = n.x * n.y;
			this.Axz = n.x * n.z;
			this.Ayy = n.y * n.y;
			this.Ayz = n.y * n.z;
			this.Azz = n.z * n.z;
			this.bx = (this.by = (this.bz = (this.c = 0.0)));
			Vector3d vector3d = this.multiplyA(ref p);
			this.bx = -vector3d.x;
			this.by = -vector3d.y;
			this.bz = -vector3d.z;
			this.c = p.Dot(ref vector3d);
		}

		public QuadricError(ref QuadricError a, ref QuadricError b)
		{
			this.Axx = a.Axx + b.Axx;
			this.Axy = a.Axy + b.Axy;
			this.Axz = a.Axz + b.Axz;
			this.Ayy = a.Ayy + b.Ayy;
			this.Ayz = a.Ayz + b.Ayz;
			this.Azz = a.Azz + b.Azz;
			this.bx = a.bx + b.bx;
			this.by = a.by + b.by;
			this.bz = a.bz + b.bz;
			this.c = a.c + b.c;
		}

		public void Add(double w, ref QuadricError b)
		{
			this.Axx += w * b.Axx;
			this.Axy += w * b.Axy;
			this.Axz += w * b.Axz;
			this.Ayy += w * b.Ayy;
			this.Ayz += w * b.Ayz;
			this.Azz += w * b.Azz;
			this.bx += w * b.bx;
			this.by += w * b.by;
			this.bz += w * b.bz;
			this.c += w * b.c;
		}

		public double Evaluate(ref Vector3d pt)
		{
			double num = this.Axx * pt.x + this.Axy * pt.y + this.Axz * pt.z;
			double num2 = this.Axy * pt.x + this.Ayy * pt.y + this.Ayz * pt.z;
			double num3 = this.Axz * pt.x + this.Ayz * pt.y + this.Azz * pt.z;
			return pt.x * num + pt.y * num2 + pt.z * num3 + 2.0 * (pt.x * this.bx + pt.y * this.by + pt.z * this.bz) + this.c;
		}

		private Vector3d multiplyA(ref Vector3d pt)
		{
			double x = this.Axx * pt.x + this.Axy * pt.y + this.Axz * pt.z;
			double y = this.Axy * pt.x + this.Ayy * pt.y + this.Ayz * pt.z;
			double z = this.Axz * pt.x + this.Ayz * pt.y + this.Azz * pt.z;
			return new Vector3d(x, y, z);
		}

		public bool OptimalPoint(ref Vector3d result)
		{
			double num = this.Azz * this.Ayy - this.Ayz * this.Ayz;
			double num2 = this.Axz * this.Ayz - this.Azz * this.Axy;
			double num3 = this.Axy * this.Ayz - this.Axz * this.Ayy;
			double num4 = this.Azz * this.Axx - this.Axz * this.Axz;
			double num5 = this.Axy * this.Axz - this.Axx * this.Ayz;
			double num6 = this.Axx * this.Ayy - this.Axy * this.Axy;
			double num7 = this.Axx * num + this.Axy * num2 + this.Axz * num3;
			if (Math.Abs(num7) > 2.220446049250313E-13)
			{
				num7 = 1.0 / num7;
				num *= num7;
				num2 *= num7;
				num3 *= num7;
				num4 *= num7;
				num5 *= num7;
				num6 *= num7;
				double num8 = num * this.bx + num2 * this.by + num3 * this.bz;
				double num9 = num2 * this.bx + num4 * this.by + num5 * this.bz;
				double num10 = num3 * this.bx + num5 * this.by + num6 * this.bz;
				result = new Vector3d(-num8, -num9, -num10);
				return true;
			}
			return false;
		}

		public double Axx;

		public double Axy;

		public double Axz;

		public double Ayy;

		public double Ayz;

		public double Azz;

		public double bx;

		public double by;

		public double bz;

		public double c;

		public static readonly QuadricError Zero = new QuadricError
		{
			Axx = 0.0,
			Axy = 0.0,
			Axz = 0.0,
			Ayy = 0.0,
			Ayz = 0.0,
			Azz = 0.0,
			bx = 0.0,
			by = 0.0,
			bz = 0.0,
			c = 0.0
		};
	}
}
