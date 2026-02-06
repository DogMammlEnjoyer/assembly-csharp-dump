using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class KIDMessagingController : MonoBehaviour
{
	private static string HasShownConfirmationScreenPlayerPref
	{
		get
		{
			return "hasShownKIDConfirmationScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	public void OnConfirmPressed()
	{
		this._closeMessageBox = true;
	}

	private void Awake()
	{
		if (KIDMessagingController.instance != null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] Trying to start a new [KIDMessagingController] but one already exists");
			Object.Destroy(this);
			return;
		}
		KIDMessagingController.instance = this;
	}

	private bool ShouldShowConfirmationScreen()
	{
		return !KIDManager.CurrentSession.IsDefault;
	}

	private Task StartKIDConfirmationScreenInternal(CancellationToken token)
	{
		KIDMessagingController.<StartKIDConfirmationScreenInternal>d__18 <StartKIDConfirmationScreenInternal>d__;
		<StartKIDConfirmationScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDConfirmationScreenInternal>d__.<>4__this = this;
		<StartKIDConfirmationScreenInternal>d__.token = token;
		<StartKIDConfirmationScreenInternal>d__.<>1__state = -1;
		<StartKIDConfirmationScreenInternal>d__.<>t__builder.Start<KIDMessagingController.<StartKIDConfirmationScreenInternal>d__18>(ref <StartKIDConfirmationScreenInternal>d__);
		return <StartKIDConfirmationScreenInternal>d__.<>t__builder.Task;
	}

	public void OnDisable()
	{
		KIDAudioManager kidaudioManager = KIDAudioManager.Instance;
		if (kidaudioManager == null)
		{
			return;
		}
		kidaudioManager.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	public static Task StartKIDConfirmationScreen(CancellationToken token)
	{
		KIDMessagingController.<StartKIDConfirmationScreen>d__20 <StartKIDConfirmationScreen>d__;
		<StartKIDConfirmationScreen>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDConfirmationScreen>d__.token = token;
		<StartKIDConfirmationScreen>d__.<>1__state = -1;
		<StartKIDConfirmationScreen>d__.<>t__builder.Start<KIDMessagingController.<StartKIDConfirmationScreen>d__20>(ref <StartKIDConfirmationScreen>d__);
		return <StartKIDConfirmationScreen>d__.<>t__builder.Task;
	}

	private static Task<string> GetSetupConfirmationMessage()
	{
		KIDMessagingController.<GetSetupConfirmationMessage>d__21 <GetSetupConfirmationMessage>d__;
		<GetSetupConfirmationMessage>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetSetupConfirmationMessage>d__.<>1__state = -1;
		<GetSetupConfirmationMessage>d__.<>t__builder.Start<KIDMessagingController.<GetSetupConfirmationMessage>d__21>(ref <GetSetupConfirmationMessage>d__);
		return <GetSetupConfirmationMessage>d__.<>t__builder.Task;
	}

	private static string GetConfirmMessageFromTitleDataJson(string jsonTxt)
	{
		if (string.IsNullOrEmpty(jsonTxt))
		{
			Debug.LogError("[KID_MANAGER] Cannot get Confirmation Message. JSON is null or empty!");
			return null;
		}
		KIDMessagingTitleData kidmessagingTitleData = JsonConvert.DeserializeObject<KIDMessagingTitleData>(jsonTxt);
		if (kidmessagingTitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		if (string.IsNullOrEmpty(kidmessagingTitleData.KIDSetupConfirmation))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDMessagingTitleData] - [KIDSetupConfirmation] is null or empty. Json: \n" + jsonTxt);
			return null;
		}
		return kidmessagingTitleData.KIDSetupConfirmation;
	}

	public static void ShowConnectionErrorScreen()
	{
		if (KIDMessagingController.instance == null || KIDMessagingController.instance.messageBox == null)
		{
			Debug.LogError("[KID::MESSAGING_CONTROLLER] No message box");
			return;
		}
		KIDMessagingController.instance._closeMessageBox = false;
		KIDMessagingController.instance.messageBox.Header = "Connection Error";
		KIDMessagingController.instance.messageBox.Body = "Unable to connect to the internet. Please restart the game and try again.";
		KIDMessagingController.instance.messageBox.RightButton = "Quit";
		KIDMessagingController.instance.messageBox.ShowQuitButtonAsPrimary();
		KIDMessagingController.instance.messageBox.RightButtonCallback.RemoveAllListeners();
		KIDMessagingController.instance.messageBox.RightButtonCallback.AddListener(new UnityAction(Application.Quit));
		KIDMessagingController.instance.messageBox.gameObject.SetActive(true);
		HandRayController.Instance.EnableHandRays();
		PrivateUIRoom.AddUI(KIDMessagingController.instance.transform);
	}

	private const string SHOWN_CONFIRMATION_SCREEN_PREFIX = "hasShownKIDConfirmationScreen-";

	private const string CONFIRMATION_HEADER = "Thank you";

	private const string CONFIRMATION_BODY = "k-ID setup is now complete. Thanks and have fun in Gorilla World!";

	private const string CONFIRMATION_BUTTON = "Continue";

	private const string KID_SETUP_CONFIRMATION_TITLE_KEY = "KID_SETUP_CONFIRMATION_TITLE";

	private const string KID_SETUP_CONFIRMATION_BODY_KEY = "KID_SETUP_CONFIRMATION_BODY";

	private const string KID_SETUP_CONFIRMATION_BUTTON_KEY = "KID_SETUP_CONFIRMATION_BUTTON";

	private static KIDMessagingController instance;

	[SerializeField]
	private MessageBox messageBox;

	private const string CONNECTION_ERROR_HEADER = "Connection Error";

	private const string CONNECTION_ERROR_BODY = "Unable to connect to the internet. Please restart the game and try again.";

	private const string CONNECTION_ERROR_BUTTON = "Quit";

	private bool _closeMessageBox;
}
