using System;

namespace g3
{
	public class DistSegment3Triangle3
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
				this.DistanceSquared = -1.0;
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
				this.DistanceSquared = -1.0;
			}
		}

		public DistSegment3Triangle3(Segment3d SegmentIn, Triangle3d TriangleIn)
		{
			this.triangle = TriangleIn;
			this.segment = SegmentIn;
		}

		public DistSegment3Triangle3 Compute()
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
			DistLine3Triangle3 distLine3Triangle = new DistLine3Triangle3(new Line3d(this.segment.Center, this.segment.Direction), this.triangle);
			double squared = distLine3Triangle.GetSquared();
			this.SegmentParam = distLine3Triangle.LineParam;
			if (this.SegmentParam >= -this.segment.Extent)
			{
				if (this.SegmentParam <= this.segment.Extent)
				{
					this.SegmentClosest = distLine3Triangle.LineClosest;
					this.TriangleClosest = distLine3Triangle.TriangleClosest;
					this.TriangleBaryCoords = distLine3Triangle.TriangleBaryCoords;
				}
				else
				{
					this.SegmentClosest = this.segment.P1;
					DistPoint3Triangle3 distPoint3Triangle = new DistPoint3Triangle3(this.SegmentClosest, this.triangle);
					squared = distPoint3Triangle.GetSquared();
					this.TriangleClosest = distPoint3Triangle.TriangleClosest;
					this.SegmentParam = this.segment.Extent;
					this.TriangleBaryCoords = distPoint3Triangle.TriangleBaryCoords;
				}
			}
			else
			{
				this.SegmentClosest = this.segment.P0;
				DistPoint3Triangle3 distPoint3Triangle2 = new DistPoint3Triangle3(this.SegmentClosest, this.triangle);
				squared = distPoint3Triangle2.GetSquared();
				this.TriangleClosest = distPoint3Triangle2.TriangleClosest;
				this.SegmentParam = -this.segment.Extent;
				this.TriangleBaryCoords = distPoint3Triangle2.TriangleBaryCoords;
			}
			this.DistanceSquared = squared;
			return this.DistanceSquared;
		}

		private Segment3d segment;

		private Triangle3d triangle;

		public double DistanceSquared = -1.0;

		public Vector3d SegmentClosest;

		public double SegmentParam;

		public Vector3d TriangleClosest;

		public Vector3d TriangleBaryCoords;
	}
}
