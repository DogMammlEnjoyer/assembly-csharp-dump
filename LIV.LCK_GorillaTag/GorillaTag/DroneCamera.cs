using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class DroneCamera
	{
		public DroneCamera(Camera camera)
		{
			this._camera = camera;
		}

		public void SetFov(float fov)
		{
			this._targetFov = fov;
		}

		public void SetSmoothness(float smoothness)
		{
			this._smoothness = smoothness;
		}

		public void Run()
		{
			if (!Mathf.Approximately(this._camera.fieldOfView, this._targetFov))
			{
				this._camera.fieldOfView = Mathf.Lerp(this._camera.fieldOfView, this._targetFov, Time.deltaTime / this._smoothness);
			}
		}

		private Camera _camera;

		private float _targetFov;

		private float _smoothness;
	}
}
