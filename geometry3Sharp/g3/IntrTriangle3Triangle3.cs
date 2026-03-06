using System;

namespace g3
{
	public class IntrTriangle3Triangle3
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
				this.Result = IntersectionResult.NotComputed;
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
				this.Result = IntersectionResult.NotComputed;
			}
		}

		public IntrTriangle3Triangle3(Triangle3d t0, Triangle3d t1)
		{
			this.triangle0 = t0;
			this.triangle1 = t1;
		}

		public IntrTriangle3Triangle3 Compute()
		{
			this.Find();
			return this;
		}

		public bool Find()
		{
			if (this.Result != IntersectionResult.NotComputed)
			{
				return this.Result != IntersectionResult.NoIntersection;
			}
			this.Result = IntersectionResult.NoIntersection;
			Plane3d plane3d = new Plane3d(this.triangle0.V0, this.triangle0.V1, this.triangle0.V2);
			Vector3d vector3d;
			Index3i index3i;
			int num;
			int num2;
			int num3;
			IntrTriangle3Triangle3.TrianglePlaneRelations(ref this.triangle1, ref plane3d, out vector3d, out index3i, out num, out num2, out num3);
			if (num == 3 || num2 == 3)
			{
				return false;
			}
			if (num3 == 3)
			{
				return this.ReportCoplanarIntersection && this.GetCoplanarIntersection(ref plane3d, ref this.triangle0, ref this.triangle1);
			}
			if (num == 0 || num2 == 0)
			{
				if (num3 == 2)
				{
					for (int i = 0; i < 3; i++)
					{
						if (index3i[i] != 0)
						{
							int key = (i + 2) % 3;
							int key2 = (i + 1) % 3;
							return this.IntersectsSegment(ref plane3d, ref this.triangle0, this.triangle1[key], this.triangle1[key2]);
						}
					}
				}
				else
				{
					for (int i = 0; i < 3; i++)
					{
						if (index3i[i] == 0)
						{
							return this.ContainsPoint(ref this.triangle0, ref plane3d, this.triangle1[i]);
						}
					}
				}
			}
			if (num3 == 0)
			{
				int num4 = (num == 1) ? 1 : -1;
				for (int i = 0; i < 3; i++)
				{
					if (index3i[i] == num4)
					{
						int key = (i + 2) % 3;
						int key2 = (i + 1) % 3;
						double f = vector3d[i] / (vector3d[i] - vector3d[key]);
						Vector3d vector3d2 = this.triangle1[i] + f * (this.triangle1[key] - this.triangle1[i]);
						f = vector3d[i] / (vector3d[i] - vector3d[key2]);
						Vector3d end = this.triangle1[i] + f * (this.triangle1[key2] - this.triangle1[i]);
						return this.IntersectsSegment(ref plane3d, ref this.triangle0, vector3d2, end);
					}
				}
			}
			for (int i = 0; i < 3; i++)
			{
				if (index3i[i] == 0)
				{
					int key = (i + 2) % 3;
					int key2 = (i + 1) % 3;
					double f = vector3d[key] / (vector3d[key] - vector3d[key2]);
					Vector3d vector3d2 = this.triangle1[key] + f * (this.triangle1[key2] - this.triangle1[key]);
					return this.IntersectsSegment(ref plane3d, ref this.triangle0, this.triangle1[i], vector3d2);
				}
			}
			return false;
		}

		public bool Test()
		{
			return IntrTriangle3Triangle3.Intersects(ref this.triangle0, ref this.triangle1, ref this.Type);
		}

		public static bool Intersects(ref Triangle3d triangle0, ref Triangle3d triangle1)
		{
			IntersectionType intersectionType = IntersectionType.Empty;
			return IntrTriangle3Triangle3.Intersects(ref triangle0, ref triangle1, ref intersectionType);
		}

		private static bool Intersects(ref Triangle3d triangle0, ref Triangle3d triangle1, ref IntersectionType type)
		{
			Vector3dTuple3 vector3dTuple;
			vector3dTuple.V0 = triangle0.V1 - triangle0.V0;
			vector3dTuple.V1 = triangle0.V2 - triangle0.V1;
			vector3dTuple.V2 = triangle0.V0 - triangle0.V2;
			Vector3d vector3d = vector3dTuple.V0.UnitCross(ref vector3dTuple.V1);
			double num = vector3d.Dot(ref triangle0.V0);
			double num2;
			double num3;
			IntrTriangle3Triangle3.ProjectOntoAxis(ref triangle1, ref vector3d, out num2, out num3);
			if (num < num2 || num > num3)
			{
				return false;
			}
			Vector3dTuple3 vector3dTuple2;
			vector3dTuple2.V0 = triangle1.V1 - triangle1.V0;
			vector3dTuple2.V1 = triangle1.V2 - triangle1.V1;
			vector3dTuple2.V2 = triangle1.V0 - triangle1.V2;
			Vector3d vector3d2 = vector3dTuple2.V0.UnitCross(ref vector3dTuple2.V1);
			Vector3d vector3d3 = vector3d.UnitCross(ref vector3d2);
			if (vector3d3.Dot(ref vector3d3) >= 1E-08)
			{
				double num4 = vector3d2.Dot(ref triangle1.V0);
				double num5;
				double num6;
				IntrTriangle3Triangle3.ProjectOntoAxis(ref triangle0, ref vector3d2, out num5, out num6);
				if (num4 < num5 || num4 > num6)
				{
					return false;
				}
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						Vector3d vector3d4 = vector3dTuple[j].UnitCross(vector3dTuple2[i]);
						IntrTriangle3Triangle3.ProjectOntoAxis(ref triangle0, ref vector3d4, out num5, out num6);
						IntrTriangle3Triangle3.ProjectOntoAxis(ref triangle1, ref vector3d4, out num2, out num3);
						if (num6 < num2 || num3 < num5)
						{
							return false;
						}
					}
				}
				type = IntersectionType.Unknown;
			}
			else
			{
				for (int j = 0; j < 3; j++)
				{
					Vector3d vector3d4 = vector3d.UnitCross(vector3dTuple[j]);
					double num5;
					double num6;
					IntrTriangle3Triangle3.ProjectOntoAxis(ref triangle0, ref vector3d4, out num5, out num6);
					IntrTriangle3Triangle3.ProjectOntoAxis(ref triangle1, ref vector3d4, out num2, out num3);
					if (num6 < num2 || num3 < num5)
					{
						return false;
					}
				}
				for (int i = 0; i < 3; i++)
				{
					Vector3d vector3d4 = vector3d2.UnitCross(vector3dTuple2[i]);
					double num5;
					double num6;
					IntrTriangle3Triangle3.ProjectOntoAxis(ref triangle0, ref vector3d4, out num5, out num6);
					IntrTriangle3Triangle3.ProjectOntoAxis(ref triangle1, ref vector3d4, out num2, out num3);
					if (num6 < num2 || num3 < num5)
					{
						return false;
					}
				}
				type = IntersectionType.Plane;
			}
			return true;
		}

		public static void ProjectOntoAxis(ref Triangle3d triangle, ref Vector3d axis, out double fmin, out double fmax)
		{
			double num = axis.Dot(triangle.V0);
			double num2 = axis.Dot(triangle.V1);
			double num3 = axis.Dot(triangle.V2);
			fmin = num;
			fmax = fmin;
			if (num2 < fmin)
			{
				fmin = num2;
			}
			else if (num2 > fmax)
			{
				fmax = num2;
			}
			if (num3 < fmin)
			{
				fmin = num3;
				return;
			}
			if (num3 > fmax)
			{
				fmax = num3;
			}
		}

		public static void TrianglePlaneRelations(ref Triangle3d triangle, ref Plane3d plane, out Vector3d distance, out Index3i sign, out int positive, out int negative, out int zero)
		{
			positive = 0;
			negative = 0;
			zero = 0;
			distance = Vector3d.Zero;
			sign = Index3i.Zero;
			for (int i = 0; i < 3; i++)
			{
				distance[i] = plane.DistanceTo(triangle[i]);
				if (distance[i] > 1E-08)
				{
					sign[i] = 1;
					positive++;
				}
				else if (distance[i] < -1E-08)
				{
					sign[i] = -1;
					negative++;
				}
				else
				{
					distance[i] = 0.0;
					sign[i] = 0;
					zero++;
				}
			}
		}

		private bool ContainsPoint(ref Triangle3d triangle, ref Plane3d plane, Vector3d point)
		{
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d.GenerateComplementBasis(ref zero, ref zero2, plane.Normal);
			Vector3d v = point - triangle[0];
			Vector3d v2 = triangle[1] - triangle[0];
			Vector3d v3 = triangle[2] - triangle[0];
			Vector2d test = new Vector2d(zero.Dot(v), zero2.Dot(v));
			if (new QueryTuple2d(new Vector2dTuple3(Vector2d.Zero, new Vector2d(zero.Dot(v2), zero2.Dot(v2)), new Vector2d(zero.Dot(v3), zero2.Dot(v3)))).ToTriangle(test, 0, 1, 2) <= 0)
			{
				this.Result = IntersectionResult.Intersects;
				this.Type = IntersectionType.Point;
				this.Quantity = 1;
				this.Points[0] = point;
				return true;
			}
			return false;
		}

		private bool IntersectsSegment(ref Plane3d plane, ref Triangle3d triangle, Vector3d end0, Vector3d end1)
		{
			int num = 0;
			double num2 = Math.Abs(plane.Normal.x);
			double num3 = Math.Abs(plane.Normal.y);
			if (num3 > num2)
			{
				num = 1;
				num2 = num3;
			}
			num3 = Math.Abs(plane.Normal.z);
			if (num3 > num2)
			{
				num = 2;
			}
			Triangle2d t = default(Triangle2d);
			Vector2d zero = Vector2d.Zero;
			Vector2d zero2 = Vector2d.Zero;
			if (num == 0)
			{
				for (int i = 0; i < 3; i++)
				{
					t[i] = triangle[i].yz;
					zero.x = end0.y;
					zero.y = end0.z;
					zero2.x = end1.y;
					zero2.y = end1.z;
				}
			}
			else if (num == 1)
			{
				for (int i = 0; i < 3; i++)
				{
					t[i] = triangle[i].xz;
					zero.x = end0.x;
					zero.y = end0.z;
					zero2.x = end1.x;
					zero2.y = end1.z;
				}
			}
			else
			{
				for (int i = 0; i < 3; i++)
				{
					t[i] = triangle[i].xy;
					zero.x = end0.x;
					zero.y = end0.y;
					zero2.x = end1.x;
					zero2.y = end1.y;
				}
			}
			IntrSegment2Triangle2 intrSegment2Triangle = new IntrSegment2Triangle2(new Segment2d(zero, zero2), t);
			if (!intrSegment2Triangle.Find())
			{
				return false;
			}
			Vector2dTuple2 vector2dTuple = default(Vector2dTuple2);
			if (intrSegment2Triangle.Type == IntersectionType.Segment)
			{
				this.Result = IntersectionResult.Intersects;
				this.Type = IntersectionType.Segment;
				this.Quantity = 2;
				vector2dTuple.V0 = intrSegment2Triangle.Point0;
				vector2dTuple.V1 = intrSegment2Triangle.Point1;
			}
			else
			{
				this.Result = IntersectionResult.Intersects;
				this.Type = IntersectionType.Point;
				this.Quantity = 1;
				vector2dTuple.V0 = intrSegment2Triangle.Point0;
			}
			if (num == 0)
			{
				double num4 = 1.0 / plane.Normal.x;
				for (int i = 0; i < this.Quantity; i++)
				{
					double x = vector2dTuple[i].x;
					double y = vector2dTuple[i].y;
					double x2 = num4 * (plane.Constant - plane.Normal.y * x - plane.Normal.z * y);
					this.Points[i] = new Vector3d(x2, x, y);
				}
			}
			else if (num == 1)
			{
				double num5 = 1.0 / plane.Normal.y;
				for (int i = 0; i < this.Quantity; i++)
				{
					double x3 = vector2dTuple[i].x;
					double y2 = vector2dTuple[i].y;
					double y3 = num5 * (plane.Constant - plane.Normal.x * x3 - plane.Normal.z * y2);
					this.Points[i] = new Vector3d(x3, y3, y2);
				}
			}
			else
			{
				double num6 = 1.0 / plane.Normal.z;
				for (int i = 0; i < this.Quantity; i++)
				{
					double x4 = vector2dTuple[i].x;
					double y4 = vector2dTuple[i].y;
					double z = num6 * (plane.Constant - plane.Normal.x * x4 - plane.Normal.y * y4);
					this.Points[i] = new Vector3d(x4, y4, z);
				}
			}
			return true;
		}

		private bool GetCoplanarIntersection(ref Plane3d plane, ref Triangle3d tri0, ref Triangle3d tri1)
		{
			int num = 0;
			double num2 = Math.Abs(plane.Normal.x);
			double num3 = Math.Abs(plane.Normal.y);
			if (num3 > num2)
			{
				num = 1;
				num2 = num3;
			}
			num3 = Math.Abs(plane.Normal.z);
			if (num3 > num2)
			{
				num = 2;
			}
			Triangle2d t = default(Triangle2d);
			Triangle2d t2 = default(Triangle2d);
			if (num == 0)
			{
				for (int i = 0; i < 3; i++)
				{
					t[i] = tri0[i].yz;
					t2[i] = tri1[i].yz;
				}
			}
			else if (num == 1)
			{
				for (int i = 0; i < 3; i++)
				{
					t[i] = tri0[i].xz;
					t2[i] = tri1[i].xz;
				}
			}
			else
			{
				for (int i = 0; i < 3; i++)
				{
					t[i] = tri0[i].xy;
					t2[i] = tri1[i].xy;
				}
			}
			Vector2d vector2d = t[1] - t[0];
			Vector2d v = t[2] - t[0];
			if (vector2d.DotPerp(v) < 0.0)
			{
				Vector2d value = t[1];
				t[1] = t[2];
				t[2] = value;
			}
			vector2d = t2[1] - t2[0];
			v = t2[2] - t2[0];
			if (vector2d.DotPerp(v) < 0.0)
			{
				Vector2d value = t2[1];
				t2[1] = t2[2];
				t2[2] = value;
			}
			IntrTriangle2Triangle2 intrTriangle2Triangle = new IntrTriangle2Triangle2(t, t2);
			if (!intrTriangle2Triangle.Find())
			{
				return false;
			}
			this.PolygonPoints = new Vector3d[intrTriangle2Triangle.Quantity];
			this.Quantity = intrTriangle2Triangle.Quantity;
			if (num == 0)
			{
				double num4 = 1.0 / plane.Normal.x;
				for (int i = 0; i < this.Quantity; i++)
				{
					double x = intrTriangle2Triangle.Points[i].x;
					double y = intrTriangle2Triangle.Points[i].y;
					double x2 = num4 * (plane.Constant - plane.Normal.y * x - plane.Normal.z * y);
					this.PolygonPoints[i] = new Vector3d(x2, x, y);
				}
			}
			else if (num == 1)
			{
				double num5 = 1.0 / plane.Normal.y;
				for (int i = 0; i < this.Quantity; i++)
				{
					double x3 = intrTriangle2Triangle.Points[i].x;
					double y2 = intrTriangle2Triangle.Points[i].y;
					double y3 = num5 * (plane.Constant - plane.Normal.x * x3 - plane.Normal.z * y2);
					this.PolygonPoints[i] = new Vector3d(x3, y3, y2);
				}
			}
			else
			{
				double num6 = 1.0 / plane.Normal.z;
				for (int i = 0; i < this.Quantity; i++)
				{
					double x4 = intrTriangle2Triangle.Points[i].x;
					double y4 = intrTriangle2Triangle.Points[i].y;
					double z = num6 * (plane.Constant - plane.Normal.x * x4 - plane.Normal.y * y4);
					this.PolygonPoints[i] = new Vector3d(x4, y4, z);
				}
			}
			this.Result = IntersectionResult.Intersects;
			this.Type = IntersectionType.Polygon;
			return true;
		}

		private Triangle3d triangle0;

		private Triangle3d triangle1;

		public bool ReportCoplanarIntersection;

		public int Quantity;

		public IntersectionResult Result;

		public IntersectionType Type;

		public Vector3dTuple3 Points;

		public Vector3d[] PolygonPoints;
	}
}
