using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddModSubscriptionRequest : IApiRequest
	{
		[JsonConstructor]
		public AddModSubscriptionRequest(bool include_dependencies)
		{
			this.IncludeDependencies = include_dependencies;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddModSubscriptionRequest._bodyParameters.Clear();
			AddModSubscriptionRequest._bodyParameters.Add("include_dependencies", this.IncludeDependencies);
			return AddModSubscriptionRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly bool IncludeDependencies;
	}
}
