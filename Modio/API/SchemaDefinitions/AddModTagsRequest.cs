using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddModTagsRequest : IApiRequest
	{
		[JsonConstructor]
		public AddModTagsRequest(string[] tags)
		{
			this.Tags = tags;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddModTagsRequest._bodyParameters.Clear();
			AddModTagsRequest._bodyParameters.Add("tags", this.Tags);
			return AddModTagsRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string[] Tags;
	}
}
