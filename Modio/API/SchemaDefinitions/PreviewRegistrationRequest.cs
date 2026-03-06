using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct PreviewRegistrationRequest : IApiRequest
	{
		[JsonConstructor]
		public PreviewRegistrationRequest(string hash)
		{
			this.Hash = hash;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			PreviewRegistrationRequest._bodyParameters.Clear();
			PreviewRegistrationRequest._bodyParameters.Add("hash", this.Hash);
			return PreviewRegistrationRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string Hash;
	}
}
