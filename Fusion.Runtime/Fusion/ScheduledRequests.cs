using System;

namespace Fusion
{
	[Flags]
	internal enum ScheduledRequests : uint
	{
		None = 0U,
		ReflexiveInfo = 2U
	}
}
