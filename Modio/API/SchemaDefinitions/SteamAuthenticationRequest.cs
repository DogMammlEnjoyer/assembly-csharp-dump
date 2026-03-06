using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct SteamAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public SteamAuthenticationRequest(string appdata, bool termsAgreed, [Nullable(2)] string email, long dateExpires)
		{
			this.Appdata = appdata;
			this.TermsAgreed = termsAgreed;
			this.Email = email;
			this.DateExpires = dateExpires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			SteamAuthenticationRequest._bodyParameters.Clear();
			SteamAuthenticationRequest._bodyParameters.Add("appdata", this.Appdata);
			SteamAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			if (!string.IsNullOrEmpty(this.Email))
			{
				SteamAuthenticationRequest._bodyParameters.Add("email", this.Email);
			}
			SteamAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return SteamAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Appdata;

		internal readonly bool TermsAgreed;

		[Nullable(2)]
		internal readonly string Email;

		internal readonly long DateExpires;
	}
}
