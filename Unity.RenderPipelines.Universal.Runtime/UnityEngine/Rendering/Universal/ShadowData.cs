using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	public struct ShadowData
	{
		internal ShadowData(ContextContainer frameData)
		{
			this.frameData = frameData;
		}

		internal UniversalShadowData universalShadowData
		{
			get
			{
				return this.frameData.Get<UniversalShadowData>();
			}
		}

		public ref bool supportsMainLightShadows
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().supportsMainLightShadows;
			}
		}

		internal ref bool mainLightShadowsEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().mainLightShadowsEnabled;
			}
		}

		public ref int mainLightShadowmapWidth
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().mainLightShadowmapWidth;
			}
		}

		public ref int mainLightShadowmapHeight
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().mainLightShadowmapHeight;
			}
		}

		public ref int mainLightShadowCascadesCount
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().mainLightShadowCascadesCount;
			}
		}

		public ref Vector3 mainLightShadowCascadesSplit
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().mainLightShadowCascadesSplit;
			}
		}

		public ref float mainLightShadowCascadeBorder
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().mainLightShadowCascadeBorder;
			}
		}

		public ref bool supportsAdditionalLightShadows
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().supportsAdditionalLightShadows;
			}
		}

		internal ref bool additionalLightShadowsEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().additionalLightShadowsEnabled;
			}
		}

		public ref int additionalLightsShadowmapWidth
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().additionalLightsShadowmapWidth;
			}
		}

		public ref int additionalLightsShadowmapHeight
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().additionalLightsShadowmapHeight;
			}
		}

		public ref bool supportsSoftShadows
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().supportsSoftShadows;
			}
		}

		public ref int shadowmapDepthBufferBits
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().shadowmapDepthBufferBits;
			}
		}

		public ref List<Vector4> bias
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().bias;
			}
		}

		public ref List<int> resolution
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().resolution;
			}
		}

		internal ref bool isKeywordAdditionalLightShadowsEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().isKeywordAdditionalLightShadowsEnabled;
			}
		}

		internal ref bool isKeywordSoftShadowsEnabled
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().isKeywordSoftShadowsEnabled;
			}
		}

		internal ref int mainLightShadowResolution
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().mainLightShadowResolution;
			}
		}

		internal ref int mainLightRenderTargetWidth
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().mainLightRenderTargetWidth;
			}
		}

		internal ref int mainLightRenderTargetHeight
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().mainLightRenderTargetHeight;
			}
		}

		internal ref NativeArray<URPLightShadowCullingInfos> visibleLightsShadowCullingInfos
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().visibleLightsShadowCullingInfos;
			}
		}

		internal ref AdditionalLightsShadowAtlasLayout shadowAtlasLayout
		{
			get
			{
				return ref this.frameData.Get<UniversalShadowData>().shadowAtlasLayout;
			}
		}

		private ContextContainer frameData;
	}
}
