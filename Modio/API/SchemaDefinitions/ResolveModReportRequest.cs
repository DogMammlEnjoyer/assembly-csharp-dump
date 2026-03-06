using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ResolveModReportRequest : IApiRequest
	{
		[JsonConstructor]
		public ResolveModReportRequest(long type)
		{
			this.Type = type;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			ResolveModReportRequest._bodyParameters.Clear();
			ResolveModReportRequest._bodyParameters.Add("type", this.Type);
			return ResolveModReportRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long Type;
	}
}
