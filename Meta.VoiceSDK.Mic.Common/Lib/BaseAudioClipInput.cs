using System;
using System.Collections;
using Meta.Voice;
using Meta.WitAi.Data;
using Meta.WitAi.Interfaces;
using UnityEngine;

namespace Meta.WitAi.Lib
{
	public abstract class BaseAudioClipInput : MonoBehaviour, IAudioInputSource, IAudioLevelRangeProvider
	{
		public abstract AudioClip Clip { get; }

		public abstract int ClipPosition { get; }

		public abstract bool CanActivateAudio { get; }

		public virtual bool ActivateOnEnable
		{
			get
			{
				return false;
			}
		}

		public virtual int AudioChannels
		{
			get
			{
				return 1;
			}
		}

		public virtual int AudioSampleRate
		{
			get
			{
				return 16000;
			}
		}

		public virtual int AudioSampleLength { get; private set; }

		public virtual float MinAudioLevel
		{
			get
			{
				return 0.5f;
			}
		}

		public virtual float MaxAudioLevel
		{
			get
			{
				return 1f;
			}
		}

		public AudioEncoding AudioEncoding
		{
			get
			{
				if (this._audioEncoding == null)
				{
					this._audioEncoding = new AudioEncoding();
				}
				this._audioEncoding.numChannels = this.AudioChannels;
				this._audioEncoding.samplerate = this.AudioSampleRate;
				this._audioEncoding.encoding = "signed-integer";
				return this._audioEncoding;
			}
		}

		public VoiceAudioInputState ActivationState { get; private set; }

		public event Action<VoiceAudioInputState> OnActivationStateChange;

		public virtual bool IsRecording { get; private set; }

		public event Action OnStartRecording;

		public event Action OnStartRecordingFailed;

		public event Action OnStopRecording;

		public event Action<int, float[], float> OnSampleReady;

		protected void SetActivationState(VoiceAudioInputState newActivationState)
		{
			this.ActivationState = newActivationState;
			Action<VoiceAudioInputState> onActivationStateChange = this.OnActivationStateChange;
			if (onActivationStateChange == null)
			{
				return;
			}
			onActivationStateChange(this.ActivationState);
		}

		public virtual bool IsMuted { get; private set; }

		public event Action OnMicMuted;

		public event Action OnMicUnmuted;

		protected virtual void SetMuted(bool muted)
		{
			if (this.IsMuted != muted)
			{
				this.IsMuted = muted;
				if (this.IsMuted)
				{
					Action onMicMuted = this.OnMicMuted;
					if (onMicMuted == null)
					{
						return;
					}
					onMicMuted();
					return;
				}
				else
				{
					Action onMicUnmuted = this.OnMicUnmuted;
					if (onMicUnmuted == null)
					{
						return;
					}
					onMicUnmuted();
				}
			}
		}

		protected virtual void OnEnable()
		{
			if (this.ActivateOnEnable && this.ActivationState != VoiceAudioInputState.Activating && this.ActivationState != VoiceAudioInputState.On)
			{
				this.ActivateAudio();
			}
		}

		private void ActivateAudio()
		{
			if (this.ActivationState == VoiceAudioInputState.On || this.ActivationState == VoiceAudioInputState.Activating)
			{
				VLog.W(base.GetType().Name, string.Format("Cannot activate when audio is already {0}", this.ActivationState), null);
				return;
			}
			if (!base.gameObject.activeInHierarchy)
			{
				VLog.W(base.GetType().Name, "Audio activation is disabled while GameObject is inactive", null);
				return;
			}
			if (!this.CanActivateAudio)
			{
				VLog.W(base.GetType().Name, "Audio activation is currently restricted", null);
				return;
			}
			if (this._activateCoroutine != null)
			{
				base.StopCoroutine(this._activateCoroutine);
				this._activateCoroutine = null;
			}
			this._activateCoroutine = base.StartCoroutine(this.PerformActivation());
		}

		private IEnumerator PerformActivation()
		{
			this.SetActivationState(VoiceAudioInputState.Activating);
			yield return this.HandleActivation();
			if (this.ActivationState == VoiceAudioInputState.Activating)
			{
				this.SetActivationState(VoiceAudioInputState.On);
			}
			this._activateCoroutine = null;
			yield break;
		}

		protected abstract IEnumerator HandleActivation();

		protected virtual void OnDisable()
		{
			if (this.IsRecording)
			{
				this.StopRecording();
			}
			if (this.ActivateOnEnable && this.ActivationState != VoiceAudioInputState.Deactivating && this.ActivationState != VoiceAudioInputState.Off)
			{
				this.DeactivateAudio();
			}
		}

