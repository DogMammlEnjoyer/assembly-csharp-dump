using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NativeWebSocket;
using UnityEngine;

public static class MothershipClientApiUnity
{
	public static MothershipSharedSettings GetSharedSettingsObject()
	{
		return Resources.Load<MothershipSharedSettings>("MothershipSharedSettings");
	}

	static MothershipClientApiUnity()
	{
		MothershipSharedSettings sharedSettingsObject = MothershipClientApiUnity.GetSharedSettingsObject();
		MothershipClientApiUnity.MothershipBaseUrl = sharedSettingsObject.BaseUrl;
		MothershipClientApiUnity.TitleId = sharedSettingsObject.TitleId;
		MothershipClientApiUnity.EnvironmentId = sharedSettingsObject.EnvironmentId;
		MothershipClientApiUnity.DeploymentId = sharedSettingsObject.DeploymentId;
		MothershipClientApiUnity.MothershipWebSocketUrl = sharedSettingsObject.WebSocketUrl;
		MothershipClientApiUnity.isEnabled = sharedSettingsObject.Enabled;
		if (MothershipClientApiUnity.isEnabled)
		{
			try
			{
				MothershipClientApiUnity.SessionId = Guid.NewGuid().ToString("D");
				Debug.Log("Mothership session ID is " + MothershipClientApiUnity.SessionId);
				MothershipClientApiUnity.client = new MothershipClientApiClient(MothershipClientApiUnity.MothershipBaseUrl, MothershipClientApiUnity.TitleId, MothershipClientApiUnity.EnvironmentId, MothershipClientApiUnity.DeploymentId, MothershipClientApiUnity.MothershipWebSocketUrl, true, MothershipClientApiUnity.SessionId);
				MothershipClientApiUnity.http = new MothershipHttpClientUnity(MothershipClientApiUnity.client, sharedSettingsObject.RequestLoggingEnabled);
				MothershipClientApiUnity.client.SetHttpRequestDelegate(MothershipClientApiUnity.http);
				MothershipClientApiUnity.auth = new MothershipAuthCallback(MothershipClientApiUnity.client);
				MothershipClientApiUnity.client.SetLoginCompleteDelegate(MothershipClientApiUnity.auth);
				MothershipClientApiUnity.getUserdataCallback = new MothershipGetUserDataCallback(MothershipClientApiUnity.client);
				MothershipClientApiUnity.client.SetGetUserDataCompleteClientDelegateWrapper(MothershipClientApiUnity.getUserdataCallback);
				MothershipClientApiUnity.setUserDataCallback = new MothershipSetUserDataCallback(MothershipClientApiUnity.client);
				MothershipClientApiUnity.client.SetSetUserDataCompleteClientDelegateWrapper(MothershipClientApiUnity.setUserDataCallback);
				MothershipClientApiUnity.createReportCallback = new MothershipCreateReportCallback();
				MothershipClientApiUnity.client.SetCreateReportCompleteClientDelegateWrapper(MothershipClientApiUnity.createReportCallback);
				MothershipClientApiUnity.getUserInventoryCallback = new MothershipGetUserInventoryCallback();
				MothershipClientApiUnity.client.SetGetUserInventoryCompleteClientDelegateWrapper(MothershipClientApiUnity.getUserInventoryCallback);
				MothershipClientApiUnity.getStorefrontCallback = new MothershipGetStorefrontCallback();
				MothershipClientApiUnity.client.SetGetStorefrontCompleteClientDelegateWrapper(MothershipClientApiUnity.getStorefrontCallback);
				MothershipClientApiUnity.purchaseOfferCallback = new MothershipPurchaseOfferCallback();
				MothershipClientApiUnity.client.SetPurchaseCompleteClientDelegateWrapper(MothershipClientApiUnity.purchaseOfferCallback);
				MothershipClientApiUnity.beginQuestCallback = new MothershipBeginQuestCallback();
				MothershipClientApiUnity.client.SetQuestAuthV2BeginRequestCompleteClientDelegateWrapper(MothershipClientApiUnity.beginQuestCallback);
				MothershipClientApiUnity.beginSteamCallback = new MothershipBeginSteamCallback();
				MothershipClientApiUnity.client.SetSteamBeginRequestCompleteClientDelegateWrapper(MothershipClientApiUnity.beginSteamCallback);
				MothershipClientApiUnity.websocket = new MothershipWebSocketWrapper(MothershipClientApiUnity.client);
				MothershipClientApiUnity.client.SetWebSocketDelegate(MothershipClientApiUnity.websocket);
				MothershipClientApiUnity.websocketDispatcher = MothershipWebSocketDispatcher.instance;
				MothershipClientApiUnity.notificationWrapper = new MothershipNotificationsWrapper(new Action<IntPtr>(MothershipClientApiUnity.InvokeOpenNotificationSocket), new Action<NotificationsMessageResponse, IntPtr>(MothershipClientApiUnity.InvokeMessageNotificationSocket), new Action<IntPtr>(MothershipClientApiUnity.InvokeCloseNotificationSocket), new Action<IntPtr>(MothershipClientApiUnity.InvokeErrorNotificationSocket));
				MothershipClientApiUnity.client.SetNotificationsMessageDelegateWrapper(MothershipClientApiUnity.notificationWrapper);
				MothershipClientApiUnity.getProgressionCallback = new MothershipGetPlayerProgressionCallback();
				MothershipClientApiUnity.client.SetGetProgressionTrackValuesForPlayerCompleteClientDelegateWrapper(MothershipClientApiUnity.getProgressionCallback);
				MothershipClientApiUnity.getProgressionTreesCallback = new MothershipGetPlayerProgressionTressCallback();
				MothershipClientApiUnity.client.SetGetProgressionTreesForPlayerCompleteClientDelegateWrapper(MothershipClientApiUnity.getProgressionTreesCallback);
				MothershipClientApiUnity.writeEventsCallback = new MothershipWriteEventsCallback();
				MothershipClientApiUnity.client.SetWriteEventsCompleteClientDelegateWrapper(MothershipClientApiUnity.writeEventsCallback);
				MothershipClientApiUnity.listMothershipTitleDataCallback = new MothershipListTitleDataCallback();
				MothershipClientApiUnity.client.SetListMothershipTitleDataCompleteClientDelegateWrapper(MothershipClientApiUnity.listMothershipTitleDataCallback);
			}
			catch (Exception exception)
			{
				MothershipClientApiUnity.isEnabled = false;
				Debug.LogException(exception);
				Debug.LogError("Mothership Client API initialization failed");
			}
		}
	}

