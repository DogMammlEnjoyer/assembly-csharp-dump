using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddGameMediaRequest : IApiRequest
	{
		[JsonConstructor]
		public AddGameMediaRequest(string[] redirect_uris)
		{
			this.RedirectUris = redirect_uris;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddGameMediaRequest._bodyParameters.Clear();
			AddGameMediaRequest._bodyParameters.Add("redirect_uris", this.RedirectUris);
			return AddGameMediaRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string[] RedirectUris;
	}
}
