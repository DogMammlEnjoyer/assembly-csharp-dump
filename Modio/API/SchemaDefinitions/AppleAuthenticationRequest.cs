using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AppleAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public AppleAuthenticationRequest(string id_token, bool terms_agreed, long date_expires)
		{
			this.IdToken = id_token;
			this.TermsAgreed = terms_agreed;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AppleAuthenticationRequest._bodyParameters.Clear();
			AppleAuthenticationRequest._bodyParameters.Add("id_token", this.IdToken);
			AppleAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			AppleAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return AppleAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string IdToken;

		internal readonly bool TermsAgreed;

		internal readonly long DateExpires;
	}
}
