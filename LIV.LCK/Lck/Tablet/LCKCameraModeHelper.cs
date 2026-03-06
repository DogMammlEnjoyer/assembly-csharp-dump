using System;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public class LCKCameraModeHelper : MonoBehaviour
	{
		public void SetCameraMode(bool isSelected)
		{
			if (!isSelected)
			{
				return;
			}
			this._settingsButtonsController.SwitchCameraModes(this._cameraMode);
		}

		[SerializeField]
		private CameraMode _cameraMode;

		[SerializeField]
		private LCKSettingsButtonsController _settingsButtonsController;
	}
}
