using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct GameTokenPackObject
	{
		[JsonConstructor]
		public GameTokenPackObject(long id, long token_pack_id, long price, long amount, string portal, string sku, string name, string description, long date_added, long date_updated)
		{
			this.Id = id;
			this.TokenPackId = token_pack_id;
			this.Price = price;
			this.Amount = amount;
			this.Portal = portal;
			this.Sku = sku;
			this.Name = name;
			this.Description = description;
			this.DateAdded = date_added;
			this.DateUpdated = date_updated;
		}

		public readonly long Id;

		public readonly long TokenPackId;

		public readonly long Price;

		public readonly long Amount;

		public readonly string Portal;

		public readonly string Sku;

		public readonly string Name;

		public readonly string Description;

		public readonly long DateAdded;

		public readonly long DateUpdated;
	}
}
