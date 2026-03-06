using System;

namespace Oculus.Platform.Models
{
	public class PartyUpdateNotification
	{
		public PartyUpdateNotification(IntPtr o)
		{
			this.Action = CAPI.ovr_PartyUpdateNotification_GetAction(o);
			this.PartyId = CAPI.ovr_PartyUpdateNotification_GetPartyId(o);
			this.SenderId = CAPI.ovr_PartyUpdateNotification_GetSenderId(o);
			this.UpdateTimestamp = CAPI.ovr_PartyUpdateNotification_GetUpdateTimestamp(o);
			this.UserAlias = CAPI.ovr_PartyUpdateNotification_GetUserAlias(o);
			this.UserId = CAPI.ovr_PartyUpdateNotification_GetUserId(o);
			this.UserName = CAPI.ovr_PartyUpdateNotification_GetUserName(o);
		}

		public readonly PartyUpdateAction Action;

		public readonly ulong PartyId;

		public readonly ulong SenderId;

		public readonly string UpdateTimestamp;

		public readonly string UserAlias;

		public readonly ulong UserId;

		public readonly string UserName;
	}
}
