using System;

namespace UnityEngine.Rendering.Universal
{
	[Flags]
	[Obsolete("Use RenderingLayerMask instead. #from(6000.2)")]
	public enum LightLayerEnum
	{
		Nothing = 0,
		LightLayerDefault = 1,
		LightLayer1 = 2,
		LightLayer2 = 4,
		LightLayer3 = 8,
		LightLayer4 = 16,
		LightLayer5 = 32,
		LightLayer6 = 64,
		LightLayer7 = 128,
		Everything = 255
	}
}
