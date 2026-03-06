using System;

namespace Fusion.Protocol
{
	[Flags]
	internal enum JoinRequests : uint
	{
		None = 0U,
		NetworkConfig = 2U,
		ReflexiveInfo = 4U,
		DisableNATPunch = 8U
	}
}
