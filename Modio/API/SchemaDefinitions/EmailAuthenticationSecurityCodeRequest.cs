using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct EmailAuthenticationSecurityCodeRequest : IApiRequest
	{
		[JsonConstructor]
		public EmailAuthenticationSecurityCodeRequest(string security_code)
		{
			this.SecurityCode = security_code;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			EmailAuthenticationSecurityCodeRequest._bodyParameters.Clear();
			EmailAuthenticationSecurityCodeRequest._bodyParameters.Add("security_code", this.SecurityCode);
			return EmailAuthenticationSecurityCodeRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string SecurityCode;
	}
}
