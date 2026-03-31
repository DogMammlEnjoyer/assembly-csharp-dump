using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck.Core;
using Liv.Lck.Encoding;
using Liv.Lck.Settings;
using Liv.Lck.Telemetry;
using Liv.Lck.Utilities;
using Liv.NGFX;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck.Recorder
{
	internal class LckRecorder : ILckRecorder, ILckCaptureStateProvider, IDisposable
	{
		public LckCaptureState CurrentCaptureState { get; private set; }

		[Preserve]
		public LckRecorder(ILckNativeRecordingService nativeRecordingService, ILckEncoder encoder, ILckOutputConfigurer outputConfigurer, ILckStorageWatcher storageWatcher, ILckEventBus eventBus, ILckTelemetryClient telemetryClient, ILckTelemetryContextProvider telemetryContextProvider)
		{
			this._nativeRecordingService = nativeRecordingService;
			this._encoder = encoder;
			this._outputConfigurer = outputConfigurer;
			this._storageWatcher = storageWatcher;
			this._eventBus = eventBus;
			this._telemetryClient = telemetryClient;
			this._telemetryContextProvider = telemetryContextProvider;
			this._eventBus.AddListener<LckEvents.LowStorageSpaceDetectedEvent>(new Action<LckEvents.LowStorageSpaceDetectedEvent>(this.OnLowStorageSpaceDetected));
			this._eventBus.AddListener<LckEvents.EncoderStoppedEvent>(new Action<LckEvents.EncoderStoppedEvent>(this.OnEncoderStopped));
			this._eventBus.AddListener<LckEvents.CaptureErrorEvent>(new Action<LckEvents.CaptureErrorEvent>(this.OnCaptureError));
		}

		public LckResult<bool> IsRecording()
		{
			return LckResult<bool>.NewSuccess(this.CurrentCaptureState == LckCaptureState.InProgress);
		}

		public LckResult<bool> IsPaused()
		{
			return LckResult<bool>.NewSuccess(this.CurrentCaptureState == LckCaptureState.Paused);
		}

		public void SetLogLevel(LogLevel logLevel)
		{
			this._nativeRecordingService.SetNativeMuxerLogLevel(logLevel);
		}

		public LckResult StartRecording()
		{
			if (this.CurrentCaptureState != LckCaptureState.Idle)
			{
				return LckResult.NewError(LckError.CaptureAlreadyStarted, "Recording already started.");
			}
			if (!this._storageWatcher.HasEnoughFreeStorage())
			{
				return LckResult.NewError(LckError.NotEnoughStorageSpace, "Not enough storage space.");
			}
			this.StartRecordingProcess();
			return LckResult.NewSuccess();
		}

		public LckResult StopRecording(LckService.StopReason stopReason)
		{
			LckLog.Log(string.Format("LCK {0} triggered with stop reason: {1}", "StopRecording", stopReason), "StopRecording", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckRecorder.cs", 99);
			if (this.CurrentCaptureState != LckCaptureState.InProgress)
			{
				return LckResult.NewError(LckError.NotCurrentlyRecording, "No recording currently in progress to stop.");
			}
			LckLog.Log(string.Format("LCK StopRecording triggered with stopreason: {0}", stopReason), "StopRecording", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckRecorder.cs", 105);
			this._stopReason = stopReason;
			this.StopRecordingProcess();
			return LckResult.NewSuccess();
		}

		public LckResult PauseRecording()
		{
			LckResult result;
			if (this.CurrentCaptureState != LckCaptureState.InProgress)
			{
				result = LckResult.NewError(LckError.NotCurrentlyRecording, "Cannot pause because recording is not in progress.");
			}
			else
			{
				this._accumulatedRecordingDuration += Time.time - this._lastActiveSegmentStartTime;
				result = LckResult.NewSuccess();
				this.CurrentCaptureState = LckCaptureState.Paused;
				LckLog.Log("LCK Recording paused.", "PauseRecording", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckRecorder.cs", 126);
			}
			this.TriggerRecordingPausedEvent(result);
			return result;
		}

		public LckResult ResumeRecording()
		{
			LckResult result;
			if (this.CurrentCaptureState != LckCaptureState.Paused)
			{
				result = LckResult.NewError(LckError.NotPaused, "Cannot resume because recording is not paused.");
			}
			else
			{
				this._lastActiveSegmentStartTime = Time.time;
				result = LckResult.NewSuccess();
				this.CurrentCaptureState = LckCaptureState.InProgress;
				LckLog.Log("LCK Recording resumed.", "ResumeRecording", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckRecorder.cs", 146);
			}
			this.TriggerRecordingResumedEvent(result);
			return result;
		}

		public LckResult<TimeSpan> GetRecordingDuration()
		{
			if (this.CurrentCaptureState == LckCaptureState.Idle)
			{
				return LckResult<TimeSpan>.NewError(LckError.NotCurrentlyRecording, "Recording has not been started.");
			}
			return LckResult<TimeSpan>.NewSuccess(TimeSpan.FromSeconds((double)this.ActualRecordingDurationSeconds));
		}

		private Task<LckResult> StartNativeMuxerAsync()
		{
			LckRecorder.<StartNativeMuxerAsync>d__30 <StartNativeMuxerAsync>d__;
			<StartNativeMuxerAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LckResult>.Create();
			<StartNativeMuxerAsync>d__.<>4__this = this;
			<StartNativeMuxerAsync>d__.<>1__state = -1;
			<StartNativeMuxerAsync>d__.<>t__builder.Start<LckRecorder.<StartNativeMuxerAsync>d__30>(ref <StartNativeMuxerAsync>d__);
			return <StartNativeMuxerAsync>d__.<>t__builder.Task;
		}

		private Task StartRecordingAsync()
		{
			LckRecorder.<StartRecordingAsync>d__31 <StartRecordingAsync>d__;
			<StartRecordingAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartRecordingAsync>d__.<>4__this = this;
			<StartRecordingAsync>d__.<>1__state = -1;
			<StartRecordingAsync>d__.<>t__builder.Start<LckRecorder.<StartRecordingAsync>d__31>(ref <StartRecordingAsync>d__);
			return <StartRecordingAsync>d__.<>t__builder.Task;
		}

		private float ActualRecordingDurationSeconds
		{
			get
			{
				if (this.CurrentCaptureState == LckCaptureState.InProgress)
				{
					return this._accumulatedRecordingDuration + (Time.time - this._lastActiveSegmentStartTime);
				}
				return this._accumulatedRecordingDuration;
			}
		}

		private Task<LckResult> StopNativeMuxerAsync()
		{
			LckRecorder.<StopNativeMuxerAsync>d__34 <StopNativeMuxerAsync>d__;
			<StopNativeMuxerAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LckResult>.Create();
			<StopNativeMuxerAsync>d__.<>4__this = this;
			<StopNativeMuxerAsync>d__.<>1__state = -1;
			<StopNativeMuxerAsync>d__.<>t__builder.Start<LckRecorder.<StopNativeMuxerAsync>d__34>(ref <StopNativeMuxerAsync>d__);
			return <StopNativeMuxerAsync>d__.<>t__builder.Task;
		}

		private Task StopRecordingAsync()
		{
			LckRecorder.<StopRecordingAsync>d__35 <StopRecordingAsync>d__;
			<StopRecordingAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StopRecordingAsync>d__.<>4__this = this;
			<StopRecordingAsync>d__.<>1__state = -1;
			<StopRecordingAsync>d__.<>t__builder.Start<LckRecorder.<StopRecordingAsync>d__35>(ref <StopRecordingAsync>d__);
			return <StopRecordingAsync>d__.<>t__builder.Task;
		}

		private IEnumerator CopyRecordingToGalleryWhenReady()
		{
			while (FileUtility.IsFileLocked(this._lastRecordingFilePath) && File.Exists(this._lastRecordingFilePath))
			{
				yield return this._copyVideoSpinWait;
			}
			using (LckRecorder._copyOutputFileToNativeGalleryMarker.Auto())
			{
				Task task = FileUtility.CopyToGallery(this._lastRecordingFilePath, LckSettings.Instance.RecordingAlbumName, delegate(bool success, string path)
				{
					LckMonoBehaviourMediator.Instance.EnqueueMainThreadAction(delegate
					{
						if (success)
						{
							LckLog.Log("LCK Recording saved to gallery: " + path, "CopyRecordingToGalleryWhenReady", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckRecorder.cs", 333);
							RecordingData result = new RecordingData
							{
								RecordingFilePath = path,
								RecordingDuration = this.ActualRecordingDurationSeconds
							};
							this.TriggerRecordingSavedEvent(LckResult<RecordingData>.NewSuccess(result));
							return;
						}
						this.TriggerRecordingSavedEvent(LckResult<RecordingData>.NewError(LckError.FailedToCopyRecordingToGallery, "Failed to copy recording to Gallery"));
						LckLog.LogError("LCK Failed to save recording to gallery", "CopyRecordingToGalleryWhenReady", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckRecorder.cs", 345);
					});
				});
				yield return new WaitUntil(() => task.IsCompleted);
			}
			ProfilerMarker.AutoScope autoScope = default(ProfilerMarker.AutoScope);
			yield break;
			yield break;
		}

		public void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			if (this.CurrentCaptureState == LckCaptureState.InProgress)
			{
				this.StopRecordingAsync();
			}
			this._storageWatcher.ClearRecordingContext();
			this._eventBus.RemoveListener<LckEvents.LowStorageSpaceDetectedEvent>(new Action<LckEvents.LowStorageSpaceDetectedEvent>(this.OnLowStorageSpaceDetected));
			this._eventBus.RemoveListener<LckEvents.EncoderStoppedEvent>(new Action<LckEvents.EncoderStoppedEvent>(this.OnEncoderStopped));
			this._eventBus.RemoveListener<LckEvents.CaptureErrorEvent>(new Action<LckEvents.CaptureErrorEvent>(this.OnCaptureError));
			this._disposed = true;
		}

		private void StartRecordingProcess()
		{
			this.CurrentCaptureState = LckCaptureState.Starting;
			this.StartRecordingAsync();
		}

		private void StopRecordingProcess()
		{
			LckLog.Log("LCK Stopping Recording", "StopRecordingProcess", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckRecorder.cs", 380);
			if (this.CurrentCaptureState == LckCaptureState.InProgress)
			{
				this._accumulatedRecordingDuration += Time.time - this._lastActiveSegmentStartTime;
			}
			this.CurrentCaptureState = LckCaptureState.Stopping;
			this.SendRecordingStoppedTelemetry();
			this.StopRecordingAsync();
		}

		private void SendRecordingStoppedTelemetry()
		{
			ulong encodedVideoFrames = this._encoder.GetCurrentSessionData().EncodedVideoFrames;
			float actualRecordingDurationSeconds = this.ActualRecordingDurationSeconds;
			float num = (actualRecordingDurationSeconds > 0f && encodedVideoFrames > 0UL) ? (encodedVideoFrames / actualRecordingDurationSeconds) : 0f;
			this._recordingTelemetryContext.Add("recording.duration", actualRecordingDurationSeconds);
			this._recordingTelemetryContext.Add("recording.encodedFrames", encodedVideoFrames);
			this._recordingTelemetryContext.Add("recording.stopReason", this._stopReason.ToString());
			this._recordingTelemetryContext.Add("recording.actualFramerate", num);
			this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.RecordingStopped, this._recordingTelemetryContext));
		}

		private void UpdateRecordingTelemetryContext()
		{
			this._recordingTelemetryContext = new Dictionary<string, object>
			{
				{
					"recording.targetFramerate",
					this._currentRecordingDescriptor.Framerate
				},
				{
					"recording.targetBitrate",
					this._currentRecordingDescriptor.Bitrate
				},
				{
					"recording.targetAudioBitrate",
					this._currentRecordingDescriptor.AudioBitrate
				},
				{
					"recording.targetResolutionX",
					this._currentRecordingDescriptor.CameraResolutionDescriptor.Width
				},
				{
					"recording.targetResolutionY",
					this._currentRecordingDescriptor.CameraResolutionDescriptor.Height
				}
			};
			this._telemetryContextProvider.SetTelemetryContext(LckTelemetryContextType.RecordingContext, this._recordingTelemetryContext);
		}

		private MuxerConfig CreateMuxerConfig()
		{
			string path = FileUtility.GenerateFilename("mp4");
			this._lastRecordingFilePath = Path.Combine(Application.temporaryCachePath, path);
			CameraTrackDescriptor result = this._outputConfigurer.GetActiveCameraTrackDescriptor().Result;
			return new MuxerConfig
			{
				outputPath = this._lastRecordingFilePath,
				videoBitrate = result.Bitrate,
				audioBitrate = result.AudioBitrate,
				width = result.CameraResolutionDescriptor.Width,
				height = result.CameraResolutionDescriptor.Height,
				framerate = result.Framerate,
				samplerate = this._outputConfigurer.GetAudioSampleRate().Result,
				channels = this._outputConfigurer.GetNumberOfAudioChannels().Result,
				numberOfTracks = 2U,
				realtimeOutput = false
			};
		}

		private void OnEncoderStopped(LckEvents.EncoderStoppedEvent encoderStoppedEvent)
		{
			if (this.CurrentCaptureState != LckCaptureState.InProgress)
			{
				return;
			}
			LckLog.LogError("Encoder stopped while recording - stopping recording", "OnEncoderStopped", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckRecorder.cs", 448);
			this.StopRecording(LckService.StopReason.Error);
		}

		private void OnLowStorageSpaceDetected(LckEvents.LowStorageSpaceDetectedEvent lowStorageSpaceDetectedEvent)
		{
			this.StopRecording(LckService.StopReason.LowStorageSpace);
		}

		private void TriggerRecordingStartedEvent(LckResult result)
		{
			this._eventBus.Trigger<LckEvents.RecordingStartedEvent>(new LckEvents.RecordingStartedEvent(result));
		}

		private void TriggerRecordingPausedEvent(LckResult result)
		{
			this._eventBus.Trigger<LckEvents.RecordingPausedEvent>(new LckEvents.RecordingPausedEvent(result));
		}

		private void TriggerRecordingResumedEvent(LckResult result)
		{
			this._eventBus.Trigger<LckEvents.RecordingResumedEvent>(new LckEvents.RecordingResumedEvent(result));
		}

		private void TriggerRecordingStoppedEvent(LckResult result)
		{
			this._eventBus.Trigger<LckEvents.RecordingStoppedEvent>(new LckEvents.RecordingStoppedEvent(result));
		}

		private void TriggerRecordingSavedEvent(LckResult<RecordingData> result)
		{
			this._eventBus.Trigger<LckEvents.RecordingSavedEvent>(new LckEvents.RecordingSavedEvent(result));
		}

		private void OnCaptureError(LckEvents.CaptureErrorEvent captureErrorEvent)
		{
			if (this.CurrentCaptureState == LckCaptureState.Idle || this.CurrentCaptureState == LckCaptureState.Stopping)
			{
				return;
			}
			LckLog.LogError("Stopping recording because a capture error occurred: " + captureErrorEvent.Error.Message, "OnCaptureError", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckRecorder.cs", 488);
			this.StopRecording(LckService.StopReason.Error);
		}

		private readonly ILckNativeRecordingService _nativeRecordingService;

		private readonly ILckStorageWatcher _storageWatcher;

		private readonly ILckEncoder _encoder;

		private readonly ILckOutputConfigurer _outputConfigurer;

		private readonly ILckEventBus _eventBus;

		private readonly ILckTelemetryClient _telemetryClient;

		private readonly ILckTelemetryContextProvider _telemetryContextProvider;

		private static readonly ProfilerMarker _copyOutputFileToNativeGalleryMarker = new ProfilerMarker("LckRecorder.CopyOutputFileToNativeGallery");

		private MuxerConfig _muxerConfig;

		private float _recordingStartTime;

		private float _accumulatedRecordingDuration;

		private float _lastActiveSegmentStartTime;

		private string _lastRecordingFilePath;

		private LckService.StopReason _stopReason;

		private CameraTrackDescriptor _currentRecordingDescriptor;

		private Dictionary<string, object> _recordingTelemetryContext = new Dictionary<string, object>();

		private bool _disposed;

		private WaitForSeconds _copyVideoSpinWait = new WaitForSeconds(0.1f);
	}
}
