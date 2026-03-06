using System;
using Oculus.Platform.Models;

namespace Oculus.Platform
{
	public static class ApplicationLifecycle
	{
		public static LaunchDetails GetLaunchDetails()
		{
			return new LaunchDetails(CAPI.ovr_ApplicationLifecycle_GetLaunchDetails());
		}

		public static void LogDeeplinkResult(string trackingID, LaunchResult result)
		{
			CAPI.ovr_ApplicationLifecycle_LogDeeplinkResult(trackingID, result);
		}

		public static void SetLaunchIntentChangedNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_ApplicationLifecycle_LaunchIntentChanged, callback);
		}
	}
}
