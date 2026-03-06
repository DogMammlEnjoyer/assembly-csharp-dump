using System;

namespace Pathfinding.Poly2Tri
{
	public class PointOnEdgeException : NotImplementedException
	{
		public PointOnEdgeException(string message, TriangulationPoint a, TriangulationPoint b, TriangulationPoint c) : base(string.Concat(new string[]
		{
			message,
			"\n",
			a.ToString(),
			"\n",
			b.ToString(),
			"\n",
			c.ToString()
		}))
		{
			this.A = a;
			this.B = b;
			this.C = c;
		}

		public readonly TriangulationPoint A;

		public readonly TriangulationPoint B;

		public readonly TriangulationPoint C;
	}
}
