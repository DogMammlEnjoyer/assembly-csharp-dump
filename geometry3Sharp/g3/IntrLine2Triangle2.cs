using System;

namespace g3
{
	public class IntrLine2Triangle2
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

		public IntrLine2Triangle2(Line2d l, Triangle2d t)
		{
			this.line = l;
			this.triangle = t;
		}

		public IntrLine2Triangle2 Compute()
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
			if (!this.line.Direction.IsNormalized)
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
			IntrLine2Triangle2.TriangleLineRelations(this.line.Origin, this.line.Direction, this.triangle, ref zero, ref zero2, ref num, ref num2, ref num3);
			if (num == 3 || num2 == 3)
			{
				this.Quantity = 0;
				this.Type = IntersectionType.Empty;
			}
			else
			{
				Vector2d zero3 = Vector2d.Zero;
				IntrLine2Triangle2.GetInterval(this.line.Origin, this.line.Direction, this.triangle, zero, zero2, ref zero3);
				Intersector1 intersector = new Intersector1(zero3[0], zero3[1], double.MinValue, double.MaxValue);
				intersector.Find();
				this.Quantity = intersector.NumIntersections;
				if (this.Quantity == 2)
				{
					this.Type = IntersectionType.Segment;
					this.Param0 = intersector.GetIntersection(0);
					this.Point0 = this.line.Origin + this.Param0 * this.line.Direction;
					this.Param1 = intersector.GetIntersection(1);
					this.Point1 = this.line.Origin + this.Param1 * this.line.Direction;
				}
				else if (this.Quantity == 1)
				{
					this.Type = IntersectionType.Point;
					this.Param0 = intersector.GetIntersection(0);
					this.Point0 = this.line.Origin + this.Param0 * this.line.Direction;
				}
				else
				{
					this.Type = IntersectionType.Empty;
				}
			}
			this.Result = ((this.Type != IntersectionType.Empty) ? IntersectionResult.Intersects : IntersectionResult.NoIntersection);
			return this.Result == IntersectionResult.Intersects;
		}

		public static void TriangleLineRelations(Vector2d origin, Vector2d direction, Triangle2d tri, ref Vector3d dist, ref Vector3i sign, ref int positive, ref int negative, ref int zero)
		{
			positive = 0;
			negative = 0;
			zero = 0;
			for (int i = 0; i < 3; i++)
			{
				dist[i] = (tri[i] - origin).DotPerp(direction);
				if (dist[i] > 1E-08)
				{
					sign[i] = 1;
					positive++;
				}
				else if (dist[i] < -1E-08)
				{
					sign[i] = -1;
					negative++;
				}
				else
				{
					dist[i] = 0.0;
					sign[i] = 0;
					zero++;
				}
			}
		}

		public static void GetInterval(Vector2d origin, Vector2d direction, Triangle2d tri, Vector3d dist, Vector3i sign, ref Vector2d param)
		{
			Vector3d zero = Vector3d.Zero;
			for (int i = 0; i < 3; i++)
			{
				Vector2d v = tri[i] - origin;
				zero[i] = direction.Dot(v);
			}
			int num = 0;
			int key = 2;
			int j = 0;
			while (j < 3)
			{
				if (sign[key] * sign[j] < 0)
				{
					if (num >= 2)
					{
						throw new Exception("IntrLine2Triangle2.GetInterval: too many intersections!");
					}
					double num2 = dist[key] * zero[j] - dist[j] * zero[key];
					double num3 = dist[key] - dist[j];
					param[num++] = num2 / num3;
				}
				key = j++;
			}
			if (num < 2)
			{
				int k = 0;
				while (k < 3)
				{
					if (sign[k] == 0)
					{
						if (num >= 2)
						{
							throw new Exception("IntrLine2Triangle2.GetInterval: too many intersections!");
						}
						param[num++] = zero[k];
					}
					int num4 = k++;
				}
			}
			if (num < 1)
			{
				throw new Exception("IntrLine2Triangle2.GetInterval: need at least one intersection");
			}
			if (num == 2)
			{
				if (param[0] > param[1])
				{
					double value = param[0];
					param[0] = param[1];
					param[1] = value;
					return;
				}
			}
			else
			{
				param[1] = param[0];
			}
		}

		private Line2d line;

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
