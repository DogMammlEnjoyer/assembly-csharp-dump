using System;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Integrations;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.TTS.Events.EventHandlers
{
	public class EmoteEventHandler : TTSEventTrigger<TTSEmoteEvent, string>
	{
		public UnityEvent<string> OnEmoteStart
		{
			get
			{
				return this.onEmoteStart;
			}
		}

		public UnityEvent<string> OnEmoteStop
		{
			get
			{
				return this.onEmoteStop;
			}
		}

		protected override void OnEventTriggered(TTSEmoteEvent queuedEvent)
		{
			if (this._lastEmote != null)
			{
				UnityEvent<string> unityEvent = this.onEmoteStop;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this._lastEmote.Data);
				}
			}
			this._lastEmote = queuedEvent;
			UnityEvent<string> unityEvent2 = this.onEmoteStart;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(queuedEvent.Data);
		}

		[SerializeField]
		private UnityEvent<string> onEmoteStart = new UnityEvent<string>();

		[SerializeField]
		private UnityEvent<string> onEmoteStop = new UnityEvent<string>();

		private TTSEmoteEvent _lastEmote;
	}
}
