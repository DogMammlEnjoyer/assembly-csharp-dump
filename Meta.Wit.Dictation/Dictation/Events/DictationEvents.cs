using System;
using Meta.WitAi.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Meta.WitAi.Dictation.Events
{
	[Serializable]
	public class DictationEvents : SpeechEvents
	{
		public DictationSessionEvent OnDictationSessionStarted
		{
			get
			{
				return this._onDictationSessionStarted;
			}
		}

		public DictationSessionEvent OnDictationSessionStopped
		{
			get
			{
				return this._onDictationSessionStopped;
			}
		}

		[Obsolete("Deprecated for 'OnDictationSessionStarted' event")]
		public DictationSessionEvent onDictationSessionStarted
		{
			get
			{
				return this.OnDictationSessionStarted;
			}
		}

		[Obsolete("Deprecated for 'OnDictationSessionStopped' event")]
		public DictationSessionEvent onDictationSessionStopped
		{
			get
			{
				return this.OnDictationSessionStopped;
			}
		}

		[Obsolete("Deprecated for 'OnStartListening' event")]
		public UnityEvent onStart
		{
			get
			{
				return base.OnStartListening;
			}
		}

		[Obsolete("Deprecated for 'OnStoppedListening' event")]
		public UnityEvent onStopped
		{
			get
			{
				return base.OnStoppedListening;
			}
		}

		[Obsolete("Deprecated for 'OnMicLevelChanged' event")]
		public WitMicLevelChangedEvent onMicAudioLevel
		{
			get
			{
				return base.OnMicLevelChanged;
			}
		}

		[Obsolete("Deprecated for 'OnError' event")]
		public WitErrorEvent onError
		{
			get
			{
				return base.OnError;
			}
		}

		[Obsolete("Deprecated for 'OnResponse' event")]
		public WitResponseEvent onResponse
		{
			get
			{
				return base.OnResponse;
			}
		}

		private const string EVENT_CATEGORY_DICTATION_EVENTS = "Dictation Events";

		[Tooltip("Called when an individual dictation session has started. This can include multiple server activations if dictation is set up to automatically reactivate when the server endpoints an utterance.")]
		[EventCategory("Dictation Events")]
		[FormerlySerializedAs("onDictationSessionStarted")]
		[SerializeField]
		[HideInInspector]
		private DictationSessionEvent _onDictationSessionStarted = new DictationSessionEvent();

		[Tooltip("Called when a dictation is completed after Deactivate has been called or auto-reactivate is disabled.")]
		[EventCategory("Dictation Events")]
		[FormerlySerializedAs("onDictationSessionStopped")]
		[SerializeField]
		[HideInInspector]
		private DictationSessionEvent _onDictationSessionStopped = new DictationSessionEvent();
	}
}
