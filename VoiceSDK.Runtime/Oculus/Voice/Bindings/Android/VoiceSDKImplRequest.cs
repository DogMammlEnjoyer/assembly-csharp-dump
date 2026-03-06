using System;
using Meta.Voice;
using Meta.WitAi.Configuration;
using Meta.WitAi.Requests;

namespace Oculus.Voice.Bindings.Android
{
	public class VoiceSDKImplRequest : VoiceServiceRequest
	{
		public VoiceSDKBinding Service { get; private set; }

		public bool Immediately { get; private set; }

		protected override bool DecodeRawResponses
		{
			get
			{
				return true;
			}
		}

		public VoiceSDKImplRequest(VoiceSDKBinding newService, NLPRequestInputType newInputType, bool newImmediately, WitRequestOptions newOptions, VoiceServiceRequestEvents newEvents) : base(newInputType, newOptions, newEvents)
		{
			this.Service = newService;
			this.Immediately = newImmediately;
		}

		protected override void HandleAudioActivation()
		{
			if (this.Immediately)
			{
				this.Service.ActivateImmediately(base.Options);
			}
			else
			{
				this.Service.Activate(base.Options);
			}
			this.SetAudioInputState(VoiceAudioInputState.On);
		}

		protected override void HandleAudioDeactivation()
		{
			this.Service.Deactivate(base.Options.RequestId);
			this.SetAudioInputState(VoiceAudioInputState.Off);
		}

		protected override void HandleSend()
		{
			if (base.InputType == NLPRequestInputType.Text)
			{
				this.Service.Activate(base.Options.Text, base.Options);
			}
		}

		protected override void HandleCancel()
		{
			this.Service.DeactivateAndAbortRequest(base.Options.RequestId);
		}

		public void HandlePartialResponse(string responseJson)
		{
			this.HandleRawResponse(responseJson, false);
		}

		public void HandlePartialTranscription(string transcription)
		{
			this.ApplyTranscription(transcription, false);
		}

		public void HandleFullTranscription(string transcription)
		{
			this.ApplyTranscription(transcription, true);
		}

		public void HandleTransmissionBegan()
		{
			if (base.InputType == NLPRequestInputType.Audio)
			{
				this.Send();
			}
		}

		public void HandleCanceled()
		{
			this.HandleCancel();
		}

		public void HandleError(string error, string message, string errorBody)
		{
			this.HandleFailure(error + " - " + message);
		}

		public void HandleResponse(string responseJson)
		{
			this.HandleRawResponse(responseJson, true);
		}
	}
}
