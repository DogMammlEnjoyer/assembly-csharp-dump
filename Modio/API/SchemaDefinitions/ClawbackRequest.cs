using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ClawbackRequest : IApiRequest
	{
		[JsonConstructor]
		public ClawbackRequest(long transaction_id, long gateway_uuid, string portal, string refund_reason, string clawback_uuid)
		{
			this.TransactionId = transaction_id;
			this.GatewayUuid = gateway_uuid;
			this.Portal = portal;
			this.RefundReason = refund_reason;
			this.ClawbackUuid = clawback_uuid;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			ClawbackRequest._bodyParameters.Clear();
			ClawbackRequest._bodyParameters.Add("transaction_id", this.TransactionId);
			ClawbackRequest._bodyParameters.Add("gateway_uuid", this.GatewayUuid);
			ClawbackRequest._bodyParameters.Add("portal", this.Portal);
			ClawbackRequest._bodyParameters.Add("refund_reason", this.RefundReason);
			ClawbackRequest._bodyParameters.Add("clawback_uuid", this.ClawbackUuid);
			return ClawbackRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long TransactionId;

		internal readonly long GatewayUuid;

		internal readonly string Portal;

		internal readonly string RefundReason;

		internal readonly string ClawbackUuid;
	}
}
