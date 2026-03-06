using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct SyncPlayStationNetworkEntitlementsRequest : IApiRequest
	{
		[JsonConstructor]
		public SyncPlayStationNetworkEntitlementsRequest(string auth_code, long env, long service_label)
		{
			this.AuthCode = auth_code;
			this.Env = env;
			this.ServiceLabel = service_label;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			SyncPlayStationNetworkEntitlementsRequest._bodyParameters.Clear();
			SyncPlayStationNetworkEntitlementsRequest._bodyParameters.Add("auth_code", this.AuthCode);
			SyncPlayStationNetworkEntitlementsRequest._bodyParameters.Add("env", this.Env);
			SyncPlayStationNetworkEntitlementsRequest._bodyParameters.Add("service_label", this.ServiceLabel);
			return SyncPlayStationNetworkEntitlementsRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		public readonly string AuthCode;

		public readonly long Env;

		public readonly long ServiceLabel;
	}
}
