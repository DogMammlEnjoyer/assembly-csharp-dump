using System;
using System.Collections.Generic;
using Meta.Voice;
using Meta.WitAi.Configuration;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Meta.WitAi.Events
{
	[Serializable]
	public class SpeechEvents : EventRegistry, ISpeechEvents, ITranscriptionEvent, IAudioInputEvents
	{
		public WitRequestOptionsEvent OnRequestOptionSetup
		{
			get
			{
				return this._onRequestOptionSetup;
			}
		}

		public VoiceServiceRequestEvent OnRequestInitialized
		{
			get
			{
				return this._onRequestInitialized;
			}
		}

		[Obsolete("Deprecated for 'OnSend' event")]
		public WitRequestCreatedEvent OnRequestCreated
		{
			get
			{
				return this._onRequestCreated;
			}
		}

		public VoiceServiceRequestEvent OnSend
		{
			get
			{
				return this._onSend;
			}
		}

		public UnityEvent OnMinimumWakeThresholdHit
		{
			get
			{
				return this._onMinimumWakeThresholdHit;
			}
		}

		public UnityEvent OnMicDataSent
		{
			get
			{
				return this._onMicDataSent;
			}
		}

		public UnityEvent OnStoppedListeningDueToDeactivation
		{
			get
			{
				return this._onStoppedListeningDueToDeactivation;
			}
		}

		public UnityEvent OnStoppedListeningDueToInactivity
		{
			get
			{
				return this._onStoppedListeningDueToInactivity;
			}
		}

		public UnityEvent OnStoppedListeningDueToTimeout
		{
			get
			{
				return this._onStoppedListeningDueToTimeout;
			}
		}

		public UnityEvent OnAborting
		{
			get
			{
				return this._onAborting;
			}
		}

		public UnityEvent OnAborted
		{
			get
			{
				return this._onAborted;
			}
		}

		public WitTranscriptionEvent OnCanceled
		{
			get
			{
				return this._onCanceled;
			}
		}

		public WitTranscriptionEvent OnRawResponse
		{
			get
			{
				return this._onRawResponse;
			}
		}

		public WitResponseEvent OnPartialResponse
		{
			get
			{
				return this._onPartialResponse;
			}
		}

		public WitResponseEvent OnResponse
		{
			get
			{
				return this._onResponse;
			}
		}

		public WitErrorEvent OnError
		{
			get
			{
				return this._onError;
			}
		}

		public UnityEvent OnRequestCompleted
		{
			get
			{
				return this._onRequestCompleted;
			}
		}

		public VoiceServiceRequestEvent OnComplete
		{
			get
			{
				return this._onComplete;
			}
		}

		public UnityEvent OnStartListening
		{
			get
			{
				return this._onStartListening;
			}
		}

		public UnityEvent OnMicStartedListening
		{
			get
			{
				return this.OnStartListening;
			}
		}

		public UnityEvent OnStoppedListening
		{
			get
			{
				return this._onStoppedListening;
			}
		}

		public UnityEvent OnMicStoppedListening
		{
			get
			{
				return this.OnStoppedListening;
			}
		}

		public WitMicLevelChangedEvent OnMicLevelChanged
		{
			get
			{
				return this._onMicLevelChanged;
			}
		}

		public WitMicLevelChangedEvent OnMicAudioLevelChanged
		{
			get
			{
				return this.OnMicLevelChanged;
			}
		}

		public WitTranscriptionEvent OnPartialTranscription
		{
			get
			{
				return this._onPartialTranscription;
			}
		}

		[Obsolete("Deprecated for 'OnPartialTranscription' event")]
		public WitTranscriptionEvent onPartialTranscription
		{
			get
			{
				return this.OnPartialTranscription;
			}
		}

		public WitTranscriptionEvent OnFullTranscription
		{
			get
			{
				return this._onFullTranscription;
			}
		}

		[Obsolete("Deprecated for 'OnPartialTranscription' event")]
		public WitTranscriptionEvent onFullTranscription
		{
			get
			{
				return this.OnFullTranscription;
			}
		}

		public UserTranscriptionRequestEvent OnUserPartialTranscription
		{
			get
			{
				return this._onUserPartialTranscription;
			}
		}

		public UserTranscriptionRequestEvent OnUserFullTranscription
		{
			get
			{
				return this._onUserFullTranscription;
			}
		}

		public void AddListener(SpeechEvents listener)
		{
			this.SetListener(listener, true);
		}

		public void RemoveListener(SpeechEvents listener)
		{
			this.SetListener(listener, false);
		}

		public virtual void SetListener(SpeechEvents listener, bool add)
		{
			if (listener == null || listener.Equals(this))
			{
				return;
			}
			if (add)
			{
				if (!this._listeners.Add(listener))
				{
					return;
				}
			}
			else if (!this._listeners.Remove(listener))
			{
				return;
			}
			this.OnRequestOptionSetup.SetListener(new UnityAction<WitRequestOptions>(listener.OnRequestOptionSetup.Invoke), add);
			this.OnRequestInitialized.SetListener(new UnityAction<VoiceServiceRequest>(listener.OnRequestInitialized.Invoke), add);
			this.OnSend.SetListener(new UnityAction<VoiceServiceRequest>(listener.OnSend.Invoke), add);
			this.OnMinimumWakeThresholdHit.SetListener(new UnityAction(listener.OnMinimumWakeThresholdHit.Invoke), add);
			this.OnMicDataSent.SetListener(new UnityAction(listener.OnMicDataSent.Invoke), add);
			this.OnStoppedListeningDueToDeactivation.SetListener(new UnityAction(listener.OnStoppedListeningDueToDeactivation.Invoke), add);
			this.OnStoppedListeningDueToInactivity.SetListener(new UnityAction(listener.OnStoppedListeningDueToInactivity.Invoke), add);
			this.OnStoppedListeningDueToTimeout.SetListener(new UnityAction(listener.OnStoppedListeningDueToTimeout.Invoke), add);
			this.OnAborting.SetListener(new UnityAction(listener.OnAborting.Invoke), add);
			this.OnAborted.SetListener(new UnityAction(listener.OnAborted.Invoke), add);
			this.OnCanceled.SetListener(new UnityAction<string>(listener.OnCanceled.Invoke), add);
			this.OnRawResponse.SetListener(new UnityAction<string>(listener.OnRawResponse.Invoke), add);
			this.OnPartialResponse.SetListener(new UnityAction<WitResponseNode>(listener.OnPartialResponse.Invoke), add);
			this.OnResponse.SetListener(new UnityAction<WitResponseNode>(listener.OnResponse.Invoke), add);
			this.OnError.SetListener(new UnityAction<string, string>(listener.OnError.Invoke), add);
			this.OnRequestCompleted.SetListener(new UnityAction(listener.OnRequestCompleted.Invoke), add);
			this.OnComplete.SetListener(new UnityAction<VoiceServiceRequest>(listener.OnComplete.Invoke), add);
			this.OnStartListening.SetListener(new UnityAction(listener.OnStartListening.Invoke), add);
			this.OnMicStartedListening.SetListener(new UnityAction(listener.OnMicStartedListening.Invoke), add);
			this.OnStoppedListening.SetListener(new UnityAction(listener.OnStoppedListening.Invoke), add);
			this.OnMicStoppedListening.SetListener(new UnityAction(listener.OnMicStoppedListening.Invoke), add);
			this.OnMicLevelChanged.SetListener(new UnityAction<float>(listener.OnMicLevelChanged.Invoke), add);
			this.OnMicAudioLevelChanged.SetListener(new UnityAction<float>(listener.OnMicAudioLevelChanged.Invoke), add);
			this.OnPartialTranscription.SetListener(new UnityAction<string>(listener.OnPartialTranscription.Invoke), add);
			this.OnFullTranscription.SetListener(new UnityAction<string>(listener.OnFullTranscription.Invoke), add);
			this.OnUserPartialTranscription.SetListener(new UnityAction<string, string>(listener.OnUserPartialTranscription.Invoke), add);
			this.OnUserFullTranscription.SetListener(new UnityAction<string, string>(listener.OnUserFullTranscription.Invoke), add);
			this.OnRequestCreated.SetListener(new UnityAction<WitRequest>(listener.OnRequestCreated.Invoke), add);
			this.onPartialTranscription.SetListener(new UnityAction<string>(listener.onPartialTranscription.Invoke), add);
			this.onFullTranscription.SetListener(new UnityAction<string>(listener.onFullTranscription.Invoke), add);
		}

		protected const string EVENT_CATEGORY_ACTIVATION_SETUP = "Activation Setup Events";

		[EventCategory("Activation Setup Events")]
		[Tooltip("Called prior to initialization for WitRequestOption customization")]
		[FormerlySerializedAs("OnRequestOptionSetup")]
		[SerializeField]
		private WitRequestOptionsEvent _onRequestOptionSetup = new WitRequestOptionsEvent();

		[EventCategory("Activation Setup Events")]
		[Tooltip("Called when a request is created.  This occurs as soon as a activation is called successfully.")]
		[FormerlySerializedAs("OnRequestInitialized")]
		[SerializeField]
		private VoiceServiceRequestEvent _onRequestInitialized = new VoiceServiceRequestEvent();

		public Action<VoiceServiceRequest> OnRequestFinalize;

		[EventCategory("Activation Setup Events")]
		[Tooltip("Called when a request is sent. This occurs immediately once data is being transmitted to the endpoint.")]
		[FormerlySerializedAs("OnRequestCreated")]
		[SerializeField]
		[HideInInspector]
		private WitRequestCreatedEvent _onRequestCreated = new WitRequestCreatedEvent();

		[EventCategory("Activation Setup Events")]
		[Tooltip("Called when a request is sent. This occurs immediately once data is being transmitted to the endpoint.")]
		[SerializeField]
		private VoiceServiceRequestEvent _onSend = new VoiceServiceRequestEvent();

		protected const string EVENT_CATEGORY_ACTIVATION_INFO = "Activation Info Events";

		[EventCategory("Activation Info Events")]
		[Tooltip("Fired when the minimum wake threshold is hit after an activation.  Not called for ActivateImmediately")]
		[FormerlySerializedAs("OnMinimumWakeThresholdHit")]
		[SerializeField]
		private UnityEvent _onMinimumWakeThresholdHit = new UnityEvent();

		[EventCategory("Activation Info Events")]
		[Tooltip("Fired when recording stops, the minimum volume threshold was hit, and data is being sent to the server.")]
		[FormerlySerializedAs("OnMicDataSent")]
		[SerializeField]
		private UnityEvent _onMicDataSent = new UnityEvent();

		[EventCategory("Activation Info Events")]
		[Tooltip("The Deactivate() method has been called ending the current activation.")]
		[FormerlySerializedAs("OnStoppedListeningDueToDeactivation")]
		[SerializeField]
		private UnityEvent _onStoppedListeningDueToDeactivation = new UnityEvent();

		[EventCategory("Activation Info Events")]
		[Tooltip("Called when the microphone input volume has been below the volume threshold for the specified duration and microphone data is no longer being collected")]
		[FormerlySerializedAs("OnStoppedListeningDueToInactivity")]
		[SerializeField]
		private UnityEvent _onStoppedListeningDueToInactivity = new UnityEvent();

		[EventCategory("Activation Info Events")]
		[Tooltip("The microphone has stopped recording because maximum recording time has been hit for this activation")]
		[FormerlySerializedAs("OnStoppedListeningDueToTimeout")]
		[SerializeField]
		private UnityEvent _onStoppedListeningDueToTimeout = new UnityEvent();

		protected const string EVENT_CATEGORY_ACTIVATION_CANCELATION = "Activation Cancelation Events";

		[EventCategory("Activation Cancelation Events")]
		[Tooltip("Called when the activation is about to be aborted by a direct user interaction via DeactivateAndAbort.")]
		[FormerlySerializedAs("OnAborting")]
		[SerializeField]
		private UnityEvent _onAborting = new UnityEvent();

		[EventCategory("Activation Cancelation Events")]
		[Tooltip("Called when the activation stopped because the network request was aborted. This can be via a timeout or call to DeactivateAndAbort.")]
		[FormerlySerializedAs("OnAborted")]
		[SerializeField]
		private UnityEvent _onAborted = new UnityEvent();

		[EventCategory("Activation Cancelation Events")]
		[Tooltip("Called when a request has been canceled either prior to or after a request has begun transmission.  Returns the cancelation reason.")]
		[FormerlySerializedAs("OnCanceled")]
		[SerializeField]
		private WitTranscriptionEvent _onCanceled = new WitTranscriptionEvent();

		protected const string EVENT_CATEGORY_ACTIVATION_RESPONSE = "Activation Response Events";

		[EventCategory("Activation Response Events")]
		[Tooltip("Called when raw text response is returned from Wit.ai")]
		[FormerlySerializedAs("OnRawResponse")]
		[SerializeField]
		[HideInInspector]
		private WitTranscriptionEvent _onRawResponse = new WitTranscriptionEvent();

		[EventCategory("Activation Response Events")]
		[Tooltip("Called when response from Wit.ai has been received from partial transcription")]
		[FormerlySerializedAs("OnPartialResponse")]
		[SerializeField]
		[HideInInspector]
		private WitResponseEvent _onPartialResponse = new WitResponseEvent();

		[EventCategory("Activation Response Events")]
		[Tooltip("Called when a response from Wit.ai has been received")]
		[FormerlySerializedAs("OnResponse")]
		[FormerlySerializedAs("onResponse")]
		[SerializeField]
		private WitResponseEvent _onResponse = new WitResponseEvent();

		[EventCategory("Activation Response Events")]
		[Tooltip("Called when there was an error with a WitRequest  or the RuntimeConfiguration is not properly configured.")]
		[FormerlySerializedAs("OnError")]
		[FormerlySerializedAs("onError")]
		[SerializeField]
		private WitErrorEvent _onError = new WitErrorEvent();

		[EventCategory("Activation Response Events")]
		[Tooltip("Called when a request has completed and all response and error callbacks have fired.  This is not called if the request was aborted.")]
		[FormerlySerializedAs("OnRequestCompleted")]
		[SerializeField]
		private UnityEvent _onRequestCompleted = new UnityEvent();

		[EventCategory("Activation Response Events")]
		[Tooltip("Called when a request has been canceled, failed, or successfully completed")]
		[FormerlySerializedAs("OnComplete")]
		[SerializeField]
		private VoiceServiceRequestEvent _onComplete = new VoiceServiceRequestEvent();

		protected const string EVENT_CATEGORY_AUDIO_EVENTS = "Audio Events";

		[EventCategory("Audio Events")]
		[Tooltip("Called when the microphone has started collecting data collecting data to be sent to Wit.ai. There may be some buffering before data transmission starts.")]
		[FormerlySerializedAs("OnStartListening")]
		[FormerlySerializedAs("onStart")]
		[SerializeField]
		private UnityEvent _onStartListening = new UnityEvent();

		[EventCategory("Audio Events")]
		[Tooltip("Called when the voice service is no longer collecting data from the microphone")]
		[FormerlySerializedAs("OnStoppedListening")]
		[FormerlySerializedAs("onStopped")]
		[SerializeField]
		private UnityEvent _onStoppedListening = new UnityEvent();

		[EventCategory("Audio Events")]
		[Tooltip("Called when the volume level of the mic input has changed")]
		[FormerlySerializedAs("OnMicLevelChanged")]
		[SerializeField]
		private WitMicLevelChangedEvent _onMicLevelChanged = new WitMicLevelChangedEvent();

		protected const string EVENT_CATEGORY_TRANSCRIPTION_EVENTS = "Transcription Events";

		[EventCategory("Transcription Events")]
		[Tooltip("Message fired when a partial transcription has been received.")]
		[FormerlySerializedAs("onPartialTranscription")]
		[FormerlySerializedAs("OnPartialTranscription")]
		[SerializeField]
		private WitTranscriptionEvent _onPartialTranscription = new WitTranscriptionEvent();

		[FormerlySerializedAs("OnFullTranscription")]
		[EventCategory("Transcription Events")]
		[Tooltip("Message received when a complete transcription is received.")]
		[FormerlySerializedAs("onFullTranscription")]
		[FormerlySerializedAs("OnFullTranscription")]
		[SerializeField]
		private WitTranscriptionEvent _onFullTranscription = new WitTranscriptionEvent();

		[Tooltip("Called on request transcription while audio is still being analyzed.  Also returns client user id as first parameter")]
		[SerializeField]
		private UserTranscriptionRequestEvent _onUserPartialTranscription = Activator.CreateInstance<UserTranscriptionRequestEvent>();

		[Tooltip("Called on request transcription when audio has been completely transferred.  Also returns client user id as first parameter")]
		[SerializeField]
		private UserTranscriptionRequestEvent _onUserFullTranscription = Activator.CreateInstance<UserTranscriptionRequestEvent>();

		private HashSet<SpeechEvents> _listeners = new HashSet<SpeechEvents>();
	}
}
