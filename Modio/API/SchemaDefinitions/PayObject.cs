using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct PayObject
	{
		[JsonConstructor]
		public PayObject(long transaction_id, string gateway_uuid, long gross_amount, long net_amount, long platform_fee, long gateway_fee, string transaction_type, JArray meta, long purchase_date, string wallet_type, long balance, long deficit, string payment_method_id, ModObject mod)
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
			this.WalletType = wallet_type;
			this.Balance = balance;
			this.Deficit = deficit;
			this.PaymentMethodId = payment_method_id;
			this.Mod = mod;
		}

		internal readonly long TransactionId;

		internal readonly string GatewayUuid;

		internal readonly long GrossAmount;

		internal readonly long NetAmount;

		internal readonly long PlatformFee;

		internal readonly long GatewayFee;

		internal readonly string TransactionType;

		internal readonly JArray Meta;

		internal readonly long PurchaseDate;

		internal readonly string WalletType;

		internal readonly long Balance;

		internal readonly long Deficit;

		internal readonly string PaymentMethodId;

		internal readonly ModObject Mod;
	}
}
