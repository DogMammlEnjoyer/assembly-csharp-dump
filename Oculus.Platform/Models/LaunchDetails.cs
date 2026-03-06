using System;

namespace Oculus.Platform.Models
{
	public class LaunchDetails
	{
		public LaunchDetails(IntPtr o)
		{
			this.DeeplinkMessage = CAPI.ovr_LaunchDetails_GetDeeplinkMessage(o);
			this.DestinationApiName = CAPI.ovr_LaunchDetails_GetDestinationApiName(o);
			this.LaunchSource = CAPI.ovr_LaunchDetails_GetLaunchSource(o);
			this.LaunchType = CAPI.ovr_LaunchDetails_GetLaunchType(o);
			this.LobbySessionID = CAPI.ovr_LaunchDetails_GetLobbySessionID(o);
			this.MatchSessionID = CAPI.ovr_LaunchDetails_GetMatchSessionID(o);
			this.TrackingID = CAPI.ovr_LaunchDetails_GetTrackingID(o);
			IntPtr intPtr = CAPI.ovr_LaunchDetails_GetUsers(o);
			this.Users = new UserList(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.UsersOptional = null;
				return;
			}
			this.UsersOptional = this.Users;
		}

		public readonly string DeeplinkMessage;

		public readonly string DestinationApiName;

		public readonly string LaunchSource;

		public readonly LaunchType LaunchType;

		public readonly string LobbySessionID;

		public readonly string MatchSessionID;

		public readonly string TrackingID;

		public readonly UserList UsersOptional;

		[Obsolete("Deprecated in favor of UsersOptional")]
		public readonly UserList Users;
	}
}
