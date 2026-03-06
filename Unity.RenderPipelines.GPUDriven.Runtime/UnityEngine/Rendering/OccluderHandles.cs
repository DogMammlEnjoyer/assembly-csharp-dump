using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	internal struct OccluderHandles
	{
		public bool IsValid()
		{
			return this.occluderDepthPyramid.IsValid();
		}

		public void UseForOcclusionTest(IBaseRenderGraphBuilder builder)
		{
			builder.UseTexture(this.occluderDepthPyramid, AccessFlags.Read);
			if (this.occlusionDebugOverlay.IsValid())
			{
				builder.UseBuffer(this.occlusionDebugOverlay, AccessFlags.ReadWrite);
			}
		}

		public void UseForOccluderUpdate(IBaseRenderGraphBuilder builder)
		{
			builder.UseTexture(this.occluderDepthPyramid, AccessFlags.ReadWrite);
			if (this.occlusionDebugOverlay.IsValid())
			{
				builder.UseBuffer(this.occlusionDebugOverlay, AccessFlags.ReadWrite);
			}
		}

		public TextureHandle occluderDepthPyramid;

		public BufferHandle occlusionDebugOverlay;
	}
}
