using System;
using Meta.WitAi.Configuration;
using Meta.WitAi.Events;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Oculus.Voice
{
	public class ObjectVoiceExperience : MonoBehaviour
	{
		private void OnEnable()
		{
			if (!this._voice)
			{
				this._voice = Object.FindAnyObjectByType<AppVoiceExperience>();
			}
			this._events.OnCancel.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleCancel));
			this._events.OnComplete.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleComplete));
			this._events.OnFailed.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleFailed));
			this._events.OnInit.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleInit));
			this._events.OnSend.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleSend));
			this._events.OnSuccess.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleSuccess));
			this._events.OnAudioActivation.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleAudioActivation));
			this._events.OnAudioDeactivation.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleAudioDeactivation));
			this._events.OnPartialTranscription.AddListener(new UnityAction<string>(this.HandlePartialTranscription));
			this._events.OnFullTranscription.AddListener(new UnityAction<string>(this.HandleFullTranscription));
			this._events.OnStateChange.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleStateChange));
			this._events.OnStartListening.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleStartListening));
			this._events.OnStopListening.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleStopListening));
			this._events.OnDownloadProgressChange.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleDownloadProgressChange));
			this._events.OnUploadProgressChange.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleUploadProgressChange));
			this._events.OnAudioInputStateChange.AddListener(new UnityAction<VoiceServiceRequest>(this.HandleAudioInputStateChange));
		}

		private void OnDisable()
		{
			this._events.OnCancel.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleCancel));
			this._events.OnComplete.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleComplete));
			this._events.OnFailed.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleFailed));
			this._events.OnInit.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleInit));
			this._events.OnSend.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleSend));
			this._events.OnSuccess.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleSuccess));
			this._events.OnAudioActivation.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleAudioActivation));
			this._events.OnAudioDeactivation.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleAudioDeactivation));
			this._events.OnFullTranscription.RemoveListener(new UnityAction<string>(this.HandleFullTranscription));
			this._events.OnPartialTranscription.RemoveListener(new UnityAction<string>(this.HandlePartialTranscription));
			this._events.OnStateChange.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleStateChange));
			this._events.OnStartListening.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleStartListening));
			this._events.OnStopListening.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleStopListening));
			this._events.OnDownloadProgressChange.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleDownloadProgressChange));
			this._events.OnUploadProgressChange.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleUploadProgressChange));
			this._events.OnAudioInputStateChange.RemoveListener(new UnityAction<VoiceServiceRequest>(this.HandleAudioInputStateChange));
		}

		private void HandleAudioInputStateChange(VoiceServiceRequest request)
		{
			base.SendMessage("OnAudioInputStateChange", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleUploadProgressChange(VoiceServiceRequest request)
		{
			base.SendMessage("OnUploadProgressChange", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleDownloadProgressChange(VoiceServiceRequest request)
		{
			base.SendMessage("OnDownloadProgressChange", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleStopListening(VoiceServiceRequest request)
		{
			base.SendMessage("OnStopListening", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleStartListening(VoiceServiceRequest request)
		{
			base.SendMessage("OnStartListening", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleStateChange(VoiceServiceRequest request)
		{
			base.SendMessage("OnStateChange", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleFullTranscription(string transcription)
		{
			base.SendMessage("OnFullTranscription", transcription, SendMessageOptions.DontRequireReceiver);
		}

		private void HandlePartialTranscription(string transcription)
		{
			base.SendMessage("OnPartialTranscription", transcription, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleAudioDeactivation(VoiceServiceRequest request)
		{
			base.SendMessage("OnAudioDeactivation", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleAudioActivation(VoiceServiceRequest request)
		{
			base.SendMessage("OnAudioActivation", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleSuccess(VoiceServiceRequest request)
		{
			base.SendMessage("OnSuccess", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleSend(VoiceServiceRequest request)
		{
			base.SendMessage("OnSend", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleInit(VoiceServiceRequest request)
		{
			base.SendMessage("OnInit", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleFailed(VoiceServiceRequest request)
		{
			base.SendMessage("OnFailed", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleComplete(VoiceServiceRequest request)
		{
			VoiceEvents voiceEvents = this._voiceEvents;
			if (voiceEvents != null)
			{
				Meta.WitAi.Events.VoiceServiceRequestEvent onComplete = voiceEvents.OnComplete;
				if (onComplete != null)
				{
					onComplete.Invoke(request);
				}
			}
			base.SendMessage("OnComplete", request, SendMessageOptions.DontRequireReceiver);
		}

		private void HandleCancel(VoiceServiceRequest request)
		{
			VoiceEvents voiceEvents = this._voiceEvents;
			if (voiceEvents != null)
			{
				WitTranscriptionEvent onCanceled = voiceEvents.OnCanceled;
				if (onCanceled != null)
				{
					onCanceled.Invoke("");
				}
			}
			base.SendMessage("OnCancel", request, SendMessageOptions.DontRequireReceiver);
		}

		public void Activate()
		{
			this._activation = this._voice.Activate(new WitRequestOptions(Array.Empty<VoiceServiceRequestOptions.QueryParam>()), this._events);
		}

		public void Deactivate()
		{
			this._activation.Cancel("Request was cancelled.");
		}

		[FormerlySerializedAs("voiceEvents")]
		[SerializeField]
		private VoiceEvents _voiceEvents = new VoiceEvents();

		private AppVoiceExperience _voice;

		private VoiceServiceRequest _activation;

		private VoiceServiceRequestEvents _events = new VoiceServiceRequestEvents();
	}
}
