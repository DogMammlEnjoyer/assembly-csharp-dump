using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class LeaderboardList : DeserializableList<Leaderboard>
	{
		public LeaderboardList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_LeaderboardArray_GetSize(a));
			this._Data = new List<Leaderboard>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new Leaderboard(CAPI.ovr_LeaderboardArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this._NextUrl = CAPI.ovr_LeaderboardArray_GetNextUrl(a);
		}
	}
}
