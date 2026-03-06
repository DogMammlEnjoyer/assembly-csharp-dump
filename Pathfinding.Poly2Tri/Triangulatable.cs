using System;
using System.Collections.Generic;

namespace Pathfinding.Poly2Tri
{
	public interface Triangulatable
	{
		void Prepare(TriangulationContext tcx);

		IList<TriangulationPoint> Points { get; }

		IList<DelaunayTriangle> Triangles { get; }

		void AddTriangle(DelaunayTriangle t);

		void AddTriangles(IEnumerable<DelaunayTriangle> list);

		void ClearTriangles();

		TriangulationMode TriangulationMode { get; }
	}
}
