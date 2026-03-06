using System;
using Meta.WitAi;
using Meta.WitAi.Dictation;
using Meta.WitAi.Dictation.Events;
using Meta.WitAi.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Voice.Dictation.Bindings.Android
{
	public class DictationListenerBinding : AndroidJavaProxy
	{
		private DictationEvents DictationEvents
		{
			get
			{
				return this._dictationService.DictationEvents;
			}
		}

		public DictationListenerBinding(IDictationService dictationService, IServiceEvents serviceEvents) : base("com.oculus.assistant.api.voicesdk.dictation.PlatformDictationListener")
		{
			this._dictationService = dictationService;
			this._serviceEvents = serviceEvents;
		}

		public void onStart(string sessionId)
		{
			UnityEvent onStartListening = this.DictationEvents.OnStartListening;
			if (onStartListening != null)
			{
				onStartListening.Invoke();
			}
			PlatformDictationSession platformDictationSession = new PlatformDictationSession();
			platformDictationSession.dictationService = this._dictationService;
			platformDictationSession.platformSessionId = sessionId;
		}

		public void onMicAudioLevel(string sessionId, int micLevel)
		{
			WitMicLevelChangedEvent onMicAudioLevelChanged = this.DictationEvents.OnMicAudioLevelChanged;
			if (onMicAudioLevelChanged == null)
			{
				return;
			}
			onMicAudioLevelChanged.Invoke((float)micLevel / 100f);
		}

		public void onPartialTranscription(string sessionId, string transcription)
		{
			WitTranscriptionEvent onPartialTranscription = this.DictationEvents.OnPartialTranscription;
			if (onPartialTranscription == null)
			{
				return;
			}
			onPartialTranscription.Invoke(transcription);
		}

		public void onFinalTranscription(string sessionId, string transcription)
		{
			WitTranscriptionEvent onFullTranscription = this.DictationEvents.OnFullTranscription;
			if (onFullTranscription == null)
			{
				return;
			}
			onFullTranscription.Invoke(transcription);
		}

		public void onError(string sessionId, string errorType, string errorMessage)
		{
			WitErrorEvent onError = this.DictationEvents.OnError;
			if (onError == null)
			{
				return;
			}
			onError.Invoke(errorType, errorMessage);
		}

		public void onStopped(string sessionId)
		{
			UnityEvent onStoppedListening = this.DictationEvents.OnStoppedListening;
			if (onStoppedListening != null)
			{
				onStoppedListening.Invoke();
			}
			PlatformDictationSession platformDictationSession = new PlatformDictationSession();
			platformDictationSession.dictationService = this._dictationService;
			platformDictationSession.platformSessionId = sessionId;
		}

		public void onServiceNotAvailable(string error, string message)
		{
			VLog.W("Platform dictation service is not available", null);
			this._serviceEvents.OnServiceNotAvailable(error, message);
		}

		private IDictationService _dictationService;

		private IServiceEvents _serviceEvents;
	}
}
