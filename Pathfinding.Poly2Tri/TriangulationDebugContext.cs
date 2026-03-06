using System;

namespace Pathfinding.Poly2Tri
{
	public abstract class TriangulationDebugContext
	{
		public TriangulationDebugContext(TriangulationContext tcx)
		{
			this._tcx = tcx;
		}

		public abstract void Clear();

		protected TriangulationContext _tcx;
	}
}
