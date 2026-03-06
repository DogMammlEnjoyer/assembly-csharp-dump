using System;

namespace Meta.XR.Acoustics
{
	[Flags]
	public enum MeshFlags : uint
	{
		NONE = 0U,
		ENABLE_SIMPLIFICATION = 1U,
		ENABLE_DIFFRACTION = 2U
	}
}
