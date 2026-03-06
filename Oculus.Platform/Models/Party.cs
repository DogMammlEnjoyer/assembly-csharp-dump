using System;

namespace Oculus.Platform.Models
{
	public class Party
	{
		public Party(IntPtr o)
		{
			this.ID = CAPI.ovr_Party_GetID(o);
			IntPtr intPtr = CAPI.ovr_Party_GetInvitedUsers(o);
			this.InvitedUsers = new UserList(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.InvitedUsersOptional = null;
			}
			else
			{
				this.InvitedUsersOptional = this.InvitedUsers;
			}
			IntPtr intPtr2 = CAPI.ovr_Party_GetLeader(o);
			this.Leader = new User(intPtr2);
			if (intPtr2 == IntPtr.Zero)
			{
				this.LeaderOptional = null;
			}
			else
			{
				this.LeaderOptional = this.Leader;
			}
			IntPtr intPtr3 = CAPI.ovr_Party_GetUsers(o);
			this.Users = new UserList(intPtr3);
			if (intPtr3 == IntPtr.Zero)
			{
				this.UsersOptional = null;
				return;
			}
			this.UsersOptional = this.Users;
		}

		public readonly ulong ID;

		public readonly UserList InvitedUsersOptional;

		[Obsolete("Deprecated in favor of InvitedUsersOptional")]
		public readonly UserList InvitedUsers;

		public readonly User LeaderOptional;

		[Obsolete("Deprecated in favor of LeaderOptional")]
		public readonly User Leader;

		public readonly UserList UsersOptional;

		[Obsolete("Deprecated in favor of UsersOptional")]
		public readonly UserList Users;
	}
}
