using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	public readonly struct SyncXboxEntitlementsRequest : IApiRequest
	{
		[JsonConstructor]
		public SyncXboxEntitlementsRequest(string xbox_token)
		{
			this.XboxToken = xbox_token;
		}

		public IReadOnlyDictionary<string, object> GetBodyParameters()
		{
			SyncXboxEntitlementsRequest._bodyParameters.Clear();
			SyncXboxEntitlementsRequest._bodyParameters.Add("xbox_token", this.XboxToken);
			return SyncXboxEntitlementsRequest._bodyParameters;
		}

		private static readonly Dictionary<string, object> _bodyParameters = new Dictionary<string, object>();

		public readonly string XboxToken;
	}
}
