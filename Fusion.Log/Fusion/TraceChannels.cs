using System;

namespace Fusion
{
	[Flags]
	public enum TraceChannels
	{
		Global = 1,
		Stun = 2,
		Object = 4,
		Network = 8,
		Prefab = 16,
		SceneInfo = 32,
		SceneManager = 64,
		SimulationMessage = 128,
		HostMigration = 256,
		Encryption = 512,
		DummyTraffic = 1024,
		Realtime = 2048,
		MemoryTrack = 4096,
		Snapshots = 8192,
		Time = 16384
	}
}
