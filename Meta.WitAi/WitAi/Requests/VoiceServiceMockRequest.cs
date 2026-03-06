using System;
using Meta.Voice;
using Meta.WitAi.Configuration;
using Meta.WitAi.Json;

namespace Meta.WitAi.Requests
{
	public class VoiceServiceMockRequest : VoiceServiceRequest
	{
		public VoiceServiceMockRequest(NLPRequestInputType newInputType, WitRequestOptions newOptions, VoiceServiceRequestEvents newEvents) : base(newInputType, newOptions, newEvents)
		{
		}

		protected override bool DecodeRawResponses
		{
			get
			{
				return true;
			}
		}

		protected override bool HasSentAudio()
		{
			return false;
		}

		protected override void HandleAudioActivation()
		{
			this.SetAudioInputState(VoiceAudioInputState.On);
		}

		protected override void HandleAudioDeactivation()
		{
			this.SetAudioInputState(VoiceAudioInputState.Off);
		}

		protected override void HandleSend()
		{
		}

		protected override void HandleCancel()
		{
		}

		public void SetRawResponse(string jsonText, bool final = false)
		{
			if (base.State != VoiceRequestState.Transmitting)
			{
				VLog.W("Cannot apply a raw response unless transmitting", null);
				return;
			}
			this.HandleRawResponse(jsonText, final);
		}

		public void SetTranscription(string newTranscription, bool full = false)
		{
			if (base.State != VoiceRequestState.Transmitting)
			{
				VLog.W("Cannot set transcription unless transmitting", null);
				return;
			}
			this.ApplyTranscription(newTranscription, full);
		}

		public void SetResponseData(WitResponseNode responseData, bool final = false)
		{
			if (base.State != VoiceRequestState.Transmitting)
			{
				VLog.W("Cannot set decoded response data unless transmitting", null);
				return;
			}
			this.ApplyResponseData(responseData, final);
		}

		public void Fail(string error)
		{
			this.Fail(-1, error);
		}

		public void Fail(int statusCode, string error)
		{
			if (base.State != VoiceRequestState.Transmitting)
			{
				VLog.W("Cannot make a request fail unless transmitting", null);
				return;
			}
			this.HandleFailure(statusCode, error);
		}
	}
}
