using System;
using System.Collections.Generic;

namespace g3
{
	public class Arrangement2d
	{
		public Arrangement2d(AxisAlignedBox2d boundsHint)
		{
			this.Graph = new DGraph2();
			double cellSize = boundsHint.MaxDim / 64.0;
			this.PointHash = new PointHashGrid2d<int>(cellSize, -1);
		}

		public void Insert(Vector2d a, Vector2d b, int gid = -1)
		{
			this.insert_segment(a, b, gid, 0.0);
		}

		public void Insert(Segment2d segment, int gid = -1)
		{
			this.insert_segment(segment.P0, segment.P1, gid, 0.0);
		}

		public void Insert(PolyLine2d pline, int gid = -1)
		{
			int num = pline.VertexCount - 1;
			for (int i = 0; i < num; i++)
			{
				Vector2d a = pline[i];
				Vector2d b = pline[i + 1];
				this.insert_segment(a, b, gid, 0.0);
			}
		}

		public void Insert(Polygon2d poly, int gid = -1)
		{
			int vertexCount = poly.VertexCount;
			for (int i = 0; i < vertexCount; i++)
			{
				Vector2d a = poly[i];
				Vector2d b = poly[(i + 1) % vertexCount];
				this.insert_segment(a, b, gid, 0.0);
			}
		}

		public void ConnectOpenBoundaries(double distThresh)
		{
			int maxVertexID = this.Graph.MaxVertexID;
			for (int i = 0; i < maxVertexID; i++)
			{
				if (this.Graph.IsBoundaryVertex(i))
				{
					Vector2d vertex = this.Graph.GetVertex(i);
					int num = this.find_nearest_boundary_vertex(vertex, distThresh, i);
					if (num != -1)
					{
						Vector2d vertex2 = this.Graph.GetVertex(num);
						this.Insert(vertex, vertex2, -1);
					}
				}
			}
		}

		protected bool insert_segment(ref Vector2d a, ref Vector2d b, int gid = -1, double tol = 0.0)
		{
			int num = this.find_existing_vertex(a);
			int num2 = this.find_existing_vertex(b);
			if (num == num2 && num >= 0)
			{
				return false;
			}
			List<Arrangement2d.Intersection> list = new List<Arrangement2d.Intersection>();
			this.find_intersecting_edges(ref a, ref b, list, tol);
			int count = list.Count;
			List<Arrangement2d.SegmentPoint> list2 = new List<Arrangement2d.SegmentPoint>();
			Segment2d segment2d = new Segment2d(a, b);
			for (int i = 0; i < count; i++)
			{
				Arrangement2d.Intersection intersection = list[i];
				int eid = intersection.eid;
				double parameter = intersection.intr.Parameter0;
				double parameter2 = intersection.intr.Parameter1;
				int num3 = -1;
				if (intersection.intr.Type == IntersectionType.Point || intersection.intr.Type == IntersectionType.Segment)
				{
					Index2i index2i = this.split_segment_at_t(eid, parameter, this.VertexSnapTol);
					num3 = index2i.b;
					Vector2d vertex = this.Graph.GetVertex(index2i.a);
					list2.Add(new Arrangement2d.SegmentPoint
					{
						t = segment2d.Project(vertex),
						vid = index2i.a
					});
				}
				if (intersection.intr.Type == IntersectionType.Segment)
				{
					if (num3 == -1)
					{
						Index2i index2i2 = this.split_segment_at_t(eid, parameter2, this.VertexSnapTol);
						Vector2d vertex2 = this.Graph.GetVertex(index2i2.a);
						list2.Add(new Arrangement2d.SegmentPoint
						{
							t = segment2d.Project(vertex2),
							vid = index2i2.a
						});
					}
					else
					{
						Segment2d edgeSegment = this.Graph.GetEdgeSegment(num3);
						Vector2d p = intersection.intr.Segment1.PointAt(parameter2);
						double t = edgeSegment.Project(p);
						Index2i index2i3 = this.split_segment_at_t(num3, t, this.VertexSnapTol);
						Vector2d vertex3 = this.Graph.GetVertex(index2i3.a);
						list2.Add(new Arrangement2d.SegmentPoint
						{
							t = segment2d.Project(vertex3),
							vid = index2i3.a
						});
					}
				}
			}
			if (num == -1)
			{
				num = this.find_existing_vertex(a);
			}
			if (num == -1)
			{
				num = this.Graph.AppendVertex(a);
				this.PointHash.InsertPointUnsafe(num, a);
			}
			if (num2 == -1)
			{
				num2 = this.find_existing_vertex(b);
			}
			if (num2 == -1)
			{
				num2 = this.Graph.AppendVertex(b);
				this.PointHash.InsertPointUnsafe(num2, b);
			}
			list2.Add(new Arrangement2d.SegmentPoint
			{
				t = segment2d.Project(a),
				vid = num
			});
			list2.Add(new Arrangement2d.SegmentPoint
			{
				t = segment2d.Project(b),
				vid = num2
			});
			list2.Sort(delegate(Arrangement2d.SegmentPoint pa, Arrangement2d.SegmentPoint pb)
			{
				if (pa.t < pb.t)
				{
					return -1;
				}
				if (pa.t <= pb.t)
				{
					return 0;
				}
				return 1;
			});
			for (int j = 0; j < list2.Count - 1; j++)
			{
				int vid = list2[j].vid;
				int vid2 = list2[j + 1].vid;
				if (vid != vid2)
				{
					if (Math.Abs(list2[j].t - list2[j + 1].t) < 1.1920928955078125E-07)
					{
						Console.WriteLine("insert_segment: different points with same t??");
					}
					if (this.Graph.FindEdge(vid, vid2) == -1)
					{
						this.Graph.AppendEdge(vid, vid2, gid);
					}
				}
			}
			return true;
		}

