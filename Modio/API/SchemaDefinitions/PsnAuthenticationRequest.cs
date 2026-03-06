using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct PsnAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public PsnAuthenticationRequest(string auth_code, bool terms_agreed, [Nullable(2)] string email, int environment, long date_expires)
		{
			this.AuthCode = auth_code;
			this.TermsAgreed = terms_agreed;
			this.Email = email;
			this.Environment = environment;
			this.DateExpires = date_expires;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			PsnAuthenticationRequest._bodyParameters.Clear();
			PsnAuthenticationRequest._bodyParameters.Add("auth_code", this.AuthCode);
			PsnAuthenticationRequest._bodyParameters.Add("terms_agreed", this.TermsAgreed);
			if (!string.IsNullOrEmpty(this.Email))
			{
				PsnAuthenticationRequest._bodyParameters.Add("email", this.Email);
			}
			PsnAuthenticationRequest._bodyParameters.Add("env", this.Environment);
			PsnAuthenticationRequest._bodyParameters.Add("date_expires", this.DateExpires);
			return PsnAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		public readonly string AuthCode;

		public readonly bool TermsAgreed;

		[Nullable(2)]
		public readonly string Email;

		public readonly int Environment;

		internal readonly long DateExpires;
	}
}
