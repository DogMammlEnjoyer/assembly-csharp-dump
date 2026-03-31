using System;
using Liv.Lck.Core;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Encoding;
using Liv.Lck.Recorder;
using Liv.Lck.Settings;
using Liv.Lck.Streaming;
using Liv.Lck.Telemetry;
using Liv.NativeAudioBridge;
using Liv.NGFX;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	[Preserve]
	public class LckService : ILckService, IDisposable
	{
		public event Action<LckResult> OnRecordingStarted;

		public event Action<LckResult> OnRecordingStopped;

		public event Action<LckResult> OnRecordingPaused;

		public event Action<LckResult> OnRecordingResumed;

		public event Action<LckResult> OnStreamingStarted;

		public event Action<LckResult> OnStreamingStopped;

		public event Action<LckResult> OnLowStorageSpace;

		public event Action<LckResult<RecordingData>> OnRecordingSaved;

		public event Action<LckResult> OnPhotoSaved;

		public event Action<LckResult<ILckCamera>> OnActiveCameraSet;

		[Preserve]
		internal LckService(ILckEncoder encoder, ILckRecorder recorder, ILckStreamer streamer, ILckEncodeLooper encodeLooper, ILckPhotoCapture photoCapture, ILckStorageWatcher storageWatcher, ILckVideoCapturer videoCapturer, ILckVideoMixer videoMixer, ILckAudioMixer audioMixer, ILckOutputConfigurer outputConfigurer, ILckPreviewer previewer, INativeAudioPlayer nativeAudioPlayer, ILckEventBus eventBus, ILckTelemetryClient telemetryClient)
		{
			this._encodeLooper = encodeLooper;
			this._nativeAudioPlayer = nativeAudioPlayer;
			this._encoder = encoder;
			this._recorder = recorder;
			this._streamer = streamer;
			this._photoCapture = photoCapture;
			this._storageWatcher = storageWatcher;
			this._videoMixer = videoMixer;
			this._audioMixer = audioMixer;
			this._outputConfigurer = outputConfigurer;
			this._previewer = previewer;
			this._eventBus = eventBus;
			this._telemetryClient = telemetryClient;
			this._videoCapturer = videoCapturer;
			this._eventBridge = new LckPublicApiEventBridge(this._eventBus);
			this._eventBridge.Forward<LckEvents.RecordingStartedEvent, LckResult>(delegate(LckResult r)
			{
				LckMonoBehaviourMediator.Instance.EnqueueMainThreadAction(delegate
				{
					Action<LckResult> onRecordingStarted = this.OnRecordingStarted;
					if (onRecordingStarted == null)
					{
						return;
					}
					onRecordingStarted(r);
				});
			});
			this._eventBridge.Forward<LckEvents.RecordingPausedEvent, LckResult>(delegate(LckResult r)
			{
				Action<LckResult> onRecordingPaused = this.OnRecordingPaused;
				if (onRecordingPaused == null)
				{
					return;
				}
				onRecordingPaused(r);
			});
			this._eventBridge.Forward<LckEvents.RecordingResumedEvent, LckResult>(delegate(LckResult r)
			{
				Action<LckResult> onRecordingResumed = this.OnRecordingResumed;
				if (onRecordingResumed == null)
				{
					return;
				}
				onRecordingResumed(r);
			});
			this._eventBridge.Forward<LckEvents.RecordingStoppedEvent, LckResult>(delegate(LckResult r)
			{
				Action<LckResult> onRecordingStopped = this.OnRecordingStopped;
				if (onRecordingStopped == null)
				{
					return;
				}
				onRecordingStopped(r);
			});
			this._eventBridge.Forward<LckEvents.StreamingStartedEvent, LckResult>(delegate(LckResult r)
			{
				LckMonoBehaviourMediator.Instance.EnqueueMainThreadAction(delegate
				{
					Action<LckResult> onStreamingStarted = this.OnStreamingStarted;
					if (onStreamingStarted == null)
					{
						return;
					}
					onStreamingStarted(r);
				});
			});
			this._eventBridge.Forward<LckEvents.StreamingStoppedEvent, LckResult>(delegate(LckResult r)
			{
				Action<LckResult> onStreamingStopped = this.OnStreamingStopped;
				if (onStreamingStopped == null)
				{
					return;
				}
				onStreamingStopped(r);
			});
			this._eventBridge.Forward<LckEvents.PhotoCaptureSavedEvent, LckResult>(delegate(LckResult r)
			{
				Action<LckResult> onPhotoSaved = this.OnPhotoSaved;
				if (onPhotoSaved == null)
				{
					return;
				}
				onPhotoSaved(r);
			});
			this._eventBridge.Forward<LckEvents.LowStorageSpaceDetectedEvent, LckResult>(delegate(LckResult r)
			{
				Action<LckResult> onLowStorageSpace = this.OnLowStorageSpace;
				if (onLowStorageSpace == null)
				{
					return;
				}
				onLowStorageSpace(r);
			});
			this._eventBridge.Forward<LckEvents.RecordingSavedEvent, LckResult<RecordingData>>((LckEvents.RecordingSavedEvent evt) => evt.SaveResult, delegate(LckResult<RecordingData> r)
			{
				Action<LckResult<RecordingData>> onRecordingSaved = this.OnRecordingSaved;
				if (onRecordingSaved == null)
				{
					return;
				}
				onRecordingSaved(r);
			});
			this._eventBridge.Forward<LckEvents.ActiveCameraChangedEvent, LckResult<ILckCamera>>((LckEvents.ActiveCameraChangedEvent evt) => evt.CameraResult, delegate(LckResult<ILckCamera> r)
			{
				Action<LckResult<ILckCamera>> onActiveCameraSet = this.OnActiveCameraSet;
				if (onActiveCameraSet == null)
				{
					return;
				}
				onActiveCameraSet(r);
			});
			this._eventErrorLogger = new LckEventErrorLogger(this._eventBus, delegate(ILckResult result)
			{
				LckLog.LogError(string.Format("{0}: {1}", result.Error, result.Message), ".ctor", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckService.cs", 133);
			});
			this._eventErrorLogger.Monitor<LckEvents.RecordingStartedEvent, LckResult>();
			this._eventErrorLogger.Monitor<LckEvents.RecordingPausedEvent, LckResult>();
			this._eventErrorLogger.Monitor<LckEvents.RecordingResumedEvent, LckResult>();
			this._eventErrorLogger.Monitor<LckEvents.RecordingStoppedEvent, LckResult>();
			this._eventErrorLogger.Monitor<LckEvents.StreamingStartedEvent, LckResult>();
			this._eventErrorLogger.Monitor<LckEvents.StreamingStoppedEvent, LckResult>();
			this._eventErrorLogger.Monitor<LckEvents.PhotoCaptureSavedEvent, LckResult>();
			this._eventErrorLogger.Monitor<LckEvents.LowStorageSpaceDetectedEvent, LckResult>();
			LogLevel nativeLogLevel = LckSettings.Instance.NativeLogLevel;
			NI.SetGlobalLogLevel(nativeLogLevel, LckSettings.Instance.ShowOpenGLMessages);
			this._encoder.SetLogLevel(nativeLogLevel);
			this._recorder.SetLogLevel(nativeLogLevel);
			ILckStreamer streamer2 = this._streamer;
			if (streamer2 != null)
			{
				streamer2.SetLogLevel(nativeLogLevel);
			}
			this._videoCapturer.StartCapturing();
			if (LckService.VerifyGraphicsApi() && !Application.isEditor)
			{
				LckLog.Log(string.Format("LCK version is v{0}#{1}", "1.4.5", -1), ".ctor", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckService.cs", 152);
			}
		}

		public static LckResult<LckService> GetService()
		{
			LckService lckService = (LckService)LckDiContainer.Instance.GetService<ILckService>();
			if (lckService == null)
			{
				return LckResult<LckService>.NewError(LckError.ServiceNotCreated, "Service not created");
			}
			return LckResult<LckService>.NewSuccess(lckService);
		}

		public LckResult StartRecording()
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._recorder.StartRecording();
		}

		public LckResult PauseRecording()
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._recorder.PauseRecording();
		}

		public LckResult ResumeRecording()
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._recorder.ResumeRecording();
		}

		public LckResult StopRecording()
		{
			return this.StopRecording(LckService.StopReason.UserStopped);
		}

		public LckResult StartStreaming()
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this._streamer == null || this._streamer is NullLckStreamer)
			{
				return LckResult.NewError(LckError.StreamerNotImplemented, "LCK streaming package not available");
			}
			return this._streamer.StartStreaming();
		}

		public LckResult StopStreaming()
		{
			return this.StopStreaming(LckService.StopReason.UserStopped);
		}

		public LckResult StopStreaming(LckService.StopReason stopReason)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this._streamer == null || this._streamer is NullLckStreamer)
			{
				return LckResult.NewError(LckError.StreamerNotImplemented, "LCK streaming package not available");
			}
			return this._streamer.StopStreaming(stopReason);
		}

		public LckResult<TimeSpan> GetRecordingDuration()
		{
			return this._recorder.GetRecordingDuration();
		}

		public LckResult<TimeSpan> GetStreamDuration()
		{
			return this._streamer.GetStreamDuration();
		}

		public LckResult SetTrackResolution(CameraResolutionDescriptor cameraResolutionDescriptor)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this.IsCapturing().Result)
			{
				return LckResult.NewError(LckError.CantEditSettingsWhileCapturing, "Can't change resolution while capturing.");
			}
			return this._outputConfigurer.SetActiveResolution(cameraResolutionDescriptor);
		}

		public LckResult SetCameraOrientation(LckCameraOrientation orientation)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this.IsCapturing().Result)
			{
				return LckResult.NewError(LckError.CantEditSettingsWhileCapturing, "Can't change camera orientation while capturing.");
			}
			return this._outputConfigurer.SetCameraOrientation(orientation);
		}

		public LckResult SetTrackFramerate(uint framerate)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this.IsCapturing().Result)
			{
				return LckResult.NewError(LckError.CantEditSettingsWhileCapturing, "Can't change framerate while capturing.");
			}
			return this._outputConfigurer.SetActiveVideoFramerate(framerate);
		}

		public LckResult SetPreviewActive(bool isActive)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			this._previewer.IsPreviewActive = isActive;
			return LckResult.NewSuccess();
		}

		public LckResult SetTrackDescriptor(CameraTrackDescriptor cameraTrackDescriptor)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this.IsCapturing().Result)
			{
				return LckResult.NewError(LckError.CantEditSettingsWhileCapturing, "Can't change track descriptor while capturing.");
			}
			return this._outputConfigurer.SetActiveCameraTrackDescriptor(cameraTrackDescriptor);
		}

		public LckResult SetTrackDescriptor(LckCaptureType captureType, CameraTrackDescriptor cameraTrackDescriptor)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this.IsCapturing().Result)
			{
				return LckResult.NewError(LckError.CantEditSettingsWhileCapturing, "Can't change track descriptor while capturing.");
			}
			return this._outputConfigurer.SetCameraTrackDescriptor(captureType, cameraTrackDescriptor);
		}

		public LckResult SetTrackBitrate(uint bitrate)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this.IsCapturing().Result)
			{
				return LckResult.NewError(LckError.CantEditSettingsWhileCapturing, "Can't change video bitrate while capturing.");
			}
			return this._outputConfigurer.SetActiveVideoBitrate(bitrate);
		}

		public LckResult SetTrackAudioBitrate(uint audioBitrate)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this.IsCapturing().Result)
			{
				return LckResult.NewError(LckError.CantEditSettingsWhileCapturing, "Can't change audio bitrate while capturing.");
			}
			return this._outputConfigurer.SetActiveAudioBitrate(audioBitrate);
		}

		public LckResult<bool> IsRecording()
		{
			if (this._disposed)
			{
				return LckResult<bool>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._recorder.IsRecording();
		}

		public LckResult<bool> IsStreaming()
		{
			if (this._disposed)
			{
				return LckResult<bool>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this._streamer == null || this._streamer is NullLckStreamer)
			{
				return LckResult<bool>.NewError(LckError.StreamerNotImplemented, "LCK streaming package not available");
			}
			return LckResult<bool>.NewSuccess(this._streamer.IsStreaming);
		}

		public LckResult<bool> IsPaused()
		{
			if (this._disposed)
			{
				return LckResult<bool>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._recorder.IsPaused();
		}

		public LckResult<bool> IsCapturing()
		{
			if (this._disposed)
			{
				return LckResult<bool>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return LckResult<bool>.NewSuccess(this._encoder.IsActive());
		}

		public LckResult SetGameAudioCaptureActive(bool isActive)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._audioMixer.SetGameAudioMute(!isActive);
		}

		public LckResult SetMicrophoneCaptureActive(bool isActive)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._audioMixer.SetMicrophoneCaptureActive(isActive);
		}

		public LckResult<float> GetMicrophoneOutputLevel()
		{
			if (this._disposed)
			{
				return LckResult<float>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return LckResult<float>.NewSuccess(this._audioMixer.GetMicrophoneOutputLevel());
		}

		public LckResult SetMicrophoneGain(float gain)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			this._audioMixer.SetMicrophoneGain(gain);
			return LckResult.NewSuccess();
		}

		public LckResult SetGameAudioGain(float gain)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			this._audioMixer.SetGameAudioGain(gain);
			return LckResult.NewSuccess();
		}

		public LckResult<float> GetGameOutputLevel()
		{
			if (this._disposed)
			{
				return LckResult<float>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return LckResult<float>.NewSuccess(this._audioMixer.GetGameOutputLevel());
		}

		public LckResult<bool> IsGameAudioMute()
		{
			if (this._disposed)
			{
				return LckResult<bool>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._audioMixer.IsGameAudioMute();
		}

		public LckResult SetActiveCamera(string cameraId, string monitorId = null)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._videoMixer.ActivateCameraById(cameraId, monitorId);
		}

		public LckResult<ILckCamera> GetActiveCamera()
		{
			if (this._disposed)
			{
				return LckResult<ILckCamera>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._videoMixer.GetActiveCamera();
		}

		public LckResult PreloadDiscreetAudio(AudioClip audioClip, float volume, bool forceReload = false)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			INativeAudioPlayer nativeAudioPlayer = this._nativeAudioPlayer;
			if (nativeAudioPlayer != null)
			{
				nativeAudioPlayer.PreloadAudioClip(audioClip, volume, forceReload);
			}
			return LckResult.NewSuccess();
		}

		public LckResult PlayDiscreetAudioClip(AudioClip audioClip)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			INativeAudioPlayer nativeAudioPlayer = this._nativeAudioPlayer;
			if (nativeAudioPlayer != null)
			{
				nativeAudioPlayer.PlayAudioClip(audioClip, 1f);
			}
			return LckResult.NewSuccess();
		}

		public LckResult StopAllDiscreetAudio()
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			INativeAudioPlayer nativeAudioPlayer = this._nativeAudioPlayer;
			if (nativeAudioPlayer != null)
			{
				nativeAudioPlayer.StopAllAudio();
			}
			return LckResult.NewSuccess();
		}

		public LckResult<LckDescriptor> GetDescriptor()
		{
			if (this._disposed)
			{
				return LckResult<LckDescriptor>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			LckResult<LckCaptureType> activeCaptureType = this.GetActiveCaptureType();
			if (!activeCaptureType.Success)
			{
				return LckResult<LckDescriptor>.NewError(LckError.UnknownError, "Failed to get active capture type");
			}
			LckCaptureType result = activeCaptureType.Result;
			LckResult<CameraTrackDescriptor> cameraTrackDescriptor = this._outputConfigurer.GetCameraTrackDescriptor(result);
			if (!cameraTrackDescriptor.Success)
			{
				return LckResult<LckDescriptor>.NewError(LckError.UnknownError, "Failed to get camera track descriptor");
			}
			return LckResult<LckDescriptor>.NewSuccess(new LckDescriptor
			{
				cameraTrackDescriptor = cameraTrackDescriptor.Result
			});
		}

		public LckResult CapturePhoto()
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			if (this._photoCapture == null)
			{
				return LckResult.NewError(LckError.PhotoCaptureError, "Failed to Capture Photo, LckPhotoCapture is null");
			}
			return this._photoCapture.Capture();
		}

		public LckResult<LckCaptureType> GetActiveCaptureType()
		{
			if (this._disposed)
			{
				return LckResult<LckCaptureType>.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._outputConfigurer.GetActiveCaptureType();
		}

		public LckResult SetActiveCaptureType(LckCaptureType captureType)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._outputConfigurer.SetActiveCaptureType(captureType);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (disposing)
				{
					ILckEncodeLooper encodeLooper = this._encodeLooper;
					if (encodeLooper != null)
					{
						encodeLooper.Dispose();
					}
					this._encodeLooper = null;
					ILckVideoCapturer videoCapturer = this._videoCapturer;
					if (videoCapturer != null)
					{
						videoCapturer.Dispose();
					}
					this._videoCapturer = null;
					ILckEncoder encoder = this._encoder;
					if (encoder != null)
					{
						encoder.Dispose();
					}
					this._encoder = null;
					ILckRecorder recorder = this._recorder;
					if (recorder != null)
					{
						recorder.Dispose();
					}
					this._recorder = null;
					ILckStreamer streamer = this._streamer;
					if (streamer != null)
					{
						streamer.Dispose();
					}
					this._streamer = null;
					ILckAudioMixer audioMixer = this._audioMixer;
					if (audioMixer != null)
					{
						audioMixer.Dispose();
					}
					this._audioMixer = null;
					ILckVideoMixer videoMixer = this._videoMixer;
					if (videoMixer != null)
					{
						videoMixer.Dispose();
					}
					this._videoMixer = null;
					ILckStorageWatcher storageWatcher = this._storageWatcher;
					if (storageWatcher != null)
					{
						storageWatcher.Dispose();
					}
					this._storageWatcher = null;
					INativeAudioPlayer nativeAudioPlayer = this._nativeAudioPlayer;
					if (nativeAudioPlayer != null)
					{
						nativeAudioPlayer.Dispose();
					}
					this._nativeAudioPlayer = null;
					ILckPreviewer previewer = this._previewer;
					if (previewer != null)
					{
						previewer.Dispose();
					}
					this._previewer = null;
					ILckPhotoCapture photoCapture = this._photoCapture;
					if (photoCapture != null)
					{
						photoCapture.Dispose();
					}
					this._photoCapture = null;
					LckPublicApiEventBridge eventBridge = this._eventBridge;
					if (eventBridge != null)
					{
						eventBridge.Dispose();
					}
					LckMonoBehaviourMediator.StopAllActiveCoroutines();
				}
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.ServiceDisposed));
				LckLog.Log("LCK service disposed.", "Dispose", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckService.cs", 686);
				this._disposed = true;
			}
		}

		~LckService()
		{
			this.Dispose(false);
		}

		internal LckResult StopRecording(LckService.StopReason stopReason = LckService.StopReason.UserStopped)
		{
			if (this._disposed)
			{
				return LckResult.NewError(LckError.ServiceDisposed, "Service has been disposed");
			}
			return this._recorder.StopRecording(stopReason);
		}

		internal static bool VerifyGraphicsApi()
		{
			GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.WindowsPlayer && platform != RuntimePlatform.WindowsEditor)
			{
				if (platform != RuntimePlatform.Android)
				{
					return false;
				}
				if (graphicsDeviceType == GraphicsDeviceType.Vulkan || graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
				{
					return true;
				}
				LckLog.LogError("LCK requires Vulkan or OpenGLES3 graphics API on Android. Any other api is not supported.", "VerifyGraphicsApi", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckService.cs", 717);
				return false;
			}
			else
			{
				if (graphicsDeviceType == GraphicsDeviceType.Vulkan || graphicsDeviceType == GraphicsDeviceType.Direct3D11 || graphicsDeviceType == GraphicsDeviceType.OpenGLCore)
				{
					return true;
				}
				LckLog.LogError("LCK requires the Vulkan, OpenGLCore or DirectX 11 graphics API on Windows. Any other api is not supported.", "VerifyGraphicsApi", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckService.cs", 726);
				return false;
			}
		}

		internal static bool VerifyPlatform()
		{
			bool flag = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
			if (!flag)
			{
				LckLog.LogError(string.Format("LCK is not supported on {0}.", Application.platform), "VerifyPlatform", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckService.cs", 737);
			}
			return flag;
		}

		private readonly ILckOutputConfigurer _outputConfigurer;

		private ILckEncodeLooper _encodeLooper;

		private INativeAudioPlayer _nativeAudioPlayer;

		private ILckEncoder _encoder;

		private bool _disposed;

		private ILckRecorder _recorder;

		private ILckStreamer _streamer;

		private ILckPhotoCapture _photoCapture;

		private ILckStorageWatcher _storageWatcher;

		private ILckVideoMixer _videoMixer;

		private ILckAudioMixer _audioMixer;

		private ILckPreviewer _previewer;

		private ILckEventBus _eventBus;

		private ILckVideoCapturer _videoCapturer;

		private readonly ILckTelemetryClient _telemetryClient;

		private readonly LckPublicApiEventBridge _eventBridge;

		private readonly LckEventErrorLogger _eventErrorLogger;

		public enum StopReason
		{
			UserStopped,
			LowStorageSpace,
			Error,
			ApplicationLifecycle
		}
	}
}
