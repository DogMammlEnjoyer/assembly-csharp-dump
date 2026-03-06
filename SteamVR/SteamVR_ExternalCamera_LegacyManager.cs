using System;
using UnityEngine.Events;

namespace Valve.VR
{
	public class SteamVR_ExternalCamera_LegacyManager
	{
		public static bool hasCamera
		{
			get
			{
				return SteamVR_ExternalCamera_LegacyManager.cameraIndex != -1;
			}
		}

		public static void SubscribeToNewPoses()
		{
			if (SteamVR_ExternalCamera_LegacyManager.newPosesAction == null)
			{
				SteamVR_ExternalCamera_LegacyManager.newPosesAction = SteamVR_Events.NewPosesAction(new UnityAction<TrackedDevicePose_t[]>(SteamVR_ExternalCamera_LegacyManager.OnNewPoses));
			}
			SteamVR_ExternalCamera_LegacyManager.newPosesAction.enabled = true;
		}

		private static void OnNewPoses(TrackedDevicePose_t[] poses)
		{
			if (SteamVR_ExternalCamera_LegacyManager.cameraIndex != -1)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < poses.Length; i++)
			{
				if (poses[i].bDeviceIsConnected)
				{
					ETrackedDeviceClass trackedDeviceClass = OpenVR.System.GetTrackedDeviceClass((uint)i);
					if (trackedDeviceClass == ETrackedDeviceClass.Controller || trackedDeviceClass == ETrackedDeviceClass.GenericTracker)
					{
						num++;
						if (num >= 3)
						{
							SteamVR_ExternalCamera_LegacyManager.cameraIndex = i;
							return;
						}
					}
				}
			}
		}

		public static int cameraIndex = -1;

		private static SteamVR_Events.Action newPosesAction = null;
	}
}
