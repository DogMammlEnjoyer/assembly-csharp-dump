using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct FacebookAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public FacebookAuthenticationRequest(string facebook_token, bool terms_agreed, string email, long date_expires)
		{
			this.FacebookToken = facebook_token;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			FacebookAuthenticationRequest._bodyParameters.Clear();
			FacebookAuthenticationRequest._bodyParameters.Add("facebook_token", this.FacebookToken);
			FacebookAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			FacebookAuthenticationRequest._bodyParameters.Add("email", this.Email);
			FacebookAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return FacebookAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string FacebookToken;

		internal readonly bool TermsAgreed;

		internal readonly string Email;

		internal readonly long DateExpires;
	}
}
