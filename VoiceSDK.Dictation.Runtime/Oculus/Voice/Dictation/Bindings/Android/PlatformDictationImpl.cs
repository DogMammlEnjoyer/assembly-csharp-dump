using System;
using Meta.WitAi;
using Meta.WitAi.Configuration;
using Meta.WitAi.Dictation;
using Meta.WitAi.Dictation.Events;
using Meta.WitAi.Events;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Requests;
using Oculus.Voice.Core.Bindings.Android;

namespace Oculus.Voice.Dictation.Bindings.Android
{
	public class PlatformDictationImpl : BaseAndroidConnectionImpl<PlatformDictationSDKBinding>, IDictationService, ITelemetryEventsProvider, IServiceEvents
	{
		public PlatformDictationImpl(IDictationService dictationService) : base("com.oculus.assistant.api.unity.dictation.UnityDictationServiceFragment")
		{
			this._baseService = dictationService;
		}

		public bool PlatformSupportsDictation
		{
			get
			{
				return this.service != null && this.service.IsSupported && this._serviceAvailable;
			}
		}

		public bool Active
		{
			get
			{
				return this.service.Active;
			}
		}

		public bool IsRequestActive
		{
			get
			{
				return this.service.IsRequestActive;
			}
		}

		public bool MicActive
		{
			get
			{
				return this.service.Active;
			}
		}

		public ITranscriptionProvider TranscriptionProvider { get; set; }

		public DictationEvents DictationEvents
		{
			get
			{
				return this._baseService.DictationEvents;
			}
			set
			{
				this._baseService.DictationEvents = value;
			}
		}

		public TelemetryEvents TelemetryEvents
		{
			get
			{
				return this._baseService.TelemetryEvents;
			}
			set
			{
				this._baseService.TelemetryEvents = value;
			}
		}

		public override void Connect(string version)
		{
			base.Connect(version);
			if (this.service != null)
			{
				this._listenerBinding = new DictationListenerBinding(this, this);
				this.service.SetListener(this._listenerBinding);
			}
		}

		public override void Disconnect()
		{
			base.Disconnect();
		}

		public void SetDictationRuntimeConfiguration(WitDictationRuntimeConfiguration configuration)
		{
			this._dictationRuntimeConfiguration = configuration;
		}

		private void Activate()
		{
			this.service.StartDictation(new DictationConfigurationBinding(this._dictationRuntimeConfiguration));
		}

		public VoiceServiceRequest Activate(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			this.Activate();
			return null;
		}

		public VoiceServiceRequest ActivateImmediately(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			this.Activate();
			return null;
		}

		public void Deactivate()
		{
			this.service.StopDictation();
		}

		public void Cancel()
		{
			this.service.StopDictation();
		}

		public void OnServiceNotAvailable(string error, string message)
		{
			this._serviceAvailable = false;
			Action onServiceNotAvailableEvent = this.OnServiceNotAvailableEvent;
			if (onServiceNotAvailableEvent == null)
			{
				return;
			}
			onServiceNotAvailableEvent();
		}

		private readonly IDictationService _baseService;

		private bool _serviceAvailable = true;

		private WitDictationRuntimeConfiguration _dictationRuntimeConfiguration;

		private DictationListenerBinding _listenerBinding;

		public Action OnServiceNotAvailableEvent;
	}
}
