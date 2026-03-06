using System;

namespace Oculus.Platform.Models
{
	public class ApplicationInvite
	{
		public ApplicationInvite(IntPtr o)
		{
			IntPtr intPtr = CAPI.ovr_ApplicationInvite_GetDestination(o);
			this.Destination = new Destination(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.DestinationOptional = null;
			}
			else
			{
				this.DestinationOptional = this.Destination;
			}
			this.ID = CAPI.ovr_ApplicationInvite_GetID(o);
			this.IsActive = CAPI.ovr_ApplicationInvite_GetIsActive(o);
			this.LobbySessionId = CAPI.ovr_ApplicationInvite_GetLobbySessionId(o);
			this.MatchSessionId = CAPI.ovr_ApplicationInvite_GetMatchSessionId(o);
			IntPtr intPtr2 = CAPI.ovr_ApplicationInvite_GetRecipient(o);
			this.Recipient = new User(intPtr2);
			if (intPtr2 == IntPtr.Zero)
			{
				this.RecipientOptional = null;
				return;
			}
			this.RecipientOptional = this.Recipient;
		}

		public readonly Destination DestinationOptional;

		[Obsolete("Deprecated in favor of DestinationOptional")]
		public readonly Destination Destination;

		public readonly ulong ID;

		public readonly bool IsActive;

		public readonly string LobbySessionId;

		public readonly string MatchSessionId;

		public readonly User RecipientOptional;

		[Obsolete("Deprecated in favor of RecipientOptional")]
		public readonly User Recipient;
	}
}
