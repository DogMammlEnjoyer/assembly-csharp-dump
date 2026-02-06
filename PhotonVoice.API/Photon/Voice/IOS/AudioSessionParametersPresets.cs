using System;

namespace Photon.Voice.IOS
{
	public static class AudioSessionParametersPresets
	{
		public static AudioSessionParameters Game = new AudioSessionParameters
		{
			Category = AudioSessionCategory.PlayAndRecord,
			Mode = AudioSessionMode.Default,
			CategoryOptions = new AudioSessionCategoryOption[]
			{
				AudioSessionCategoryOption.DefaultToSpeaker,
				AudioSessionCategoryOption.AllowBluetooth
			}
		};

		public static AudioSessionParameters VoIP = new AudioSessionParameters
		{
			Category = AudioSessionCategory.PlayAndRecord,
			Mode = AudioSessionMode.VoiceChat,
			CategoryOptions = new AudioSessionCategoryOption[]
			{
				AudioSessionCategoryOption.AllowBluetooth
			}
		};
	}
}
