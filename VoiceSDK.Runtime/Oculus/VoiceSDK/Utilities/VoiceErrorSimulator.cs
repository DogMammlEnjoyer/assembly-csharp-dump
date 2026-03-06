using System;
using System.Collections.Concurrent;
using Meta.Voice;
using Meta.WitAi;
using Meta.WitAi.Requests;
using Meta.WitAi.TTS;
using Oculus.Voice;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.VoiceSDK.Utilities
{
	public class VoiceErrorSimulator : MonoBehaviour
	{
		protected virtual void OnEnable()
		{
			this.RefreshServices();
			this.SetListeners(true);
		}

		protected virtual void RefreshServices()
		{
			if (this.voiceServices == null)
			{
				VoiceService[] componentsInChildren = base.gameObject.GetComponentsInChildren<AppVoiceExperience>(true);
				this.voiceServices = componentsInChildren;
			}
			if (this.ttsService == null)
			{
				this.ttsService = base.gameObject.GetComponentInChildren<TTSService>(true);
			}
		}

		private void SetListeners(bool add)
		{
			if (this.voiceServices == null)
			{
				return;
			}
			VoiceService[] array = this.voiceServices;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].VoiceEvents.OnRequestInitialized.SetListener(new UnityAction<VoiceServiceRequest>(this.SimulateVoiceRequestError), add);
			}
		}

		protected virtual void OnDisable()
		{
			this.SetListeners(false);
		}

		public void SimulateError(VoiceErrorRequestType requestType, VoiceErrorSimulationType simulationType)
		{
			if (requestType == VoiceErrorRequestType.TextToSpeechRequest)
			{
				this.ttsService.SimulatedErrorType = simulationType;
				return;
			}
			this._requests[requestType] = simulationType;
		}

		private void SimulateVoiceRequestError(VoiceServiceRequest request)
		{
			if (!request.IsLocalRequest)
			{
				return;
			}
			VoiceErrorRequestType voiceErrorRequestType = (VoiceErrorRequestType)(-1);
			VoiceErrorSimulationType voiceErrorSimulationType = (VoiceErrorSimulationType)(-1);
			if (request.InputType == NLPRequestInputType.Audio && this._requests.ContainsKey(VoiceErrorRequestType.AudioInputAnalysisRequest))
			{
				voiceErrorRequestType = VoiceErrorRequestType.AudioInputAnalysisRequest;
				this._requests.TryRemove(voiceErrorRequestType, out voiceErrorSimulationType);
			}
			else if (request.InputType == NLPRequestInputType.Text && this._requests.ContainsKey(VoiceErrorRequestType.TextInputAnalysisRequest))
			{
				voiceErrorRequestType = VoiceErrorRequestType.TextInputAnalysisRequest;
				this._requests.TryRemove(voiceErrorRequestType, out voiceErrorSimulationType);
			}
			if (voiceErrorRequestType == (VoiceErrorRequestType)(-1) || voiceErrorSimulationType == (VoiceErrorSimulationType)(-1))
			{
				return;
			}
			request.SimulateError(voiceErrorSimulationType);
		}

		public VoiceService[] voiceServices;

		public TTSService ttsService;

		private ConcurrentDictionary<VoiceErrorRequestType, VoiceErrorSimulationType> _requests = new ConcurrentDictionary<VoiceErrorRequestType, VoiceErrorSimulationType>();
	}
}
