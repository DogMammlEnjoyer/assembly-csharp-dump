using System;

namespace Fusion.Photon.Realtime
{
	public enum CustomAuthenticationType : byte
	{
		Custom,
		Steam,
		Facebook,
		Oculus,
		PlayStation4,
		[Obsolete("Use PlayStation4 or PlayStation5 as needed")]
		PlayStation = 4,
		Xbox,
		Viveport = 10,
		NintendoSwitch,
		PlayStation5,
		[Obsolete("Use PlayStation4 or PlayStation5 as needed")]
		Playstation5 = 12,
		Epic,
		FacebookGaming = 15,
		None = 255
	}
}
