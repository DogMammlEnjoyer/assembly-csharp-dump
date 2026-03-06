using System;
using Modio.Mods;
using Modio.Unity.Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyEnabled : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._mod = mod;
			bool flag = mod.File.State == ModFileState.Installed && mod.IsSubscribed;
			ModioComponentUISettings platformSettings = ModioClient.Settings.GetPlatformSettings<ModioComponentUISettings>();
			if (platformSettings == null || !platformSettings.ShowEnableModToggle)
			{
				if (this._showIfInstalledWhenEnabledNotAvailable != null)
				{
					this._showIfInstalledWhenEnabledNotAvailable.SetActive(flag);
				}
				flag = false;
			}
			else if (this._showIfInstalledWhenEnabledNotAvailable != null)
			{
				this._showIfInstalledWhenEnabledNotAvailable.SetActive(false);
			}
			if (this._enabledToggle != null)
			{
				this._enabledToggle.gameObject.SetActive(flag);
				this._enabledToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggleValueChanged));
				this._enabledToggle.isOn = mod.IsEnabled;
				this._enabledToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
			}
			if (this._enableButton != null)
			{
				this._enableButton.onClick.RemoveListener(new UnityAction(this.EnableButtonClicked));
				this._enableButton.onClick.AddListener(new UnityAction(this.EnableButtonClicked));
				this._enableButton.gameObject.SetActive(!this._mod.IsEnabled && flag);
			}
			if (this._disableButton != null)
			{
				this._disableButton.onClick.RemoveListener(new UnityAction(this.DisableButtonClicked));
				this._disableButton.onClick.AddListener(new UnityAction(this.DisableButtonClicked));
				this._disableButton.gameObject.SetActive(this._mod.IsEnabled && flag);
			}
		}

		private void OnToggleValueChanged(bool isEnabled)
		{
			this._mod.SetIsEnabled(isEnabled);
		}

		private void EnableButtonClicked()
		{
			this.OnToggleValueChanged(true);
		}

		private void DisableButtonClicked()
		{
			this.OnToggleValueChanged(false);
		}

		[SerializeField]
		private Toggle _enabledToggle;

		[SerializeField]
		private Button _enableButton;

		[SerializeField]
		private Button _disableButton;

		[SerializeField]
		private GameObject _showIfInstalledWhenEnabledNotAvailable;

		private Mod _mod;
	}
}
