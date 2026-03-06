using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct WalletBalanceObject
	{
		[JsonConstructor]
		public WalletBalanceObject(long balance)
		{
			this.Balance = balance;
		}

		internal readonly long Balance;
	}
}
