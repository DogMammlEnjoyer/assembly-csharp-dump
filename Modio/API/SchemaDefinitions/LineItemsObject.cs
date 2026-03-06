using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct LineItemsObject
	{
		[JsonConstructor]
		public LineItemsObject(long game_id, long buyer_id, string game_name, string buyer_name, string token_name, long token_pack_id, string token_pack_name)
		{
			this.GameId = game_id;
			this.BuyerId = buyer_id;
			this.GameName = game_name;
			this.BuyerName = buyer_name;
			this.TokenName = token_name;
			this.TokenPackId = token_pack_id;
			this.TokenPackName = token_pack_name;
		}

		internal readonly long GameId;

		internal readonly long BuyerId;

		internal readonly string GameName;

		internal readonly string BuyerName;

		internal readonly string TokenName;

		internal readonly long TokenPackId;

		internal readonly string TokenPackName;
	}
}
