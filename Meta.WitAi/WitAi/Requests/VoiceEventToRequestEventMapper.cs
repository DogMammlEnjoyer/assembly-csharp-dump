using System;
using Meta.WitAi.Events;
using Meta.WitAi.Json;
using UnityEngine.Events;

namespace Meta.WitAi.Requests
{
	public class VoiceEventToRequestEventMapper : VoiceServiceRequestEventsWrapper
	{
		public VoiceEvents VoiceEvents
		{
			get
			{
				return this._voiceEvents;
			}
			set
			{
				this._voiceEvents = value;
			}
		}

		public VoiceEventToRequestEventMapper()
		{
		}

		public VoiceEventToRequestEventMapper(VoiceEvents voiceEvents)
		{
			this._voiceEvents = voiceEvents;
		}

		protected override void OnStateChange(VoiceServiceRequest request)
		{
		}

		protected override void OnStopListening(VoiceServiceRequest request)
		{
			this._voiceEvents.OnStoppedListening.Invoke();
		}

		protected override void OnStartListening(VoiceServiceRequest request)
		{
			this._voiceEvents.OnStartListening.Invoke();
		}

		protected override void OnFullTranscription(string transcription)
		{
			this._voiceEvents.OnFullTranscription.Invoke(transcription);
		}

		protected override void OnPartialTranscription(string transcription)
		{
			this._voiceEvents.OnPartialTranscription.Invoke(transcription);
		}

		protected override void OnPartialResponse(WitResponseNode response)
		{
			this._voiceEvents.OnPartialResponse.Invoke(response);
		}

		protected override void OnFullResponse(WitResponseNode response)
		{
			this._voiceEvents.OnResponse.Invoke(response);
		}

		protected override void OnSuccess(VoiceServiceRequest request)
		{
		}

		protected override void OnSend(VoiceServiceRequest request)
		{
		}

		protected override void OnInit(VoiceServiceRequest request)
		{
		}

		protected override void OnFailed(VoiceServiceRequest request)
		{
			this._voiceEvents.OnError.Invoke(request.Results.Message, "Error: " + request.Results.StatusCode.ToString());
		}

		protected override void OnComplete(VoiceServiceRequest request)
		{
			this._voiceEvents.OnComplete.Invoke(request);
		}

		protected override void OnCancel(VoiceServiceRequest request)
		{
			UnityEvent<string> onCanceled = this._voiceEvents.OnCanceled;
			VoiceServiceRequestResults results = request.Results;
			onCanceled.Invoke(((results != null) ? results.Message : null) ?? "");
		}

		private VoiceEvents _voiceEvents;
	}
}
