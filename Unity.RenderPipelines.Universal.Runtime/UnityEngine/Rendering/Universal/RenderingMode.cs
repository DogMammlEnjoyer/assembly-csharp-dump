using System;

namespace UnityEngine.Rendering.Universal
{
	public enum RenderingMode
	{
		Forward,
		[InspectorName("Forward+")]
		ForwardPlus = 2,
		Deferred = 1,
		[InspectorName("Deferred+")]
		DeferredPlus = 3
	}
}
