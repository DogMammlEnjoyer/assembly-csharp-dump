using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class DistantPointDetector
	{
		public DistantPointDetector(DistantPointDetectorFrustums frustums)
		{
			this._frustums = frustums;
		}

		public bool ComputeIsPointing(Collider[] colliders, bool isSelecting, out float bestScore, out Vector3 bestHitPoint)
		{
			ConicalFrustum conicalFrustum = (isSelecting || this._frustums.DeselectionFrustum == null) ? this._frustums.SelectionFrustum : this._frustums.DeselectionFrustum;
			bestHitPoint = Vector3.zero;
			bestScore = float.NegativeInfinity;
			bool result = false;
			foreach (Collider collider in colliders)
			{
				float num = 0f;
				Vector3 vector;
				if (conicalFrustum.HitsCollider(collider, out num, out vector))
				{
					if (this._frustums.AidFrustum != null)
					{
						float num2;
						Vector3 vector2;
						if (!this._frustums.AidFrustum.HitsCollider(collider, out num2, out vector2))
						{
							goto IL_CC;
						}
						num = num * (1f - this._frustums.AidBlending) + num2 * this._frustums.AidBlending;
					}
					if (num > bestScore)
					{
						bestHitPoint = vector;
						bestScore = num;
						result = true;
					}
				}
				IL_CC:;
			}
			return result;
		}

		public bool IsPointingWithoutAid(Collider[] colliders, out Vector3 bestHitPoint)
		{
			if (this._frustums.AidFrustum == null)
			{
				bestHitPoint = Vector3.zero;
				return false;
			}
			return !this.IsPointingAtColliders(colliders, this._frustums.AidFrustum, out bestHitPoint) && this.IsWithinDeselectionRange(colliders);
		}

		public bool IsWithinDeselectionRange(Collider[] colliders)
		{
			return this.IsPointingAtColliders(colliders, this._frustums.DeselectionFrustum) || this.IsPointingAtColliders(colliders, this._frustums.SelectionFrustum);
		}

		private bool IsPointingAtColliders(Collider[] colliders, ConicalFrustum frustum)
		{
			if (frustum == null)
			{
				return false;
			}
			foreach (Collider collider in colliders)
			{
				float num;
				Vector3 vector;
				if (frustum.HitsCollider(collider, out num, out vector))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsPointingAtColliders(Collider[] colliders, ConicalFrustum frustum, out Vector3 bestHitPoint)
		{
			bestHitPoint = Vector3.zero;
			float num = float.NegativeInfinity;
			bool result = false;
			if (frustum == null)
			{
				return false;
			}
			foreach (Collider collider in colliders)
			{
				float num2;
				Vector3 vector;
				if (frustum.HitsCollider(collider, out num2, out vector))
				{
					result = true;
					if (num2 > num)
					{
						num = num2;
						bestHitPoint = vector;
					}
				}
			}
			return result;
		}

		private DistantPointDetectorFrustums _frustums;
	}
}
