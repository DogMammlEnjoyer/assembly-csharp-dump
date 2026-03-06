using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GetTransactions
	{
		[JsonConstructor]
		public GetTransactions(TransactionObject[] data, PaginationObject download)
		{
			this.Data = data;
			this.Download = download;
		}

		internal readonly TransactionObject[] Data;

		internal readonly PaginationObject Download;
	}
}
