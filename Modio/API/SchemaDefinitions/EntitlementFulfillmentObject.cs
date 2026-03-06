using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct EntitlementFulfillmentObject
	{
		[JsonConstructor]
		public EntitlementFulfillmentObject(string transaction_id, long transaction_state, string sku_id, bool entitlement_consumed, long entitlement_type, EntitlementDetailsObject details)
		{
			this.TransactionId = transaction_id;
			this.TransactionState = transaction_state;
			this.SkuId = sku_id;
			this.EntitlementConsumed = entitlement_consumed;
			this.EntitlementType = entitlement_type;
			this.Details = details;
		}

		internal readonly string TransactionId;

		internal readonly long TransactionState;

		internal readonly string SkuId;

		internal readonly bool EntitlementConsumed;

		internal readonly long EntitlementType;

		internal readonly EntitlementDetailsObject Details;
	}
}
