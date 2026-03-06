using System;

namespace Oculus.Platform.Models
{
	public class Challenge
	{
		public Challenge(IntPtr o)
		{
			this.CreationType = CAPI.ovr_Challenge_GetCreationType(o);
			this.Description = CAPI.ovr_Challenge_GetDescription(o);
			this.EndDate = CAPI.ovr_Challenge_GetEndDate(o);
			this.ID = CAPI.ovr_Challenge_GetID(o);
			IntPtr intPtr = CAPI.ovr_Challenge_GetInvitedUsers(o);
			this.InvitedUsers = new UserList(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.InvitedUsersOptional = null;
			}
			else
			{
				this.InvitedUsersOptional = this.InvitedUsers;
			}
			this.Leaderboard = new Leaderboard(CAPI.ovr_Challenge_GetLeaderboard(o));
			IntPtr intPtr2 = CAPI.ovr_Challenge_GetParticipants(o);
			this.Participants = new UserList(intPtr2);
			if (intPtr2 == IntPtr.Zero)
			{
				this.ParticipantsOptional = null;
			}
			else
			{
				this.ParticipantsOptional = this.Participants;
			}
			this.StartDate = CAPI.ovr_Challenge_GetStartDate(o);
			this.Title = CAPI.ovr_Challenge_GetTitle(o);
			this.Visibility = CAPI.ovr_Challenge_GetVisibility(o);
		}

		public readonly ChallengeCreationType CreationType;

		public readonly string Description;

		public readonly DateTime EndDate;

		public readonly ulong ID;

		public readonly UserList InvitedUsersOptional;

		[Obsolete("Deprecated in favor of InvitedUsersOptional")]
		public readonly UserList InvitedUsers;

		public readonly Leaderboard Leaderboard;

		public readonly UserList ParticipantsOptional;

		[Obsolete("Deprecated in favor of ParticipantsOptional")]
		public readonly UserList Participants;

		public readonly DateTime StartDate;

		public readonly string Title;

		public readonly ChallengeVisibility Visibility;
	}
}
