using System;
using Meta.WitAi.Json;

namespace Meta.WitAi.TTS.Data
{
	[Serializable]
	public class TTSActionEvent : TTSStringEvent
	{
		public WitResponseNode Response
		{
			get
			{
				if (string.IsNullOrEmpty(base.Data))
				{
					return TTSActionEvent.EMPTY_RESPONSE;
				}
				if (null == this.response)
				{
					this.response = WitResponseNode.Parse(base.Data);
				}
				return this.response;
			}
		}

		private WitResponseNode response;

		public static readonly WitResponseNode EMPTY_RESPONSE = new WitResponseNode();
	}
}
