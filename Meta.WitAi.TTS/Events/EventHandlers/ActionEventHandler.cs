using System;
using Meta.WitAi.Json;
using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Integrations;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.WitAi.TTS.Events.EventHandlers
{
	public class ActionEventHandler : TTSEventTrigger<TTSActionEvent, string>
	{
		public UnityEvent<WitResponseNode> OnEvent
		{
			get
			{
				return this.onEvent;
			}
		}

		protected override void OnEventTriggered(TTSActionEvent queuedEvent)
		{
			UnityEvent<WitResponseNode> unityEvent = this.onEvent;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(queuedEvent.Response);
		}

		[SerializeField]
		private UnityEvent<WitResponseNode> onEvent = new UnityEvent<WitResponseNode>();
	}
}
