using System;
using Meta.WitAi.Composer.Data;
using Meta.WitAi.Json;
using Meta.WitAi.TTS.Integrations;

namespace Meta.WitAi.Composer.Integrations
{
	public static class WitComposerResponseExtensions
	{
		public static ComposerResponseData GetComposerResponse(this WitResponseNode response)
		{
			return new ComposerResponseData
			{
				witResponse = response,
				error = response.GetError(),
				expectsInput = response.GetExpectsInput(),
				actionID = response.GetActionId(),
				responseIsFinal = response.GetIsResponseFinal(),
				responsePhrase = response.GetResponseText(),
				responseTts = response.GetTTS(),
				responseTtsSettings = response.GetTTSSettings(),
				requestId = response.GetRequestId()
			};
		}

		public static WitResponseNode GetContextMap(this WitResponseNode response)
		{
			return ((response != null) ? response["context_map"].AsObject : null) ?? null;
		}

		public static bool GetExpectsInput(this WitResponseNode response)
		{
			return response != null && response["expects_input"].AsBool;
		}

		public static string GetActionId(this WitResponseNode response)
		{
			return ((response != null) ? response["action"].Value : null) ?? string.Empty;
		}

		public static bool GetIsResponseFinal(this WitResponseNode response)
		{
			return ((response != null) ? response.GetFinalResponse() : null) != null;
		}

		public static string GetResponseText(this WitResponseNode response)
		{
			string text;
			if (response == null)
			{
				text = null;
			}
			else
			{
				WitResponseClass response2 = response.GetResponse();
				if (response2 == null)
				{
					text = null;
				}
				else
				{
					WitResponseNode witResponseNode = response2.SafeGet("text");
					text = ((witResponseNode != null) ? witResponseNode.Value : null);
				}
			}
			return text ?? string.Empty;
		}

		public static WitResponseClass GetSpeech(this WitResponseNode response)
		{
			WitResponseClass witResponseClass;
			if (response == null)
			{
				witResponseClass = null;
			}
			else
			{
				WitResponseClass response2 = response.GetResponse();
				if (response2 == null)
				{
					witResponseClass = null;
				}
				else
				{
					WitResponseNode witResponseNode = response2.SafeGet("speech");
					witResponseClass = ((witResponseNode != null) ? witResponseNode.AsObject : null);
				}
			}
			return witResponseClass ?? null;
		}

		public static string GetTTS(this WitResponseNode response)
		{
			string text;
			if (response == null)
			{
				text = null;
			}
			else
			{
				WitResponseClass speech = response.GetSpeech();
				if (speech == null)
				{
					text = null;
				}
				else
				{
					WitResponseNode witResponseNode = speech.SafeGet("q");
					text = ((witResponseNode != null) ? witResponseNode.Value : null);
				}
			}
			return text ?? response.GetResponseText();
		}

		public static TTSWitVoiceSettings GetTTSSettings(this WitResponseNode response)
		{
			WitResponseClass witResponseClass = (response != null) ? response.GetSpeech() : null;
			if (witResponseClass == null)
			{
				return null;
			}
			TTSWitVoiceSettings ttswitVoiceSettings = new TTSWitVoiceSettings();
			ttswitVoiceSettings.DeserializeObject(witResponseClass);
			return ttswitVoiceSettings;
		}
	}
}
