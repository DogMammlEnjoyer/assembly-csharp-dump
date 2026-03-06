using System;
using Meta.WitAi.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.Events.UnityEventListeners
{
	[RequireComponent(typeof(IAudioEventProvider))]
	public class AudioEventListener : MonoBehaviour, IAudioInputEvents
	{
		public WitMicLevelChangedEvent OnMicAudioLevelChanged
		{
			get
			{
				return this.onMicAudioLevelChanged;
			}
		}

		public UnityEvent OnMicStartedListening
		{
			get
			{
				return this.onMicStartedListening;
			}
		}

		public UnityEvent OnMicStoppedListening
		{
			get
			{
				return this.onMicStoppedListening;
			}
		}

		private IAudioInputEvents AudioInputEvents
		{
			get
			{
				if (this._events == null)
				{
					IAudioEventProvider component = base.GetComponent<IAudioEventProvider>();
					if (component != null)
					{
						this._events = component.AudioEvents;
					}
				}
				return this._events;
			}
		}

		private void OnEnable()
		{
			IAudioInputEvents audioInputEvents = this.AudioInputEvents;
			if (audioInputEvents != null)
			{
				audioInputEvents.OnMicAudioLevelChanged.AddListener(new UnityAction<float>(this.onMicAudioLevelChanged.Invoke));
				audioInputEvents.OnMicStartedListening.AddListener(new UnityAction(this.onMicStartedListening.Invoke));
				audioInputEvents.OnMicStoppedListening.AddListener(new UnityAction(this.onMicStoppedListening.Invoke));
			}
		}

		private void OnDisable()
		{
			IAudioInputEvents audioInputEvents = this.AudioInputEvents;
			if (audioInputEvents != null)
			{
				audioInputEvents.OnMicAudioLevelChanged.RemoveListener(new UnityAction<float>(this.onMicAudioLevelChanged.Invoke));
				audioInputEvents.OnMicStartedListening.RemoveListener(new UnityAction(this.onMicStartedListening.Invoke));
				audioInputEvents.OnMicStoppedListening.RemoveListener(new UnityAction(this.onMicStoppedListening.Invoke));
			}
		}

		[SerializeField]
		private WitMicLevelChangedEvent onMicAudioLevelChanged = new WitMicLevelChangedEvent();

		[SerializeField]
		private UnityEvent onMicStartedListening = new UnityEvent();

		[SerializeField]
		private UnityEvent onMicStoppedListening = new UnityEvent();

		private IAudioInputEvents _events;
	}
}
