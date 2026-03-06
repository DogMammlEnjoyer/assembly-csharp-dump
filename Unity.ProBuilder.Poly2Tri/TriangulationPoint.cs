using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class TriangulationPoint
	{
		public List<DTSweepConstraint> Edges { get; private set; }

		public TriangulationPoint(double x, double y, int index = -1)
		{
			this.X = x;
			this.Y = y;
			this.Index = index;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[",
				this.X.ToString(),
				",",
				this.Y.ToString(),
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

		public const int INSERTED_INDEX = -1;

		public const int INVALID_INDEX = -2;

		public double X;

		public double Y;

		public int Index;
	}
}
