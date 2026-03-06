using System;

namespace g3
{
	public class IntrRay3Triangle3
	{
		public Ray3d Ray
		{
			get
			{
				return this.ray;
			}
			set
			{
				this.ray = value;
				this.Result = IntersectionResult.NotComputed;
			}
		}

		public Triangle3d Triangle
		{
			get
			{
				return this.triangle;
			}
			set
			{
				this.triangle = value;
				this.Result = IntersectionResult.NotComputed;
			}
		}

		public bool IsSimpleIntersection
		{
			get
			{
				return this.Result == IntersectionResult.Intersects && this.Type == IntersectionType.Point;
			}
		}

		public IntrRay3Triangle3(Ray3d r, Triangle3d t)
		{
			this.ray = r;
			this.triangle = t;
		}

		public IntrRay3Triangle3 Compute()
		{
			this.Find();
			return this;
		}

		public bool Find()
		{
			if (this.Result != IntersectionResult.NotComputed)
			{
				return this.Result != IntersectionResult.NoIntersection;
			}
			Vector3d v = this.ray.Origin - this.triangle.V0;
			Vector3d vector3d = this.triangle.V1 - this.triangle.V0;
			Vector3d v2 = this.triangle.V2 - this.triangle.V0;
			Vector3d v3 = vector3d.Cross(v2);
			double num = this.ray.Direction.Dot(v3);
			double num2;
			if (num > 1E-08)
			{
				num2 = 1.0;
			}
			else
			{
				if (num >= -1E-08)
				{
					this.Result = IntersectionResult.NoIntersection;
					return false;
				}
				num2 = -1.0;
				num = -num;
			}
			double num3 = num2 * this.ray.Direction.Dot(v.Cross(v2));
			if (num3 >= 0.0)
			{
				double num4 = num2 * this.ray.Direction.Dot(vector3d.Cross(v));
				if (num4 >= 0.0 && num3 + num4 <= num)
				{
					double num5 = -num2 * v.Dot(v3);
					if (num5 >= 0.0)
					{
						double num6 = 1.0 / num;
						this.RayParameter = num5 * num6;
						double num7 = num3 * num6;
						double num8 = num4 * num6;
						this.TriangleBaryCoords = new Vector3d(1.0 - num7 - num8, num7, num8);
						this.Type = IntersectionType.Point;
						this.Quantity = 1;
						this.Result = IntersectionResult.Intersects;
						return true;
					}
				}
			}
			this.Result = IntersectionResult.NoIntersection;
			return false;
		}

		public static bool Intersects(ref Ray3d ray, ref Vector3d V0, ref Vector3d V1, ref Vector3d V2, out double rayT)
		{
			Vector3d vector3d = ray.Origin - V0;
			Vector3d vector3d2 = V1 - V0;
			Vector3d vector3d3 = V2 - V0;
			Vector3d vector3d4 = vector3d2.Cross(ref vector3d3);
			rayT = double.MaxValue;
			double num = ray.Direction.Dot(ref vector3d4);
			double num2;
			if (num > 1E-08)
			{
				num2 = 1.0;
			}
			else
			{
				if (num >= -1E-08)
				{
					return false;
				}
				num2 = -1.0;
				num = -num;
			}
			Vector3d vector3d5 = vector3d.Cross(ref vector3d3);
			double num3 = num2 * ray.Direction.Dot(ref vector3d5);
			if (num3 >= 0.0)
			{
				vector3d5 = vector3d2.Cross(ref vector3d);
				double num4 = num2 * ray.Direction.Dot(ref vector3d5);
				if (num4 >= 0.0 && num3 + num4 <= num)
				{
					double num5 = -num2 * vector3d.Dot(ref vector3d4);
					if (num5 >= 0.0)
					{
						double num6 = 1.0 / num;
						rayT = num5 * num6;
						return true;
					}
				}
			}
			return false;
		}

		private Ray3d ray;

		private Triangle3d triangle;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public double RayParameter;

		public Vector3d TriangleBaryCoords;
	}
}
