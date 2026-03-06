using System;
using Meta.WitAi.Json;

namespace Meta.WitAi.TTS.Data
{
	[Serializable]
	public class TTSEvent<TData> : ITTSEvent
	{
		public string EventType
		{
			get
			{
				return this.type;
			}
		}

		public int SampleOffset
		{
			get
			{
				return this.offset;
			}
		}

		public TData Data
		{
			get
			{
				return this.data;
			}
		}

		[JsonProperty]
		internal string type;

		[JsonProperty]
		internal int offset;

		[JsonProperty]
		internal TData data;
	}
}
