using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct S2SPayObject
	{
		[JsonConstructor]
		public S2SPayObject(long transaction_id, string gateway_uuid, long gross_amount, long net_amount, long platform_fee, long gateway_fee, string transaction_type, JObject meta, long purchase_date)
		{
			this.TransactionId = transaction_id;
			this.GatewayUuid = gateway_uuid;
			this.GrossAmount = gross_amount;
			this.NetAmount = net_amount;
			this.PlatformFee = platform_fee;
			this.GatewayFee = gateway_fee;
			this.TransactionType = transaction_type;
			this.Meta = meta;
			this.PurchaseDate = purchase_date;
		}

		internal readonly long TransactionId;

		internal readonly string GatewayUuid;

		internal readonly long GrossAmount;

		internal readonly long NetAmount;

		internal readonly long PlatformFee;

		internal readonly long GatewayFee;

		internal readonly string TransactionType;

		internal readonly JObject Meta;

		internal readonly long PurchaseDate;
	}
}
