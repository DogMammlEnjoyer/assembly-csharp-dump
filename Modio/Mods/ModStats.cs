using System;
using Modio.API.SchemaDefinitions;

namespace Modio.Mods
{
	public class ModStats
	{
		public long Subscribers { get; private set; }

		public long Downloads { get; private set; }

		public long RatingsPositive { get; private set; }

		public long RatingsNegative { get; private set; }

		public long RatingsPercent { get; private set; }

		internal ModStats(ModStatsObject statsObject, ModRating previousRating)
		{
			this.Subscribers = statsObject.SubscribersTotal;
			this.Downloads = statsObject.DownloadsTotal;
			this.RatingsPositive = statsObject.RatingsPositive;
			this.RatingsNegative = statsObject.RatingsNegative;
			this.RatingsPercent = statsObject.RatingsPercentagePositive;
			this._previousRating = previousRating;
		}

		internal void UpdateEstimateFromLocalRatingChange(ModRating rating)
		{
			if (this._previousRating == ModRating.Negative)
			{
				long num = this.RatingsNegative;
				this.RatingsNegative = num - 1L;
			}
			if (this._previousRating == ModRating.Positive)
			{
				long num = this.RatingsPositive;
				this.RatingsPositive = num - 1L;
			}
			if (rating == ModRating.Negative)
			{
				long num = this.RatingsNegative;
				this.RatingsNegative = num + 1L;
			}
			if (rating == ModRating.Positive)
			{
				long num = this.RatingsPositive;
				this.RatingsPositive = num + 1L;
			}
			this._previousRating = rating;
			long num2 = this.RatingsPositive + this.RatingsNegative;
			this.RatingsPercent = ((num2 > 0L) ? (this.RatingsPositive * 100L / num2) : 100L);
		}

		internal void UpdatePreviousRating(ModRating rating)
		{
			this._previousRating = rating;
		}

		private ModRating _previousRating;
	}
}
