using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Parties
	{
		public static Request<Party> GetCurrent()
		{
			if (Core.IsInitialized())
			{
				return new Request<Party>(CAPI.ovr_Party_GetCurrent());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static void SetPartyUpdateNotificationCallback(Message<PartyUpdateNotification>.Callback callback)
		{
			Callback.SetNotificationCallback<PartyUpdateNotification>(Message.MessageType.Notification_Party_PartyUpdate, callback);
		}
	}
}
