using System;
using System.IO;
using Meta.Voice.Net.WebSockets;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Dictation.Events;
using Meta.WitAi.Events;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Requests;
using UnityEngine;

namespace Meta.WitAi.Dictation
{
	public class WitDictation : DictationService, IWitRuntimeConfigProvider, IVoiceEventProvider, IVoiceServiceRequestProvider, IWitConfigurationProvider
	{
		public WitRuntimeConfiguration RuntimeConfiguration
		{
			get
			{
				return this.witRuntimeConfiguration;
			}
			set
			{
				this.witRuntimeConfiguration = value;
			}
		}

		public WitConfiguration Configuration
		{
			get
			{
				WitRuntimeConfiguration runtimeConfiguration = this.RuntimeConfiguration;
				if (runtimeConfiguration == null)
				{
					return null;
				}
				return runtimeConfiguration.witConfiguration;
			}
		}

		public override bool Active
		{
			get
			{
				return null != this.witService && this.witService.Active;
			}
		}

		public override bool IsRequestActive
		{
			get
			{
				return null != this.witService && this.witService.IsRequestActive;
			}
		}

		public override ITranscriptionProvider TranscriptionProvider
		{
			get
			{
				return this.witService.TranscriptionProvider;
			}
			set
			{
				this.witService.TranscriptionProvider = value;
			}
		}

		public override bool MicActive
		{
			get
			{
				return null != this.witService && this.witService.MicActive;
			}
		}

		protected override bool ShouldSendMicData
		{
			get
			{
				return this.witRuntimeConfiguration.sendAudioToWit || this.TranscriptionProvider == null;
			}
		}

		public VoiceEvents VoiceEvents
		{
			get
			{
				return this._voiceEvents;
			}
		}

		public override DictationEvents DictationEvents
		{
			get
			{
				return this.dictationEvents;
			}
			set
			{
				DictationEvents dictationEvents = this.dictationEvents;
				this.dictationEvents = value;
				if (base.gameObject.activeSelf)
				{
					this.VoiceEvents.RemoveListener(dictationEvents);
					this.VoiceEvents.AddListener(this.dictationEvents);
				}
			}
		}

		public VoiceServiceRequest CreateRequest(WitRuntimeConfiguration requestSettings, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			WitConfiguration witConfiguration = (requestSettings != null) ? requestSettings.witConfiguration : null;
			if (witConfiguration != null && witConfiguration.RequestType == WitRequestType.WebSocket)
			{
				return WitSocketRequest.GetDictationRequest(witConfiguration, base.GetComponent<WitWebSocketAdapter>(), AudioBuffer.Instance, requestOptions, requestEvents);
			}
			return witConfiguration.CreateDictationRequest(requestOptions, requestEvents);
		}

		public override VoiceServiceRequest Activate(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			this.SetupRequestParameters(ref requestOptions, ref requestEvents);
			return this.witService.Activate(requestOptions, requestEvents);
		}

		public override VoiceServiceRequest ActivateImmediately(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			this.SetupRequestParameters(ref requestOptions, ref requestEvents);
			return this.witService.ActivateImmediately(requestOptions, requestEvents);
		}

		public override void Deactivate()
		{
			this.witService.Deactivate();
		}

		public override void Cancel()
		{
			this.witService.DeactivateAndAbortRequest();
		}

		protected override void Awake()
		{
			base.Awake();
			this.witService = base.gameObject.AddComponent<WitService>();
			this.witService.VoiceEventProvider = this;
			this.witService.ConfigurationProvider = this;
			this.witService.RequestProvider = this;
			this.witService.TelemetryEventsProvider = this;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.VoiceEvents.AddListener(this.DictationEvents);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.VoiceEvents.RemoveListener(this.DictationEvents);
		}

		public void TranscribeFile(string fileName)
		{
			WitRequest witRequest = this.CreateRequest(this.witRuntimeConfiguration, new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), new VoiceServiceRequestEvents()) as WitRequest;
			if (witRequest != null)
			{
				byte[] postData = File.ReadAllBytes(fileName);
				witRequest.postData = postData;
				this.witService.ExecuteRequest(witRequest);
			}
		}

		[SerializeField]
		private WitRuntimeConfiguration witRuntimeConfiguration;

		private WitService witService;

		private readonly VoiceEvents _voiceEvents = new VoiceEvents();
	}
}
