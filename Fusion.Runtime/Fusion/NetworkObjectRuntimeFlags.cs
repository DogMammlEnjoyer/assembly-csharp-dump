using System;

namespace Fusion
{
	[Flags]
	internal enum NetworkObjectRuntimeFlags
	{
		None = 0,
		HadAwake = 1,
		IsDestroyed = 2,
		IsNested = 4,
		NotAwakeWhenAttaching = 8192,
		ClearMask = 268369920,
		InSimulation = 65536,
		PreexistingObject = 131072,
		AttachOptionLocalSpawn = 1048576,
		Spawned = 8388608,
		OwnsNestedObjects = 16777216,
		HasMainNetworkTRSP = 67108864
	}
}
