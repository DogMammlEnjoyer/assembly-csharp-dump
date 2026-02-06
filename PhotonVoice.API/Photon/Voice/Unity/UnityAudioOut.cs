using System;
using UnityEngine;

namespace Photon.Voice.Unity
{
	public class UnityAudioOut : AudioOutDelayControl<float>
	{
		public UnityAudioOut(AudioSource audioSource, AudioOutDelayControl.PlayDelayConfig playDelayConfig, ILogger logger, string logPrefix, bool debugInfo) : base(true, playDelayConfig, logger, "[PV] [Unity] AudioOut" + ((logPrefix == "") ? "" : (" " + logPrefix)), debugInfo)
		{
			this.source = audioSource;
		}

		public override int OutPos
		{
			get
			{
				if (!this.source.clip)
				{
					return 0;
				}
				return this.source.timeSamples;
			}
		}

		public override void OutCreate(int frequency, int channels, int bufferSamples)
		{
			Debug.Log("UnityAudioOut :: OutCreate " + this.source.gameObject.name, this.source);
			this.source.loop = true;
			this.clip = AudioClip.Create("UnityAudioOut", bufferSamples, channels, frequency, false);
			this.source.clip = this.clip;
		}

		public override void OutStart()
		{
			if (this.source.clip != null)
			{
				this.source.Play();
			}
		}

		public override void OutWrite(float[] data, int offsetSamples)
		{
			this.clip.SetData(data, offsetSamples);
		}

		public override void Stop()
		{
			base.Stop();
			this.source.Stop();
			if (this.source != null)
			{
				this.source.clip = null;
				Object.Destroy(this.clip);
				this.clip = null;
			}
		}

		public override void ToggleAudioSource(bool toggle)
		{
			if (toggle)
			{
				this.source.clip = this.clip;
				this.source.Play();
				return;
			}
			this.source.Stop();
			this.source.clip = null;
		}

		protected readonly AudioSource source;

		protected AudioClip clip;
	}
}
