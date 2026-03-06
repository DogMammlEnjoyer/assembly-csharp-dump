using System;

namespace g3
{
	public class SculptCurveMove : StandardSculptCurveDeformation
	{
		public SculptCurveMove()
		{
			this.SmoothAlpha = 0.0;
			this.SmoothIterations = 0;
		}

		public override SculptCurveDeformation.DeformInfo Apply(Frame3f vNextPos)
		{
			if ((vNextPos.Origin - this.vPreviousPos.Origin).Length < 9.999999747378752E-05)
			{
				return new SculptCurveDeformation.DeformInfo
				{
					bNoChange = true,
					maxEdgeLenSqr = 0.0,
					minEdgeLenSqr = double.MaxValue
				};
			}
			this.DeformF = delegate(int idx, double t)
			{
				Vector3d v = this.vPreviousPos.ToFrameP(this.Curve[idx]);
				Vector3d b = vNextPos.FromFrameP(v);
				return Vector3d.Lerp(this.Curve[idx], b, t);
			};
			return base.Apply(vNextPos);
		}
	}
}
