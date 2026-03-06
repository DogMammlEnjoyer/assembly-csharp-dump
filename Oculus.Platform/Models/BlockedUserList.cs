using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class BlockedUserList : DeserializableList<BlockedUser>
	{
		public BlockedUserList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_BlockedUserArray_GetSize(a));
			this._Data = new List<BlockedUser>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new BlockedUser(CAPI.ovr_BlockedUserArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this._NextUrl = CAPI.ovr_BlockedUserArray_GetNextUrl(a);
		}
	}
}
