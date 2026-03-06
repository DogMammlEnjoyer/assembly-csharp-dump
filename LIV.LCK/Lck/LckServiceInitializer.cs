using System;
using Liv.Lck.Core;
using Liv.Lck.Core.Cosmetics;
using Liv.Lck.Core.Serialization;
using Liv.Lck.DependencyInjection;
using Liv.Lck.Encoding;
using Liv.Lck.ErrorHandling;
using Liv.Lck.Recorder;
using Liv.Lck.Streaming;
using Liv.Lck.Telemetry;
using Liv.NativeAudioBridge;
using UnityEngine;

namespace Liv.Lck
{
	[DefaultExecutionOrder(-900)]
	public class LckServiceInitializer : MonoBehaviour
	{
		private void Awake()
		{
			if (LckDiContainer.Instance.HasService<ILckService>())
			{
				return;
			}
			LckServiceInitializer.ConfigureServices(LckDiContainer.Instance, this._qualityConfig, null);
		}

		public static void ConfigureServices(LckDiContainer container, ILckQualityConfig qualityConfig, Action<LckDiContainer> overrides = null)
		{
			container.AddSingleton<ILckStreamer, NullLckStreamer>();
			LckModuleLoader.Configure(container);
			container.AddSingleton<ILckQualityConfig>(qualityConfig);
			container.AddSingletonFactory<ILckCore>((LckServiceProvider provider) => new LckCoreWrapper());
			container.AddSingleton<ILckCaptureErrorDispatcher, MainThreadCaptureErrorDispatcher>();
			container.AddSingleton<ILckSerializer, LckMsgPackSerializer>();
			container.AddSingleton<ILckTelemetryContextProvider, LckTelemetryContextProvider>();
			container.AddSingleton<ILckTelemetryClient, LckTelemetryClient>();
			container.AddSingleton<ILckEventBus, LckEventBus>();
			container.AddSingleton<ILckPhotoCapture, LckPhotoCapture>();
			container.AddSingleton<ILckNativeRecordingService, LckNativeRecordingService>();
			container.AddSingleton<ILckRecorder, LckRecorder>();
			container.AddSingleton<ILckEncoder, LckEncoder>();
			container.AddSingleton<ILckPreviewer, LckPreviewer>();
			container.AddSingleton<ILckStorageWatcher, LckStorageWatcher>();
			container.AddSingleton<ILckCosmeticsCoordinator, NullLckCosmeticsCoordinator>();
			container.AddSingleton<ILckVideoMixer, LckVideoMixer>();
			container.AddSingletonForward<ILckVideoTextureProvider, ILckVideoMixer>();
			container.AddSingletonForward<ILckActiveCameraConfigurer, ILckVideoMixer>();
			container.AddSingleton<ILckAudioMixer, LckAudioMixer>();
			container.AddSingleton<ILckOutputConfigurer, LckOutputConfigurer>();
			container.AddSingleton<ILckVideoCapturer, LckVideoCapturer>();
			container.AddSingleton<ILckEncodeLooper, LckEncodeLooper>();
			container.AddSingletonFactory<INativeAudioPlayer>((LckServiceProvider provider) => new NativeAudioPlayerWindows());
			container.AddSingleton<ILckService, LckService>();
			if (overrides != null)
			{
				overrides(container);
			}
			container.Build();
		}

		[SerializeReference]
		private LckQualityConfig _qualityConfig;
	}
}
