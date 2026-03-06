using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AOT;
using Liv.Lck.Collections;
using Liv.Lck.Core;
using Liv.Lck.ErrorHandling;
using Liv.Lck.Telemetry;
using Liv.NGFX;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Liv.Lck.Encoding
{
	internal class LckEncoder : ILckEncoder, IDisposable
	{
		private static ILckCaptureErrorDispatcher CaptureErrorDispatcher { get; set; }

		[Preserve]
		public LckEncoder(ILckOutputConfigurer outputConfigurer, ILckVideoTextureProvider videoTextureProvider, ILckEventBus eventBus, ILckTelemetryClient telemetryClient, ILckCaptureErrorDispatcher captureErrorDispatcher)
		{
			this._outputConfigurer = outputConfigurer;
			this._videoTextureProvider = videoTextureProvider;
			this._eventBus = eventBus;
			this._telemetryClient = telemetryClient;
			LckEncoder.CaptureErrorDispatcher = captureErrorDispatcher;
		}

		public bool IsActive()
		{
			return this._isActive;
		}

		public bool IsPaused()
		{
			return this._registeredPacketHandlers.All((LckEncodedPacketHandler encodedPacketHandler) => encodedPacketHandler.CaptureStateProvider.IsPaused().Result);
		}

		public LckResult StartEncoding(CameraTrackDescriptor cameraTrackDescriptor, IEnumerable<LckEncodedPacketHandler> encodedPacketHandlers)
		{
			if (this._isActive)
			{
				return LckResult.NewError(LckError.CaptureAlreadyStarted, "Encoding has already started");
			}
			if (!this.CreateEncoderInstance())
			{
				return LckResult.NewError(LckError.EncodingError, "Failed to create encoder instance");
			}
			this.AddEncodedPacketHandlers(encodedPacketHandlers);
			this._resourceContext = LckNativeEncodingApi.GetResourceContext(this._encoderContext);
			if (this._resourceContext == IntPtr.Zero)
			{
				return LckResult.NewError(LckError.EncodingError, "Resource context pointer is not set");
			}
			this._resourceInitData = new Handle<LckNativeEncodingApi.ResourceData>(new LckNativeEncodingApi.ResourceData
			{
				encoderContext = this._encoderContext
			});
			LckNativeEncodingApi.TrackInfo[] array = LckEncoder.CreateTrackInfoInteropData(cameraTrackDescriptor, this._outputConfigurer.GetAudioSampleRate().Result, this._outputConfigurer.GetNumberOfAudioChannels().Result);
			LckResult lckResult = this.InitCameraRenderData(array);
			if (!lckResult.Success)
			{
				return lckResult;
			}
			if (!LckNativeEncodingApi.StartEncoder(this._encoderContext, array, (uint)array.Length))
			{
				return LckResult.NewError(LckError.EncodingError, "Failed to start native encoder");
			}
			this.ExecuteNativeInitResourcesFunction();
			this.InitTextureHandles();
			this._isActive = true;
			this._currentEncoderSessionData = default(EncoderSessionData);
			LckLog.Log("Encoder started successfully");
			LckResult result = LckResult.NewSuccess();
			this._eventBus.Trigger<LckEvents.EncoderStartedEvent>(new LckEvents.EncoderStartedEvent(result));
			return result;
		}

		public Task<LckResult> StopEncodingAsync()
		{
			LckEncoder.<StopEncodingAsync>d__30 <StopEncodingAsync>d__;
			<StopEncodingAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LckResult>.Create();
			<StopEncodingAsync>d__.<>4__this = this;
			<StopEncodingAsync>d__.<>1__state = -1;
			<StopEncodingAsync>d__.<>t__builder.Start<LckEncoder.<StopEncodingAsync>d__30>(ref <StopEncodingAsync>d__);
			return <StopEncodingAsync>d__.<>t__builder.Task;
		}

		public bool EncodeFrame(float videoTimeSeconds, AudioBuffer audioData, bool encodeVideo)
		{
			if (!this.IsActive())
			{
				LckLog.LogError("Cannot encode frame - encoder is not open");
				return false;
			}
			try
			{
				this.ProvideDataToEncoder(videoTimeSeconds, audioData, encodeVideo);
			}
			catch (Exception ex)
			{
				this.HandleEncodeFrameError("LCK EncodeFrame failed: " + ex.Message, new Dictionary<string, object>
				{
					{
						"errorString",
						"EncodeFrameFailed"
					},
					{
						"message",
						ex.Message
					}
				});
				return false;
			}
			return true;
		}

		public void SetLogLevel(LogLevel logLevel)
		{
			this._logLevel = logLevel;
			if (this._encoderContext != IntPtr.Zero)
			{
				LckNativeEncodingApi.SetEncoderLogLevel(this._encoderContext, (uint)this._logLevel);
			}
		}

		public EncoderSessionData GetCurrentSessionData()
		{
			return this._currentEncoderSessionData;
		}

		private void ProvideDataToEncoder(float videoTime, AudioBuffer audioData, bool encodeVideo)
		{
			using (Handle<float[]> handle = new Handle<float[]>(audioData.Buffer))
			{
				this._audioTracks[0].data = handle.ptr();
				this._audioTracks[0].dataSize = (uint)audioData.Count;
				this._audioTracks[0].timestampSamples = this._currentEncoderSessionData.EncodedAudioSamplesPerChannel;
				this._audioTracks[0].trackIndex = 0U;
				this._readyVideoTracks[0] = encodeVideo;
				LckEncoder.EncodeFrameData(this.AllocateFrameSubmission(videoTime, this._readyVideoTracks, this._audioTracks));
				if (this._readyVideoTracks[0])
				{
					ulong encodedVideoFrames = this._currentEncoderSessionData.EncodedVideoFrames;
					this._currentEncoderSessionData.EncodedVideoFrames = encodedVideoFrames + 1UL;
				}
				this._currentEncoderSessionData.EncodedAudioSamplesPerChannel = this._currentEncoderSessionData.EncodedAudioSamplesPerChannel + (ulong)(this._audioTracks[0].dataSize / this._outputConfigurer.GetNumberOfAudioChannels().Result);
				this._currentEncoderSessionData.CaptureTimeSeconds = videoTime;
			}
		}

		private void AddEncodedPacketHandler(LckEncodedPacketHandler handler)
		{
			if (this._encoderContext == IntPtr.Zero)
			{
				LckLog.LogError("Cannot add encoded packet handler - invalid encoder context");
				return;
			}
			if (!handler.EncodedPacketCallback.IsValid)
			{
				LckLog.LogError("Cannot add encoded packet handler - missing callback object or function pointer");
				return;
			}
			if (this._registeredPacketHandlers.Contains(handler))
			{
				LckLog.LogError("Cannot add encoded packet handler - it is already registered");
				return;
			}
			this._registeredPacketHandlers.Add(handler);
			LckEncodedPacketCallback encodedPacketCallback = handler.EncodedPacketCallback;
			LckNativeEncodingApi.AddEncoderPacketCallback(this._encoderContext, encodedPacketCallback.CallbackObjectPtr, encodedPacketCallback.CallbackFunctionPtr);
			LckLog.Log("Encoder packet handler added");
		}

		private void AddEncodedPacketHandlers(IEnumerable<LckEncodedPacketHandler> encodedPacketHandlers)
		{
			foreach (LckEncodedPacketHandler handler in encodedPacketHandlers)
			{
				this.AddEncodedPacketHandler(handler);
			}
		}

		private void RemoveEncodedPacketHandler(LckEncodedPacketHandler handler)
		{
			if (!this._registeredPacketHandlers.Remove(handler))
			{
				LckLog.LogError("Cannot remove encoded packet handler - it is not registered");
				return;
			}
			LckEncodedPacketCallback encodedPacketCallback = handler.EncodedPacketCallback;
			LckNativeEncodingApi.RemoveEncoderPacketCallback(this._encoderContext, encodedPacketCallback.CallbackObjectPtr, encodedPacketCallback.CallbackFunctionPtr);
			LckLog.Log("Removed encoded packet handler");
		}

		private bool CreateEncoderInstance()
		{
			if (this._encoderContext != IntPtr.Zero)
			{
				LckLog.LogWarning("Encoder context is already set");
				return false;
			}
			this._encoderContext = LckNativeEncodingApi.CreateEncoder();
			if (this._encoderContext == IntPtr.Zero)
			{
				LckLog.LogError("Failed to create native encoder");
				return false;
			}
			LckNativeEncodingApi.SetEncoderLogLevel(this._encoderContext, (uint)this._logLevel);
			if (!LckNativeEncodingApi.SetCaptureErrorCallback(this._encoderContext, new LckNativeEncodingApi.CaptureErrorCallback(LckEncoder.OnNativeCaptureError)))
			{
				LckLog.LogError("Failed to set encoder error callback");
				return false;
			}
			LckLog.Log("Encoder created successfully");
			return true;
		}

		private IntPtr AllocateFrameSubmission(float frameTime, bool[] readyTracks, LckNativeEncodingApi.AudioTrack[] audioTracks)
		{
			return LckNativeEncodingApi.AllocateFrameSubmission(new LckNativeEncodingApi.FrameSubmission
			{
				encoderContext = this._encoderContext,
				textureIDs = this._textureIds.ptr(),
				textureIDsSize = (uint)this._textureIds.data().Length,
				videoTimestampMilli = (ulong)(frameTime * 1000f),
				audioTracksSize = 1U,
				readyFramesSize = 1U
			}, audioTracks, readyTracks);
		}

		private static void EncodeFrameData(IntPtr framePtr)
		{
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(LckNativeEncodingApi.GetPluginUpdateFunction(), 1, framePtr);
			commandBuffer.name = "qck Encoder";
			Graphics.ExecuteCommandBuffer(commandBuffer);
		}

		public void ReleaseNativeRenderBuffers()
		{
			using (LckEncoder._releaseNativeRenderBufferMarker.Auto())
			{
				if (this.IsActive())
				{
					LckLog.LogWarning("LCK can't release native render buffers while encoder is active");
				}
				else
				{
					foreach (LckEncoder.CaptureData captureData in this._cameraRenderData)
					{
						captureData.nativeRenderBuffer.Dispose();
					}
				}
			}
		}

		public int GetAudioFrameSize()
		{
			return (int)LckNativeEncodingApi.GetAudioTrackFrameSize(this._encoderContext, 0U);
		}

		private void ReleaseResources()
		{
			LckLog.Log("Releasing encoder resources");
			this.ReleaseNativeRenderBuffers();
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(LckNativeEncodingApi.GetReleaseResourcesFunction(), 1, this._resourceInitData.ptr());
			commandBuffer.name = "qck ReleaseResource";
			Graphics.ExecuteCommandBuffer(commandBuffer);
		}

		private LckResult InitCameraRenderData(LckNativeEncodingApi.TrackInfo[] trackInfo)
		{
			this._cameraRenderData = new List<LckEncoder.CaptureData>();
			foreach (ValueTuple<LckNativeEncodingApi.TrackInfo, int> valueTuple in trackInfo.Select((LckNativeEncodingApi.TrackInfo track, int trackIndex) => new ValueTuple<LckNativeEncodingApi.TrackInfo, int>(track, trackIndex)).ToArray<ValueTuple<LckNativeEncodingApi.TrackInfo, int>>())
			{
				LckNativeEncodingApi.TrackInfo item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				if (item.type == LckNativeEncodingApi.TrackType.Video)
				{
					this._cameraRenderData.Add(this.InitCameraRenderData(item2));
				}
			}
			if (!this._cameraRenderData.Any<LckEncoder.CaptureData>())
			{
				return LckResult.NewError(LckError.EncodingError, "No video tracks found");
			}
			return LckResult.NewSuccess();
		}

		private LckEncoder.CaptureData InitCameraRenderData(int trackIndex)
		{
			bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3;
			RenderTexture cameraTrackTexture = this._videoTextureProvider.CameraTrackTexture;
			return new LckEncoder.CaptureData
			{
				nativeRenderBuffer = (flag ? new NativeRenderBuffer(this._resourceContext, cameraTrackTexture.colorBuffer, cameraTrackTexture.GetNativeTexturePtr(), cameraTrackTexture.width, cameraTrackTexture.height, 1, GraphicsFormat.R8G8B8A8_UNorm) : new NativeRenderBuffer(this._resourceContext, cameraTrackTexture.colorBuffer, cameraTrackTexture.width, cameraTrackTexture.height, 1, GraphicsFormat.R8G8B8A8_UNorm)),
				trackIndex = (uint)trackIndex
			};
		}

		private void InitTextureHandles()
		{
			this._texturesList = new List<LckNativeEncodingApi.FrameTexture>();
			foreach (LckEncoder.CaptureData captureData in this._cameraRenderData)
			{
				this._texturesList.Add(new LckNativeEncodingApi.FrameTexture
				{
					id = captureData.nativeRenderBuffer.id,
					trackIndex = captureData.trackIndex
				});
			}
			this._textureIds = new Handle<LckNativeEncodingApi.FrameTexture[]>(this._texturesList.ToArray());
		}

		private void ExecuteNativeInitResourcesFunction()
		{
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.IssuePluginEventAndData(LckNativeEncodingApi.GetInitResourcesFunction(), 1, this._resourceInitData.ptr());
			commandBuffer.name = "qck InitResource";
			Graphics.ExecuteCommandBuffer(commandBuffer);
		}

		private void UnregisterEncodedPacketHandlers()
		{
			foreach (LckEncodedPacketHandler handler in this._registeredPacketHandlers.ToArray<LckEncodedPacketHandler>())
			{
				this.RemoveEncodedPacketHandler(handler);
			}
		}

		private void HandleEncodeFrameError(string errorMessage, Dictionary<string, object> telemetryData)
		{
			LckLog.LogError(errorMessage);
			this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.RecorderError, telemetryData));
			this.StopEncodingAsync();
		}

		private static LckNativeEncodingApi.TrackInfo[] CreateTrackInfoInteropData(CameraTrackDescriptor cameraTrackDescriptor, uint audioSampleRate, uint numberOfAudioChannels)
		{
			return new LckNativeEncodingApi.TrackInfo[]
			{
				new LckNativeEncodingApi.TrackInfo
				{
					type = LckNativeEncodingApi.TrackType.Audio,
					bitrate = cameraTrackDescriptor.AudioBitrate,
					samplerate = audioSampleRate,
					channels = numberOfAudioChannels
				},
				new LckNativeEncodingApi.TrackInfo
				{
					type = LckNativeEncodingApi.TrackType.Video,
					bitrate = cameraTrackDescriptor.Bitrate,
					width = cameraTrackDescriptor.CameraResolutionDescriptor.Width,
					height = cameraTrackDescriptor.CameraResolutionDescriptor.Height,
					framerate = cameraTrackDescriptor.Framerate
				}
			};
		}

		[MonoPInvokeCallback(typeof(LckNativeEncodingApi.CaptureErrorCallback))]
		private static void OnNativeCaptureError(CaptureErrorType errorType, string errorMessage)
		{
			if (LckEncoder.CaptureErrorDispatcher != null)
			{
				LckEncoder.CaptureErrorDispatcher.PushError(new LckCaptureError(errorType, errorMessage));
				return;
			}
			LckLog.LogError("The CaptureErrorDispatcher reference is null while error occurred - Error will not be handled: " + errorMessage);
		}

		public void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			if (this.IsActive())
			{
				LckResult result = this.StopEncodingAsync().Result;
				if (!result.Success)
				{
					LckLog.LogError("LckEncoder was disposed while active, but failed to stop encoding: " + result.Message);
				}
			}
			LckEncoder.CaptureErrorDispatcher = null;
			this._disposed = true;
		}

		private readonly ILckOutputConfigurer _outputConfigurer;

		private readonly ILckVideoTextureProvider _videoTextureProvider;

		private readonly ILckEventBus _eventBus;

		private readonly ILckTelemetryClient _telemetryClient;

		private readonly ILckCaptureErrorDispatcher _captureErrorDispatcher;

		private readonly IList<LckEncodedPacketHandler> _registeredPacketHandlers = new List<LckEncodedPacketHandler>();

		private readonly LckNativeEncodingApi.AudioTrack[] _audioTracks = new LckNativeEncodingApi.AudioTrack[1];

		private readonly bool[] _readyVideoTracks = new bool[1];

		private static readonly ProfilerMarker _allocateFrameSubmissionMarker = new ProfilerMarker("LckEncoder.AllocateFrameSubmission");

		private static readonly ProfilerMarker _commandBufferMarker = new ProfilerMarker("LckEncoder.CommandBuffer");

		private static readonly ProfilerMarker _releaseNativeRenderBufferMarker = new ProfilerMarker("LckEncoder.ReleaseNativeRenderBuffer");

		private LogLevel _logLevel = LogLevel.Error;

		private IntPtr _encoderContext;

		private Handle<LckNativeEncodingApi.FrameTexture[]> _textureIds;

		private List<LckNativeEncodingApi.FrameTexture> _texturesList;

		private Handle<LckNativeEncodingApi.ResourceData> _resourceInitData;

		private List<LckEncoder.CaptureData> _cameraRenderData = new List<LckEncoder.CaptureData>();

		private bool _isActive;

		private IntPtr _resourceContext = IntPtr.Zero;

		private bool _disposed;

		private EncoderSessionData _currentEncoderSessionData;

		private struct CaptureData
		{
			public NativeRenderBuffer nativeRenderBuffer;

			public uint trackIndex;
		}
	}
}
