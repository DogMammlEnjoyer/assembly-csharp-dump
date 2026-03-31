using System;

namespace Cysharp.Threading.Tasks
{
	[Flags]
	public enum InjectPlayerLoopTimings
	{
		All = 65535,
		Standard = 30037,
		Minimum = 8464,
		Initialization = 1,
		LastInitialization = 2,
		EarlyUpdate = 4,
		LastEarlyUpdate = 8,
		FixedUpdate = 16,
		LastFixedUpdate = 32,
		PreUpdate = 64,
		LastPreUpdate = 128,
		Update = 256,
		LastUpdate = 512,
		PreLateUpdate = 1024,
		LastPreLateUpdate = 2048,
		PostLateUpdate = 4096,
		LastPostLateUpdate = 8192,
		TimeUpdate = 16384,
		LastTimeUpdate = 32768
	}
}
