using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ContactRequest : IApiRequest
	{
		[JsonConstructor]
		public ContactRequest(string email, string subject, string message)
		{
			this.Email = email;
			this.Subject = subject;
			this.Message = message;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			ContactRequest._bodyParameters.Clear();
			ContactRequest._bodyParameters.Add("email", this.Email);
			ContactRequest._bodyParameters.Add("subject", this.Subject);
			ContactRequest._bodyParameters.Add("message", this.Message);
			return ContactRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Email;

		internal readonly string Subject;

		internal readonly string Message;
	}
}
