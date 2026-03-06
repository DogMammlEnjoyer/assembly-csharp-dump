using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct DeleteModDependenciesRequest : IApiRequest
	{
		[JsonConstructor]
		public DeleteModDependenciesRequest(long[] dependencies)
		{
			this.Dependencies = dependencies;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			DeleteModDependenciesRequest._bodyParameters.Clear();
			DeleteModDependenciesRequest._bodyParameters.Add("dependencies", this.Dependencies);
			return DeleteModDependenciesRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long[] Dependencies;
	}
}
