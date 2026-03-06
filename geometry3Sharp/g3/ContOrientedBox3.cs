using System;
using System.Collections.Generic;

namespace g3
{
	public class ContOrientedBox3
	{
		public ContOrientedBox3(IEnumerable<Vector3d> points)
		{
			GaussPointsFit3 gaussPointsFit = new GaussPointsFit3(points);
			if (!gaussPointsFit.ResultValid)
			{
				return;
			}
			this.Box = gaussPointsFit.Box;
			this.Box.Contain(points);
		}

		public ContOrientedBox3(IEnumerable<Vector3d> points, IEnumerable<double> pointWeights)
		{
			GaussPointsFit3 gaussPointsFit = new GaussPointsFit3(points, pointWeights);
			if (!gaussPointsFit.ResultValid)
			{
				return;
			}
			this.Box = gaussPointsFit.Box;
			this.Box.Contain(points);
		}

		public Box3d Box;

		public bool ResultValid;
	}
}
