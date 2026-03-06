using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GuideStatsObject
	{
		[JsonConstructor]
		public GuideStatsObject(long guide_id, long visits_today, long visits_total, long comments_total)
		{
			this.GuideId = guide_id;
			this.VisitsToday = visits_today;
			this.VisitsTotal = visits_total;
			this.CommentsTotal = comments_total;
		}

		internal readonly long GuideId;

		internal readonly long VisitsToday;

		internal readonly long VisitsTotal;

		internal readonly long CommentsTotal;
	}
}
