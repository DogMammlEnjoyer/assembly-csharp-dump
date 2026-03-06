using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AddModDependenciesRequest : IApiRequest
	{
		[JsonConstructor]
		public AddModDependenciesRequest(long[] dependencies, bool sync)
		{
			this.Dependencies = dependencies;
			this.Sync = sync;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AddModDependenciesRequest._bodyParameters.Clear();
			AddModDependenciesRequest._bodyParameters.Add("dependencies", this.Dependencies);
			AddModDependenciesRequest._bodyParameters.Add("sync", this.Sync);
			return AddModDependenciesRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long[] Dependencies;

		internal readonly bool Sync;
	}
}
