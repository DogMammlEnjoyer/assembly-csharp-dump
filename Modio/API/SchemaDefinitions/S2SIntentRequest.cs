using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct S2SIntentRequest : IApiRequest
	{
		[JsonConstructor]
		public S2SIntentRequest(string sku, string portal, string gateway_uuid)
		{
			this.Sku = sku;
			this.Portal = portal;
			this.GatewayUuid = gateway_uuid;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			S2SIntentRequest._bodyParameters.Clear();
			S2SIntentRequest._bodyParameters.Add("sku", this.Sku);
			S2SIntentRequest._bodyParameters.Add("portal", this.Portal);
			S2SIntentRequest._bodyParameters.Add("gateway_uuid", this.GatewayUuid);
			return S2SIntentRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Sku;

		internal readonly string Portal;

		internal readonly string GatewayUuid;
	}
}