	public static bool IsClientLoggedIn()
	{
		return MothershipClientContext.IsClientLoggedIn();
	}

	public static void ForgetAllCredentials()
	{
		MothershipClientContext.ForgetAllCredentials();
	}

	public static bool IsEnabled()
	{
		return MothershipClientApiUnity.isEnabled;
	}

	public static void Tick(float deltaTime)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to tick Mothership SDK, but Mothership is not enabled!");
			return;
		}
		MothershipClientApiUnity.client.TickRetryQueue(deltaTime);
	}

	public static void SetLanguage(string newLanguage)
	{
		Debug.Log("[MothershipClient] Set language to " + newLanguage);
		MothershipClientApiUnity.client.SetAcceptLanguage(newLanguage);
	}

	public static void SetAuthRefreshedCallback(Action<string> callback)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return;
		}
		MothershipClientApiUnity.authRefreshRequiredCallback = new MothershipAuthRefreshRequiredCallback(callback);
		MothershipClientApiUnity.client.SetAuthRefreshRequiredDelegateWrapper(MothershipClientApiUnity.authRefreshRequiredCallback);
	}

	public static bool LogInWithInsecure1(string Username, string AccountId, Action<LoginResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<LoginResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.LoginWithInsecure1(Username, AccountId, userData);
	}

	public static bool LogInWithQuest(string nonce, string userId, Action<LoginResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<LoginResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.LoginWithQuest(nonce, userId, userData);
	}

	public static bool StartLogInWithQuest(string userId, Action<PlayerQuestBeginLoginV2Response> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<PlayerQuestBeginLoginV2Response>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.BeginQuestV2Auth(userId, userData);
	}

	public static bool CompleteLogInWithQuest(string userId, string attestationToken, string nonce, Action<LoginResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<LoginResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.CompleteQuestV2Auth(userId, attestationToken, nonce, userData);
	}

	public static bool LogInWithRift(string nonce, string userId, Action<LoginResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<LoginResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.LoginWithRift(nonce, userId, userData);
	}

	public static bool LogInWithGoogle(string token, string userId, Action<LoginResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<LoginResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.LoginWithGoogle(token, userId, userData);
	}

	public static bool LogInWithApple(string signature, string gamePlayerId, string teamPlayerId, string certUri, string salt, string timestamp, Action<LoginResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<LoginResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.LoginWithApple(signature, gamePlayerId, teamPlayerId, certUri, salt, timestamp, userData);
	}

	public static bool StartLoginWithSteam(Action<PlayerSteamBeginLoginResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<PlayerSteamBeginLoginResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.BeginSteamAuth(userData);
	}

	public static bool CompleteLoginWithSteam(string nonce, string steamTicket, Action<LoginResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled)
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<LoginResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.CompleteSteamAuth(nonce, steamTicket, userData);
	}

	public static bool GetUserDataValue(string keyName, Action<MothershipUserData> successAction, Action<MothershipError, int> errorAction, string targetId = "")
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled or the user hasn't ever logged in");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<MothershipUserData>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		string userId = string.IsNullOrEmpty(targetId) ? MothershipClientContext.MothershipId : targetId;
		return MothershipClientApiUnity.client.GetUserData(MothershipClientContext.MothershipId, userId, keyName, "", userData);
	}

	public static bool SetUserDataValue(string keyName, string value, Action<SetUserDataResponse> successAction, Action<MothershipError, int> errorAction, string targetId = "")
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled or the user hasn't ever logged in");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<SetUserDataResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		string userId = string.IsNullOrEmpty(targetId) ? MothershipClientContext.MothershipId : targetId;
		return MothershipClientApiUnity.client.SetUserData(MothershipClientContext.MothershipId, userId, keyName, value, -1, userData);
	}

	public static bool CreateReport(string reportedUserId, int category, bool moddedClient, string metadata, Action<CreateReportResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled or the user hasn't ever logged in");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<CreateReportResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		string platform = "STEAM";
		return MothershipClientApiUnity.client.CreateReport(MothershipClientContext.MothershipId, reportedUserId, category, platform, moddedClient, metadata, userData);
	}

	public static bool GetUserInventory(Action<MothershipGetInventoryResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled or the user hasn't ever logged in");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<MothershipGetInventoryResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.GetUserInventory(MothershipClientContext.MothershipId, userData);
	}

	public static bool GetStorefront(string[] offerDisplays, Action<MothershipGetStorefrontResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled or the user hasn't ever logged in");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<MothershipGetStorefrontResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.GetStorefront(MothershipClientContext.MothershipId, new StringVector(offerDisplays), userData);
	}

	public static bool PurchaseOffer(string offerDisplayId, string offerId, int displayIndex, Action<MothershipPurchaseOfferResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled or the user hasn't ever logged in");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<MothershipPurchaseOfferResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.PurchaseOffer(MothershipClientContext.MothershipId, offerDisplayId, offerId, displayIndex, userData);
	}

	[NativeInteger]
	[NativeInteger]
	public static event Action<IntPtr> OnOpenNotificationSocket;

	[NativeInteger]
	[NativeInteger]
	public static event Action<NotificationsMessageResponse, IntPtr> OnMessageNotificationSocket;

	[NativeInteger]
	[NativeInteger]
	public static event Action<IntPtr> OnCloseNotificationSocket;

	[NativeInteger]
	[NativeInteger]
	public static event Action<IntPtr> OnErrorNotificationSocket;

	public static bool OpenNotificationsSocket()
	{
		if (MothershipClientApiUnity.notificationWrapper.SocketState == WebSocketState.Open)
		{
			MothershipClientApiUnity.websocket.RefreshClientTokenHeaders();
			return false;
		}
		return MothershipClientApiUnity.client.OpenNotificationsSocket(MothershipClientContext.MothershipId, IntPtr.Zero);
	}

	public static void CloseWebSockets()
	{
		MothershipWebSocketWrapper mothershipWebSocketWrapper = MothershipClientApiUnity.websocket;
		if (mothershipWebSocketWrapper == null)
		{
			return;
		}
		mothershipWebSocketWrapper.CloseConnections();
	}

	public static void TickWebSockets(float deltaTime)
	{
		MothershipClientApiUnity.websocket.TickWebSockets(deltaTime);
	}

	private static void InvokeOpenNotificationSocket([NativeInteger] IntPtr userData)
	{
		Action<IntPtr> onOpenNotificationSocket = MothershipClientApiUnity.OnOpenNotificationSocket;
		if (onOpenNotificationSocket != null)
		{
			onOpenNotificationSocket(userData);
		}
		Debug.Log("NOTIFICATIONS opened");
	}

	private static void InvokeMessageNotificationSocket(NotificationsMessageResponse notification, [NativeInteger] IntPtr userData)
	{
		Action<NotificationsMessageResponse, IntPtr> onMessageNotificationSocket = MothershipClientApiUnity.OnMessageNotificationSocket;
		if (onMessageNotificationSocket != null)
		{
			onMessageNotificationSocket(notification, userData);
		}
		Debug.Log("NOTIFICATIONS messaged \n" + notification.Title + "\n" + notification.Body);
	}

	private static void InvokeCloseNotificationSocket([NativeInteger] IntPtr userData)
	{
		Action<IntPtr> onCloseNotificationSocket = MothershipClientApiUnity.OnCloseNotificationSocket;
		if (onCloseNotificationSocket != null)
		{
			onCloseNotificationSocket(userData);
		}
		Debug.Log("NOTIFICATIONS closed");
	}

	private static void InvokeErrorNotificationSocket([NativeInteger] IntPtr userData)
	{
		Action<IntPtr> onErrorNotificationSocket = MothershipClientApiUnity.OnErrorNotificationSocket;
		if (onErrorNotificationSocket != null)
		{
			onErrorNotificationSocket(userData);
		}
		Debug.Log("NOTIFICATIONS errored");
	}

	public static bool GetPlayerProgressionData(Action<GetProgressionTrackValuesForPlayerResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled or the user hasn't ever logged in");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<GetProgressionTrackValuesForPlayerResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.GetProgressionTrackValuesForPlayer(MothershipClientContext.MothershipId, userData);
	}

	public static bool GetPlayerProgressionTreesData(Action<GetProgressionTreesForPlayerResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled or the user hasn't ever logged in");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<GetProgressionTreesForPlayerResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.GetProgressionTreesForPlayer(MothershipClientContext.MothershipId, userData);
	}

	public static bool WriteEvents(string callerId, MothershipWriteEventsRequest req, Action<MothershipWriteEventsResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled or the user hasn't ever logged in");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<MothershipWriteEventsResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		return MothershipClientApiUnity.client.WriteEvents(callerId, req, userData);
	}

	public static bool ListMothershipTitleData(string titleId, string envId, string deploymentId, StringVector keys, Action<ListClientMothershipTitleDataResponse> successAction, Action<MothershipError, int> errorAction)
	{
		if (!MothershipClientApiUnity.isEnabled || !MothershipClientContext.IsClientLoggedIn())
		{
			Debug.LogError("Tried to call a Mothership API, but Mothership is not enabled");
			return false;
		}
		IntPtr userData = (IntPtr)GCHandle.Alloc(new CallbackPair<ListClientMothershipTitleDataResponse>
		{
			successCallback = successAction,
			errorCallback = errorAction
		});
		Debug.Log(string.Format("ListMothershipTitleData: {0}, {1}, {2}, {3}", new object[]
		{
			MothershipClientApiUnity.TitleId,
			MothershipClientApiUnity.EnvironmentId,
			MothershipClientApiUnity.DeploymentId,
			keys
		}));
		return MothershipClientApiUnity.client.ListClientMothershipTitleData(MothershipClientContext.MothershipId, keys, userData);
	}

	private static bool isEnabled;

	public static string MothershipBaseUrl = "";

	public static string TitleId = "";

	public static string EnvironmentId = "";

	public static string DeploymentId = "";

	public static string MothershipWebSocketUrl = "";

	public static string SessionId = "";

	private static MothershipClientApiClient client;

	private static MothershipHttpClientUnity http;

	private static MothershipWebSocketWrapper websocket;

	private static MothershipWebSocketDispatcher websocketDispatcher;

	private static LoginCompleteDelegateWrapper auth;

	private static MothershipAuthRefreshRequiredCallback authRefreshRequiredCallback;

	private static MothershipBeginQuestCallback beginQuestCallback;

	private static MothershipBeginSteamCallback beginSteamCallback;

	private static MothershipGetUserDataCallback getUserdataCallback;

	private static MothershipSetUserDataCallback setUserDataCallback;

	private static MothershipCreateReportCallback createReportCallback;

	private static MothershipGetUserInventoryCallback getUserInventoryCallback;

	private static MothershipGetStorefrontCallback getStorefrontCallback;

	private static MothershipPurchaseOfferCallback purchaseOfferCallback;

	private static MothershipNotificationsWrapper notificationWrapper;

	private static MothershipGetPlayerProgressionCallback getProgressionCallback;

	private static MothershipGetPlayerProgressionTressCallback getProgressionTreesCallback;

	private static MothershipWriteEventsCallback writeEventsCallback;

	private static MothershipListTitleDataCallback listMothershipTitleDataCallback;
}
