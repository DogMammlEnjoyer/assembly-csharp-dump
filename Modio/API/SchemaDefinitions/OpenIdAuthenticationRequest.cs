using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct OpenIdAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public OpenIdAuthenticationRequest(string id_token, bool terms_agreed, string email, long date_expires, bool monetization_account)
		{
			this.IdToken = id_token;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.DateExpires = date_expires;
			this.MonetizationAccount = monetization_account;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			OpenIdAuthenticationRequest._bodyParameters.Clear();
			OpenIdAuthenticationRequest._bodyParameters.Add("id_token", this.IdToken);
			OpenIdAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			OpenIdAuthenticationRequest._bodyParameters.Add("email", this.Email);
			OpenIdAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			OpenIdAuthenticationRequest._bodyParameters.Add("monetization_account", this.MonetizationAccount);
			return OpenIdAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string IdToken;

		internal readonly bool TermsAgreed;

		internal readonly string Email;

		internal readonly long DateExpires;

		internal readonly bool MonetizationAccount;
	}
}
