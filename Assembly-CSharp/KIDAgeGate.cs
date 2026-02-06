using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class KIDAgeGate : MonoBehaviour
{
	public static int UserAge
	{
		get
		{
			return KIDAgeGate._ageValue;
		}
	}

	public static bool DisplayedScreen { get; private set; }

	private void Awake()
	{
		if (KIDAgeGate._activeReference != null)
		{
			Debug.LogError("[KID::Age_Gate] Age Gate already exists, this is a duplicate, deleting the new one");
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		KIDAgeGate._activeReference = this;
	}

	private void Start()
	{
		KIDAgeGate.<Start>d__29 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<KIDAgeGate.<Start>d__29>(ref <Start>d__);
	}

	private void OnDestroy()
	{
		this.requestCancellationSource.Cancel();
	}

	public static Task BeginAgeGate()
	{
		KIDAgeGate.<BeginAgeGate>d__31 <BeginAgeGate>d__;
		<BeginAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<BeginAgeGate>d__.<>1__state = -1;
		<BeginAgeGate>d__.<>t__builder.Start<KIDAgeGate.<BeginAgeGate>d__31>(ref <BeginAgeGate>d__);
		return <BeginAgeGate>d__.<>t__builder.Task;
	}

	private Task StartAgeGate()
	{
		KIDAgeGate.<StartAgeGate>d__32 <StartAgeGate>d__;
		<StartAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAgeGate>d__.<>4__this = this;
		<StartAgeGate>d__.<>1__state = -1;
		<StartAgeGate>d__.<>t__builder.Start<KIDAgeGate.<StartAgeGate>d__32>(ref <StartAgeGate>d__);
		return <StartAgeGate>d__.<>t__builder.Task;
	}

	private Task InitialiseAgeGate()
	{
		KIDAgeGate.<InitialiseAgeGate>d__33 <InitialiseAgeGate>d__;
		<InitialiseAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<InitialiseAgeGate>d__.<>4__this = this;
		<InitialiseAgeGate>d__.<>1__state = -1;
		<InitialiseAgeGate>d__.<>t__builder.Start<KIDAgeGate.<InitialiseAgeGate>d__33>(ref <InitialiseAgeGate>d__);
		return <InitialiseAgeGate>d__.<>t__builder.Task;
	}

	private Task ProcessAgeGate()
	{
		KIDAgeGate.<ProcessAgeGate>d__34 <ProcessAgeGate>d__;
		<ProcessAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ProcessAgeGate>d__.<>4__this = this;
		<ProcessAgeGate>d__.<>1__state = -1;
		<ProcessAgeGate>d__.<>t__builder.Start<KIDAgeGate.<ProcessAgeGate>d__34>(ref <ProcessAgeGate>d__);
		return <ProcessAgeGate>d__.<>t__builder.Task;
	}

	private Task<bool> ProcessAgeGateConfirmation()
	{
		KIDAgeGate.<ProcessAgeGateConfirmation>d__35 <ProcessAgeGateConfirmation>d__;
		<ProcessAgeGateConfirmation>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<ProcessAgeGateConfirmation>d__.<>4__this = this;
		<ProcessAgeGateConfirmation>d__.<>1__state = -1;
		<ProcessAgeGateConfirmation>d__.<>t__builder.Start<KIDAgeGate.<ProcessAgeGateConfirmation>d__35>(ref <ProcessAgeGateConfirmation>d__);
		return <ProcessAgeGateConfirmation>d__.<>t__builder.Task;
	}

	private Task WaitForAgeChoice()
	{
		KIDAgeGate.<WaitForAgeChoice>d__36 <WaitForAgeChoice>d__;
		<WaitForAgeChoice>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAgeChoice>d__.<>4__this = this;
		<WaitForAgeChoice>d__.<>1__state = -1;
		<WaitForAgeChoice>d__.<>t__builder.Start<KIDAgeGate.<WaitForAgeChoice>d__36>(ref <WaitForAgeChoice>d__);
		return <WaitForAgeChoice>d__.<>t__builder.Task;
	}

	public static void OnConfirmAgePressed(int currentAge)
	{
		KIDAgeGate._hasChosenAge = true;
	}

	private void OnAgeGateCompleted()
	{
		this.FinaliseAgeGateAndContinue();
	}

	private void FinaliseAgeGateAndContinue()
	{
		if (this.requestCancellationSource.IsCancellationRequested)
		{
			return;
		}
		Debug.Log("[KID::AGE_GATE] Age gate completed");
		Object.Destroy(base.gameObject);
	}

	private void QuitGame()
	{
		Debug.Log("[KID] QUIT PRESSED");
		Application.Quit();
	}

	private void AppealAge()
	{
		KIDAgeGate.<AppealAge>d__41 <AppealAge>d__;
		<AppealAge>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<AppealAge>d__.<>4__this = this;
		<AppealAge>d__.<>1__state = -1;
		<AppealAge>d__.<>t__builder.Start<KIDAgeGate.<AppealAge>d__41>(ref <AppealAge>d__);
	}

	private void AppealRejected()
	{
		Debug.Log("[KID] APPEAL REJECTED");
		string messageTitle = "UNDER AGE";
		string messageBody = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";
		string messageConfirmation = "Hold any face button to appeal";
		this._pregameMessageReference.ShowMessage(messageTitle, messageBody, messageConfirmation, new Action(this.AppealAge), 0.25f, 0f);
	}

	private void RefreshChallengeStatus()
	{
	}

	public static void SetAgeGateConfig(GetRequirementsData response)
	{
		KIDAgeGate._ageGateConfig = response;
	}

	public void OnWhyAgeGateButtonPressed()
	{
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"why_age_gate"
				}
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		this._uiParent.SetActive(false);
		PrivateUIRoom.AddUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(true);
	}

	public void OnWhyAgeGateButtonBackPressed()
	{
		this._uiParent.SetActive(true);
		PrivateUIRoom.RemoveUI(this._whyAgeGateScreen.transform);
		this._whyAgeGateScreen.SetActive(false);
	}

	public void OnLearnMoreAboutKIDPressed()
	{
		this._metrics_LearnMorePressed = true;
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "kid_screen_shown",
			CustomTags = new string[]
			{
				"kid_age_gate",
				KIDTelemetry.GameVersionCustomTag,
				KIDTelemetry.GameEnvironment
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"screen",
					"learn_more_url"
				}
			}
		};
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		Application.OpenURL("https://whyagegate.com/");
	}

	private const string LEARN_MORE_URL = "https://whyagegate.com/";

	private const string DEFAULT_AGE_VALUE_STRING = "SET AGE";

	private const int MINIMUM_PLATFORM_AGE = 13;

	[Header("Age Gate Settings")]
	[SerializeField]
	private PreGameMessage _pregameMessageReference;

	[SerializeField]
	private KIDUI_AgeDiscrepancyScreen _ageDiscrepancyScreen;

	[SerializeField]
	private GameObject _uiParent;

	[SerializeField]
	private AgeSliderWithProgressBar _ageSlider;

	[SerializeField]
	private GameObject _confirmationUI;

	[SerializeField]
	private KIDAgeGateConfirmation _confirmationUIManager;

	[SerializeField]
	private TMP_Text _confirmationAgeText;

	[SerializeField]
	private GameObject _whyAgeGateScreen;

	private const string strBlockAccessTitle = "UNDER AGE";

	private const string strBlockAccessMessage = "Your VR platform requires a certain minimum age to play Gorilla Tag. Unfortunately, due to those age requirements, we cannot allow you to play Gorilla Tag at this time.\n\nIf you incorrectly submitted your age, please appeal.";

	private const string strBlockAccessConfirm = "Hold any face button to appeal";

	private const string strVerifyAgeTitle = "VERIFY AGE";

	private const string strVerifyAgeMessage = "GETTING ONE TIME PASSCODE. PLEASE WAIT.\n\nGIVE IT TO A PARENT/GUARDIAN TO ENTER IT AT: k-id.com/code";

	private const string strDiscrepancyMessage = "You entered {0} for your age,\nbut your Meta account says you should be {1}. You could be logged into the wrong Meta account on this device.\n\nWe will use the lowest age ({2})\nif you Continue.";

	private static KIDAgeGate _activeReference;

	private static GetRequirementsData _ageGateConfig;

	private static int _ageValue;

	private CancellationTokenSource requestCancellationSource = new CancellationTokenSource();

	private static bool _hasChosenAge;

	private bool _metrics_LearnMorePressed;
}
