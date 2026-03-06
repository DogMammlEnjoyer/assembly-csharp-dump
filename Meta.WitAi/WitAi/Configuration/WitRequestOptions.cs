using System;
using System.Collections.Generic;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using UnityEngine;

namespace Meta.WitAi.Configuration
{
	public class WitRequestOptions : VoiceServiceRequestOptions
	{
		[Obsolete("Use 'RequestId' property instead")]
		[JsonIgnore]
		public string requestID
		{
			get
			{
				return base.RequestId;
			}
		}

		public WitRequestOptions(params VoiceServiceRequestOptions.QueryParam[] newParams) : base(newParams)
		{
		}

		public WitRequestOptions(string newRequestId, string newClientUserId, string newOperationId, params VoiceServiceRequestOptions.QueryParam[] newParams) : base(newRequestId, newClientUserId, newOperationId, newParams)
		{
		}

		public string ToJsonString()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["nBestIntents"] = this.nBestIntents.ToString();
			dictionary["requestID"] = base.RequestId;
			if (!string.IsNullOrEmpty(base.OperationId))
			{
				dictionary["operationId"] = base.OperationId;
				WitRequestOptions.OpIdRegistry[base.RequestId] = base.OperationId;
			}
			foreach (string key in base.QueryParams.Keys)
			{
				dictionary[key] = base.QueryParams[key];
			}
			return JsonConvert.SerializeObject<Dictionary<string, string>>(dictionary, null, false);
		}

		public static Dictionary<string, string> OpIdRegistry { get; set; } = new Dictionary<string, string>();

		public IDynamicEntitiesProvider dynamicEntities;

		public int nBestIntents = -1;

		[Obsolete("Use WitConfiguration.editorVersionTag or WitConfiguration.buildVersionTag")]
		[SerializeField]
		[HideInInspector]
		public string tag;

		public Action<WitRequest> onResponse;
	}
}
