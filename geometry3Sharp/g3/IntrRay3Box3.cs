using System;

namespace g3
{
	public class IntrRay3Box3
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

		public Box3d Box
		{
			get
			{
				return this.box;
			}
			set
			{
				this.box = value;
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

		public IntrRay3Box3(Ray3d r, Box3d b)
		{
			this.ray = r;
			this.box = b;
		}

		public IntrRay3Box3 Compute()
		{
			this.Find();
			return this;
		}

		public bool Find()
		{
			if (this.Result != IntersectionResult.NotComputed)
			{
				return this.Result == IntersectionResult.Intersects;
			}
			if (!this.ray.Direction.IsNormalized)
			{
				this.Type = IntersectionType.Empty;
				this.Result = IntersectionResult.InvalidQuery;
				return false;
			}
			this.RayParam0 = 0.0;
			this.RayParam1 = double.MaxValue;
			IntrLine3Box3.DoClipping(ref this.RayParam0, ref this.RayParam1, this.ray.Origin, this.ray.Direction, this.box, true, ref this.Quantity, ref this.Point0, ref this.Point1, ref this.Type);
			this.Result = ((this.Type != IntersectionType.Empty) ? IntersectionResult.Intersects : IntersectionResult.NoIntersection);
			return this.Result == IntersectionResult.Intersects;
		}

		public bool Test()
		{
			return IntrRay3Box3.Intersects(ref this.ray, ref this.box, 0.0);
		}

		public static bool Intersects(ref Ray3d ray, ref Box3d box, double expandExtents = 0.0)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			Vector3d zero4 = Vector3d.Zero;
			Vector3d zero5 = Vector3d.Zero;
			Vector3d v = ray.Origin - box.Center;
			Vector3d vector3d = box.Extent + expandExtents;
			zero[0] = ray.Direction.Dot(ref box.AxisX);
			zero2[0] = Math.Abs(zero[0]);
			zero3[0] = v.Dot(ref box.AxisX);
			zero4[0] = Math.Abs(zero3[0]);
			if (zero4[0] > vector3d.x && zero3[0] * zero[0] >= 0.0)
			{
				return false;
			}
			zero[1] = ray.Direction.Dot(ref box.AxisY);
			zero2[1] = Math.Abs(zero[1]);
			zero3[1] = v.Dot(ref box.AxisY);
			zero4[1] = Math.Abs(zero3[1]);
			if (zero4[1] > vector3d.y && zero3[1] * zero[1] >= 0.0)
			{
				return false;
			}
			zero[2] = ray.Direction.Dot(ref box.AxisZ);
			zero2[2] = Math.Abs(zero[2]);
			zero3[2] = v.Dot(ref box.AxisZ);
			zero4[2] = Math.Abs(zero3[2]);
			if (zero4[2] > vector3d.z && zero3[2] * zero[2] >= 0.0)
			{
				return false;
			}
			Vector3d vector3d2 = ray.Direction.Cross(v);
			zero5[0] = Math.Abs(vector3d2.Dot(ref box.AxisX));
			double num = vector3d.y * zero2[2] + vector3d.z * zero2[1];
			if (zero5[0] > num)
			{
				return false;
			}
			zero5[1] = Math.Abs(vector3d2.Dot(ref box.AxisY));
			num = vector3d.x * zero2[2] + vector3d.z * zero2[0];
			if (zero5[1] > num)
			{
				return false;
			}
			zero5[2] = Math.Abs(vector3d2.Dot(ref box.AxisZ));
			num = vector3d.x * zero2[1] + vector3d.y * zero2[0];
			return zero5[2] <= num;
		}

		private Ray3d ray;

		private Box3d box;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public double RayParam0;

		public double RayParam1;

		public Vector3d Point0 = Vector3d.Zero;

		public Vector3d Point1 = Vector3d.Zero;
	}
}
