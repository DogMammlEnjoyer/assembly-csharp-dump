using System;
using System.Collections.Generic;
using System.Linq;
using Liv.Lck.Core;
using Liv.Lck.Settings;
using Liv.Lck.Telemetry;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	internal class LckVideoMixer : ILckVideoMixer, ILckVideoTextureProvider, ILckActiveCameraConfigurer, IDisposable
	{
		public RenderTexture CameraTrackTexture { get; private set; }

		[Preserve]
		public LckVideoMixer(ILckOutputConfigurer outputConfigurer, ILckEventBus eventBus, ILckTelemetryClient telemetryClient)
		{
			this._eventBus = eventBus;
			this._telemetryClient = telemetryClient;
			this._eventBus.AddListener<LckEvents.CameraResolutionChangedEvent>(new Action<LckEvents.CameraResolutionChangedEvent>(this.OnResolutionChanged));
			LckMediator.CameraRegistered += this.OnCameraRegistered;
			LckMediator.CameraUnregistered += this.OnCameraUnregistered;
			CameraTrackDescriptor result = outputConfigurer.GetActiveCameraTrackDescriptor().Result;
			this.UpdateTextureResolution(result.CameraResolutionDescriptor);
		}

		public LckResult<ILckCamera> GetActiveCamera()
		{
			return LckResult<ILckCamera>.NewSuccess(this._activeCamera);
		}

		public LckResult ActivateCameraById(string cameraId, string monitorId = null)
		{
			ILckCamera cameraById = LckMediator.GetCameraById(cameraId);
			if (cameraById == null)
			{
				return LckResult.NewError(LckError.CameraIdNotFound, LckResultMessageBuilder.BuildCameraIdNotFoundMessage(cameraId, LckMediator.GetCameras().ToList<ILckCamera>()));
			}
			ILckCamera activeCamera = this._activeCamera;
			if (activeCamera != null)
			{
				activeCamera.DeactivateCamera();
			}
			this._activeCamera = cameraById;
			this._activeCamera.ActivateCamera(this.CameraTrackTexture);
			this.TriggerActiveCameraChangedEvent();
			if (!string.IsNullOrEmpty(monitorId))
			{
				LckResult lckResult = this.UpdateMonitorTexture(monitorId);
				if (!lckResult.Success)
				{
					return lckResult;
				}
			}
			return LckResult.NewSuccess();
		}

		public LckResult StopActiveCamera()
		{
			if (this._activeCamera != null)
			{
				this._activeCamera.DeactivateCamera();
				this._activeCamera = null;
				this.TriggerActiveCameraChangedEvent();
			}
			return LckResult.NewSuccess();
		}

		public void Dispose()
		{
			this.ReleaseCameraTrackTextures();
			LckMediator.CameraRegistered -= this.OnCameraRegistered;
			LckMediator.CameraUnregistered -= this.OnCameraUnregistered;
		}

		private void TriggerActiveCameraChangedEvent()
		{
			this.TriggerActiveCameraChangedEvent(LckResult<ILckCamera>.NewSuccess(this._activeCamera));
		}

		private void TriggerActiveCameraChangedEvent(LckResult<ILckCamera> result)
		{
			this._eventBus.Trigger<LckEvents.ActiveCameraChangedEvent>(new LckEvents.ActiveCameraChangedEvent(result));
		}

		private void ReleaseCameraTrackTextures()
		{
			if (!this.CameraTrackTexture)
			{
				return;
			}
			this.CameraTrackTexture.Release();
			Object.Destroy(this.CameraTrackTexture);
			this.CameraTrackTexture = null;
			LckLog.Log("Released camera track texture");
		}

		private LckResult UpdateMonitorTexture(string monitorId)
		{
			ILckMonitor monitorById = LckMediator.GetMonitorById(monitorId);
			if (monitorById == null)
			{
				return LckResult.NewError(LckError.MonitorIdNotFound, LckResultMessageBuilder.BuildMonitorIdNotFoundMessage(monitorId, LckMediator.GetMonitors().ToList<ILckMonitor>()));
			}
			monitorById.SetRenderTexture(this.CameraTrackTexture);
			return LckResult.NewSuccess();
		}

		private static RenderTexture InitializeTargetRenderTexture(CameraResolutionDescriptor cameraResolutionDescriptor)
		{
			int width = (int)cameraResolutionDescriptor.Width;
			int height = (int)cameraResolutionDescriptor.Height;
			RenderTexture renderTexture = new RenderTexture(new RenderTextureDescriptor(width, height, GraphicsFormat.R8G8B8A8_UNorm, LckSettings.Instance.EnableStencilSupport ? GraphicsFormat.D24_UNorm_S8_UInt : GraphicsFormat.D16_UNorm)
			{
				memoryless = RenderTextureMemoryless.None,
				useMipMap = false,
				msaaSamples = 1,
				sRGB = true
			});
			renderTexture.antiAliasing = 1;
			renderTexture.filterMode = FilterMode.Point;
			renderTexture.name = "LCK RenderTexture";
			renderTexture.Create();
			renderTexture.GetNativeTexturePtr();
			renderTexture.GetNativeDepthBufferPtr();
			return renderTexture;
		}

		private void InitCameraTexture(CameraResolutionDescriptor resolution)
		{
			this.ReleaseCameraTrackTextures();
			this.CameraTrackTexture = LckVideoMixer.InitializeTargetRenderTexture(resolution);
			IEnumerable<ILckCamera> cameras = LckMediator.GetCameras();
			if (!this.CameraTrackTexture)
			{
				return;
			}
			if (this._activeCamera == null)
			{
				using (IEnumerator<ILckCamera> enumerator = cameras.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						goto IL_75;
					}
					ILckCamera lckCamera = enumerator.Current;
					this.ActivateCameraById(lckCamera.CameraId, null);
					goto IL_75;
				}
			}
			this.ActivateCameraById(this._activeCamera.CameraId, null);
			IL_75:
			this._eventBus.Trigger<LckEvents.ActiveCameraTrackTextureChangedEvent>(new LckEvents.ActiveCameraTrackTextureChangedEvent(LckResult<RenderTexture>.NewSuccess(this.CameraTrackTexture)));
		}

		private void OnCameraRegistered(ILckCamera camera)
		{
		}

		private void OnCameraUnregistered(ILckCamera camera)
		{
			if (this._activeCamera == camera)
			{
				this.StopActiveCamera();
			}
		}

		private void OnResolutionChanged(LckEvents.CameraResolutionChangedEvent cameraResolutionChangedEvent)
		{
			LckResult<CameraResolutionDescriptor> result = cameraResolutionChangedEvent.Result;
			if (!result.Success)
			{
				LckLog.LogWarning("LckVideoMixer ignoring failed camera resolution change (" + cameraResolutionChangedEvent.Result.Message + ")");
				return;
			}
			this.UpdateTextureResolution(result.Result);
		}

		private void UpdateTextureResolution(CameraResolutionDescriptor resolution)
		{
			try
			{
				this.InitCameraTexture(resolution);
			}
			catch (Exception ex)
			{
				Dictionary<string, object> context = new Dictionary<string, object>
				{
					{
						"errorString",
						"SetTrackResolutionFailed"
					},
					{
						"message",
						ex.Message
					}
				};
				this._telemetryClient.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.RecorderError, context));
				LckLog.LogError(ex.Message);
			}
		}

		private ILckCamera _activeCamera;

		private readonly ILckEventBus _eventBus;

		private readonly ILckTelemetryClient _telemetryClient;
	}
}
