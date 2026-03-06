using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	[RequireComponent(typeof(AudioSource))]
	public class AdjustableAudio : MonoBehaviour
	{
		public AudioClip AudioClip
		{
			get
			{
				return this._audioClip;
			}
			set
			{
				this._audioClip = value;
			}
		}

		public float VolumeFactor
		{
			get
			{
				return this._volumeFactor;
			}
			set
			{
				this._volumeFactor = value;
			}
		}

		public AnimationCurve VolumeCurve
		{
			get
			{
				return this._volumeCurve;
			}
			set
			{
				this._volumeCurve = value;
			}
		}

		public AnimationCurve PitchCurve
		{
			get
			{
				return this._pitchCurve;
			}
			set
			{
				this._pitchCurve = value;
			}
		}

		protected virtual void Reset()
		{
			this._audioSource = base.gameObject.GetComponent<AudioSource>();
			this._audioClip = this._audioSource.clip;
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		public void PlayAudio(float volumeT, float pitchT, float pan = 0f)
		{
			if (!this._audioSource.isActiveAndEnabled)
			{
				return;
			}
			this._audioSource.volume = this._volumeCurve.Evaluate(volumeT) * this.VolumeFactor;
			this._audioSource.pitch = this._pitchCurve.Evaluate(pitchT);
			this._audioSource.panStereo = pan;
			this._audioSource.PlayOneShot(this._audioClip);
		}

		public void InjectAllAdjustableAudio(AudioSource audioSource)
		{
			this.InjectAudioSource(audioSource);
		}

		public void InjectAudioSource(AudioSource audioSource)
		{
			this._audioSource = audioSource;
		}

		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private AudioClip _audioClip;

		[SerializeField]
		[Range(0f, 1f)]
		private float _volumeFactor = 1f;

		[SerializeField]
		private AnimationCurve _volumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private AnimationCurve _pitchCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 1.5f);

		protected bool _started;
	}
}
