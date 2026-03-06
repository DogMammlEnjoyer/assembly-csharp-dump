using System;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	public struct LightData
	{
		internal LightData(ContextContainer frameData)
		{
			this.frameData = frameData;
		}

		internal UniversalLightData universalLightData
		{
			get
			{
				return this.frameData.Get<UniversalLightData>();
			}
		}

		public ref int mainLightIndex
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().mainLightIndex;
			}
		}

		public ref int additionalLightsCount
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().additionalLightsCount;
			}
		}

		public ref int maxPerObjectAdditionalLightsCount
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().maxPerObjectAdditionalLightsCount;
			}
		}

		public ref NativeArray<VisibleLight> visibleLights
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().visibleLights;
			}
		}

		public ref bool shadeAdditionalLightsPerVertex
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().shadeAdditionalLightsPerVertex;
			}
		}

		public ref bool supportsMixedLighting
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().supportsMixedLighting;
			}
		}

		public ref bool reflectionProbeBoxProjection
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().reflectionProbeBoxProjection;
			}
		}

		public ref bool reflectionProbeBlending
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().reflectionProbeBlending;
			}
		}

		public ref bool reflectionProbeAtlas
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().reflectionProbeAtlas;
			}
		}

		public ref bool supportsLightLayers
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().supportsLightLayers;
			}
		}

		public ref bool supportsAdditionalLights
		{
			get
			{
				return ref this.frameData.Get<UniversalLightData>().supportsAdditionalLights;
			}
		}

		private ContextContainer frameData;
	}
}
