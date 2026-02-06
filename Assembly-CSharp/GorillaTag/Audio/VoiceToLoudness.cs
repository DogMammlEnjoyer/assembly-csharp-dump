using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	[RequireComponent(typeof(Recorder))]
	public class VoiceToLoudness : MonoBehaviour
	{
		protected void Awake()
		{
			this._recorder = base.GetComponent<Recorder>();
		}

		protected void PhotonVoiceCreated(PhotonVoiceCreatedParams photonVoiceCreatedParams)
		{
			this.CreateProcessVoiceData(photonVoiceCreatedParams.Voice);
		}

		private void CreateProcessVoiceData(LocalVoice voice)
		{
			LocalVoiceAudioFloat localVoiceAudioFloat = voice as LocalVoiceAudioFloat;
			if (localVoiceAudioFloat != null)
			{
				this._photonVoiceCreated = true;
				localVoiceAudioFloat.AddPostProcessor(new IProcessor<float>[]
				{
					new ProcessVoiceDataToLoudness(this)
				});
			}
		}

		private void Update()
		{
			if (this._photonVoiceCreated)
			{
				return;
			}
			if (this._recorder != null && this._recorder.Voice != null)
			{
				this.CreateProcessVoiceData(this._recorder.Voice);
			}
		}

		[NonSerialized]
		public float Loudness;

		private Recorder _recorder;

		private bool _photonVoiceCreated;

		private float _checkVoice;
	}
}
