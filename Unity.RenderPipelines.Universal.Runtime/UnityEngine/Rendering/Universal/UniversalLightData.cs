using System;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	public class UniversalLightData : ContextItem
	{
		public override void Reset()
		{
			this.mainLightIndex = -1;
			this.additionalLightsCount = 0;
			this.maxPerObjectAdditionalLightsCount = 0;
			this.visibleLights = default(NativeArray<VisibleLight>);
			this.shadeAdditionalLightsPerVertex = false;
			this.supportsMixedLighting = false;
			this.reflectionProbeBoxProjection = false;
			this.reflectionProbeBlending = false;
			this.supportsLightLayers = false;
			this.supportsAdditionalLights = false;
		}

		public int mainLightIndex;

		public int additionalLightsCount;

		public int maxPerObjectAdditionalLightsCount;

		public NativeArray<VisibleLight> visibleLights;

		public bool shadeAdditionalLightsPerVertex;

		public bool supportsMixedLighting;

		public bool reflectionProbeBoxProjection;

		public bool reflectionProbeBlending;

		public bool reflectionProbeAtlas;

		public bool supportsLightLayers;

		public bool supportsAdditionalLights;
	}
}
