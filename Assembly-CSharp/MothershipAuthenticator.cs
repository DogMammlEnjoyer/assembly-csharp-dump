using System;
using GorillaExtensions;
using Steamworks;
using UnityEngine;

public class MothershipAuthenticator : MonoBehaviour, IGorillaSliceableSimple
{
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = null;
		}
	}

	public void Awake()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = this;
		}
		else if (MothershipAuthenticator.Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		if (!MothershipClientApiUnity.IsEnabled())
		{
			Debug.Log("Mothership is not enabled.");
			return;
		}
		if (MothershipAuthenticator.Instance.SteamAuthenticator == null)
		{
			MothershipAuthenticator.Instance.SteamAuthenticator = MothershipAuthenticator.Instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
		}
		MothershipClientApiUnity.SetAuthRefreshedCallback(delegate(string id)
		{
			this.BeginLoginFlow();
		});
	}

	public void BeginLoginFlow()
	{
		Debug.Log("making login call");
		this.LogInWithSteam();
	}

	private void LogInWithInsecure()
	{
		MothershipClientApiUnity.LogInWithInsecure1(this.TestNickname, this.TestAccountId, delegate(LoginResponse LoginResponse)
		{
			Debug.Log("Logged in with Mothership Id " + LoginResponse.MothershipPlayerId);
			MothershipClientApiUnity.OpenNotificationsSocket();
			Action onLoginSuccess = this.OnLoginSuccess;
			if (onLoginSuccess == null)
			{
				return;
			}
			onLoginSuccess();
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.LogError(string.Format("Failed to log in, error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
			{
				MothershipError.Message,
				MothershipError.TraceId,
				errorCode,
				MothershipError.MothershipErrorCode
			}));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure(1);
			}
			Action<string, string, string> onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure(MothershipError.Message, MothershipError.MothershipErrorCode, MothershipError.TraceId);
		});
	}

	private void LogInWithSteam()
	{
		MothershipClientApiUnity.StartLoginWithSteam(delegate(PlayerSteamBeginLoginResponse resp)
		{
			string nonce = resp.Nonce;
			SteamAuthTicket ticketHandle = HAuthTicket.Invalid;
			Action<LoginResponse> <>9__4;
			Action<MothershipError, int> <>9__5;
			ticketHandle = this.SteamAuthenticator.GetAuthTicketForWebApi(nonce, delegate(string ticket)
			{
				string nonce = nonce;
				Action<LoginResponse> successAction;
				if ((successAction = <>9__4) == null)
				{
					successAction = (<>9__4 = delegate(LoginResponse successResp)
					{
						ticketHandle.Dispose();
						Debug.Log("Logged in to Mothership with Steam");
						MothershipClientApiUnity.OpenNotificationsSocket();
						Action onLoginSuccess = this.OnLoginSuccess;
						if (onLoginSuccess == null)
						{
							return;
						}
						onLoginSuccess();
					});
				}
				Action<MothershipError, int> errorAction;
				if ((errorAction = <>9__5) == null)
				{
					errorAction = (<>9__5 = delegate(MothershipError MothershipError, int errorCode)
					{
						ticketHandle.Dispose();
						Debug.LogError(string.Format("Couldn't log into Mothership with Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
						{
							MothershipError.Message,
							MothershipError.TraceId,
							errorCode,
							MothershipError.MothershipErrorCode
						}));
						Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
						if (onLoginAttemptFailure != null)
						{
							onLoginAttemptFailure(1);
						}
						Action<string, string, string> onLoginFailure = this.OnLoginFailure;
						if (onLoginFailure == null)
						{
							return;
						}
						onLoginFailure(MothershipError.Message, MothershipError.MothershipErrorCode, MothershipError.TraceId);
					});
				}
				MothershipClientApiUnity.CompleteLoginWithSteam(nonce, ticket, successAction, errorAction);
			}, delegate(EResult error)
			{
				string text = string.Format("Couldn't get an auth ticket for logging into Mothership with Steam: {0}", error);
				Debug.LogError(text);
				Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
				if (onLoginAttemptFailure != null)
				{
					onLoginAttemptFailure(1);
				}
				Action<string, string, string> onLoginFailure = this.OnLoginFailure;
				if (onLoginFailure == null)
				{
					return;
				}
				onLoginFailure(text, "", "");
			});
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.LogError(string.Format("Couldn't start Mothership auth for Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
			{
				MothershipError.Message,
				MothershipError.TraceId,
				errorCode,
				MothershipError.MothershipErrorCode
			}));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure(1);
			}
			Action<string, string, string> onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure(MothershipError.Message, MothershipError.MothershipErrorCode, MothershipError.TraceId);
		});
	}

	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	public void SliceUpdate()
	{
		if (MothershipClientApiUnity.IsEnabled())
		{
			MothershipClientApiUnity.Tick(Time.deltaTime);
		}
	}

	public static volatile MothershipAuthenticator Instance;

	public MetaAuthenticator MetaAuthenticator;

	public SteamAuthenticator SteamAuthenticator;

	public string TestNickname;

	public string TestAccountId;

	public bool UseConstantTestAccountId;

	public int MaxMetaLoginAttempts = 5;

	public Action OnLoginSuccess;

	public Action<string, string, string> OnLoginFailure;

	public Action<int> OnLoginAttemptFailure;
}
