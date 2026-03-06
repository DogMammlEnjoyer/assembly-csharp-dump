using System;

namespace Meta.XR.Acoustics
{
	[Flags]
	public enum AcousticMapFlags : uint
	{
		NONE = 0U,
		STATIC_ONLY = 1U,
		NO_FLOATING = 2U,
		MAP_ONLY = 4U,
		DIFFRACTION = 8U
	}
}
