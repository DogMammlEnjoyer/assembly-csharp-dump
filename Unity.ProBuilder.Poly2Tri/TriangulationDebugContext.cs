using System;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal abstract class TriangulationDebugContext
	{
		public TriangulationDebugContext(TriangulationContext tcx)
		{
			this._tcx = tcx;
		}

		public abstract void Clear();

		protected TriangulationContext _tcx;
	}
}
