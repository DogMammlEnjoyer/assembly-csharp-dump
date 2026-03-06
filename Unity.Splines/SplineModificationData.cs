using System;

namespace UnityEngine.Splines
{
	internal struct SplineModificationData
	{
		public SplineModificationData(Spline spline, SplineModification modification, int knotIndex, float prevCurveLength, float nextCurveLength)
		{
			this.Spline = spline;
			this.Modification = modification;
			this.KnotIndex = knotIndex;
			this.PrevCurveLength = prevCurveLength;
			this.NextCurveLength = nextCurveLength;
		}

		public readonly Spline Spline;

		public readonly SplineModification Modification;

		public readonly int KnotIndex;

		public readonly float PrevCurveLength;

		public readonly float NextCurveLength;
	}
}
