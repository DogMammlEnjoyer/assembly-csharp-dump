using System;
using Meta.WitAi.Events;
using Meta.WitAi.Events.UnityEventListeners;
using Meta.WitAi.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.ServiceReferences
{
	public class CombinedAudioEventReference : AudioInputServiceReference, IAudioInputEvents
	{
		public override IAudioInputEvents AudioEvents
		{
			get
			{
				return this;
			}
		}

		private void Awake()
		{
			this._sourceListeners = Object.FindObjectsByType<AudioEventListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		}

		private void OnEnable()
		{
			foreach (AudioEventListener audioEventListener in this._sourceListeners)
			{
				audioEventListener.OnMicAudioLevelChanged.AddListener(new UnityAction<float>(this.OnMicAudioLevelChanged.Invoke));
				audioEventListener.OnMicStartedListening.AddListener(new UnityAction(this.OnMicStartedListening.Invoke));
				audioEventListener.OnMicStoppedListening.AddListener(new UnityAction(this.OnMicStoppedListening.Invoke));
			}
		}

		private void OnDisable()
		{
			foreach (AudioEventListener audioEventListener in this._sourceListeners)
			{
				audioEventListener.OnMicAudioLevelChanged.RemoveListener(new UnityAction<float>(this.OnMicAudioLevelChanged.Invoke));
				audioEventListener.OnMicStartedListening.RemoveListener(new UnityAction(this.OnMicStartedListening.Invoke));
				audioEventListener.OnMicStoppedListening.RemoveListener(new UnityAction(this.OnMicStoppedListening.Invoke));
			}
		}

		public WitMicLevelChangedEvent OnMicAudioLevelChanged
		{
			get
			{
				return this._onMicAudioLevelChanged;
			}
		}

		public UnityEvent OnMicStartedListening
		{
			get
			{
				return this._onMicStartedListening;
			}
		}

		public UnityEvent OnMicStoppedListening
		{
			get
			{
				return this._onMicStoppedListening;
			}
		}

		private WitMicLevelChangedEvent _onMicAudioLevelChanged = new WitMicLevelChangedEvent();

		private UnityEvent _onMicStartedListening = new UnityEvent();

		private UnityEvent _onMicStoppedListening = new UnityEvent();

		private AudioEventListener[] _sourceListeners;
	}
}
