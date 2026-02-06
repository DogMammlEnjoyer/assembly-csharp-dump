using System;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	public class VelocityBasedAudioTriggerCosmetic : MonoBehaviour
	{
		private void Awake()
		{
			if (this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
			}
			if (this.soundBank != null && this.audioSource != null)
			{
				this.soundBank.audioSource = this.audioSource;
			}
		}

		private void Update()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (averageVelocity.magnitude < this.minVelocityThreshold)
			{
				return;
			}
			float t = Mathf.InverseLerp(this.minVelocityThreshold, this.maxVelocity, averageVelocity.magnitude);
			float num = Mathf.Lerp(this.minOutputVolume, this.maxOutputVolume, t);
			this.audioSource.volume = num;
			if (this.audioSource != null && !this.audioSource.isPlaying && this.audioClip != null)
			{
				this.audioSource.clip = this.audioClip;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (this.soundBank != null && this.soundBank.soundBank != null && !this.soundBank.isPlaying)
			{
				this.soundBank.Play(new float?(num), null);
			}
		}

		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private AudioClip audioClip;

		[SerializeField]
		private SoundBankPlayer soundBank;

		[Tooltip(" Minimum velocity to trigger audio")]
		[SerializeField]
		private float minVelocityThreshold = 0.5f;

		[SerializeField]
		private float maxVelocity = 2f;

		[SerializeField]
		private float minOutputVolume;

		[SerializeField]
		private float maxOutputVolume = 1f;
	}
}
