using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.WitAi.Attributes;
using Meta.WitAi.Composer.Data;
using Meta.WitAi.Composer.Integrations;
using Meta.WitAi.Composer.Interfaces;
using Meta.WitAi.Configuration;
using Meta.WitAi.Events;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Meta.WitAi.Composer
{
	public abstract class ComposerService : MonoBehaviour, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Composer, null);

		public ConcurrentDictionary<string, IComposerSession> ActiveSessions { get; } = new ConcurrentDictionary<string, IComposerSession>();

		public string LastSessionId { get; private set; }

		public ComposerContextMap CurrentContextMap { get; } = new ComposerContextMap();

		public bool RouteVoiceServiceToComposer
		{
			get
			{
				return this._routeVoiceServiceToComposer;
			}
			set
			{
				this._routeVoiceServiceToComposer = value;
				ComposerActiveEvent onComposerActiveChange = this.Events.OnComposerActiveChange;
				if (onComposerActiveChange == null)
				{
					return;
				}
				onComposerActiveChange.Invoke(this, value);
			}
		}

		public VoiceService VoiceService
		{
			get
			{
				return this._voiceService;
			}
		}

		public bool IsComposerActive
		{
			get
			{
				return this._requests.Count > 0;
			}
		}

		public ComposerEvents Events
		{
			get
			{
				return this._events;
			}
		}

		public ContextEvents ContextMapEvents
		{
			get
			{
				return this._contextEvents;
			}
		}

		protected abstract IComposerRequestHandler GetRequestHandler();

		public IComposerSessionProvider SessionProvider
		{
			get
			{
				return this._sessionProvider as IComposerSessionProvider;
			}
			set
			{
				Object @object = value as Object;
				if (@object != null)
				{
					this._sessionProvider = @object;
					return;
				}
				if (value != null)
				{
					this.Logger.Error("Set invalid IComposerSessionProvider type ({0})\nReason: Must inherit from {1}", new object[]
					{
						((value != null) ? value.GetType().Name : null) ?? "Null",
						"Object"
					});
					throw new ArgumentException("Set invalid IComposerSessionProvider type");
				}
				this._sessionProvider = null;
			}
		}

		[Obsolete("Use 'LastSessionId' or 'ActiveSessions' instead.")]
		public string SessionID
		{
			get
			{
				return this.LastSessionId;
			}
		}

		[Obsolete("Use 'ActiveSessions' instead.")]
		public DateTime SessionStart
		{
			get
			{
				IComposerSession composerSession;
				if (this.ActiveSessions.TryGetValue(this.LastSessionId, out composerSession))
				{
					return composerSession.SessionStart;
				}
				return DateTime.MinValue;
			}
		}

		[Obsolete("Use 'ActiveSessions' instead.")]
		public TimeSpan SessionElapsed
		{
			get
			{
				return this.SessionStart - DateTime.UtcNow;
			}
		}

		protected virtual void Awake()
		{
			if (this._voiceService == null)
			{
				this._voiceService = base.gameObject.GetComponentInChildren<VoiceService>();
				if (this._voiceService == null)
				{
					this.Log("No Voice Service found", true);
				}
			}
			if (this._speechHandlers == null)
			{
				this._speechHandlers = base.gameObject.GetComponentsInChildren<IComposerSpeechHandler>();
			}
			if (this._actionHandler == null)
			{
				this._actionHandler = base.gameObject.GetComponentInChildren<IComposerActionHandler>();
			}
		}

		protected virtual void OnEnable()
		{
			this._enabled = true;
			if (this._voiceService != null)
			{
				VoiceEvents voiceEvents = this._voiceService.VoiceEvents;
				voiceEvents.OnRequestFinalize = (Action<VoiceServiceRequest>)Delegate.Combine(voiceEvents.OnRequestFinalize, new Action<VoiceServiceRequest>(this.RouteVoiceServiceActivation));
			}
			this.CurrentContextMap.OnContextMapValueChanged.AddListener(new UnityAction<string, string, string>(this.RaiseOnContextMapValueChanged));
			this.CurrentContextMap.OnContextMapValueRemoved.AddListener(new UnityAction<string>(this.RaiseOnContextMapValueRemoved));
		}

		protected virtual void OnDisable()
		{
			this._enabled = false;
			if (this._voiceService != null)
			{
				VoiceEvents voiceEvents = this._voiceService.VoiceEvents;
				voiceEvents.OnRequestFinalize = (Action<VoiceServiceRequest>)Delegate.Remove(voiceEvents.OnRequestFinalize, new Action<VoiceServiceRequest>(this.RouteVoiceServiceActivation));
			}
			this.CurrentContextMap.OnContextMapValueChanged.RemoveListener(new UnityAction<string, string, string>(this.RaiseOnContextMapValueChanged));
			this.CurrentContextMap.OnContextMapValueRemoved.RemoveListener(new UnityAction<string>(this.RaiseOnContextMapValueRemoved));
		}

		protected virtual void OnDestroy()
		{
		}

		private void LogState(string state, VoiceServiceRequest request)
		{
			if (this.debug)
			{
				ICoreLogger logger = this.Logger;
				string message = "{0} [{1}]";
				object obj;
				if (request == null)
				{
					obj = null;
				}
				else
				{
					WitRequestOptions options = request.Options;
					obj = ((options != null) ? options.RequestId : null);
				}
				logger.Verbose(message, state, obj ?? "Unknown ID", null, null, "LogState", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Features\\Composer\\Composer\\Lib\\Wit.ai\\Features\\composer\\Scripts\\Runtime\\ComposerService.cs", 343);
			}
		}

		protected void Log(string comment, bool error = false)
		{
			if (!error && !this.debug)
			{
				return;
			}
			if (error)
			{
				this.Logger.Error("{0}\nScript: {1}\nGameObject: {2}\nActive Sessions: {3}", new object[]
				{
					comment,
					base.GetType().Name,
					(base.gameObject == null) ? "Null" : base.gameObject.name,
					this.ActiveSessions.Count
				});
				return;
			}
			if (this.debug)
			{
				this.Logger.Verbose("{0}\nScript: {1}\nGameObject: {2}\nActive Sessions: {3}", comment, base.GetType().Name, (base.gameObject == null) ? "Null" : base.gameObject.name, this.ActiveSessions.Count, "Log", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Features\\Composer\\Composer\\Lib\\Wit.ai\\Features\\composer\\Scripts\\Runtime\\ComposerService.cs", 364);
			}
		}

		public bool IsSessionActive(string sessionId)
		{
			return !string.IsNullOrEmpty(sessionId) && this.ActiveSessions.ContainsKey(sessionId);
		}

		public IComposerSession StartSession(string sessionId = null)
		{
			if (string.IsNullOrEmpty(sessionId))
			{
				sessionId = this.GetDefaultSessionID();
			}
			IComposerSession composerSession;
			if (this.ActiveSessions.TryGetValue(sessionId, out composerSession))
			{
				return composerSession;
			}
			if (this.EndLastSessionOnStart && !string.IsNullOrEmpty(this.LastSessionId))
			{
				this.EndSession(this.LastSessionId);
			}
			this.Logger.Verbose("Start Composer Session\nSession Id: {0}", sessionId, null, null, null, "StartSession", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Features\\Composer\\Composer\\Lib\\Wit.ai\\Features\\composer\\Scripts\\Runtime\\ComposerService.cs", 405);
			composerSession = this.GenerateSession(sessionId);
			this.ActiveSessions[sessionId] = composerSession;
			this.LastSessionId = sessionId;
			composerSession.StartSession();
			ComposerEvents events = this.Events;
			if (events != null)
			{
				ComposerSessionEvent onComposerSessionBegin = events.OnComposerSessionBegin;
				if (onComposerSessionBegin != null)
				{
					onComposerSessionBegin.Invoke(this.GetSessionData(composerSession));
				}
			}
			return composerSession;
		}

		public bool EndSession(string sessionId)
		{
			if (string.IsNullOrEmpty(sessionId))
			{
				return false;
			}
			IComposerSession composerSession;
			if (!this.ActiveSessions.TryRemove(sessionId, out composerSession))
			{
				return false;
			}
			this.Logger.Verbose("End Composer Session\nSession Id: {0}", sessionId, null, null, null, "EndSession", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Features\\Composer\\Composer\\Lib\\Wit.ai\\Features\\composer\\Scripts\\Runtime\\ComposerService.cs", 435);
			if (string.Equals(this.LastSessionId, sessionId))
			{
				this.LastSessionId = null;
			}
			ComposerSessionData sessionData = this.GetSessionData(composerSession);
			composerSession.EndSession();
			ComposerEvents events = this.Events;
			if (events != null)
			{
				ComposerSessionEvent onComposerSessionEnd = events.OnComposerSessionEnd;
				if (onComposerSessionEnd != null)
				{
					onComposerSessionEnd.Invoke(sessionData);
				}
			}
			return true;
		}

		protected virtual IComposerSession GenerateSession(string sessionId)
		{
			return new BaseComposerSession(sessionId, this.CurrentContextMap);
		}

		protected virtual string GetSessionId(VoiceServiceRequest request)
		{
			if (request.ResponseData != null)
			{
				string value = request.ResponseData["session_id"].Value;
				if (!string.IsNullOrEmpty(value))
				{
					return value;
				}
			}
			IComposerSessionProvider sessionProvider = this.SessionProvider;
			string text = (sessionProvider != null) ? sessionProvider.GetComposerSessionId(this, request) : null;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (!string.IsNullOrEmpty(this.LastSessionId))
			{
				return this.LastSessionId;
			}
			return this.GetDefaultSessionID();
		}

		public virtual string GetDefaultSessionID()
		{
			return WitConstants.GetUniqueId();
		}

		protected virtual ComposerSessionData GetSessionData(IComposerSession session)
		{
			return new ComposerSessionData
			{
				session = session,
				composer = this,
				responseData = null
			};
		}

		protected virtual void UpdateContextMap(ComposerSessionData sessionData)
		{
			if (this.CurrentContextMap.UpdateData(sessionData.responseData.witResponse))
			{
				this.RaiseContextMapChanged(sessionData);
			}
		}

		protected virtual void RaiseContextMapChanged(ComposerSessionData sessionData)
		{
			if (this.Events.OnComposerContextMapChange == null)
			{
				return;
			}
			ThreadUtility.CallOnMainThread(this.Logger, delegate()
			{
				this.Events.OnComposerContextMapChange.Invoke(sessionData);
			});
		}

		protected virtual void RaiseOnContextMapValueChanged(string key, string oldValue, string newValue)
		{
			if (this.ContextMapEvents.OnContextMapValueChanged == null)
			{
				return;
			}
			ThreadUtility.CallOnMainThread(this.Logger, delegate()
			{
				this.ContextMapEvents.OnContextMapValueChanged.Invoke(key, oldValue, newValue);
			});
		}

		protected virtual void RaiseOnContextMapValueRemoved(string key)
		{
			if (this.ContextMapEvents.OnContextMapValueRemoved == null)
			{
				return;
			}
			ThreadUtility.CallOnMainThread(this.Logger, delegate()
			{
				this.ContextMapEvents.OnContextMapValueRemoved.Invoke(key);
			});
		}

		public void Activate(string message)
		{
			VoiceService voiceService = this._voiceService;
			if (voiceService == null)
			{
				return;
			}
			voiceService.Activate(message);
		}

		public void Activate()
		{
			VoiceService voiceService = this._voiceService;
			if (voiceService == null)
			{
				return;
			}
			voiceService.Activate();
		}

		public void ActivateImmediately()
		{
			VoiceService voiceService = this._voiceService;
			if (voiceService == null)
			{
				return;
			}
			voiceService.ActivateImmediately();
		}

		public void Deactivate()
		{
			VoiceService voiceService = this._voiceService;
			if (voiceService == null)
			{
				return;
			}
			voiceService.Deactivate();
		}

		public void DeactivateAndAbortRequest()
		{
			VoiceService voiceService = this._voiceService;
			if (voiceService == null)
			{
				return;
			}
			voiceService.DeactivateAndAbortRequest();
		}

		public void SendContextMapEvent()
		{
			this.SendEvent(string.Empty);
		}

		public Task SendContextMapEvent(WitRequestOptions requestOptions)
		{
			return this.SendEvent(string.Empty, requestOptions);
		}

		public void SendEvent(string eventJson)
		{
			if (this._voiceService == null)
			{
				return;
			}
			this._voiceService.Activate(eventJson);
		}

		public Task SendEvent(string eventJson, WitRequestOptions requestOptions)
		{
			ComposerService.<SendEvent>d__84 <SendEvent>d__;
			<SendEvent>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendEvent>d__.<>4__this = this;
			<SendEvent>d__.eventJson = eventJson;
			<SendEvent>d__.requestOptions = requestOptions;
			<SendEvent>d__.<>1__state = -1;
			<SendEvent>d__.<>t__builder.Start<ComposerService.<SendEvent>d__84>(ref <SendEvent>d__);
			return <SendEvent>d__.<>t__builder.Task;
		}

		public void AddEventTtsTask(Task ttsTask)
		{
			if (this._eventTtsTasks != null)
			{
				this._eventTtsTasks.Enqueue(ttsTask);
			}
		}

		private bool IsRequestTracked(VoiceServiceRequest request)
		{
			return this._requests.FirstOrDefault((ComposerService.CurrentComposerRequest compRequest) => ((compRequest != null) ? compRequest.Request : null) != null && compRequest.Request.Equals(request)) != null;
		}

		protected virtual void RouteVoiceServiceActivation(VoiceServiceRequest request)
		{
			if (!this.RouteVoiceServiceToComposer)
			{
				return;
			}
			if (this.IsRequestTracked(request))
			{
				return;
			}
			if (!this._enabled)
			{
				request.Cancel("Composer disabled");
				return;
			}
			string sessionId = this.GetSessionId(request);
			IComposerSession session;
			if (!this.ActiveSessions.TryGetValue(sessionId, out session))
			{
				session = this.StartSession(sessionId);
			}
			ComposerSessionData sessionData = this.GetSessionData(session);
			ComposerService.CurrentComposerRequest item = new ComposerService.CurrentComposerRequest(this, request, sessionData);
			object requestLock = this._requestLock;
			lock (requestLock)
			{
				request.HoldTask = Task.WhenAll<bool>(this._requests.Select(delegate(ComposerService.CurrentComposerRequest check)
				{
					if (!string.Equals(sessionData.sessionID, (check != null) ? check.SessionId : null))
					{
						return null;
					}
					if (check == null)
					{
						return null;
					}
					VoiceServiceRequest request2 = check.Request;
					if (request2 == null)
					{
						return null;
					}
					TaskCompletionSource<bool> completion = request2.Completion;
					if (completion == null)
					{
						return null;
					}
					return completion.Task;
				}));
				this._requests.Add(item);
			}
			this.RaiseRequestCreated(sessionData, request);
			this.SetupComposerRequest(sessionData, request);
		}

		protected virtual void SetupComposerRequest(ComposerSessionData sessionData, VoiceServiceRequest request)
		{
			IComposerRequestHandler requestHandler = this.GetRequestHandler();
			if (requestHandler != null)
			{
				requestHandler.OnComposerRequestSetup(sessionData, request);
			}
			this.RaiseRequestSetup(sessionData, request);
		}

		protected virtual void RaiseRequestCreated(ComposerSessionData sessionData, VoiceServiceRequest request)
		{
			this.LogState("Request Setup", request);
			ComposerEvents events = this.Events;
			if (events != null)
			{
				ComposerSessionRequestEvent onComposerRequestCreated = events.OnComposerRequestCreated;
				if (onComposerRequestCreated != null)
				{
					onComposerRequestCreated.Invoke(sessionData, request);
				}
			}
			ThreadUtility.CallOnMainThread(delegate()
			{
				ComposerEvents events2 = this.Events;
				if (events2 == null)
				{
					return;
				}
				ComposerSessionEvent onComposerActivation = events2.OnComposerActivation;
				if (onComposerActivation == null)
				{
					return;
				}
				onComposerActivation.Invoke(sessionData);
			}).WrapErrors();
		}

		protected virtual void RaiseRequestSetup(ComposerSessionData sessionData, VoiceServiceRequest request)
		{
			this.LogState("Request Setup", request);
			ComposerSessionEvent onComposerRequestInit = this.Events.OnComposerRequestInit;
			if (onComposerRequestInit != null)
			{
				onComposerRequestInit.Invoke(sessionData);
			}
			ComposerSessionRequestEvent onComposerRequestSetup = this.Events.OnComposerRequestSetup;
			if (onComposerRequestSetup == null)
			{
				return;
			}
			onComposerRequestSetup.Invoke(sessionData, request);
		}

		protected virtual void OnVoiceRequestSend(ComposerSessionData sessionData, VoiceServiceRequest request)
		{
			this.LogState("Request Send", request);
			ComposerSessionEvent onComposerRequestBegin = this.Events.OnComposerRequestBegin;
			if (onComposerRequestBegin == null)
			{
				return;
			}
			onComposerRequestBegin.Invoke(sessionData);
		}

		protected virtual void OnVoicePartialResponse(ComposerSessionData sessionData)
		{
			this.UpdateContextMap(sessionData);
			if (!string.IsNullOrEmpty(sessionData.responseData.responsePhrase))
			{
				this._ttsHandled |= this.OnComposerSpeakPhrase(sessionData);
			}
			ComposerResponseData responseData = sessionData.responseData;
			string text = (responseData != null) ? responseData.actionID : null;
			if (!this._actionsHandled.Contains(text) && !string.IsNullOrEmpty(text) && this.OnComposerPerformAction(sessionData))
			{
				this._actionsHandled.Add(text);
			}
			ComposerResponseData responseData2 = sessionData.responseData;
			if (responseData2 != null && responseData2.witResponse["FULL_COMPOSER"].AsBool)
			{
				this._actionsHandled.Clear();
			}
		}

		private void OnVoiceRequestComplete(ComposerService.CurrentComposerRequest composerRequest)
		{
			ComposerService.<>c__DisplayClass93_0 CS$<>8__locals1 = new ComposerService.<>c__DisplayClass93_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.composerRequest = composerRequest;
			ThreadUtility.BackgroundAsync(this.Logger, delegate()
			{
				ComposerService.<>c__DisplayClass93_0.<<OnVoiceRequestComplete>b__0>d <<OnVoiceRequestComplete>b__0>d;
				<<OnVoiceRequestComplete>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<OnVoiceRequestComplete>b__0>d.<>4__this = CS$<>8__locals1;
				<<OnVoiceRequestComplete>b__0>d.<>1__state = -1;
				<<OnVoiceRequestComplete>b__0>d.<>t__builder.Start<ComposerService.<>c__DisplayClass93_0.<<OnVoiceRequestComplete>b__0>d>(ref <<OnVoiceRequestComplete>b__0>d);
				return <<OnVoiceRequestComplete>b__0>d.<>t__builder.Task;
			});
		}

		protected virtual void OnComposerCanceled(ComposerSessionData sessionData, string reason)
		{
			sessionData.responseData = new ComposerResponseData(reason);
			if (this.debug)
			{
				this.Logger.Verbose(string.Format("Request Canceled\nReason: {0}", 0), sessionData.responseData.error, null, null, null, "OnComposerCanceled", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Features\\Composer\\Composer\\Lib\\Wit.ai\\Features\\composer\\Scripts\\Runtime\\ComposerService.cs", 849);
			}
			ComposerSessionEvent onComposerCanceled = this.Events.OnComposerCanceled;
			if (onComposerCanceled == null)
			{
				return;
			}
			onComposerCanceled.Invoke(sessionData);
		}

		protected virtual void OnComposerError(ComposerSessionData sessionData, string error)
		{
			sessionData.responseData = new ComposerResponseData(error);
			if (this.debug)
			{
				this.Logger.Error("Request Error\nError: {0}\n{1}", new object[]
				{
					sessionData.responseData.error,
					sessionData.responseData.witResponse
				});
			}
			ComposerSessionEvent onComposerError = this.Events.OnComposerError;
			if (onComposerError == null)
			{
				return;
			}
			onComposerError.Invoke(sessionData);
		}

		protected virtual void OnComposerResponse(ComposerSessionData sessionData, WitResponseNode response)
		{
			ComposerResponseData responseData = sessionData.responseData;
			string text = (responseData != null) ? responseData.actionID : null;
			ComposerResponseData responseData2 = sessionData.responseData;
			if (response != ((responseData2 != null) ? responseData2.witResponse : null))
			{
				sessionData.responseData = response.GetComposerResponse();
				this.OnVoicePartialResponse(sessionData);
			}
			else if (!string.IsNullOrEmpty(text) && !this._actionsHandled.Contains(text) && this.OnComposerPerformAction(sessionData))
			{
				this._actionsHandled.Add(text);
			}
			this.Log("Request Success", false);
			ComposerSessionEvent onComposerResponse = this.Events.OnComposerResponse;
			if (onComposerResponse != null)
			{
				onComposerResponse.Invoke(sessionData);
			}
			bool flag = this._ttsHandled || this._actionsHandled.Count > 0;
			this._ttsHandled = false;
			this._actionsHandled.Clear();
			if (sessionData.responseData != null && sessionData.responseData.expectsInput)
			{
				flag = true;
			}
			if (flag)
			{
				CoroutineUtility.StartCoroutine(this.WaitToContinue(sessionData), false);
			}
		}

		protected virtual bool OnComposerSpeakPhrase(ComposerSessionData sessionData)
		{
			bool responseIsFinal = sessionData.responseData.responseIsFinal;
			if (!responseIsFinal && !this.handlePartialTts)
			{
				return false;
			}
			if (responseIsFinal && !this.handleFinalTts)
			{
				return false;
			}
			if (this.debug)
			{
				this.Logger.Verbose(string.Format("Perform Speak\nPhrase: {0}\nFinal Response: {1}", 0, 1), sessionData.responseData.responsePhrase, responseIsFinal, null, null, "OnComposerSpeakPhrase", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Features\\Composer\\Composer\\Lib\\Wit.ai\\Features\\composer\\Scripts\\Runtime\\ComposerService.cs", 920);
			}
			ComposerSessionEvent onComposerSpeakPhrase = this.Events.OnComposerSpeakPhrase;
			if (onComposerSpeakPhrase != null)
			{
				onComposerSpeakPhrase.Invoke(sessionData);
			}
			int num = 0;
			while (this._speechHandlers != null && num < this._speechHandlers.Length)
			{
				this._speechHandlers[num].SpeakPhrase(sessionData);
				num++;
			}
			return true;
		}

		protected virtual bool OnComposerPerformAction(ComposerSessionData sessionData)
		{
			if (!this.handleActions)
			{
				return false;
			}
			if (this.debug)
			{
				ICoreLogger logger = this.Logger;
				string message = "Perform Action\nAction: {0}";
				object p;
				if (sessionData == null)
				{
					p = null;
				}
				else
				{
					ComposerResponseData responseData = sessionData.responseData;
					p = ((responseData != null) ? responseData.actionID : null);
				}
				logger.Verbose(message, p, null, null, null, "OnComposerPerformAction", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Features\\Composer\\Composer\\Lib\\Wit.ai\\Features\\composer\\Scripts\\Runtime\\ComposerService.cs", 942);
			}
			ComposerSessionEvent onComposerPerformAction = this.Events.OnComposerPerformAction;
			if (onComposerPerformAction != null)
			{
				onComposerPerformAction.Invoke(sessionData);
			}
			if (this._actionHandler != null)
			{
				this._actionHandler.PerformAction(sessionData);
			}
			return true;
		}

		protected virtual void OnComposerExpectsInput(ComposerSessionData sessionData)
		{
			this.Log("Expects Input", false);
			ComposerSessionEvent onComposerExpectsInput = this.Events.OnComposerExpectsInput;
			if (onComposerExpectsInput != null)
			{
				onComposerExpectsInput.Invoke(sessionData);
			}
			if (this.expectInputAutoActivation && this._voiceService != null)
			{
				this._voiceService.Activate();
			}
		}

		protected virtual void OnComposerComplete(ComposerSessionData sessionData)
		{
			this.Log("Graph Complete", false);
			ComposerSessionEvent onComposerComplete = this.Events.OnComposerComplete;
			if (onComposerComplete != null)
			{
				onComposerComplete.Invoke(sessionData);
			}
			if (this.endSessionOnCompletion)
			{
				this.EndSession(sessionData.sessionID);
			}
			if (this.clearContextMapOnCompletion)
			{
				this.CurrentContextMap.ClearAllNonReservedData();
			}
		}

		private IEnumerator WaitToContinue(ComposerSessionData sessionData)
		{
			this.Log("Wait to Continue - Begin", false);
			yield return null;
			yield return new WaitUntil(() => this.IsContinueAllowed(sessionData));
			yield return new WaitForSeconds(this.continueDelay);
			this.Log("Wait to Continue - Complete", false);
			if (sessionData.responseData.expectsInput)
			{
				this.OnComposerExpectsInput(sessionData);
			}
			else
			{
				this.OnComposerComplete(sessionData);
			}
			yield break;
		}

		protected virtual bool IsContinueAllowed(ComposerSessionData sessionData)
		{
			if (this._voiceService.IsRequestActive)
			{
				return false;
			}
			int num = 0;
			while (this._speechHandlers != null && num < this._speechHandlers.Length)
			{
				if (this._speechHandlers[num].IsSpeaking(sessionData))
				{
					return false;
				}
				num++;
			}
			return this._actionHandler == null || !this._actionHandler.IsPerformingAction(sessionData);
		}

		[Header("Voice Settings")]
		[SerializeField]
		private VoiceService _voiceService;

		[Tooltip("Whether or not to send all voice service requests through composer.  If disabled, composer will only send requests made directly from composer.")]
		[FormerlySerializedAs("RouteVoiceServiceToComposer")]
		[SerializeField]
		private bool _routeVoiceServiceToComposer = true;

		[Tooltip("Whether or not to end the previous session when starting a new one")]
		public bool EndLastSessionOnStart = true;

		[Header("Tts Settings")]
		[Tooltip("Whether or not partial tts responses should be sent to attached speech handlers")]
		[FormerlySerializedAs("_handlePartialTts")]
		[SerializeField]
		public bool handlePartialTts = true;

		[Tooltip("Whether or not final tts responses should be sent to attached speech handlers")]
		[FormerlySerializedAs("handleTts")]
		[FormerlySerializedAs("_handleFinalTts")]
		[SerializeField]
		public bool handleFinalTts;

		[Tooltip("Handles response message load and playback")]
		[SerializeField]
		protected IComposerSpeechHandler[] _speechHandlers;

		[Header("Action Settings")]
		[Tooltip("Whether or not response actions should be handled using the action handlers")]
		[FormerlySerializedAs("_handleActions")]
		[SerializeField]
		public bool handleActions = true;

		[Tooltip("Handles response message action calls")]
		[SerializeField]
		protected IComposerActionHandler _actionHandler;

		private List<ComposerService.CurrentComposerRequest> _requests = new List<ComposerService.CurrentComposerRequest>();

		private object _requestLock = new object();

		[Header("Composer Settings")]
		public float continueDelay;

		[Tooltip("A configurable flag for use in the Composer graph to differentiate activations to the server without text/voice input, such as a context map update. In such cases, this will be set to true. \nFor voice and text activations, this will be set to false.")]
		[SerializeField]
		public string contextMapEventKey = "state_event";

		public bool expectInputAutoActivation = true;

		public bool endSessionOnCompletion;

		public bool clearContextMapOnCompletion;

		[SerializeField]
		public bool debug;

		[Obsolete("Use WitConfiguration.editorVersionTag instead.")]
		[SerializeField]
		[HideInInspector]
		public string editorVersionTag;

		[Obsolete("Use WitConfiguration.buildVersionTag instead.")]
		[SerializeField]
		[HideInInspector]
		public string buildVersionTag;

		[Tooltip("Events that will fire before, during and after an activation")]
		[SerializeField]
		private ComposerEvents _events = new ComposerEvents();

		[Tooltip("Events that will fire during Context Map change")]
		[SerializeField]
		private ContextEvents _contextEvents = new ContextEvents();

		[ObjectType(typeof(IComposerSessionProvider), new Type[]
		{

		})]
		[SerializeField]
		private Object _sessionProvider;

		private bool _ttsHandled;

		private HashSet<string> _actionsHandled = new HashSet<string>();

		private bool _enabled;

		private ConcurrentQueue<Task> _eventTtsTasks = new ConcurrentQueue<Task>();

		private class CurrentComposerRequest
		{
			public VoiceServiceRequest Request { get; private set; }

			public string SessionId
			{
				get
				{
					ComposerSessionData sessionData = this.SessionData;
					if (sessionData == null)
					{
						return null;
					}
					return sessionData.sessionID;
				}
			}

			public bool IsActive
			{
				get
				{
					return this.Request != null && this.Request.IsActive;
				}
			}

			public CurrentComposerRequest(ComposerService service, VoiceServiceRequest request, ComposerSessionData sessionData)
			{
				this._service = service;
				this.SessionData = sessionData;
				this.SessionData.responseData = new ComposerResponseData();
				this.Request = request;
				this.Request.Events.OnSend.AddListener(new UnityAction<VoiceServiceRequest>(this.OnSend));
				this.Request.Events.OnPartialResponse.AddListener(new UnityAction<WitResponseNode>(this.OnPartial));
				this.Request.Events.OnValidateResponse.AddListener(new UnityAction<WitResponseNode, StringBuilder>(this.OnValidate));
				this.Request.Events.OnComplete.AddListener(new UnityAction<VoiceServiceRequest>(this.OnComplete));
				if (this.Request.ResponseData != null)
				{
					this.OnPartial(this.Request.ResponseData);
				}
			}

			private void OnSend(VoiceServiceRequest r)
			{
				this.UpdateResponseData(r.ResponseData);
				this._service.OnVoiceRequestSend(this.SessionData, r);
			}

			private void OnPartial(WitResponseNode r)
			{
				this.UpdateResponseData(r);
				this._service.OnVoicePartialResponse(this.SessionData);
			}

			private void OnValidate(WitResponseNode r, StringBuilder validationErrors)
			{
				WitResponseClass witResponseClass = (r != null) ? r.AsObject : null;
				if (witResponseClass == null || !witResponseClass.HasChild("context_map"))
				{
					if (validationErrors.Length > 0)
					{
						validationErrors.Append(", ");
					}
					validationErrors.Append("missing context map");
				}
			}

			private void OnComplete(VoiceServiceRequest r)
			{
				this.Request.Events.OnSend.RemoveListener(new UnityAction<VoiceServiceRequest>(this.OnSend));
				this.Request.Events.OnPartialResponse.RemoveListener(new UnityAction<WitResponseNode>(this.OnPartial));
				this.Request.Events.OnValidateResponse.RemoveListener(new UnityAction<WitResponseNode, StringBuilder>(this.OnValidate));
				this.Request.Events.OnComplete.RemoveListener(new UnityAction<VoiceServiceRequest>(this.OnComplete));
				try
				{
					this.UpdateResponseData(r.ResponseData);
					this._service.OnVoiceRequestComplete(this);
				}
				catch (Exception arg)
				{
					Debug.LogError(string.Format("Update Failure\n{0}", arg));
				}
			}

			private void UpdateResponseData(WitResponseNode r)
			{
				this.SessionData.responseData = r.GetComposerResponse();
			}

			private ComposerService _service;

			public readonly ComposerSessionData SessionData;
		}
	}
}
