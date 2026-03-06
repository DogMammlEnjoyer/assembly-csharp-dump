using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class DelaunayTriangle
	{
		public bool IsInterior { get; set; }

		public DelaunayTriangle(TriangulationPoint p1, TriangulationPoint p2, TriangulationPoint p3)
		{
			this.Points[0] = p1;
			this.Points[1] = p2;
			this.Points[2] = p3;
		}

		public int IndexOf(TriangulationPoint p)
		{
			int num = this.Points.IndexOf(p);
			if (num == -1)
			{
				throw new Exception("Calling index with a point that doesn't exist in triangle");
			}
			return num;
		}

		public int IndexCWFrom(TriangulationPoint p)
		{
			return (this.IndexOf(p) + 2) % 3;
		}

		public int IndexCCWFrom(TriangulationPoint p)
		{
			return (this.IndexOf(p) + 1) % 3;
		}

		public bool Contains(TriangulationPoint p)
		{
			return this.Points.Contains(p);
		}

		private void MarkNeighbor(TriangulationPoint p1, TriangulationPoint p2, DelaunayTriangle t)
		{
			int num = this.EdgeIndex(p1, p2);
			if (num == -1)
			{
				throw new Exception("Error marking neighbors -- t doesn't contain edge p1-p2!");
			}
			this.Neighbors[num] = t;
		}

		public void MarkNeighbor(DelaunayTriangle t)
		{
			bool flag = t.Contains(this.Points[0]);
			bool flag2 = t.Contains(this.Points[1]);
			bool flag3 = t.Contains(this.Points[2]);
			if (flag2 && flag3)
			{
				this.Neighbors[0] = t;
				t.MarkNeighbor(this.Points[1], this.Points[2], this);
				return;
			}
			if (flag && flag3)
			{
				this.Neighbors[1] = t;
				t.MarkNeighbor(this.Points[0], this.Points[2], this);
				return;
			}
			if (flag && flag2)
			{
				this.Neighbors[2] = t;
				t.MarkNeighbor(this.Points[0], this.Points[1], this);
				return;
			}
			throw new Exception("Failed to mark neighbor, doesn't share an edge!");
		}

		public TriangulationPoint OppositePoint(DelaunayTriangle t, TriangulationPoint p)
		{
			return this.PointCWFrom(t.PointCWFrom(p));
		}

		public DelaunayTriangle NeighborCWFrom(TriangulationPoint point)
		{
			return this.Neighbors[(this.Points.IndexOf(point) + 1) % 3];
		}

		public DelaunayTriangle NeighborCCWFrom(TriangulationPoint point)
		{
			return this.Neighbors[(this.Points.IndexOf(point) + 2) % 3];
		}

		public DelaunayTriangle NeighborAcrossFrom(TriangulationPoint point)
		{
			return this.Neighbors[this.Points.IndexOf(point)];
		}

		public TriangulationPoint PointCCWFrom(TriangulationPoint point)
		{
			return this.Points[(this.IndexOf(point) + 1) % 3];
		}

		public TriangulationPoint PointCWFrom(TriangulationPoint point)
		{
			return this.Points[(this.IndexOf(point) + 2) % 3];
		}

		private void RotateCW()
		{
			TriangulationPoint value = this.Points[2];
			this.Points[2] = this.Points[1];
			this.Points[1] = this.Points[0];
			this.Points[0] = value;
		}

		public void Legalize(TriangulationPoint oPoint, TriangulationPoint nPoint)
		{
			this.RotateCW();
			this.Points[this.IndexCCWFrom(oPoint)] = nPoint;
		}

		public override string ToString()
		{
			string[] array = new string[5];
			int num = 0;
			TriangulationPoint triangulationPoint = this.Points[0];
			array[num] = ((triangulationPoint != null) ? triangulationPoint.ToString() : null);
			array[1] = ",";
			int num2 = 2;
			TriangulationPoint triangulationPoint2 = this.Points[1];
			array[num2] = ((triangulationPoint2 != null) ? triangulationPoint2.ToString() : null);
			array[3] = ",";
			int num3 = 4;
			TriangulationPoint triangulationPoint3 = this.Points[2];
			array[num3] = ((triangulationPoint3 != null) ? triangulationPoint3.ToString() : null);
			return string.Concat(array);
		}

		public void MarkNeighborEdges()
		{
			for (int i = 0; i < 3; i++)
			{
				if (this.EdgeIsConstrained[i] && this.Neighbors[i] != null)
				{
					this.Neighbors[i].MarkConstrainedEdge(this.Points[(i + 1) % 3], this.Points[(i + 2) % 3]);
				}
			}
		}

		public void MarkEdge(DelaunayTriangle triangle)
		{
			for (int i = 0; i < 3; i++)
			{
				if (this.EdgeIsConstrained[i])
				{
					triangle.MarkConstrainedEdge(this.Points[(i + 1) % 3], this.Points[(i + 2) % 3]);
				}
			}
		}

		public void MarkEdge(List<DelaunayTriangle> tList)
		{
			foreach (DelaunayTriangle delaunayTriangle in tList)
			{
				for (int i = 0; i < 3; i++)
				{
					if (delaunayTriangle.EdgeIsConstrained[i])
					{
						this.MarkConstrainedEdge(delaunayTriangle.Points[(i + 1) % 3], delaunayTriangle.Points[(i + 2) % 3]);
					}
				}
			}
		}

		public void MarkConstrainedEdge(int index)
		{
			this.EdgeIsConstrained[index] = true;
		}

		public void MarkConstrainedEdge(DTSweepConstraint edge)
		{
			this.MarkConstrainedEdge(edge.P, edge.Q);
		}

		public void MarkConstrainedEdge(TriangulationPoint p, TriangulationPoint q)
		{
			int num = this.EdgeIndex(p, q);
			if (num != -1)
			{
				this.EdgeIsConstrained[num] = true;
			}
		}

		public double Area()
		{
			double num = this.Points[0].X - this.Points[1].X;
			double num2 = this.Points[2].Y - this.Points[1].Y;
			return Math.Abs(num * num2 * 0.5);
		}

		public TriangulationPoint Centroid()
		{
			double x = (this.Points[0].X + this.Points[1].X + this.Points[2].X) / 3.0;
			double y = (this.Points[0].Y + this.Points[1].Y + this.Points[2].Y) / 3.0;
			return new TriangulationPoint(x, y, -1);
		}

		public int EdgeIndex(TriangulationPoint p1, TriangulationPoint p2)
		{
			int num = this.Points.IndexOf(p1);
			int num2 = this.Points.IndexOf(p2);
			bool flag = num == 0 || num2 == 0;
			bool flag2 = num == 1 || num2 == 1;
			bool flag3 = num == 2 || num2 == 2;
			if (flag2 && flag3)
			{
				return 0;
			}
			if (flag && flag3)
			{
				return 1;
			}
			if (flag && flag2)
			{
				return 2;
			}
			return -1;
		}

		public bool GetConstrainedEdgeCCW(TriangulationPoint p)
		{
			return this.EdgeIsConstrained[(this.IndexOf(p) + 2) % 3];
		}

		public bool GetConstrainedEdgeCW(TriangulationPoint p)
		{
			return this.EdgeIsConstrained[(this.IndexOf(p) + 1) % 3];
		}

		public bool GetConstrainedEdgeAcross(TriangulationPoint p)
		{
			return this.EdgeIsConstrained[this.IndexOf(p)];
		}

		public void SetConstrainedEdgeCCW(TriangulationPoint p, bool ce)
		{
			this.EdgeIsConstrained[(this.IndexOf(p) + 2) % 3] = ce;
		}

		public void SetConstrainedEdgeCW(TriangulationPoint p, bool ce)
		{
			this.EdgeIsConstrained[(this.IndexOf(p) + 1) % 3] = ce;
		}

		public void SetConstrainedEdgeAcross(TriangulationPoint p, bool ce)
		{
			this.EdgeIsConstrained[this.IndexOf(p)] = ce;
		}

		public bool GetDelaunayEdgeCCW(TriangulationPoint p)
		{
			return this.EdgeIsDelaunay[(this.IndexOf(p) + 2) % 3];
		}

		public bool GetDelaunayEdgeCW(TriangulationPoint p)
		{
			return this.EdgeIsDelaunay[(this.IndexOf(p) + 1) % 3];
		}

		public bool GetDelaunayEdgeAcross(TriangulationPoint p)
		{
			return this.EdgeIsDelaunay[this.IndexOf(p)];
		}

		public void SetDelaunayEdgeCCW(TriangulationPoint p, bool ce)
		{
			this.EdgeIsDelaunay[(this.IndexOf(p) + 2) % 3] = ce;
		}

		public void SetDelaunayEdgeCW(TriangulationPoint p, bool ce)
		{
			this.EdgeIsDelaunay[(this.IndexOf(p) + 1) % 3] = ce;
		}

		public void SetDelaunayEdgeAcross(TriangulationPoint p, bool ce)
		{
			this.EdgeIsDelaunay[this.IndexOf(p)] = ce;
		}

		public FixedArray3<TriangulationPoint> Points;

		public FixedArray3<DelaunayTriangle> Neighbors;

		public FixedBitArray3 EdgeIsConstrained;

		public FixedBitArray3 EdgeIsDelaunay;
	}
}
