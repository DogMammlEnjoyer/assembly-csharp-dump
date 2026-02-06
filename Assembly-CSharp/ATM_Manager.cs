using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using GorillaNetworking.Store;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ATM_Manager : MonoBehaviour, IBuildValidation
{
	public ATM_Manager.ATMStages CurrentATMStage
	{
		get
		{
			return this.currentATMStage;
		}
	}

	public void Awake()
	{
		if (ATM_Manager.instance)
		{
			Object.Destroy(this);
		}
		else
		{
			ATM_Manager.instance = this;
		}
		string defaultResult = "CREATOR CODE: ";
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE", out text, defaultResult))
		{
			Debug.LogError("[LOCALIZATION::ATM_MANAGER] Failed to get key for [ATM_CREATOR_CODE]");
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = text;
		}
		this.SwitchToStage(ATM_Manager.ATMStages.Unavailable);
		this.smallDisplays = new List<CreatorCodeSmallDisplay>();
		this.ATM_TERMINAL_ID = string.Empty;
		for (int i = 0; i < this.nexusGroups.Length; i++)
		{
			string atm_TERMINAL_ID = this.ATM_TERMINAL_ID;
			NexusGroupId nexusGroupId = this.nexusGroups[i];
			this.ATM_TERMINAL_ID = atm_TERMINAL_ID + ((nexusGroupId != null) ? nexusGroupId.ToString() : null);
		}
		this.HookupToCreatorCodes();
	}

	public void Start()
	{
		Debug.Log("ATM COUNT: " + this.atmUIs.Count.ToString());
		Debug.Log("SMALL DISPLAY COUNT: " + this.smallDisplays.Count.ToString());
		GameEvents.OnGorrillaATMKeyButtonPressedEvent.AddListener(new UnityAction<GorillaATMKeyBindings>(this.PressButton));
	}

	public void HookupToCreatorCodes()
	{
		CreatorCodes.InitializedEvent += this.CreatorCodesInitialized;
		CreatorCodes.OnCreatorCodeChangedEvent += this.OnCreatorCodeChanged;
		CreatorCodes.OnCreatorCodeFailureEvent += this.OnOnCreatorCodeFailureEvent;
		if (CreatorCodes.Intialized)
		{
			this.CreatorCodesInitialized();
		}
	}

	public void CreatorCodesInitialized()
	{
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeField.text = CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID);
		}
	}

	public void OnCreatorCodeChanged(string id)
	{
		if (id != this.ATM_TERMINAL_ID)
		{
			return;
		}
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID));
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeField.text = CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID);
		}
		string text = "CREATOR CODE:";
		CreatorCodes.CreatorCodeStatus currentCreatorCodeStatus = CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID);
		if (currentCreatorCodeStatus != CreatorCodes.CreatorCodeStatus.Validating)
		{
			if (currentCreatorCodeStatus == CreatorCodes.CreatorCodeStatus.Valid)
			{
				text += " VALID";
			}
		}
		else
		{
			text += " VALIDATING";
		}
		foreach (ATM_UI atm_UI2 in this.atmUIs)
		{
			atm_UI2.creatorCodeTitle.text = text;
		}
	}

	private void OnOnCreatorCodeFailureEvent(string id)
	{
		if (id != this.ATM_TERMINAL_ID)
		{
			return;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: INVALID";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE_INVALID", out text, atm_UI.atmText.text);
			atm_UI.creatorCodeTitle.text = text;
		}
		Debug.Log("ATM CODE FAILURE");
	}

	public void OnCreatorCodeInvalid(string id)
	{
		if (id != this.ATM_TERMINAL_ID)
		{
			return;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}
	}

	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this.SwitchToStage(this.currentATMStage);
	}

	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	private void OnLanguageChanged()
	{
		this.SwitchToStage(this.currentATMStage);
	}

	public void PressButton(GorillaATMKeyBindings buttonPressed)
	{
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm && CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID) != CreatorCodes.CreatorCodeStatus.Validating)
		{
			string defaultResult = "CREATOR CODE: ";
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("ATM_CREATOR_CODE", out text, defaultResult);
			foreach (ATM_UI atm_UI in this.atmUIs)
			{
				atm_UI.creatorCodeTitle.text = text;
			}
			if (buttonPressed == GorillaATMKeyBindings.delete)
			{
				CreatorCodes.DeleteCharacter(this.ATM_TERMINAL_ID);
				return;
			}
			string atm_TERMINAL_ID = this.ATM_TERMINAL_ID;
			string input;
			if (buttonPressed >= GorillaATMKeyBindings.delete)
			{
				input = buttonPressed.ToString();
			}
			else
			{
				int num = (int)buttonPressed;
				input = num.ToString();
			}
			CreatorCodes.AppendKey(atm_TERMINAL_ID, input);
		}
	}

	public void ProcessATMState(string currencyButton)
	{
		ATM_Manager.<ProcessATMState>d__55 <ProcessATMState>d__;
		<ProcessATMState>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ProcessATMState>d__.<>4__this = this;
		<ProcessATMState>d__.currencyButton = currencyButton;
		<ProcessATMState>d__.<>1__state = -1;
		<ProcessATMState>d__.<>t__builder.Start<ATM_Manager.<ProcessATMState>d__55>(ref <ProcessATMState>d__);
	}

	public void AddATM(ATM_UI newATM)
	{
		this.atmUIs.Add(newATM);
		newATM.creatorCodeField.text = CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID);
		this.SwitchToStage(this.currentATMStage);
	}

	public void RemoveATM(ATM_UI atmToRemove)
	{
		this.atmUIs.Remove(atmToRemove);
	}

	public void CreatorCodeValidating()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}
	}

	public void CreatorCodeValid()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm)
		{
			this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
		}
	}

	public void SwitchToStage(ATM_Manager.ATMStages newStage)
	{
		this.currentATMStage = newStage;
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			if (atm_UI.atmText)
			{
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				switch (newStage)
				{
				case ATM_Manager.ATMStages.Unavailable:
					atm_UI.atmText.text = "ATM NOT AVAILABLE! PLEASE TRY AGAIN LATER!";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_NOT_AVAILABLE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Begin:
					atm_UI.atmText.text = "WELCOME! PRESS ANY BUTTON TO BEGIN.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_STARTUP", out text, atm_UI.atmText.text);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_BEGIN", out text5, "BEGIN");
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = text5;
					atm_UI.ATM_RightColumnArrowText[3].enabled = true;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Menu:
					if (PlayFabAuthenticator.instance.GetSafety())
					{
						atm_UI.atmText.text = "CHECK YOUR BALANCE.";
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_CHECK_YOUR_BALANCE", out text, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_BALANCE", out text2, atm_UI.atmText.text);
						atm_UI.atmText.text = text;
						atm_UI.ATM_RightColumnButtonText[0].text = text2;
						atm_UI.ATM_RightColumnArrowText[0].enabled = true;
						atm_UI.ATM_RightColumnButtonText[1].text = "";
						atm_UI.ATM_RightColumnArrowText[1].enabled = false;
						atm_UI.ATM_RightColumnButtonText[2].text = "";
						atm_UI.ATM_RightColumnArrowText[2].enabled = false;
						atm_UI.ATM_RightColumnButtonText[3].text = "";
						atm_UI.ATM_RightColumnArrowText[3].enabled = false;
						atm_UI.creatorCodeObject.SetActive(false);
					}
					else
					{
						atm_UI.atmText.text = "CHECK YOUR BALANCE OR PURCHASE MORE SHINY ROCKS.";
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_MAIN_SCREEN", out text, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_BALANCE", out text2, atm_UI.atmText.text);
						LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE", out text3, atm_UI.atmText.text);
						atm_UI.atmText.text = text;
						atm_UI.ATM_RightColumnButtonText[0].text = text2;
						atm_UI.ATM_RightColumnArrowText[0].enabled = true;
						atm_UI.ATM_RightColumnButtonText[1].text = text3;
						atm_UI.ATM_RightColumnArrowText[1].enabled = true;
						atm_UI.ATM_RightColumnButtonText[2].text = "";
						atm_UI.ATM_RightColumnArrowText[2].enabled = false;
						atm_UI.ATM_RightColumnButtonText[3].text = "";
						atm_UI.ATM_RightColumnArrowText[3].enabled = false;
						atm_UI.creatorCodeObject.SetActive(false);
					}
					break;
				case ATM_Manager.ATMStages.Balance:
					atm_UI.atmText.text = "CURRENT BALANCE:\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CURRENT_BALANCE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text + "\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Choose:
				{
					string defaultResult = "{numShinyRocksToBuy} - {currencySymbol}{shinyRocksCost}";
					string defaultResult2 = "{numShinyRocksToBuy} - {currencySymbol}{shinyRocksCost}\r\n({discount}% BONUS!";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_FIRST", out text2, defaultResult);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text3, defaultResult2);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text4, defaultResult2);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_OPTION_SECOND", out text5, defaultResult2);
					text2 = text2.Replace("{numShinyRocksToBuy}", "1000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "4.99");
					text3 = text3.Replace("{numShinyRocksToBuy}", "2200").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "9.99").Replace("{discount}", "10");
					text4 = text4.Replace("{numShinyRocksToBuy}", "5000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "19.99").Replace("{discount}", "25");
					text5 = text5.Replace("{numShinyRocksToBuy}", "11000").Replace("{currencySymbol}", "$").Replace("{shinyRocksCost}", "39.99").Replace("{discount}", "37");
					atm_UI.atmText.text = "CHOOSE AN AMOUNT OF SHINY ROCKS TO PURCHASE.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CHOOSE_PURCHASE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = text2;
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = text3;
					atm_UI.ATM_RightColumnArrowText[1].enabled = true;
					atm_UI.ATM_RightColumnButtonText[2].text = text4;
					atm_UI.ATM_RightColumnArrowText[2].enabled = true;
					atm_UI.ATM_RightColumnButtonText[3].text = text5;
					atm_UI.ATM_RightColumnArrowText[3].enabled = true;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				}
				case ATM_Manager.ATMStages.Confirm:
					atm_UI.atmText.text = string.Concat(new string[]
					{
						"YOU HAVE CHOSEN TO PURCHASE ",
						this.numShinyRocksToBuy.ToString(),
						" SHINY ROCKS FOR $",
						this.shinyRocksCost.ToString(),
						". CONFIRM TO LAUNCH A STEAM WINDOW TO COMPLETE YOUR PURCHASE."
					});
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_CONFIRMATION_STEAM", out text, atm_UI.atmText.text);
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_CONFIRM", out text2, "CONFIRM");
					text = text.Replace("{numShinyRocksToBuy}", this.numShinyRocksToBuy.ToString());
					text = text.Replace("{currencySymbol}", "$");
					text = text.Replace("{shinyRocksCost}", this.shinyRocksCost.ToString());
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = text2;
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(true);
					break;
				case ATM_Manager.ATMStages.Purchasing:
					atm_UI.atmText.text = "PURCHASING IN STEAM...";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASING", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Success:
					atm_UI.atmText.text = "SUCCESS! NEW SHINY ROCKS BALANCE: " + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_SUCCESS_NEW_BALANCE", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
					if (CreatorCodes.getCurrentCreatorCodeStatus(this.ATM_TERMINAL_ID) == CreatorCodes.CreatorCodeStatus.Valid)
					{
						string name = CreatorCodes.supportedMember.name;
						if (!string.IsNullOrEmpty(name))
						{
							TMP_Text atmText = atm_UI.atmText;
							atmText.text = atmText.text + "\n\nTHIS PURCHASE SUPPORTED\n" + name + "!";
							foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
							{
								creatorCodeSmallDisplay.SuccessfulPurchase(name);
							}
						}
					}
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.Failure:
					atm_UI.atmText.text = "PURCHASE CANCELLED. NO FUNDS WERE SPENT.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASE_CANCELLED", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				case ATM_Manager.ATMStages.SafeAccount:
					atm_UI.atmText.text = "Out Of Order.";
					LocalisationManager.TryGetKeyForCurrentLocale("ATM_PURCHASING_DISABLED_OUT_OF_ORDER", out text, atm_UI.atmText.text);
					atm_UI.atmText.text = text;
					atm_UI.ATM_RightColumnButtonText[0].text = "";
					atm_UI.ATM_RightColumnArrowText[0].enabled = false;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
					break;
				}
			}
		}
	}

	public void SetATMText(string newText)
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.atmText.text = newText;
		}
	}

	public void PressCurrencyPurchaseButton(string currencyPurchaseSize)
	{
		this.ProcessATMState(currencyPurchaseSize);
	}

	public void LeaveSystemMenu()
	{
	}

	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.nexusGroups.Length == 0)
		{
			Debug.LogError("You have to set at least one nexusGroup in " + base.name + " or things will not work!");
			return false;
		}
		return true;
	}

	internal void SetTemporaryCreatorCode(string code)
	{
		if (code == null)
		{
			CreatorCodes.ResetCreatorCode(this.ATM_TERMINAL_ID);
			CreatorCodes.AppendKey(this.ATM_TERMINAL_ID, this._tempCreatorCodeOveride);
			this._tempCreatorCodeOveride = null;
			return;
		}
		if (this._tempCreatorCodeOveride == null)
		{
			this._tempCreatorCodeOveride = CreatorCodes.getCurrentCreatorCode(this.ATM_TERMINAL_ID);
		}
		CreatorCodes.ResetCreatorCode(this.ATM_TERMINAL_ID);
		CreatorCodes.AppendKey(this.ATM_TERMINAL_ID, code);
	}

	private const string ATM_STARTUP_KEY = "ATM_STARTUP";

	private const string ATM_SCREEN_KEY = "ATM_SCREEN";

	private const string ATM_NOT_AVAILABLE_KEY = "ATM_NOT_AVAILABLE";

	private const string ATM_BEGIN_KEY = "ATM_BEGIN";

	private const string ATM_MAIN_SCREEN_KEY = "ATM_MAIN_SCREEN";

	private const string ATM_CHECK_YOUR_BALANCE_KEY = "ATM_CHECK_YOUR_BALANCE";

	private const string ATM_PURCHASING_DISABLED_OUT_OF_ORDER_KEY = "ATM_PURCHASING_DISABLED_OUT_OF_ORDER";

	private const string ATM_CURRENT_BALANCE_KEY = "ATM_CURRENT_BALANCE";

	private const string ATM_MODDED_CLIENT_KEY = "ATM_MODDED_CLIENT";

	private const string ATM_CHOOSE_PURCHASE_KEY = "ATM_CHOOSE_PURCHASE";

	private const string ATM_PURCHASE_CONFIRMATION_KEY = "ATM_PURCHASE_CONFIRMATION";

	private const string ATM_PURCHASE_CONFIRMATION_STEAM_KEY = "ATM_PURCHASE_CONFIRMATION_STEAM";

	private const string ATM_PURCHASING_KEY = "ATM_PURCHASING";

	private const string ATM_SUCCESS_NEW_BALANCE_KEY = "ATM_SUCCESS_NEW_BALANCE";

	private const string ATM_PURCHASE_CANCELLED_KEY = "ATM_PURCHASE_CANCELLED";

	private const string ATM_LOCKED_KEY = "ATM_LOCKED";

	private const string ATM_RETURN_KEY = "ATM_RETURN";

	private const string ATM_BACK_KEY = "ATM_BACK";

	private const string ATM_CONFIRM_KEY = "ATM_CONFIRM";

	private const string ATM_IAP_NOT_AVAILABLE_KEY = "ATM_IAP_NOT_AVAILABLE";

	private const string ATM_BALANCE_KEY = "ATM_BALANCE";

	private const string ATM_PURCHASE_KEY = "ATM_PURCHASE";

	private const string ATM_CREATOR_CODE_KEY = "ATM_CREATOR_CODE";

	private const string ATM_CREATOR_CODE_VALIDATING_KEY = "ATM_CREATOR_CODE_VALIDATING";

	private const string ATM_CREATOR_CODE_VALID_KEY = "ATM_CREATOR_CODE_VALID";

	private const string ATM_CREATOR_CODE_INVALID_KEY = "ATM_CREATOR_CODE_INVALID";

	private const string ATM_PURCHASE_OPTION_FIRST_KEY = "ATM_PURCHASE_OPTION_FIRST";

	private const string ATM_PURCHASE_OPTION_SECOND_KEY = "ATM_PURCHASE_OPTION_SECOND";

	private const string ATM_PURCHASE_OPTION_THIRD_KEY = "ATM_PURCHASE_OPTION_THIRD";

	private const string ATM_PURCHASE_OPTION_FOURTH_KEY = "ATM_PURCHASE_OPTION_FOURTH";

	[OnEnterPlay_SetNull]
	public static volatile ATM_Manager instance;

	private const int MAX_CODE_LENGTH = 10;

	public List<ATM_UI> atmUIs = new List<ATM_UI>();

	[HideInInspector]
	public List<CreatorCodeSmallDisplay> smallDisplays;

	private ATM_Manager.ATMStages currentATMStage;

	public int numShinyRocksToBuy;

	public float shinyRocksCost;

	public bool alreadyBegan;

	[SerializeField]
	private NexusGroupId[] nexusGroups;

	private string _tempCreatorCodeOveride;

	private string ATM_TERMINAL_ID = "atm_terminal_id";

	public enum ATMStages
	{
		Unavailable,
		Begin,
		Menu,
		Balance,
		Choose,
		Confirm,
		Purchasing,
		Success,
		Failure,
		SafeAccount
	}
}
