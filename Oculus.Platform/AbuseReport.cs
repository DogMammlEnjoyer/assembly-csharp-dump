using System;
using UnityEngine;

namespace Oculus.Platform
{
	public static class AbuseReport
	{
		public static Request ReportRequestHandled(ReportRequestResponse response)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_AbuseReport_ReportRequestHandled(response));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static void SetReportButtonPressedNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_AbuseReport_ReportButtonPressed, callback);
		}
	}
}
