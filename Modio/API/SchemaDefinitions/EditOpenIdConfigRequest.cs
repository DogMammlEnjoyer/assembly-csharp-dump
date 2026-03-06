using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct EditOpenIdConfigRequest : IApiRequest
	{
		[JsonConstructor]
		public EditOpenIdConfigRequest(string jwk_url, string aud, string display_name_claim, string avatar_url_claim)
		{
			this.JwkUrl = jwk_url;
			this.Aud = aud;
			this.DisplayNameClaim = display_name_claim;
			this.AvatarUrlClaim = avatar_url_claim;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			EditOpenIdConfigRequest._bodyParameters.Clear();
			EditOpenIdConfigRequest._bodyParameters.Add("jwk_url", this.JwkUrl);
			EditOpenIdConfigRequest._bodyParameters.Add("aud", this.Aud);
			EditOpenIdConfigRequest._bodyParameters.Add("display_name_claim", this.DisplayNameClaim);
			EditOpenIdConfigRequest._bodyParameters.Add("avatar_url_claim", this.AvatarUrlClaim);
			return EditOpenIdConfigRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string JwkUrl;

		internal readonly string Aud;

		internal readonly string DisplayNameClaim;

		internal readonly string AvatarUrlClaim;
	}
}
