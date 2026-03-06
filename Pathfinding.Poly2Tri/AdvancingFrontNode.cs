using System;

namespace Pathfinding.Poly2Tri
{
	public class AdvancingFrontNode
	{
		public AdvancingFrontNode(TriangulationPoint point)
		{
			this.Point = point;
			this.Value = point.X;
		}

		public bool HasNext
		{
			get
			{
				return this.Next != null;
			}
		}

		public bool HasPrev
		{
			get
			{
				return this.Prev != null;
			}
		}

		public AdvancingFrontNode Next;

		public AdvancingFrontNode Prev;

		public double Value;

		public TriangulationPoint Point;

		public DelaunayTriangle Triangle;
	}
}
