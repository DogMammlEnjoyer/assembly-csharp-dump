using System;

namespace Oculus.Platform.Models
{
	public class BlockedUser
	{
		public BlockedUser(IntPtr o)
		{
			this.Id = CAPI.ovr_BlockedUser_GetId(o);
		}

		public readonly ulong Id;
	}
}
