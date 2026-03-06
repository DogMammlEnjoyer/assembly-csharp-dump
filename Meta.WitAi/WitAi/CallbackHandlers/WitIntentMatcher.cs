using System;
using Meta.Conduit;
using Meta.WitAi.Data.Intents;
using Meta.WitAi.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.CallbackHandlers
{
	public abstract class WitIntentMatcher : WitResponseHandler
	{
		protected override string OnValidateResponse(WitResponseNode response, bool isEarlyResponse)
		{
			if (response == null)
			{
				return "No response";
			}
			WitIntentData[] intents = response.GetIntents();
			if (intents == null || intents.Length == 0)
			{
				return "No intents found";
			}
			WitIntentData witIntentData = null;
			foreach (WitIntentData witIntentData2 in intents)
			{
				if (string.Equals(this.intent, witIntentData2.name, StringComparison.CurrentCultureIgnoreCase))
				{
					witIntentData = witIntentData2;
					break;
				}
			}
			if (witIntentData == null)
			{
				return "Missing required intent '" + this.intent + "'";
			}
			if (witIntentData.confidence < this.confidenceThreshold)
			{
				return string.Format("Required intent '{0}' confidence too low: {1:0.000}\nRequired: {2:0.000}", this.intent, witIntentData.confidence, this.confidenceThreshold);
			}
			return string.Empty;
		}

		protected override void OnEnable()
		{
			Manifest.WitResponseMatcherIntents.Add(this.intent);
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			Manifest.WitResponseMatcherIntents.Remove(this.intent);
			base.OnDisable();
		}

		[Header("Intent Settings")]
		[SerializeField]
		public string intent;

		[FormerlySerializedAs("confidence")]
		[Range(0f, 1f)]
		[SerializeField]
		public float confidenceThreshold = 0.6f;
	}
}
