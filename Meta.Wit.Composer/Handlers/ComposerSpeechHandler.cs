using System;
using Meta.WitAi.Composer.Data;
using Meta.WitAi.Composer.Interfaces;
using Meta.WitAi.TTS.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.Composer.Handlers
{
	public class ComposerSpeechHandler : MonoBehaviour, IComposerSpeechHandler
	{
		public void SpeakPhrase(ComposerSessionData sessionData)
		{
			string responsePhrase = sessionData.responseData.responsePhrase;
			TTSSpeaker speaker = this.GetSpeaker(sessionData);
			if (speaker == null)
			{
				VLog.E(string.Format("Composer Speech Handler - No Speaker Found\nPhrase: {0}\nPartial: {1}", responsePhrase, sessionData.responseData.responseIsFinal), null);
				return;
			}
			if (sessionData.responseData.responseTtsSettings != null)
			{
				speaker.SetVoiceOverride(sessionData.responseData.responseTtsSettings);
			}
			if (this.queuePhrases)
			{
				speaker.SpeakQueued(responsePhrase);
				return;
			}
			speaker.Speak(responsePhrase);
		}

		public bool IsSpeaking(ComposerSessionData sessionData)
		{
			TTSSpeaker speaker = this.GetSpeaker(sessionData);
			return speaker != null && (speaker.IsLoading || speaker.IsSpeaking);
		}

		private TTSSpeaker GetSpeaker(ComposerSessionData sessionData)
		{
			if (this.Speakers == null || this.Speakers.Length == 0)
			{
				return null;
			}
			int num = 0;
			string speakerName = this.GetSpeakerName(sessionData.contextMap);
			if (!string.IsNullOrEmpty(speakerName))
			{
				num = Array.FindIndex<ComposerSpeakerData>(this.Speakers, (ComposerSpeakerData s) => string.Equals(s.SpeakerName, speakerName, StringComparison.CurrentCultureIgnoreCase));
				if (num == -1)
				{
					return null;
				}
			}
			return this.Speakers[num].Speaker;
		}

		public string GetSpeakerName(ComposerContextMap contextMap)
		{
			if (contextMap != null && !(contextMap.Data == null))
			{
				return contextMap.Data[this.SpeakerNameContextMapKey].Value;
			}
			return null;
		}

		[Tooltip("If true, queues tts phrases and plays them back in order.  If false, stops previous phrase to play new phrases")]
		public bool queuePhrases = true;

		[SerializeField]
		[FormerlySerializedAs("speakerNameContextMapKey")]
		public string SpeakerNameContextMapKey = "wit_composer_speaker";

		[SerializeField]
		[FormerlySerializedAs("_speakers")]
		public ComposerSpeakerData[] Speakers;
	}
}
