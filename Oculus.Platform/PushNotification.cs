using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class PushNotification
	{
		public static Request<PushNotificationResult> Register()
		{
			if (Core.IsInitialized())
			{
				return new Request<PushNotificationResult>(CAPI.ovr_PushNotification_Register());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}
	}
}
