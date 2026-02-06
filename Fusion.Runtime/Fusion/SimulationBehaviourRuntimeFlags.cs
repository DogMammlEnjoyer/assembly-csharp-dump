using System;

namespace Fusion
{
	[Flags]
	internal enum SimulationBehaviourRuntimeFlags
	{
		IsGlobal = 1,
		InSimulation = 2,
		PendingRemoval = 4,
		IsUnityDestroyed = 8,
		IsUnityDisabled = 16,
		SkipNextUpdate = 32,
		ClearMask = 39
	}
}
