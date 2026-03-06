using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Liv.Lck.Tablet
{
	public class LCKSettingsButtonsController : MonoBehaviour
	{
		public Action<CameraMode> OnCameraModeChanged { get; set; }

		private void Awake()
		{
			this._settingsDictionary = new Dictionary<CameraMode, GameObject>
			{
				{
					CameraMode.Selfie,
					this._selfieSettings
				},
				{
					CameraMode.FirstPerson,
					this._firstPersonSettings
				},
				{
					CameraMode.ThirdPerson,
					this._thirdPersonSettings
				}
			};
		}

		private void OnEnable()
		{
			this._selfieToggle.group = this._toggleGroup;
			this._firstPersonToggle.group = this._toggleGroup;
			this._thirdPersonToggle.group = this._toggleGroup;
		}

		public void SwitchCameraModes(CameraMode mode)
		{
			foreach (KeyValuePair<CameraMode, GameObject> keyValuePair in this._settingsDictionary)
			{
				if (keyValuePair.Key.Equals(mode))
				{
					keyValuePair.Value.SetActive(true);
					Action<CameraMode> onCameraModeChanged = this.OnCameraModeChanged;
					if (onCameraModeChanged != null)
					{
						onCameraModeChanged(mode);
					}
				}
				else
				{
					keyValuePair.Value.SetActive(false);
				}
			}
		}

		[Header("Camera Mode Settings Groups")]
		[SerializeField]
		private GameObject _selfieSettings;

		[SerializeField]
		private GameObject _firstPersonSettings;

		[SerializeField]
		private GameObject _thirdPersonSettings;

		[Header("Toggle References")]
		[SerializeField]
		private ToggleGroup _toggleGroup;

		[SerializeField]
		private Toggle _selfieToggle;

		[SerializeField]
		private Toggle _firstPersonToggle;

		[SerializeField]
		private Toggle _thirdPersonToggle;

		private Dictionary<CameraMode, GameObject> _settingsDictionary;
	}
}
