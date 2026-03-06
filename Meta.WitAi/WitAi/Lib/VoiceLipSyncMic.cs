using System;
using Meta.WitAi.Data;
using Meta.WitAi.Events;
using UnityEngine;

namespace Meta.WitAi.Lib
{
	public class VoiceLipSyncMic : MonoBehaviour
	{
		private void Awake()
		{
			if (!this.AudioSource)
			{
				this.AudioSource = base.GetComponent<AudioSource>();
				if (!this.AudioSource)
				{
					this.AudioSource = base.gameObject.AddComponent<AudioSource>();
				}
			}
			this.AudioSource.loop = true;
			this.AudioSource.playOnAwake = false;
			if (this.AudioSource.isPlaying)
			{
				this.AudioSource.Stop();
			}
			AudioBuffer instance = AudioBuffer.Instance;
			Mic mic = ((instance != null) ? instance.MicInput : null) as Mic;
			if (mic != null)
			{
				mic.SetAudioSampleRate(this.AudioSampleRate);
				return;
			}
			Debug.LogError("VoiceMicLipSync only works with Mic script.");
		}

		private void OnEnable()
		{
			AudioBuffer instance = AudioBuffer.Instance;
			if (instance == null)
			{
				return;
			}
			AudioBuffer instance2 = AudioBuffer.Instance;
			Mic mic = ((instance2 != null) ? instance2.MicInput : null) as Mic;
			if (mic != null)
			{
				this.AudioSource.clip = mic.Clip;
			}
			AudioBufferEvents events = instance.Events;
			events.OnSampleReady = (AudioBufferEvents.OnSampleReadyEvent)Delegate.Combine(events.OnSampleReady, new AudioBufferEvents.OnSampleReadyEvent(this.OnMicSampleReady));
			instance.StartRecording(this);
		}

		private void OnMicSampleReady(RingBuffer<byte>.Marker marker, float levelMax)
		{
			if (!this.AudioSource.isPlaying && this.AudioSource.clip != null)
			{
				this.AudioSource.Play();
			}
		}

		private void OnDisable()
		{
			if (this.AudioSource.isPlaying)
			{
				this.AudioSource.Stop();
			}
			this.AudioSource.clip = null;
			AudioBuffer instance = AudioBuffer.Instance;
			if (instance == null)
			{
				return;
			}
			instance.StopRecording(this);
			AudioBufferEvents events = instance.Events;
			events.OnSampleReady = (AudioBufferEvents.OnSampleReadyEvent)Delegate.Remove(events.OnSampleReady, new AudioBufferEvents.OnSampleReadyEvent(this.OnMicSampleReady));
		}

		[Tooltip("Audio desired sample size for lipsync. The mic frequency will be adjusted to match this.")]
		public int AudioSampleRate = 48000;

		[Tooltip("Manual specification of Audio Source. Default will use any attached to the same object.")]
		public AudioSource AudioSource;
	}
}
