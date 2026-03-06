using System;
using System.Threading.Tasks;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Requests;
using UnityEngine;

namespace Meta.WitAi
{
	public class Wit : VoiceService, IWitRuntimeConfigProvider, IWitRuntimeConfigSetter
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

		public override bool Active
		{
			get
			{
				return base.Active || (null != this.witService && this.witService.Active);
			}
		}

		public override bool IsRequestActive
		{
			get
			{
				return base.IsRequestActive || (null != this.witService && this.witService.IsRequestActive);
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

		public override string GetSendError()
		{
			WitRuntimeConfiguration runtimeConfiguration = this.RuntimeConfiguration;
			if (!((runtimeConfiguration != null) ? runtimeConfiguration.witConfiguration : null))
			{
				return string.Concat(new string[]
				{
					"Your ",
					base.GetType().Name,
					" \"",
					base.gameObject.name,
					"\" does not have a wit configuration assigned.   Voice interactions are not possible without the configuration."
				});
			}
			if (string.IsNullOrEmpty(this.RuntimeConfiguration.witConfiguration.GetClientAccessToken()))
			{
				return "The configuration \"" + this.RuntimeConfiguration.witConfiguration.name + "\" is not setup with a valid client access token.   Voice interactions are not possible without the token.";
			}
			return base.GetSendError();
		}

		public override string GetActivateAudioError()
		{
			if (!AudioBuffer.Instance.IsInputAvailable)
			{
				return "No Microphone(s)/recording devices found.  You will be unable to capture audio on this device.";
			}
			return base.GetActivateAudioError();
		}

		public override Task<VoiceServiceRequest> Activate(string text, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			this.SetupRequestParameters(ref requestOptions, ref requestEvents);
			return this.witService.Activate(text, requestOptions, requestEvents);
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
			base.Deactivate();
			this.witService.Deactivate();
		}

		public override void DeactivateAndAbortRequest()
		{
			base.DeactivateAndAbortRequest();
			this.witService.DeactivateAndAbortRequest();
		}

		protected override void Awake()
		{
			base.Awake();
			this.witService = base.gameObject.AddComponent<WitService>();
			this.witService.VoiceEventProvider = this;
			this.witService.TelemetryEventsProvider = this;
			this.witService.ConfigurationProvider = this;
		}

		[SerializeField]
		private WitRuntimeConfiguration witRuntimeConfiguration;

		private WitService witService;
	}
}
