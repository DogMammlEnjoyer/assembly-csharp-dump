using System;

namespace UnityEngine.Rendering.Universal.Internal
{
	internal enum StencilUsage
	{
		UserMask = 15,
		StencilLight,
		MaterialMask = 96,
		MaterialUnlit = 0,
		MaterialLit = 32,
		MaterialSimpleLit = 64
	}
}
