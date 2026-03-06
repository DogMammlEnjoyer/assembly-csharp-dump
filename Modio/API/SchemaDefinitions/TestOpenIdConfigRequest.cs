using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct TestOpenIdConfigRequest : IApiRequest
	{
		[JsonConstructor]
		public TestOpenIdConfigRequest(string jwk_url, string id_token)
		{
			this.JwkUrl = jwk_url;
			this.IdToken = id_token;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			TestOpenIdConfigRequest._bodyParameters.Clear();
			TestOpenIdConfigRequest._bodyParameters.Add("jwk_url", this.JwkUrl);
			TestOpenIdConfigRequest._bodyParameters.Add("id_token", this.IdToken);
			return TestOpenIdConfigRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string JwkUrl;

		internal readonly string IdToken;
	}
}
