using System;

namespace g3
{
	public class IntrLine2Segment2
	{
		public Line2d Line
		{
			get
			{
				return this.line;
			}
			set
			{
				this.line = value;
				this.Result = IntersectionResult.NotComputed;
			}
		}

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

		public IntrLine2Segment2(Line2d line, Segment2d seg)
		{
			this.line = line;
			this.segment = seg;
		}

		public IntrLine2Segment2 Compute()
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
			if (!this.line.Direction.IsNormalized || !this.segment.Direction.IsNormalized)
			{
				this.Type = IntersectionType.Empty;
				this.Result = IntersectionResult.InvalidQuery;
				return false;
			}
			Vector2d zero = Vector2d.Zero;
			this.Type = IntrLine2Line2.Classify(this.line.Origin, this.line.Direction, this.segment.Center, this.segment.Direction, this.dotThresh, ref zero);
			if (this.Type == IntersectionType.Point)
			{
				if (Math.Abs(zero[1]) <= this.segment.Extent + this.intervalThresh)
				{
					this.Quantity = 1;
					this.Point = this.line.Origin + zero[0] * this.line.Direction;
					this.Parameter = zero[0];
				}
				else
				{
					this.Quantity = 0;
					this.Type = IntersectionType.Empty;
				}
			}
			else if (this.Type == IntersectionType.Line)
			{
				this.Type = IntersectionType.Segment;
				this.Quantity = int.MaxValue;
			}
			else
			{
				this.Quantity = 0;
			}
			this.Result = ((this.Type != IntersectionType.Empty) ? IntersectionResult.Intersects : IntersectionResult.NoIntersection);
			return this.Result == IntersectionResult.Intersects;
		}

		private Line2d line;

		private Segment2d segment;

		private double intervalThresh;

		private double dotThresh = 1E-08;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public Vector2d Point;

		public double Parameter;
	}
}
