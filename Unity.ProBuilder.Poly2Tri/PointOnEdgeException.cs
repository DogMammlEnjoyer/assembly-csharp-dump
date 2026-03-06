using System;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class PointOnEdgeException : NotImplementedException
	{
		public PointOnEdgeException(string message, TriangulationPoint a, TriangulationPoint b, TriangulationPoint c) : base(message)
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
