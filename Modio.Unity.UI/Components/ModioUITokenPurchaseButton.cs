using System;
using System.Threading.Tasks;
using Modio.Monetization;
using Modio.Unity.UI.Input;
using Modio.Unity.UI.Panels;
using Modio.Unity.UI.Panels.Monetization;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUITokenPurchaseButton : MonoBehaviour
	{
		private void Awake()
		{
			this._panel = base.GetComponentInParent<ModioPanelBase>();
		}

		private void OnEnable()
		{
			if (this._panel != null)
			{
				this._panel.OnHasFocusChanged += this.OnHasFocusChanged;
				return;
			}
			this.OnHasFocusChanged(true);
		}

		private void OnDisable()
		{
			if (this._panel != null)
			{
				this._panel.OnHasFocusChanged -= this.OnHasFocusChanged;
			}
			this.OnHasFocusChanged(false);
		}

		private void OnHasFocusChanged(bool panelHasFocus)
		{
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.BuyTokens, new Action(this.OpenTokens));
			if (panelHasFocus)
			{
				ModioUIInput.AddHandler(ModioUIInput.ModioAction.BuyTokens, new Action(this.OpenTokens));
			}
		}

		private void OpenTokens()
		{
			if (ModioClient.AuthService == null)
			{
				ModioLog error = ModioLog.Error;
				if (error == null)
				{
					return;
				}
				error.Log("No IModioAuthService is bound! Cannot auth");
				return;
			}
			else
			{
				IModioVirtualCurrencyProviderService modioVirtualCurrencyProviderService;
				if (ModioServices.TryResolve<IModioVirtualCurrencyProviderService>(out modioVirtualCurrencyProviderService))
				{
					ModioBuyTokensPanel panelOfType = ModioPanelManager.GetPanelOfType<ModioBuyTokensPanel>();
					if (panelOfType != null)
					{
						panelOfType.OpenPanel();
						return;
					}
				}
				IModioStorefrontService modioStorefrontService;
				if (ModioServices.TryResolve<IModioStorefrontService>(out modioStorefrontService))
				{
					Task<Error> task = modioStorefrontService.OpenPlatformPurchaseFlow();
					if (task != null)
					{
						ModioWaitingPanelGeneric panelOfType2 = ModioPanelManager.GetPanelOfType<ModioWaitingPanelGeneric>();
						if (panelOfType2 == null)
						{
							return;
						}
						panelOfType2.OpenAndWaitFor<Error>(task, new Action<Error>(this.PlatformPurchaseFlowCompleted));
					}
					return;
				}
				ModioLog error2 = ModioLog.Error;
				if (error2 == null)
				{
					return;
				}
				error2.Log("No IModioStorefrontService found, unable to open store front.");
				return;
			}
		}

		private void PlatformPurchaseFlowCompleted(Error error)
		{
			if (error)
			{
				ModioErrorPanelGeneric panelOfType = ModioPanelManager.GetPanelOfType<ModioErrorPanelGeneric>();
				if (panelOfType == null)
				{
					return;
				}
				panelOfType.OpenPanel(error);
			}
		}

		private ModioPanelBase _panel;
	}
}
