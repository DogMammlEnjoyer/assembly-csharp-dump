using System;
using System.Collections.Generic;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class DrawObjectsPass : ScriptableRenderPass
	{
		public DrawObjectsPass(string profilerTag, ShaderTagId[] shaderTagIds, bool opaque, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference)
		{
			this.Init(opaque, evt, renderQueueRange, layerMask, stencilState, stencilReference, shaderTagIds);
			base.profilingSampler = new ProfilingSampler(profilerTag);
		}

		public DrawObjectsPass(string profilerTag, bool opaque, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference) : this(profilerTag, null, opaque, evt, renderQueueRange, layerMask, stencilState, stencilReference)
		{
		}

		internal DrawObjectsPass(URPProfileId profileId, bool opaque, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference)
		{
			this.Init(opaque, evt, renderQueueRange, layerMask, stencilState, stencilReference, null);
			base.profilingSampler = ProfilingSampler.Get<URPProfileId>(profileId);
		}

		internal void Init(bool opaque, RenderPassEvent evt, RenderQueueRange renderQueueRange, LayerMask layerMask, StencilState stencilState, int stencilReference, ShaderTagId[] shaderTagIds = null)
		{
			if (shaderTagIds == null)
			{
				shaderTagIds = new ShaderTagId[]
				{
					new ShaderTagId("SRPDefaultUnlit"),
					new ShaderTagId("UniversalForward"),
					new ShaderTagId("UniversalForwardOnly")
				};
			}
			this.m_PassData = new DrawObjectsPass.PassData();
			foreach (ShaderTagId item in shaderTagIds)
			{
				this.m_ShaderTagIdList.Add(item);
			}
			base.renderPassEvent = evt;
			this.m_FilteringSettings = new FilteringSettings(new RenderQueueRange?(renderQueueRange), layerMask, uint.MaxValue, 0);
			this.m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
			this.m_IsOpaque = opaque;
			this.m_ShouldTransparentsReceiveShadows = false;
			this.m_IsActiveTargetBackBuffer = false;
			if (stencilState.enabled)
			{
				this.m_RenderStateBlock.stencilReference = stencilReference;
				this.m_RenderStateBlock.mask = RenderStateMask.Stencil;
				this.m_RenderStateBlock.stencilState = stencilState;
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData renderingData2 = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			this.InitPassData(cameraData, ref this.m_PassData, uint.MaxValue, this.m_IsActiveTargetBackBuffer);
			this.InitRendererLists(renderingData2, cameraData, lightData, ref this.m_PassData, context, null, false);
			using (new ProfilingScope(*renderingData.commandBuffer, base.profilingSampler))
			{
				DrawObjectsPass.ExecutePass(CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer), this.m_PassData, this.m_PassData.rendererList, this.m_PassData.objectsWithErrorRendererList, this.m_PassData.cameraData.IsCameraProjectionMatrixFlipped());
			}
		}

		internal static void ExecutePass(RasterCommandBuffer cmd, DrawObjectsPass.PassData data, RendererList rendererList, RendererList objectsWithErrorRendererList, bool yFlip)
		{
			Vector4 value = new Vector4(0f, 0f, 0f, data.isOpaque ? 1f : 0f);
			cmd.SetGlobalVector(DrawObjectsPass.s_DrawObjectPassDataPropID, value);
			if (data.cameraData.xr.enabled && data.isActiveTargetBackBuffer)
			{
				cmd.SetViewport(data.cameraData.xr.GetViewport(0));
			}
			float num = yFlip ? -1f : 1f;
			Vector4 value2 = (num < 0f) ? new Vector4(num, 1f, -1f, 1f) : new Vector4(num, 0f, 1f, 1f);
			cmd.SetGlobalVector(ShaderPropertyId.scaleBiasRt, value2);
			float value3 = (data.cameraData.cameraTargetDescriptor.msaaSamples > 1 && data.isOpaque) ? 1f : 0f;
			cmd.SetGlobalFloat(ShaderPropertyId.alphaToMaskAvailable, value3);
			if (ScriptableRenderPass.GetActiveDebugHandler(data.cameraData) != null)
			{
				data.debugRendererLists.DrawWithRendererList(cmd);
				return;
			}
			cmd.DrawRendererList(rendererList);
		}

		internal void InitPassData(UniversalCameraData cameraData, ref DrawObjectsPass.PassData passData, uint batchLayerMask, bool isActiveTargetBackBuffer = false)
		{
			passData.cameraData = cameraData;
			passData.isOpaque = this.m_IsOpaque;
			passData.shouldTransparentsReceiveShadows = this.m_ShouldTransparentsReceiveShadows;
			passData.batchLayerMask = batchLayerMask;
			passData.isActiveTargetBackBuffer = isActiveTargetBackBuffer;
		}

		internal void InitRendererLists(UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData, ref DrawObjectsPass.PassData passData, ScriptableRenderContext context, RenderGraph renderGraph, bool useRenderGraph)
		{
			Camera camera = cameraData.camera;
			SortingCriteria sortingCriteria = this.m_IsOpaque ? cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
			if (cameraData.renderer.useDepthPriming && this.m_IsOpaque && (cameraData.renderType == CameraRenderType.Base || cameraData.clearDepth))
			{
				sortingCriteria = (SortingCriteria.SortingLayer | SortingCriteria.RenderQueue | SortingCriteria.OptimizeStateChanges | SortingCriteria.CanvasOrder);
			}
			FilteringSettings filteringSettings = this.m_FilteringSettings;
			filteringSettings.batchLayerMask = passData.batchLayerMask;
			DrawingSettings ds = RenderingUtils.CreateDrawingSettings(this.m_ShaderTagIdList, renderingData, cameraData, lightData, sortingCriteria);
			if (cameraData.renderer.useDepthPriming && this.m_IsOpaque && (cameraData.renderType == CameraRenderType.Base || cameraData.clearDepth))
			{
				this.m_RenderStateBlock.depthState = new DepthState(false, CompareFunction.Equal);
				this.m_RenderStateBlock.mask = (this.m_RenderStateBlock.mask | RenderStateMask.Depth);
			}
			else if (this.m_RenderStateBlock.depthState.compareFunction == CompareFunction.Equal)
			{
				this.m_RenderStateBlock.depthState = new DepthState(true, CompareFunction.LessEqual);
				this.m_RenderStateBlock.mask = (this.m_RenderStateBlock.mask | RenderStateMask.Depth);
			}
			DebugHandler activeDebugHandler = ScriptableRenderPass.GetActiveDebugHandler(cameraData);
			if (useRenderGraph)
			{
				if (activeDebugHandler != null)
				{
					passData.debugRendererLists = activeDebugHandler.CreateRendererListsWithDebugRenderState(renderGraph, ref renderingData.cullResults, ref ds, ref filteringSettings, ref this.m_RenderStateBlock);
					return;
				}
				RenderingUtils.CreateRendererListWithRenderStateBlock(renderGraph, ref renderingData.cullResults, ds, filteringSettings, this.m_RenderStateBlock, ref passData.rendererListHdl);
				return;
			}
			else
			{
				if (activeDebugHandler != null)
				{
					passData.debugRendererLists = activeDebugHandler.CreateRendererListsWithDebugRenderState(context, ref renderingData.cullResults, ref ds, ref filteringSettings, ref this.m_RenderStateBlock);
					return;
				}
				RenderingUtils.CreateRendererListWithRenderStateBlock(context, ref renderingData.cullResults, ds, filteringSettings, this.m_RenderStateBlock, ref passData.rendererList);
				return;
			}
		}

		internal void Render(RenderGraph renderGraph, ContextContainer frameData, TextureHandle colorTarget, TextureHandle depthTarget, TextureHandle mainShadowsTexture, TextureHandle additionalShadowsTexture, uint batchLayerMask = 4294967295U)
		{
			UniversalResourceData universalResourceData = frameData.Get<UniversalResourceData>();
			UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			DrawObjectsPass.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<DrawObjectsPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\DrawObjectsPass.cs", 264))
			{
				rasterRenderGraphBuilder.UseAllGlobalTextures(true);
				this.InitPassData(universalCameraData, ref passData, batchLayerMask, universalResourceData.isActiveTargetBackBuffer);
				if (colorTarget.IsValid())
				{
					passData.albedoHdl = colorTarget;
					rasterRenderGraphBuilder.SetRenderAttachment(colorTarget, 0, AccessFlags.Write);
				}
				if (depthTarget.IsValid())
				{
					passData.depthHdl = depthTarget;
					rasterRenderGraphBuilder.SetRenderAttachmentDepth(depthTarget, AccessFlags.Write);
				}
				if (mainShadowsTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(mainShadowsTexture, AccessFlags.Read);
				}
				if (additionalShadowsTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(additionalShadowsTexture, AccessFlags.Read);
				}
				TextureHandle ssaoTexture = universalResourceData.ssaoTexture;
				if (ssaoTexture.IsValid())
				{
					rasterRenderGraphBuilder.UseTexture(ssaoTexture, AccessFlags.Read);
				}
				RenderGraphUtils.UseDBufferIfValid(rasterRenderGraphBuilder, universalResourceData);
				this.InitRendererLists(renderingData, universalCameraData, lightData, ref passData, default(ScriptableRenderContext), renderGraph, true);
				if (ScriptableRenderPass.GetActiveDebugHandler(universalCameraData) != null)
				{
					passData.debugRendererLists.PrepareRendererListForRasterPass(rasterRenderGraphBuilder);
				}
				else
				{
					rasterRenderGraphBuilder.UseRendererList(passData.rendererListHdl);
					rasterRenderGraphBuilder.UseRendererList(passData.objectsWithErrorRendererListHdl);
				}
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				if (universalCameraData.xr.enabled)
				{
					bool flag = universalCameraData.xrUniversal.canFoveateIntermediatePasses || universalResourceData.isActiveTargetBackBuffer;
					rasterRenderGraphBuilder.EnableFoveatedRasterization(universalCameraData.xr.supportsFoveatedRendering && flag);
				}
				rasterRenderGraphBuilder.SetRenderFunc<DrawObjectsPass.PassData>(delegate(DrawObjectsPass.PassData data, RasterGraphContext context)
				{
					if (!data.isOpaque && !data.shouldTransparentsReceiveShadows)
					{
						TransparentSettingsPass.ExecutePass(context.cmd);
					}
					bool yFlip = data.cameraData.IsRenderTargetProjectionMatrixFlipped(data.albedoHdl, data.depthHdl);
					DrawObjectsPass.ExecutePass(context.cmd, data, data.rendererListHdl, data.objectsWithErrorRendererListHdl, yFlip);
				});
			}
		}

		private FilteringSettings m_FilteringSettings;

		private RenderStateBlock m_RenderStateBlock;

		private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

		private bool m_IsOpaque;

		public bool m_IsActiveTargetBackBuffer;

		public bool m_ShouldTransparentsReceiveShadows;

		private DrawObjectsPass.PassData m_PassData;

		private static readonly int s_DrawObjectPassDataPropID = Shader.PropertyToID("_DrawObjectPassData");

		internal class PassData
		{
			internal TextureHandle albedoHdl;

			internal TextureHandle depthHdl;

			internal UniversalCameraData cameraData;

			internal bool isOpaque;

			internal bool shouldTransparentsReceiveShadows;

			internal uint batchLayerMask;

			internal bool isActiveTargetBackBuffer;

			internal RendererListHandle rendererListHdl;

			internal RendererListHandle objectsWithErrorRendererListHdl;

			internal DebugRendererLists debugRendererLists;

			internal RendererList rendererList;

			internal RendererList objectsWithErrorRendererList;
		}
	}
}
