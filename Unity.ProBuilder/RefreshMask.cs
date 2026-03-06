using System;

namespace UnityEngine.ProBuilder
{
	[Flags]
	public enum RefreshMask
	{
		UV = 1,
		Colors = 2,
		Normals = 4,
		Tangents = 8,
		Collisions = 16,
		Bounds = 22,
		All = 31
	}
}
