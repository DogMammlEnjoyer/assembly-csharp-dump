using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AdminEditModStatusRequest : IApiRequest
	{
		[JsonConstructor]
		public AdminEditModStatusRequest(long status, string reason)
		{
			this.Status = status;
			this.Reason = reason;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			AdminEditModStatusRequest._bodyParameters.Clear();
			AdminEditModStatusRequest._bodyParameters.Add("status", this.Status);
			AdminEditModStatusRequest._bodyParameters.Add("reason", this.Reason);
			return AdminEditModStatusRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long Status;

		internal readonly string Reason;
	}
}
