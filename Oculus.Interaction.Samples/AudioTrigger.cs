using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class AudioTrigger : MonoBehaviour
	{
		public float Volume
		{
			get
			{
				return this._volume;
			}
			set
			{
				this._volume = value;
			}
		}

		public MinMaxPair VolumeRandomization
		{
			get
			{
				return this._volumeRandomization;
			}
			set
			{
				this._volumeRandomization = value;
			}
		}

		public float Pitch
		{
			get
			{
				return this._pitch;
			}
			set
			{
				this._pitch = value;
			}
		}

		public MinMaxPair PitchRandomization
		{
			get
			{
				return this._pitchRandomization;
			}
			set
			{
				this._pitchRandomization = value;
			}
		}

		public bool Spatialize
		{
			get
			{
				return this._spatialize;
			}
			set
			{
				this._spatialize = value;
			}
		}

		public bool Loop
		{
			get
			{
				return this._loop;
			}
			set
			{
				this._loop = value;
			}
		}

		public float ChanceToPlay
		{
			get
			{
				return this._chanceToPlay;
			}
			set
			{
				this._chanceToPlay = value;
			}
		}

		protected virtual void Start()
		{
			if (this._audioSource == null)
			{
				this._audioSource = base.gameObject.GetComponent<AudioSource>();
			}
			if (this._playOnStart)
			{
				this.PlayAudio();
			}
		}

		public void PlayAudio()
		{
			float num = Random.Range(0f, 100f);
			if (this._chanceToPlay < 100f && num > this._chanceToPlay)
			{
				return;
			}
			if (this._volumeRandomization.UseRandomRange)
			{
				this._audioSource.volume = Random.Range(this._volumeRandomization.Min, this._volumeRandomization.Max);
			}
			else
			{
				this._audioSource.volume = this._volume;
			}
			if (this._pitchRandomization.UseRandomRange)
			{
				this._audioSource.pitch = Random.Range(this._pitchRandomization.Min, this._pitchRandomization.Max);
			}
			else
			{
				this._audioSource.pitch = this._pitch;
			}
			this._audioSource.spatialize = this._spatialize;
			this._audioSource.loop = this._loop;
			this._audioSource.clip = this.RandomClipWithoutRepeat();
			this._audioSource.Play();
		}

		private AudioClip RandomClipWithoutRepeat()
		{
			if (this._audioClips.Length == 1)
			{
				return this._audioClips[0];
			}
			int num = Random.Range(1, this._audioClips.Length);
			int num2 = (this._previousAudioClipIndex + num) % this._audioClips.Length;
			this._previousAudioClipIndex = num2;
			return this._audioClips[num2];
		}

		public void InjectAllAudioTrigger(AudioSource audioSource, AudioClip[] audioClips)
		{
			this.InjectAudioSource(audioSource);
			this.InjectAudioClips(audioClips);
		}

		public void InjectAudioSource(AudioSource audioSource)
		{
			this._audioSource = audioSource;
		}

		public void InjectAudioClips(AudioClip[] audioClips)
		{
			this._audioClips = audioClips;
		}

		public void InjectOptionalPlayOnStart(bool playOnStart)
		{
			this._playOnStart = playOnStart;
		}

		[SerializeField]
		private AudioSource _audioSource;

		[Tooltip("Audio clip arrays with a value greater than 1 will have randomized playback.")]
		[SerializeField]
		private AudioClip[] _audioClips;

		[Tooltip("Volume set here will override the volume set on the attached sound source component.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float _volume = 0.7f;

		[Tooltip("Check the 'Use Random Range' bool and adjust the min and max slider values for randomized volume level playback.")]
		[SerializeField]
		private MinMaxPair _volumeRandomization;

		[Tooltip("Pitch set here will override the volume set on the attached sound source component.")]
		[SerializeField]
		[Range(-3f, 3f)]
		[Space(10f)]
		private float _pitch = 1f;

		[Tooltip("Check the 'Use Random Range' bool and adjust the min and max slider values for randomized volume level playback.")]
		[SerializeField]
		private MinMaxPair _pitchRandomization;

		[Tooltip("True by default. Set to false for sounds to bypass the spatializer plugin. Will override settings on attached audio source.")]
		[SerializeField]
		[Space(10f)]
		private bool _spatialize = true;

		[Tooltip("False by default. Set to true to enable looping on this sound. Will override settings on attached audio source.")]
		[SerializeField]
		private bool _loop;

		[Tooltip("100% by default. Sets likelihood sample will actually play when called.")]
		[SerializeField]
		private float _chanceToPlay = 100f;

		[Tooltip("If enabled, audio will play automatically when this gameobject is enabled.")]
		[SerializeField]
		[Optional]
		private bool _playOnStart;

		private int _previousAudioClipIndex = -1;
	}
}
