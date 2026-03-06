using System;

namespace Fusion.Protocol
{
	[Flags]
	internal enum StartRequests : uint
	{
		None = 0U,
		ConnectToShared = 2U,
		WaitForReflexiveInfo = 4U
	}
}
