using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class LeaderboardEntryList : DeserializableList<LeaderboardEntry>
	{
		public LeaderboardEntryList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_LeaderboardEntryArray_GetSize(a));
			this._Data = new List<LeaderboardEntry>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new LeaderboardEntry(CAPI.ovr_LeaderboardEntryArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this.TotalCount = CAPI.ovr_LeaderboardEntryArray_GetTotalCount(a);
			this._PreviousUrl = CAPI.ovr_LeaderboardEntryArray_GetPreviousUrl(a);
			this._NextUrl = CAPI.ovr_LeaderboardEntryArray_GetNextUrl(a);
		}

		public readonly ulong TotalCount;
	}
}
