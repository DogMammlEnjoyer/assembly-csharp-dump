using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public static class Livestreaming
	{
		public static void SetStatusUpdateNotificationCallback(Message<LivestreamingStatus>.Callback callback)
		{
			Callback.SetNotificationCallback<LivestreamingStatus>(Message.MessageType.Notification_Livestreaming_StatusChange, callback);
		}
	}
}
