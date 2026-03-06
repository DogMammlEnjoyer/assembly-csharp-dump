using System;
using UnityEngine;

namespace Oculus.Interaction.UnityCanvas
{
	public enum OVRRenderingMode
	{
		[InspectorName("Alpha-Blended")]
		AlphaBlended,
		[InspectorName("Alpha-Cutout")]
		AlphaCutout,
		[InspectorName("Opaque")]
		Opaque,
		[InspectorName("OVR/Overlay")]
		Overlay = 100,
		[InspectorName("OVR/Underlay")]
		Underlay
	}
}
