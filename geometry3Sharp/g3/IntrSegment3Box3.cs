using System;

namespace g3
{
	public class IntrSegment3Box3
	{
		public Segment3d Segment
		{
			get
			{
				return this.segment;
			}
			set
			{
				this.segment = value;
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

		public bool Solid
		{
			get
			{
				return this.solid;
			}
			set
			{
				this.solid = value;
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

		public IntrSegment3Box3(Segment3d s, Box3d b, bool solidBox)
		{
			this.segment = s;
			this.box = b;
			this.solid = solidBox;
		}

		public IntrSegment3Box3 Compute()
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
			if (!this.segment.Direction.IsNormalized)
			{
				this.Type = IntersectionType.Empty;
				this.Result = IntersectionResult.InvalidQuery;
				return false;
			}
			this.SegmentParam0 = -this.segment.Extent;
			this.SegmentParam1 = this.segment.Extent;
			IntrSegment3Box3.DoClipping(ref this.SegmentParam0, ref this.SegmentParam1, this.segment.Center, this.segment.Direction, this.box, this.solid, ref this.Quantity, ref this.Point0, ref this.Point1, ref this.Type);
			this.Result = ((this.Type != IntersectionType.Empty) ? IntersectionResult.Intersects : IntersectionResult.NoIntersection);
			return this.Result == IntersectionResult.Intersects;
		}

		public bool Test()
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			Vector3d v = this.segment.Center - this.box.Center;
			zero[0] = Math.Abs(this.segment.Direction.Dot(this.box.AxisX));
			zero2[0] = Math.Abs(v.Dot(this.box.AxisX));
			double num = this.box.Extent.x + this.segment.Extent * zero[0];
			if (zero2[0] > num)
			{
				return false;
			}
			zero[1] = Math.Abs(this.segment.Direction.Dot(this.box.AxisY));
			zero2[1] = Math.Abs(v.Dot(this.box.AxisY));
			num = this.box.Extent.y + this.segment.Extent * zero[1];
			if (zero2[1] > num)
			{
				return false;
			}
			zero[2] = Math.Abs(this.segment.Direction.Dot(this.box.AxisZ));
			zero2[2] = Math.Abs(v.Dot(this.box.AxisZ));
			num = this.box.Extent.z + this.segment.Extent * zero[2];
			if (zero2[2] > num)
			{
				return false;
			}
			Vector3d vector3d = this.segment.Direction.Cross(v);
			zero3[0] = Math.Abs(vector3d.Dot(this.box.AxisX));
			num = this.box.Extent.y * zero[2] + this.box.Extent.z * zero[1];
			if (zero3[0] > num)
			{
				return false;
			}
			zero3[1] = Math.Abs(vector3d.Dot(this.box.AxisY));
			num = this.box.Extent.x * zero[2] + this.box.Extent.z * zero[0];
			if (zero3[1] > num)
			{
				return false;
			}
			zero3[2] = Math.Abs(vector3d.Dot(this.box.AxisZ));
			num = this.box.Extent.x * zero[1] + this.box.Extent.y * zero[0];
			return zero3[2] <= num;
		}

		public static bool DoClipping(ref double t0, ref double t1, Vector3d origin, Vector3d direction, Box3d box, bool solid, ref int quantity, ref Vector3d point0, ref Vector3d point1, ref IntersectionType intrType)
		{
			Vector3d vector3d = origin - box.Center;
			Vector3d vector3d2 = new Vector3d(vector3d.Dot(box.AxisX), vector3d.Dot(box.AxisY), vector3d.Dot(box.AxisZ));
			Vector3d vector3d3 = new Vector3d(direction.Dot(box.AxisX), direction.Dot(box.AxisY), direction.Dot(box.AxisZ));
			double num = t0;
			double num2 = t1;
			if (IntrSegment3Box3.Clip(vector3d3.x, -vector3d2.x - box.Extent.x, ref t0, ref t1) && IntrSegment3Box3.Clip(-vector3d3.x, vector3d2.x - box.Extent.x, ref t0, ref t1) && IntrSegment3Box3.Clip(vector3d3.y, -vector3d2.y - box.Extent.y, ref t0, ref t1) && IntrSegment3Box3.Clip(-vector3d3.y, vector3d2.y - box.Extent.y, ref t0, ref t1) && IntrSegment3Box3.Clip(vector3d3.z, -vector3d2.z - box.Extent.z, ref t0, ref t1) && IntrSegment3Box3.Clip(-vector3d3.z, vector3d2.z - box.Extent.z, ref t0, ref t1) && (solid || t0 != num || t1 != num2))
			{
				if (t1 > t0)
				{
					intrType = IntersectionType.Segment;
					quantity = 2;
					point0 = origin + t0 * direction;
					point1 = origin + t1 * direction;
				}
				else
				{
					intrType = IntersectionType.Point;
					quantity = 1;
					point0 = origin + t0 * direction;
				}
			}
			else
			{
				quantity = 0;
				intrType = IntersectionType.Empty;
			}
			return intrType > IntersectionType.Empty;
		}

		public static bool Clip(double denom, double numer, ref double t0, ref double t1)
		{
			if (denom > 0.0)
			{
				if (numer > denom * t1)
				{
					return false;
				}
				if (numer > denom * t0)
				{
					t0 = numer / denom;
				}
				return true;
			}
			else
			{
				if (denom >= 0.0)
				{
					return numer <= 0.0;
				}
				if (numer > denom * t0)
				{
					return false;
				}
				if (numer > denom * t1)
				{
					t1 = numer / denom;
				}
				return true;
			}
		}

		private Segment3d segment;

		private Box3d box;

		private bool solid;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public double SegmentParam0;

		public double SegmentParam1;

		public Vector3d Point0 = Vector3d.Zero;

		public Vector3d Point1 = Vector3d.Zero;
	}
}
