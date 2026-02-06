using System;

namespace Fusion
{
	[Flags]
	public enum NetworkObjectHeaderFlags
	{
		GlobalObjectInterest = 1,
		DestroyWhenStateAuthorityLeaves = 2,
		SpawnedByClient = 4,
		AllowStateAuthorityOverride = 16,
		Struct = 32,
		StructArray = 128,
		DontDestroyOnLoad = 64,
		HasMainNetworkTRSP = 8,
		AreaOfInterest = 256
	}
}
