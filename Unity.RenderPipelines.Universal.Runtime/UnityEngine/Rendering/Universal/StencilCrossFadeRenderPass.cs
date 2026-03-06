using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal sealed class StencilCrossFadeRenderPass
	{
		internal StencilCrossFadeRenderPass(Shader shader)
		{
			this.m_StencilDitherMaskSeedMaterials = new Material[3];
			this.m_ProfilingSampler = new ProfilingSampler("StencilDitherMaskSeed");
			int[] array = new int[]
			{
				4,
				8,
				12
			};
			int num = 12;
			for (int i = 0; i < this.m_StencilDitherMaskSeedMaterials.Length; i++)
			{
				this.m_StencilDitherMaskSeedMaterials[i] = CoreUtils.CreateEngineMaterial(shader);
				this.m_StencilDitherMaskSeedMaterials[i].SetInteger(this._StencilDitherPattern, i + 1);
				this.m_StencilDitherMaskSeedMaterials[i].SetFloat(this._StencilWriteDitherMask, (float)num);
				this.m_StencilDitherMaskSeedMaterials[i].SetFloat(this._StencilRefDitherMask, (float)array[i]);
			}
		}

		public void Dispose()
		{
			Material[] stencilDitherMaskSeedMaterials = this.m_StencilDitherMaskSeedMaterials;
			for (int i = 0; i < stencilDitherMaskSeedMaterials.Length; i++)
			{
				CoreUtils.Destroy(stencilDitherMaskSeedMaterials[i]);
			}
			this.m_StencilDitherMaskSeedMaterials = null;
		}

		public void Render(RenderGraph renderGraph, ScriptableRenderContext context, TextureHandle depthTarget)
		{
			StencilCrossFadeRenderPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<StencilCrossFadeRenderPass.PassData>("Prepare Cross Fade Stencil", out passData, this.m_ProfilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\StencilCrossFadeRenderPass.cs", 61))
			{
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(depthTarget, AccessFlags.Write);
				passData.stencilDitherMaskSeedMaterials = this.m_StencilDitherMaskSeedMaterials;
				passData.depthTarget = depthTarget;
				rasterRenderGraphBuilder.SetRenderFunc<StencilCrossFadeRenderPass.PassData>(delegate(StencilCrossFadeRenderPass.PassData data, RasterGraphContext context)
				{
					StencilCrossFadeRenderPass.ExecutePass(context.cmd, data.depthTarget, data.stencilDitherMaskSeedMaterials);
				});
			}
		}

		private static void ExecutePass(RasterCommandBuffer cmd, RTHandle depthTarget, Material[] stencilDitherMaskSeedMaterials)
		{
			Vector2Int scaledSize = depthTarget.GetScaledSize(depthTarget.rtHandleProperties.currentViewportSize);
			Rect viewport = new Rect(0f, 0f, (float)scaledSize.x, (float)scaledSize.y);
			cmd.SetViewport(viewport);
			for (int i = 0; i < stencilDitherMaskSeedMaterials.Length; i++)
			{
				cmd.DrawProcedural(Matrix4x4.identity, stencilDitherMaskSeedMaterials[i], 0, MeshTopology.Triangles, 3, 1);
			}
		}

		private Material[] m_StencilDitherMaskSeedMaterials;

		private readonly int _StencilDitherPattern = Shader.PropertyToID("_StencilDitherPattern");

		private readonly int _StencilRefDitherMask = Shader.PropertyToID("_StencilRefDitherMask");

		private readonly int _StencilWriteDitherMask = Shader.PropertyToID("_StencilWriteDitherMask");

		private readonly ProfilingSampler m_ProfilingSampler;

		private class PassData
		{
			public TextureHandle depthTarget;

			public Material[] stencilDitherMaskSeedMaterials;
		}
	}
}
