using System;
using Oculus.Assistant.VoiceCommand.Configuration;
using Oculus.Assistant.VoiceCommand.Data;
using Oculus.Voice.Core.Utilities;

namespace Oculus.Assistant.VoiceCommand.Listeners
{
	[Serializable]
	public class VoiceCommandResultHandler : VoiceCommandListener
	{
		public void OnCallback(VoiceCommandResult result)
		{
			if (this.voiceCommand.actionId == result.ActionId)
			{
				this.onVoiceCommandReceived.Invoke(result);
				foreach (SlotHandler slotHandler in this.slotHandlers)
				{
					string arg;
					if (result.TryGetSlot(slotHandler.slotName, out arg))
					{
						slotHandler.onCommandSlotReceived.Invoke(arg);
					}
				}
			}
		}

		public VoiceCommand voiceCommand;

		public VoiceCommandCallbackEvent onVoiceCommandReceived = new VoiceCommandCallbackEvent();

		[ArrayElementTitle("slotName", "Unassigned Slot")]
		public SlotHandler[] slotHandlers = Array.Empty<SlotHandler>();
	}
}
