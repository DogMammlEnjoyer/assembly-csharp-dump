using System;

namespace UnityEngine.XR.OpenXR.Input
{
	[Obsolete("OpenXR.Input.Pose is deprecated, Please use UnityEngine.InputSystem.XR.PoseState instead", false)]
	public struct Pose
	{
		public bool isTracked { readonly get; set; }

		public InputTrackingState trackingState { readonly get; set; }

		public Vector3 position { readonly get; set; }

		public Quaternion rotation { readonly get; set; }

		public Vector3 velocity { readonly get; set; }

		public Vector3 angularVelocity { readonly get; set; }
	}
}
