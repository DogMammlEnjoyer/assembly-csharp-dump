using System;
using System.Collections.Generic;
using Liv.Lck.Collections;
using Liv.Lck.NativeMicrophone;
using Liv.Lck.Settings;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	internal class LckAudioMixer : ILckAudioMixer, IDisposable, ILckLateUpdate
	{
		[Preserve]
		public LckAudioMixer(ILckEventBus eventBus, ILckOutputConfigurer outputConfigurer)
		{
			this._sampleRate = (int)outputConfigurer.GetAudioSampleRate().Result;
			this.VerifyAudioCaptureComponent();
			this._nativeMicrophoneCapture = new LckNativeMicrophone(this._sampleRate);
			this._lckAudioLimiterHard = new LckAudioHardLimiter(0.65f, 6f, 1f, 0.01f, 0.1f);
			this._lckAudioLimiterSoft = new LckAudioSoftLimiter(0.6f, 0.8f, 12f, 1f, 0.01f, 0.1f);
			eventBus.AddListener<LckEvents.EncoderStartedEvent>(new Action<LckEvents.EncoderStartedEvent>(this.OnEncoderStarted));
			LckUpdateManager.RegisterSingleLateUpdate(this);
		}

		public AudioBuffer GetMixedAudio(float recordingTime)
		{
			return this.MixAudioArrays(recordingTime);
		}

		public void ReadAvailableAudioData()
		{
			if (!this.VerifyAudioCaptureComponent())
			{
				return;
			}
			if (this._nativeMicrophoneCapture.IsCapturing())
			{
				this._nativeMicrophoneCapture.GetAudioData(new ILckAudioSource.AudioDataCallbackDelegate(this.MicrophoneAudioDataCallback));
			}
			this._gameAudioSource.GetAudioData(new ILckAudioSource.AudioDataCallbackDelegate(this.GameAudioDataCallback));
		}

		public void EnableCapture()
		{
			this.VerifyAudioCaptureComponent();
			if (this._gameAudioSource == null)
			{
				LckLog.LogError("Unable to enable audio capture - game audio source is null");
				return;
			}
			this._gameAudioSource.EnableCapture();
			this._micCaptureStartRecordingTime = null;
			this._microphoneQueue.Clear();
			this._gameAudioQueue.Clear();
			this._totalGameSamples = 0;
			this._totalMicSamples = 0;
			this.PrepareGameAudioSyncOffset();
		}

		public void DisableCapture()
		{
			ILckAudioSource gameAudioSource = this._gameAudioSource;
			if (gameAudioSource != null)
			{
				gameAudioSource.DisableCapture();
			}
			this._micCaptureStartRecordingTime = null;
			this._microphoneQueue.Clear();
			this._gameAudioQueue.Clear();
		}

		private AudioBuffer MixAudioArrays(float recordingTime)
		{
			if (this._gameAudioSource == null)
			{
				LckLog.LogError("LCK No game audio source found");
				return null;
			}
			bool flag = this._nativeMicrophoneCapture.IsCapturing();
			if (this._micCaptureStartRecordingTime == null && flag)
			{
				this._micCaptureStartRecordingTime = new float?(recordingTime);
			}
			this.EnqueueGameBufferSamples();
			if (flag)
			{
				this.EnqueueMicBufferSamples();
			}
			else
			{
				this._microphoneQueue.Clear();
			}
			this.EnsureAudioSourceSamplesWithinTolerance("_gameAudioSource", recordingTime, this._gameAudioQueue, ref this._totalGameSamples);
			if (flag && this._micCaptureStartRecordingTime != null)
			{
				float captureTime = recordingTime - this._micCaptureStartRecordingTime.Value;
				this.EnsureAudioSourceSamplesWithinTolerance("_nativeMicrophoneCapture", captureTime, this._microphoneQueue, ref this._totalMicSamples);
			}
			int blocks = this.DetermineAvailableBlockCount(flag);
			return this.MixBlocksIntoMixedAudioBuffer(flag, blocks);
		}

		private void EnqueueGameBufferSamples()
		{
			if (this._remainingGameAudioValuesToAdjust > 0)
			{
				int num = Math.Max(this._remainingGameAudioValuesToAdjust - this._gameAudioQueue.Count, 0);
				for (int i = 0; i < num; i++)
				{
					this._gameAudioQueue.Enqueue(0f);
				}
				this._remainingGameAudioValuesToAdjust -= num;
			}
			this._totalGameSamples += this._gameAudioBuffer.Count / 2;
			for (int j = 0; j < this._gameAudioBuffer.Count; j++)
			{
				this._gameAudioQueue.Enqueue(this._gameAudioBuffer[j] * this._gameAudioGain * (this._isGameAudioMuted ? 0f : 1f));
			}
			if (this._remainingGameAudioValuesToAdjust < 0)
			{
				int num2 = Mathf.Min(Mathf.Abs(this._remainingGameAudioValuesToAdjust), this._gameAudioQueue.Count);
				for (int k = 0; k < num2; k++)
				{
					this._gameAudioQueue.Dequeue();
				}
				this._remainingGameAudioValuesToAdjust += num2;
			}
		}

		private void EnqueueMicBufferSamples()
		{
			if (this._micAudioBuffer == null)
			{
				return;
			}
			this._totalMicSamples += this._micAudioBuffer.Count / 2;
			for (int i = 0; i < this._micAudioBuffer.Count; i++)
			{
				this._microphoneQueue.Enqueue(this._micAudioBuffer[i] * this._microphoneGain * (this._isMicrophoneMuted ? 0f : 1f));
			}
		}

		private int DetermineAvailableBlockCount(bool shouldIncludeMicAudio)
		{
			int num = this.CountAvailableGameBlocks();
			if (!shouldIncludeMicAudio)
			{
				return num;
			}
			int b = this.CountAvailableMicrophoneBlocks();
			return Mathf.Min(num, b);
		}

		private int CountAvailableGameBlocks()
		{
			int num = this._gameAudioQueue.Count;
			if (this._gameAudioValueCountOffset > 0)
			{
				num -= this._gameAudioValueCountOffset;
			}
			return Math.Max(0, num / 1024);
		}

		private int CountAvailableMicrophoneBlocks()
		{
			int num = this._microphoneQueue.Count;
			if (this._gameAudioValueCountOffset < 0)
			{
				num += this._gameAudioValueCountOffset;
			}
			return Math.Max(0, num / 1024);
		}

		private AudioBuffer MixBlocksIntoMixedAudioBuffer(bool shouldIncludeMicAudio, int blocks)
		{
			int num = blocks * 1024;
			this._mixedAudioBuffer.Clear();
			for (int i = 0; i < num; i++)
			{
				float num2 = this._gameAudioQueue.Dequeue();
				if (shouldIncludeMicAudio)
				{
					num2 += this._microphoneQueue.Dequeue();
				}
				float value = this.ApplyLimiter(num2);
				if (!this._mixedAudioBuffer.TryAdd(value))
				{
					LckLog.LogWarning("LCK Mixed audio buffer overflow");
					break;
				}
			}
			return this._mixedAudioBuffer;
		}

		private float ApplyLimiter(float mixedAudioRaw)
		{
			float result = mixedAudioRaw;
			LckSettings.LimiterType audioLimiter = LckSettings.Instance.AudioLimiter;
			if (audioLimiter != LckSettings.LimiterType.SoftClip)
			{
				if (audioLimiter == LckSettings.LimiterType.None)
				{
					result = mixedAudioRaw;
				}
			}
			else
			{
				result = LckAudioLimiterUtils.ApplySoftClip(mixedAudioRaw);
			}
			return result;
		}

		private void MicrophoneAudioDataCallback(AudioBuffer audioBuffer)
		{
			this._micAudioBuffer.Clear();
			if (audioBuffer.Count > 0)
			{
				if (!this._micAudioBuffer.TryCopyFrom(audioBuffer))
				{
					LckLog.LogError("LCK Mic audio data copy failed");
					return;
				}
				this._lastMicrophoneLevel = (this._lastMicrophoneLevel + LckAudioMixer.CalculateRootMeanSquare(this._micAudioBuffer)) / 2f;
			}
		}

		private void GameAudioDataCallback(AudioBuffer audioBuffer)
		{
			this._gameAudioBuffer.Clear();
			if (audioBuffer.Count > 0)
			{
				if (!this._gameAudioBuffer.TryCopyFrom(audioBuffer))
				{
					LckLog.LogError("LCK Game audio data copy failed");
					return;
				}
				this._lastGameAudioLevel = (this._lastGameAudioLevel + LckAudioMixer.CalculateRootMeanSquare(audioBuffer)) / 2f;
				if (float.IsNaN(this._lastGameAudioLevel))
				{
					this._lastGameAudioLevel = 0f;
				}
			}
		}

		private bool VerifyAudioCaptureComponent()
		{
			if (this._audioCaptureMarker == null)
			{
				AudioListener[] array = Object.FindObjectsOfType<AudioListener>(false);
				List<AudioListener> list = new List<AudioListener>();
				foreach (AudioListener audioListener in array)
				{
					if (audioListener.enabled)
					{
						list.Add(audioListener);
					}
				}
				if (list.Count == 0)
				{
					LckLog.Log("LCK Found no audio listener in the scene, looking for AudioCaptureMarker");
					LckAudioMarker[] array2 = Object.FindObjectsOfType<LckAudioMarker>(false);
					if (array2.Length != 0)
					{
						this._audioCaptureMarker = array2[0];
					}
					if (array2.Length > 1)
					{
						LckLog.LogError("LCK found more than one AudioCaptureMarker in the scene. This is not valid");
					}
				}
				else
				{
					if (list.Count > 0)
					{
						this._audioCaptureMarker = list[0];
					}
					if (list.Count > 1)
					{
						LckLog.LogError("LCK found more than one active AudioListener in the scene. This is not valid");
					}
				}
			}
			if (this._gameAudioSource == null)
			{
				this._gameAudioSource = this._audioCaptureMarker.gameObject.GetComponent<ILckAudioSource>();
				if (this._gameAudioSource == null)
				{
					this._gameAudioSource = this._audioCaptureMarker.gameObject.AddComponent<LckAudioCapture>();
				}
			}
			return true;
		}

		private bool CheckMicAudioPermissions()
		{
			return true;
		}

		public LckResult SetMicrophoneCaptureActive(bool active)
		{
			this._lastMicrophoneLevel = 0f;
			if (!this.CheckMicAudioPermissions())
			{
				return LckResult.NewError(LckError.MicrophonePermissionDenied, "The app has not been granted microphone permissions.");
			}
			if (active)
			{
				this._nativeMicrophoneCapture.EnableCapture();
			}
			else
			{
				this._nativeMicrophoneCapture.DisableCapture();
				this._micCaptureStartRecordingTime = null;
			}
			this._totalMicSamples = 0;
			return LckResult.NewSuccess();
		}

		public LckResult<bool> GetMicrophoneCaptureActive()
		{
			return LckResult<bool>.NewSuccess(this._nativeMicrophoneCapture.IsCapturing());
		}

		public LckResult SetGameAudioMute(bool isMute)
		{
			this._isGameAudioMuted = isMute;
			return LckResult.NewSuccess();
		}

		public LckResult<bool> IsGameAudioMute()
		{
			return LckResult<bool>.NewSuccess(this._isGameAudioMuted);
		}

		public void SetMicrophoneGain(float gain)
		{
			this._microphoneGain = gain;
		}

		public void SetGameAudioGain(float gain)
		{
			this._gameAudioGain = gain;
		}

		public float GetMicrophoneOutputLevel()
		{
			return this._lastMicrophoneLevel;
		}

		public float GetGameOutputLevel()
		{
			return this._lastGameAudioLevel;
		}

		private static float CalculateRootMeanSquare(AudioBuffer audioBuffer)
		{
			if (audioBuffer == null || audioBuffer.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < audioBuffer.Count; i++)
			{
				num += audioBuffer[i] * audioBuffer[i];
			}
			return Mathf.Sqrt(num / (float)audioBuffer.Count);
		}

		private static void PadWithSilence(Queue<float> audioQueue, int samplesToAdd, ref int runningSampleCount)
		{
			for (int i = 0; i < samplesToAdd; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					audioQueue.Enqueue(0f);
				}
				runningSampleCount++;
			}
		}

		private void EnsureAudioSourceSamplesWithinTolerance(string audioSourceName, float captureTime, Queue<float> audioSourceQueue, ref int audioSourceRunningSampleCount)
		{
			int num = Mathf.FloorToInt(captureTime * (float)this._sampleRate);
			int num2 = audioSourceRunningSampleCount - num;
			int num3 = Math.Abs(num2);
			int num4 = 100 * (this._sampleRate / 1000);
			if (num3 <= num4)
			{
				return;
			}
			if (num2 < 0)
			{
				LckLog.LogWarning(string.Format("{0} is behind expected sample count ({1}) by ", audioSourceName, num) + string.Format("{0} samples - Padding with silence for missing samples", num3));
				LckAudioMixer.PadWithSilence(audioSourceQueue, num3, ref audioSourceRunningSampleCount);
				return;
			}
			LckLog.LogWarning(string.Format("{0} is ahead of expected sample count ({1}) by ", audioSourceName, num) + string.Format("{0} samples - Expecting this to be a result of a lag spike", num3));
		}

		private void PrepareGameAudioSyncOffset()
		{
			float num = LckSettings.Instance.GameAudioSyncTimeOffsetInMS / 1000f;
			this._gameAudioValueCountOffset = Mathf.CeilToInt(num * (float)this._sampleRate) * 2;
			this._remainingGameAudioValuesToAdjust = this._gameAudioValueCountOffset;
		}

		private void OnEncoderStarted(LckEvents.EncoderStartedEvent encoderStartedEvent)
		{
			if (!encoderStartedEvent.Result.Success)
			{
				return;
			}
			this.EnableCapture();
		}

		public void LateUpdate()
		{
			using (LckAudioMixer._lateUpdateProfileMarker.Auto())
			{
				this.ReadAvailableAudioData();
			}
		}

		public void Dispose()
		{
			LckUpdateManager.UnregisterSingleLateUpdate(this);
			LckNativeMicrophone lckNativeMicrophone = this._nativeMicrophoneCapture as LckNativeMicrophone;
			if (lckNativeMicrophone != null)
			{
				lckNativeMicrophone.Dispose();
			}
			this._nativeMicrophoneCapture = null;
		}

		private ILckAudioSource _gameAudioSource;

		private bool _isGameAudioMuted;

		private float _gameAudioGain = 1f;

		private Queue<float> _gameAudioQueue = new Queue<float>();

		private ILckAudioSource _nativeMicrophoneCapture;

		private bool _isMicrophoneMuted;

		private float _microphoneGain = 1f;

		private Queue<float> _microphoneQueue = new Queue<float>();

		private AudioBuffer _micAudioBuffer = new AudioBuffer(96000);

		private float _lastMicrophoneLevel;

		private AudioBuffer _gameAudioBuffer = new AudioBuffer(96000);

		private float _lastGameAudioLevel;

		private AudioBuffer _mixedAudioBuffer = new AudioBuffer(96000);

		private int _remainingGameAudioValuesToAdjust;

		private int _gameAudioValueCountOffset;

		private readonly int _sampleRate;

		private Component _audioCaptureMarker;

		private const int _targetAudioBufferLength = 1024;

		private ILckAudioLimiter _lckAudioLimiterHard;

		private ILckAudioLimiter _lckAudioLimiterSoft;

		private ILckAudioLimiter _lckAudioLimiterCurve;

		private const int TrackTimeDifferenceToleranceMilli = 100;

		private const int NumberOfChannels = 2;

		private float? _micCaptureStartRecordingTime;

		private int _totalGameSamples;

		private int _totalMicSamples;

		private static readonly ProfilerMarker _lateUpdateProfileMarker = new ProfilerMarker("LckAudioMixer.LateUpdate");
	}
}
