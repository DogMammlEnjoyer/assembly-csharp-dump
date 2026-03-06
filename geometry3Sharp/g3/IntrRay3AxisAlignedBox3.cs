using System;

namespace g3
{
	public class IntrRay3AxisAlignedBox3
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

		public AxisAlignedBox3d Box
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

		public IntrRay3AxisAlignedBox3(Ray3d r, AxisAlignedBox3d b)
		{
			this.ray = r;
			this.box = b;
		}

		public IntrRay3AxisAlignedBox3 Compute()
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
			IntrLine3AxisAlignedBox3.DoClipping(ref this.RayParam0, ref this.RayParam1, ref this.ray.Origin, ref this.ray.Direction, ref this.box, true, ref this.Quantity, ref this.Point0, ref this.Point1, ref this.Type);
			this.Result = ((this.Type != IntersectionType.Empty) ? IntersectionResult.Intersects : IntersectionResult.NoIntersection);
			return this.Result == IntersectionResult.Intersects;
		}

		public bool Test()
		{
			return IntrRay3AxisAlignedBox3.Intersects(ref this.ray, ref this.box, 0.0);
		}

		public static bool Intersects(ref Ray3d ray, ref AxisAlignedBox3d box, double expandExtents = 0.0)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			Vector3d zero4 = Vector3d.Zero;
			Vector3d vector3d = ray.Origin - box.Center;
			Vector3d vector3d2 = box.Extents + expandExtents;
			zero.x = ray.Direction.x;
			zero2.x = Math.Abs(zero.x);
			zero3.x = vector3d.x;
			zero4.x = Math.Abs(zero3.x);
			if (zero4.x > vector3d2.x && zero3.x * zero.x >= 0.0)
			{
				return false;
			}
			zero.y = ray.Direction.y;
			zero2.y = Math.Abs(zero.y);
			zero3.y = vector3d.y;
			zero4.y = Math.Abs(zero3.y);
			if (zero4.y > vector3d2.y && zero3.y * zero.y >= 0.0)
			{
				return false;
			}
			zero.z = ray.Direction.z;
			zero2.z = Math.Abs(zero.z);
			zero3.z = vector3d.z;
			zero4.z = Math.Abs(zero3.z);
			if (zero4.z > vector3d2.z && zero3.z * zero.z >= 0.0)
			{
				return false;
			}
			Vector3d vector3d3 = ray.Direction.Cross(vector3d);
			Vector3d zero5 = Vector3d.Zero;
			zero5.x = Math.Abs(vector3d3.x);
			double num = vector3d2.y * zero2.z + vector3d2.z * zero2.y;
			if (zero5.x > num)
			{
				return false;
			}
			zero5.y = Math.Abs(vector3d3.y);
			num = vector3d2.x * zero2.z + vector3d2.z * zero2.x;
			if (zero5.y > num)
			{
				return false;
			}
			zero5.z = Math.Abs(vector3d3.z);
			num = vector3d2.x * zero2.y + vector3d2.y * zero2.x;
			return zero5.z <= num;
		}

		public static bool FindRayIntersectT(ref Ray3d ray, ref AxisAlignedBox3d box, out double RayParam)
		{
			double num = 0.0;
			double maxValue = double.MaxValue;
			int num2 = 0;
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			IntersectionType intersectionType = IntersectionType.Empty;
			IntrLine3AxisAlignedBox3.DoClipping(ref num, ref maxValue, ref ray.Origin, ref ray.Direction, ref box, true, ref num2, ref zero, ref zero2, ref intersectionType);
			if (intersectionType != IntersectionType.Empty)
			{
				RayParam = num;
				return true;
			}
			RayParam = double.MaxValue;
			return false;
		}

		private Ray3d ray;

		private AxisAlignedBox3d box;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public double RayParam0;

		public double RayParam1;

		public Vector3d Point0 = Vector3d.Zero;

		public Vector3d Point1 = Vector3d.Zero;
	}
}
