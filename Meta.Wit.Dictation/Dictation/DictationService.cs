using System;
using Meta.WitAi.Configuration;
using Meta.WitAi.Dictation.Events;
using Meta.WitAi.Events;
using Meta.WitAi.Events.UnityEventListeners;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Requests;
using UnityEngine;

namespace Meta.WitAi.Dictation
{
	public abstract class DictationService : BaseSpeechService, IDictationService, ITelemetryEventsProvider, IAudioEventProvider, ITranscriptionEventProvider
	{
		public virtual bool IsRequestActive
		{
			get
			{
				return this.Active;
			}
		}

		public abstract ITranscriptionProvider TranscriptionProvider { get; set; }

		public abstract bool MicActive { get; }

		public virtual DictationEvents DictationEvents
		{
			get
			{
				return this.dictationEvents;
			}
			set
			{
				this.dictationEvents = value;
			}
		}

		protected override SpeechEvents GetSpeechEvents()
		{
			return this.DictationEvents;
		}

		public TelemetryEvents TelemetryEvents
		{
			get
			{
				return this.telemetryEvents;
			}
			set
			{
				this.telemetryEvents = value;
			}
		}

		public IAudioInputEvents AudioEvents
		{
			get
			{
				return this.DictationEvents;
			}
		}

		public ITranscriptionEvent TranscriptionEvents
		{
			get
			{
				return this.DictationEvents;
			}
		}

		protected abstract bool ShouldSendMicData { get; }

		public void Activate()
		{
			this.Activate(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), new VoiceServiceRequestEvents());
		}

		public void Activate(WitRequestOptions requestOptions)
		{
			this.Activate(requestOptions, new VoiceServiceRequestEvents());
		}

		public VoiceServiceRequest Activate(VoiceServiceRequestEvents requestEvents)
		{
			return this.Activate(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), requestEvents);
		}

		public abstract VoiceServiceRequest Activate(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents);

		public void ActivateImmediately()
		{
			this.ActivateImmediately(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), new VoiceServiceRequestEvents());
		}

		public void ActivateImmediately(WitRequestOptions requestOptions)
		{
			this.ActivateImmediately(requestOptions, new VoiceServiceRequestEvents());
		}

		public VoiceServiceRequest ActivateImmediately(VoiceServiceRequestEvents requestEvents)
		{
			return this.ActivateImmediately(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), requestEvents);
		}

		public abstract VoiceServiceRequest ActivateImmediately(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents);

		public abstract void Cancel();

		protected virtual void Awake()
		{
			if (!base.GetComponent<AudioEventListener>())
			{
				base.gameObject.AddComponent<AudioEventListener>();
			}
			if (!base.GetComponent<TranscriptionEventListener>())
			{
				base.gameObject.AddComponent<TranscriptionEventListener>();
			}
		}

		[Tooltip("Events that will fire before, during and after an activation")]
		[SerializeField]
		protected DictationEvents dictationEvents = new DictationEvents();

		protected TelemetryEvents telemetryEvents = new TelemetryEvents();
	}
}
