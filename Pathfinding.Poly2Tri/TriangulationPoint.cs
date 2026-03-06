using System;
using System.Collections.Generic;

namespace Pathfinding.Poly2Tri
{
	public class TriangulationPoint
	{
		public TriangulationPoint(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}

		public List<DTSweepConstraint> Edges { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[",
				this.X,
				",",
				this.Y,
				"]"
			});
		}

		public float Xf
		{
			get
			{
				return (float)this.X;
			}
			set
			{
				this.X = (double)value;
			}
		}

		public float Yf
		{
			get
			{
				return (float)this.Y;
			}
			set
			{
				this.Y = (double)value;
			}
		}

		public void AddEdge(DTSweepConstraint e)
		{
			if (this.Edges == null)
			{
				this.Edges = new List<DTSweepConstraint>();
			}
			this.Edges.Add(e);
		}

		public bool HasEdges
		{
			get
			{
				return this.Edges != null;
			}
		}

		public double X;

		public double Y;
	}
}
