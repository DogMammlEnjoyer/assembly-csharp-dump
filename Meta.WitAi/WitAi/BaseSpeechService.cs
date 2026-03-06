using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Meta.Voice;
using Meta.Voice.Logging;
using Meta.Voice.TelemetryUtilities;
using Meta.WitAi.Configuration;
using Meta.WitAi.Events;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi
{
	[LogCategory(LogCategory.SpeechService)]
	public abstract class BaseSpeechService : MonoBehaviour
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.SpeechService, null);

		public HashSet<VoiceServiceRequest> Requests { get; } = new HashSet<VoiceServiceRequest>();

		public virtual bool Active
		{
			get
			{
				return this.Requests != null && this.Requests.Count > 0;
			}
		}

		protected virtual SpeechEvents GetSpeechEvents()
		{
			return null;
		}

		public virtual bool IsAudioInputActive
		{
			get
			{
				VoiceServiceRequest audioRequest = this.GetAudioRequest();
				return audioRequest != null && audioRequest.IsAudioInputActivated;
			}
		}

		protected virtual VoiceServiceRequest GetAudioRequest()
		{
			HashSet<VoiceServiceRequest> requests = this.Requests;
			if (requests == null)
			{
				return null;
			}
			return requests.FirstOrDefault((VoiceServiceRequest request) => request.InputType == NLPRequestInputType.Audio);
		}

		public virtual string GetActivateAudioError()
		{
			if (this.IsAudioInputActive)
			{
				return "Audio input is already being performed for this service.";
			}
			return string.Empty;
		}

		public virtual bool CanActivateAudio()
		{
			return string.IsNullOrEmpty(this.GetActivateAudioError());
		}

		public virtual string GetSendError()
		{
			return string.Empty;
		}

		public virtual bool CanSend()
		{
			return string.IsNullOrEmpty(this.GetSendError());
		}

		protected virtual void OnEnable()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				this.Logger.Error("Unable to reach the internet. Check your connection.", Array.Empty<object>());
			}
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents == null)
			{
				return;
			}
			speechEvents.OnRequestInitialized.AddListener(new UnityAction<VoiceServiceRequest>(this.OnRequestInit));
		}

		protected virtual void OnDisable()
		{
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents == null)
			{
				return;
			}
			speechEvents.OnRequestInitialized.RemoveListener(new UnityAction<VoiceServiceRequest>(this.OnRequestInit));
		}

		public virtual void Deactivate()
		{
			foreach (VoiceServiceRequest request in this.Requests.ToArray<VoiceServiceRequest>())
			{
				this.Deactivate(request);
			}
		}

		public virtual void Deactivate(VoiceServiceRequest request)
		{
			if (request == null || !request.IsLocalRequest)
			{
				return;
			}
			request.DeactivateAudio();
		}

		public virtual void DeactivateAndAbortRequest()
		{
			foreach (VoiceServiceRequest request in this.Requests.ToArray<VoiceServiceRequest>())
			{
				this.DeactivateAndAbortRequest(request);
			}
		}

		public virtual void DeactivateAndAbortRequest(VoiceServiceRequest request)
		{
			if (request == null || !request.IsLocalRequest)
			{
				return;
			}
			request.Cancel("Request was cancelled.");
		}

		public virtual void SetupRequestParameters(ref WitRequestOptions options, ref VoiceServiceRequestEvents events)
		{
			if (options == null)
			{
				options = new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>());
			}
			if (events == null)
			{
				events = new VoiceServiceRequestEvents();
			}
			if (this.ShouldWrap)
			{
				WitRequestOptionsEvent onRequestOptionSetup = this.GetSpeechEvents().OnRequestOptionSetup;
				if (onRequestOptionSetup == null)
				{
					return;
				}
				onRequestOptionSetup.Invoke(options);
			}
		}

		public virtual bool WrapRequest(VoiceServiceRequest request)
		{
			if (request == null)
			{
				this.Log(null, "Cannot wrap a null VoiceServiceRequest", true);
				return false;
			}
			if (request.State == VoiceRequestState.Canceled)
			{
				RuntimeTelemetry.Instance.LogEventTermination((OperationID)request.Options.OperationId, TerminationReason.Canceled, "");
				this.OnRequestCancel(request);
				this.OnRequestComplete(request);
				return true;
			}
			if (request.State == VoiceRequestState.Failed)
			{
				RuntimeTelemetry.Instance.LogEventTermination((OperationID)request.Options.OperationId, TerminationReason.Failed, "");
				this.OnRequestFailed(request);
				this.OnRequestComplete(request);
				return true;
			}
			if (request.State == VoiceRequestState.Successful)
			{
				RuntimeTelemetry.Instance.LogEventTermination((OperationID)request.Options.OperationId, TerminationReason.Successful, "");
				this.OnRequestPartialResponse(request, (request != null) ? request.ResponseData : null);
				this.OnRequestSuccess(request);
				this.OnRequestComplete(request);
				return true;
			}
			if (this.ShouldWrap)
			{
				SpeechEvents speechEvents = this.GetSpeechEvents();
				if (speechEvents != null)
				{
					Meta.WitAi.Events.VoiceServiceRequestEvent onRequestInitialized = speechEvents.OnRequestInitialized;
					if (onRequestInitialized != null)
					{
						onRequestInitialized.Invoke(request);
					}
				}
				if (request.State == VoiceRequestState.Transmitting)
				{
					this.OnRequestSend(request);
				}
			}
			return true;
		}

		protected virtual void Log(VoiceServiceRequest request, string log, bool warn = false)
		{
			if (!this.ShouldLog)
			{
				return;
			}
			if (warn)
			{
				ICoreLogger logger = this.Logger;
				string message = "{0}\nRequest Id: {1}";
				object[] array = new object[2];
				array[0] = log;
				int num = 1;
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
				array[num] = obj;
				logger.Warning(message, array);
				return;
			}
			this.Logger.Info(log, null, null, null, null, "Log", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\BaseSpeechService.cs", 275);
		}

		protected virtual void OnRequestInit(VoiceServiceRequest request)
		{
			if (this.Requests.Contains(request))
			{
				return;
			}
			this.SetEventListeners(request, true);
			this.Requests.Add(request);
			this.Log(request, "Request Initialized", false);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents == null)
			{
				return;
			}
			WitRequestCreatedEvent onRequestCreated = speechEvents.OnRequestCreated;
			if (onRequestCreated == null)
			{
				return;
			}
			WitRequest witRequest = request as WitRequest;
			onRequestCreated.Invoke((witRequest != null) ? witRequest : null);
		}

		protected virtual void OnRequestStartListening(VoiceServiceRequest request)
		{
			this.Log(request, "Request Start Listening", false);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents == null)
			{
				return;
			}
			UnityEvent onStartListening = speechEvents.OnStartListening;
			if (onStartListening == null)
			{
				return;
			}
			onStartListening.Invoke();
		}

		protected virtual void OnRequestStopListening(VoiceServiceRequest request)
		{
			this.Log(request, "Request Stop Listening", false);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents == null)
			{
				return;
			}
			UnityEvent onStoppedListening = speechEvents.OnStoppedListening;
			if (onStoppedListening == null)
			{
				return;
			}
			onStoppedListening.Invoke();
		}

		protected virtual void OnRequestSend(VoiceServiceRequest request)
		{
			this.Log(request, "Request Send", false);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents == null)
			{
				return;
			}
			Meta.WitAi.Events.VoiceServiceRequestEvent onSend = speechEvents.OnSend;
			if (onSend == null)
			{
				return;
			}
			onSend.Invoke(request);
		}

		protected virtual void OnRequestRawResponse(VoiceServiceRequest request, string rawResponse)
		{
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents == null)
			{
				return;
			}
			WitTranscriptionEvent onRawResponse = speechEvents.OnRawResponse;
			if (onRawResponse == null)
			{
				return;
			}
			onRawResponse.Invoke(rawResponse);
		}

		protected virtual void OnRequestPartialTranscription(VoiceServiceRequest request, string transcription)
		{
			this.Log(request, "Request partial transcription received \nText: " + transcription, false);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents != null)
			{
				WitTranscriptionEvent onPartialTranscription = speechEvents.OnPartialTranscription;
				if (onPartialTranscription != null)
				{
					onPartialTranscription.Invoke(transcription);
				}
			}
			SpeechEvents speechEvents2 = this.GetSpeechEvents();
			if (speechEvents2 == null)
			{
				return;
			}
			UserTranscriptionRequestEvent onUserPartialTranscription = speechEvents2.OnUserPartialTranscription;
			if (onUserPartialTranscription == null)
			{
				return;
			}
			onUserPartialTranscription.Invoke(request.Options.ClientUserId, transcription);
		}

		protected virtual void OnRequestFullTranscription(VoiceServiceRequest request, string transcription)
		{
			this.Log(request, "Request Full Transcription received\nText: " + transcription, false);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents != null)
			{
				WitTranscriptionEvent onFullTranscription = speechEvents.OnFullTranscription;
				if (onFullTranscription != null)
				{
					onFullTranscription.Invoke(transcription);
				}
			}
			SpeechEvents speechEvents2 = this.GetSpeechEvents();
			if (speechEvents2 == null)
			{
				return;
			}
			UserTranscriptionRequestEvent onUserFullTranscription = speechEvents2.OnUserFullTranscription;
			if (onUserFullTranscription == null)
			{
				return;
			}
			onUserFullTranscription.Invoke(request.Options.ClientUserId, transcription);
		}

		protected virtual void OnRequestPartialResponse(VoiceServiceRequest request, WitResponseNode responseData)
		{
			if (responseData != null)
			{
				SpeechEvents speechEvents = this.GetSpeechEvents();
				if (speechEvents == null)
				{
					return;
				}
				WitResponseEvent onPartialResponse = speechEvents.OnPartialResponse;
				if (onPartialResponse == null)
				{
					return;
				}
				onPartialResponse.Invoke(responseData);
			}
		}

		protected virtual void OnRequestCancel(VoiceServiceRequest request)
		{
			string text;
			if (request == null)
			{
				text = null;
			}
			else
			{
				VoiceServiceRequestResults results = request.Results;
				text = ((results != null) ? results.Message : null);
			}
			string text2 = text;
			this.Log(request, "Request Canceled\nReason: " + text2, false);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents != null)
			{
				WitTranscriptionEvent onCanceled = speechEvents.OnCanceled;
				if (onCanceled != null)
				{
					onCanceled.Invoke(text2);
				}
			}
			if (!string.Equals(text2, "Request cancelled prior to transmission begin"))
			{
				SpeechEvents speechEvents2 = this.GetSpeechEvents();
				if (speechEvents2 == null)
				{
					return;
				}
				UnityEvent onAborted = speechEvents2.OnAborted;
				if (onAborted == null)
				{
					return;
				}
				onAborted.Invoke();
			}
		}

		protected virtual void OnRequestFailed(VoiceServiceRequest request)
		{
			string text = string.Format("HTTP Error {0}", request.Results.StatusCode);
			string text2;
			if (request == null)
			{
				text2 = null;
			}
			else
			{
				VoiceServiceRequestResults results = request.Results;
				text2 = ((results != null) ? results.Message : null);
			}
			string text3 = text2;
			string text4 = text3;
			if (string.Equals(text4, "timeout"))
			{
				text4 += string.Format("\nTimeout Ms: {0}", request.Options.TimeoutMs);
			}
			this.Log(request, "Request Failed\n" + text + ": " + text4, true);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents != null)
			{
				WitErrorEvent onError = speechEvents.OnError;
				if (onError != null)
				{
					onError.Invoke(text, text3);
				}
			}
			SpeechEvents speechEvents2 = this.GetSpeechEvents();
			if (speechEvents2 == null)
			{
				return;
			}
			UnityEvent onRequestCompleted = speechEvents2.OnRequestCompleted;
			if (onRequestCompleted == null)
			{
				return;
			}
			onRequestCompleted.Invoke();
		}

		protected virtual void OnRequestSuccess(VoiceServiceRequest request)
		{
			this.Log(request, "Request Success", false);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents != null)
			{
				WitResponseEvent onResponse = speechEvents.OnResponse;
				if (onResponse != null)
				{
					onResponse.Invoke((request != null) ? request.ResponseData : null);
				}
			}
			SpeechEvents speechEvents2 = this.GetSpeechEvents();
			if (speechEvents2 == null)
			{
				return;
			}
			UnityEvent onRequestCompleted = speechEvents2.OnRequestCompleted;
			if (onRequestCompleted == null)
			{
				return;
			}
			onRequestCompleted.Invoke();
		}

		protected virtual void OnRequestComplete(VoiceServiceRequest request)
		{
			if (this.Requests.Contains(request))
			{
				this.SetEventListeners(request, false);
				this.Requests.Remove(request);
			}
			this.Log(request, string.Format("Request Complete\nRemaining: {0}", this.Requests.Count), false);
			SpeechEvents speechEvents = this.GetSpeechEvents();
			if (speechEvents == null)
			{
				return;
			}
			Meta.WitAi.Events.VoiceServiceRequestEvent onComplete = speechEvents.OnComplete;
			if (onComplete == null)
			{
				return;
			}
			onComplete.Invoke(request);
		}

		protected virtual void SetEventListeners(VoiceServiceRequest request, bool addListeners)
		{
			VoiceServiceRequestEvents events = request.Events;
			events.OnStartListening.SetListener(new UnityAction<VoiceServiceRequest>(this.OnRequestStartListening), addListeners);
			events.OnStopListening.SetListener(new UnityAction<VoiceServiceRequest>(this.OnRequestStopListening), addListeners);
			events.OnSend.SetListener(new UnityAction<VoiceServiceRequest>(this.OnRequestSend), addListeners);
			events.OnSuccess.SetListener(new UnityAction<VoiceServiceRequest>(this.OnRequestSuccess), addListeners);
			events.OnFailed.SetListener(new UnityAction<VoiceServiceRequest>(this.OnRequestFailed), addListeners);
			events.OnCancel.SetListener(new UnityAction<VoiceServiceRequest>(this.OnRequestCancel), addListeners);
			events.OnComplete.SetListener(new UnityAction<VoiceServiceRequest>(this.OnRequestComplete), addListeners);
			this.SetRequestEventListener<string>(events.OnRawResponse, request, new UnityAction<VoiceServiceRequest, string>(this.OnRequestRawResponse), addListeners);
			this.SetRequestEventListener<string>(events.OnPartialTranscription, request, new UnityAction<VoiceServiceRequest, string>(this.OnRequestPartialTranscription), addListeners);
			this.SetRequestEventListener<string>(events.OnFullTranscription, request, new UnityAction<VoiceServiceRequest, string>(this.OnRequestFullTranscription), addListeners);
			this.SetRequestEventListener<WitResponseNode>(events.OnPartialResponse, request, new UnityAction<VoiceServiceRequest, WitResponseNode>(this.OnRequestPartialResponse), addListeners);
		}

		private void SetRequestEventListener<TParam>(UnityEvent<TParam> baseEvent, VoiceServiceRequest request, UnityAction<VoiceServiceRequest, TParam> callbackWithRequest, bool addListener)
		{
			int hashCode = baseEvent.GetHashCode();
			if (addListener)
			{
				UnityAction<TParam> unityAction = delegate(TParam param)
				{
					UnityAction<VoiceServiceRequest, TParam> callbackWithRequest2 = callbackWithRequest;
					if (callbackWithRequest2 == null)
					{
						return;
					}
					callbackWithRequest2(request, param);
				};
				this._customRequestEvents[hashCode] = unityAction;
				baseEvent.AddListener(unityAction);
				return;
			}
			object obj;
			if (this._customRequestEvents.TryRemove(hashCode, out obj))
			{
				UnityAction<TParam> unityAction2 = obj as UnityAction<TParam>;
				if (unityAction2 != null)
				{
					baseEvent.RemoveListener(unityAction2);
				}
			}
		}

		public bool ShouldWrap = true;

		public bool ShouldLog = true;

		private ConcurrentDictionary<int, object> _customRequestEvents = new ConcurrentDictionary<int, object>();
	}
}
