using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GooglePlayAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public GooglePlayAuthenticationRequest(string auth_code, bool terms_agreed, string email, long date_expires)
		{
			this.AuthCode = auth_code;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			GooglePlayAuthenticationRequest._bodyParameters.Clear();
			GooglePlayAuthenticationRequest._bodyParameters.Add("auth_code", this.AuthCode);
			GooglePlayAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			GooglePlayAuthenticationRequest._bodyParameters.Add("email", this.Email);
			GooglePlayAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return GooglePlayAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string AuthCode;

		internal readonly bool TermsAgreed;

		internal readonly string Email;

		internal readonly long DateExpires;
	}
}
