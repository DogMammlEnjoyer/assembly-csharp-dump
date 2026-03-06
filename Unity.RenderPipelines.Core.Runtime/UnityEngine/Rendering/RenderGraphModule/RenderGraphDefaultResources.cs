using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.RenderGraphModule", "UnityEngine.Rendering.RenderGraphModule", null)]
	public class RenderGraphDefaultResources
	{
		public TextureHandle blackTexture { get; private set; }

		public TextureHandle whiteTexture { get; private set; }

		public TextureHandle clearTextureXR { get; private set; }

		public TextureHandle magentaTextureXR { get; private set; }

		public TextureHandle blackTextureXR { get; private set; }

		public TextureHandle blackTextureArrayXR { get; private set; }

		public TextureHandle blackUIntTextureXR { get; private set; }

		public TextureHandle blackTexture3DXR { get; private set; }

		public TextureHandle whiteTextureXR { get; private set; }

		public TextureHandle defaultShadowTexture { get; private set; }

		internal RenderGraphDefaultResources()
		{
			this.InitDefaultResourcesIfNeeded();
		}

		private void InitDefaultResourcesIfNeeded()
		{
			if (this.m_BlackTexture2D == null)
			{
				this.m_BlackTexture2D = RTHandles.Alloc(Texture2D.blackTexture);
			}
			if (this.m_WhiteTexture2D == null)
			{
				this.m_WhiteTexture2D = RTHandles.Alloc(Texture2D.whiteTexture);
			}
			if (this.m_ShadowTexture2D == null)
			{
				this.m_ShadowTexture2D = RTHandles.Alloc(1, 1, CoreUtils.GetDefaultDepthOnlyFormat(), 1, FilterMode.Point, TextureWrapMode.Repeat, TextureDimension.Tex2D, false, false, true, true, 1, 0f, MSAASamples.None, false, false, false, RenderTextureMemoryless.None, VRTextureUsage.None, "DefaultShadowTexture");
				CommandBuffer commandBuffer = CommandBufferPool.Get();
				commandBuffer.SetRenderTarget(this.m_ShadowTexture2D);
				commandBuffer.ClearRenderTarget(RTClearFlags.All, Color.white, 1f, 0U);
				Graphics.ExecuteCommandBuffer(commandBuffer);
				CommandBufferPool.Release(commandBuffer);
			}
		}

		internal void Cleanup()
		{
			RTHandle blackTexture2D = this.m_BlackTexture2D;
			if (blackTexture2D != null)
			{
				blackTexture2D.Release();
			}
			this.m_BlackTexture2D = null;
			RTHandle whiteTexture2D = this.m_WhiteTexture2D;
			if (whiteTexture2D != null)
			{
				whiteTexture2D.Release();
			}
			this.m_WhiteTexture2D = null;
			RTHandle shadowTexture2D = this.m_ShadowTexture2D;
			if (shadowTexture2D != null)
			{
				shadowTexture2D.Release();
			}
			this.m_ShadowTexture2D = null;
		}

		internal void InitializeForRendering(RenderGraph renderGraph)
		{
			this.InitDefaultResourcesIfNeeded();
			this.blackTexture = renderGraph.ImportTexture(this.m_BlackTexture2D, true);
			this.whiteTexture = renderGraph.ImportTexture(this.m_WhiteTexture2D, true);
			this.defaultShadowTexture = renderGraph.ImportTexture(this.m_ShadowTexture2D, true);
			this.clearTextureXR = renderGraph.ImportTexture(TextureXR.GetClearTexture(), true);
			this.magentaTextureXR = renderGraph.ImportTexture(TextureXR.GetMagentaTexture(), true);
			this.blackTextureXR = renderGraph.ImportTexture(TextureXR.GetBlackTexture(), true);
			this.blackTextureArrayXR = renderGraph.ImportTexture(TextureXR.GetBlackTextureArray(), true);
			this.blackUIntTextureXR = renderGraph.ImportTexture(TextureXR.GetBlackUIntTexture(), true);
			this.blackTexture3DXR = renderGraph.ImportTexture(TextureXR.GetBlackTexture3D(), true);
			this.whiteTextureXR = renderGraph.ImportTexture(TextureXR.GetWhiteTexture(), true);
		}

		private RTHandle m_BlackTexture2D;

		private RTHandle m_WhiteTexture2D;

		private RTHandle m_ShadowTexture2D;
	}
}
