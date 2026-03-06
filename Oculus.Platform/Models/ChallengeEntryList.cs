using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class ChallengeEntryList : DeserializableList<ChallengeEntry>
	{
		public ChallengeEntryList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_ChallengeEntryArray_GetSize(a));
			this._Data = new List<ChallengeEntry>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new ChallengeEntry(CAPI.ovr_ChallengeEntryArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this.TotalCount = CAPI.ovr_ChallengeEntryArray_GetTotalCount(a);
			this._PreviousUrl = CAPI.ovr_ChallengeEntryArray_GetPreviousUrl(a);
			this._NextUrl = CAPI.ovr_ChallengeEntryArray_GetNextUrl(a);
		}

		public readonly ulong TotalCount;
	}
}
