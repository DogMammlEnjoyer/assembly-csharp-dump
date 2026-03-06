using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct RatingObject
	{
		[JsonConstructor]
		public RatingObject(long game_id, long mod_id, long rating, long date_added)
		{
			this.GameId = game_id;
			this.ModId = mod_id;
			this.Rating = rating;
			this.DateAdded = date_added;
		}

		internal readonly long GameId;

		internal readonly long ModId;

		internal readonly long Rating;

		internal readonly long DateAdded;
	}
}
