using System;
using Meta.WitAi.Data.Intents;
using Meta.WitAi.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.CallbackHandlers
{
	[AddComponentMenu("Wit.ai/Response Matchers/Simple Intent Handler")]
	public class SimpleIntentHandler : WitIntentMatcher
	{
		public UnityEvent OnIntentTriggered
		{
			get
			{
				return this.onIntentTriggered;
			}
		}

		protected override void OnResponseSuccess(WitResponseNode response)
		{
			this.onIntentTriggered.Invoke();
			this.UpdateRanges(response);
		}

		protected override void OnResponseInvalid(WitResponseNode response, string error)
		{
			this.UpdateRanges(response);
		}

		private void UpdateRanges(WitResponseNode response)
		{
			WitIntentData[] array = (response != null) ? response.GetIntents() : null;
			if (array == null)
			{
				return;
			}
			foreach (WitIntentData witIntentData in array)
			{
				if (string.Equals(this.intent, witIntentData.name, StringComparison.CurrentCultureIgnoreCase))
				{
					WitResponseHandler.RefreshConfidenceRange(witIntentData.confidence, this.confidenceRanges, this.allowConfidenceOverlap);
					return;
				}
			}
			WitResponseHandler.RefreshConfidenceRange(0f, this.confidenceRanges, this.allowConfidenceOverlap);
		}

		[SerializeField]
		private UnityEvent onIntentTriggered = new UnityEvent();

		[Tooltip("Confidence ranges are executed in order. If checked, all confidence values will be checked instead of stopping on the first one that matches.")]
		[SerializeField]
		public bool allowConfidenceOverlap;

		[SerializeField]
		public ConfidenceRange[] confidenceRanges;
	}
}
