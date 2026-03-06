using System;
using Modio.API;
using Modio.Unity.Settings;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUIMonetizationHider : MonoBehaviour
	{
		private void Start()
		{
			ModioClient.OnInitialized += this.OnPluginInitialized;
			ModioAPI.OnOfflineStatusChanged += this.OnOfflineStatusChanged;
		}

		private void OnDestroy()
		{
			ModioClient.OnInitialized -= this.OnPluginInitialized;
			ModioAPI.OnOfflineStatusChanged -= this.OnOfflineStatusChanged;
		}

		private void OnOfflineStatusChanged(bool isOffline)
		{
			this._isOffline = isOffline;
			this.ChangeActiveStateIfNeeded();
		}

		private void OnPluginInitialized()
		{
			ModioSettings modioSettings = ModioServices.Resolve<ModioSettings>();
			ModioComponentUISettings platformSettings = modioSettings.GetPlatformSettings<ModioComponentUISettings>();
			this._isMonetizationDisabled = (platformSettings == null || !platformSettings.ShowMonetizationUI);
			this.ChangeActiveStateIfNeeded();
		}

		private void ChangeActiveStateIfNeeded()
		{
			base.gameObject.SetActive(!this._isOffline && !this._isMonetizationDisabled);
		}

		private bool _isOffline;

		private bool _isMonetizationDisabled;
	}
}
