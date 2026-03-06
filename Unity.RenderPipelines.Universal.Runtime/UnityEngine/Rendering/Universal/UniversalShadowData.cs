using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	public class UniversalShadowData : ContextItem
	{
		public override void Reset()
		{
			this.supportsMainLightShadows = false;
			this.mainLightShadowmapWidth = 0;
			this.mainLightShadowmapHeight = 0;
			this.mainLightShadowCascadesCount = 0;
			this.mainLightShadowCascadesSplit = Vector3.zero;
			this.mainLightShadowCascadeBorder = 0f;
			this.supportsAdditionalLightShadows = false;
			this.additionalLightsShadowmapWidth = 0;
			this.additionalLightsShadowmapHeight = 0;
			this.supportsSoftShadows = false;
			this.shadowmapDepthBufferBits = 0;
			List<Vector4> list = this.bias;
			if (list != null)
			{
				list.Clear();
			}
			List<int> list2 = this.resolution;
			if (list2 != null)
			{
				list2.Clear();
			}
			this.isKeywordAdditionalLightShadowsEnabled = false;
			this.isKeywordSoftShadowsEnabled = false;
			this.mainLightShadowResolution = 0;
			this.mainLightRenderTargetWidth = 0;
			this.mainLightRenderTargetHeight = 0;
			this.visibleLightsShadowCullingInfos = default(NativeArray<URPLightShadowCullingInfos>);
			this.shadowAtlasLayout = default(AdditionalLightsShadowAtlasLayout);
		}

		public bool supportsMainLightShadows;

		internal bool mainLightShadowsEnabled;

		public int mainLightShadowmapWidth;

		public int mainLightShadowmapHeight;

		public int mainLightShadowCascadesCount;

		public Vector3 mainLightShadowCascadesSplit;

		public float mainLightShadowCascadeBorder;

		public bool supportsAdditionalLightShadows;

		internal bool additionalLightShadowsEnabled;

		public int additionalLightsShadowmapWidth;

		public int additionalLightsShadowmapHeight;

		public bool supportsSoftShadows;

		public int shadowmapDepthBufferBits;

		public List<Vector4> bias;

		public List<int> resolution;

		internal bool isKeywordAdditionalLightShadowsEnabled;

		internal bool isKeywordSoftShadowsEnabled;

		internal int mainLightShadowResolution;

		internal int mainLightRenderTargetWidth;

		internal int mainLightRenderTargetHeight;

		internal NativeArray<URPLightShadowCullingInfos> visibleLightsShadowCullingInfos;

		internal AdditionalLightsShadowAtlasLayout shadowAtlasLayout;
	}
}
