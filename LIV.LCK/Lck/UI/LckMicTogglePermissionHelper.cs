using System;
using Liv.Lck.Settings;
using Liv.Lck.Tablet;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Liv.Lck.UI
{
	public class LckMicTogglePermissionHelper : MonoBehaviour
	{
		private void Start()
		{
			if (Application.platform != RuntimePlatform.Android || Application.isEditor || LckSettings.Instance.MicPermissionType == LckSettings.MicPermissionAskType.NeverAskFromLck)
			{
				this._controller.ToggleMicrophoneRecording(true);
				return;
			}
			if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
			{
				if (!Permission.HasUserAuthorizedPermission("android.permission.RECORD_AUDIO"))
				{
					this._hasMicPermission = false;
					this.SetMicPermissionOffVisuals();
					this._controller.ToggleMicrophoneRecording(false);
					switch (LckSettings.Instance.MicPermissionType)
					{
					case LckSettings.MicPermissionAskType.OnAppStartup:
						this._micLckToggle.SetDisabledState(false);
						return;
					case LckSettings.MicPermissionAskType.OnTabletSpawn:
						this._micLckToggle.SetDisabledState(false);
						if (!this.UserSelectedDontShowAgain())
						{
							this.CheckForMicPermission(true);
							return;
						}
						break;
					case LckSettings.MicPermissionAskType.OnMicUnmute:
						this._micToggle.onValueChanged.AddListener(new UnityAction<bool>(this.CheckForMicPermission));
						return;
					default:
						return;
					}
				}
				else
				{
					this._controller.ToggleMicrophoneRecording(true);
				}
			}
		}

		private void CheckForMicPermission(bool toggleValue = true)
		{
			if (Application.platform == RuntimePlatform.Android && !Application.isEditor && !Permission.HasUserAuthorizedPermission("android.permission.RECORD_AUDIO"))
			{
				if (this.UserSelectedDontShowAgain())
				{
					if (this._micToggle && LckSettings.Instance.MicPermissionType == LckSettings.MicPermissionAskType.OnMicUnmute)
					{
						this._micLckToggle.SetDisabledState(false);
						LCKCameraController.ColliderButtonsInUse = false;
						this._micToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.CheckForMicPermission));
					}
					return;
				}
				LckMicTogglePermissionHelper._permissionAskCount++;
				PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
				permissionCallbacks.PermissionGranted += this.PermissionCallbacks_PermissionGranted;
				permissionCallbacks.PermissionDenied += this.PermissionCallbacks_PermissionDenied;
				LckLog.Log("Requesting Microphone Permission", "CheckForMicPermission", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Tablet\\LckMicTogglePermissionHelper.cs", 96);
				Permission.RequestUserPermission("android.permission.RECORD_AUDIO", permissionCallbacks);
			}
		}

		internal void PermissionCallbacks_PermissionGranted(string permissionName)
		{
			this._hasMicPermission = true;
			LckLog.Log("Microphone Permission Granted", "PermissionCallbacks_PermissionGranted", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Tablet\\LckMicTogglePermissionHelper.cs", 105);
			this.SetMicPermissionOnVisuals();
			this._controller.ToggleMicrophoneRecording(true);
			this._micLckToggle.RestoreToggleState();
			if (this._micToggle && LckSettings.Instance.MicPermissionType == LckSettings.MicPermissionAskType.OnMicUnmute)
			{
				this._micToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.CheckForMicPermission));
			}
		}

		internal void PermissionCallbacks_PermissionDenied(string permissionName)
		{
			this._hasMicPermission = false;
			LckLog.Log("Microphone Permission Denied", "PermissionCallbacks_PermissionDenied", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Tablet\\LckMicTogglePermissionHelper.cs", 120);
			this.SetMicPermissionOffVisuals();
			this._controller.ToggleMicrophoneRecording(false);
			if (LckSettings.Instance.MicPermissionType != LckSettings.MicPermissionAskType.OnMicUnmute)
			{
				this._micLckToggle.SetDisabledState(false);
			}
		}

		private bool UserSelectedDontShowAgain()
		{
			return LckMicTogglePermissionHelper._permissionAskCount >= 1 && !Permission.ShouldShowRequestPermissionRationale("android.permission.RECORD_AUDIO");
		}

		public void SetMicPermissionOnVisuals()
		{
			this._micLckToggle.RestoreDefaultColors();
			this._micLckToggle.RestoreDefaultIcons();
			this._micLckToggle.SetToggleVisualsOn();
			this.SetToggleIconAlpha(1f);
		}

		public void SetMicPermissionOffVisuals()
		{
			this._micLckToggle.SetCustomColors(this._noMicPermissionColors, this._noMicPermissionColors);
			this._micLckToggle.SetCustomIcons(this._noMicPermissionIcon, this._noMicPermissionIcon);
			this.SetToggleIconAlpha(0.2f);
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus && LckSettings.Instance.MicPermissionType != LckSettings.MicPermissionAskType.NeverAskFromLck)
			{
				if (!this._hasMicPermission && Permission.HasUserAuthorizedPermission("android.permission.RECORD_AUDIO"))
				{
					LckLog.Log("User allowed mic permission from settings", "OnApplicationFocus", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Tablet\\LckMicTogglePermissionHelper.cs", 161);
					this._controller.ToggleMicrophoneRecording(true);
					this.SetMicPermissionOnVisuals();
					this._micLckToggle.RestoreToggleState();
					this._hasMicPermission = true;
					return;
				}
				if (this._hasMicPermission && !Permission.HasUserAuthorizedPermission("android.permission.RECORD_AUDIO"))
				{
					LckLog.Log("User disabled mic permission from settings", "OnApplicationFocus", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Tablet\\LckMicTogglePermissionHelper.cs", 169);
					this._controller.ToggleMicrophoneRecording(false);
					this.SetMicPermissionOffVisuals();
					this._micLckToggle.SetDisabledState(false);
					this._hasMicPermission = false;
				}
			}
		}

		private void SetToggleIconAlpha(float alpha)
		{
			Color color = this._micToggleIcon.color;
			color.a = alpha;
			this._micToggleIcon.color = color;
		}

		[SerializeField]
		private LCKCameraController _controller;

		[SerializeField]
		private LckButtonColors _noMicPermissionColors;

		[SerializeField]
		private Sprite _noMicPermissionIcon;

		[SerializeField]
		private LckToggle _micLckToggle;

		[SerializeField]
		private Toggle _micToggle;

		[SerializeField]
		private Image _micToggleIcon;

		private static int _permissionAskCount;

		private bool _hasMicPermission = true;
	}
}
