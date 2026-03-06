using System;
using Meta.WitAi;
using UnityEngine;

namespace Meta.Voice.Audio
{
	[Serializable]
	public class UnityAudioPlayer : BaseAudioPlayer, IAudioSourceProvider
	{
		public AudioSource AudioSource
		{
			get
			{
				return this._audioSource;
			}
		}

		public bool CloneAudioSource
		{
			get
			{
				return this._cloneAudioSource;
			}
		}

		public override bool IsPlaying
		{
			get
			{
				return this.AudioSource != null && this.AudioSource.isPlaying;
			}
		}

		public override bool CanSetElapsedSamples
		{
			get
			{
				return true;
			}
		}

		public override int ElapsedSamples
		{
			get
			{
				if (!(this.AudioSource != null))
				{
					return 0;
				}
				return this.AudioSource.timeSamples;
			}
		}

		private void Awake()
		{
			if (!this.AudioSource)
			{
				this._audioSource = base.gameObject.GetComponentInChildren<AudioSource>();
			}
		}

		public override void Init()
		{
			if (!this.AudioSource)
			{
				this._audioSource = base.gameObject.GetComponentInChildren<AudioSource>();
			}
			if (this.CloneAudioSource)
			{
				AudioSource audioSource = new GameObject(base.gameObject.name + "_AudioOneShot").AddComponent<AudioSource>();
				audioSource.PreloadCopyData<AudioSource>();
				if (this.AudioSource == null)
				{
					audioSource.transform.SetParent(base.transform, false);
					audioSource.spread = 1f;
				}
				else
				{
					audioSource.transform.SetParent(this.AudioSource.transform, false);
					audioSource.Copy(this.AudioSource);
				}
				audioSource.transform.localPosition = Vector3.zero;
				audioSource.transform.localRotation = Quaternion.identity;
				audioSource.transform.localScale = Vector3.one;
				this._audioSource = audioSource;
			}
			this.AudioSource.playOnAwake = false;
		}

		public override string GetPlaybackErrors()
		{
			if (this.AudioSource == null)
			{
				return "Audio source is missing";
			}
			return string.Empty;
		}

		protected override void Play(int offsetSamples = 0)
		{
			AudioClip audioClip = null;
			IAudioClipProvider audioClipProvider = base.ClipStream as IAudioClipProvider;
			if (audioClipProvider != null)
			{
				audioClip = audioClipProvider.Clip;
			}
			else
			{
				RawAudioClipStream rawAudioClipStream = base.ClipStream as RawAudioClipStream;
				if (rawAudioClipStream != null)
				{
					audioClip = AudioClip.Create("CustomClip", rawAudioClipStream.SampleBuffer.Length, rawAudioClipStream.Channels, rawAudioClipStream.SampleRate, true, new AudioClip.PCMReaderCallback(this.OnReadRawSamples), new AudioClip.PCMSetPositionCallback(this.OnSetRawPosition));
					this._local = true;
				}
			}
			if (audioClip == null)
			{
				VLog.E(string.Format("{0} cannot play null AudioClip", base.GetType()), null);
				return;
			}
			this.AudioSource.loop = false;
			this.AudioSource.clip = audioClip;
			this.AudioSource.timeSamples = offsetSamples;
			this.AudioSource.Play();
		}

		private void OnSetRawPosition(int offset)
		{
			this._offset = offset;
		}

		private void OnReadRawSamples(float[] samples)
		{
			int num = 0;
			RawAudioClipStream rawAudioClipStream = base.ClipStream as RawAudioClipStream;
			if (rawAudioClipStream != null)
			{
				int offset = this._offset;
				int b = Mathf.Max(0, rawAudioClipStream.AddedSamples - offset);
				num = Mathf.Min(samples.Length, b);
				if (num > 0)
				{
					Array.Copy(rawAudioClipStream.SampleBuffer, offset, samples, 0, num);
					this._offset += num;
				}
			}
			if (num < samples.Length)
			{
				int num2 = samples.Length - num;
				Array.Clear(samples, num, num2);
				this._offset += num2;
			}
		}

		public override void Pause()
		{
			if (this.IsPlaying)
			{
				this.AudioSource.Pause();
			}
		}

		public override void Resume()
		{
			if (!this.IsPlaying)
			{
				this.AudioSource.UnPause();
			}
		}

		public override void Stop()
		{
			if (this.IsPlaying)
			{
				this.AudioSource.Stop();
			}
			if (this._local)
			{
				if (this.AudioSource.clip != null)
				{
					Object.Destroy(this.AudioSource.clip);
				}
				this._local = false;
			}
			this.AudioSource.clip = null;
			base.Stop();
		}

		[Header("Playback Settings")]
		[Tooltip("Audio source to be used for text-to-speech playback")]
		[SerializeField]
		private AudioSource _audioSource;

		[Tooltip("Duplicates audio source reference on awake instead of using it directly.")]
		[SerializeField]
		private bool _cloneAudioSource;

		private bool _local;

		private int _offset;
	}
}
