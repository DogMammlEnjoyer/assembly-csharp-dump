using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GameStatsObject
	{
		[JsonConstructor]
		public GameStatsObject(long game_id, long mods_count_total, long mods_downloads_today, long mods_downloads_total, long mods_downloads_daily_average, long mods_subscribers_total, long date_expires)
		{
			this.GameId = game_id;
			this.ModsCountTotal = mods_count_total;
			this.ModsDownloadsToday = mods_downloads_today;
			this.ModsDownloadsTotal = mods_downloads_total;
			this.ModsDownloadsDailyAverage = mods_downloads_daily_average;
			this.ModsSubscribersTotal = mods_subscribers_total;
			this.DateExpires = date_expires;
		}

		internal readonly long GameId;

		internal readonly long ModsCountTotal;

		internal readonly long ModsDownloadsToday;

		internal readonly long ModsDownloadsTotal;

		internal readonly long ModsDownloadsDailyAverage;

		internal readonly long ModsSubscribersTotal;

		internal readonly long DateExpires;
	}
}
