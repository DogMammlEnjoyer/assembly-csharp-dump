using System;
using System.Collections.Generic;

namespace Liv.Lck
{
	public static class LckMediator
	{
		public static event Action<ILckCamera> CameraRegistered;

		public static event Action<ILckCamera> CameraUnregistered;

		public static event Action<ILckMonitor> MonitorRegistered;

		public static event Action<ILckMonitor> MonitorUnregistered;

		public static event Action<string, string> MonitorToCameraAssignment;

		public static void RegisterCamera(ILckCamera camera)
		{
			if (!LckMediator._cameras.ContainsKey(camera.CameraId))
			{
				LckMediator._cameras.Add(camera.CameraId, camera);
				Action<ILckCamera> cameraRegistered = LckMediator.CameraRegistered;
				if (cameraRegistered == null)
				{
					return;
				}
				cameraRegistered(camera);
			}
		}

		public static void UnregisterCamera(ILckCamera camera)
		{
			if (LckMediator._cameras.ContainsKey(camera.CameraId))
			{
				LckMediator._cameras.Remove(camera.CameraId);
				Action<ILckCamera> cameraUnregistered = LckMediator.CameraUnregistered;
				if (cameraUnregistered == null)
				{
					return;
				}
				cameraUnregistered(camera);
			}
		}

		public static void RegisterMonitor(ILckMonitor monitor)
		{
			if (!LckMediator._monitors.ContainsKey(monitor.MonitorId))
			{
				LckMediator._monitors.Add(monitor.MonitorId, monitor);
				Action<ILckMonitor> monitorRegistered = LckMediator.MonitorRegistered;
				if (monitorRegistered == null)
				{
					return;
				}
				monitorRegistered(monitor);
			}
		}

		public static void UnregisterMonitor(ILckMonitor monitor)
		{
			if (LckMediator._monitors.ContainsKey(monitor.MonitorId))
			{
				LckMediator._monitors.Remove(monitor.MonitorId);
				Action<ILckMonitor> monitorUnregistered = LckMediator.MonitorUnregistered;
				if (monitorUnregistered == null)
				{
					return;
				}
				monitorUnregistered(monitor);
			}
		}

		public static ILckCamera GetCameraById(string id)
		{
			ILckCamera result;
			LckMediator._cameras.TryGetValue(id, out result);
			return result;
		}

		public static ILckMonitor GetMonitorById(string id)
		{
			ILckMonitor result;
			LckMediator._monitors.TryGetValue(id, out result);
			return result;
		}

		public static IEnumerable<ILckCamera> GetCameras()
		{
			return LckMediator._cameras.Values;
		}

		public static IEnumerable<ILckMonitor> GetMonitors()
		{
			return LckMediator._monitors.Values;
		}

		public static void NotifyMixerAboutMonitorForCamera(string monitorId, string cameraId)
		{
			Action<string, string> monitorToCameraAssignment = LckMediator.MonitorToCameraAssignment;
			if (monitorToCameraAssignment == null)
			{
				return;
			}
			monitorToCameraAssignment(monitorId, cameraId);
		}

		private static readonly Dictionary<string, ILckCamera> _cameras = new Dictionary<string, ILckCamera>();

		private static readonly Dictionary<string, ILckMonitor> _monitors = new Dictionary<string, ILckMonitor>();
	}
}
