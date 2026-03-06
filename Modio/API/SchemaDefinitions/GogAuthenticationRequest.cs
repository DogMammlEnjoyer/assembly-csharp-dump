using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct GogAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public GogAuthenticationRequest(string appdata, bool terms_agreed, string email, long date_expires)
		{
			this.Appdata = appdata;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			GogAuthenticationRequest._bodyParameters.Clear();
			GogAuthenticationRequest._bodyParameters.Add("appdata", this.Appdata);
			GogAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			GogAuthenticationRequest._bodyParameters.Add("email", this.Email);
			GogAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return GogAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		public readonly string Appdata;

		public readonly bool TermsAgreed;

		public readonly string Email;

		public readonly long DateExpires;
	}
}
