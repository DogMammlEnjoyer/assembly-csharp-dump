using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct DeleteModTagsRequest : IApiRequest
	{
		[JsonConstructor]
		public DeleteModTagsRequest(string[] tags)
		{
			this.Tags = tags;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			DeleteModTagsRequest._bodyParameters.Clear();
			DeleteModTagsRequest._bodyParameters.Add("tags", this.Tags);
			return DeleteModTagsRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string[] Tags;
	}
}
