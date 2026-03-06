using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class ChallengeList : DeserializableList<Challenge>
	{
		public ChallengeList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_ChallengeArray_GetSize(a));
			this._Data = new List<Challenge>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new Challenge(CAPI.ovr_ChallengeArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this.TotalCount = CAPI.ovr_ChallengeArray_GetTotalCount(a);
			this._PreviousUrl = CAPI.ovr_ChallengeArray_GetPreviousUrl(a);
			this._NextUrl = CAPI.ovr_ChallengeArray_GetNextUrl(a);
		}

		public readonly ulong TotalCount;
	}
}
