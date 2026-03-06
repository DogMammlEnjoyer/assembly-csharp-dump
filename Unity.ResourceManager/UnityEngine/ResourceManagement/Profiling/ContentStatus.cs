using System;

namespace UnityEngine.ResourceManagement.Profiling
{
	[Flags]
	internal enum ContentStatus
	{
		None = 0,
		Queue = 2,
		Downloading = 4,
		Released = 16,
		Loading = 64,
		Active = 256
	}
}
