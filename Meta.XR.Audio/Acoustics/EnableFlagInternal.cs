using System;

namespace Meta.XR.Acoustics
{
	[Flags]
	public enum EnableFlagInternal : uint
	{
		NONE = 0U,
		SIMPLE_ROOM_MODELING = 2U,
		LATE_REVERBERATION = 3U,
		RANDOMIZE_REVERB = 4U,
		PERFORMANCE_COUNTERS = 5U,
		DIFFRACTION = 6U
	}
}
