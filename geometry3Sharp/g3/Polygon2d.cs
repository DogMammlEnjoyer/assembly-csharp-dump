using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace g3
{
	public class Polygon2d : IDuplicatable<Polygon2d>
	{
		public Polygon2d()
		{
			this.vertices = new List<Vector2d>();
			this.Timestamp = 0;
		}

		public Polygon2d(Polygon2d copy)
		{
			this.vertices = new List<Vector2d>(copy.vertices);
			this.Timestamp = 0;
		}

		public Polygon2d(IList<Vector2d> copy)
		{
			this.vertices = new List<Vector2d>(copy);
			this.Timestamp = 0;
		}

		public Polygon2d(IEnumerable<Vector2d> copy)
		{
			this.vertices = new List<Vector2d>(copy);
			this.Timestamp = 0;
		}

		public Polygon2d(Vector2d[] v)
		{
			this.vertices = new List<Vector2d>(v);
			this.Timestamp = 0;
		}

		public Polygon2d(VectorArray2d v)
		{
			this.vertices = new List<Vector2d>(v.AsVector2d());
			this.Timestamp = 0;
		}

		public Polygon2d(double[] values)
		{
			int num = values.Length / 2;
			this.vertices = new List<Vector2d>(num);
			for (int i = 0; i < num; i++)
			{
				this.vertices.Add(new Vector2d(values[2 * i], values[2 * i + 1]));
			}
			this.Timestamp = 0;
		}

		public Polygon2d(Func<int, Vector2d> SourceF, int N)
		{
			this.vertices = new List<Vector2d>();
			for (int i = 0; i < N; i++)
			{
				this.vertices.Add(SourceF(i));
			}
			this.Timestamp = 0;
		}

		public virtual Polygon2d Duplicate()
		{
			return new Polygon2d(this)
			{
				Timestamp = this.Timestamp
			};
		}

		public Vector2d this[int key]
		{
			get
			{
				return this.vertices[key];
			}
			set
			{
				this.vertices[key] = value;
				this.Timestamp++;
			}
		}

		public Vector2d Start
		{
			get
			{
				return this.vertices[0];
			}
		}

		public ReadOnlyCollection<Vector2d> Vertices
		{
			get
			{
				return this.vertices.AsReadOnly();
			}
		}

		public int VertexCount
		{
			get
			{
				return this.vertices.Count;
			}
		}

		public void AppendVertex(Vector2d v)
		{
			this.vertices.Add(v);
			this.Timestamp++;
		}

		public void AppendVertices(IEnumerable<Vector2d> v)
		{
			this.vertices.AddRange(v);
			this.Timestamp++;
		}

		public void RemoveVertex(int idx)
		{
			this.vertices.RemoveAt(idx);
			this.Timestamp++;
		}

		public void SetVertices(List<Vector2d> newVertices, bool bTakeOwnership)
		{
			if (bTakeOwnership)
			{
				this.vertices = newVertices;
				return;
			}
			this.vertices.Clear();
			int count = newVertices.Count;
			for (int i = 0; i < count; i++)
			{
				this.vertices.Add(newVertices[i]);
			}
		}

		public void Reverse()
		{
			this.vertices.Reverse();
			this.Timestamp++;
		}

		public Vector2d GetTangent(int i)
		{
			Vector2d a = this.vertices[(i + 1) % this.vertices.Count];
			Vector2d o = this.vertices[(i == 0) ? (this.vertices.Count - 1) : (i - 1)];
			return (a - o).Normalized;
		}

		public Vector2d GetNormal(int i)
		{
			return this.GetTangent(i).Perp;
		}

		public Vector2d GetNormal_FaceAvg(int i)
		{
			Vector2d a = this.vertices[(i + 1) % this.vertices.Count];
			Vector2d vector2d = this.vertices[(i == 0) ? (this.vertices.Count - 1) : (i - 1)];
			a -= this.vertices[i];
			a.Normalize(2.220446049250313E-16);
			vector2d -= this.vertices[i];
			vector2d.Normalize(2.220446049250313E-16);
			Vector2d result = a.Perp - vector2d.Perp;
			if (result.Normalize(2.220446049250313E-16) == 0.0)
			{
				return (a + vector2d).Normalized;
			}
			return result;
		}

		public AxisAlignedBox2d GetBounds()
		{
			AxisAlignedBox2d empty = AxisAlignedBox2d.Empty;
			empty.Contain(this.vertices);
			return empty;
		}

		public AxisAlignedBox2d Bounds
		{
			get
			{
				return this.GetBounds();
			}
		}

		public IEnumerable<Segment2d> SegmentItr()
		{
			int num;
			for (int i = 0; i < this.vertices.Count; i = num)
			{
				yield return new Segment2d(this.vertices[i], this.vertices[(i + 1) % this.vertices.Count]);
				num = i + 1;
			}
			yield break;
		}

		public IEnumerable<Vector2d> VerticesItr(bool bRepeatFirstAtEnd)
		{
			int N = this.vertices.Count;
			int num;
			for (int i = 0; i < N; i = num)
			{
				yield return this.vertices[i];
				num = i + 1;
			}
			if (bRepeatFirstAtEnd)
			{
				yield return this.vertices[0];
			}
			yield break;
		}

		public IEnumerable<Index2i> EdgeItr()
		{
			int num;
			for (int i = 0; i < this.VertexCount; i = num)
			{
				yield return new Index2i(i, (i != this.VertexCount - 1) ? (i + 1) : 0);
				num = i + 1;
			}
			yield break;
		}

		public bool IsClockwise
		{
			get
			{
				return this.SignedArea < 0.0;
			}
		}

		public bool BiContains(Segment2d seg)
		{
			foreach (Segment2d segment2d in this.SegmentItr())
			{
				if (segment2d.BiEquals(seg))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsMember(Segment2d seg, out bool IsOutside)
		{
			IsOutside = true;
			if (this.Vertices.Contains(seg.P0) && this.Vertices.Contains(seg.P1))
			{
				if (this.BiContains(seg))
				{
					IsOutside = false;
				}
				return true;
			}
			return false;
		}

		public double SignedArea
		{
			get
			{
				double num = 0.0;
				int count = this.vertices.Count;
				if (count == 0)
				{
					return 0.0;
				}
				Vector2d vector2d = this.vertices[0];
				Vector2d vector2d2 = Vector2d.Zero;
				for (int i = 0; i < count; i++)
				{
					vector2d2 = this.vertices[(i + 1) % count];
					num += vector2d.x * vector2d2.y - vector2d.y * vector2d2.x;
					vector2d = vector2d2;
				}
				return num * 0.5;
			}
		}

		public double Area
		{
			get
			{
				return Math.Abs(this.SignedArea);
			}
		}

		public double Perimeter
		{
			get
			{
				double num = 0.0;
				int count = this.vertices.Count;
				for (int i = 0; i < count; i++)
				{
					num += this.vertices[i].Distance(this.vertices[(i + 1) % count]);
				}
				return num;
			}
		}

		public double ArcLength
		{
			get
			{
				return this.Perimeter;
			}
		}

		public void NeighboursP(int iVertex, ref Vector2d p0, ref Vector2d p1)
		{
			int count = this.vertices.Count;
			p0 = this.vertices[(iVertex == 0) ? (count - 1) : (iVertex - 1)];
			p1 = this.vertices[(iVertex + 1) % count];
		}

		public void NeighboursV(int iVertex, ref Vector2d v0, ref Vector2d v1, bool bNormalize = false)
		{
			int count = this.vertices.Count;
			v0 = this.vertices[(iVertex == 0) ? (count - 1) : (iVertex - 1)] - this.vertices[iVertex];
			v1 = this.vertices[(iVertex + 1) % count] - this.vertices[iVertex];
			if (bNormalize)
			{
				v0.Normalize(2.220446049250313E-16);
				v1.Normalize(2.220446049250313E-16);
			}
		}

		public double OpeningAngleDeg(int iVertex)
		{
			Vector2d zero = Vector2d.Zero;
			Vector2d zero2 = Vector2d.Zero;
			this.NeighboursV(iVertex, ref zero, ref zero2, true);
			return Vector2d.AngleD(zero, zero2);
		}

		public double WindingIntegral(Vector2d P)
		{
			double num = 0.0;
			int count = this.vertices.Count;
			Vector2d vector2d = this.vertices[0] - P;
			Vector2d vector2d2 = Vector2d.Zero;
			for (int i = 0; i < count; i++)
			{
				vector2d2 = this.vertices[(i + 1) % count] - P;
				num += Math.Atan2(vector2d.x * vector2d2.y - vector2d.y * vector2d2.x, vector2d.x * vector2d2.x + vector2d.y * vector2d2.y);
				vector2d = vector2d2;
			}
			return num / 6.283185307179586;
		}

		public bool Contains(Vector2d P)
		{
			int num = 0;
			int count = this.vertices.Count;
			Vector2d vector2d = this.vertices[0];
			Vector2d vector2d2 = Vector2d.Zero;
			for (int i = 0; i < count; i++)
			{
				vector2d2 = this.vertices[(i + 1) % count];
				if (vector2d.y <= P.y)
				{
					if (vector2d2.y > P.y && MathUtil.IsLeft(ref vector2d, ref vector2d2, ref P) > 0.0)
					{
						num++;
					}
				}
				else if (vector2d2.y <= P.y && MathUtil.IsLeft(ref vector2d, ref vector2d2, ref P) < 0.0)
				{
					num--;
				}
				vector2d = vector2d2;
			}
			return num != 0;
		}

		public bool Contains(Polygon2d o)
		{
			int vertexCount = o.VertexCount;
			for (int i = 0; i < vertexCount; i++)
			{
				if (!this.Contains(o[i]))
				{
					return false;
				}
			}
			return !this.Intersects(o);
		}

		public bool Contains(Segment2d o)
		{
			if (!this.Contains(o.P0) || !this.Contains(o.P1))
			{
				return false;
			}
			foreach (Segment2d segment2d in this.SegmentItr())
			{
				if (segment2d.Intersects(o, 5E-324, 0.0))
				{
					return false;
				}
			}
			return true;
		}

		public bool Intersects(Polygon2d o)
		{
			if (!this.GetBounds().Intersects(o.GetBounds()))
			{
				return false;
			}
			foreach (Segment2d segment2d in this.SegmentItr())
			{
				foreach (Segment2d seg in o.SegmentItr())
				{
					if (segment2d.Intersects(seg, 5E-324, 0.0))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool Intersects(Segment2d o)
		{
			if (this.Contains(o.P0) || this.Contains(o.P1))
			{
				return true;
			}
			foreach (Segment2d segment2d in this.SegmentItr())
			{
				if (segment2d.Intersects(o, 5E-324, 0.0))
				{
					return true;
				}
			}
			return false;
		}

		public List<Vector2d> FindIntersections(Polygon2d o)
		{
			List<Vector2d> list = new List<Vector2d>();
			if (!this.GetBounds().Intersects(o.GetBounds()))
			{
				return list;
			}
			foreach (Segment2d seg in this.SegmentItr())
			{
				foreach (Segment2d seg2 in o.SegmentItr())
				{
					if (seg.Intersects(seg2, 5E-324, 0.0))
					{
						IntrSegment2Segment2 intrSegment2Segment = new IntrSegment2Segment2(seg, seg2);
						if (intrSegment2Segment.Find())
						{
							list.Add(intrSegment2Segment.Point0);
							if (intrSegment2Segment.Quantity == 2)
							{
								list.Add(intrSegment2Segment.Point1);
							}
						}
					}
				}
			}
			return list;
		}

		public List<Vector2d> FindIntersections(Segment2d s)
		{
			List<Vector2d> list = new List<Vector2d>();
			foreach (Segment2d seg in this.SegmentItr())
			{
				if (seg.Intersects(s, 5E-324, 0.0))
				{
					IntrSegment2Segment2 intrSegment2Segment = new IntrSegment2Segment2(seg, s);
					if (intrSegment2Segment.Find())
					{
						list.Add(intrSegment2Segment.Point0);
						if (intrSegment2Segment.Quantity == 2)
						{
							list.Add(intrSegment2Segment.Point1);
						}
					}
				}
			}
			return list;
		}

		public Segment2d Segment(int iSegment)
		{
			return new Segment2d(this.vertices[iSegment], this.vertices[(iSegment + 1) % this.vertices.Count]);
		}

		public Vector2d PointAt(int iSegment, double fSegT)
		{
			Segment2d segment2d = new Segment2d(this.vertices[iSegment], this.vertices[(iSegment + 1) % this.vertices.Count]);
			return segment2d.PointAt(fSegT);
		}

		public Vector2d GetNormal(int iSeg, double segT)
		{
			Segment2d segment2d = new Segment2d(this.vertices[iSeg], this.vertices[(iSeg + 1) % this.vertices.Count]);
			double num = (segT / segment2d.Extent + 1.0) / 2.0;
			Vector2d normal = this.GetNormal(iSeg);
			Vector2d normal2 = this.GetNormal((iSeg + 1) % this.vertices.Count);
			return ((1.0 - num) * normal + num * normal2).Normalized;
		}

		public double DistanceSquared(Vector2d p, out int iNearSeg, out double fNearSegT)
		{
			iNearSeg = -1;
			fNearSegT = double.MaxValue;
			double num = double.MaxValue;
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				Segment2d segment2d = new Segment2d(this.vertices[i], this.vertices[(i + 1) % count]);
				double num2 = (p - segment2d.Center).Dot(segment2d.Direction);
				double num3;
				if (num2 >= segment2d.Extent)
				{
					num3 = segment2d.P1.DistanceSquared(p);
				}
				else if (num2 <= -segment2d.Extent)
				{
					num3 = segment2d.P0.DistanceSquared(p);
				}
				else
				{
					num3 = (segment2d.PointAt(num2) - p).LengthSquared;
				}
				if (num3 < num)
				{
					num = num3;
					iNearSeg = i;
					fNearSegT = num2;
				}
			}
			return num;
		}

		public double DistanceSquared(Vector2d p)
		{
			int num;
			double num2;
			return this.DistanceSquared(p, out num, out num2);
		}

		public double AverageEdgeLength
		{
			get
			{
				double num = 0.0;
				int count = this.vertices.Count;
				for (int i = 1; i < count; i++)
				{
					num += this.vertices[i].Distance(this.vertices[i - 1]);
				}
				num += this.vertices[count - 1].Distance(this.vertices[0]);
				return num / (double)count;
			}
		}

		public Polygon2d Translate(Vector2d translate)
		{
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				List<Vector2d> list = this.vertices;
				int index = i;
				list[index] += translate;
			}
			this.Timestamp++;
			return this;
		}

		public Polygon2d Rotate(Matrix2d rotation, Vector2d origin)
		{
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				this.vertices[i] = rotation * (this.vertices[i] - origin) + origin;
			}
			this.Timestamp++;
			return this;
		}

		public Polygon2d Scale(Vector2d scale, Vector2d origin)
		{
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				this.vertices[i] = scale * (this.vertices[i] - origin) + origin;
			}
			this.Timestamp++;
			return this;
		}

		public Polygon2d Transform(Func<Vector2d, Vector2d> transformF)
		{
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				this.vertices[i] = transformF(this.vertices[i]);
			}
			this.Timestamp++;
			return this;
		}

		public Polygon2d Transform(ITransform2 xform)
		{
			int count = this.vertices.Count;
			for (int i = 0; i < count; i++)
			{
				this.vertices[i] = xform.TransformP(this.vertices[i]);
			}
			this.Timestamp++;
			return this;
		}

		public void VtxNormalOffset(double dist, bool bUseFaceAvg = false)
		{
			Vector2d[] array = new Vector2d[this.vertices.Count];
			if (bUseFaceAvg)
			{
				for (int i = 0; i < this.vertices.Count; i++)
				{
					array[i] = this.vertices[i] + dist * this.GetNormal_FaceAvg(i);
				}
			}
			else
			{
				for (int j = 0; j < this.vertices.Count; j++)
				{
					array[j] = this.vertices[j] + dist * this.GetNormal(j);
				}
			}
			for (int k = 0; k < this.vertices.Count; k++)
			{
				this.vertices[k] = array[k];
			}
			this.Timestamp++;
		}

		public void PolyOffset(double dist)
		{
			Vector2d[] array = new Vector2d[this.vertices.Count];
			for (int i = 0; i < this.vertices.Count; i++)
			{
				Vector2d vector2d = this.vertices[i];
				Vector2d a = this.vertices[(i + 1) % this.vertices.Count];
				Vector2d a2 = this.vertices[(i == 0) ? (this.vertices.Count - 1) : (i - 1)];
				Vector2d normalized = (a - vector2d).Normalized;
				Vector2d normalized2 = (a2 - vector2d).Normalized;
				Line2d line2d = new Line2d(vector2d + dist * normalized.Perp, normalized);
				Line2d line2d2 = new Line2d(vector2d - dist * normalized2.Perp, normalized2);
				array[i] = line2d.IntersectionPoint(ref line2d2, 1E-08);
				if (array[i] == Vector2d.MaxValue)
				{
					array[i] = this.vertices[i] + dist * this.GetNormal_FaceAvg(i);
				}
			}
			for (int j = 0; j < this.vertices.Count; j++)
			{
				this.vertices[j] = array[j];
			}
			this.Timestamp++;
		}

		private static void simplifyDP(double tol, Vector2d[] v, int j, int k, bool[] mk)
		{
			if (k <= j + 1)
			{
				return;
			}
			int num = j;
			double num2 = 0.0;
			double num3 = tol * tol;
			Segment2d segment2d = new Segment2d(v[j], v[k]);
			for (int i = j + 1; i < k; i++)
			{
				double num4 = segment2d.DistanceSquared(v[i]);
				if (num4 > num2)
				{
					num = i;
					num2 = num4;
				}
			}
			if (num2 > num3)
			{
				mk[num] = true;
				Polygon2d.simplifyDP(tol, v, j, num, mk);
				Polygon2d.simplifyDP(tol, v, num, k, mk);
			}
		}

		public void Simplify(double clusterTol = 0.0001, double lineDeviationTol = 0.01, bool bSimplifyStraightLines = true)
		{
			int count = this.vertices.Count;
			if (count < 3)
			{
				return;
			}
			Vector2d[] array = new Vector2d[count + 1];
			bool[] array2 = new bool[count + 1];
			int i;
			for (i = 0; i < count + 1; i++)
			{
				array2[i] = false;
			}
			double num = clusterTol * clusterTol;
			array[0] = this.vertices[0];
			i = 1;
			int num2 = 1;
			int index = 0;
			while (i < count)
			{
				if ((this.vertices[i] - this.vertices[index]).LengthSquared >= num)
				{
					array[num2++] = this.vertices[i];
					index = i;
				}
				i++;
			}
			bool flag = false;
			if (num2 == 1)
			{
				array[num2++] = this.vertices[1];
				array[num2++] = this.vertices[2];
				flag = true;
			}
			else if (num2 == 2)
			{
				array[num2++] = this.vertices[0];
				flag = true;
			}
			array[num2++] = this.vertices[0];
			int num3 = 0;
			if (!flag && lineDeviationTol > 0.0)
			{
				array2[0] = (array2[num2 - 1] = true);
				Polygon2d.simplifyDP(lineDeviationTol, array, 0, num2 - 1, array2);
				for (i = 0; i < num2 - 1; i++)
				{
					if (array2[i])
					{
						num3++;
					}
				}
			}
			else
			{
				for (i = 0; i < num2; i++)
				{
					array2[i] = true;
				}
				num3 = num2 - 1;
			}
			if (num3 == 2)
			{
				for (i = 1; i < num2 - 1; i++)
				{
					if (!array2[1])
					{
						array2[1] = true;
					}
					else if (!array2[num2 - 2])
					{
						array2[num2 - 2] = true;
					}
				}
				num3++;
			}
			else if (num3 == 1)
			{
				array2[1] = true;
				array2[2] = true;
				num3 += 2;
			}
			this.vertices = new List<Vector2d>();
			for (i = 0; i < num2 - 1; i++)
			{
				if (array2[i])
				{
					this.vertices.Add(array[i]);
				}
			}
			this.Timestamp++;
		}

		public void Chamfer(double chamfer_dist, double minConvexAngleDeg = 30.0, double minConcaveAngleDeg = 30.0)
		{
			if (this.IsClockwise)
			{
				throw new Exception("must be ccw?");
			}
			List<Vector2d> list = new List<Vector2d>();
			int count = this.Vertices.Count;
			int num = 0;
			do
			{
				Vector2d vector2d = this.Vertices[num];
				int index = (num == 0) ? (count - 1) : (num - 1);
				Vector2d a = this.Vertices[index];
				int num2 = (num + 1) % count;
				Vector2d a2 = this.Vertices[num2];
				Vector2d vector2d2 = a - vector2d;
				double num3 = vector2d2.Normalize(2.220446049250313E-16);
				Vector2d vector2d3 = a2 - vector2d;
				double num4 = vector2d3.Normalize(2.220446049250313E-16);
				if (num3 < 9.999999974752427E-07 || num4 < 9.999999974752427E-07)
				{
					num = num2;
				}
				else
				{
					double num5 = Vector2d.AngleD(vector2d2, vector2d3);
					double num6 = (vector2d2.Perp.Dot(vector2d3) > 0.0) ? minConcaveAngleDeg : minConvexAngleDeg;
					if (num5 > num6)
					{
						list.Add(vector2d);
						num = num2;
					}
					else
					{
						double f = Math.Min(chamfer_dist, num3 * 0.5);
						Vector2d item = vector2d + f * vector2d2;
						double f2 = Math.Min(chamfer_dist, num4 * 0.5);
						Vector2d item2 = vector2d + f2 * vector2d3;
						list.Add(item);
						list.Add(item2);
						num = num2;
					}
				}
			}
			while (num != 0);
			this.vertices = list;
			this.Timestamp++;
		}

		public Vector2d PointInPolygon()
		{
			AxisAlignedBox2d bounds = this.Bounds;
			Vector2d corner = bounds.GetCorner(3);
			Vector2d corner2 = bounds.GetCorner(1);
			if (this.Vertices.Contains(corner) && this.Vertices.Contains(corner2))
			{
				corner = bounds.GetCorner(2);
				corner2 = bounds.GetCorner(0);
			}
			List<Vector2d> list = this.FindIntersections(new Segment2d(corner, corner2));
			Segment2d segment2d = new Segment2d(list[0], list[1]);
			if (this.Contains(segment2d.Center))
			{
				return segment2d.Center;
			}
			throw new Exception("Failed to find a point in the polygon");
		}

		public Box2d MinimalBoundingBox(double epsilon)
		{
			return new ContMinBox2(this.vertices, epsilon, QueryNumberType.QT_DOUBLE, false).MinBox;
		}

		public static Polygon2d MakeRectangle(Vector2d center, double width, double height)
		{
			VectorArray2d vectorArray2d = new VectorArray2d(4);
			vectorArray2d.Set(0, center.x - width / 2.0, center.y - height / 2.0);
			vectorArray2d.Set(1, center.x + width / 2.0, center.y - height / 2.0);
			vectorArray2d.Set(2, center.x + width / 2.0, center.y + height / 2.0);
			vectorArray2d.Set(3, center.x - width / 2.0, center.y + height / 2.0);
			return new Polygon2d(vectorArray2d);
		}

		public static Polygon2d MakeCircle(double fRadius, int nSteps, double angleShiftRad = 0.0)
		{
			VectorArray2d vectorArray2d = new VectorArray2d(nSteps);
			for (int i = 0; i < nSteps; i++)
			{
				double num = (double)i / (double)nSteps;
				double num2 = 6.283185307179586 * num + angleShiftRad;
				vectorArray2d.Set(i, fRadius * Math.Cos(num2), fRadius * Math.Sin(num2));
			}
			return new Polygon2d(vectorArray2d);
		}

		protected List<Vector2d> vertices;

		public int Timestamp;
	}
}
