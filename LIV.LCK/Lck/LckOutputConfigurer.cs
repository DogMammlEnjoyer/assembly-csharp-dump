using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	internal class LckOutputConfigurer : ILckOutputConfigurer
	{
		[Preserve]
		public LckOutputConfigurer(ILckQualityConfig qualityConfig, ILckEventBus eventBus)
		{
			this._eventBus = eventBus;
			this.ConfigureDefaultSettings(qualityConfig);
		}

		public LckResult ConfigureFromQualityConfig(QualityOption qualityOption)
		{
			bool flag = LckOutputConfigurer.IsValidDescriptor(qualityOption.RecordingCameraTrackDescriptor);
			if (flag)
			{
				this.SetCameraTrackDescriptor(LckCaptureType.Recording, qualityOption.RecordingCameraTrackDescriptor);
			}
			bool flag2 = LckOutputConfigurer.IsValidDescriptor(qualityOption.StreamingCameraTrackDescriptor);
			if (flag2)
			{
				this.SetCameraTrackDescriptor(LckCaptureType.Streaming, qualityOption.StreamingCameraTrackDescriptor);
			}
			return LckOutputConfigurer.CreateQualityConfigurationResult(qualityOption, flag, flag2);
		}

		public LckResult<LckCaptureType> GetActiveCaptureType()
		{
			return LckResult<LckCaptureType>.NewSuccess(this._activeCaptureType);
		}

		public LckResult SetActiveCaptureType(LckCaptureType captureType)
		{
			if (this._activeCaptureType != captureType)
			{
				this._activeCaptureType = captureType;
				this.OnActiveCameraTrackDescriptorChanged();
			}
			return LckResult.NewSuccess();
		}

		public LckResult SetActiveVideoFramerate(uint framerate)
		{
			LckCaptureType activeCaptureType = this._activeCaptureType;
			if (activeCaptureType != LckCaptureType.Recording)
			{
				if (activeCaptureType != LckCaptureType.Streaming)
				{
					return LckOutputConfigurer.NewUnknownCaptureTypeError();
				}
				this._streamingCameraTrackDescriptor.Framerate = framerate;
			}
			else
			{
				this._recordingCameraTrackDescriptor.Framerate = framerate;
			}
			this.TriggerCameraFramerateChangedEvent(framerate);
			return LckResult.NewSuccess();
		}

		public LckResult SetActiveVideoBitrate(uint bitrate)
		{
			LckCaptureType activeCaptureType = this._activeCaptureType;
			if (activeCaptureType != LckCaptureType.Recording)
			{
				if (activeCaptureType != LckCaptureType.Streaming)
				{
					return LckOutputConfigurer.NewUnknownCaptureTypeError();
				}
				this._streamingCameraTrackDescriptor.Bitrate = bitrate;
			}
			else
			{
				this._recordingCameraTrackDescriptor.Bitrate = bitrate;
			}
			return LckResult.NewSuccess();
		}

		public LckResult SetActiveAudioBitrate(uint bitrate)
		{
			LckCaptureType activeCaptureType = this._activeCaptureType;
			if (activeCaptureType != LckCaptureType.Recording)
			{
				if (activeCaptureType != LckCaptureType.Streaming)
				{
					return LckOutputConfigurer.NewUnknownCaptureTypeError();
				}
				this._streamingCameraTrackDescriptor.AudioBitrate = bitrate;
			}
			else
			{
				this._recordingCameraTrackDescriptor.AudioBitrate = bitrate;
			}
			return LckResult.NewSuccess();
		}

		public LckResult SetActiveResolution(CameraResolutionDescriptor resolution)
		{
			return this.SetResolution(this._activeCaptureType, resolution);
		}

		public LckResult<CameraTrackDescriptor> GetCameraTrackDescriptor(LckCaptureType captureType)
		{
			LckResult<CameraTrackDescriptor> result;
			if (captureType != LckCaptureType.Recording)
			{
				if (captureType != LckCaptureType.Streaming)
				{
					result = LckOutputConfigurer.NewUnknownCaptureTypeError<CameraTrackDescriptor>();
				}
				else
				{
					result = LckResult<CameraTrackDescriptor>.NewSuccess(this._streamingCameraTrackDescriptor);
				}
			}
			else
			{
				result = LckResult<CameraTrackDescriptor>.NewSuccess(this._recordingCameraTrackDescriptor);
			}
			return result;
		}

		public LckResult SetCameraTrackDescriptor(LckCaptureType captureType, CameraTrackDescriptor trackDescriptor)
		{
			if (captureType != LckCaptureType.Recording)
			{
				if (captureType != LckCaptureType.Streaming)
				{
					return LckResult.NewError(LckError.UnknownError, "Unknown capture type");
				}
				this._streamingCameraTrackDescriptor = trackDescriptor;
			}
			else
			{
				this._recordingCameraTrackDescriptor = trackDescriptor;
			}
			if (captureType == this._activeCaptureType)
			{
				this.OnActiveCameraTrackDescriptorChanged();
			}
			return LckResult.NewSuccess();
		}

		public LckResult SetCameraOrientation(LckCameraOrientation orientation)
		{
			LckResult lckResult = this.SetCameraOrientation(LckCaptureType.Recording, orientation);
			LckResult lckResult2 = this.SetCameraOrientation(LckCaptureType.Streaming, orientation);
			if (lckResult.Success && lckResult2.Success)
			{
				return LckResult.NewSuccess();
			}
			string text = "SetCameraOrientation failed with the following errors: ";
			if (!lckResult.Success)
			{
				text = text + "\n  - " + lckResult.Message;
			}
			if (!lckResult2.Success)
			{
				text = text + "\n  - " + lckResult2.Message;
			}
			return LckResult.NewError(LckError.UnknownError, text);
		}

		public LckResult<CameraTrackDescriptor> GetActiveCameraTrackDescriptor()
		{
			LckCaptureType activeCaptureType = this._activeCaptureType;
			LckResult<CameraTrackDescriptor> result;
			if (activeCaptureType != LckCaptureType.Recording)
			{
				if (activeCaptureType != LckCaptureType.Streaming)
				{
					result = LckOutputConfigurer.NewUnknownCaptureTypeError<CameraTrackDescriptor>();
				}
				else
				{
					result = LckResult<CameraTrackDescriptor>.NewSuccess(this._streamingCameraTrackDescriptor);
				}
			}
			else
			{
				result = LckResult<CameraTrackDescriptor>.NewSuccess(this._recordingCameraTrackDescriptor);
			}
			return result;
		}

		public LckResult SetActiveCameraTrackDescriptor(CameraTrackDescriptor trackDescriptor)
		{
			return this.SetCameraTrackDescriptor(this._activeCaptureType, trackDescriptor);
		}

		public LckResult<uint> GetNumberOfAudioChannels()
		{
			return LckResult<uint>.NewSuccess(2U);
		}

		public LckResult<uint> GetAudioSampleRate()
		{
			int num = LckOutputConfigurer.DetermineAudioSystemSampleRate();
			if (num <= 0)
			{
				return LckResult<uint>.NewError(LckError.UnknownError, string.Format("Invalid audio sample rate retrieved from audio system: {0}Hz", num));
			}
			return LckResult<uint>.NewSuccess((uint)num);
		}

		private void ConfigureDefaultSettings(ILckQualityConfig qualityConfig)
		{
			QualityOption qualityOption = qualityConfig.GetQualityOptionsForSystem().Find((QualityOption option) => option.IsDefault);
			LckResult lckResult = this.ConfigureFromQualityConfig(qualityOption);
			if (!lckResult.Success)
			{
				LckLog.LogError("LCK: Failed to configure default output settings - " + lckResult.Message);
			}
		}

		private void TriggerCameraResolutionChangedEvent(CameraResolutionDescriptor resolution)
		{
			LckResult<CameraResolutionDescriptor> cameraResult = LckResult<CameraResolutionDescriptor>.NewSuccess(resolution);
			this._eventBus.Trigger<LckEvents.CameraResolutionChangedEvent>(new LckEvents.CameraResolutionChangedEvent(cameraResult));
		}

		private void TriggerCameraFramerateChangedEvent(uint framerate)
		{
			LckResult<uint> result = LckResult<uint>.NewSuccess(framerate);
			this._eventBus.Trigger<LckEvents.CameraFramerateChangedEvent>(new LckEvents.CameraFramerateChangedEvent(result));
		}

		private void OnActiveCameraTrackDescriptorChanged()
		{
			CameraTrackDescriptor result = this.GetActiveCameraTrackDescriptor().Result;
			this.TriggerCameraFramerateChangedEvent(result.Framerate);
			this.TriggerCameraResolutionChangedEvent(result.CameraResolutionDescriptor);
		}

		private CameraResolutionDescriptor GetResolution(LckCaptureType captureType)
		{
			return this.GetCameraTrackDescriptor(captureType).Result.CameraResolutionDescriptor;
		}

		private LckResult SetResolution(LckCaptureType captureType, CameraResolutionDescriptor resolution)
		{
			if (captureType != LckCaptureType.Recording)
			{
				if (captureType != LckCaptureType.Streaming)
				{
					return LckOutputConfigurer.NewUnknownCaptureTypeError();
				}
				this._streamingCameraTrackDescriptor.CameraResolutionDescriptor = resolution;
			}
			else
			{
				this._recordingCameraTrackDescriptor.CameraResolutionDescriptor = resolution;
			}
			if (captureType == this._activeCaptureType)
			{
				this.TriggerCameraResolutionChangedEvent(resolution);
			}
			return LckResult.NewSuccess();
		}

		private LckResult SetCameraOrientation(LckCaptureType captureType, LckCameraOrientation orientation)
		{
			CameraResolutionDescriptor resolution = this.GetResolution(captureType);
			if (LckOutputConfigurer.GetCameraOrientation(resolution) == orientation)
			{
				return LckResult.NewSuccess();
			}
			CameraResolutionDescriptor resolutionInOrientation = resolution.GetResolutionInOrientation(orientation);
			return this.SetResolution(captureType, resolutionInOrientation);
		}

		private static LckResult CreateQualityConfigurationResult(QualityOption qualityOption, bool isRecordingValid, bool isStreamingValid)
		{
			if (isRecordingValid && isStreamingValid)
			{
				return LckResult.NewSuccess();
			}
			string text = "QualityOption (" + qualityOption.Name + ") has an invalid CameraTrackDescriptor for the following capture type(s): ";
			if (!isRecordingValid)
			{
				text += "\n  - Recording";
			}
			if (!isStreamingValid)
			{
				text += "\n  - Streaming";
			}
			return LckResult.NewError(LckError.InvalidDescriptor, text);
		}

		private static int DetermineAudioSystemSampleRate()
		{
			return AudioSettings.outputSampleRate;
		}

		private static bool IsValidDescriptor(CameraTrackDescriptor descriptor)
		{
			return descriptor.CameraResolutionDescriptor.IsValid() && (descriptor.Bitrate > 0U && descriptor.Framerate > 0U) && descriptor.AudioBitrate > 0U;
		}

		private static LckCameraOrientation GetCameraOrientation(CameraResolutionDescriptor resolution)
		{
			if (resolution.Width < resolution.Height)
			{
				return LckCameraOrientation.Portrait;
			}
			return LckCameraOrientation.Landscape;
		}

		private static LckResult<T> NewUnknownCaptureTypeError<T>()
		{
			return LckResult<T>.NewError(LckError.UnknownError, "Unknown capture type");
		}

		private static LckResult NewUnknownCaptureTypeError()
		{
			return LckResult.NewError(LckError.UnknownError, "Unknown capture type");
		}

		private readonly ILckEventBus _eventBus;

		private CameraTrackDescriptor _recordingCameraTrackDescriptor;

		private CameraTrackDescriptor _streamingCameraTrackDescriptor;

		private LckCaptureType _activeCaptureType;
	}
}
