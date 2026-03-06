using System;
using Meta.WitAi.Attributes;
using Meta.WitAi.Json;
using Meta.WitAi.Utilities;
using UnityEngine;

namespace Meta.WitAi.CallbackHandlers
{
	[AddComponentMenu("Wit.ai/Response Matchers/Out Of Domain")]
	public class OutOfScopeUtteranceHandler : WitResponseHandler
	{
		protected override string OnValidateResponse(WitResponseNode response, bool isEarlyResponse)
		{
			if (response == null)
			{
				return "Response is null";
			}
			if (response["intents"].Count <= 0)
			{
				return string.Empty;
			}
			if (response.GetFirstIntent()["confidence"].AsFloat < this.confidenceThreshold)
			{
				return string.Empty;
			}
			return "Intents found";
		}

		protected override void OnResponseInvalid(WitResponseNode response, string error)
		{
		}

		protected override void OnResponseSuccess(WitResponseNode response)
		{
			StringEvent stringEvent = this.onOutOfDomain;
			if (stringEvent == null)
			{
				return;
			}
			stringEvent.Invoke(response.GetTranscription());
		}

		[Tooltip("If set to a value greater than zero, any intent that returns with a confidence lower than this value will be treated as out of domain/scope.")]
		[Range(0f, 1f)]
		[SerializeField]
		private float confidenceThreshold;

		[Space(8f)]
		[TooltipBox("Triggered when a activation on the associated AppVoiceExperience does not return any intents.")]
		[SerializeField]
		private StringEvent onOutOfDomain = new StringEvent();
	}
}
