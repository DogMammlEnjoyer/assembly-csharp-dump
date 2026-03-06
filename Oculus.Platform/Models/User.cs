using System;

namespace Oculus.Platform.Models
{
	public class User
	{
		public User(IntPtr o)
		{
			this.DisplayName = CAPI.ovr_User_GetDisplayName(o);
			this.ID = CAPI.ovr_User_GetID(o);
			this.ImageURL = CAPI.ovr_User_GetImageUrl(o);
			IntPtr intPtr = CAPI.ovr_User_GetManagedInfo(o);
			this.ManagedInfo = new ManagedInfo(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.ManagedInfoOptional = null;
			}
			else
			{
				this.ManagedInfoOptional = this.ManagedInfo;
			}
			this.OculusID = CAPI.ovr_User_GetOculusID(o);
			this.Presence = CAPI.ovr_User_GetPresence(o);
			this.PresenceDeeplinkMessage = CAPI.ovr_User_GetPresenceDeeplinkMessage(o);
			this.PresenceDestinationApiName = CAPI.ovr_User_GetPresenceDestinationApiName(o);
			this.PresenceLobbySessionId = CAPI.ovr_User_GetPresenceLobbySessionId(o);
			this.PresenceMatchSessionId = CAPI.ovr_User_GetPresenceMatchSessionId(o);
			this.PresenceStatus = CAPI.ovr_User_GetPresenceStatus(o);
			this.SmallImageUrl = CAPI.ovr_User_GetSmallImageUrl(o);
		}

		public readonly string DisplayName;

		public readonly ulong ID;

		public readonly string ImageURL;

		public readonly ManagedInfo ManagedInfoOptional;

		[Obsolete("Deprecated in favor of ManagedInfoOptional")]
		public readonly ManagedInfo ManagedInfo;

		public readonly string OculusID;

		public readonly string Presence;

		public readonly string PresenceDeeplinkMessage;

		public readonly string PresenceDestinationApiName;

		public readonly string PresenceLobbySessionId;

		public readonly string PresenceMatchSessionId;

		public readonly UserPresenceStatus PresenceStatus;

		public readonly string SmallImageUrl;
	}
}
