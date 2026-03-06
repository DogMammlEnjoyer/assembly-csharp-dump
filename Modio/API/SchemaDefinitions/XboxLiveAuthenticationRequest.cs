using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct XboxLiveAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public XboxLiveAuthenticationRequest(string xbox_token, bool terms_agreed, string email, long date_expires)
		{
			this.XboxToken = xbox_token;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			XboxLiveAuthenticationRequest._bodyParameters.Clear();
			XboxLiveAuthenticationRequest._bodyParameters.Add("xbox_token", this.XboxToken);
			XboxLiveAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			XboxLiveAuthenticationRequest._bodyParameters.Add("email", this.Email);
			XboxLiveAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return XboxLiveAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		public readonly string XboxToken;

		public readonly bool TermsAgreed;

		public readonly string Email;

		public readonly long DateExpires;
	}
}
