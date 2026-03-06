using System;

namespace Oculus.Platform.Models
{
	public class LeaderboardEntry
	{
		public LeaderboardEntry(IntPtr o)
		{
			this.DisplayScore = CAPI.ovr_LeaderboardEntry_GetDisplayScore(o);
			this.ExtraData = CAPI.ovr_LeaderboardEntry_GetExtraData(o);
			this.ID = CAPI.ovr_LeaderboardEntry_GetID(o);
			this.Rank = CAPI.ovr_LeaderboardEntry_GetRank(o);
			this.Score = CAPI.ovr_LeaderboardEntry_GetScore(o);
			IntPtr intPtr = CAPI.ovr_LeaderboardEntry_GetSupplementaryMetric(o);
			this.SupplementaryMetric = new SupplementaryMetric(intPtr);
			if (intPtr == IntPtr.Zero)
			{
				this.SupplementaryMetricOptional = null;
			}
			else
			{
				this.SupplementaryMetricOptional = this.SupplementaryMetric;
			}
			this.Timestamp = CAPI.ovr_LeaderboardEntry_GetTimestamp(o);
			this.User = new User(CAPI.ovr_LeaderboardEntry_GetUser(o));
		}

		public readonly string DisplayScore;

		public readonly byte[] ExtraData;

		public readonly ulong ID;

		public readonly int Rank;

		public readonly long Score;

		public readonly SupplementaryMetric SupplementaryMetricOptional;

		[Obsolete("Deprecated in favor of SupplementaryMetricOptional")]
		public readonly SupplementaryMetric SupplementaryMetric;

		public readonly DateTime Timestamp;

		public readonly User User;
	}
}
