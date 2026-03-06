using System;

namespace g3
{
	public class IntrLine2Line2
	{
		public Line2d Line1
		{
			get
			{
				return this.line1;
			}
			set
			{
				this.line1 = value;
				this.Result = IntersectionResult.NotComputed;
			}
		}

		public Line2d Line2
		{
			get
			{
				return this.line2;
			}
			set
			{
				this.line2 = value;
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

		public IntrLine2Line2(Line2d l1, Line2d l2)
		{
			this.line1 = l1;
			this.line2 = l2;
		}

		public IntrLine2Line2 Compute()
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
			if (!this.line1.Direction.IsNormalized || !this.line2.Direction.IsNormalized)
			{
				this.Type = IntersectionType.Empty;
				this.Result = IntersectionResult.InvalidQuery;
				return false;
			}
			Vector2d zero = Vector2d.Zero;
			this.Type = IntrLine2Line2.Classify(this.line1.Origin, this.line1.Direction, this.line2.Origin, this.line2.Direction, this.dotThresh, ref zero);
			if (this.Type == IntersectionType.Point)
			{
				this.Quantity = 1;
				this.Point = this.line1.Origin + zero.x * this.line1.Direction;
				this.Segment1Parameter = zero.x;
				this.Segment2Parameter = zero.y;
			}
			else if (this.Type == IntersectionType.Line)
			{
				this.Quantity = int.MaxValue;
			}
			else
			{
				this.Quantity = 0;
			}
			this.Result = ((this.Type != IntersectionType.Empty) ? IntersectionResult.Intersects : IntersectionResult.NoIntersection);
			return this.Result == IntersectionResult.Intersects;
		}

		public static IntersectionType Classify(Vector2d P0, Vector2d D0, Vector2d P1, Vector2d D1, double dotThreshold, ref Vector2d s)
		{
			dotThreshold = Math.Max(dotThreshold, 0.0);
			Vector2d vector2d = P1 - P0;
			double num = D0.DotPerp(D1);
			if (Math.Abs(num) > dotThreshold)
			{
				double num2 = 1.0 / num;
				double num3 = vector2d.DotPerp(D0);
				double num4 = vector2d.DotPerp(D1);
				s[0] = num4 * num2;
				s[1] = num3 * num2;
				return IntersectionType.Point;
			}
			vector2d.Normalize(2.220446049250313E-16);
			if (Math.Abs(vector2d.DotPerp(D1)) <= dotThreshold)
			{
				return IntersectionType.Line;
			}
			return IntersectionType.Empty;
		}

		private Line2d line1;

		private Line2d line2;

		private double dotThresh = 1E-08;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public Vector2d Point;

		public double Segment1Parameter;

		public double Segment2Parameter;
	}
}
