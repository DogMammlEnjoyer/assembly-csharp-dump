using System;

namespace g3
{
	public class IntrTriangle2Triangle2
	{
		public Triangle2d Triangle0
		{
			get
			{
				return this.triangle0;
			}
			set
			{
				this.triangle0 = value;
				this.Result = IntersectionResult.NotComputed;
			}
		}

		public Triangle2d Triangle1
		{
			get
			{
				return this.triangle1;
			}
			set
			{
				this.triangle1 = value;
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

		public IntrTriangle2Triangle2(Triangle2d t0, Triangle2d t1)
		{
			this.triangle0 = t0;
			this.triangle1 = t1;
			this.Points = null;
		}

		public bool Test()
		{
			Vector2d zero = Vector2d.Zero;
			int i = 0;
			int key = 2;
			while (i < 3)
			{
				zero.x = this.triangle0[i].y - this.triangle0[key].y;
				zero.y = this.triangle0[key].x - this.triangle0[i].x;
				if (IntrTriangle2Triangle2.WhichSide(this.triangle1, this.triangle0[key], zero) > 0)
				{
					return false;
				}
				key = i++;
			}
			i = 0;
			key = 2;
			while (i < 3)
			{
				zero.x = this.triangle1[i].y - this.triangle1[key].y;
				zero.y = this.triangle1[key].x - this.triangle1[i].x;
				if (IntrTriangle2Triangle2.WhichSide(this.triangle0, this.triangle1[key], zero) > 0)
				{
					return false;
				}
				key = i++;
			}
			return true;
		}

		public IntrTriangle2Triangle2 Compute()
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
			this.Quantity = 3;
			this.Points = new Vector2d[6];
			for (int i = 0; i < 3; i++)
			{
				this.Points[i] = this.triangle1[i];
			}
			int key = 2;
			int j = 0;
			while (j < 3)
			{
				Vector2d n = new Vector2d(this.triangle0[key].y - this.triangle0[j].y, this.triangle0[j].x - this.triangle0[key].x);
				double c = n.Dot(this.triangle0[key]);
				IntrTriangle2Triangle2.ClipConvexPolygonAgainstLine(n, c, ref this.Quantity, ref this.Points);
				if (this.Quantity == 0)
				{
					this.Type = IntersectionType.Empty;
				}
				else if (this.Quantity == 1)
				{
					this.Type = IntersectionType.Point;
				}
				else if (this.Quantity == 2)
				{
					this.Type = IntersectionType.Segment;
				}
				else
				{
					this.Type = IntersectionType.Polygon;
				}
				key = j++;
			}
			this.Result = ((this.Type != IntersectionType.Empty) ? IntersectionResult.Intersects : IntersectionResult.NoIntersection);
			return this.Result == IntersectionResult.Intersects;
		}

		public static int WhichSide(Triangle2d V, Vector2d P, Vector2d D)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < 3; i++)
			{
				double num4 = D.Dot(V[i] - P);
				if (num4 > 0.0)
				{
					num++;
				}
				else if (num4 < 0.0)
				{
					num2++;
				}
				else
				{
					num3++;
				}
				if (num > 0 && num2 > 0)
				{
					return 0;
				}
			}
			if (num3 != 0)
			{
				return 0;
			}
			if (num <= 0)
			{
				return -1;
			}
			return 1;
		}

		public static void ClipConvexPolygonAgainstLine(Vector2d N, double c, ref int quantity, ref Vector2d[] V)
		{
			int num = 0;
			int num2 = 0;
			int num3 = -1;
			double[] array = new double[6];
			for (int i = 0; i < quantity; i++)
			{
				array[i] = N.Dot(V[i]) - c;
				if (array[i] > 0.0)
				{
					num++;
					if (num3 < 0)
					{
						num3 = i;
					}
				}
				else if (array[i] < 0.0)
				{
					num2++;
				}
			}
			if (num > 0)
			{
				if (num2 > 0)
				{
					Vector2d[] array2 = new Vector2d[6];
					int num4 = 0;
					if (num3 > 0)
					{
						int j = num3;
						int num5 = j - 1;
						double f = array[j] / (array[j] - array[num5]);
						array2[num4++] = V[j] + f * (V[num5] - V[j]);
						while (j < quantity && array[j] > 0.0)
						{
							array2[num4++] = V[j++];
						}
						if (j < quantity)
						{
							num5 = j - 1;
						}
						else
						{
							j = 0;
							num5 = quantity - 1;
						}
						f = array[j] / (array[j] - array[num5]);
						array2[num4++] = V[j] + f * (V[num5] - V[j]);
					}
					else
					{
						int j = 0;
						while (j < quantity && array[j] > 0.0)
						{
							array2[num4++] = V[j++];
						}
						int num5 = j - 1;
						double f = array[j] / (array[j] - array[num5]);
						array2[num4++] = V[j] + f * (V[num5] - V[j]);
						while (j < quantity && array[j] <= 0.0)
						{
							j++;
						}
						if (j < quantity)
						{
							num5 = j - 1;
							f = array[j] / (array[j] - array[num5]);
							array2[num4++] = V[j] + f * (V[num5] - V[j]);
							while (j < quantity)
							{
								if (array[j] <= 0.0)
								{
									break;
								}
								array2[num4++] = V[j++];
							}
						}
						else
						{
							num5 = quantity - 1;
							f = array[0] / (array[0] - array[num5]);
							array2[num4++] = V[0] + f * (V[num5] - V[0]);
						}
					}
					quantity = num4;
					Array.Copy(array2, V, num4);
					return;
				}
			}
			else
			{
				quantity = 0;
			}
		}

		private Triangle2d triangle0;

		private Triangle2d triangle1;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public Vector2d[] Points;
	}
}
