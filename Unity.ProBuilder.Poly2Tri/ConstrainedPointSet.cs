using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class ConstrainedPointSet : PointSet
	{
		public int[] EdgeIndex { get; private set; }

		public ConstrainedPointSet(List<TriangulationPoint> points, int[] index) : base(points)
		{
			this.EdgeIndex = index;
		}

		public override TriangulationMode TriangulationMode
		{
			get
			{
				return TriangulationMode.Constrained;
			}
		}

		public override void Prepare(TriangulationContext tcx)
		{
			base.Prepare(tcx);
			for (int i = 0; i < this.EdgeIndex.Length; i += 2)
			{
				tcx.NewConstraint(base.Points[this.EdgeIndex[i]], base.Points[this.EdgeIndex[i + 1]]);
			}
		}
	}
}
