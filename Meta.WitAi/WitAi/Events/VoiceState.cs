using System;

namespace Meta.WitAi.Events
{
	[Flags]
	public enum VoiceState
	{
		MicOff = 1,
		MicOn = 2,
		Listening = 4,
		StartProcessing = 8,
		Response = 16,
		Error = 32
	}
}
