using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	public class SplinePlanetZone : PlanetZone
	{
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			Vector3 vector;
			float closestEvaluationOnSpline = this.spline.GetClosestEvaluationOnSpline(worldPosition, out vector);
			Vector3 b = this.spline.Evaluate(closestEvaluationOnSpline);
			return worldPosition - b;
		}

		[SerializeField]
		private CatmullRomSpline spline;
	}
}
