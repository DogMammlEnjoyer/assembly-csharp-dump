using System;
using Meta.Voice;
using Meta.WitAi.Json;
using UnityEngine.Events;

namespace Meta.WitAi.Requests
{
	[Serializable]
	public class VoiceServiceRequestEvents : NLPRequestEvents<VoiceServiceRequestEvent, WitResponseNode>
	{
		public void AddListeners(VoiceServiceRequestEvents events)
		{
			this.SetListeners(events, true);
		}

		public void RemoveListeners(VoiceServiceRequestEvents events)
		{
			this.SetListeners(events, false);
		}

		public void SetListeners(VoiceServiceRequestEvents events, bool add)
		{
			base.OnInit.SetListener(new UnityAction<VoiceServiceRequest>(events.OnInit.Invoke), add);
			base.OnStateChange.SetListener(new UnityAction<VoiceServiceRequest>(events.OnStateChange.Invoke), add);
			base.OnAudioInputStateChange.SetListener(new UnityAction<VoiceServiceRequest>(events.OnAudioInputStateChange.Invoke), add);
			base.OnStartListening.SetListener(new UnityAction<VoiceServiceRequest>(events.OnStartListening.Invoke), add);
			base.OnStopListening.SetListener(new UnityAction<VoiceServiceRequest>(events.OnStopListening.Invoke), add);
			base.OnAudioActivation.SetListener(new UnityAction<VoiceServiceRequest>(events.OnAudioActivation.Invoke), add);
			base.OnAudioDeactivation.SetListener(new UnityAction<VoiceServiceRequest>(events.OnAudioDeactivation.Invoke), add);
			base.OnSend.SetListener(new UnityAction<VoiceServiceRequest>(events.OnSend.Invoke), add);
			base.OnRawResponse.SetListener(new UnityAction<string>(events.OnRawResponse.Invoke), add);
			base.OnPartialTranscription.SetListener(new UnityAction<string>(events.OnPartialTranscription.Invoke), add);
			base.OnFullTranscription.SetListener(new UnityAction<string>(events.OnFullTranscription.Invoke), add);
			base.OnPartialResponse.SetListener(new UnityAction<WitResponseNode>(events.OnPartialResponse.Invoke), add);
			base.OnFullResponse.SetListener(new UnityAction<WitResponseNode>(events.OnFullResponse.Invoke), add);
			base.OnDownloadProgressChange.SetListener(new UnityAction<VoiceServiceRequest>(events.OnDownloadProgressChange.Invoke), add);
			base.OnUploadProgressChange.SetListener(new UnityAction<VoiceServiceRequest>(events.OnUploadProgressChange.Invoke), add);
			base.OnCancel.SetListener(new UnityAction<VoiceServiceRequest>(events.OnCancel.Invoke), add);
			base.OnFailed.SetListener(new UnityAction<VoiceServiceRequest>(events.OnFailed.Invoke), add);
			base.OnSuccess.SetListener(new UnityAction<VoiceServiceRequest>(events.OnSuccess.Invoke), add);
			base.OnComplete.SetListener(new UnityAction<VoiceServiceRequest>(events.OnComplete.Invoke), add);
		}
	}
}
