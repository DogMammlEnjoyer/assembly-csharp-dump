using System;
using System.Collections.Generic;
using System.Linq;
using Meta.WitAi;
using Meta.WitAi.Configuration;
using Meta.WitAi.Events;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Voice.Bindings.Android
{
	public class VoiceSDKListenerBinding : AndroidJavaProxy
	{
		public VoiceEvents VoiceEvents
		{
			get
			{
				return this._voiceService.VoiceEvents;
			}
		}

		public TelemetryEvents TelemetryEvents
		{
			get
			{
				return this._voiceService.TelemetryEvents;
			}
		}

		public VoiceSDKListenerBinding(IVoiceService voiceService, IVCBindingEvents bindingEvents) : base("com.oculus.assistant.api.voicesdk.immersivevoicecommands.IVCEventsListener")
		{
			this._voiceService = voiceService;
			this._bindingEvents = bindingEvents;
		}

		private VoiceServiceRequest GetRequest(string requestId)
		{
			HashSet<VoiceServiceRequest> requests = this._voiceService.Requests;
			if (requests == null || requests.Count == 0)
			{
				return null;
			}
			foreach (VoiceServiceRequest voiceServiceRequest in requests)
			{
				string text;
				if (voiceServiceRequest == null)
				{
					text = null;
				}
				else
				{
					WitRequestOptions options = voiceServiceRequest.Options;
					text = ((options != null) ? options.RequestId : null);
				}
				string b = text;
				if (string.Equals(requestId, b))
				{
					return voiceServiceRequest;
				}
			}
			return requests.First<VoiceServiceRequest>();
		}

		public void onStartListening(string requestId)
		{
			UnityEvent onStartListening = this.VoiceEvents.OnStartListening;
			if (onStartListening == null)
			{
				return;
			}
			onStartListening.Invoke();
		}

		public void onStartListening()
		{
			this.onStartListening(null);
		}

		public void onStoppedListening(int reason, string requestId)
		{
			VoiceServiceRequest request = this.GetRequest(requestId);
			UnityEvent onStoppedListening = this.VoiceEvents.OnStoppedListening;
			if (onStoppedListening != null)
			{
				onStoppedListening.Invoke();
			}
			switch (reason)
			{
			case 0:
				break;
			case 1:
			{
				UnityEvent onStoppedListeningDueToInactivity = this.VoiceEvents.OnStoppedListeningDueToInactivity;
				if (onStoppedListeningDueToInactivity != null)
				{
					onStoppedListeningDueToInactivity.Invoke();
				}
				request.Cancel("Request was cancelled.");
				return;
			}
			case 2:
			{
				UnityEvent onStoppedListeningDueToTimeout = this.VoiceEvents.OnStoppedListeningDueToTimeout;
				if (onStoppedListeningDueToTimeout != null)
				{
					onStoppedListeningDueToTimeout.Invoke();
				}
				request.Cancel("Request was cancelled.");
				return;
			}
			case 3:
			{
				UnityEvent onStoppedListeningDueToDeactivation = this.VoiceEvents.OnStoppedListeningDueToDeactivation;
				if (onStoppedListeningDueToDeactivation == null)
				{
					return;
				}
				onStoppedListeningDueToDeactivation.Invoke();
				break;
			}
			default:
				return;
			}
		}

		public void onStoppedListening(int reason)
		{
			this.onStoppedListening(reason, null);
		}

		public void onRequestCreated(string requestId)
		{
			VoiceSDKImplRequest voiceSDKImplRequest = this.GetRequest(requestId) as VoiceSDKImplRequest;
			if (voiceSDKImplRequest != null)
			{
				voiceSDKImplRequest.HandleTransmissionBegan();
			}
		}

		private void onRequestCreated()
		{
			this.onRequestCreated(null);
		}

		public void onPartialTranscription(string transcription, string requestId)
		{
			VoiceSDKImplRequest voiceSDKImplRequest = this.GetRequest(requestId) as VoiceSDKImplRequest;
			if (voiceSDKImplRequest != null)
			{
				voiceSDKImplRequest.HandlePartialTranscription(transcription);
			}
		}

		public void onPartialTranscription(string transcription)
		{
			this.onPartialTranscription(transcription, null);
		}

		public void onFullTranscription(string transcription, string requestId)
		{
			VoiceSDKImplRequest voiceSDKImplRequest = this.GetRequest(requestId) as VoiceSDKImplRequest;
			if (voiceSDKImplRequest != null)
			{
				voiceSDKImplRequest.HandleFullTranscription(transcription);
			}
		}

		public void onFullTranscription(string transcription)
		{
			this.onFullTranscription(transcription, null);
		}

		public void onPartialResponse(string responseJson, string requestId)
		{
			VoiceSDKImplRequest voiceSDKImplRequest = this.GetRequest(requestId) as VoiceSDKImplRequest;
			if (voiceSDKImplRequest != null)
			{
				voiceSDKImplRequest.HandlePartialResponse(responseJson);
			}
		}

		public void onPartialResponse(string responseJson)
		{
			this.onPartialResponse(responseJson, null);
		}

		public void onAborted(string requestId)
		{
			VoiceSDKImplRequest voiceSDKImplRequest = this.GetRequest(requestId) as VoiceSDKImplRequest;
			if (voiceSDKImplRequest != null)
			{
				voiceSDKImplRequest.HandleCanceled();
			}
		}

		public void onAborted()
		{
			this.onAborted(null);
		}

		public void onError(string error, string message, string errorBody, string requestId)
		{
			VoiceSDKImplRequest voiceSDKImplRequest = this.GetRequest(requestId) as VoiceSDKImplRequest;
			if (voiceSDKImplRequest != null)
			{
				voiceSDKImplRequest.HandleError(error, message, errorBody);
			}
		}

		public void onError(string error, string message, string errorBody)
		{
			this.onError(error, message, errorBody, null);
		}

		public void onResponse(string responseJson, string requestId)
		{
			VoiceSDKImplRequest voiceSDKImplRequest = this.GetRequest(requestId) as VoiceSDKImplRequest;
			if (voiceSDKImplRequest != null)
			{
				voiceSDKImplRequest.HandleResponse(responseJson);
			}
		}

		public void onResponse(string responseJson)
		{
			this.onResponse(responseJson, null);
		}

		public void onMicLevelChanged(float level, string requestId)
		{
			WitMicLevelChangedEvent onMicLevelChanged = this.VoiceEvents.OnMicLevelChanged;
			if (onMicLevelChanged == null)
			{
				return;
			}
			onMicLevelChanged.Invoke(level);
		}

		public void onMicLevelChanged(float level)
		{
			this.onMicLevelChanged(level, null);
		}

		public void onMicDataSent(string requestId)
		{
			UnityEvent onMicDataSent = this.VoiceEvents.OnMicDataSent;
			if (onMicDataSent == null)
			{
				return;
			}
			onMicDataSent.Invoke();
		}

		public void onMicDataSent()
		{
			this.onMicDataSent(null);
		}

		public void onMinimumWakeThresholdHit(string requestId)
		{
			UnityEvent onMinimumWakeThresholdHit = this.VoiceEvents.OnMinimumWakeThresholdHit;
			if (onMinimumWakeThresholdHit == null)
			{
				return;
			}
			onMinimumWakeThresholdHit.Invoke();
		}

		public void onMinimumWakeThresholdHit()
		{
			this.onMinimumWakeThresholdHit(null);
		}

		public void onRequestCompleted(string requestId)
		{
		}

		public void onRequestCompleted()
		{
			this.onRequestCompleted(null);
		}

		public void onServiceNotAvailable(string error, string message)
		{
			VLog.W("Platform service is not available: " + error + " - " + message, null);
			this._bindingEvents.OnServiceNotAvailable(error, message);
		}

		public void onAudioDurationTrackerFinished(long timestamp, double duration)
		{
			long arg = this.NativeTimestampToDateTime(timestamp).Ticks / 10000L;
			AudioDurationTrackerFinishedEvent onAudioTrackerFinished = this.TelemetryEvents.OnAudioTrackerFinished;
			if (onAudioTrackerFinished == null)
			{
				return;
			}
			onAudioTrackerFinished.Invoke(arg, duration);
		}

		private DateTime NativeTimestampToDateTime(long javaTimestamp)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return dateTime.AddMilliseconds((double)javaTimestamp);
		}

		private IVoiceService _voiceService;

		private readonly IVCBindingEvents _bindingEvents;

		public enum StoppedListeningReason
		{
			NoReasonProvided,
			Inactivity,
			Timeout,
			Deactivation
		}
	}
}
