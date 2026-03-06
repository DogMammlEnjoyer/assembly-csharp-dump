using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct TransactionObject
	{
		[JsonConstructor]
		public TransactionObject(long id, string gateway_uuid, string gateway_name, long account_id, long gross_amount, long net_amount, long platform_fee, long gateway_fee, long tax, string tax_type, string currency, long tokens, string transaction_type, string monetization_type, string purchase_date, string created_at, PaymentMethodObject[] payment_method, LineItemsObject[] line_items)
		{
			this.Id = id;
			this.GatewayUuid = gateway_uuid;
			this.GatewayName = gateway_name;
			this.AccountId = account_id;
			this.GrossAmount = gross_amount;
			this.NetAmount = net_amount;
			this.PlatformFee = platform_fee;
			this.GatewayFee = gateway_fee;
			this.Tax = tax;
			this.TaxType = tax_type;
			this.Currency = currency;
			this.Tokens = tokens;
			this.TransactionType = transaction_type;
			this.MonetizationType = monetization_type;
			this.PurchaseDate = purchase_date;
			this.CreatedAt = created_at;
			this.PaymentMethod = payment_method;
			this.LineItems = line_items;
		}

		internal readonly long Id;

		internal readonly string GatewayUuid;

		internal readonly string GatewayName;

		internal readonly long AccountId;

		internal readonly long GrossAmount;

		internal readonly long NetAmount;

		internal readonly long PlatformFee;

		internal readonly long GatewayFee;

		internal readonly long Tax;

		internal readonly string TaxType;

		internal readonly string Currency;

		internal readonly long Tokens;

		internal readonly string TransactionType;

		internal readonly string MonetizationType;

		internal readonly string PurchaseDate;

		internal readonly string CreatedAt;

		internal readonly PaymentMethodObject[] PaymentMethod;

		internal readonly LineItemsObject[] LineItems;
	}
}
