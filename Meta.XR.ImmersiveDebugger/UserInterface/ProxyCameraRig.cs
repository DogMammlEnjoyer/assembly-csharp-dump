using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class ProxyCameraRig
	{
		public Camera Camera { get; private set; }

		public Transform CameraTransform { get; private set; }

		public bool Refresh()
		{
			if (this.Camera != null && this.Camera.isActiveAndEnabled)
			{
				return true;
			}
			this.SearchForCamera();
			return this.Camera;
		}

		private void SearchForCamera()
		{
			if (RuntimeSettings.Instance.UseCustomIntegrationConfig)
			{
				this.Camera = CustomIntegrationConfig.GetCamera();
				Camera camera = this.Camera;
				this.CameraTransform = ((camera != null) ? camera.gameObject.transform : null);
				return;
			}
			if (this._cameraRig == null)
			{
				this._cameraRig = Object.FindAnyObjectByType<OVRCameraRig>();
			}
			if (this._cameraRig != null)
			{
				this.Camera = this._cameraRig.leftEyeCamera;
				this.CameraTransform = this._cameraRig.leftEyeAnchor;
			}
		}

		private OVRCameraRig _cameraRig;
	}
}
