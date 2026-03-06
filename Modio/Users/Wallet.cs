using System;
using Modio.API.SchemaDefinitions;

namespace Modio.Users
{
	public class Wallet
	{
		public string Type { get; private set; }

		public string Currency { get; private set; }

		public long Balance { get; private set; }

		internal Wallet()
		{
		}

		internal void ApplyDetailsFromWalletObject(WalletObject walletObject)
		{
			this.Type = walletObject.Type;
			this.Currency = walletObject.Currency;
			this.Balance = walletObject.Balance;
		}

		internal void UpdateBalance(long newBalance)
		{
			this.Balance = newBalance;
		}
	}
}
