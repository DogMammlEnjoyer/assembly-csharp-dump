using System;

namespace Meta.XR.Acoustics
{
	[Flags]
	public enum AcousticMapStatus : uint
	{
		EMPTY = 0U,
		MAPPED = 1U,
		READY = 3U
	}
}
