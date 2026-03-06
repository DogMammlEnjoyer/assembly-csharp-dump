using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck.Core;
using Liv.Lck.Encoding;
using Liv.Lck.Telemetry;
using Liv.NGFX;
using UnityEngine;

namespace Liv.Lck.Streaming
{
	internal class LckStreamer : ILckStreamer, ILckCaptureStateProvider, IDisposable
	{
		public bool IsStreaming
		{
			get
			{
				return this.CurrentCaptureState > LckCaptureState.Idle;
			}
		}

		public LckResult<bool> IsPaused()
		{
			return LckResult<bool>.NewSuccess(this.CurrentCaptureState == LckCaptureState.Paused);
		}

		public LckCaptureState CurrentCaptureState { get; private set; }

		private float CurrentStreamDurationSeconds
		{
			get
			{
				return Time.time - this._streamStartTime;
			}
		}

		public LckStreamer(ILckNativeStreamingService nativeStreamingService, ILckEncoder encoder, ILckOutputConfigurer outputConfigurer, ILckEventBus eventBus, ILckTelemetryClient telemetryClient, ILckTelemetryContextProvider telemetryContextProvider)
		{
			this._nativeStreamingService = nativeStreamingService;
			this._encoder = encoder;
			this._outputConfigurer = outputConfigurer;
			this._eventBus = eventBus;
			this._telemetryClient = telemetryClient;
			this._telemetryContextProvider = telemetryContextProvider;
			this._eventBus.AddListener<LckEvents.EncoderStoppedEvent>(new Action<LckEvents.EncoderStoppedEvent>(this.OnEncoderStopped));
			this._eventBus.AddListener<LckEvents.CaptureErrorEvent>(new Action<LckEvents.CaptureErrorEvent>(this.OnCaptureError));
			Dictionary<string, object> context = new Dictionary<string, object>
			{
				{
					"service",
					"LckStreamer"
				}
			};
			this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.ServiceCreated, context));
		}

		public LckResult StartStreaming()
		{
			if (this.IsStreaming)
			{
				Dictionary<string, object> context = new Dictionary<string, object>
				{
					{
						"error",
						"Streaming already started"
					}
				};
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.StreamingError, context));
				return LckResult.NewError(LckError.StreamingError, "Streaming already started");
			}
			if (!this._nativeStreamingService.HasNativeStreamer())
			{
				LckResult lckResult = this.SetUpNativeStreamer();
				if (!lckResult.Success)
				{
					return lckResult;
				}
			}
			this.CurrentCaptureState = LckCaptureState.Starting;
			this.StartStreamingAsync();
			return LckResult.NewSuccess();
		}

		public LckResult StopStreaming(LckService.StopReason stopReason)
		{
			LckLog.Log(string.Format("LCK {0} triggered with stop reason: {1}", "StopStreaming", stopReason));
			if (!this._nativeStreamingService.HasNativeStreamer())
			{
				Dictionary<string, object> context = new Dictionary<string, object>
				{
					{
						"error",
						"Native streamer does not exist"
					}
				};
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.StreamingError, context));
				return LckResult.NewError(LckError.StreamingError, "Native streamer does not exist");
			}
			if (this._encoder == null)
			{
				Dictionary<string, object> context2 = new Dictionary<string, object>
				{
					{
						"error",
						"Encoder is null"
					}
				};
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.StreamingError, context2));
				return LckResult.NewError(LckError.StreamingError, "Encoder is null");
			}
			if (!this.IsStreaming)
			{
				Dictionary<string, object> context3 = new Dictionary<string, object>
				{
					{
						"error",
						"Streaming is already stopped"
					}
				};
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.StreamingError, context3));
				return LckResult.NewError(LckError.StreamingError, "Streaming is already stopped");
			}
			LckLog.Log("LCK Stopping Streaming");
			this.CurrentCaptureState = LckCaptureState.Stopping;
			this.StopStreamingAsync(stopReason);
			return LckResult.NewSuccess();
		}

		public LckResult<TimeSpan> GetStreamDuration()
		{
			if (!this.IsStreaming)
			{
				return LckResult<TimeSpan>.NewError(LckError.StreamingError, "Stream has not been started.");
			}
			return LckResult<TimeSpan>.NewSuccess(TimeSpan.FromSeconds((double)this.CurrentStreamDurationSeconds));
		}

		public void SetLogLevel(LogLevel logLevel)
		{
			this._nativeStreamingService.SetNativeStreamerLogLevel(logLevel);
		}

		private Task<LckResult> StartNativeStreamerAsync(int width, int height)
		{
			LckStreamer.<StartNativeStreamerAsync>d__24 <StartNativeStreamerAsync>d__;
			<StartNativeStreamerAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LckResult>.Create();
			<StartNativeStreamerAsync>d__.<>4__this = this;
			<StartNativeStreamerAsync>d__.width = width;
			<StartNativeStreamerAsync>d__.height = height;
			<StartNativeStreamerAsync>d__.<>1__state = -1;
			<StartNativeStreamerAsync>d__.<>t__builder.Start<LckStreamer.<StartNativeStreamerAsync>d__24>(ref <StartNativeStreamerAsync>d__);
			return <StartNativeStreamerAsync>d__.<>t__builder.Task;
		}

		private Task StartStreamingAsync()
		{
			LckStreamer.<StartStreamingAsync>d__25 <StartStreamingAsync>d__;
			<StartStreamingAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartStreamingAsync>d__.<>4__this = this;
			<StartStreamingAsync>d__.<>1__state = -1;
			<StartStreamingAsync>d__.<>t__builder.Start<LckStreamer.<StartStreamingAsync>d__25>(ref <StartStreamingAsync>d__);
			return <StartStreamingAsync>d__.<>t__builder.Task;
		}

		private Task<LckResult> StopNativeStreamerAsync()
		{
			LckStreamer.<StopNativeStreamerAsync>d__26 <StopNativeStreamerAsync>d__;
			<StopNativeStreamerAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LckResult>.Create();
			<StopNativeStreamerAsync>d__.<>4__this = this;
			<StopNativeStreamerAsync>d__.<>1__state = -1;
			<StopNativeStreamerAsync>d__.<>t__builder.Start<LckStreamer.<StopNativeStreamerAsync>d__26>(ref <StopNativeStreamerAsync>d__);
			return <StopNativeStreamerAsync>d__.<>t__builder.Task;
		}

		private Task StopStreamingAsync(LckService.StopReason stopReason)
		{
			LckStreamer.<StopStreamingAsync>d__27 <StopStreamingAsync>d__;
			<StopStreamingAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StopStreamingAsync>d__.<>4__this = this;
			<StopStreamingAsync>d__.<>1__state = -1;
			<StopStreamingAsync>d__.<>t__builder.Start<LckStreamer.<StopStreamingAsync>d__27>(ref <StopStreamingAsync>d__);
			return <StopStreamingAsync>d__.<>t__builder.Task;
		}

		private LckResult SetUpNativeStreamer()
		{
			if (this._nativeStreamingService.HasNativeStreamer())
			{
				Dictionary<string, object> context = new Dictionary<string, object>
				{
					{
						"error",
						"Streamer already exists"
					}
				};
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.StreamingError, context));
				return LckResult.NewError(LckError.StreamingError, "Streamer already exists");
			}
			if (!this._nativeStreamingService.CreateNativeStreamer())
			{
				Dictionary<string, object> context2 = new Dictionary<string, object>
				{
					{
						"error",
						"LCK Failed to create native streamer"
					}
				};
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.SdkError, context2));
				return LckResult.NewError(LckError.StreamingError, "LCK Failed to create native streamer");
			}
			if (!this._nativeStreamingService.GetStreamPacketCallback().IsValid)
			{
				this._nativeStreamingService.DestroyNativeStreamer();
				Dictionary<string, object> context3 = new Dictionary<string, object>
				{
					{
						"error",
						"LCK Failed to get streamer callback function"
					}
				};
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.SdkError, context3));
				return LckResult.NewError(LckError.StreamingError, "LCK Failed to get streamer callback function");
			}
			return LckResult.NewSuccess();
		}

		private void OnEncoderStopped(LckEvents.EncoderStoppedEvent encoderStoppedEvent)
		{
			if (this.CurrentCaptureState != LckCaptureState.InProgress)
			{
				return;
			}
			LckLog.LogError("Encoder stopped while streaming - stopping stream");
			Dictionary<string, object> context = new Dictionary<string, object>
			{
				{
					"error",
					"Encoder stopped unexpectedly during streaming."
				},
				{
					"streaming.durationSeconds",
					this.CurrentStreamDurationSeconds
				}
			};
			this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.RecorderError, context));
			this.StopStreaming(LckService.StopReason.Error);
		}

		private void TriggerStreamingStoppedEvent(LckResult result)
		{
			this._eventBus.Trigger<LckEvents.StreamingStoppedEvent>(new LckEvents.StreamingStoppedEvent(result));
		}

		private void TriggerStreamingStartedEvent(LckResult result)
		{
			this._eventBus.Trigger<LckEvents.StreamingStartedEvent>(new LckEvents.StreamingStartedEvent(result));
		}

		private void UpdateStreamingTelemetryContext()
		{
			this._streamingTelemetryContext = new Dictionary<string, object>
			{
				{
					"streaming.durationSeconds",
					this.CurrentStreamDurationSeconds
				},
				{
					"streaming.targetResolutionX",
					this._currentStreamDescriptor.CameraResolutionDescriptor.Width
				},
				{
					"streaming.targetResolutionY",
					this._currentStreamDescriptor.CameraResolutionDescriptor.Height
				},
				{
					"streaming.targetFramerate",
					this._currentStreamDescriptor.Framerate
				},
				{
					"streaming.targetBitrate",
					this._currentStreamDescriptor.Bitrate
				},
				{
					"streaming.targetAudioBitrate",
					this._currentStreamDescriptor.AudioBitrate
				}
			};
			LckResult<uint> numberOfAudioChannels = this._outputConfigurer.GetNumberOfAudioChannels();
			if (numberOfAudioChannels.Success)
			{
				this._streamingTelemetryContext.Add("streaming.audioChannels", numberOfAudioChannels.Result);
			}
			LckResult<uint> audioSampleRate = this._outputConfigurer.GetAudioSampleRate();
			if (audioSampleRate.Success)
			{
				this._streamingTelemetryContext.Add("streaming.audioSampleRate", audioSampleRate.Result);
			}
			this._telemetryContextProvider.SetTelemetryContext(LckTelemetryContextType.StreamingContext, this._streamingTelemetryContext);
		}

		private void OnCaptureError(LckEvents.CaptureErrorEvent captureErrorEvent)
		{
			LckCaptureState currentCaptureState = this.CurrentCaptureState;
			if (currentCaptureState == LckCaptureState.Idle || currentCaptureState == LckCaptureState.Stopping)
			{
				return;
			}
			string message = "Stopping stream because a capture error occurred: " + captureErrorEvent.Error.Message;
			this._telemetryClient.SendErrorTelemetry(LckResult.NewError(LckError.StreamingError, message));
			LckLog.LogError(message);
			this.StopStreaming(LckService.StopReason.Error);
		}

		public void Dispose()
		{
			if (this._disposed)
			{
				return;
			}
			if (this.IsStreaming && !this.StopStreaming(LckService.StopReason.ApplicationLifecycle).Success)
			{
				LckLog.LogError("Failed to stop streaming while disposing LckStreamer");
				Dictionary<string, object> context = new Dictionary<string, object>
				{
					{
						"error",
						"Failed to stop streaming while disposing LckStreamer"
					}
				};
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.StreamingError, context));
			}
			this._eventBus.RemoveListener<LckEvents.EncoderStoppedEvent>(new Action<LckEvents.EncoderStoppedEvent>(this.OnEncoderStopped));
			this._eventBus.RemoveListener<LckEvents.CaptureErrorEvent>(new Action<LckEvents.CaptureErrorEvent>(this.OnCaptureError));
			this._nativeStreamingService.DestroyNativeStreamer();
			this._disposed = true;
			Dictionary<string, object> context2 = new Dictionary<string, object>
			{
				{
					"service",
					"LckStreamer"
				}
			};
			this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.ServiceDisposed, context2));
			LckLog.Log("LckStreamer disposed");
		}

		private readonly ILckNativeStreamingService _nativeStreamingService;

		private readonly ILckEncoder _encoder;

		private readonly ILckOutputConfigurer _outputConfigurer;

		private readonly ILckEventBus _eventBus;

		private readonly ILckTelemetryClient _telemetryClient;

		private readonly ILckTelemetryContextProvider _telemetryContextProvider;

		private float _streamStartTime;

		private bool _disposed;

		private CameraTrackDescriptor _currentStreamDescriptor;

		private Dictionary<string, object> _streamingTelemetryContext = new Dictionary<string, object>();
	}
}
