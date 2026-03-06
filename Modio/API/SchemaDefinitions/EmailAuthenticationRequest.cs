using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct EmailAuthenticationRequest : IApiRequest
	{
		[JsonConstructor]
		public EmailAuthenticationRequest(string email)
		{
			this.Email = email;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			EmailAuthenticationRequest._bodyParameters.Clear();
			EmailAuthenticationRequest._bodyParameters.Add("email", this.Email);
			return EmailAuthenticationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Email;
	}
}
