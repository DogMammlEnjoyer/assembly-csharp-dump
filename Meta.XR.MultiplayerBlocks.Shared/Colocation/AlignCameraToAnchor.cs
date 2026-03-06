using System;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Colocation
{
	[DefaultExecutionOrder(10)]
	internal class AlignCameraToAnchor : MonoBehaviour
	{
		public OVRSpatialAnchor CameraAlignmentAnchor { get; set; }

		private void Update()
		{
			this.RealignToAnchor();
		}

		public void RealignToAnchor()
		{
			this.Align(this.CameraAlignmentAnchor.transform);
		}

		private void Align(Transform anchorTransform)
		{
			Vector3 localScale = anchorTransform.localScale;
			anchorTransform.localScale = Vector3.one;
			OVRPose ovrpose = anchorTransform.ToTrackingSpacePose(Camera.main);
			anchorTransform.SetPositionAndRotation(ovrpose.position, ovrpose.orientation);
			base.transform.position = anchorTransform.InverseTransformPoint(Vector3.zero);
			base.transform.eulerAngles = new Vector3(0f, -anchorTransform.eulerAngles.y, 0f);
			OVRPose ovrpose2 = ovrpose.ToWorldSpacePose(Camera.main);
			anchorTransform.SetPositionAndRotation(ovrpose2.position, ovrpose2.orientation);
			anchorTransform.localScale = localScale;
		}
	}
}
