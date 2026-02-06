using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GorillaTagScripts.GhostReactor
{
	public class GRKiosk : MonoBehaviour
	{
		private void Start()
		{
			GRKiosk.<Start>d__16 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<GRKiosk.<Start>d__16>(ref <Start>d__);
		}

		private void ProcessPurchaseItemState(GRKiosk.ButtonSide? button, HashSet<GRKiosk.PurchaseState> recentStates = null)
		{
			if (recentStates == null)
			{
				recentStates = new HashSet<GRKiosk.PurchaseState>();
			}
			recentStates.Add(this._purchaseState);
			switch (this._purchaseState)
			{
			case GRKiosk.PurchaseState.Initialize:
				throw new Exception("ProcessPurchaseItemState called in non-initialized GRKiosk!");
			case GRKiosk.PurchaseState.AlreadyOwned:
				this.ResetButtons();
				break;
			case GRKiosk.PurchaseState.AvailableForPurchase:
				this.SetAvailableForPurchaseDisplays(button);
				break;
			case GRKiosk.PurchaseState.CheckoutPressed:
				this.SetCheckoutConfirmationDisplays(button);
				break;
			case GRKiosk.PurchaseState.CheckoutConfirmation:
				this.ConfirmCheckout(button);
				break;
			}
			if (!recentStates.Contains(this._purchaseState))
			{
				this.ProcessPurchaseItemState(null, recentStates);
			}
			this.FormattedPurchaseText();
		}

		private bool PlayerOwnsItem()
		{
			return CosmeticsController.instance.unlockedCosmetics.Any(new Func<CosmeticsController.CosmeticItem, bool>(this.MatchesCosmeticForPurchase));
		}

		private void OnGetCurrency()
		{
			this.ProcessPurchaseItemState(null, null);
		}

		private void ResetButtons()
		{
			this.LeftPurchaseButton.myTmpText.text = "-";
			this.RightPurchaseButton.myTmpText.text = "-";
			this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.pressedMaterial;
			this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.pressedMaterial;
		}

		private void SetAvailableForPurchaseDisplays(GRKiosk.ButtonSide? button)
		{
			if (this._cosmeticForPurchase.cost <= CosmeticsController.instance.currencyBalance)
			{
				string text;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CANCEL", out text, "NO!");
				string text2;
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_WANT_TO_BUY_CONFIRM", out text2, "YES!");
				this.LeftPurchaseButton.myTmpText.text = text;
				this.RightPurchaseButton.myTmpText.text = text2;
				this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.unpressedMaterial;
				this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.unpressedMaterial;
				GRKiosk.ButtonSide? buttonSide = button;
				GRKiosk.ButtonSide buttonSide2 = GRKiosk.ButtonSide.Right;
				if (buttonSide.GetValueOrDefault() == buttonSide2 & buttonSide != null)
				{
					this._purchaseState = GRKiosk.PurchaseState.CheckoutPressed;
					return;
				}
			}
			else
			{
				this.LeftPurchaseButton.myTmpText.text = "-";
				this.RightPurchaseButton.myTmpText.text = "-";
				this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.pressedMaterial;
				this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.pressedMaterial;
				this._purchaseState = GRKiosk.PurchaseState.AvailableForPurchase;
			}
		}

		private void SetCheckoutConfirmationDisplays(GRKiosk.ButtonSide? button)
		{
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CANCEL", out text, "LET ME THINK ABOUT IT");
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_PURCHASE_BUTTON_CONFIRMATION_CONFIRM", out text2, "YES! I NEED IT!");
			this.LeftPurchaseButton.myTmpText.text = text2;
			this.RightPurchaseButton.myTmpText.text = text;
			this.LeftPurchaseButton.buttonRenderer.material = this.LeftPurchaseButton.unpressedMaterial;
			this.RightPurchaseButton.buttonRenderer.material = this.RightPurchaseButton.unpressedMaterial;
			this._purchaseState = GRKiosk.PurchaseState.CheckoutConfirmation;
		}

		private void ConfirmCheckout(GRKiosk.ButtonSide? button)
		{
			GRKiosk.ButtonSide? buttonSide = button;
			GRKiosk.ButtonSide buttonSide2 = GRKiosk.ButtonSide.Left;
			if (buttonSide.GetValueOrDefault() == buttonSide2 & buttonSide != null)
			{
				this.PurchaseItem();
				return;
			}
			buttonSide = button;
			buttonSide2 = GRKiosk.ButtonSide.Right;
			if (buttonSide.GetValueOrDefault() == buttonSide2 & buttonSide != null)
			{
				this._purchaseState = GRKiosk.PurchaseState.AvailableForPurchase;
			}
		}

		private void PurchaseItem()
		{
			PurchaseItemRequest purchaseItemRequest = new PurchaseItemRequest();
			purchaseItemRequest.ItemId = this._cosmeticForPurchase.itemName;
			purchaseItemRequest.Price = this._cosmeticForPurchase.cost;
			purchaseItemRequest.VirtualCurrency = "SR";
			PlayFabClientAPI.PurchaseItem(purchaseItemRequest, delegate(PurchaseItemResult result)
			{
				this._purchaseState = ((result.Items.Count > 0) ? GRKiosk.PurchaseState.AlreadyOwned : GRKiosk.PurchaseState.AvailableForPurchase);
				if (this._purchaseParticles != null)
				{
					this._purchaseParticles.Play();
				}
				GorillaTagger.Instance.offlineVRRig.AddCosmetic(this._cosmeticForPurchase.itemName);
				this.ProcessPurchaseItemState(null, null);
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.ToString());
			}, null, null);
		}

		private bool MatchesCosmeticForPurchase(CosmeticsController.CosmeticItem item)
		{
			return this.CosmeticNameForPurchase == item.displayName || this.CosmeticNameForPurchase == item.overrideDisplayName || this.CosmeticNameForPurchase == item.itemName;
		}

		private void OnLeftPurchaseButtonPressed(GorillaPressableButton button, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(new GRKiosk.ButtonSide?(GRKiosk.ButtonSide.Left), null);
		}

		private void OnRightPurchaseButtonPressed(GorillaPressableButton button, bool isLeftHand)
		{
			this.ProcessPurchaseItemState(new GRKiosk.ButtonSide?(GRKiosk.ButtonSide.Right), null);
		}

		private void FormattedPurchaseText()
		{
			if (this._itemNameVar == null || this._itemCostVar == null || this._currencyBalanceVar == null)
			{
				Debug.LogError("[LOCALIZATION::GRKIOSK] One of the dynamic variables is NULL and cannot update the [PurchaseText] screen");
				return;
			}
			this._itemNameVar.Value = this._cosmeticForPurchase.displayName.ToUpper();
			this._itemCostVar.Value = this._cosmeticForPurchase.cost;
			this._currencyBalanceVar.Value = CosmeticsController.instance.currencyBalance;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ITEM: ").Append(this._cosmeticForPurchase.overrideDisplayName.ToUpper());
			stringBuilder.Append("\nITEM COST: ").Append(this._cosmeticForPurchase.cost);
			stringBuilder.Append("\nYOU HAVE: ").Append(CosmeticsController.instance.currencyBalance);
			StringBuilder stringBuilder2 = stringBuilder.Append("\n");
			string value;
			switch (this._purchaseState)
			{
			case GRKiosk.PurchaseState.AlreadyOwned:
				value = "YOU ALREADY OWN THIS!";
				break;
			case GRKiosk.PurchaseState.AvailableForPurchase:
				value = "PURCHASE?";
				break;
			case GRKiosk.PurchaseState.CheckoutPressed:
				value = "CONFIRM PURCHASE?";
				break;
			case GRKiosk.PurchaseState.CheckoutConfirmation:
				value = "CONFIRMING PURCHASE...";
				break;
			default:
				value = "ERROR";
				break;
			}
			stringBuilder2.Append(value);
			this.PurchaseText.text = stringBuilder.ToString();
		}

		[SerializeField]
		public string CosmeticNameForPurchase;

		[SerializeField]
		public GorillaPressableButton LeftPurchaseButton;

		[SerializeField]
		public GorillaPressableButton RightPurchaseButton;

		[SerializeField]
		public TMP_Text PurchaseText;

		private CosmeticsController.CosmeticItem _cosmeticForPurchase;

		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private AudioClip _purchaseAudioClip;

		[SerializeField]
		private ParticleSystem _purchaseParticles;

		[SerializeField]
		private LocalizedText _purchaseTextLoc;

		private LocalizedString _purchaseTextLocStr;

		private StringVariable _itemNameVar;

		private IntVariable _itemCostVar;

		private IntVariable _currencyBalanceVar;

		private GRKiosk.PurchaseState _purchaseState;

		private enum PurchaseState
		{
			Initialize,
			AlreadyOwned,
			AvailableForPurchase,
			CheckoutPressed,
			CheckoutConfirmation
		}

		private enum ButtonSide
		{
			Left,
			Right
		}
	}
}
