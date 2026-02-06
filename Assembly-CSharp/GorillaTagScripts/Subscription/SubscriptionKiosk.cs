using System;
using System.Collections.Generic;
using GorillaNetworking;
using Oculus.Platform;
using Oculus.Platform.Models;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Subscription
{
	public class SubscriptionKiosk : MonoBehaviour, ITouchScreenStation, IGorillaSliceableSimple
	{
		public SIScreenRegion ScreenRegion { get; }

		private void Awake()
		{
			this.toggleButtonContainers = new List<SITouchscreenButtonContainer>(base.GetComponentsInChildren<SITouchscreenButtonContainer>(true));
			for (int i = this.toggleButtonContainers.Count - 1; i >= 0; i--)
			{
				if (this.toggleButtonContainers[i].button.buttonMode != SITouchscreenButton.ButtonMode.Toggle)
				{
					this.toggleButtonContainers.RemoveAt(i);
				}
			}
			this.screensByState = new Dictionary<SubscriptionKiosk.ScreenState, GameObject>();
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SafeAccount, this.safeAccountScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.WaitingForScan, this.waitingForScanScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.Scanning, this.scanningScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown, this.subStatusUnknownScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.MainMenuSubscribed, this.mainMenuSubscribedScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.MainMenuUnsubscribed, this.mainMenuUnsubscribedScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionData, this.subDataScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.PurchaseSubscription, this.purchaseSubScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress, this.purchaseProgressScreen);
			this.screensByState.Add(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult, this.purchaseResultScreen);
		}

		private void OnEnable()
		{
			this.steamComingSoon.SetActive(true);
			this.waitingForScanScreen.SetActive(false);
			Object.Destroy(this);
		}

		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.LocalSubscriptionDataUpdated));
		}

		public void HandScanAborted()
		{
			if (this.currentState == SubscriptionKiosk.ScreenState.Scanning)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.WaitingForScan);
			}
		}

		public void KioskAbandoned()
		{
			this.UpdateState(SubscriptionKiosk.ScreenState.WaitingForScan);
		}

		public void HandScanStarted()
		{
			if (this.currentState == SubscriptionKiosk.ScreenState.WaitingForScan)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.Scanning);
			}
		}

		public void HandScanned()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				return;
			}
			SubscriptionManager.SubscriptionStatus subscriptionStatus = SubscriptionManager.LocalSubscriptionStatus();
			if (subscriptionStatus == SubscriptionManager.SubscriptionStatus.Active)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuSubscribed);
				return;
			}
			if (subscriptionStatus == SubscriptionManager.SubscriptionStatus.Inactive)
			{
				this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuUnsubscribed);
				return;
			}
			this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown);
		}

		private void UpdateState(SubscriptionKiosk.ScreenState newState)
		{
			this.lastState = this.currentState;
			this.currentState = newState;
			if (this.lastState == this.currentState)
			{
				return;
			}
			this.ActivateScreen(this.currentState);
			switch (this.currentState)
			{
			case SubscriptionKiosk.ScreenState.WaitingForScan:
			case SubscriptionKiosk.ScreenState.Scanning:
			case SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown:
			case SubscriptionKiosk.ScreenState.PurchaseSubscription:
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress:
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult:
				break;
			case SubscriptionKiosk.ScreenState.MainMenuSubscribed:
				this.UpdateSubscribedMenu();
				return;
			case SubscriptionKiosk.ScreenState.MainMenuUnsubscribed:
				this.UpdateUnsubscribedMenu();
				return;
			case SubscriptionKiosk.ScreenState.SubscriptionData:
				this.UpdateSubscriptionData();
				break;
			default:
				return;
			}
		}

		private void ActivateScreen(SubscriptionKiosk.ScreenState activeScreen)
		{
			foreach (KeyValuePair<SubscriptionKiosk.ScreenState, GameObject> keyValuePair in this.screensByState)
			{
				keyValuePair.Value.SetActive(keyValuePair.Key == activeScreen);
			}
		}

		public void AddButton(SITouchscreenButton button, bool isPopupButton = false)
		{
		}

		public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr)
		{
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			switch (this.currentState)
			{
			case SubscriptionKiosk.ScreenState.MainMenuSubscribed:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Help)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionData);
					return;
				}
				break;
			case SubscriptionKiosk.ScreenState.MainMenuUnsubscribed:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.PurchaseSubscription);
					return;
				}
				break;
			case SubscriptionKiosk.ScreenState.SubscriptionData:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Cancel)
				{
					Debug.Log("yeet");
					return;
				}
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
				{
					this.HandScanned();
					return;
				}
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.PurchaseSubscription);
					return;
				}
				break;
			case SubscriptionKiosk.ScreenState.PurchaseSubscription:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Subscribe)
				{
					this.PurchaseSubscription((SubscriptionManager.SubscriptionTerm)data);
					return;
				}
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Back)
				{
					this.HandScanned();
					return;
				}
				break;
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress:
				break;
			case SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult:
				if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Confirm)
				{
					this.HandScanned();
				}
				break;
			default:
				return;
			}
		}

		public void TouchscreenToggleButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, bool isToggledOn)
		{
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			if (this.currentState != SubscriptionKiosk.ScreenState.MainMenuSubscribed)
			{
				return;
			}
			if (buttonType == SITouchscreenButton.SITouchscreenButtonType.Select)
			{
				this.HandleSubscriptionToggle(data, isToggledOn);
			}
		}

		private void HandleSubscriptionToggle(int buttonData, bool requestedState)
		{
			if (!SubscriptionManager.IsLocalSubscribed())
			{
				return;
			}
			bool state = this.UpdateSubscriptionFeatureState(buttonData, requestedState);
			this.UpdateToggleButtonState(buttonData, state);
		}

		private void UpdateToggleButtonState(int buttonData, bool state)
		{
			foreach (SITouchscreenButtonContainer sitouchscreenButtonContainer in this.toggleButtonContainers)
			{
				if (sitouchscreenButtonContainer.button.data == buttonData)
				{
					sitouchscreenButtonContainer.button.SetToggleState(state, true);
					break;
				}
			}
		}

		private bool UpdateSubscriptionFeatureState(int buttonData, bool newState)
		{
			if (buttonData != 0)
			{
				if (buttonData == 1)
				{
					this.UpdateExperimentalFeature(newState);
				}
			}
			else
			{
				this.UpdateGoldNameTag(newState);
			}
			Debug.Log(string.Format("Updating subscription kiosk {0} to state: {1}", buttonData, newState));
			return newState;
		}

		private bool GetSubscriptionFeatureState(int buttonData)
		{
			if (buttonData == 0)
			{
				return SubscriptionManager.GetSubscriptionSettingValue("SMKEYPREFIXGOLDEN_NAME_KEY") >= 1;
			}
			if (buttonData != 1)
			{
				Debug.Log(string.Format("Getting current state for subscription kiosk {0}", buttonData));
				return false;
			}
			return SubscriptionManager.GetSubscriptionSettingValue("SMKEYPREFIXIOBT_ENABLE_KEY") >= 1;
		}

		private void UpdateGoldNameTag(bool state)
		{
			SubscriptionManager.SetSubscriptionSettingValue("SMKEYPREFIXGOLDEN_NAME_KEY", state ? 1 : 0);
			VRRig.LocalRig.OnSubscriptionData();
			if (GorillaScoreboardTotalUpdater.instance != null)
			{
				GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
			}
		}

		private void UpdateExperimentalFeature(bool state)
		{
			SubscriptionManager.SetSubscriptionSettingValue("SMKEYPREFIXIOBT_ENABLE_KEY", state ? 1 : 0);
			if (GorillaIK.playerIK != null)
			{
				GorillaIK.playerIK.ResetIKData();
				GorillaIK.playerIK.usingUpdatedIK = state;
			}
		}

		private void UpdateSubscribedMenu()
		{
			this.subMenuPlayerName.text = NetworkSystem.Instance.LocalPlayer.SanitizedNickName;
			this.subMenuDaysAccrued.text = SubscriptionManager.GetSubscriptionDetails().daysAccrued.ToString();
			foreach (SITouchscreenButtonContainer sitouchscreenButtonContainer in this.toggleButtonContainers)
			{
				this.UpdateToggleButtonState(sitouchscreenButtonContainer.data, this.GetSubscriptionFeatureState(sitouchscreenButtonContainer.data));
			}
		}

		private void UpdateUnsubscribedMenu()
		{
			this.unsubscribedMenuPlayerName.text = NetworkSystem.Instance.LocalPlayer.SanitizedNickName;
		}

		private void UpdateSubscriptionData()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails();
			this.subDataPlayerName.text = NetworkSystem.Instance.LocalPlayer.SanitizedNickName;
			this.subDataDaysAccrued.text = subscriptionDetails.daysAccrued.ToString();
			this.subDataDaysRemaining.text = Mathf.RoundToInt((float)(subscriptionDetails.subscriptionActiveUntilDate - DateTime.UtcNow).TotalDays).ToString();
			this.subDataAutoRenew.text = (subscriptionDetails.autoRenew ? "ENABLED" : "DISABLED");
			this.subDataRenewDate.text = subscriptionDetails.subscriptionActiveUntilDate.ToString("MMM d, yyyy").ToUpper();
			this.subDataSubscriptionTerm.text = subscriptionDetails.autoRenewMonths.ToString() + " MONTH" + ((subscriptionDetails.autoRenewMonths > 1) ? "S" : "");
			if (this.subDataSubscribeButton.activeSelf == subscriptionDetails.autoRenew)
			{
				this.subDataSubscribeButton.SetActive(!subscriptionDetails.autoRenew);
			}
		}

		private void UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult result)
		{
			this.lastPurchase = result;
			string text = "";
			if (result == SubscriptionKiosk.PurchaseResult.Success)
			{
				text = "SUBSCRIPTION SUCCESSFUL! WELCOME TO THE FAN CLUB, YOU ARE NOW A VERY IMPORTANT MONKE (V.I.M.)!";
				LocalisationManager.TryGetKeyForCurrentLocale("SUBKIOSKPURCHASE_SUCCESS", out text, text);
			}
			else if (result == SubscriptionKiosk.PurchaseResult.Failure)
			{
				text = "PURCHASE FAILED! WE'RE NOT SURE WHAT HAPPENED, BUT PLEASE CHECK YOUR INFORMATION, OR TRY AGAIN LATER. IF IT LOOKED LIKE THE PURCHASE SHOULD HAVE SUCCEEDED, TRY RESTARTING THE GAME.";
				LocalisationManager.TryGetKeyForCurrentLocale("SUBKIOSKPURCHASE_FAIL", out text, text);
			}
			else if (result == SubscriptionKiosk.PurchaseResult.Cancel)
			{
				text = "PURCHASE CANCELED! WE'LL BE HERE IF YOU CHANGE YOUR MIND!";
				LocalisationManager.TryGetKeyForCurrentLocale("SUBKIOSKPURCHASE_CANCEL", out text, text);
			}
			this.purchaseResultText.text = text;
		}

		private void PurchaseSubscription(SubscriptionManager.SubscriptionTerm subTerm)
		{
			IAP.LaunchCheckoutFlow("fan_club:SUBSCRIPTION__" + subTerm.ToString()).OnComplete(new Message<Purchase>.Callback(this.LaunchCheckoutFlowCallback));
			this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseInProgress);
		}

		public void LaunchCheckoutFlowCallback(Message<Purchase> msg)
		{
			Debug.Log("SubscriptionKiosk Purchase result: " + msg.Data.ToString());
			if (msg.IsError)
			{
				Error error = msg.GetError();
				if (error != null && error.Message != null && error.Message.Contains("cancel"))
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Cancel);
					return;
				}
				this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
			}
			else
			{
				Purchase purchase = msg.GetPurchase();
				if (purchase != null && purchase.Sku != "" && purchase.Sku != null)
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Success);
				}
				else
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Failure);
				}
			}
			SubscriptionManager.InitializePersonalSubscriptionData();
			this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionPurchaseResult);
		}

		public void LocalSubscriptionDataUpdated()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.LocalSubscriptionDetails();
			if (subscriptionDetails.active)
			{
				if (this.lastPurchase == SubscriptionKiosk.PurchaseResult.Failure)
				{
					this.UpdatePurchaseResultScreen(SubscriptionKiosk.PurchaseResult.Success);
				}
				if (this.currentState == SubscriptionKiosk.ScreenState.MainMenuUnsubscribed)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.MainMenuSubscribed);
				}
				if (this.currentState == SubscriptionKiosk.ScreenState.PurchaseSubscription && subscriptionDetails.autoRenew)
				{
					this.UpdateState(SubscriptionKiosk.ScreenState.SubscriptionData);
				}
			}
		}

		public void SliceUpdate()
		{
			if (this.currentState == SubscriptionKiosk.ScreenState.SubscriptionStatusUnknown)
			{
				this.HandScanned();
			}
		}

		GameObject ITouchScreenStation.get_gameObject()
		{
			return base.gameObject;
		}

		private const string SUBSCRIPTION_KIOSK_PREFIX = "SUBKIOSK";

		private const string PURCHASE_SUCCESS_KEY = "SUBKIOSKPURCHASE_SUCCESS";

		private const string PURCHASE_CANCEL_KEY = "SUBKIOSKPURCHASE_CANCEL";

		private const string PURCHASE_FAIL_KEY = "SUBKIOSKPURCHASE_FAIL";

		private const string subSKU = "fan_club";

		[SerializeField]
		private GameObject steamComingSoon;

		[SerializeField]
		private GameObject safeAccountScreen;

		[SerializeField]
		private GameObject waitingForScanScreen;

		[SerializeField]
		private GameObject scanningScreen;

		[SerializeField]
		private GameObject subStatusUnknownScreen;

		[SerializeField]
		private GameObject mainMenuSubscribedScreen;

		[SerializeField]
		private GameObject mainMenuUnsubscribedScreen;

		[SerializeField]
		private GameObject subDataScreen;

		[SerializeField]
		private GameObject purchaseSubScreen;

		[SerializeField]
		private GameObject purchaseProgressScreen;

		[SerializeField]
		private GameObject purchaseResultScreen;

		private List<SITouchscreenButtonContainer> toggleButtonContainers;

		private Dictionary<SubscriptionKiosk.ScreenState, GameObject> screensByState;

		[SerializeField]
		private TextMeshPro subMenuPlayerName;

		[SerializeField]
		private TextMeshPro subMenuDaysAccrued;

		[SerializeField]
		private TextMeshPro unsubscribedMenuPlayerName;

		[SerializeField]
		private TextMeshPro subDataPlayerName;

		[SerializeField]
		private TextMeshPro subDataDaysAccrued;

		[SerializeField]
		private TextMeshPro subDataDaysRemaining;

		[SerializeField]
		private TextMeshPro subDataAutoRenew;

		[SerializeField]
		private TextMeshPro subDataRenewDate;

		[SerializeField]
		private TextMeshPro subDataSubscriptionTerm;

		[SerializeField]
		private GameObject subDataSubscribeButton;

		[SerializeField]
		private TextMeshPro purchaseResultText;

		private SubscriptionKiosk.ScreenState currentState = SubscriptionKiosk.ScreenState.WaitingForScan;

		private SubscriptionKiosk.ScreenState lastState;

		private SubscriptionKiosk.PurchaseResult lastPurchase;

		private enum ScreenState
		{
			SafeAccount,
			WaitingForScan,
			Scanning,
			SubscriptionStatusUnknown,
			MainMenuSubscribed,
			MainMenuUnsubscribed,
			SubscriptionData,
			PurchaseSubscription,
			SubscriptionPurchaseInProgress,
			SubscriptionPurchaseResult,
			None
		}

		private enum PurchaseResult
		{
			Success,
			Failure,
			Cancel
		}
	}
}
