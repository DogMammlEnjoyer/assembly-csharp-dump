using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct S2SCommitRequest : IApiRequest
	{
		[JsonConstructor]
		public S2SCommitRequest(long transaction_id, string clawback_uuid)
		{
			this.TransactionId = transaction_id;
			this.ClawbackUuid = clawback_uuid;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			S2SCommitRequest._bodyParameters.Clear();
			S2SCommitRequest._bodyParameters.Add("transaction_id", this.TransactionId);
			S2SCommitRequest._bodyParameters.Add("clawback_uuid", this.ClawbackUuid);
			return S2SCommitRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		internal readonly long TransactionId;

		internal readonly string ClawbackUuid;
	}
}
