using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ResolveReportsRequest : IApiRequest
	{
		[JsonConstructor]
		public ResolveReportsRequest(long[] report_ids)
		{
			this.ReportIds = report_ids;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			ResolveReportsRequest._bodyParameters.Clear();
			ResolveReportsRequest._bodyParameters.Add("report_ids", this.ReportIds);
			return ResolveReportsRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long[] ReportIds;
	}
}
