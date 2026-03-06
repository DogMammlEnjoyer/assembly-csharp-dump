using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct DeleteModMetadataRequest : IApiRequest
	{
		[JsonConstructor]
		public DeleteModMetadataRequest(string[] metadata)
		{
			this.Metadata = metadata;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			DeleteModMetadataRequest._bodyParameters.Clear();
			DeleteModMetadataRequest._bodyParameters.Add("metadata", this.Metadata);
			return DeleteModMetadataRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string[] Metadata;
	}
}
