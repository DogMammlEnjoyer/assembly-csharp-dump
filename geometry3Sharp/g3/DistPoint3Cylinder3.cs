using System;

namespace g3
{
	public class DistPoint3Cylinder3
	{
		public Vector3d Point
		{
			get
			{
				return this.point;
			}
			set
			{
				this.point = value;
				this.DistanceSquared = -1.0;
			}
		}

		public Cylinder3d Cylinder
		{
			get
			{
				return this.cylinder;
			}
			set
			{
				this.cylinder = value;
				this.DistanceSquared = -1.0;
			}
		}

		public bool IsInside
		{
			get
			{
				return this.SignedDistance < 0.0;
			}
		}

		public double SolidDistance
		{
			get
			{
				if (this.SignedDistance >= 0.0)
				{
					return this.SignedDistance;
				}
				return 0.0;
			}
		}

		public DistPoint3Cylinder3(Vector3d PointIn, Cylinder3d CylinderIn)
		{
			this.point = PointIn;
			this.cylinder = CylinderIn;
		}

		public DistPoint3Cylinder3 Compute()
		{
			this.GetSquared();
			return this;
		}

		public double Get()
		{
			return Math.Sqrt(this.GetSquared());
		}

		public double GetSquared()
		{
			if (this.DistanceSquared >= 0.0)
			{
				return this.DistanceSquared;
			}
			if (this.cylinder.Height >= 1.7976931348623157E+308)
			{
				return this.get_squared_infinite();
			}
			Vector3d direction = this.cylinder.Axis.Direction;
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d.ComputeOrthogonalComplement(1, direction, ref zero, ref zero2);
			double num = this.Cylinder.Height / 2.0;
			Vector3d v = this.point - this.cylinder.Axis.Origin;
			Vector3d vector3d = new Vector3d(zero.Dot(v), zero2.Dot(v), direction.Dot(v));
			Vector3d vector3d2 = Vector3d.Zero;
			double num2 = this.cylinder.Radius * this.cylinder.Radius;
			double num3 = vector3d[0] * vector3d[0] + vector3d[1] * vector3d[1];
			double num4 = Math.Sqrt(num3);
			double num5 = num4 - this.Cylinder.Radius;
			double num6 = this.Cylinder.Radius / num4;
			Vector3d vector3d3 = new Vector3d(num6 * vector3d.x, num6 * vector3d.y, vector3d.z);
			bool flag = num3 >= num2;
			vector3d2 = vector3d3;
			double num7 = num5;
			if (vector3d3.z >= num)
			{
				vector3d2 = (flag ? vector3d3 : vector3d);
				vector3d2.z = num;
				num7 = vector3d2.Distance(vector3d);
				flag = true;
			}
			else if (vector3d3.z <= -num)
			{
				vector3d2 = (flag ? vector3d3 : vector3d);
				vector3d2.z = -num;
				num7 = vector3d2.Distance(vector3d);
				flag = true;
			}
			else if (!flag)
			{
				if (vector3d3.z > 0.0 && Math.Abs(vector3d3.z - num) < Math.Abs(num5))
				{
					vector3d2 = vector3d;
					vector3d2.z = num;
					num7 = vector3d2.Distance(vector3d);
				}
				else if (vector3d3.z < 0.0 && Math.Abs(vector3d3.z - -num) < Math.Abs(num5))
				{
					vector3d2 = vector3d;
					vector3d2.z = -num;
					num7 = vector3d2.Distance(vector3d);
				}
			}
			this.SignedDistance = (flag ? Math.Abs(num7) : (-Math.Abs(num7)));
			this.CylinderClosest = this.cylinder.Axis.Origin + vector3d2.x * zero + vector3d2.y * zero2 + vector3d2.z * direction;
			this.DistanceSquared = num7 * num7;
			return this.DistanceSquared;
		}

		public double get_squared_infinite()
		{
			Vector3d direction = this.cylinder.Axis.Direction;
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d.ComputeOrthogonalComplement(1, direction, ref zero, ref zero2);
			Vector3d v = this.point - this.cylinder.Axis.Origin;
			Vector3d vector3d = new Vector3d(zero.Dot(v), zero2.Dot(v), direction.Dot(v));
			Vector3d zero3 = Vector3d.Zero;
			double num = Math.Sqrt(vector3d[0] * vector3d[0] + vector3d[1] * vector3d[1]);
			double num2 = num - this.Cylinder.Radius;
			double num3 = this.Cylinder.Radius / num;
			zero3 = new Vector3d(num3 * vector3d.x, num3 * vector3d.y, vector3d.z);
			this.CylinderClosest = this.cylinder.Axis.Origin + zero3.x * zero + zero3.y * zero2 + zero3.z * direction;
			this.SignedDistance = num2;
			this.DistanceSquared = num2 * num2;
			return this.DistanceSquared;
		}

		private Vector3d point;

		private Cylinder3d cylinder;

		public double DistanceSquared = -1.0;

		public double SignedDistance;

		public Vector3d CylinderClosest;
	}
}
