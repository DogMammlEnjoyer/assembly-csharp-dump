using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct EpicGamesAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public EpicGamesAuthenticationRequest(string access_token, bool terms_agreed, string email, long date_expires)
		{
			this.AccessToken = access_token;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			EpicGamesAuthenticationRequest._bodyParameters.Clear();
			EpicGamesAuthenticationRequest._bodyParameters.Add("access_token", this.AccessToken);
			EpicGamesAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			EpicGamesAuthenticationRequest._bodyParameters.Add("email", this.Email);
			EpicGamesAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return EpicGamesAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string AccessToken;

		internal readonly bool TermsAgreed;

		internal readonly string Email;

		internal readonly long DateExpires;
	}
}
