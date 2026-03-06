using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	internal readonly struct MetricsSessionRequest : IApiRequest
	{
		public MetricsSessionRequest(string sessionId, long sessionTs, string sessionHash, string sessionNonce, long sessionOrderId, long[] ids)
		{
			this.SessionId = sessionId;
			this.SessionTs = sessionTs;
			this.SessionHash = sessionHash;
			this.SessionNonce = sessionNonce;
			this.SessionOrderId = sessionOrderId;
			this.Ids = ids;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			MetricsSessionRequest._bodyParameters.Clear();
			MetricsSessionRequest._bodyParameters.Add("session_id", this.SessionId);
			MetricsSessionRequest._bodyParameters.Add("session_ts", this.SessionTs);
			MetricsSessionRequest._bodyParameters.Add("session_hash", this.SessionHash);
			MetricsSessionRequest._bodyParameters.Add("session_nonce", this.SessionNonce);
			MetricsSessionRequest._bodyParameters.Add("session_order_id", this.SessionOrderId);
			if (this.Ids != null)
			{
				MetricsSessionRequest._bodyParameters.Add("ids", this.Ids);
			}
			return MetricsSessionRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly string SessionId;

		internal readonly long SessionTs;

		internal readonly string SessionHash;

		internal readonly string SessionNonce;

		internal readonly long SessionOrderId;

		internal readonly long[] Ids;
	}
}
