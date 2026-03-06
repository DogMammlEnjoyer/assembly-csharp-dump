using System;
using Liv.Lck.ErrorHandling;
using Liv.Lck.Recorder;
using UnityEngine;

namespace Liv.Lck
{
	internal class LckEvents
	{
		internal interface IEventWithResult<out TResult> where TResult : ILckResult
		{
			TResult Result { get; }
		}

		internal struct EncoderStartedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public EncoderStartedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct EncoderStoppedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public EncoderStoppedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct RecordingStartedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public RecordingStartedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct RecordingPausedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public RecordingPausedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct RecordingResumedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public RecordingResumedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct RecordingStoppedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public RecordingStoppedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct RecordingSavedEvent : LckEvents.IEventWithResult<LckResult<RecordingData>>
		{
			public readonly LckResult<RecordingData> SaveResult { get; }

			public LckResult<RecordingData> Result
			{
				get
				{
					return this.SaveResult;
				}
			}

			public RecordingSavedEvent(LckResult<RecordingData> saveResult)
			{
				this.SaveResult = saveResult;
			}
		}

		internal struct StreamingStartedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public StreamingStartedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct StreamingStoppedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public StreamingStoppedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct PhotoCaptureSavedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public PhotoCaptureSavedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct LowStorageSpaceDetectedEvent : LckEvents.IEventWithResult<LckResult>
		{
			public readonly LckResult Result { get; }

			public LowStorageSpaceDetectedEvent(LckResult result)
			{
				this.Result = result;
			}
		}

		internal struct ActiveCameraChangedEvent : LckEvents.IEventWithResult<LckResult<ILckCamera>>
		{
			public readonly LckResult<ILckCamera> CameraResult { get; }

			public LckResult<ILckCamera> Result
			{
				get
				{
					return this.CameraResult;
				}
			}

			public ActiveCameraChangedEvent(LckResult<ILckCamera> cameraResult)
			{
				this.CameraResult = cameraResult;
			}
		}

		internal struct ActiveCameraTrackTextureChangedEvent : LckEvents.IEventWithResult<LckResult<RenderTexture>>
		{
			public readonly LckResult<RenderTexture> CameraTrackTextureResult { get; }

			public LckResult<RenderTexture> Result
			{
				get
				{
					return this.CameraTrackTextureResult;
				}
			}

			public ActiveCameraTrackTextureChangedEvent(LckResult<RenderTexture> cameraTrackTextureResult)
			{
				this.CameraTrackTextureResult = cameraTrackTextureResult;
			}
		}

		internal struct CameraResolutionChangedEvent : LckEvents.IEventWithResult<LckResult<CameraResolutionDescriptor>>
		{
			public readonly LckResult<CameraResolutionDescriptor> Result { get; }

			public CameraResolutionChangedEvent(LckResult<CameraResolutionDescriptor> cameraResult)
			{
				this.Result = cameraResult;
			}
		}

		internal struct CameraFramerateChangedEvent : LckEvents.IEventWithResult<LckResult<uint>>
		{
			public readonly LckResult<uint> Result { get; }

			public CameraFramerateChangedEvent(LckResult<uint> result)
			{
				this.Result = result;
			}
		}

		internal struct CaptureErrorEvent
		{
			public readonly LckCaptureError Error { get; }

			public CaptureErrorEvent(LckCaptureError error)
			{
				this.Error = error;
			}
		}
	}
}
