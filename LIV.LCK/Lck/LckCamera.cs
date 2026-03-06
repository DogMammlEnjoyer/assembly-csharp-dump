using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Liv.Lck
{
	[RequireComponent(typeof(Camera))]
	public class LckCamera : MonoBehaviour, ILckCamera
	{
		public string CameraId
		{
			get
			{
				return this._cameraId;
			}
		}

		private void Awake()
		{
			if (string.IsNullOrEmpty(this._cameraId))
			{
				this._cameraId = Guid.NewGuid().ToString();
			}
			this._camera.enabled = false;
			LckMediator.RegisterCamera(this);
			LckLog.Log("Configuring URP camera data for camera: " + this._cameraId);
			UniversalAdditionalCameraData universalAdditionalCameraData;
			if (!this._camera.TryGetComponent<UniversalAdditionalCameraData>(out universalAdditionalCameraData))
			{
				universalAdditionalCameraData = this._camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
			}
			universalAdditionalCameraData.allowXRRendering = false;
		}

		private void OnDestroy()
		{
			LckMediator.UnregisterCamera(this);
		}

		public void ActivateCamera(RenderTexture renderTexture)
		{
			this._camera.enabled = true;
			this._camera.targetTexture = renderTexture;
		}

		public void DeactivateCamera()
		{
			this._camera.enabled = false;
			this._camera.targetTexture = null;
		}

		public Camera GetCameraComponent()
		{
			return this._camera;
		}

		[SerializeField]
		private Camera _camera;

		[SerializeField]
		private string _cameraId;
	}
}
