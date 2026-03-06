using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meta.WitAi.Requests
{
	internal class WitMessageVRequest : WitVRequest
	{
		public WitMessageVRequest(IWitRequestConfiguration configuration, string requestId, string operationId) : base(configuration, requestId, operationId, false)
		{
		}

		public Task<VRequestResponse<string>> MessageRequest(string endpoint, bool post, string text, Dictionary<string, string> urlParameters, Action<string> onPartial = null)
		{
			if (urlParameters == null)
			{
				urlParameters = new Dictionary<string, string>();
			}
			if (!post)
			{
				urlParameters["q"] = text;
				return base.RequestWitGet<string>(endpoint, urlParameters, onPartial);
			}
			return base.RequestWitPost<string>(endpoint, urlParameters, text, onPartial);
		}
	}
}
