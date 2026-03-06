using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct RefundObject
	{
		[JsonConstructor]
		public RefundObject(long transaction_id, long gross_amount, long net_amount, long platform_fee, long gateway_fee, long tax, string tax_type, string transaction_type, JObject meta, long purchase_date)
		{
			this.TransactionId = transaction_id;
			this.GrossAmount = gross_amount;
			this.NetAmount = net_amount;
			this.PlatformFee = platform_fee;
			this.GatewayFee = gateway_fee;
			this.Tax = tax;
			this.TaxType = tax_type;
			this.TransactionType = transaction_type;
			this.Meta = meta;
			this.PurchaseDate = purchase_date;
		}

		internal readonly long TransactionId;

		internal readonly long GrossAmount;

		internal readonly long NetAmount;

		internal readonly long PlatformFee;

		internal readonly long GatewayFee;

		internal readonly long Tax;

		internal readonly string TaxType;

		internal readonly string TransactionType;

		internal readonly JObject Meta;

		internal readonly long PurchaseDate;
	}
}
