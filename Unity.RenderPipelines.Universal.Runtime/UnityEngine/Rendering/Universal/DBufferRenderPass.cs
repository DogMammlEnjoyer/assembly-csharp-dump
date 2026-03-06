using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
	internal class DBufferRenderPass : ScriptableRenderPass
	{
		internal RTHandle[] dBufferColorHandles { get; private set; }

		internal RTHandle depthHandle { get; private set; }

		internal RTHandle dBufferDepth
		{
			get
			{
				return this.m_DBufferDepth;
			}
		}

		public DBufferRenderPass(Material dBufferClear, DBufferSettings settings, DecalDrawDBufferSystem drawSystem, bool decalLayers)
		{
			base.renderPassEvent = (RenderPassEvent)201;
			ScriptableRenderPassInput passInput = ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal;
			base.ConfigureInput(passInput);
			base.requiresIntermediateTexture = true;
			this.m_DrawSystem = drawSystem;
			this.m_Settings = settings;
			this.m_DBufferClear = dBufferClear;
			base.profilingSampler = new ProfilingSampler("Draw DBuffer");
			this.m_DBufferClearSampler = new ProfilingSampler("Clear");
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(RenderQueueRange.opaque), -1, uint.MaxValue, 0);
			this.m_DecalLayers = decalLayers;
			this.m_ShaderTagIdList = new List<ShaderTagId>();
			this.m_ShaderTagIdList.Add(new ShaderTagId("DBufferMesh"));
			this.m_ShaderTagIdList.Add(new ShaderTagId("DBufferProjectorVFX"));
			int num = (int)(settings.surfaceData + 1);
			this.dBufferColorHandles = new RTHandle[num];
			this.m_PassData = new DBufferRenderPass.PassData();
		}

		public void Dispose()
		{
			RTHandle dbufferDepth = this.m_DBufferDepth;
			if (dbufferDepth != null)
			{
				dbufferDepth.Release();
			}
			foreach (RTHandle rthandle in this.dBufferColorHandles)
			{
				if (rthandle != null)
				{
					rthandle.Release();
				}
			}
		}

		public unsafe void Setup(in CameraData cameraData)
		{
			CameraData cameraData2 = cameraData;
			RenderTextureDescriptor renderTextureDescriptor = *cameraData2.cameraTargetDescriptor;
			renderTextureDescriptor.graphicsFormat = GraphicsFormat.None;
			cameraData2 = cameraData;
			renderTextureDescriptor.depthStencilFormat = cameraData2.cameraTargetDescriptor.depthStencilFormat;
			renderTextureDescriptor.msaaSamples = 1;
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.m_DBufferDepth, renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, DBufferRenderPass.s_DBufferDepthName);
			this.Setup(cameraData, this.m_DBufferDepth);
		}

		public unsafe void Setup(in CameraData cameraData, RTHandle depthTextureHandle)
		{
			CameraData cameraData2 = cameraData;
			RenderTextureDescriptor renderTextureDescriptor = *cameraData2.cameraTargetDescriptor;
			renderTextureDescriptor.graphicsFormat = ((QualitySettings.activeColorSpace == ColorSpace.Linear) ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm);
			renderTextureDescriptor.depthStencilFormat = GraphicsFormat.None;
			renderTextureDescriptor.msaaSamples = 1;
			RenderingUtils.ReAllocateHandleIfNeeded(ref this.dBufferColorHandles[0], renderTextureDescriptor, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, DBufferRenderPass.s_DBufferNames[0]);
			if (this.m_Settings.surfaceData == DecalSurfaceData.AlbedoNormal || this.m_Settings.surfaceData == DecalSurfaceData.AlbedoNormalMAOS)
			{
				cameraData2 = cameraData;
				RenderTextureDescriptor renderTextureDescriptor2 = *cameraData2.cameraTargetDescriptor;
				renderTextureDescriptor2.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
				renderTextureDescriptor2.depthStencilFormat = GraphicsFormat.None;
				renderTextureDescriptor2.msaaSamples = 1;
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.dBufferColorHandles[1], renderTextureDescriptor2, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, DBufferRenderPass.s_DBufferNames[1]);
			}
			if (this.m_Settings.surfaceData == DecalSurfaceData.AlbedoNormalMAOS)
			{
				cameraData2 = cameraData;
				RenderTextureDescriptor renderTextureDescriptor3 = *cameraData2.cameraTargetDescriptor;
				renderTextureDescriptor3.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
				renderTextureDescriptor3.depthStencilFormat = GraphicsFormat.None;
				renderTextureDescriptor3.msaaSamples = 1;
				RenderingUtils.ReAllocateHandleIfNeeded(ref this.dBufferColorHandles[2], renderTextureDescriptor3, FilterMode.Point, TextureWrapMode.Repeat, 1, 0f, DBufferRenderPass.s_DBufferNames[2]);
			}
			this.depthHandle = depthTextureHandle;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
		{
			base.ConfigureTarget(this.dBufferColorHandles, this.depthHandle);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			this.InitPassData(ref this.m_PassData);
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			DBufferRenderPass.PassData passData = this.m_PassData;
			using (new ProfilingScope(commandBuffer, base.profilingSampler))
			{
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
				DBufferRenderPass.SetGlobalTextures(*renderingData.commandBuffer, this.m_PassData);
				DBufferRenderPass.SetKeywords(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData);
				using (new ProfilingScope(commandBuffer, this.m_DBufferClearSampler))
				{
					Blitter.BlitTexture(commandBuffer, passData.dBufferColorHandles[0], new Vector4(1f, 1f, 0f, 0f), this.m_DBufferClear, 0);
				}
				UniversalRenderingData renderingData2 = renderingData.frameData.Get<UniversalRenderingData>();
				UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
				UniversalLightData lightData = renderingData.frameData.Get<UniversalLightData>();
				RendererListParams rendererListParams = this.InitRendererListParams(renderingData2, cameraData, lightData);
				RendererList rendererList = context.CreateRendererList(ref rendererListParams);
				DBufferRenderPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData, rendererList, false);
			}
		}

		private static void ExecutePass(RasterCommandBuffer cmd, DBufferRenderPass.PassData passData, RendererList rendererList, bool renderGraph)
		{
			passData.drawSystem.Execute(cmd);
			cmd.DrawRendererList(rendererList);
		}

		private static void SetGlobalTextures(CommandBuffer cmd, DBufferRenderPass.PassData passData)
		{
			RTHandle[] dBufferColorHandles = passData.dBufferColorHandles;
			cmd.SetGlobalTexture(dBufferColorHandles[0].name, dBufferColorHandles[0].nameID);
			if (passData.settings.surfaceData == DecalSurfaceData.AlbedoNormal || passData.settings.surfaceData == DecalSurfaceData.AlbedoNormalMAOS)
			{
				cmd.SetGlobalTexture(dBufferColorHandles[1].name, dBufferColorHandles[1].nameID);
			}
			if (passData.settings.surfaceData == DecalSurfaceData.AlbedoNormalMAOS)
			{
				cmd.SetGlobalTexture(dBufferColorHandles[2].name, dBufferColorHandles[2].nameID);
			}
		}

		private static void SetKeywords(RasterCommandBuffer cmd, DBufferRenderPass.PassData passData)
		{
			cmd.SetKeyword(ShaderGlobalKeywords.DBufferMRT1, passData.settings.surfaceData == DecalSurfaceData.Albedo);
			cmd.SetKeyword(ShaderGlobalKeywords.DBufferMRT2, passData.settings.surfaceData == DecalSurfaceData.AlbedoNormal);
			cmd.SetKeyword(ShaderGlobalKeywords.DBufferMRT3, passData.settings.surfaceData == DecalSurfaceData.AlbedoNormalMAOS);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalLayers, passData.decalLayers);
		}

		private void InitPassData(ref DBufferRenderPass.PassData passData)
		{
			passData.drawSystem = this.m_DrawSystem;
			passData.settings = this.m_Settings;
			passData.decalLayers = this.m_DecalLayers;
			passData.dBufferDepth = this.m_DBufferDepth;
			passData.dBufferColorHandles = this.dBufferColorHandles;
		}

		private RendererListParams InitRendererListParams(UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData)
		{
			SortingCriteria defaultOpaqueSortFlags = cameraData.defaultOpaqueSortFlags;
			DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(this.m_ShaderTagIdList, renderingData, cameraData, lightData, defaultOpaqueSortFlags);
			return new RendererListParams(renderingData.cullResults, drawSettings, this.m_FilteringSettings);
		}

		public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
		{
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			TextureHandle cameraDepthTexture = universalResourceData.cameraDepthTexture;
			TextureHandle cameraNormalsTexture = universalResourceData.cameraNormalsTexture;
			TextureHandle tex = universalResourceData.dBufferDepth.IsValid() ? universalResourceData.dBufferDepth : universalResourceData.activeDepthTexture;
			TextureHandle renderingLayersTexture = universalResourceData.renderingLayersTexture;
			DBufferRenderPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DBufferRenderPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Decal\\DBuffer\\DBufferRenderPass.cs", 233))
			{
				this.InitPassData(ref passData);
				if (this.dbufferHandles == null)
				{
					this.dbufferHandles = new TextureHandle[3];
				}
				RenderTextureDescriptor cameraTargetDescriptor = universalCameraData.cameraTargetDescriptor;
				cameraTargetDescriptor.graphicsFormat = ((QualitySettings.activeColorSpace == ColorSpace.Linear) ? GraphicsFormat.R8G8B8A8_SRGB : GraphicsFormat.R8G8B8A8_UNorm);
				cameraTargetDescriptor.depthStencilFormat = GraphicsFormat.None;
				cameraTargetDescriptor.msaaSamples = 1;
				this.dbufferHandles[0] = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, DBufferRenderPass.s_DBufferNames[0], true, new Color(0f, 0f, 0f, 1f), FilterMode.Point, TextureWrapMode.Clamp, false);
				rasterRenderGraphBuilder.SetRenderAttachment(this.dbufferHandles[0], 0, AccessFlags.Write);
				if (this.m_Settings.surfaceData == DecalSurfaceData.AlbedoNormal || this.m_Settings.surfaceData == DecalSurfaceData.AlbedoNormalMAOS)
				{
					RenderTextureDescriptor cameraTargetDescriptor2 = universalCameraData.cameraTargetDescriptor;
					cameraTargetDescriptor2.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
					cameraTargetDescriptor2.depthStencilFormat = GraphicsFormat.None;
					cameraTargetDescriptor2.msaaSamples = 1;
					this.dbufferHandles[1] = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor2, DBufferRenderPass.s_DBufferNames[1], true, new Color(0.5f, 0.5f, 0.5f, 1f), FilterMode.Point, TextureWrapMode.Clamp, false);
					rasterRenderGraphBuilder.SetRenderAttachment(this.dbufferHandles[1], 1, AccessFlags.Write);
				}
				if (this.m_Settings.surfaceData == DecalSurfaceData.AlbedoNormalMAOS)
				{
					RenderTextureDescriptor cameraTargetDescriptor3 = universalCameraData.cameraTargetDescriptor;
					cameraTargetDescriptor3.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
					cameraTargetDescriptor3.depthStencilFormat = GraphicsFormat.None;
					cameraTargetDescriptor3.msaaSamples = 1;
					this.dbufferHandles[2] = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor3, DBufferRenderPass.s_DBufferNames[2], true, new Color(0f, 0f, 0f, 1f), FilterMode.Point, TextureWrapMode.Clamp, false);
					rasterRenderGraphBuilder.SetRenderAttachment(this.dbufferHandles[2], 2, AccessFlags.Write);
				}
				rasterRenderGraphBuilder.SetRenderAttachmentDepth(tex, AccessFlags.Read);
				if (cameraDepthTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(cameraDepthTexture, AccessFlags.Read);
				}
				if (cameraNormalsTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(cameraNormalsTexture, AccessFlags.Read);
				}
				if (passData.decalLayers && renderingLayersTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(renderingLayersTexture, AccessFlags.Read);
				}
				if (universalResourceData.ssaoTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseGlobalTexture(DBufferRenderPass.s_SSAOTextureID, AccessFlags.Read);
				}
				RendererListParams rendererListParams = this.InitRendererListParams(renderingData, universalCameraData, lightData);
				passData.rendererList = renderGraph.CreateRendererList(rendererListParams);
				rasterRenderGraphBuilder.UseRendererList(passData.rendererList);
				for (int i = 0; i < 3; i++)
				{
					if (this.dbufferHandles[i].IsValid())
					{
						rasterRenderGraphBuilder.SetGlobalTextureAfterPass(this.dbufferHandles[i], Shader.PropertyToID(DBufferRenderPass.s_DBufferNames[i]));
					}
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<DBufferRenderPass.PassData>(delegate(DBufferRenderPass.PassData data, RasterGraphContext rgContext)
				{
					DBufferRenderPass.SetKeywords(rgContext.cmd, data);
					DBufferRenderPass.ExecutePass(rgContext.cmd, data, data.rendererList, true);
				});
			}
			universalResourceData.dBuffer = this.dbufferHandles;
		}

		public override void OnCameraCleanup(CommandBuffer cmd)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("cmd");
			}
			cmd.SetKeyword(ShaderGlobalKeywords.DBufferMRT1, false);
			cmd.SetKeyword(ShaderGlobalKeywords.DBufferMRT2, false);
			cmd.SetKeyword(ShaderGlobalKeywords.DBufferMRT3, false);
			cmd.SetKeyword(ShaderGlobalKeywords.DecalLayers, false);
		}

		internal static string[] s_DBufferNames = new string[]
		{
			"_DBufferTexture0",
			"_DBufferTexture1",
			"_DBufferTexture2",
			"_DBufferTexture3"
		};

		internal static string s_DBufferDepthName = "DBufferDepth";

		private static readonly int s_SSAOTextureID = Shader.PropertyToID("_ScreenSpaceOcclusionTexture");

		private DecalDrawDBufferSystem m_DrawSystem;

		private DBufferSettings m_Settings;

		private Material m_DBufferClear;

		private FilteringSettings m_FilteringSettings;

		private List<ShaderTagId> m_ShaderTagIdList;

		private ProfilingSampler m_DBufferClearSampler;

		private bool m_DecalLayers;

		private RTHandle m_DBufferDepth;

		private DBufferRenderPass.PassData m_PassData;

		private TextureHandle[] dbufferHandles;

		private class PassData
		{
			internal DecalDrawDBufferSystem drawSystem;

			internal DBufferSettings settings;

			internal bool decalLayers;

			internal RTHandle dBufferDepth;

			internal RTHandle[] dBufferColorHandles;

			internal RendererListHandle rendererList;
		}
	}
}
