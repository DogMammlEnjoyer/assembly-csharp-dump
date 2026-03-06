using System;

namespace Pathfinding.Poly2Tri
{
	public static class P2T
	{
		public static void Triangulate(PolygonSet ps)
		{
			TriangulationContext triangulationContext = P2T.CreateContext(P2T._defaultAlgorithm);
			foreach (Polygon t in ps.Polygons)
			{
				triangulationContext.PrepareTriangulation(t);
				P2T.Triangulate(triangulationContext);
				triangulationContext.Clear();
			}
		}

		public static void Triangulate(Polygon p)
		{
			P2T.Triangulate(P2T._defaultAlgorithm, p);
		}

		public static void Triangulate(ConstrainedPointSet cps)
		{
			P2T.Triangulate(P2T._defaultAlgorithm, cps);
		}

		public static void Triangulate(PointSet ps)
		{
			P2T.Triangulate(P2T._defaultAlgorithm, ps);
		}

		public static TriangulationContext CreateContext(TriangulationAlgorithm algorithm)
		{
			if (algorithm != TriangulationAlgorithm.DTSweep)
			{
			}
			return new DTSweepContext();
		}

		public static void Triangulate(TriangulationAlgorithm algorithm, Triangulatable t)
		{
			TriangulationContext triangulationContext = P2T.CreateContext(algorithm);
			triangulationContext.PrepareTriangulation(t);
			P2T.Triangulate(triangulationContext);
		}

		public static void Triangulate(TriangulationContext tcx)
		{
			TriangulationAlgorithm algorithm = tcx.Algorithm;
			if (algorithm != TriangulationAlgorithm.DTSweep)
			{
			}
			DTSweep.Triangulate((DTSweepContext)tcx);
		}

		public static void Warmup()
		{
		}

		private static TriangulationAlgorithm _defaultAlgorithm;
	}
}
