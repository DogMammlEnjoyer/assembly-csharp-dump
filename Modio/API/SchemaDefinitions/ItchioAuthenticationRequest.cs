using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ItchioAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public ItchioAuthenticationRequest(string itchio_token, bool terms_agreed, string email, long date_expires)
		{
			this.ItchioToken = itchio_token;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			ItchioAuthenticationRequest._bodyParameters.Clear();
			ItchioAuthenticationRequest._bodyParameters.Add("itchio_token", this.ItchioToken);
			ItchioAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			ItchioAuthenticationRequest._bodyParameters.Add("email", this.Email);
			ItchioAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return ItchioAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string ItchioToken;

		internal readonly bool TermsAgreed;

		internal readonly string Email;

		internal readonly long DateExpires;
	}
}
