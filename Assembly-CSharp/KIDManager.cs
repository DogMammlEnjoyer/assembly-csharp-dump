using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using Newtonsoft.Json;
using UnityEngine;

public class KIDManager : MonoBehaviour
{
	public static KIDManager Instance
	{
		get
		{
			return KIDManager._instance;
		}
	}

	public static bool InitialisationComplete { get; private set; } = false;

	public static bool InitialisationSuccessful { get; private set; } = false;

	public static TMPSession CurrentSession { get; private set; }

	public static SessionStatus PreviousStatus { get; private set; }

	public static GetRequirementsData _ageGateRequirements { get; private set; }

	public static bool KidTitleDataReady
	{
		get
		{
			return KIDManager._titleDataReady;
		}
	}

	public static bool KidEnabled
	{
		get
		{
			return KIDManager.KidTitleDataReady && KIDManager._useKid;
		}
	}

	public static bool KidEnabledAndReady
	{
		get
		{
			return KIDManager.KidEnabled && KIDManager.InitialisationSuccessful;
		}
	}

	public static bool HasSession
	{
		get
		{
			return KIDManager.CurrentSession != null && KIDManager.CurrentSession.SessionId != Guid.Empty;
		}
	}

	public static string PreviousStatusPlayerPrefRef
	{
		get
		{
			return "previous-status-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	public static bool HasOptedInToKID { get; private set; }

	private static string KIDSetupPlayerPref
	{
		get
		{
			return "KID-Setup-";
		}
	}

	public static string DbgLocale { get; set; }

	public static string DebugKIDLocalePlayerPrefRef
	{
		get
		{
			return KIDManager._debugKIDLocalePlayerPrefRef;
		}
	}

	public static string GetEmailForUserPlayerPrefRef
	{
		get
		{
			if (string.IsNullOrEmpty(KIDManager.parentEmailForUserPlayerPrefRef))
			{
				KIDManager.parentEmailForUserPlayerPrefRef = "k-id_EmailAddress" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return KIDManager.parentEmailForUserPlayerPrefRef;
		}
	}

	public static string GetChallengedBeforePlayerPrefRef
	{
		get
		{
			return "k-id_ChallengedBefore" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		}
	}

	private void Awake()
	{
		if (KIDManager._instance != null)
		{
			Debug.LogError("Trying to create new instance of [KIDManager], but one already exists. Destroying object [" + base.gameObject.name + "].");
			Object.Destroy(base.gameObject);
			return;
		}
		Debug.Log("[KID] INIT");
		KIDManager._instance = this;
		KIDManager.DbgLocale = PlayerPrefs.GetString(KIDManager._debugKIDLocalePlayerPrefRef, "");
	}

	private void Start()
	{
		KIDManager.<Start>d__70 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<KIDManager.<Start>d__70>(ref <Start>d__);
	}

	private void OnDestroy()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	public static string GetActiveAccountStatusNiceString()
	{
		switch (KIDManager.GetActiveAccountStatus())
		{
		case AgeStatusType.DIGITALMINOR:
			return "Digital Minor";
		case AgeStatusType.DIGITALYOUTH:
			return "Digital Youth";
		case AgeStatusType.LEGALADULT:
			return "Legal Adult";
		default:
			return "UNKNOWN";
		}
	}

	public static AgeStatusType GetActiveAccountStatus()
	{
		if (KIDManager.CurrentSession != null)
		{
			return KIDManager.CurrentSession.AgeStatus;
		}
		if (!PlayFabAuthenticator.instance.GetSafety())
		{
			return AgeStatusType.LEGALADULT;
		}
		return AgeStatusType.DIGITALMINOR;
	}

	public static List<Permission> GetAllPermissionsData()
	{
		if (KIDManager.CurrentSession == null)
		{
			Debug.LogError("[KID::MANAGER] There is no current session. Unless the age-gate has not yet finished there should always be a session even if it is the default session");
			return new List<Permission>();
		}
		return KIDManager.CurrentSession.GetAllPermissions();
	}

	public static bool TryGetAgeStatusTypeFromAge(int age, out AgeStatusType ageType)
	{
		if (KIDManager._ageGateRequirements == null)
		{
			Debug.LogError("[KID::MANAGER] [_ageGateRequirements] is not set - need to Get AgeGate Requirements first");
			ageType = AgeStatusType.DIGITALMINOR;
			return false;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.DigitalConsentAge)
		{
			ageType = AgeStatusType.DIGITALMINOR;
			return true;
		}
		if (age < KIDManager._ageGateRequirements.AgeGateRequirements.CivilAge)
		{
			ageType = AgeStatusType.DIGITALYOUTH;
			return true;
		}
		ageType = AgeStatusType.LEGALADULT;
		return true;
	}

	[return: TupleElementNames(new string[]
	{
		"requiresOptIn",
		"hasOptedInPreviously"
	})]
	public static ValueTuple<bool, bool> CheckFeatureOptIn(EKIDFeatures feature, Permission permissionData = null)
	{
		if (permissionData == null)
		{
			permissionData = KIDManager.GetPermissionDataByFeature(feature);
			if (permissionData == null)
			{
				Debug.LogError("[KID::MANAGER] Unable to retrieve permission data for feature [" + feature.ToStandardisedString() + "]");
				return new ValueTuple<bool, bool>(false, false);
			}
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return new ValueTuple<bool, bool>(false, false);
		}
		bool item = true;
		if (KIDManager.CurrentSession != null)
		{
			item = KIDManager.CurrentSession.HasOptedInToPermission(feature);
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.GUARDIAN)
		{
			return new ValueTuple<bool, bool>(false, item);
		}
		if (permissionData.ManagedBy == Permission.ManagedByEnum.PLAYER && permissionData.Enabled)
		{
			return new ValueTuple<bool, bool>(false, true);
		}
		return new ValueTuple<bool, bool>(true, item);
	}

	public static void SetFeatureOptIn(EKIDFeatures feature, bool optedIn)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogErrorFormat("[KID] Trying to set Feature Opt in for feature [" + feature.ToStandardisedString() + "] but permission data could not be found. Assumed is opt-in", Array.Empty<object>());
			return;
		}
		if (KIDManager.CurrentSession == null)
		{
			Debug.Log("[KID::MANAGER] CurrentSession is null, cannot set feature opt-in. Returning.");
			return;
		}
		switch (permissionDataByFeature.ManagedBy)
		{
		case Permission.ManagedByEnum.PLAYER:
			KIDManager.CurrentSession.OptInToPermission(feature, optedIn);
			return;
		case Permission.ManagedByEnum.GUARDIAN:
			KIDManager.CurrentSession.OptInToPermission(feature, permissionDataByFeature.Enabled);
			return;
		case Permission.ManagedByEnum.PROHIBITED:
			KIDManager.CurrentSession.OptInToPermission(feature, false);
			return;
		default:
			return;
		}
	}

	public static bool CheckFeatureSettingEnabled(EKIDFeatures feature)
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		if (permissionDataByFeature == null)
		{
			Debug.LogError("[KID::MANAGER] Unable to permissions for feature [" + feature.ToStandardisedString() + "]");
			return false;
		}
		if (permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PROHIBITED)
		{
			return false;
		}
		bool item = KIDManager.CheckFeatureOptIn(feature, null).Item2;
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
		case EKIDFeatures.Mods:
			return item;
		case EKIDFeatures.Custom_Nametags:
			return item && GorillaComputer.instance.NametagsEnabled;
		case EKIDFeatures.Voice_Chat:
			return item && GorillaComputer.instance.CheckVoiceChatEnabled();
		case EKIDFeatures.Groups:
			return permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.GUARDIAN || permissionDataByFeature.Enabled;
		default:
			Debug.LogError("[KID::MANAGER] Tried finding feature setting for [" + feature.ToStandardisedString() + "] but failed.");
			return false;
		}
	}

	private static Task<GetPlayerData_Data> TryGetPlayerData(bool forceRefresh)
	{
		KIDManager.<TryGetPlayerData>d__81 <TryGetPlayerData>d__;
		<TryGetPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<GetPlayerData_Data>.Create();
		<TryGetPlayerData>d__.forceRefresh = forceRefresh;
		<TryGetPlayerData>d__.<>1__state = -1;
		<TryGetPlayerData>d__.<>t__builder.Start<KIDManager.<TryGetPlayerData>d__81>(ref <TryGetPlayerData>d__);
		return <TryGetPlayerData>d__.<>t__builder.Task;
	}

	private static Task<GetRequirementsData> TryGetRequirements()
	{
		KIDManager.<TryGetRequirements>d__82 <TryGetRequirements>d__;
		<TryGetRequirements>d__.<>t__builder = AsyncTaskMethodBuilder<GetRequirementsData>.Create();
		<TryGetRequirements>d__.<>1__state = -1;
		<TryGetRequirements>d__.<>t__builder.Start<KIDManager.<TryGetRequirements>d__82>(ref <TryGetRequirements>d__);
		return <TryGetRequirements>d__.<>t__builder.Task;
	}

	private static Task<VerifyAgeData> TryVerifyAgeResponse()
	{
		KIDManager.<TryVerifyAgeResponse>d__83 <TryVerifyAgeResponse>d__;
		<TryVerifyAgeResponse>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<TryVerifyAgeResponse>d__.<>1__state = -1;
		<TryVerifyAgeResponse>d__.<>t__builder.Start<KIDManager.<TryVerifyAgeResponse>d__83>(ref <TryVerifyAgeResponse>d__);
		return <TryVerifyAgeResponse>d__.<>t__builder.Task;
	}

	[return: TupleElementNames(new string[]
	{
		"success",
		"exception"
	})]
	private static Task<ValueTuple<bool, string>> TrySendChallengeEmailRequest()
	{
		KIDManager.<TrySendChallengeEmailRequest>d__84 <TrySendChallengeEmailRequest>d__;
		<TrySendChallengeEmailRequest>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<TrySendChallengeEmailRequest>d__.<>1__state = -1;
		<TrySendChallengeEmailRequest>d__.<>t__builder.Start<KIDManager.<TrySendChallengeEmailRequest>d__84>(ref <TrySendChallengeEmailRequest>d__);
		return <TrySendChallengeEmailRequest>d__.<>t__builder.Task;
	}

	private static Task<bool> TrySendOptInPermissions()
	{
		KIDManager.<TrySendOptInPermissions>d__85 <TrySendOptInPermissions>d__;
		<TrySendOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TrySendOptInPermissions>d__.<>1__state = -1;
		<TrySendOptInPermissions>d__.<>t__builder.Start<KIDManager.<TrySendOptInPermissions>d__85>(ref <TrySendOptInPermissions>d__);
		return <TrySendOptInPermissions>d__.<>t__builder.Task;
	}

	public static Task<ValueTuple<bool, string>> TrySendUpgradeSessionChallengeEmail()
	{
		KIDManager.<TrySendUpgradeSessionChallengeEmail>d__86 <TrySendUpgradeSessionChallengeEmail>d__;
		<TrySendUpgradeSessionChallengeEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<TrySendUpgradeSessionChallengeEmail>d__.<>1__state = -1;
		<TrySendUpgradeSessionChallengeEmail>d__.<>t__builder.Start<KIDManager.<TrySendUpgradeSessionChallengeEmail>d__86>(ref <TrySendUpgradeSessionChallengeEmail>d__);
		return <TrySendUpgradeSessionChallengeEmail>d__.<>t__builder.Task;
	}

	public static Task<bool> TrySetHasConfirmedStatus()
	{
		KIDManager.<TrySetHasConfirmedStatus>d__87 <TrySetHasConfirmedStatus>d__;
		<TrySetHasConfirmedStatus>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TrySetHasConfirmedStatus>d__.<>1__state = -1;
		<TrySetHasConfirmedStatus>d__.<>t__builder.Start<KIDManager.<TrySetHasConfirmedStatus>d__87>(ref <TrySetHasConfirmedStatus>d__);
		return <TrySetHasConfirmedStatus>d__.<>t__builder.Task;
	}

	public static Task<UpgradeSessionData> TryUpgradeSession(List<string> requestedPermissions)
	{
		KIDManager.<TryUpgradeSession>d__88 <TryUpgradeSession>d__;
		<TryUpgradeSession>d__.<>t__builder = AsyncTaskMethodBuilder<UpgradeSessionData>.Create();
		<TryUpgradeSession>d__.requestedPermissions = requestedPermissions;
		<TryUpgradeSession>d__.<>1__state = -1;
		<TryUpgradeSession>d__.<>t__builder.Start<KIDManager.<TryUpgradeSession>d__88>(ref <TryUpgradeSession>d__);
		return <TryUpgradeSession>d__.<>t__builder.Task;
	}

	public static Task<AttemptAgeUpdateData> TryAttemptAgeUpdate(int age)
	{
		KIDManager.<TryAttemptAgeUpdate>d__89 <TryAttemptAgeUpdate>d__;
		<TryAttemptAgeUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<AttemptAgeUpdateData>.Create();
		<TryAttemptAgeUpdate>d__.age = age;
		<TryAttemptAgeUpdate>d__.<>1__state = -1;
		<TryAttemptAgeUpdate>d__.<>t__builder.Start<KIDManager.<TryAttemptAgeUpdate>d__89>(ref <TryAttemptAgeUpdate>d__);
		return <TryAttemptAgeUpdate>d__.<>t__builder.Task;
	}

	public static Task<bool> TryAppealAge(string email, int newAge)
	{
		KIDManager.<TryAppealAge>d__90 <TryAppealAge>d__;
		<TryAppealAge>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TryAppealAge>d__.email = email;
		<TryAppealAge>d__.newAge = newAge;
		<TryAppealAge>d__.<>1__state = -1;
		<TryAppealAge>d__.<>t__builder.Start<KIDManager.<TryAppealAge>d__90>(ref <TryAppealAge>d__);
		return <TryAppealAge>d__.<>t__builder.Task;
	}

	public static Task UpdateSession(Action<bool> getDataCompleted = null)
	{
		KIDManager.<UpdateSession>d__91 <UpdateSession>d__;
		<UpdateSession>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateSession>d__.getDataCompleted = getDataCompleted;
		<UpdateSession>d__.<>1__state = -1;
		<UpdateSession>d__.<>t__builder.Start<KIDManager.<UpdateSession>d__91>(ref <UpdateSession>d__);
		return <UpdateSession>d__.<>t__builder.Task;
	}

	private static Task<bool> CheckWarningScreensOptedIn()
	{
		KIDManager.<CheckWarningScreensOptedIn>d__92 <CheckWarningScreensOptedIn>d__;
		<CheckWarningScreensOptedIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<CheckWarningScreensOptedIn>d__.<>1__state = -1;
		<CheckWarningScreensOptedIn>d__.<>t__builder.Start<KIDManager.<CheckWarningScreensOptedIn>d__92>(ref <CheckWarningScreensOptedIn>d__);
		return <CheckWarningScreensOptedIn>d__.<>t__builder.Task;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void InitialiseBootFlow()
	{
		Debug.Log("[KID::MANAGER] PHASE ZERO -- START -- Checking K-ID Flag");
		if (PlayerPrefs.GetInt(KIDManager.KIDSetupPlayerPref, 0) != 0)
		{
			return;
		}
		Debug.Log("[KID::MANAGER] INITIALISE BOOT FLOW - Force Starting Overlay");
		PrivateUIRoom.ForceStartOverlay();
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void InitialiseKID()
	{
		KIDManager.<InitialiseKID>d__94 <InitialiseKID>d__;
		<InitialiseKID>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<InitialiseKID>d__.<>1__state = -1;
		<InitialiseKID>d__.<>t__builder.Start<KIDManager.<InitialiseKID>d__94>(ref <InitialiseKID>d__);
	}

	private static bool UpdatePermissions(TMPSession newSession)
	{
		Debug.Log("[KID::MANAGER] Updating Permissions to reflect session.");
		if (newSession == null || !newSession.IsValidSession)
		{
			Debug.LogError("[KID::MANAGER] A NULL or Invalid Session was received!");
			return false;
		}
		KIDManager.CurrentSession = newSession;
		if (KIDUI_Controller.IsKIDUIActive)
		{
			KIDManager.PreviousStatus = KIDManager.CurrentSession.SessionStatus;
			PlayerPrefs.SetInt(KIDManager.PreviousStatusPlayerPrefRef, (int)KIDManager.PreviousStatus);
			PlayerPrefs.Save();
		}
		if (!KIDManager.CurrentSession.IsDefault)
		{
			PlayerPrefs.SetInt(KIDManager.KIDSetupPlayerPref, 1);
			PlayerPrefs.Save();
		}
		KIDManager.OnSessionUpdated();
		if (KIDUI_Controller.Instance)
		{
			KIDUI_Controller.Instance.UpdateScreenStatus();
		}
		return true;
	}

	private static void ClearSession()
	{
		KIDManager.CurrentSession = null;
		KIDManager.DeleteStoredPermissions();
	}

	private static void DeleteStoredPermissions()
	{
	}

	public static CancellationTokenSource ResetCancellationToken()
	{
		KIDManager._requestCancellationSource.Dispose();
		KIDManager._requestCancellationSource = new CancellationTokenSource();
		return KIDManager._requestCancellationSource;
	}

	public static Permission GetPermissionDataByFeature(EKIDFeatures feature)
	{
		if (KIDManager.CurrentSession == null)
		{
			if (!PlayFabAuthenticator.instance.GetSafety())
			{
				return new Permission(feature.ToStandardisedString(), true, Permission.ManagedByEnum.PLAYER);
			}
			return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
		}
		else
		{
			Permission result;
			if (!KIDManager.CurrentSession.TryGetPermission(feature, out result))
			{
				Debug.LogError("[KID::MANAGER] Failed to retreive permission from session for [" + feature.ToStandardisedString() + "]. Assuming disabled permission");
				return new Permission(feature.ToStandardisedString(), false, Permission.ManagedByEnum.GUARDIAN);
			}
			return result;
		}
	}

	public static void CancelToken()
	{
		KIDManager._requestCancellationSource.Cancel();
	}

	public static Task<bool> UseKID()
	{
		KIDManager.<UseKID>d__101 <UseKID>d__;
		<UseKID>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UseKID>d__.<>1__state = -1;
		<UseKID>d__.<>t__builder.Start<KIDManager.<UseKID>d__101>(ref <UseKID>d__);
		return <UseKID>d__.<>t__builder.Task;
	}

	public static Task<int> CheckKIDPhase()
	{
		KIDManager.<CheckKIDPhase>d__102 <CheckKIDPhase>d__;
		<CheckKIDPhase>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
		<CheckKIDPhase>d__.<>1__state = -1;
		<CheckKIDPhase>d__.<>t__builder.Start<KIDManager.<CheckKIDPhase>d__102>(ref <CheckKIDPhase>d__);
		return <CheckKIDPhase>d__.<>t__builder.Task;
	}

	public static Task<DateTime?> CheckKIDNewPlayerDateTime()
	{
		KIDManager.<CheckKIDNewPlayerDateTime>d__103 <CheckKIDNewPlayerDateTime>d__;
		<CheckKIDNewPlayerDateTime>d__.<>t__builder = AsyncTaskMethodBuilder<DateTime?>.Create();
		<CheckKIDNewPlayerDateTime>d__.<>1__state = -1;
		<CheckKIDNewPlayerDateTime>d__.<>t__builder.Start<KIDManager.<CheckKIDNewPlayerDateTime>d__103>(ref <CheckKIDNewPlayerDateTime>d__);
		return <CheckKIDNewPlayerDateTime>d__.<>t__builder.Task;
	}

	private static bool GetIsEnabled(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return false;
		}
		bool result;
		if (!bool.TryParse(kidtitleData.KIDEnabled, out result))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDEnabled': [KIDEnabled] to bool.");
			return false;
		}
		return result;
	}

	private static int GetPhase(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return 0;
		}
		return kidtitleData.KIDPhase;
	}

	private static DateTime? GetNewPlayerDateTime(string jsonTxt)
	{
		KIDTitleData kidtitleData = JsonConvert.DeserializeObject<KIDTitleData>(jsonTxt);
		if (kidtitleData == null)
		{
			Debug.LogError("[KID_MANAGER] Failed to parse json to [KIDTitleData]. Json: \n" + jsonTxt);
			return null;
		}
		DateTime value;
		if (!DateTime.TryParse(kidtitleData.KIDNewPlayerIsoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out value))
		{
			Debug.LogError("[KID_MANAGER] Failed to parse 'KIDNewPlayerIsoTimestamp': [KIDNewPlayerIsoTimestamp] to DateTime.");
			return null;
		}
		return new DateTime?(value);
	}

	public static bool IsAdult()
	{
		return KIDManager.CurrentSession.IsValidSession && KIDManager.CurrentSession.AgeStatus == AgeStatusType.LEGALADULT;
	}

	public static bool HasAllPermissions()
	{
		List<Permission> allPermissions = KIDManager.CurrentSession.GetAllPermissions();
		for (int i = 0; i < allPermissions.Count; i++)
		{
			if (allPermissions[i].ManagedBy == Permission.ManagedByEnum.GUARDIAN || !allPermissions[i].Enabled)
			{
				return false;
			}
		}
		return true;
	}

	public static Task<bool> SetKIDOptIn()
	{
		KIDManager.<SetKIDOptIn>d__109 <SetKIDOptIn>d__;
		<SetKIDOptIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<SetKIDOptIn>d__.<>1__state = -1;
		<SetKIDOptIn>d__.<>t__builder.Start<KIDManager.<SetKIDOptIn>d__109>(ref <SetKIDOptIn>d__);
		return <SetKIDOptIn>d__.<>t__builder.Task;
	}

	[return: TupleElementNames(new string[]
	{
		"success",
		"message"
	})]
	public static Task<ValueTuple<bool, string>> SetAndSendEmail(string email)
	{
		KIDManager.<SetAndSendEmail>d__110 <SetAndSendEmail>d__;
		<SetAndSendEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<SetAndSendEmail>d__.email = email;
		<SetAndSendEmail>d__.<>1__state = -1;
		<SetAndSendEmail>d__.<>t__builder.Start<KIDManager.<SetAndSendEmail>d__110>(ref <SetAndSendEmail>d__);
		return <SetAndSendEmail>d__.<>t__builder.Task;
	}

	public static Task<bool> SendOptInPermissions()
	{
		KIDManager.<SendOptInPermissions>d__111 <SendOptInPermissions>d__;
		<SendOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<SendOptInPermissions>d__.<>1__state = -1;
		<SendOptInPermissions>d__.<>t__builder.Start<KIDManager.<SendOptInPermissions>d__111>(ref <SendOptInPermissions>d__);
		return <SendOptInPermissions>d__.<>t__builder.Task;
	}

	public static bool HasPermissionToUseFeature(EKIDFeatures feature)
	{
		if (!KIDManager.KidEnabledAndReady)
		{
			return !PlayFabAuthenticator.instance.GetSafety();
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(feature);
		return (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
	}

	private static Task<bool> WaitForAuthentication()
	{
		KIDManager.<WaitForAuthentication>d__113 <WaitForAuthentication>d__;
		<WaitForAuthentication>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForAuthentication>d__.<>1__state = -1;
		<WaitForAuthentication>d__.<>t__builder.Start<KIDManager.<WaitForAuthentication>d__113>(ref <WaitForAuthentication>d__);
		return <WaitForAuthentication>d__.<>t__builder.Task;
	}

	[return: TupleElementNames(new string[]
	{
		"ageStatus",
		"resp"
	})]
	private static Task<ValueTuple<AgeStatusType, TMPSession>> AgeGateFlow(GetPlayerData_Data newPlayerData)
	{
		KIDManager.<AgeGateFlow>d__114 <AgeGateFlow>d__;
		<AgeGateFlow>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<AgeStatusType, TMPSession>>.Create();
		<AgeGateFlow>d__.newPlayerData = newPlayerData;
		<AgeGateFlow>d__.<>1__state = -1;
		<AgeGateFlow>d__.<>t__builder.Start<KIDManager.<AgeGateFlow>d__114>(ref <AgeGateFlow>d__);
		return <AgeGateFlow>d__.<>t__builder.Task;
	}

	private static Task<VerifyAgeData> ProcessAgeGate()
	{
		KIDManager.<ProcessAgeGate>d__115 <ProcessAgeGate>d__;
		<ProcessAgeGate>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<ProcessAgeGate>d__.<>1__state = -1;
		<ProcessAgeGate>d__.<>t__builder.Start<KIDManager.<ProcessAgeGate>d__115>(ref <ProcessAgeGate>d__);
		return <ProcessAgeGate>d__.<>t__builder.Task;
	}

	public static string GetOptInKey(EKIDFeatures feature)
	{
		return feature.ToStandardisedString() + "-opt-in-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
	}

	private static Task<GetPlayerData_Data> Server_GetPlayerData(bool forceRefresh, Action failureCallback)
	{
		KIDManager.<Server_GetPlayerData>d__130 <Server_GetPlayerData>d__;
		<Server_GetPlayerData>d__.<>t__builder = AsyncTaskMethodBuilder<GetPlayerData_Data>.Create();
		<Server_GetPlayerData>d__.forceRefresh = forceRefresh;
		<Server_GetPlayerData>d__.failureCallback = failureCallback;
		<Server_GetPlayerData>d__.<>1__state = -1;
		<Server_GetPlayerData>d__.<>t__builder.Start<KIDManager.<Server_GetPlayerData>d__130>(ref <Server_GetPlayerData>d__);
		return <Server_GetPlayerData>d__.<>t__builder.Task;
	}

	private static Task<bool> Server_SetConfirmedStatus()
	{
		KIDManager.<Server_SetConfirmedStatus>d__131 <Server_SetConfirmedStatus>d__;
		<Server_SetConfirmedStatus>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_SetConfirmedStatus>d__.<>1__state = -1;
		<Server_SetConfirmedStatus>d__.<>t__builder.Start<KIDManager.<Server_SetConfirmedStatus>d__131>(ref <Server_SetConfirmedStatus>d__);
		return <Server_SetConfirmedStatus>d__.<>t__builder.Task;
	}

	private static Task<UpgradeSessionData> Server_UpgradeSession(global::UpgradeSessionRequest request)
	{
		KIDManager.<Server_UpgradeSession>d__132 <Server_UpgradeSession>d__;
		<Server_UpgradeSession>d__.<>t__builder = AsyncTaskMethodBuilder<UpgradeSessionData>.Create();
		<Server_UpgradeSession>d__.request = request;
		<Server_UpgradeSession>d__.<>1__state = -1;
		<Server_UpgradeSession>d__.<>t__builder.Start<KIDManager.<Server_UpgradeSession>d__132>(ref <Server_UpgradeSession>d__);
		return <Server_UpgradeSession>d__.<>t__builder.Task;
	}

	private static Task<VerifyAgeData> Server_VerifyAge(VerifyAgeRequest request, Action failureCallback)
	{
		KIDManager.<Server_VerifyAge>d__133 <Server_VerifyAge>d__;
		<Server_VerifyAge>d__.<>t__builder = AsyncTaskMethodBuilder<VerifyAgeData>.Create();
		<Server_VerifyAge>d__.request = request;
		<Server_VerifyAge>d__.failureCallback = failureCallback;
		<Server_VerifyAge>d__.<>1__state = -1;
		<Server_VerifyAge>d__.<>t__builder.Start<KIDManager.<Server_VerifyAge>d__133>(ref <Server_VerifyAge>d__);
		return <Server_VerifyAge>d__.<>t__builder.Task;
	}

	private static Task<AttemptAgeUpdateData> Server_AttemptAgeUpdate(AttemptAgeUpdateRequest request, Action failureCallback)
	{
		KIDManager.<Server_AttemptAgeUpdate>d__134 <Server_AttemptAgeUpdate>d__;
		<Server_AttemptAgeUpdate>d__.<>t__builder = AsyncTaskMethodBuilder<AttemptAgeUpdateData>.Create();
		<Server_AttemptAgeUpdate>d__.request = request;
		<Server_AttemptAgeUpdate>d__.<>1__state = -1;
		<Server_AttemptAgeUpdate>d__.<>t__builder.Start<KIDManager.<Server_AttemptAgeUpdate>d__134>(ref <Server_AttemptAgeUpdate>d__);
		return <Server_AttemptAgeUpdate>d__.<>t__builder.Task;
	}

	private static Task<bool> Server_AppealAge(AppealAgeRequest request, Action failureCallback)
	{
		KIDManager.<Server_AppealAge>d__135 <Server_AppealAge>d__;
		<Server_AppealAge>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_AppealAge>d__.request = request;
		<Server_AppealAge>d__.<>1__state = -1;
		<Server_AppealAge>d__.<>t__builder.Start<KIDManager.<Server_AppealAge>d__135>(ref <Server_AppealAge>d__);
		return <Server_AppealAge>d__.<>t__builder.Task;
	}

	private static Task<ValueTuple<bool, string>> Server_SendChallengeEmail(SendChallengeEmailRequest request)
	{
		KIDManager.<Server_SendChallengeEmail>d__136 <Server_SendChallengeEmail>d__;
		<Server_SendChallengeEmail>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
		<Server_SendChallengeEmail>d__.request = request;
		<Server_SendChallengeEmail>d__.<>1__state = -1;
		<Server_SendChallengeEmail>d__.<>t__builder.Start<KIDManager.<Server_SendChallengeEmail>d__136>(ref <Server_SendChallengeEmail>d__);
		return <Server_SendChallengeEmail>d__.<>t__builder.Task;
	}

	private static Task<bool> Server_SetOptInPermissions(SetOptInPermissionsRequest request, Action failureCallback)
	{
		KIDManager.<Server_SetOptInPermissions>d__137 <Server_SetOptInPermissions>d__;
		<Server_SetOptInPermissions>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_SetOptInPermissions>d__.request = request;
		<Server_SetOptInPermissions>d__.failureCallback = failureCallback;
		<Server_SetOptInPermissions>d__.<>1__state = -1;
		<Server_SetOptInPermissions>d__.<>t__builder.Start<KIDManager.<Server_SetOptInPermissions>d__137>(ref <Server_SetOptInPermissions>d__);
		return <Server_SetOptInPermissions>d__.<>t__builder.Task;
	}

	private static Task<bool> Server_OptIn()
	{
		KIDManager.<Server_OptIn>d__138 <Server_OptIn>d__;
		<Server_OptIn>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<Server_OptIn>d__.<>1__state = -1;
		<Server_OptIn>d__.<>t__builder.Start<KIDManager.<Server_OptIn>d__138>(ref <Server_OptIn>d__);
		return <Server_OptIn>d__.<>t__builder.Task;
	}

	private static Task<GetRequirementsData> Server_GetRequirements()
	{
		KIDManager.<Server_GetRequirements>d__139 <Server_GetRequirements>d__;
		<Server_GetRequirements>d__.<>t__builder = AsyncTaskMethodBuilder<GetRequirementsData>.Create();
		<Server_GetRequirements>d__.<>1__state = -1;
		<Server_GetRequirements>d__.<>t__builder.Start<KIDManager.<Server_GetRequirements>d__139>(ref <Server_GetRequirements>d__);
		return <Server_GetRequirements>d__.<>t__builder.Task;
	}

	[return: TupleElementNames(new string[]
	{
		"code",
		"responseModel",
		"errorMessage"
	})]
	private static Task<ValueTuple<long, T, string>> KIDServerWebRequest<T, Q>(string endpoint, string operationType, Q requestData, string queryParams = null, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where T : class where Q : KIDRequestData
	{
		KIDManager.<KIDServerWebRequest>d__140<T, Q> <KIDServerWebRequest>d__;
		<KIDServerWebRequest>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<long, T, string>>.Create();
		<KIDServerWebRequest>d__.endpoint = endpoint;
		<KIDServerWebRequest>d__.operationType = operationType;
		<KIDServerWebRequest>d__.requestData = requestData;
		<KIDServerWebRequest>d__.queryParams = queryParams;
		<KIDServerWebRequest>d__.maxRetries = maxRetries;
		<KIDServerWebRequest>d__.responseCodeIsRetryable = responseCodeIsRetryable;
		<KIDServerWebRequest>d__.<>1__state = -1;
		<KIDServerWebRequest>d__.<>t__builder.Start<KIDManager.<KIDServerWebRequest>d__140<T, Q>>(ref <KIDServerWebRequest>d__);
		return <KIDServerWebRequest>d__.<>t__builder.Task;
	}

	private static Task<long> KIDServerWebRequestNoResponse<Q>(string endpoint, string operationType, Q requestData, int maxRetries = 2, Func<long, bool> responseCodeIsRetryable = null) where Q : KIDRequestData
	{
		KIDManager.<KIDServerWebRequestNoResponse>d__141<Q> <KIDServerWebRequestNoResponse>d__;
		<KIDServerWebRequestNoResponse>d__.<>t__builder = AsyncTaskMethodBuilder<long>.Create();
		<KIDServerWebRequestNoResponse>d__.endpoint = endpoint;
		<KIDServerWebRequestNoResponse>d__.operationType = operationType;
		<KIDServerWebRequestNoResponse>d__.requestData = requestData;
		<KIDServerWebRequestNoResponse>d__.maxRetries = maxRetries;
		<KIDServerWebRequestNoResponse>d__.responseCodeIsRetryable = responseCodeIsRetryable;
		<KIDServerWebRequestNoResponse>d__.<>1__state = -1;
		<KIDServerWebRequestNoResponse>d__.<>t__builder.Start<KIDManager.<KIDServerWebRequestNoResponse>d__141<Q>>(ref <KIDServerWebRequestNoResponse>d__);
		return <KIDServerWebRequestNoResponse>d__.<>t__builder.Task;
	}

	public static void RegisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors any permission change");
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Combine(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	public static void UnregisterSessionUpdateCallback_AnyPermission(Action callback)
	{
		Debug.Log("[KID] Successfully unregistered a new callback to SessionUpdate which monitors any permission change");
		KIDManager._onSessionUpdated_AnyPermission = (Action)Delegate.Remove(KIDManager._onSessionUpdated_AnyPermission, callback);
	}

	public static void RegisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Voice Chat permission");
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	public static void UnregisterSessionUpdatedCallback_VoiceChat(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Voice Chat permission");
		KIDManager._onSessionUpdated_VoiceChat = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_VoiceChat, callback);
	}

	public static void RegisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Custom Usernames permission");
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	public static void UnregisterSessionUpdatedCallback_CustomUsernames(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Custom Usernames permission");
		KIDManager._onSessionUpdated_CustomUsernames = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_CustomUsernames, callback);
	}

	public static void RegisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Private Rooms permission");
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	public static void UnregisterSessionUpdatedCallback_PrivateRooms(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Private Rooms permission");
		KIDManager._onSessionUpdated_PrivateRooms = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_PrivateRooms, callback);
	}

	public static void RegisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the Multiplayer permission");
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	public static void UnregisterSessionUpdatedCallback_Multiplayer(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully unregistered a callback to SessionUpdate which monitors the Multiplayer permission");
		KIDManager._onSessionUpdated_Multiplayer = (Action<bool, Permission.ManagedByEnum>)Delegate.Remove(KIDManager._onSessionUpdated_Multiplayer, callback);
	}

	public static void RegisterSessionUpdatedCallback_UGC(Action<bool, Permission.ManagedByEnum> callback)
	{
		Debug.Log("[KID] Successfully registered a new callback to SessionUpdate which monitors the UGC permission");
		KIDManager._onSessionUpdated_UGC = (Action<bool, Permission.ManagedByEnum>)Delegate.Combine(KIDManager._onSessionUpdated_UGC, callback);
	}

	public static Task<bool> WaitForAndUpdateNewSession(bool forceRefresh)
	{
		KIDManager.<WaitForAndUpdateNewSession>d__168 <WaitForAndUpdateNewSession>d__;
		<WaitForAndUpdateNewSession>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<WaitForAndUpdateNewSession>d__.forceRefresh = forceRefresh;
		<WaitForAndUpdateNewSession>d__.<>1__state = -1;
		<WaitForAndUpdateNewSession>d__.<>t__builder.Start<KIDManager.<WaitForAndUpdateNewSession>d__168>(ref <WaitForAndUpdateNewSession>d__);
		return <WaitForAndUpdateNewSession>d__.<>t__builder.Task;
	}

	private static bool HasSessionChanged(TMPSession newSession)
	{
		if (newSession == null)
		{
			return false;
		}
		if (KIDManager.CurrentSession == null)
		{
			return true;
		}
		if (!newSession.IsValidSession)
		{
			return false;
		}
		if (newSession.IsDefault)
		{
			Debug.LogError(string.Format("[KID::MANAGER] DEBUG - New Session Is Default! Age: [{0}]", newSession.Age));
			return false;
		}
		return KIDManager.CurrentSession.IsDefault || !newSession.Etag.Equals(KIDManager.CurrentSession.Etag);
	}

	private static void OnSessionUpdated()
	{
		Action onSessionUpdated_AnyPermission = KIDManager._onSessionUpdated_AnyPermission;
		if (onSessionUpdated_AnyPermission != null)
		{
			onSessionUpdated_AnyPermission();
		}
		bool voiceChatEnabled = false;
		bool joinGroupsEnabled = false;
		bool customUsernamesEnabled = false;
		List<Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
		int count = allPermissionsData.Count;
		for (int i = 0; i < count; i++)
		{
			Permission permission = allPermissionsData[i];
			string name = permission.Name;
			if (!(name == "voice-chat"))
			{
				if (!(name == "custom-username"))
				{
					if (!(name == "join-groups"))
					{
						if (!(name == "multiplayer"))
						{
							if (!(name == "mods"))
							{
								Debug.Log("[KID] Tried updating permission with name [" + permission.Name + "] but did not match any of the set cases. Unable to process");
							}
							else if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_UGC = KIDManager._onSessionUpdated_UGC;
								if (onSessionUpdated_UGC != null)
								{
									onSessionUpdated_UGC(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
						}
						else
						{
							if (KIDManager.HasPermissionChanged(permission))
							{
								Action<bool, Permission.ManagedByEnum> onSessionUpdated_Multiplayer = KIDManager._onSessionUpdated_Multiplayer;
								if (onSessionUpdated_Multiplayer != null)
								{
									onSessionUpdated_Multiplayer(permission.Enabled, permission.ManagedBy);
								}
								KIDManager._previousPermissionSettings[permission.Name] = permission;
							}
							bool enabled = permission.Enabled;
						}
					}
					else
					{
						if (KIDManager.HasPermissionChanged(permission))
						{
							Action<bool, Permission.ManagedByEnum> onSessionUpdated_PrivateRooms = KIDManager._onSessionUpdated_PrivateRooms;
							if (onSessionUpdated_PrivateRooms != null)
							{
								onSessionUpdated_PrivateRooms(permission.Enabled, permission.ManagedBy);
							}
							KIDManager._previousPermissionSettings[permission.Name] = permission;
						}
						joinGroupsEnabled = permission.Enabled;
					}
				}
				else
				{
					if (KIDManager.HasPermissionChanged(permission))
					{
						Action<bool, Permission.ManagedByEnum> onSessionUpdated_CustomUsernames = KIDManager._onSessionUpdated_CustomUsernames;
						if (onSessionUpdated_CustomUsernames != null)
						{
							onSessionUpdated_CustomUsernames(permission.Enabled, permission.ManagedBy);
						}
						KIDManager._previousPermissionSettings[permission.Name] = permission;
					}
					customUsernamesEnabled = permission.Enabled;
				}
			}
			else
			{
				if (KIDManager.HasPermissionChanged(permission))
				{
					Action<bool, Permission.ManagedByEnum> onSessionUpdated_VoiceChat = KIDManager._onSessionUpdated_VoiceChat;
					if (onSessionUpdated_VoiceChat != null)
					{
						onSessionUpdated_VoiceChat(permission.Enabled, permission.ManagedBy);
					}
					KIDManager._previousPermissionSettings[permission.Name] = permission;
				}
				voiceChatEnabled = permission.Enabled;
			}
		}
		GorillaTelemetry.PostKidEvent(joinGroupsEnabled, voiceChatEnabled, customUsernamesEnabled, KIDManager.CurrentSession.AgeStatus, GTKidEventType.permission_update);
	}

	private static bool HasPermissionChanged(Permission newValue)
	{
		Permission permission;
		if (KIDManager._previousPermissionSettings.TryGetValue(newValue.Name, out permission))
		{
			return permission.Enabled != newValue.Enabled || permission.ManagedBy != newValue.ManagedBy;
		}
		KIDManager._previousPermissionSettings.Add(newValue.Name, newValue);
		return true;
	}

	public const string MULTIPLAYER_PERMISSION_NAME = "multiplayer";

	public const string UGC_PERMISSION_NAME = "mods";

	public const string PRIVATE_ROOM_PERMISSION_NAME = "join-groups";

	public const string VOICE_CHAT_PERMISSION_NAME = "voice-chat";

	public const string CUSTOM_USERNAME_PERMISSION_NAME = "custom-username";

	public const string PREVIOUS_STATUS_PREF_KEY_PREFIX = "previous-status-";

	public const string KID_DATA_KEY = "KIDData";

	private const string KID_EMAIL_KEY = "k-id_EmailAddress";

	private const int SECONDS_BETWEEN_UPDATE_ATTEMPTS = 30;

	private const string KID_SETUP_FLAG = "KID-Setup-";

	[OnEnterPlay_SetNull]
	private static KIDManager _instance;

	private static string _emailAddress;

	private static CancellationTokenSource _requestCancellationSource = new CancellationTokenSource();

	private static bool _titleDataReady = false;

	private static bool _useKid = false;

	private static int _kIDPhase = 0;

	private static DateTime? _kIDNewPlayerDateTime = null;

	private static string _debugKIDLocalePlayerPrefRef = "KID_SPOOF_LOCALE";

	private static string parentEmailForUserPlayerPrefRef;

	[OnEnterPlay_SetNull]
	private static Action _sessionUpdatedCallback = null;

	[OnEnterPlay_SetNull]
	private static Action _onKIDInitialisationComplete = null;

	public static KIDManager.OnEmailResultReceived onEmailResultReceived;

	private const string KID_GET_SESSION = "GetPlayerData";

	private const string KID_VERIFY_AGE = "VerifyAge";

	private const string KID_UPGRADE_SESSION = "UpgradeSession";

	private const string KID_SEND_CHALLENGE_EMAIL = "SendChallengeEmail";

	private const string KID_ATTEMPT_AGE_UPDATE = "AttemptAgeUpdate";

	private const string KID_APPEAL_AGE = "AppealAge";

	private const string KID_OPT_IN = "OptIn";

	private const string KID_GET_REQUIREMENTS = "GetRequirements";

	private const string KID_SET_CONFIRMED_STATUS = "SetConfirmedStatus";

	private const string KID_SET_OPT_IN_PERMISSIONS = "SetOptInPermissions";

	private const string KID_FORCE_REFRESH = "sessionRefresh";

	private const int MAX_RETRIES_FOR_CRITICAL_KID_SERVER_REQUESTS = 3;

	private const int MAX_RETRIES_FOR_NORMAL_KID_SERVER_REQUESTS = 2;

	public const string KID_PERMISSION__VOICE_CHAT = "voice-chat";

	public const string KID_PERMISSION__CUSTOM_NAMES = "custom-username";

	public const string KID_PERMISSION__PRIVATE_ROOMS = "join-groups";

	public const string KID_PERMISSION__MULTIPLAYER = "multiplayer";

	public const string KID_PERMISSION__UGC = "mods";

	private const float MAX_SESSION_UPDATE_TIME = 600f;

	private const int TIME_BETWEEN_SESSION_UPDATE_ATTEMPTS = 30;

	[OnEnterPlay_SetNull]
	private static Action _onSessionUpdated_AnyPermission;

	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_VoiceChat;

	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_CustomUsernames;

	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_PrivateRooms;

	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_Multiplayer;

	[OnEnterPlay_SetNull]
	private static Action<bool, Permission.ManagedByEnum> _onSessionUpdated_UGC;

	private static bool _isUpdatingNewSession = false;

	[OnEnterPlay_SetNull]
	private static Dictionary<string, Permission> _previousPermissionSettings = new Dictionary<string, Permission>();

	public delegate void OnEmailResultReceived(bool result);
}
