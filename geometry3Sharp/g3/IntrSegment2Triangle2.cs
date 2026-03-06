using System;

namespace g3
{
	public class IntrSegment2Triangle2
	{
		public Segment2d Segment
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

		public Triangle2d Triangle
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

		public IntrSegment2Triangle2(Segment2d s, Triangle2d t)
		{
			this.segment = s;
			this.triangle = t;
		}

		public IntrSegment2Triangle2 Compute()
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
			Vector3d zero = Vector3d.Zero;
			Vector3i zero2 = Vector3i.Zero;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			IntrLine2Triangle2.TriangleLineRelations(this.segment.Center, this.segment.Direction, this.triangle, ref zero, ref zero2, ref num, ref num2, ref num3);
			if (num == 3 || num2 == 3)
			{
				this.Quantity = 0;
				this.Type = IntersectionType.Empty;
			}
			else
			{
				Vector2d zero3 = Vector2d.Zero;
				IntrLine2Triangle2.GetInterval(this.segment.Center, this.segment.Direction, this.triangle, zero, zero2, ref zero3);
				Intersector1 intersector = new Intersector1(zero3[0], zero3[1], -this.segment.Extent, this.segment.Extent);
				intersector.Find();
				this.Quantity = intersector.NumIntersections;
				if (this.Quantity == 2)
				{
					this.Type = IntersectionType.Segment;
					this.Param0 = intersector.GetIntersection(0);
					this.Point0 = this.segment.Center + this.Param0 * this.segment.Direction;
					this.Param1 = intersector.GetIntersection(1);
					this.Point1 = this.segment.Center + this.Param1 * this.segment.Direction;
				}
				else if (this.Quantity == 1)
				{
					this.Type = IntersectionType.Point;
					this.Param0 = intersector.GetIntersection(0);
					this.Point0 = this.segment.Center + this.Param0 * this.segment.Direction;
				}
				else
				{
					this.Type = IntersectionType.Empty;
				}
			}
			this.Result = ((this.Type != IntersectionType.Empty) ? IntersectionResult.Intersects : IntersectionResult.NoIntersection);
			return this.Result == IntersectionResult.Intersects;
		}

		private Segment2d segment;

		private Triangle2d triangle;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public Vector2d Point0;

		public Vector2d Point1;

		public double Param0;

		public double Param1;
	}
}
