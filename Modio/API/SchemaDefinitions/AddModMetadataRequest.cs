using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddModMetadataRequest : IApiRequest
	{
		[JsonConstructor]
		public AddModMetadataRequest(string[] metadata)
		{
			this.Metadata = metadata;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddModMetadataRequest._bodyParameters.Clear();
			AddModMetadataRequest._bodyParameters.Add("metadata", this.Metadata);
			return AddModMetadataRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string[] Metadata;
	}
}
