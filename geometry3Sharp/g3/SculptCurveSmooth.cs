using System;

namespace g3
{
	public class SculptCurveSmooth : StandardSculptCurveDeformation
	{
		public SculptCurveSmooth()
		{
			this.DeformF = null;
			this.SmoothAlpha = 0.10000000149011612;
			this.SmoothIterations = 3;
		}
	}
}
