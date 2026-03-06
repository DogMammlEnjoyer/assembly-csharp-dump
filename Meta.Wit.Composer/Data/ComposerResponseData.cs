using System;
using Meta.WitAi.Json;
using Meta.WitAi.TTS.Data;

namespace Meta.WitAi.Composer.Data
{
	[Serializable]
	public class ComposerResponseData
	{
		public ComposerResponseData()
		{
		}

		public ComposerResponseData(string newError)
		{
			this.error = newError;
		}

		public bool expectsInput;

		public string actionID;

		public bool responseIsFinal;

		public string responsePhrase;

		public string responseTts;

		public TTSVoiceSettings responseTtsSettings;

		public string requestId;

		public string error;

		[NonSerialized]
		public WitResponseNode witResponse;
	}
}
