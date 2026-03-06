using System;

namespace g3
{
	public class DistTriangle3Triangle3
	{
		public Triangle3d Triangle0
		{
			get
			{
				return this.triangle0;
			}
			set
			{
				this.triangle0 = value;
				this.DistanceSquared = -1.0;
			}
		}

		public Triangle3d Triangle1
		{
			get
			{
				return this.triangle1;
			}
			set
			{
				this.triangle1 = value;
				this.DistanceSquared = -1.0;
			}
		}

		public DistTriangle3Triangle3(Triangle3d Triangle0in, Triangle3d Triangle1in)
		{
			this.triangle0 = Triangle0in;
			this.triangle1 = Triangle1in;
		}

		public DistTriangle3Triangle3 Compute()
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
			double num = double.MaxValue;
			Segment3d segment3d = default(Segment3d);
			int num2 = 2;
			int i = 0;
			while (i < 3)
			{
				segment3d.SetEndpoints(this.triangle0[num2], this.triangle0[i]);
				DistSegment3Triangle3 distSegment3Triangle = new DistSegment3Triangle3(segment3d, this.triangle1);
				double squared = distSegment3Triangle.GetSquared();
				if (squared < num)
				{
					this.Triangle0Closest = distSegment3Triangle.SegmentClosest;
					this.Triangle1Closest = distSegment3Triangle.TriangleClosest;
					num = squared;
					double num3 = distSegment3Triangle.SegmentParam / segment3d.Extent;
					this.Triangle0BaryCoords = Vector3d.Zero;
					this.Triangle0BaryCoords[num2] = 0.5 * (1.0 - num3);
					this.Triangle0BaryCoords[i] = 1.0 - this.Triangle0BaryCoords[num2];
					this.Triangle0BaryCoords[3 - num2 - i] = 0.0;
					this.Triangle1BaryCoords = distSegment3Triangle.TriangleBaryCoords;
					if (num <= 1E-08)
					{
						this.DistanceSquared = 0.0;
						return 0.0;
					}
				}
				num2 = i++;
			}
			num2 = 2;
			i = 0;
			while (i < 3)
			{
				segment3d.SetEndpoints(this.triangle1[num2], this.triangle1[i]);
				DistSegment3Triangle3 distSegment3Triangle2 = new DistSegment3Triangle3(segment3d, this.triangle0);
				double squared = distSegment3Triangle2.GetSquared();
				if (squared < num)
				{
					this.Triangle0Closest = distSegment3Triangle2.SegmentClosest;
					this.Triangle1Closest = distSegment3Triangle2.TriangleClosest;
					num = squared;
					double num3 = distSegment3Triangle2.SegmentParam / segment3d.Extent;
					this.Triangle1BaryCoords = Vector3d.Zero;
					this.Triangle1BaryCoords[num2] = 0.5 * (1.0 - num3);
					this.Triangle1BaryCoords[i] = 1.0 - this.Triangle1BaryCoords[num2];
					this.Triangle1BaryCoords[3 - num2 - i] = 0.0;
					this.Triangle0BaryCoords = distSegment3Triangle2.TriangleBaryCoords;
					if (num <= 1E-08)
					{
						this.DistanceSquared = 0.0;
						return 0.0;
					}
				}
				num2 = i++;
			}
			this.DistanceSquared = num;
			return this.DistanceSquared;
		}

		private Triangle3d triangle0;

		private Triangle3d triangle1;

		public double DistanceSquared = -1.0;

		public Vector3d Triangle0Closest;

		public Vector3d Triangle0BaryCoords;

		public Vector3d Triangle1Closest;

		public Vector3d Triangle1BaryCoords;
	}
}
