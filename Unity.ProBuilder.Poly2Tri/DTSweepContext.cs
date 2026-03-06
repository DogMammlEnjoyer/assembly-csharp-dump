using System;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class DTSweepContext : TriangulationContext
	{
		public TriangulationPoint Head { get; set; }

		public TriangulationPoint Tail { get; set; }

		public DTSweepContext()
		{
			this.Clear();
		}

		public override bool IsDebugEnabled
		{
			get
			{
				return base.IsDebugEnabled;
			}
			protected set
			{
				if (value && base.DebugContext == null)
				{
					base.DebugContext = new DTSweepDebugContext(this);
				}
				base.IsDebugEnabled = value;
			}
		}

		public void RemoveFromList(DelaunayTriangle triangle)
		{
			this.Triangles.Remove(triangle);
		}

		public void MeshClean(DelaunayTriangle triangle)
		{
			this.MeshCleanReq(triangle);
		}

		private void MeshCleanReq(DelaunayTriangle triangle)
		{
			if (triangle != null && !triangle.IsInterior)
			{
				triangle.IsInterior = true;
				base.Triangulatable.AddTriangle(triangle);
				for (int i = 0; i < 3; i++)
				{
					if (!triangle.EdgeIsConstrained[i])
					{
						this.MeshCleanReq(triangle.Neighbors[i]);
					}
				}
			}
		}

		public override void Clear()
		{
			base.Clear();
			this.Triangles.Clear();
		}

		public void AddNode(AdvancingFrontNode node)
		{
			this.Front.AddNode(node);
		}

		public void RemoveNode(AdvancingFrontNode node)
		{
			this.Front.RemoveNode(node);
		}

		public AdvancingFrontNode LocateNode(TriangulationPoint point)
		{
			return this.Front.LocateNode(point);
		}

		public void CreateAdvancingFront()
		{
			DelaunayTriangle delaunayTriangle = new DelaunayTriangle(this.Points[0], this.Tail, this.Head);
			this.Triangles.Add(delaunayTriangle);
			AdvancingFrontNode advancingFrontNode = new AdvancingFrontNode(delaunayTriangle.Points[1]);
			advancingFrontNode.Triangle = delaunayTriangle;
			AdvancingFrontNode advancingFrontNode2 = new AdvancingFrontNode(delaunayTriangle.Points[0]);
			advancingFrontNode2.Triangle = delaunayTriangle;
			AdvancingFrontNode tail = new AdvancingFrontNode(delaunayTriangle.Points[2]);
			this.Front = new AdvancingFront(advancingFrontNode, tail);
			this.Front.AddNode(advancingFrontNode2);
			this.Front.Head.Next = advancingFrontNode2;
			advancingFrontNode2.Next = this.Front.Tail;
			advancingFrontNode2.Prev = this.Front.Head;
			this.Front.Tail.Prev = advancingFrontNode2;
		}

		public void MapTriangleToNodes(DelaunayTriangle t)
		{
			for (int i = 0; i < 3; i++)
			{
				if (t.Neighbors[i] == null)
				{
					AdvancingFrontNode advancingFrontNode = this.Front.LocatePoint(t.PointCWFrom(t.Points[i]));
					if (advancingFrontNode != null)
					{
						advancingFrontNode.Triangle = t;
					}
				}
			}
		}

		public override void PrepareTriangulation(Triangulatable t)
		{
			base.PrepareTriangulation(t);
			double x;
			double num = x = this.Points[0].X;
			double y;
			double num2 = y = this.Points[0].Y;
			foreach (TriangulationPoint triangulationPoint in this.Points)
			{
				if (triangulationPoint.X > x)
				{
					x = triangulationPoint.X;
				}
				if (triangulationPoint.X < num)
				{
					num = triangulationPoint.X;
				}
				if (triangulationPoint.Y > y)
				{
					y = triangulationPoint.Y;
				}
				if (triangulationPoint.Y < num2)
				{
					num2 = triangulationPoint.Y;
				}
			}
			double num3 = (double)this.ALPHA * (x - num);
			double num4 = (double)this.ALPHA * (y - num2);
			TriangulationPoint head = new TriangulationPoint(x + num3, num2 - num4, -1);
			TriangulationPoint tail = new TriangulationPoint(num - num3, num2 - num4, -1);
			this.Head = head;
			this.Tail = tail;
			this.Points.Sort(this._comparator);
		}

		public void FinalizeTriangulation()
		{
			base.Triangulatable.AddTriangles(this.Triangles);
			this.Triangles.Clear();
		}

		public override TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b)
		{
			return new DTSweepConstraint(a, b);
		}

		public override TriangulationAlgorithm Algorithm
		{
			get
			{
				return TriangulationAlgorithm.DTSweep;
			}
		}

		private readonly float ALPHA = 0.3f;

		public AdvancingFront Front;

		public DTSweepBasin Basin = new DTSweepBasin();

		public DTSweepEdgeEvent EdgeEvent = new DTSweepEdgeEvent();

		private DTSweepPointComparator _comparator = new DTSweepPointComparator();
	}
}
