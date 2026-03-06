using System;
using UnityEngine;

namespace Oculus.Assistant.VoiceCommand.Listeners
{
	[Serializable]
	public class SlotHandler
	{
		public override string ToString()
		{
			return this.slotName;
		}

		[Tooltip("The name of the slot to listen for")]
		public string slotName;

		public OnCommandSlotReceived onCommandSlotReceived = new OnCommandSlotReceived();
	}
}
