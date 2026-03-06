using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct SyncAppleEntitlementsRequest : IApiRequest
	{
		[JsonConstructor]
		public SyncAppleEntitlementsRequest(string receipt)
		{
			this.Receipt = receipt;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			SyncAppleEntitlementsRequest._bodyParameters.Clear();
			SyncAppleEntitlementsRequest._bodyParameters.Add("receipt", this.Receipt);
			return SyncAppleEntitlementsRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Receipt;
	}
}
