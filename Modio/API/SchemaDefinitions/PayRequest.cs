using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct PayRequest : IApiRequest
	{
		[JsonConstructor]
		public PayRequest(long display_amount, bool subscribe, string idempotent_key)
		{
			this.DisplayAmount = display_amount;
			this.Subscribe = subscribe;
			this.IdempotentKey = idempotent_key;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			PayRequest._bodyParameters.Clear();
			PayRequest._bodyParameters.Add("display_amount", this.DisplayAmount);
			PayRequest._bodyParameters.Add("subscribe", this.Subscribe);
			PayRequest._bodyParameters.Add("idempotent_key", this.IdempotentKey);
			return PayRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long DisplayAmount;

		internal readonly bool Subscribe;

		internal readonly string IdempotentKey;
	}
}
