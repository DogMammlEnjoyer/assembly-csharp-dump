using System;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Notifications
	{
		public static Request MarkAsRead(ulong notificationID)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Notification_MarkAsRead(notificationID));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}
	}
}
