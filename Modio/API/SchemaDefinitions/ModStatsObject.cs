using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct ModStatsObject
	{
		[JsonConstructor]
		public ModStatsObject(long mod_id, long popularity_rank_position, long popularity_rank_total_mods, long downloads_today, long downloads_total, long subscribers_total, long ratings_total, long ratings_positive, long ratings_negative, long ratings_percentage_positive, float ratings_weighted_aggregate, string ratings_display_text, long date_expires)
		{
			this.ModId = mod_id;
			this.PopularityRankPosition = popularity_rank_position;
			this.PopularityRankTotalMods = popularity_rank_total_mods;
			this.DownloadsToday = downloads_today;
			this.DownloadsTotal = downloads_total;
			this.SubscribersTotal = subscribers_total;
			this.RatingsTotal = ratings_total;
			this.RatingsPositive = ratings_positive;
			this.RatingsNegative = ratings_negative;
			this.RatingsPercentagePositive = ratings_percentage_positive;
			this.RatingsWeightedAggregate = ratings_weighted_aggregate;
			this.RatingsDisplayText = ratings_display_text;
			this.DateExpires = date_expires;
		}

		internal readonly long ModId;

		internal readonly long PopularityRankPosition;

		internal readonly long PopularityRankTotalMods;

		internal readonly long DownloadsToday;

		internal readonly long DownloadsTotal;

		internal readonly long SubscribersTotal;

		internal readonly long RatingsTotal;

		internal readonly long RatingsPositive;

		internal readonly long RatingsNegative;

		internal readonly long RatingsPercentagePositive;

		internal readonly float RatingsWeightedAggregate;

		internal readonly string RatingsDisplayText;

		internal readonly long DateExpires;
	}
}
