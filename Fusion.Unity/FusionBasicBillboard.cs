using System;
using UnityEngine;

namespace Fusion
{
	[ScriptHelp(BackColor = ScriptHeaderBackColor.Olive)]
	[ExecuteAlways]
	public class FusionBasicBillboard : Behaviour
	{
		private void OnEnable()
		{
			this.UpdateLookAt();
		}

		private void OnDisable()
		{
			base.transform.localRotation = default(Quaternion);
		}

		private Camera MainCamera
		{
			get
			{
				float time = Time.time;
				if (time == FusionBasicBillboard._lastCameraFindTime)
				{
					return FusionBasicBillboard._currentCam;
				}
				FusionBasicBillboard._lastCameraFindTime = time;
				return FusionBasicBillboard._currentCam = Camera.main;
			}
			set
			{
				FusionBasicBillboard._currentCam = value;
			}
		}

		private void LateUpdate()
		{
			this.UpdateLookAt();
		}

		public void UpdateLookAt()
		{
			Camera camera = this.Camera ? this.Camera : this.MainCamera;
			if (camera && base.enabled)
			{
				base.transform.rotation = camera.transform.rotation;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			FusionBasicBillboard._currentCam = null;
			FusionBasicBillboard._lastCameraFindTime = 0f;
		}

		[InlineHelp]
		public Camera Camera;

		private static float _lastCameraFindTime;

		private static Camera _currentCam;
	}
}
