using System;

namespace OVR.OpenVR
{
	public enum EVRNotificationError
	{
		OK,
		InvalidNotificationId = 100,
		NotificationQueueFull,
		InvalidOverlayHandle,
		SystemWithUserValueAlreadyExists
	}
}
