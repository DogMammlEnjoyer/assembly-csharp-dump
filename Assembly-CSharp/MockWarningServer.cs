using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

internal class MockWarningServer : WarningsServer
{
	public static string ShownScreenPlayerPref
	{
		get
		{
			return "screen-shown-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	private void Awake()
	{
		if (WarningsServer.Instance == null)
		{
			WarningsServer.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	private PlayerAgeGateWarningStatus CreateWarningStatus(string header, string body, MockWarningServer.ButtonSetup? leftButtonSetup, MockWarningServer.ButtonSetup? rightButtonSetup, EImageVisibility showImage, Action leftButtonCallback, Action rightButtonCallback)
	{
		PlayerAgeGateWarningStatus result;
		result.header = header;
		result.body = body;
		result.leftButtonText = string.Empty;
		result.rightButtonText = string.Empty;
		result.leftButtonResult = WarningButtonResult.None;
		result.rightButtonResult = WarningButtonResult.None;
		result.noWarningResult = WarningButtonResult.None;
		result.showImage = showImage;
		result.onLeftButtonPressedAction = leftButtonCallback;
		result.onRightButtonPressedAction = rightButtonCallback;
		if (leftButtonSetup != null)
		{
			result.leftButtonText = leftButtonSetup.Value.buttonText;
			result.leftButtonResult = leftButtonSetup.Value.buttonResult;
		}
		if (rightButtonSetup != null)
		{
			result.rightButtonText = rightButtonSetup.Value.buttonText;
			result.rightButtonResult = rightButtonSetup.Value.buttonResult;
		}
		return result;
	}

	public override Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token)
	{
		MockWarningServer.<FetchPlayerData>d__12 <FetchPlayerData>d__;
		<FetchPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerAgeGateWarningStatus?>.Create();
		<FetchPlayerData>d__.<>4__this = this;
		<FetchPlayerData>d__.token = token;
		<FetchPlayerData>d__.<>1__state = -1;
		<FetchPlayerData>d__.<>t__builder.Start<MockWarningServer.<FetchPlayerData>d__12>(ref <FetchPlayerData>d__);
		return <FetchPlayerData>d__.<>t__builder.Task;
	}

	public override Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token)
	{
		MockWarningServer.<GetOptInFollowUpMessage>d__13 <GetOptInFollowUpMessage>d__;
		<GetOptInFollowUpMessage>d__.<>t__builder = AsyncTaskMethodBuilder<PlayerAgeGateWarningStatus?>.Create();
		<GetOptInFollowUpMessage>d__.<>4__this = this;
		<GetOptInFollowUpMessage>d__.token = token;
		<GetOptInFollowUpMessage>d__.<>1__state = -1;
		<GetOptInFollowUpMessage>d__.<>t__builder.Start<MockWarningServer.<GetOptInFollowUpMessage>d__13>(ref <GetOptInFollowUpMessage>d__);
		return <GetOptInFollowUpMessage>d__.<>t__builder.Task;
	}

	private bool ShouldShowWarningScreen(int phase, bool inOptInCohort)
	{
		if (PlayerPrefs.GetInt(string.Format("phase-{0}-{1}", phase, MockWarningServer.ShownScreenPlayerPref), 0) == 0)
		{
			return true;
		}
		switch (phase)
		{
		default:
			return false;
		case 2:
			return inOptInCohort;
		case 3:
		case 4:
		case 5:
			return true;
		}
	}

	private const string SHOWN_SCREEN_PREFIX = "screen-shown-";

	private const string KID_WARNING_TITLE_KEY = "KID_WARNING_TITLE";

	private const string KID_WARNING_CONTINUE_KEY = "KID_WARNING_CONTINUE";

	private const string KID_WARNING_PHASE_THREE_IN_COHORT_KEY = "KID_WARNING_PHASE_THREE_IN_COHORT";

	private const string KID_WARNING_PHASE_FOUR_RETURNING_PLAYER_KEY = "KID_WARNING_PHASE_FOUR_RETURNING_PLAYER";

	private const string KID_WARNING_OPT_IN_FOLLOW_MESSAGE_KEY = "KID_WARNING_OPT_IN_FOLLOW_MESSAGE";

	private const string KID_WARNING_FOLLOW_UP_YAY_KEY = "KID_WARNING_FOLLOW_UP_YAY";

	public struct ButtonSetup
	{
		public ButtonSetup(string txt, WarningButtonResult result)
		{
			this.buttonText = txt;
			this.buttonResult = result;
		}

		public string buttonText;

		public WarningButtonResult buttonResult;
	}
}
