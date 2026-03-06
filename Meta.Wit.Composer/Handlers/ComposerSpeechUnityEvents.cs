using System;
using Meta.WitAi.Attributes;
using Meta.WitAi.Composer.Data;
using Meta.WitAi.Composer.Interfaces;
using Meta.WitAi.Events;
using Meta.WitAi.Json;
using Meta.WitAi.Utilities;
using UnityEngine;

namespace Meta.WitAi.Composer.Handlers
{
	public class ComposerSpeechUnityEvents : MonoBehaviour, IComposerSpeechHandler
	{
		public void SpeakPhrase(ComposerSessionData sessionData)
		{
			ComposerResponseData responseData = sessionData.responseData;
			string responsePhrase = responseData.responsePhrase;
			if (!responseData.responseIsFinal)
			{
				ComposerResponseDataEvent composerResponseDataEvent = this.onPartialComposerResponse;
				if (composerResponseDataEvent != null)
				{
					composerResponseDataEvent.Invoke(responseData);
				}
				WitObjectEvent witObjectEvent = this.onPartialTextResponse;
				if (witObjectEvent != null)
				{
					witObjectEvent.Invoke(this.HandleResponse(sessionData, responsePhrase));
				}
				this.onPartialText.Invoke(responsePhrase);
				return;
			}
			ComposerResponseDataEvent composerResponseDataEvent2 = this.onFullComposerResponse;
			if (composerResponseDataEvent2 != null)
			{
				composerResponseDataEvent2.Invoke(responseData);
			}
			WitObjectEvent witObjectEvent2 = this.onFullTextResponse;
			if (witObjectEvent2 != null)
			{
				witObjectEvent2.Invoke(this.HandleResponse(sessionData, responsePhrase));
			}
			this.onFullText.Invoke(responsePhrase);
		}

		private WitResponseClass HandleResponse(ComposerSessionData sessionData, string text)
		{
			WitResponseNode witResponse = sessionData.responseData.witResponse;
			WitResponseClass witResponseClass = (witResponse != null) ? witResponse.AsObject : null;
			if (null == witResponseClass)
			{
				witResponseClass = new WitResponseClass();
				witResponseClass["q"] = text;
			}
			return witResponseClass;
		}

		public bool IsSpeaking(ComposerSessionData sessionData)
		{
			return false;
		}

		[TooltipBox("Events for receipt of partial transcriptions")]
		[SerializeField]
		private StringEvent onPartialText;

		[SerializeField]
		private WitObjectEvent onPartialTextResponse;

		[SerializeField]
		private ComposerResponseDataEvent onPartialComposerResponse;

		[TooltipBox("Events for receipt of full transcriptions")]
		[SerializeField]
		private StringEvent onFullText;

		[SerializeField]
		private WitObjectEvent onFullTextResponse;

		[SerializeField]
		private ComposerResponseDataEvent onFullComposerResponse;
	}
}
