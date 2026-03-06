using System;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.BuildingBlocks
{
	public class PlaceWithAnchor : MonoBehaviour
	{
		private void Awake()
		{
			this._spatialAnchorTransform = new GameObject("[" + base.gameObject.name + "] Spatial Anchor").transform;
			if (this.Target == null)
			{
				this.Target = base.transform;
			}
		}

		public void RequestMove(Pose pose)
		{
			this._requestMove = true;
			this._surfacePose = pose;
		}

		private void Update()
		{
			if (this._requestMove && this._surfacePose != default(Pose))
			{
				this.SetTargetWithAnchor(this._surfacePose);
				this._requestMove = false;
			}
		}

		public void OnLocateSpace(Pose surfacePose, bool success)
		{
			if (!success)
			{
				Debug.Log("[PlaceWithAnchor] Failed to locate space.");
				return;
			}
			this.RequestMove(surfacePose);
		}

		private void SetTargetWithAnchor(Pose pose)
		{
			this.EraseAnchor();
			this.Target.SetParent(null);
			this.Target.SetPositionAndRotation(pose.position, pose.rotation);
			this.SetAnchor();
		}

		private void EraseAnchor()
		{
			if (this._spatialAnchorTransform.TryGetComponent<OVRSpatialAnchor>(out this._spatialAnchor))
			{
				this._spatialAnchor.EraseAnchorAsync();
				Object.DestroyImmediate(this._spatialAnchor);
			}
		}

		private void SetAnchor()
		{
			this._spatialAnchorTransform.transform.SetPositionAndRotation(base.transform.position, base.transform.rotation);
			this._spatialAnchor = this._spatialAnchorTransform.gameObject.AddComponent<OVRSpatialAnchor>();
			this.Target.SetParent(this._spatialAnchorTransform.transform);
		}

		[Tooltip("Target transform to place")]
		public Transform Target;

		private Transform _spatialAnchorTransform;

		private OVRSpatialAnchor _spatialAnchor;

		private bool _requestMove;

		private Pose _surfacePose;
	}
}