		protected bool insert_segment(Vector2d a, Vector2d b, int gid = -1, double tol = 0.0)
		{
			return this.insert_segment(ref a, ref b, gid, tol);
		}

		protected Index2i split_segment_at_t(int eid, double t, double tol)
		{
			Index2i edgeV = this.Graph.GetEdgeV(eid);
			Segment2d segment2d = new Segment2d(this.Graph.GetVertex(edgeV.a), this.Graph.GetVertex(edgeV.b));
			int jj = -1;
			int num;
			if (t < -(segment2d.Extent - tol))
			{
				num = edgeV.a;
			}
			else if (t > segment2d.Extent - tol)
			{
				num = edgeV.b;
			}
			else
			{
				DGraph.EdgeSplitInfo edgeSplitInfo;
				if (this.Graph.SplitEdge(eid, out edgeSplitInfo) != MeshResult.Ok)
				{
					throw new Exception("insert_into_segment: edge split failed?");
				}
				num = edgeSplitInfo.vNew;
				jj = edgeSplitInfo.eNewBN;
				Vector2d vector2d = segment2d.PointAt(t);
				this.Graph.SetVertex(num, vector2d);
				this.PointHash.InsertPointUnsafe(edgeSplitInfo.vNew, vector2d);
			}
			return new Index2i(num, jj);
		}

		protected int find_existing_vertex(Vector2d pt)
		{
			return this.find_nearest_vertex(pt, this.VertexSnapTol, -1);
		}

		protected int find_nearest_vertex(Vector2d pt, double searchRadius, int ignore_vid = -1)
		{
			KeyValuePair<int, double> keyValuePair = (ignore_vid == -1) ? this.PointHash.FindNearestInRadius(pt, searchRadius, (int b) => pt.DistanceSquared(this.Graph.GetVertex(b)), null) : this.PointHash.FindNearestInRadius(pt, searchRadius, (int b) => pt.DistanceSquared(this.Graph.GetVertex(b)), (int vid) => vid == ignore_vid);
			if (keyValuePair.Key == this.PointHash.InvalidValue)
			{
				return -1;
			}
			return keyValuePair.Key;
		}

		protected int find_nearest_boundary_vertex(Vector2d pt, double searchRadius, int ignore_vid = -1)
		{
			KeyValuePair<int, double> keyValuePair = this.PointHash.FindNearestInRadius(pt, searchRadius, (int b) => pt.Distance(this.Graph.GetVertex(b)), (int vid) => !this.Graph.IsBoundaryVertex(vid) || vid == ignore_vid);
			if (keyValuePair.Key == this.PointHash.InvalidValue)
			{
				return -1;
			}
			return keyValuePair.Key;
		}

		protected bool find_intersecting_edges(ref Vector2d a, ref Vector2d b, List<Arrangement2d.Intersection> hits, double tol = 0.0)
		{
			int num = 0;
			Vector2d zero = Vector2d.Zero;
			Vector2d zero2 = Vector2d.Zero;
			foreach (int num2 in this.Graph.EdgeIndices())
			{
				this.Graph.GetEdgeV(num2, ref zero, ref zero2);
				int num3 = Segment2d.WhichSide(ref a, ref b, ref zero, tol);
				int num4 = Segment2d.WhichSide(ref a, ref b, ref zero2, tol);
				if (num3 != num4 || num3 == 0)
				{
					IntrSegment2Segment2 intrSegment2Segment = new IntrSegment2Segment2(new Segment2d(zero, zero2), new Segment2d(a, b));
					if (intrSegment2Segment.Find())
					{
						hits.Add(new Arrangement2d.Intersection
						{
							eid = num2,
							sidex = num3,
							sidey = num4,
							intr = intrSegment2Segment
						});
						num++;
					}
				}
			}
			return num > 0;
		}

		public DGraph2 Graph;

		public PointHashGrid2d<int> PointHash;

		public double VertexSnapTol = 1E-05;

		protected struct SegmentPoint
		{
			public double t;

			public int vid;
		}

		protected struct Intersection
		{
			public int eid;

			public int sidex;

			public int sidey;

			public IntrSegment2Segment2 intr;
		}
	}
}