		private void DeactivateAudio()
		{
			if (this.ActivationState == VoiceAudioInputState.Off || this.ActivationState == VoiceAudioInputState.Deactivating)
			{
				VLog.W(base.GetType().Name, string.Format("Cannot deactivate when audio is already {0}", this.ActivationState), null);
				return;
			}
			if (this._activateCoroutine != null)
			{
				base.StopCoroutine(this._activateCoroutine);
				this._activateCoroutine = null;
			}
			this.SetActivationState(VoiceAudioInputState.Deactivating);
			this.HandleDeactivation();
			if (this.ActivationState == VoiceAudioInputState.Deactivating)
			{
				this.SetActivationState(VoiceAudioInputState.Off);
			}
		}

		protected abstract void HandleDeactivation();

		public virtual void StartRecording(int sampleDurationMS)
		{
			if (!this.IsRecording)
			{
				this.IsRecording = true;
				this.AudioSampleLength = sampleDurationMS;
				if (this._recordCoroutine != null)
				{
					base.StopCoroutine(this._recordCoroutine);
					this._recordCoroutine = null;
				}
				this._recordCoroutine = base.StartCoroutine(this.ReadRawAudio());
				return;
			}
			VLog.W(base.GetType().Name, "Cannot start recording when already recording", null);
			Action onStartRecordingFailed = this.OnStartRecordingFailed;
			if (onStartRecordingFailed == null)
			{
				return;
			}
			onStartRecordingFailed();
		}

		private IEnumerator ReadRawAudio()
		{
			if (this.ActivationState != VoiceAudioInputState.On)
			{
				if (this.ActivationState != VoiceAudioInputState.Activating)
				{
					this.ActivateAudio();
				}
				while (this.ActivationState == VoiceAudioInputState.Activating)
				{
					yield return null;
				}
				if (this.ActivationState != VoiceAudioInputState.On)
				{
					this.IsRecording = false;
					Action onStartRecordingFailed = this.OnStartRecordingFailed;
					if (onStartRecordingFailed != null)
					{
						onStartRecordingFailed();
					}
					yield break;
				}
			}
			AudioClip micClip = this.Clip;
			if (micClip == null)
			{
				VLog.W(base.GetType().Name, "No AudioClip found following activation", null);
				this.IsRecording = false;
				Action onStartRecordingFailed2 = this.OnStartRecordingFailed;
				if (onStartRecordingFailed2 != null)
				{
					onStartRecordingFailed2();
				}
				yield break;
			}
			Action onStartRecording = this.OnStartRecording;
			if (onStartRecording != null)
			{
				onStartRecording();
			}
			float num = (float)this.AudioSampleLength / 1000f;
			float audioChannels = (float)this.AudioChannels;
			int audioSampleRate = this.AudioSampleRate;
			int num2 = Mathf.CeilToInt(audioChannels * (float)audioSampleRate * num);
			float[] samples = new float[num2];
			int prevMicPosition = this.ClipPosition;
			int readAbsPosition = prevMicPosition;
			int loops = 0;
			while (micClip != null && this.IsRecording)
			{
				yield return null;
				bool flag = true;
				while (micClip != null && flag)
				{
					int clipPosition = this.ClipPosition;
					if (clipPosition < prevMicPosition)
					{
						int num3 = loops;
						loops = num3 + 1;
					}
					prevMicPosition = clipPosition;
					int num4 = loops * micClip.samples + clipPosition;
					int num5 = readAbsPosition + samples.Length;
					flag = (num5 < num4);
					if (flag && micClip.GetData(samples, readAbsPosition % micClip.samples))
					{
						Action<int, float[], float> onSampleReady = this.OnSampleReady;
						if (onSampleReady != null)
						{
							onSampleReady(0, samples, 0f);
						}
						readAbsPosition = num5;
					}
				}
			}
			if (this.IsRecording)
			{
				this.StopRecording();
			}
			yield break;
		}

		public virtual void StopRecording()
		{
			if (!this.IsRecording)
			{
				VLog.E(base.GetType().Name, "Cannot stop recording when not recording", null);
				return;
			}
			if (!this.ActivateOnEnable || !base.gameObject.activeInHierarchy)
			{
				this.DeactivateAudio();
			}
			this.IsRecording = false;
			Action onStopRecording = this.OnStopRecording;
			if (onStopRecording == null)
			{
				return;
			}
			onStopRecording();
		}

		private AudioEncoding _audioEncoding;

		private Coroutine _activateCoroutine;

		private Coroutine _recordCoroutine;
	}
}
