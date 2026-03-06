using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct PaymentMethodObject
	{
		[JsonConstructor]
		public PaymentMethodObject(string name, string id, long amount, string display_amount)
		{
			this.Name = name;
			this.Id = id;
			this.Amount = amount;
			this.DisplayAmount = display_amount;
		}

		internal readonly string Name;

		internal readonly string Id;

		internal readonly long Amount;

		internal readonly string DisplayAmount;
	}
}
