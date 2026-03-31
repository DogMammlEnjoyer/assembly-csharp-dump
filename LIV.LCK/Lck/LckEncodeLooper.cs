using System;
using System.Collections;
using Liv.Lck.Collections;
using Liv.Lck.Encoding;
using Liv.Lck.Telemetry;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	internal class LckEncodeLooper : ILckEncodeLooper, IDisposable, ILckEarlyUpdate
	{
		[Preserve]
		public LckEncodeLooper(ILckEncoder encoder, ILckOutputConfigurer outputConfigurer, ILckAudioMixer audioMixer, ILckVideoCapturer videoCapturer, ILckEventBus eventBus, ILckTelemetryClient telemetryClient)
		{
			this._encoder = encoder;
			this._outputConfigurer = outputConfigurer;
			this._audioMixer = audioMixer;
			this._videoCapturer = videoCapturer;
			this._telemetryClient = telemetryClient;
			eventBus.AddListener<LckEvents.EncoderStartedEvent>(new Action<LckEvents.EncoderStartedEvent>(this.OnEncoderStarted));
		}

		public void EarlyUpdate()
		{
			ILckEncoder encoder = this._encoder;
			if (encoder == null || !encoder.IsActive())
			{
				this.UnregisterEncodeFrameEarlyUpdate();
				return;
			}
			float num = Time.unscaledDeltaTime;
			AudioBuffer mixedAudio = this._audioMixer.GetMixedAudio(this._videoTime + this._pausedForTime);
			if (num > 1f)
			{
				LckLog.LogWarning("LCK detected lag spike during capture - adjusting capture time accordingly", "EarlyUpdate", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckEncodeLooper.cs", 67);
				uint num2 = this._outputConfigurer.GetAudioSampleRate().Result * this._outputConfigurer.GetNumberOfAudioChannels().Result;
				num = (float)mixedAudio.Count / num2;
			}
			if (this._encoder.IsPaused())
			{
				this._pausedForTime += num;
				return;
			}
			if (!this.IsAudioDataValid(mixedAudio))
			{
				return;
			}
			EncoderSessionData currentSessionData = this._encoder.GetCurrentSessionData();
			if (currentSessionData.EncodedAudioSamplesPerChannel == 0UL && mixedAudio.Count == 0)
			{
				for (int i = 0; i < 1024; i++)
				{
					mixedAudio.TryAdd(0f);
				}
			}
			LckEncodeLooper.EnsureTrackTimeAlignment(ref this._videoTime, this.CalculateAudioTime(), this._prevVideoTime);
			if (!this._encoder.EncodeFrame(this._videoTime, mixedAudio, this._videoCapturer.HasCurrentFrameBeenCaptured()))
			{
				this.HandleEncodeFrameError(string.Format("LCK EncodeFrame returned false. This indicates a critical error. (recordingTime: {0}, audioTimestampSamples: {1})", currentSessionData.CaptureTimeSeconds, currentSessionData.EncodedAudioSamplesPerChannel));
			}
			this._videoTime += num;
		}

		private float CalculateAudioTime()
		{
			EncoderSessionData currentSessionData = this._encoder.GetCurrentSessionData();
			uint result = this._outputConfigurer.GetAudioSampleRate().Result;
			return currentSessionData.EncodedAudioSamplesPerChannel / result;
		}

		private bool IsAudioDataValid(AudioBuffer audioData)
		{
			if (audioData != null)
			{
				return true;
			}
			EncoderSessionData currentSessionData = this._encoder.GetCurrentSessionData();
			this.HandleEncodeFrameError(string.Format("LCK Audio data is null (captureTime: {0}, audioTimestampSamples: {1})", currentSessionData.CaptureTimeSeconds, currentSessionData.EncodedAudioSamplesPerChannel));
			return false;
		}

		private void StartEncodingFrames()
		{
			this._videoTime = (this._prevVideoTime = (this._pausedForTime = 0f));
			LckUpdateManager.RegisterSingleEarlyUpdate(this);
		}

		private void UnregisterEncodeFrameEarlyUpdate()
		{
			LckUpdateManager.UnregisterSingleEarlyUpdate(this);
		}

		private void HandleEncodeFrameError(string errorMessage)
		{
			LckLog.LogError(errorMessage, "HandleEncodeFrameError", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckEncodeLooper.cs", 135);
			this._encoder.StopEncodingAsync();
		}

		private IEnumerator StartEncodingAfterWarmupFrames(int warmupFrameCount)
		{
			this._videoCapturer.ForceCaptureAllFrames = true;
			while (warmupFrameCount > 0)
			{
				yield return null;
				int num = warmupFrameCount;
				warmupFrameCount = num - 1;
			}
			this._videoCapturer.ForceCaptureAllFrames = false;
			this.StartEncodingFrames();
			yield break;
		}

		private void OnEncoderStarted(LckEvents.EncoderStartedEvent encoderStartedEvent)
		{
			if (!encoderStartedEvent.Result.Success)
			{
				return;
			}
			LckMonoBehaviourMediator.StartCoroutine("LckEncodeLooper:StartEncodingAfterWarmupFrames", this.StartEncodingAfterWarmupFrames(3));
		}

		private static void EnsureTrackTimeAlignment(ref float videoTime, float audioTime, float prevVideoTime)
		{
			float num = videoTime - audioTime;
			float num2 = Math.Abs(num);
			if (num2 <= 0.3f)
			{
				return;
			}
			LckLog.LogError(string.Format("Video track is {0}ms ", Mathf.FloorToInt(1000f * num2)) + ((num > 0f) ? "ahead of" : "behind") + " audio track - adjusting video time to re-sync", "EnsureTrackTimeAlignment", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckEncodeLooper.cs", 176);
			videoTime = Math.Max(audioTime, prevVideoTime + 0.001f);
		}

		public void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			LckMonoBehaviourMediator.StopCoroutineByName("LckEncodeLooper:StartEncodingAfterWarmupFrames");
			this.UnregisterEncodeFrameEarlyUpdate();
			this._disposed = true;
		}

		private readonly ILckEncoder _encoder;

		private readonly ILckOutputConfigurer _outputConfigurer;

		private readonly ILckAudioMixer _audioMixer;

		private readonly ILckVideoCapturer _videoCapturer;

		private readonly ILckTelemetryClient _telemetryClient;

		private float _pausedForTime;

		private float _videoTime;

		private float _prevVideoTime;

		private bool _disposed;

		private const float MinVideoTimeIncrement = 0.001f;

		private const float TrackTimestampDifferenceTolerance = 0.3f;

		private const int EncodingWarmupFrames = 3;

		private const string EncodingWarmupCoroutineName = "LckEncodeLooper:StartEncodingAfterWarmupFrames";
	}
}
