using System;
using Meta.WitAi.Json;
using UnityEngine.Events;

namespace Meta.WitAi.Requests
{
	public class VoiceServiceRequestEventsWrapper
	{
		public void Wrap(VoiceServiceRequestEvents events)
		{
			this.SetListeners(events, true);
		}

		public void Unwrap(VoiceServiceRequestEvents events)
		{
			this.SetListeners(events, false);
		}

		private void SetListeners(VoiceServiceRequestEvents events, bool add)
		{
			events.OnInit.SetListener(new UnityAction<VoiceServiceRequest>(this.OnInit), add);
			events.OnStateChange.SetListener(new UnityAction<VoiceServiceRequest>(this.OnStateChange), add);
			events.OnAudioInputStateChange.SetListener(new UnityAction<VoiceServiceRequest>(this.OnAudioInputStateChange), add);
			events.OnStartListening.SetListener(new UnityAction<VoiceServiceRequest>(this.OnStartListening), add);
			events.OnStopListening.SetListener(new UnityAction<VoiceServiceRequest>(this.OnStopListening), add);
			events.OnAudioActivation.SetListener(new UnityAction<VoiceServiceRequest>(this.OnAudioActivation), add);
			events.OnAudioDeactivation.SetListener(new UnityAction<VoiceServiceRequest>(this.OnAudioDeactivation), add);
			events.OnSend.SetListener(new UnityAction<VoiceServiceRequest>(this.OnSend), add);
			events.OnRawResponse.SetListener(new UnityAction<string>(this.OnRawResponse), add);
			events.OnPartialTranscription.SetListener(new UnityAction<string>(this.OnPartialTranscription), add);
			events.OnFullTranscription.SetListener(new UnityAction<string>(this.OnFullTranscription), add);
			events.OnPartialResponse.SetListener(new UnityAction<WitResponseNode>(this.OnPartialResponse), add);
			events.OnFullResponse.SetListener(new UnityAction<WitResponseNode>(this.OnFullResponse), add);
			events.OnDownloadProgressChange.SetListener(new UnityAction<VoiceServiceRequest>(this.OnDownloadProgressChange), add);
			events.OnUploadProgressChange.SetListener(new UnityAction<VoiceServiceRequest>(this.OnUploadProgressChange), add);
			events.OnCancel.SetListener(new UnityAction<VoiceServiceRequest>(this.OnCancel), add);
			events.OnFailed.SetListener(new UnityAction<VoiceServiceRequest>(this.OnFailed), add);
			events.OnSuccess.SetListener(new UnityAction<VoiceServiceRequest>(this.OnSuccess), add);
			events.OnComplete.SetListener(new UnityAction<VoiceServiceRequest>(this.OnComplete), add);
		}

		protected virtual void OnAudioInputStateChange(VoiceServiceRequest request)
		{
		}

		protected virtual void OnUploadProgressChange(VoiceServiceRequest request)
		{
		}

		protected virtual void OnDownloadProgressChange(VoiceServiceRequest request)
		{
		}

		protected virtual void OnStateChange(VoiceServiceRequest request)
		{
		}

		protected virtual void OnStopListening(VoiceServiceRequest request)
		{
		}

		protected virtual void OnStartListening(VoiceServiceRequest request)
		{
		}

		protected virtual void OnFullTranscription(string transcription)
		{
		}

		protected virtual void OnPartialTranscription(string transcription)
		{
		}

		protected virtual void OnRawResponse(string rawResponse)
		{
		}

		protected virtual void OnPartialResponse(WitResponseNode request)
		{
		}

		protected virtual void OnFullResponse(WitResponseNode request)
		{
		}

		protected virtual void OnAudioDeactivation(VoiceServiceRequest request)
		{
		}

		protected virtual void OnAudioActivation(VoiceServiceRequest request)
		{
		}

		protected virtual void OnSuccess(VoiceServiceRequest request)
		{
		}

		protected virtual void OnSend(VoiceServiceRequest request)
		{
		}

		protected virtual void OnInit(VoiceServiceRequest request)
		{
		}

		protected virtual void OnFailed(VoiceServiceRequest request)
		{
		}

		protected virtual void OnComplete(VoiceServiceRequest request)
		{
		}

		protected virtual void OnCancel(VoiceServiceRequest request)
		{
		}
	}
}
