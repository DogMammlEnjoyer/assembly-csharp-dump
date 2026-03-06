using System;
using System.Threading.Tasks;
using Modio.Errors;
using Modio.Monetization;
using Modio.Unity.UI.Panels;
using Modio.Unity.UI.Panels.Monetization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components
{
	public class ModioUITokenPack : MonoBehaviour
	{
		public void SetPack(PortalSku sku)
		{
			this._tokenPack = sku;
			if (this._amount != null)
			{
				this._amount.text = this._tokenPack.Value.ToString();
			}
			if (this._price != null)
			{
				this._price.text = sku.FormattedPrice;
			}
			if (this._name != null)
			{
				this._name.text = sku.Name;
			}
			if (this._icon != null)
			{
				this._icon.sprite = this.GetImageForValue(sku.Value);
			}
		}

		public void OnPressedPurchase()
		{
			IModioVirtualCurrencyProviderService modioVirtualCurrencyProviderService;
			if (!ModioServices.TryResolve<IModioVirtualCurrencyProviderService>(out modioVirtualCurrencyProviderService))
			{
				ModioPanelManager.GetPanelOfType<ModioBuyTokensPanel>().ClosePanel();
				return;
			}
			Task<Error> task = modioVirtualCurrencyProviderService.OpenCheckoutFlow(this._tokenPack);
			if (task != null)
			{
				ModioWaitingPanelGeneric panelOfType = ModioPanelManager.GetPanelOfType<ModioWaitingPanelGeneric>();
				if (panelOfType == null)
				{
					return;
				}
				panelOfType.OpenAndWaitFor<Error>(task, delegate(Error error)
				{
					if (error)
					{
						if (error.Code == ErrorCode.OPERATION_CANCELLED)
						{
							return;
						}
						ModioErrorPanelGeneric panelOfType2 = ModioPanelManager.GetPanelOfType<ModioErrorPanelGeneric>();
						if (panelOfType2 != null)
						{
							panelOfType2.OpenPanel(error);
						}
					}
					ModioBuyTokensPanel panelOfType3 = ModioPanelManager.GetPanelOfType<ModioBuyTokensPanel>();
					if (panelOfType3 == null)
					{
						return;
					}
					panelOfType3.ClosePanel();
				});
			}
		}

		private Sprite GetImageForValue(int amount)
		{
			foreach (ModioUITokenPack.ValueImageMap valueImageMap in this._valuesToImages)
			{
				if (valueImageMap.value == amount)
				{
					return valueImageMap.image;
				}
			}
			ModioLog warning = ModioLog.Warning;
			if (warning != null)
			{
				warning.Log(string.Format("No image mapped for token pack value [{0}]!", amount));
			}
			return null;
		}

		[SerializeField]
		private TMP_Text _amount;

		[SerializeField]
		private TMP_Text _price;

		[SerializeField]
		private TMP_Text _name;

		[SerializeField]
		private Image _icon;

		private PortalSku _tokenPack;

		[SerializeField]
		private ModioUITokenPack.ValueImageMap[] _valuesToImages;

		[Serializable]
		private class ValueImageMap
		{
			public int value;

			public Sprite image;
		}
	}
}
