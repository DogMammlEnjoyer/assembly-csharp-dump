using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public static class CameraCaptureBridge
	{
		public static bool enabled
		{
			get
			{
				return CameraCaptureBridge._enabled;
			}
			set
			{
				CameraCaptureBridge._enabled = value;
			}
		}

		public static IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> GetCaptureActions(Camera camera)
		{
			CameraCaptureBridge.CameraEntry cameraEntry;
			if (!CameraCaptureBridge.actionDict.TryGetValue(camera, out cameraEntry) || cameraEntry.actions.Count == 0)
			{
				return null;
			}
			return cameraEntry.actions.GetEnumerator();
		}

		internal static IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> GetCachedCaptureActionsEnumerator(Camera camera)
		{
			CameraCaptureBridge.CameraEntry cameraEntry;
			if (!CameraCaptureBridge.actionDict.TryGetValue(camera, out cameraEntry) || cameraEntry.actions.Count == 0)
			{
				return null;
			}
			cameraEntry.cachedEnumerator.Reset();
			return cameraEntry.cachedEnumerator;
		}

		public static void AddCaptureAction(Camera camera, Action<RenderTargetIdentifier, CommandBuffer> action)
		{
			CameraCaptureBridge.CameraEntry cameraEntry;
			CameraCaptureBridge.actionDict.TryGetValue(camera, out cameraEntry);
			if (cameraEntry == null)
			{
				cameraEntry = new CameraCaptureBridge.CameraEntry
				{
					actions = new HashSet<Action<RenderTargetIdentifier, CommandBuffer>>()
				};
				CameraCaptureBridge.actionDict.Add(camera, cameraEntry);
			}
			cameraEntry.actions.Add(action);
			cameraEntry.cachedEnumerator = cameraEntry.actions.GetEnumerator();
		}

		public static void RemoveCaptureAction(Camera camera, Action<RenderTargetIdentifier, CommandBuffer> action)
		{
			if (camera == null)
			{
				return;
			}
			CameraCaptureBridge.CameraEntry cameraEntry;
			if (CameraCaptureBridge.actionDict.TryGetValue(camera, out cameraEntry))
			{
				cameraEntry.actions.Remove(action);
				cameraEntry.cachedEnumerator = cameraEntry.actions.GetEnumerator();
			}
		}

		private static Dictionary<Camera, CameraCaptureBridge.CameraEntry> actionDict = new Dictionary<Camera, CameraCaptureBridge.CameraEntry>();

		private static bool _enabled;

		private class CameraEntry
		{
			internal HashSet<Action<RenderTargetIdentifier, CommandBuffer>> actions;

			internal IEnumerator<Action<RenderTargetIdentifier, CommandBuffer>> cachedEnumerator;
		}
	}
}
