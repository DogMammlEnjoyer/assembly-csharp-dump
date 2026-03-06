using System;
using System.Threading.Tasks;
using Modio.Mods;
using Modio.Unity.UI.Components.Localization;
using Modio.Unity.UI.Panels;
using Modio.Unity.UI.Panels.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertySubscriptionToggle : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._mod = mod;
			if (this._text != null)
			{
				this._text.text = (mod.IsSubscribed ? "UNSUBSCRIBE" : "SUBSCRIBE");
			}
			if (this._localisedText != null)
			{
				this._localisedText.SetKey(mod.IsSubscribed ? "modio_btn_unsubscribe" : "modio_btn_subscribe");
			}
			bool flag = mod.IsMonetized && !mod.IsPurchased;
			if (this._purchaseButton != null)
			{
				this._purchaseButton.onClick.RemoveListener(new UnityAction(this.PurchaseButtonClicked));
				this._purchaseButton.gameObject.SetActive(flag);
				this._purchaseButton.onClick.AddListener(new UnityAction(this.PurchaseButtonClicked));
			}
			if (this._subscribeToggle != null)
			{
				this._subscribeToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.SubscribeToggleValueChanged));
				this._subscribeToggle.isOn = mod.IsSubscribed;
				this._subscribeToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SubscribeToggleValueChanged));
				this._subscribeToggle.gameObject.SetActive(!flag);
			}
			if (this._subscribeButton != null)
			{
				this._subscribeButton.onClick.RemoveListener(new UnityAction(this.SubscribeButtonClicked));
				this._subscribeButton.onClick.AddListener(new UnityAction(this.SubscribeButtonClicked));
				this._subscribeButton.gameObject.SetActive(!flag && (this._unsubscribeButton == null || !this._mod.IsSubscribed));
			}
			if (this._unsubscribeButton != null)
			{
				this._unsubscribeButton.onClick.RemoveListener(new UnityAction(this.SubscribeButtonClicked));
				this._unsubscribeButton.onClick.AddListener(new UnityAction(this.SubscribeButtonClicked));
				this._unsubscribeButton.gameObject.SetActive(!flag && this._mod.IsSubscribed);
			}
		}

		private void SubscribeButtonClicked()
		{
			this.UpdateSubscribed(!this._mod.IsSubscribed);
		}

		private void SubscribeToggleValueChanged(bool arg0)
		{
			this.UpdateSubscribed(this._subscribeToggle.isOn);
		}

		private void UpdateSubscribed(bool shouldBeSubscribed)
		{
			if (shouldBeSubscribed && this._mod.Dependencies.HasDependencies)
			{
				if (this._dependenciesAreConfirmed)
				{
					Task<Error> task = this._mod.Subscribe(true);
					ModioErrorPanelGeneric panelOfType = ModioPanelManager.GetPanelOfType<ModioErrorPanelGeneric>();
					if (panelOfType != null)
					{
						panelOfType.MonitorTaskThenOpenPanelIfError(task);
					}
					if (this._subscribeToggle != null)
					{
						this._subscribeToggle.SetIsOnWithoutNotify(this._mod.IsSubscribed);
					}
					return;
				}
				ModDependenciesPanel panelOfType2 = ModioPanelManager.GetPanelOfType<ModDependenciesPanel>();
				if (panelOfType2 != null)
				{
					panelOfType2.IsSubscribeFlow(true);
					panelOfType2.OpenPanel(this._mod);
					if (this._subscribeToggle != null)
					{
						this._subscribeToggle.SetIsOnWithoutNotify(this._mod.IsSubscribed);
					}
					return;
				}
			}
			Task<Error> task2 = shouldBeSubscribed ? this._mod.Subscribe(true) : this._mod.Unsubscribe();
			if (this._subscribeToggle != null)
			{
				this._subscribeToggle.SetIsOnWithoutNotify(this._mod.IsSubscribed);
			}
			ModioErrorPanelGeneric panelOfType3 = ModioPanelManager.GetPanelOfType<ModioErrorPanelGeneric>();
			if (panelOfType3 == null)
			{
				return;
			}
			panelOfType3.MonitorTaskThenOpenPanelIfError(task2);
		}

		private void PurchaseButtonClicked()
		{
			ModioPanelManager.GetPanelOfType<ModioConfirmPurchasePanel>().OpenPanel(this._mod);
		}

		[SerializeField]
		private Button _subscribeButton;

		[SerializeField]
		private Toggle _subscribeToggle;

		[SerializeField]
		private Button _unsubscribeButton;

		[SerializeField]
		private Button _purchaseButton;

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private ModioUILocalizedText _localisedText;

		[SerializeField]
		private bool _dependenciesAreConfirmed;

		private Mod _mod;
	}
}
