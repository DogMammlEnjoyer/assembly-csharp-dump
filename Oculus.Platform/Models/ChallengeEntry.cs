using System;

namespace Oculus.Platform.Models
{
	public class ChallengeEntry
	{
		public ChallengeEntry(IntPtr o)
		{
			this.DisplayScore = CAPI.ovr_ChallengeEntry_GetDisplayScore(o);
			this.ExtraData = CAPI.ovr_ChallengeEntry_GetExtraData(o);
			this.ID = CAPI.ovr_ChallengeEntry_GetID(o);
			this.Rank = CAPI.ovr_ChallengeEntry_GetRank(o);
			this.Score = CAPI.ovr_ChallengeEntry_GetScore(o);
			this.Timestamp = CAPI.ovr_ChallengeEntry_GetTimestamp(o);
			this.User = new User(CAPI.ovr_ChallengeEntry_GetUser(o));
		}

		public readonly string DisplayScore;

		public readonly byte[] ExtraData;

		public readonly ulong ID;

		public readonly int Rank;

		public readonly long Score;

		public readonly DateTime Timestamp;

		public readonly User User;
	}
}
