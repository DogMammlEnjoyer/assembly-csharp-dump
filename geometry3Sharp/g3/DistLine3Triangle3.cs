using System;

namespace g3
{
	public class DistLine3Triangle3
	{
		public Line3d Line
		{
			get
			{
				return this.line;
			}
			set
			{
				this.line = value;
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

		public DistLine3Triangle3(Line3d LineIn, Triangle3d TriangleIn)
		{
			this.triangle = TriangleIn;
			this.line = LineIn;
		}

		public DistLine3Triangle3 Compute()
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
			Vector3d vector3d = this.triangle.V1 - this.triangle.V0;
			Vector3d vector3d2 = this.triangle.V2 - this.triangle.V0;
			if (Math.Abs(vector3d.UnitCross(vector3d2).Dot(this.line.Direction)) > 1E-08)
			{
				Vector3d v = this.line.Origin - this.triangle.V0;
				Vector3d zero = Vector3d.Zero;
				Vector3d zero2 = Vector3d.Zero;
				Vector3d.GenerateComplementBasis(ref zero, ref zero2, this.line.Direction);
				double num = zero.Dot(vector3d);
				double num2 = zero.Dot(vector3d2);
				double num3 = zero.Dot(v);
				double num4 = zero2.Dot(vector3d);
				double num5 = zero2.Dot(vector3d2);
				double num6 = zero2.Dot(v);
				double num7 = 1.0 / (num * num5 - num2 * num4);
				double num8 = (num5 * num3 - num2 * num6) * num7;
				double num9 = (num * num6 - num4 * num3) * num7;
				double num10 = 1.0 - num8 - num9;
				if (num10 >= 0.0 && num8 >= 0.0 && num9 >= 0.0)
				{
					double num11 = this.line.Direction.Dot(vector3d);
					double num12 = this.line.Direction.Dot(vector3d2);
					double num13 = this.line.Direction.Dot(v);
					this.LineParam = num8 * num11 + num9 * num12 - num13;
					this.TriangleBaryCoords = new Vector3d(num10, num8, num9);
					this.LineClosest = this.line.Origin + this.LineParam * this.line.Direction;
					this.TriangleClosest = this.triangle.V0 + num8 * vector3d + num9 * vector3d2;
					this.DistanceSquared = 0.0;
					return 0.0;
				}
			}
			double num14 = double.MaxValue;
			int num15 = 2;
			int i = 0;
			while (i < 3)
			{
				Segment3d segment3d = new Segment3d(this.triangle[num15], this.triangle[i]);
				DistLine3Segment3 distLine3Segment = new DistLine3Segment3(this.line, segment3d);
				double squared = distLine3Segment.GetSquared();
				if (squared < num14)
				{
					this.LineClosest = distLine3Segment.LineClosest;
					this.TriangleClosest = distLine3Segment.SegmentClosest;
					num14 = squared;
					this.LineParam = distLine3Segment.LineParameter;
					double num16 = distLine3Segment.SegmentParameter / segment3d.Extent;
					this.TriangleBaryCoords = Vector3d.Zero;
					this.TriangleBaryCoords[num15] = 0.5 * (1.0 - num16);
					this.TriangleBaryCoords[i] = 1.0 - this.TriangleBaryCoords[num15];
					this.TriangleBaryCoords[3 - num15 - i] = 0.0;
				}
				num15 = i++;
			}
			this.DistanceSquared = num14;
			return this.DistanceSquared;
		}

		private Line3d line;

		private Triangle3d triangle;

		public double DistanceSquared = -1.0;

		public Vector3d LineClosest;

		public double LineParam;

		public Vector3d TriangleClosest;

		public Vector3d TriangleBaryCoords;
	}
}
