using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

public class KIDUI_Controller : MonoBehaviour
{
	public static KIDUI_Controller Instance
	{
		get
		{
			return KIDUI_Controller._instance;
		}
	}

	public static bool IsKIDUIActive
	{
		get
		{
			return !(KIDUI_Controller.Instance == null) && KIDUI_Controller.Instance._isKidUIActive;
		}
	}

	private static string EtagOnCloseBlackScreenPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr))
			{
				KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr = "closeBlackScreen-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDUI_Controller.etagOnCloseBlackScreenPlayerPrefStr;
		}
	}

	private void Awake()
	{
		KIDUI_Controller._instance = this;
		Debug.LogFormat("[KID::UI::CONTROLLER] Controller Initialised", Array.Empty<object>());
	}

	private void OnDestroy()
	{
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	public Task StartKIDScreens(CancellationToken cancellationToken)
	{
		KIDUI_Controller.<StartKIDScreens>d__20 <StartKIDScreens>d__;
		<StartKIDScreens>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartKIDScreens>d__.<>4__this = this;
		<StartKIDScreens>d__.cancellationToken = cancellationToken;
		<StartKIDScreens>d__.<>1__state = -1;
		<StartKIDScreens>d__.<>t__builder.Start<KIDUI_Controller.<StartKIDScreens>d__20>(ref <StartKIDScreens>d__);
		return <StartKIDScreens>d__.<>t__builder.Task;
	}

	public void CloseKIDScreens()
	{
		this.SaveEtagOnCloseScreen();
		this._isKidUIActive = false;
		this._mainKIDScreen.HideMainScreen();
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
		}
		PrivateUIRoom.RemoveUI(base.transform);
		HandRayController.Instance.DisableHandRays();
		Object.DestroyImmediate(base.gameObject);
		KIDManager.onEmailResultReceived = (KIDManager.OnEmailResultReceived)Delegate.Remove(KIDManager.onEmailResultReceived, new KIDManager.OnEmailResultReceived(this.NotifyOfEmailResult));
	}

	public void UpdateScreenStatus()
	{
		EMainScreenStatus screenStatusFromSession = this.GetScreenStatusFromSession();
		KIDUI_MainScreen mainKIDScreen = this._mainKIDScreen;
		if (mainKIDScreen == null)
		{
			return;
		}
		mainKIDScreen.UpdateScreenStatus(screenStatusFromSession, true);
	}

	public void NotifyOfEmailResult(bool success)
	{
		if (this._confirmScreen == null)
		{
			Debug.LogError("[KID::UI_CONTROLLER] _confirmScreen has not been set yet and is NULL. Cannot inform of result");
			return;
		}
		if (success)
		{
			PlayerPrefs.SetInt(KIDManager.GetChallengedBeforePlayerPrefRef, 1);
			PlayerPrefs.Save();
		}
		Debug.Log("[KID::UI_CONTROLLER] Notifying user about email result. Showing confirm screen.");
		this._confirmScreen.NotifyOfResult(success);
	}

	private EMainScreenStatus GetScreenStatusFromSession()
	{
		EMainScreenStatus result;
		switch (KIDManager.CurrentSession.SessionStatus)
		{
		case SessionStatus.PASS:
			if (this.ShouldShowScreenOnPermissionChange())
			{
				result = EMainScreenStatus.Updated;
			}
			else if (KIDManager.PreviousStatus == SessionStatus.CHALLENGE_SESSION_UPGRADE)
			{
				result = EMainScreenStatus.Declined;
			}
			else
			{
				result = EMainScreenStatus.Missing;
			}
			break;
		case SessionStatus.PROHIBITED:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Status is PROHIBITED but is trying to show k-ID screens");
			result = EMainScreenStatus.Declined;
			break;
		case SessionStatus.CHALLENGE:
		case SessionStatus.CHALLENGE_SESSION_UPGRADE:
		case SessionStatus.PENDING_AGE_APPEAL:
			if (string.IsNullOrEmpty(PlayerPrefs.GetString(KIDManager.GetEmailForUserPlayerPrefRef, "")))
			{
				result = EMainScreenStatus.Setup;
			}
			else
			{
				result = EMainScreenStatus.Pending;
			}
			break;
		default:
			Debug.LogError("[KID::KIDUI_CONTROLLER] Unknown status");
			result = EMainScreenStatus.None;
			break;
		}
		return result;
	}

	private Task<bool> ShouldShowKIDScreen(CancellationToken cancellationToken)
	{
		KIDUI_Controller.<ShouldShowKIDScreen>d__25 <ShouldShowKIDScreen>d__;
		<ShouldShowKIDScreen>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<ShouldShowKIDScreen>d__.<>4__this = this;
		<ShouldShowKIDScreen>d__.cancellationToken = cancellationToken;
		<ShouldShowKIDScreen>d__.<>1__state = -1;
		<ShouldShowKIDScreen>d__.<>t__builder.Start<KIDUI_Controller.<ShouldShowKIDScreen>d__25>(ref <ShouldShowKIDScreen>d__);
		return <ShouldShowKIDScreen>d__.<>t__builder.Task;
	}

	private bool ShouldShowScreenOnPermissionChange()
	{
		this._lastEtagOnClose = this.GetLastBlackScreenEtag();
		string lastEtagOnClose = this._lastEtagOnClose;
		TMPSession currentSession = KIDManager.CurrentSession;
		return lastEtagOnClose != (((currentSession != null) ? currentSession.Etag : null) ?? string.Empty);
	}

	private string GetLastBlackScreenEtag()
	{
		return PlayerPrefs.GetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, "");
	}

	private void SaveEtagOnCloseScreen()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] Trying to save Pre-Game Screen ETAG, but [CurrentSession] is null");
			return;
		}
		PlayerPrefs.SetString(KIDUI_Controller.EtagOnCloseBlackScreenPlayerPrefRef, KIDManager.CurrentSession.Etag);
		PlayerPrefs.Save();
	}

	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	private const string CLOSE_BLACK_SCREEN_ETAG_PLAYER_PREF_PREFIX = "closeBlackScreen-";

	private const string FIRST_TIME_POST_CHANGE_PLAYER_PREF = "hasShownFirstTimePostChange-";

	private static KIDUI_Controller _instance;

	[SerializeField]
	private KIDUI_MainScreen _mainKIDScreen;

	[SerializeField]
	private KIDUI_ConfirmScreen _confirmScreen;

	[SerializeField]
	private List<string> _PermissionsWithToggles = new List<string>();

	[SerializeField]
	private List<EKIDFeatures> _inaccessibleSettings = new List<EKIDFeatures>
	{
		EKIDFeatures.Multiplayer,
		EKIDFeatures.Mods
	};

	private KIDUI_Controller.Metrics_ShowReason _showReason;

	private bool _isKidUIActive;

	private static string etagOnCloseBlackScreenPlayerPrefStr;

	private string _lastEtagOnClose;

	public enum Metrics_ShowReason
	{
		None,
		Inaccessible,
		Guardian_Disabled,
		Permissions_Changed,
		Default_Session,
		No_Session
	}
}
