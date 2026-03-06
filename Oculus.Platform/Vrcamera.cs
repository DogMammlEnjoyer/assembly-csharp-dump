using System;

namespace Oculus.Platform
{
	public static class Vrcamera
	{
		public static void SetGetDataChannelMessageUpdateNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_Vrcamera_GetDataChannelMessageUpdate, callback);
		}

		public static void SetGetSurfaceUpdateNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_Vrcamera_GetSurfaceUpdate, callback);
		}
	}
}
