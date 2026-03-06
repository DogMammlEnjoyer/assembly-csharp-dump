using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Leaderboards
	{
		public static Request<LeaderboardEntryList> GetNextEntries(LeaderboardEntryList list)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 1310751961));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<LeaderboardEntryList> GetPreviousEntries(LeaderboardEntryList list)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardEntryList>(CAPI.ovr_HTTP_GetWithMessageType(list.PreviousUrl, 1224858304));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<LeaderboardList> Get(string leaderboardName)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardList>(CAPI.ovr_Leaderboard_Get(leaderboardName));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<LeaderboardEntryList> GetEntries(string leaderboardName, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntries(leaderboardName, limit, filter, startAt));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<LeaderboardEntryList> GetEntriesAfterRank(string leaderboardName, int limit, ulong afterRank)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntriesAfterRank(leaderboardName, limit, afterRank));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<LeaderboardEntryList> GetEntriesByIds(string leaderboardName, int limit, LeaderboardStartAt startAt, ulong[] userIDs)
		{
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardEntryList>(CAPI.ovr_Leaderboard_GetEntriesByIds(leaderboardName, limit, startAt, userIDs, (uint)((userIDs != null) ? userIDs.Length : 0)));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<bool> WriteEntry(string leaderboardName, long score, byte[] extraData = null, bool forceUpdate = false)
		{
			if (Core.IsInitialized())
			{
				return new Request<bool>(CAPI.ovr_Leaderboard_WriteEntry(leaderboardName, score, extraData, (uint)((extraData != null) ? extraData.Length : 0), forceUpdate));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<bool> WriteEntryWithSupplementaryMetric(string leaderboardName, long score, long supplementaryMetric, byte[] extraData = null, bool forceUpdate = false)
		{
			if (Core.IsInitialized())
			{
				return new Request<bool>(CAPI.ovr_Leaderboard_WriteEntryWithSupplementaryMetric(leaderboardName, score, supplementaryMetric, extraData, (uint)((extraData != null) ? extraData.Length : 0), forceUpdate));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<LeaderboardList> GetNextLeaderboardListPage(LeaderboardList list)
		{
			if (!list.HasNextPage)
			{
				Debug.LogWarning("Oculus.Platform.GetNextLeaderboardListPage: List has no next page");
				return null;
			}
			if (Core.IsInitialized())
			{
				return new Request<LeaderboardList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 905344667));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}
	}
}
