using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct SwitchAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public SwitchAuthenticationRequest(string id_token, bool terms_agreed, string email, long date_expires)
		{
			this.IdToken = id_token;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			SwitchAuthenticationRequest._bodyParameters.Clear();
			SwitchAuthenticationRequest._bodyParameters.Add("id_token", this.IdToken);
			SwitchAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			SwitchAuthenticationRequest._bodyParameters.Add("email", this.Email);
			SwitchAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return SwitchAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string IdToken;

		internal readonly bool TermsAgreed;

		internal readonly string Email;

		internal readonly long DateExpires;
	}
}
