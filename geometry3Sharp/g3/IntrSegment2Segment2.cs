using System;

namespace g3
{
	public class IntrSegment2Segment2
	{
		public Segment2d Segment1
		{
			get
			{
				return this.segment1;
			}
			set
			{
				this.segment1 = value;
				this.Result = IntersectionResult.NotComputed;
			}
		}

		public Segment2d Segment2
		{
			get
			{
				return this.segment2;
			}
			set
			{
				this.segment2 = value;
				this.Result = IntersectionResult.NotComputed;
			}
		}

		public double IntervalThreshold
		{
			get
			{
				return this.intervalThresh;
			}
			set
			{
				this.intervalThresh = Math.Max(value, 0.0);
				this.Result = IntersectionResult.NotComputed;
			}
		}

		public double DotThreshold
		{
			get
			{
				return this.dotThresh;
			}
			set
			{
				this.dotThresh = Math.Max(value, 0.0);
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

		public IntrSegment2Segment2(Segment2d seg1, Segment2d seg2)
		{
			this.segment1 = seg1;
			this.segment2 = seg2;
		}

		public IntrSegment2Segment2 Compute()
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
			if (!this.segment1.Direction.IsNormalized || !this.segment2.Direction.IsNormalized)
			{
				this.Type = IntersectionType.Empty;
				this.Result = IntersectionResult.InvalidQuery;
				return false;
			}
			Vector2d zero = Vector2d.Zero;
			this.Type = IntrLine2Line2.Classify(this.segment1.Center, this.segment1.Direction, this.segment2.Center, this.segment2.Direction, this.dotThresh, ref zero);
			if (this.Type == IntersectionType.Point)
			{
				if (Math.Abs(zero[0]) <= this.segment1.Extent + this.intervalThresh && Math.Abs(zero[1]) <= this.segment2.Extent + this.intervalThresh)
				{
					this.Quantity = 1;
					this.Point0 = this.segment1.Center + zero[0] * this.segment1.Direction;
					this.Parameter0 = zero[0];
				}
				else
				{
					this.Quantity = 0;
					this.Type = IntersectionType.Empty;
				}
			}
			else if (this.Type == IntersectionType.Line)
			{
				Vector2d v = this.segment2.Center - this.segment1.Center;
				double num = this.segment1.Direction.Dot(v);
				double v2 = num - this.segment2.Extent;
				double v3 = num + this.segment2.Extent;
				Intersector1 intersector = new Intersector1(-this.segment1.Extent, this.segment1.Extent, v2, v3);
				intersector.Find();
				this.Quantity = intersector.NumIntersections;
				if (this.Quantity == 2)
				{
					this.Type = IntersectionType.Segment;
					this.Parameter0 = intersector.GetIntersection(0);
					this.Point0 = this.segment1.Center + this.Parameter0 * this.segment1.Direction;
					this.Parameter1 = intersector.GetIntersection(1);
					this.Point1 = this.segment1.Center + this.Parameter1 * this.segment1.Direction;
				}
				else if (this.Quantity == 1)
				{
					this.Type = IntersectionType.Point;
					this.Parameter0 = intersector.GetIntersection(0);
					this.Point0 = this.segment1.Center + this.Parameter0 * this.segment1.Direction;
				}
				else
				{
					this.Type = IntersectionType.Empty;
				}
			}
			else
			{
				this.Quantity = 0;
			}
			this.Result = ((this.Type != IntersectionType.Empty) ? IntersectionResult.Intersects : IntersectionResult.NoIntersection);
			return this.Result == IntersectionResult.Intersects;
		}

		private void sanity_check()
		{
			if (this.Quantity != 0 && this.Quantity != 1)
			{
				int quantity = this.Quantity;
			}
		}

		private Segment2d segment1;

		private Segment2d segment2;

		private double intervalThresh;

		private double dotThresh = 1E-08;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public Vector2d Point0;

		public Vector2d Point1;

		public double Parameter0;

		public double Parameter1;
	}
}
