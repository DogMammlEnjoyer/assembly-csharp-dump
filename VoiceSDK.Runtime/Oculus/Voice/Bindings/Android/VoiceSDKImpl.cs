using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meta.Voice;
using Meta.WitAi;
using Meta.WitAi.Configuration;
using Meta.WitAi.Events;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Requests;
using Oculus.Voice.Core.Bindings.Android;
using Oculus.Voice.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Voice.Bindings.Android
{
	public class VoiceSDKImpl : BaseAndroidConnectionImpl<VoiceSDKBinding>, IPlatformVoiceService, IVoiceService, IVoiceEventProvider, ITelemetryEventsProvider, IVoiceActivationHandler, IVCBindingEvents
	{
		public VoiceSDKImpl(IVoiceService baseVoiceService) : base("com.oculus.assistant.api.unity.immersivevoicecommands.UnityIVCServiceFragment")
		{
			this._baseVoiceService = baseVoiceService;
		}

		public bool UsePlatformIntegrations
		{
			get
			{
				return true;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool PlatformSupportsWit
		{
			get
			{
				return this.service.PlatformSupportsWit && this._isServiceAvailable;
			}
		}

		public bool Active
		{
			get
			{
				return this.service.Active && this._isActive;
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
				return this.service.MicActive;
			}
		}

		public void SetRuntimeConfiguration(WitRuntimeConfiguration configuration)
		{
			this.service.SetRuntimeConfiguration(configuration);
		}

		public HashSet<VoiceServiceRequest> Requests { get; } = new HashSet<VoiceServiceRequest>();

		public ITranscriptionProvider TranscriptionProvider { get; set; }

		public bool CanActivateAudio()
		{
			return true;
		}

		public bool CanSend()
		{
			return true;
		}

		public override void Connect(string version)
		{
			base.Connect(version);
			this.eventBinding = new VoiceSDKListenerBinding(this, this);
			this.eventBinding.VoiceEvents.OnStoppedListening.AddListener(new UnityAction(this.OnStoppedListening));
			this.service.SetListener(this.eventBinding);
			this.service.Connect();
			Debug.Log("Platform integration initialization complete. Platform integrations are " + (this.PlatformSupportsWit ? "active" : "inactive"));
		}

		public override void Disconnect()
		{
			base.Disconnect();
			if (this.eventBinding != null)
			{
				this.eventBinding.VoiceEvents.OnStoppedListening.RemoveListener(new UnityAction(this.OnStoppedListening));
			}
		}

		private void OnStoppedListening()
		{
			this._isActive = false;
		}

		public Task<VoiceServiceRequest> Activate(string text, WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			if (requestOptions == null)
			{
				requestOptions = new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>());
			}
			requestOptions.Text = text;
			VoiceServiceRequest request = this.GetRequest(requestOptions, requestEvents, NLPRequestInputType.Text, false);
			request.Send();
			return Task.FromResult<VoiceServiceRequest>(request);
		}

		public VoiceServiceRequest Activate(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			if (this._isActive)
			{
				return null;
			}
			this._isActive = true;
			if (requestOptions == null)
			{
				requestOptions = new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>());
			}
			VoiceServiceRequest request = this.GetRequest(requestOptions, requestEvents, NLPRequestInputType.Audio, false);
			request.ActivateAudio();
			return request;
		}

		public VoiceServiceRequest ActivateImmediately(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents)
		{
			if (this._isActive)
			{
				return null;
			}
			this._isActive = true;
			if (requestOptions == null)
			{
				requestOptions = new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>());
			}
			VoiceServiceRequest request = this.GetRequest(requestOptions, requestEvents, NLPRequestInputType.Audio, true);
			request.ActivateAudio();
			return request;
		}

		public void Deactivate()
		{
			this._isActive = false;
			foreach (VoiceServiceRequest voiceServiceRequest in this.Requests)
			{
				if (voiceServiceRequest.InputType == NLPRequestInputType.Audio)
				{
					voiceServiceRequest.DeactivateAudio();
				}
			}
		}

		public void DeactivateAndAbortRequest()
		{
			this._isActive = false;
			foreach (VoiceServiceRequest voiceServiceRequest in this.Requests)
			{
				if (voiceServiceRequest.InputType == NLPRequestInputType.Audio)
				{
					voiceServiceRequest.Cancel("Request was cancelled.");
				}
			}
		}

		public void DeactivateAndAbortRequest(VoiceServiceRequest request)
		{
			if (!this.Requests.Contains(request))
			{
				return;
			}
			request.Cancel("Request was cancelled.");
		}

		public void OnServiceNotAvailable(string error, string message)
		{
			this._isActive = false;
			this._isServiceAvailable = false;
			Action onServiceNotAvailableEvent = this.OnServiceNotAvailableEvent;
			if (onServiceNotAvailableEvent == null)
			{
				return;
			}
			onServiceNotAvailableEvent();
		}

		public VoiceEvents VoiceEvents
		{
			get
			{
				return this._baseVoiceService.VoiceEvents;
			}
			set
			{
				this._baseVoiceService.VoiceEvents = value;
			}
		}

		public TelemetryEvents TelemetryEvents
		{
			get
			{
				return this._baseVoiceService.TelemetryEvents;
			}
			set
			{
				this._baseVoiceService.TelemetryEvents = value;
			}
		}

		private VoiceServiceRequest GetRequest(WitRequestOptions requestOptions, VoiceServiceRequestEvents requestEvents, NLPRequestInputType inputType, bool audioImmediate = false)
		{
			VoiceSDKImplRequest voiceSDKImplRequest = new VoiceSDKImplRequest(this.service, inputType, audioImmediate, requestOptions, requestEvents);
			this.Requests.Add(voiceSDKImplRequest);
			return voiceSDKImplRequest;
		}

		private bool _isServiceAvailable = true;

		public Action OnServiceNotAvailableEvent;

		private IVoiceService _baseVoiceService;

		private bool _isActive;

		private VoiceSDKListenerBinding eventBinding;
	}
}
