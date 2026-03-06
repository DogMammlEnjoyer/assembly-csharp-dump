using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PlayFab.AuthenticationModels;
using PlayFab.ClientModels;
using PlayFab.Public;
using PlayFab.SharedModels;
using UnityEngine;

namespace PlayFab.Internal
{
	public class PlayFabHttp : SingletonMonoBehaviour<PlayFabHttp>
	{
		public static event PlayFabHttp.ApiProcessingEvent<ApiProcessingEventArgs> ApiProcessingEventHandler;

		public static event PlayFabHttp.ApiProcessErrorEvent ApiProcessingErrorEventHandler;

		public static int GetPendingMessages()
		{
			ITransportPlugin plugin = PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport, "");
			if (!plugin.IsInitialized)
			{
				return 0;
			}
			return plugin.GetPendingMessages();
		}

		public static void InitializeHttp()
		{
			if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
			{
				throw new PlayFabException(PlayFabExceptionCode.TitleNotSet, "You must set PlayFabSettings.TitleId before making API Calls.");
			}
			ITransportPlugin plugin = PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport, "");
			if (plugin.IsInitialized)
			{
				return;
			}
			plugin.Initialize();
			SingletonMonoBehaviour<PlayFabHttp>.CreateInstance();
		}

		public static void InitializeLogger(IPlayFabLogger setLogger = null)
		{
			if (PlayFabHttp._logger != null)
			{
				throw new InvalidOperationException("Once initialized, the logger cannot be reset.");
			}
			if (setLogger == null)
			{
				setLogger = new PlayFabLogger();
			}
			PlayFabHttp._logger = setLogger;
		}

		public static void InitializeScreenTimeTracker(string entityId, string entityType, string playFabUserId)
		{
			PlayFabHttp.screenTimeTracker.ClientSessionStart(entityId, entityType, playFabUserId);
			SingletonMonoBehaviour<PlayFabHttp>.instance.StartCoroutine(PlayFabHttp.SendScreenTimeEvents(5f));
		}

		private static IEnumerator SendScreenTimeEvents(float secondsBetweenBatches)
		{
			WaitForSeconds delay = new WaitForSeconds(secondsBetweenBatches);
			while (!PlayFabSettings.DisableFocusTimeCollection)
			{
				PlayFabHttp.screenTimeTracker.Send();
				yield return delay;
			}
			yield break;
		}

		public static void SimpleGetCall(string fullUrl, Action<byte[]> successCallback, Action<string> errorCallback)
		{
			PlayFabHttp.InitializeHttp();
			PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport, "").SimpleGetCall(fullUrl, successCallback, errorCallback);
		}

		public static void SimplePutCall(string fullUrl, byte[] payload, Action<byte[]> successCallback, Action<string> errorCallback)
		{
			PlayFabHttp.InitializeHttp();
			PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport, "").SimplePutCall(fullUrl, payload, successCallback, errorCallback);
		}

		public static void SimplePostCall(string fullUrl, byte[] payload, Action<byte[]> successCallback, Action<string> errorCallback)
		{
			PlayFabHttp.InitializeHttp();
			PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport, "").SimplePostCall(fullUrl, payload, successCallback, errorCallback);
		}

		protected internal static void MakeApiCall<TResult>(string apiEndpoint, PlayFabRequestCommon request, AuthType authType, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null, PlayFabAuthenticationContext authenticationContext = null, PlayFabApiSettings apiSettings = null, IPlayFabInstanceApi instanceApi = null) where TResult : PlayFabResultCommon
		{
			apiSettings = (apiSettings ?? PlayFabSettings.staticSettings);
			string fullUrl = apiSettings.GetFullUrl(apiEndpoint, apiSettings.RequestGetParams);
			PlayFabHttp._MakeApiCall<TResult>(apiEndpoint, fullUrl, request, authType, resultCallback, errorCallback, customData, extraHeaders, false, authenticationContext, apiSettings, instanceApi);
		}

		protected internal static void MakeApiCallWithFullUri<TResult>(string fullUri, PlayFabRequestCommon request, AuthType authType, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null, PlayFabAuthenticationContext authenticationContext = null, PlayFabApiSettings apiSettings = null, IPlayFabInstanceApi instanceApi = null) where TResult : PlayFabResultCommon
		{
			apiSettings = (apiSettings ?? PlayFabSettings.staticSettings);
			PlayFabHttp._MakeApiCall<TResult>(null, fullUri, request, authType, resultCallback, errorCallback, customData, extraHeaders, false, authenticationContext, apiSettings, instanceApi);
		}

		private static void _MakeApiCall<TResult>(string apiEndpoint, string fullUrl, PlayFabRequestCommon request, AuthType authType, Action<TResult> resultCallback, Action<PlayFabError> errorCallback, object customData, Dictionary<string, string> extraHeaders, bool allowQueueing, PlayFabAuthenticationContext authenticationContext, PlayFabApiSettings apiSettings, IPlayFabInstanceApi instanceApi) where TResult : PlayFabResultCommon
		{
			PlayFabHttp.InitializeHttp();
			PlayFabHttp.SendEvent(apiEndpoint, request, null, ApiProcessingEventType.Pre);
			ISerializerPlugin serializer = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer, "");
			CallRequestContainer reqContainer = new CallRequestContainer
			{
				ApiEndpoint = apiEndpoint,
				FullUrl = fullUrl,
				settings = apiSettings,
				context = authenticationContext,
				CustomData = customData,
				Payload = Encoding.UTF8.GetBytes(serializer.SerializeObject(request)),
				ApiRequest = request,
				ErrorCallback = errorCallback,
				RequestHeaders = (extraHeaders ?? new Dictionary<string, string>()),
				instanceApi = instanceApi
			};
			foreach (KeyValuePair<string, string> keyValuePair in PlayFabHttp.GlobalHeaderInjection)
			{
				if (!reqContainer.RequestHeaders.ContainsKey(keyValuePair.Key))
				{
					reqContainer.RequestHeaders[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			ITransportPlugin plugin = PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport, "");
			reqContainer.RequestHeaders["X-ReportErrorAsSuccess"] = "true";
			reqContainer.RequestHeaders["X-PlayFabSDK"] = "UnitySDK-2.87.200602";
			if (authType != AuthType.LoginSession)
			{
				if (authType == AuthType.EntityToken)
				{
					if (authenticationContext != null)
					{
						reqContainer.RequestHeaders["X-EntityToken"] = authenticationContext.EntityToken;
					}
				}
			}
			else if (authenticationContext != null)
			{
				reqContainer.RequestHeaders["X-Authorization"] = authenticationContext.ClientSessionTicket;
			}
			reqContainer.DeserializeResultJson = delegate()
			{
				reqContainer.ApiResult = serializer.DeserializeObject<TResult>(reqContainer.JsonResponse);
			};
			reqContainer.InvokeSuccessCallback = delegate()
			{
				if (resultCallback != null)
				{
					resultCallback((TResult)((object)reqContainer.ApiResult));
				}
			};
			if (allowQueueing && PlayFabHttp._apiCallQueue != null)
			{
				for (int i = PlayFabHttp._apiCallQueue.Count - 1; i >= 0; i--)
				{
					if (PlayFabHttp._apiCallQueue[i].ApiEndpoint == apiEndpoint)
					{
						PlayFabHttp._apiCallQueue.RemoveAt(i);
					}
				}
				PlayFabHttp._apiCallQueue.Add(reqContainer);
				return;
			}
			plugin.MakeApiCall(reqContainer);
		}

		internal void OnPlayFabApiResult(CallRequestContainer reqContainer)
		{
			PlayFabResultCommon apiResult = reqContainer.ApiResult;
			GetEntityTokenResponse getEntityTokenResponse = apiResult as GetEntityTokenResponse;
			if (getEntityTokenResponse != null)
			{
				PlayFabSettings.staticPlayer.EntityToken = getEntityTokenResponse.EntityToken;
			}
			LoginResult loginResult = apiResult as LoginResult;
			RegisterPlayFabUserResult registerPlayFabUserResult = apiResult as RegisterPlayFabUserResult;
			if (loginResult != null)
			{
				loginResult.AuthenticationContext = new PlayFabAuthenticationContext(loginResult.SessionTicket, loginResult.EntityToken.EntityToken, loginResult.PlayFabId, loginResult.EntityToken.Entity.Id, loginResult.EntityToken.Entity.Type);
				if (reqContainer.context != null)
				{
					reqContainer.context.CopyFrom(loginResult.AuthenticationContext);
					return;
				}
			}
			else if (registerPlayFabUserResult != null)
			{
				registerPlayFabUserResult.AuthenticationContext = new PlayFabAuthenticationContext(registerPlayFabUserResult.SessionTicket, registerPlayFabUserResult.EntityToken.EntityToken, registerPlayFabUserResult.PlayFabId, registerPlayFabUserResult.EntityToken.Entity.Id, registerPlayFabUserResult.EntityToken.Entity.Type);
				if (reqContainer.context != null)
				{
					reqContainer.context.CopyFrom(registerPlayFabUserResult.AuthenticationContext);
				}
			}
		}

		private void OnEnable()
		{
			if (PlayFabHttp._logger != null)
			{
				PlayFabHttp._logger.OnEnable();
			}
			if (PlayFabHttp.screenTimeTracker != null && !PlayFabSettings.DisableFocusTimeCollection)
			{
				PlayFabHttp.screenTimeTracker.OnEnable();
			}
		}

		private void OnDisable()
		{
			if (PlayFabHttp._logger != null)
			{
				PlayFabHttp._logger.OnDisable();
			}
			if (PlayFabHttp.screenTimeTracker != null && !PlayFabSettings.DisableFocusTimeCollection)
			{
				PlayFabHttp.screenTimeTracker.OnDisable();
			}
		}

		private void OnDestroy()
		{
			ITransportPlugin plugin = PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport, "");
			if (plugin.IsInitialized)
			{
				plugin.OnDestroy();
			}
			if (PlayFabHttp._logger != null)
			{
				PlayFabHttp._logger.OnDestroy();
			}
			if (PlayFabHttp.screenTimeTracker != null && !PlayFabSettings.DisableFocusTimeCollection)
			{
				PlayFabHttp.screenTimeTracker.OnDestroy();
			}
		}

		public void OnApplicationFocus(bool isFocused)
		{
			if (PlayFabHttp.screenTimeTracker != null && !PlayFabSettings.DisableFocusTimeCollection)
			{
				PlayFabHttp.screenTimeTracker.OnApplicationFocus(isFocused);
			}
		}

		public void OnApplicationQuit()
		{
			if (PlayFabHttp.screenTimeTracker != null && !PlayFabSettings.DisableFocusTimeCollection)
			{
				PlayFabHttp.screenTimeTracker.OnApplicationQuit();
			}
		}

		private void Update()
		{
			ITransportPlugin plugin = PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport, "");
			if (plugin.IsInitialized)
			{
				if (PlayFabHttp._apiCallQueue != null)
				{
					foreach (CallRequestContainer reqContainer in PlayFabHttp._apiCallQueue)
					{
						plugin.MakeApiCall(reqContainer);
					}
					PlayFabHttp._apiCallQueue = null;
				}
				plugin.Update();
			}
			while (this._injectedCoroutines.Count > 0)
			{
				base.StartCoroutine(this._injectedCoroutines.Dequeue());
			}
			while (this._injectedAction.Count > 0)
			{
				Action action = this._injectedAction.Dequeue();
				if (action != null)
				{
					action();
				}
			}
		}

		protected internal static PlayFabError GeneratePlayFabError(string apiEndpoint, string json, object customData)
		{
			Dictionary<string, object> dictionary = null;
			Dictionary<string, List<string>> errorDetails = null;
			ISerializerPlugin plugin = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer, "");
			try
			{
				dictionary = plugin.DeserializeObject<Dictionary<string, object>>(json);
			}
			catch (Exception)
			{
			}
			try
			{
				object obj;
				if (dictionary != null && dictionary.TryGetValue("errorDetails", out obj))
				{
					errorDetails = plugin.DeserializeObject<Dictionary<string, List<string>>>(obj.ToString());
				}
			}
			catch (Exception)
			{
			}
			return new PlayFabError
			{
				ApiEndpoint = apiEndpoint,
				HttpCode = ((dictionary != null && dictionary.ContainsKey("code")) ? Convert.ToInt32(dictionary["code"]) : 400),
				HttpStatus = ((dictionary != null && dictionary.ContainsKey("status")) ? ((string)dictionary["status"]) : "BadRequest"),
				Error = (PlayFabErrorCode)((dictionary != null && dictionary.ContainsKey("errorCode")) ? Convert.ToInt32(dictionary["errorCode"]) : 1123),
				ErrorMessage = ((dictionary != null && dictionary.ContainsKey("errorMessage")) ? ((string)dictionary["errorMessage"]) : json),
				ErrorDetails = errorDetails,
				CustomData = customData
			};
		}

		protected internal static void SendErrorEvent(PlayFabRequestCommon request, PlayFabError error)
		{
			if (PlayFabHttp.ApiProcessingErrorEventHandler == null)
			{
				return;
			}
			try
			{
				PlayFabHttp.ApiProcessingErrorEventHandler(request, error);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		protected internal static void SendEvent(string apiEndpoint, PlayFabRequestCommon request, PlayFabResultCommon result, ApiProcessingEventType eventType)
		{
			if (PlayFabHttp.ApiProcessingEventHandler == null)
			{
				return;
			}
			try
			{
				PlayFabHttp.ApiProcessingEventHandler(new ApiProcessingEventArgs
				{
					ApiEndpoint = apiEndpoint,
					EventType = eventType,
					Request = request,
					Result = result
				});
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void ClearAllEvents()
		{
			PlayFabHttp.ApiProcessingEventHandler = null;
			PlayFabHttp.ApiProcessingErrorEventHandler = null;
		}

		public void InjectInUnityThread(IEnumerator x)
		{
			this._injectedCoroutines.Enqueue(x);
		}

		public void InjectInUnityThread(Action action)
		{
			this._injectedAction.Enqueue(action);
		}

		private static List<CallRequestContainer> _apiCallQueue = new List<CallRequestContainer>();

		public static readonly Dictionary<string, string> GlobalHeaderInjection = new Dictionary<string, string>();

		private static IPlayFabLogger _logger;

		private static IScreenTimeTracker screenTimeTracker = new ScreenTimeTracker();

		private const float delayBetweenBatches = 5f;

		private readonly Queue<IEnumerator> _injectedCoroutines = new Queue<IEnumerator>();

		private readonly Queue<Action> _injectedAction = new Queue<Action>();

		public delegate void ApiProcessingEvent<in TEventArgs>(TEventArgs e);

		public delegate void ApiProcessErrorEvent(PlayFabRequestCommon request, PlayFabError error);
	}
}
