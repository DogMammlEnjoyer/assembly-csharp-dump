using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public static class NetSync
	{
		public static void SetConnectionStatusChangedNotificationCallback(Message<NetSyncConnection>.Callback callback)
		{
			Callback.SetNotificationCallback<NetSyncConnection>(Message.MessageType.Notification_NetSync_ConnectionStatusChanged, callback);
		}

		public static void SetSessionsChangedNotificationCallback(Message<NetSyncSessionsChangedNotification>.Callback callback)
		{
			Callback.SetNotificationCallback<NetSyncSessionsChangedNotification>(Message.MessageType.Notification_NetSync_SessionsChanged, callback);
		}
	}
}
