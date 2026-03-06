using System;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.BuildingBlocks
{
	public class VisualizeEnvRaycast : MonoBehaviour
	{
		private void Awake()
		{
			this._raycastManager = Object.FindFirstObjectByType<EnvironmentRaycastManager>();
		}

		private void Update()
		{
			this.VisualizeRay();
		}

		private void VisualizeRay()
		{
			if (this._raycastManager == null)
			{
				return;
			}
			Ray raycastRay = this._spaceLocator.GetRaycastRay();
			EnvironmentRaycastHit environmentRaycastHit;
			bool flag = this._raycastManager.Raycast(raycastRay, out environmentRaycastHit, 100f) || environmentRaycastHit.status == EnvironmentRaycastHitStatus.HitPointOccluded;
			bool flag2 = environmentRaycastHit.normalConfidence > 0f;
			this._raycastLine.enabled = flag;
			this._raycastHitPoint.gameObject.SetActive(flag && flag2);
			if (this._raycastLine != null)
			{
				this._raycastLine.SetPosition(0, raycastRay.origin);
				this._raycastLine.SetPosition(1, environmentRaycastHit.point);
			}
			if (this._raycastHitPoint != null && flag2)
			{
				this._raycastHitPoint.SetPositionAndRotation(environmentRaycastHit.point, Quaternion.LookRotation(environmentRaycastHit.normal));
			}
		}

		[SerializeField]
		[Tooltip("Supply a LineRenderer to visualize the raycast ray")]
		private LineRenderer _raycastLine;

		[SerializeField]
		[Tooltip("Supply a Transform to see the ray hit point")]
		private Transform _raycastHitPoint;

		[SerializeField]
		internal SpaceLocator _spaceLocator;

		private EnvironmentRaycastManager _raycastManager;
	}
}
