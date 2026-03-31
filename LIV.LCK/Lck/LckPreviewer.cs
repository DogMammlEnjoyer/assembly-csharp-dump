using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	internal class LckPreviewer : ILckPreviewer, IDisposable
	{
		public bool IsPreviewActive { get; set; } = true;

		[Preserve]
		public LckPreviewer(ILckVideoTextureProvider videoTextureProvider, ILckEventBus eventBus)
		{
			this._videoTextureProvider = videoTextureProvider;
			this._eventBus = eventBus;
			this._eventBus.AddListener<LckEvents.ActiveCameraTrackTextureChangedEvent>(new Action<LckEvents.ActiveCameraTrackTextureChangedEvent>(this.OnCameraTrackTextureChanged));
			LckMediator.MonitorRegistered += this.OnMonitorRegistered;
			LckMediator.MonitorUnregistered += LckPreviewer.OnMonitorUnregistered;
		}

		private void SetMonitorRenderTexture(ILckMonitor monitor)
		{
			RenderTexture cameraTrackTexture = this._videoTextureProvider.CameraTrackTexture;
			if (cameraTrackTexture == null)
			{
				LckLog.LogWarning("LCK Camera track texture not found.", "SetMonitorRenderTexture", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckPreviewer.cs", 29);
				return;
			}
			if (monitor == null)
			{
				LckLog.LogWarning("LCK Monitor not found.", "SetMonitorRenderTexture", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\LckPreviewer.cs", 35);
				return;
			}
			monitor.SetRenderTexture(cameraTrackTexture);
		}

		private void OnMonitorRegistered(ILckMonitor monitor)
		{
			this.SetMonitorRenderTexture(monitor);
		}

		private static void OnMonitorUnregistered(ILckMonitor monitor)
		{
			if (monitor != null)
			{
				monitor.SetRenderTexture(null);
			}
		}

		private void SetMonitorTextureForAllMonitors()
		{
			foreach (ILckMonitor monitorRenderTexture in LckMediator.GetMonitors())
			{
				this.SetMonitorRenderTexture(monitorRenderTexture);
			}
		}

		private void OnCameraTrackTextureChanged(LckEvents.ActiveCameraTrackTextureChangedEvent activeCameraTrackTextureChangedEvent)
		{
			this.SetMonitorTextureForAllMonitors();
		}

		public void Dispose()
		{
			ILckEventBus eventBus = this._eventBus;
			if (eventBus != null)
			{
				eventBus.RemoveListener<LckEvents.ActiveCameraTrackTextureChangedEvent>(new Action<LckEvents.ActiveCameraTrackTextureChangedEvent>(this.OnCameraTrackTextureChanged));
			}
			LckMediator.MonitorRegistered -= this.OnMonitorRegistered;
			LckMediator.MonitorUnregistered -= LckPreviewer.OnMonitorUnregistered;
		}

		private readonly ILckVideoTextureProvider _videoTextureProvider;

		private readonly ILckEventBus _eventBus;
	}
}
