using System;
using Meta.WitAi.Json;
using UnityEngine;

namespace Meta.WitAi.CallbackHandlers
{
	[AddComponentMenu("Wit.ai/Response Matchers/Simple String Entity Handler")]
	public class SimpleStringEntityHandler : WitIntentMatcher
	{
		public StringEntityMatchEvent OnIntentEntityTriggered
		{
			get
			{
				return this.onIntentEntityTriggered;
			}
		}

		protected override string OnValidateResponse(WitResponseNode response, bool isEarlyResponse)
		{
			string text = base.OnValidateResponse(response, isEarlyResponse);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (string.IsNullOrEmpty(response.GetFirstEntityValue(this.entity)))
			{
				return "Missing required entity: " + this.entity;
			}
			return string.Empty;
		}

		protected override void OnResponseInvalid(WitResponseNode response, string error)
		{
		}

		protected override void OnResponseSuccess(WitResponseNode response)
		{
			string firstEntityValue = response.GetFirstEntityValue(this.entity);
			if (!string.IsNullOrEmpty(this.format))
			{
				this.onIntentEntityTriggered.Invoke(this.format.Replace("{value}", firstEntityValue));
				return;
			}
			this.onIntentEntityTriggered.Invoke(firstEntityValue);
		}

		[SerializeField]
		public string entity;

		[SerializeField]
		public string format;

		[SerializeField]
		private StringEntityMatchEvent onIntentEntityTriggered = new StringEntityMatchEvent();
	}
}
