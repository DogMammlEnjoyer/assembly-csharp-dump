using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct WalletObject
	{
		[JsonConstructor]
		public WalletObject(string type, string payment_method_id, string game_id, string currency, long balance, long pending_balance, long deficit, long monetization_status)
		{
			this.Type = type;
			this.PaymentMethodId = payment_method_id;
			this.GameId = game_id;
			this.Currency = currency;
			this.Balance = balance;
			this.PendingBalance = pending_balance;
			this.Deficit = deficit;
			this.MonetizationStatus = monetization_status;
		}

		internal readonly string Type;

		internal readonly string PaymentMethodId;

		internal readonly string GameId;

		internal readonly string Currency;

		internal readonly long Balance;

		internal readonly long PendingBalance;

		internal readonly long Deficit;

		internal readonly long MonetizationStatus;
	}
}
