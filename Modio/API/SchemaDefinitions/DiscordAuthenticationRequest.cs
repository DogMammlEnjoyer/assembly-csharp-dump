using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct DiscordAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public DiscordAuthenticationRequest(string discord_token, bool terms_agreed, string email, long date_expires)
		{
			this.DiscordToken = discord_token;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			DiscordAuthenticationRequest._bodyParameters.Clear();
			DiscordAuthenticationRequest._bodyParameters.Add("discord_token", this.DiscordToken);
			DiscordAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			DiscordAuthenticationRequest._bodyParameters.Add("email", this.Email);
			DiscordAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return DiscordAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string DiscordToken;

		internal readonly bool TermsAgreed;

		internal readonly string Email;

		internal readonly long DateExpires;
	}
}
